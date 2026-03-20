using AzRanger.Models.ComplianceCenter;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    class ComplianceCenterCollector : PowerShellCollectorBase
    {
        public const String DLPPolicies = "Get-DlpCompliancePolicy";
        public const String DLPLabels = "Get-Label";
        public const String DLPLabelPolicy = "Get-LabelPolicy";
        public const String InitBaseAddress = "https://ps.compliance.protection.outlook.com";
        public ComplianceCenterCollector(IAuthenticator authenticator, String tenantId, String proxy)
        {
            this.Authenticator = authenticator;
            this.TenantId = tenantId;
            this.EndPoint = "/adminapi/beta/" + tenantId + "/InvokeCommand";
            this.Scope = new String[] { "https://ps.compliance.protection.outlook.com/.default", "offline_access" };
            this.client = Helper.GetDefaultClient(this.additionalHeaders, proxy);
        }

        public Task<List<DlpCompliancePolicy>> GetDLPPolicies()
        {
            if (BaseAddress == null) return Task.FromResult<List<DlpCompliancePolicy>>(null);
            return GetAllOf<DlpCompliancePolicy>(DLPPolicies, null);
        }

        public Task<List<DlpLabel>> GetDLPLabels()
        {
            if (BaseAddress == null) return Task.FromResult<List<DlpLabel>>(null);
            return GetAllOf<DlpLabel>(DLPLabels, null);
        }

        public Task<List<DlpLabelPolicy>> GetDLPLabelPolicies()
        {
            if (BaseAddress == null) return Task.FromResult<List<DlpLabelPolicy>>(null);
            return GetAllOf<DlpLabelPolicy>(DLPLabelPolicy, null);
        }

        public async Task<String> GetBaseAddress()
        {
            String accessToken = await this.Authenticator.GetAccessToken(this.Scope);
            if (string.IsNullOrWhiteSpace(accessToken))
                return null;

            var requestUri = new Uri(new Uri(InitBaseAddress, UriKind.Absolute), EndPoint);

            HttpResponseMessage response = null;
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                }
                if (response.StatusCode == HttpStatusCode.Redirect)
                {
                    var location = response.Headers.Location;
                    if (location is null)
                    {
                        logger.Warn("ComplianceCenterScanner.GetBaseAddress: Redirect without Location header.");
                        return null;
                    }
                    var locationUri = location.IsAbsoluteUri ? location : new Uri(requestUri, location);
                    var normalized = NormalizeHost(locationUri);
                    var baseAddress = ExtractBaseAddress(normalized, EndPoint);

                    return baseAddress;
                }
                else
                {
                    logger.Warn("ComplianceCenterScanner.GetBaseAddress: Failed getting base url");
                    logger.Debug("ComplianceCenterScanner.GetBaseAddress: Status code: {0}", (int)response.StatusCode);
                }
                return null;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        private static Uri NormalizeHost(Uri uri)
        {
            var builder = new UriBuilder(uri);
            builder.Host = builder.Host.Replace("admin", "ps.compliance");

            return builder.Uri;
        }

        private static string ExtractBaseAddress(Uri uri, string endPoint)
        {
            var marker = $":446{endPoint}";
            var s = uri.ToString();

            var idx = s.IndexOf(marker, StringComparison.Ordinal);
            if (idx >= 0)
                return s.Substring(0, idx);

            return $"{uri.Scheme}://{uri.Authority}";
        }

    }
}
