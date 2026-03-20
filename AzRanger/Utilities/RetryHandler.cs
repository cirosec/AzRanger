using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzRanger.Utilities
{
    public class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 3;
        private readonly object rndLock = new object();
        private readonly Random rnd = new Random();
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public RetryHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            for (int i = 0; i < MaxRetries; i++)
            {
                using (var clonedRequest = await CloneHttpRequestMessageAsync(request))
                {
                    try
                    {
                        response?.Dispose();
                        response = await base.SendAsync(clonedRequest, cancellationToken);

                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }

                        if (response.StatusCode == HttpStatusCode.BadRequest ||
                            response.StatusCode == HttpStatusCode.Unauthorized ||
                            response.StatusCode == HttpStatusCode.Forbidden ||
                            response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return response;
                        }

                        // HTTP 429 Too Many Requests (not available as enum in .NET Framework 4.8)
                        if (response.StatusCode == (HttpStatusCode)429)
                        {
                            logger.Info("[-] Rate limited, backing off...");
                            var retryAfter = response.Headers.RetryAfter?.Delta
                                ?? TimeSpan.FromSeconds(Math.Pow(2, i + 1));
                            await Task.Delay(retryAfter, cancellationToken);
                            continue;
                        }

                        if ((int)response.StatusCode >= 500)
                        {
                            double jitter;
                            lock (rndLock) { jitter = rnd.NextDouble(); }
                            var backoff = TimeSpan.FromSeconds(Math.Pow(2, i) + jitter);
                            logger.Debug("Server error {0}, retry {1}/{2} after {3:F1}s", response.StatusCode, i + 1, MaxRetries, backoff.TotalSeconds);
                            await Task.Delay(backoff, cancellationToken);
                            continue;
                        }
                    }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                    {
                        response?.Dispose();
                        response = null;
                        logger.Warn("Request failed: {0}", ex.Message);
                        if (i == MaxRetries - 1) throw;

                        var backoff = TimeSpan.FromSeconds(Math.Pow(2, i));
                        await Task.Delay(backoff, cancellationToken);
                    }
                }
            }

            return response;
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            if (request.Content != null)
            {
                var contentBytes = await request.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);

                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var prop in request.Properties)
            {
                clone.Properties[prop.Key] = prop.Value;
            }

            return clone;
        }
    }
}
