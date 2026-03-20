using AzRanger.Utilities;
using Microsoft.Identity.Client;
using NLog;
using System;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public class AppAuthenticator : IAuthenticator
    {
        public bool IsUserContext => false;
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private IConfidentialClientApplication app;
        private String ClientId;
        private String TenantId;

        public AppAuthenticator(String ClientId, String ClientSecret, string tenantId, IMsalHttpClientFactory httpClientFactory)
        {
            this.ClientId = ClientId;
            this.TenantId = tenantId;
            var builder = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(ClientSecret)
                .WithTenantId(tenantId);
            if (httpClientFactory != null)
            {
                builder.WithHttpClientFactory(httpClientFactory);
            }
            app = builder.Build();
        }
        public async Task<string> GetAccessToken(string[] scopes)
        {
            try
            {
                var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return authResult.AccessToken;
            }
            catch (MsalException ex)
            {
                logger.Warn("AppAuthenticator.GetAccessToken: {0}", ex.ErrorCode);
                logger.Debug("AppAuthenticator.GetAccessToken: {0}", ex.Message);
                return null;
            }
        }

        public Task<string> GetTenantId()
        {
            return Task.FromResult(this.TenantId);
        }

        public Task<string> GetUserId()
        {
            return Task.FromResult(this.ClientId);
        }

        public Task<string> GetUsername()
        {
            return Task.FromResult(this.ClientId);
        }
    }
}
