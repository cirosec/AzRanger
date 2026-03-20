using AzRanger.Models.Generic;
using AzRanger.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzRanger
{
    static class Helper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly object clientLock = new object();
        private static HttpClient cachedClient;
        private static string cachedClientProxy;

        public static async Task<String> GetTenantIdToDomain(string domain, string proxy)
        {
            String result = null;
            result = await GetOpenIDConfiguration(domain, proxy);
            if (result == null)
            {
                return null;
            }
            var resultParsed = JsonSerializer.Deserialize<OpenIDConfiguration>(result);
            if (resultParsed.authorization_endpoint != null)
            {
                result = resultParsed.authorization_endpoint.Split('/')[3];
            }
            else
            {
                return null;
            }
            return result;
        }

        internal static async Task<string> GetOpenIDConfiguration(string domain, string proxy)
        {
            string uri = "/" + domain + "/.well-known/openid-configuration";
            return await GetFrom("https://login.microsoftonline.com", uri, proxy);
        }

        internal static async Task<string> GetFrom(string baseAddress, string uri, string proxy)
        {
            var client = GetSharedClient(proxy);
            using (var message = new HttpRequestMessage(HttpMethod.Get, baseAddress + uri))
            using (var response = await client.SendAsync(message))
            {
                if (!response.IsSuccessStatusCode)
                {
                    logger.Debug("Helper.GetFrom: {0}{1} returned {2}", baseAddress, uri, (int)response.StatusCode);
                    return null;
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        private static HttpClient GetSharedClient(string proxy)
        {
            lock (clientLock)
            {
                if (cachedClient != null && cachedClientProxy == proxy)
                {
                    return cachedClient;
                }
                cachedClient = GetDefaultClient(null, proxy);
                cachedClientProxy = proxy;
                return cachedClient;
            }
        }

        internal static async Task PressKeyToContinue(string message = "Press any key to continue...")
        {
            Console.WriteLine(message);
            while (!Console.KeyAvailable)
            {
                await Task.Delay(250);
            }
            Console.ReadKey(true);
        }

        internal static HttpClient GetDefaultClient(List<Tuple<String, String>> additionalHeaders = null, String proxy = null)
        {

            HttpClientHandler handler = new HttpClientHandler();
            if (proxy != null)
            {
                var usedproxy = new WebProxy
                {
                    Address = new Uri($"http://{proxy}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false
                };
                handler.Proxy = usedproxy;
            }

            if (proxy != null)
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
            }
            handler.AllowAutoRedirect = false;
            handler.MaxConnectionsPerServer = 25;
            var client = new HttpClient(new RetryHandler(handler));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; Win64; x64; Trident/7.0; .NET4.0C; .NET4.0E)");
            client.DefaultRequestHeaders.Add("X-Ms-Client-Request-Id", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (additionalHeaders != null)
            {
                foreach (Tuple<string, string> header in additionalHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Item1, header.Item2);
                }
            }
            return client;
        }
    }
}
