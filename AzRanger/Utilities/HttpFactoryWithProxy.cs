using Microsoft.Identity.Client;
using System.Net.Http;

namespace AzRanger.Utilities
{
    internal class HttpFactoryWithProxy : IMsalHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public HttpFactoryWithProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
