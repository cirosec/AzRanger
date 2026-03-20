using AzRanger.Utilities;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Logger = NLog.Logger;

namespace AzRanger.AzScanner
{
    public enum AuthFlow
    {
        Interactive,
        UsernamePassword,
        DeviceCode
    }

    public class UserAuthenticator : IAuthenticator
    {
        public bool IsUserContext => true;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string Authority = "https://login.microsoftonline.com";
        IPublicClientApplication App;
        private String ClientId;
        private String Username;
        private SecureString Password;
        private readonly AuthFlow authFlow;
        private readonly bool disableCache;
        private int FailedInteractiveLogonCounter = 0;
        private bool userCanceled = false;
        private bool cacheRegistered = false;
        public const string CacheFilePrefix = "azranger";
        public readonly static string CacheDir = MsalCacheHelper.UserRootDirectory;
        // https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/#:~:text=The%20lock%20keyword%20can%20only,is%20used%20pretty%20much%20everywhere.
        readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private UserAuthenticator(AuthFlow flow, string tenantId, IMsalHttpClientFactory httpClientFactory, string clientID, string redirectUrl, string username, string password, bool disableCache)
        {
            this.ClientId = clientID;
            this.authFlow = flow;
            this.disableCache = disableCache;

            if (username != null && password != null)
            {
                this.Username = username;
                this.Password = new SecureString();
                foreach (char c in password)
                {
                    this.Password.AppendChar(c);
                }
            }

            PublicClientApplicationBuilder builder = PublicClientApplicationBuilder.Create(this.ClientId);
            if (tenantId != null)
            {
                this.Authority = Authority + "/" + tenantId + "/";
                builder.WithTenantId(tenantId);
            }
            if (httpClientFactory != null)
            {
                builder.WithHttpClientFactory(httpClientFactory);
            }
            if (redirectUrl != null)
            {
                builder.WithRedirectUri(redirectUrl);
            }
            App = builder.Build();
        }

        public static UserAuthenticator CreateInteractive(string tenantId, IMsalHttpClientFactory httpClientFactory, string clientID, string redirectUrl = null, bool disableCache = false)
        {
            return new UserAuthenticator(AuthFlow.Interactive, tenantId, httpClientFactory, clientID, redirectUrl, null, null, disableCache);
        }

        public static UserAuthenticator CreateDeviceCode(string tenantId, IMsalHttpClientFactory httpClientFactory, string clientID, bool disableCache = false)
        {
            return new UserAuthenticator(AuthFlow.DeviceCode, tenantId, httpClientFactory, clientID, null, null, null, disableCache);
        }

        public static UserAuthenticator CreateWithPassword(string username, string password, string tenantId, IMsalHttpClientFactory httpClientFactory, string clientID, string redirectUrl = null, bool disableCache = false)
        {
            return new UserAuthenticator(AuthFlow.UsernamePassword, tenantId, httpClientFactory, clientID, redirectUrl, username, password, disableCache);
        }

        public static void ClearCache()
        {
            try
            {
                string[] cacheFiles = Directory.GetFiles(CacheDir, CacheFilePrefix + "_*.cache");
                foreach (string cachePath in cacheFiles)
                {
                    File.Delete(cachePath);
                    logger.Info("UserAuthenticator: Deleted persistent token cache at {0}", cachePath);
                }
                // Also clean up legacy single-file cache
                string legacyPath = Path.Combine(CacheDir, CacheFilePrefix + ".cache");
                if (File.Exists(legacyPath))
                {
                    File.Delete(legacyPath);
                    logger.Info("UserAuthenticator: Deleted legacy token cache at {0}", legacyPath);
                }
            }
            catch (Exception ex)
            {
                logger.Warn("UserAuthenticator: Failed to clear cache: {0}", ex.Message);
            }
        }

        public async Task<String> GetUserId()
        {
            String[] scope = new string[] { "offline_access" };
            AuthenticationResult result = await GetAuthenticationResult(scope);
            if (result == null || result.UniqueId == null)
            {
                return null;
            }
            return result.UniqueId;
        }

        public async Task<String> GetAccessToken(String[] scopes)
        {
            AuthenticationResult authenticationResult = await GetAuthenticationResult(scopes);
            if (authenticationResult == null)
            {
                return null;
            }
            return authenticationResult.AccessToken;
        }

        public async Task<String> GetTenantId()
        {
            String[] scope = new string[] { "offline_access" };
            AuthenticationResult result = await GetAuthenticationResult(scope);
            if (result == null) return null;
            return result.TenantId;
        }

        public async Task<String> GetUsername()
        {
            String[] scope = new string[] { "offline_access" };
            AuthenticationResult result = await GetAuthenticationResult(scope);
            if (result == null || result.Account == null) return null;
            return result.Account.Username;
        }

        private string GetCacheFileName()
        {
            return CacheFilePrefix + "_" + ClientId + ".cache";
        }

        private async Task RegisterCacheAsync()
        {
            if (disableCache || cacheRegistered) return;
            try
            {
                string cacheFileName = GetCacheFileName();
                var storageProperties = new StorageCreationPropertiesBuilder(cacheFileName, CacheDir)
                    .Build();
                var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
                cacheHelper.RegisterCache(App.UserTokenCache);
                cacheRegistered = true;
                logger.Debug("UserAuthenticator: Token cache registered at {0}", Path.Combine(CacheDir, cacheFileName));
            }
            catch (Exception ex)
            {
                logger.Warn("UserAuthenticator: Failed to register persistent token cache: {0}", ex.Message);
                // Continue without persistent cache — in-memory cache still works
            }
        }

        private async Task<AuthenticationResult> GetAuthenticationResult(String[] scopes)
        {
            if (userCanceled)
            {
                return null;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                await RegisterCacheAsync();
                var accounts = await App.GetAccountsAsync();
                AuthenticationResult result;

                switch (authFlow)
                {
                    case AuthFlow.UsernamePassword:
                        if (accounts.Any())
                        {
                            try
                            {
                                result = await App.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
                                return result;
                            }
                            catch (MsalUiRequiredException ex)
                            {
                                // Can happen if we require UI because of MFA
                                logger.Warn(ex.ErrorCode);
                                logger.Warn(ex.Message);

                                if (FailedInteractiveLogonCounter > 4)
                                {
                                    return null;
                                }

                                try
                                {
                                    result = await App.AcquireTokenInteractive(scopes).WithUseEmbeddedWebView(true).ExecuteAsync();
                                    return result;
                                }
                                catch (MsalServiceException ex2)
                                {
                                    logger.Warn(ex2.ErrorCode);
                                    logger.Warn(ex2.Message);
                                    FailedInteractiveLogonCounter++;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                result = await App.AcquireTokenByUsernamePassword(scopes, Username, Password).ExecuteAsync();
                                return result;
                            }
                            catch (MsalException ex)
                            {
                                logger.Warn(ex.ErrorCode);
                                logger.Warn(ex.Message);
                                return null;
                            }
                        }

                    case AuthFlow.Interactive:
                    case AuthFlow.DeviceCode:
                        try
                        {
                            result = await App.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
                            return result;
                        }
                        catch (MsalClientException ex) when (ex.ErrorCode == MsalError.MultipleTokensMatchedError)
                        {
                            logger.Warn("UserAuthenticator: Multiple matching tokens detected in cache for ClientId {0}. " +
                                        "Clearing stale entries and falling back to interactive login. " +
                                        "Run with --nocache to prevent this on future runs.", ClientId);
                            foreach (var account in accounts)
                            {
                                await App.RemoveAsync(account);
                            }
                        }
                        catch (MsalUiRequiredException)
                        {
                            // No cached token — falls through to interactive/device-code login below.
                        }

                        try
                        {
                            if (authFlow == AuthFlow.DeviceCode)
                            {
                                result = await App.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
                                {
                                    Console.WriteLine(deviceCodeResult.Message);
                                    return Task.CompletedTask;
                                }).ExecuteAsync();
                            }
                            else
                            {
                                if (FailedInteractiveLogonCounter > 4)
                                {
                                    return null;
                                }
                                result = await App.AcquireTokenInteractive(scopes).ExecuteAsync();
                            }
                            return result;
                        }
                        catch (MsalClientException ex)
                        {
                            if (ex.ErrorCode.Equals("authentication_canceled"))
                            {
                                userCanceled = true;
                            }
                            return null;
                        }
                        catch (MsalServiceException ex)
                        {
                            logger.Warn(ex.ErrorCode);
                            logger.Warn(ex.Message);
                            FailedInteractiveLogonCounter++;
                            return null;
                        }

                    default:
                        logger.Warn("UserAuthenticator: Unknown auth flow: {0}", authFlow);
                        return null;
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
