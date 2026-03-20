using AzRanger.Models.WinGraph;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public class GraphWinCollector : AbstractCollector
    {
        public const String UsersInternal = "/{0}/users/{1}";
        public const String RoleDefinitions = "/myorganization/roleDefinitions";

        public GraphWinCollector(IAuthenticator authenticator, String tenantId, String proxy)
        {
            this.Authenticator = authenticator;
            this.TenantId = tenantId;
            this.BaseAddress = "https://graph.windows.net";
            this.Scope = new string[] { "https://graph.windows.net/.default", "offline_access" };
            this.client = Helper.GetDefaultClient(this.additionalHeaders, proxy);
        }
        public Task<List<RoleDefinition>> GetRoleDefinitions()
        {
            return GetAllOf<RoleDefinition>(RoleDefinitions, "?api-version=1.61-internal&$select=objectId,displayName,isBuilt,isEnabled");
        }
        public Task<StrongAuthenticationDetail> GetStrongAuthenticationDetail(Guid objectId)
        {
            String endPoint = string.Format(UsersInternal, this.TenantId, objectId);
            return Get<StrongAuthenticationDetail>(endPoint, "?api-version=1.61-internal&$select=strongAuthenticationDetail,objectId");
        }

        internal async override Task<List<T>> GetAllOf<T>(string endPoint, string query = null, List<Tuple<string, string>> additionalHeaders = null)
        {
            String accessToken = await this.Authenticator.GetAccessToken(this.Scope);
            if (accessToken == null)
            {
                return new List<T>();
            }
            string usedEndpoint = endPoint;
            if (query != null)
            {
                if (query.StartsWith("?"))
                {
                    usedEndpoint = endPoint + query;
                }
                else
                {
                    usedEndpoint = endPoint + "?" + query;
                }
            }
            // Create the result list
            List<T> resultList = new List<T>();
            string url = BaseAddress + usedEndpoint;
            int unauthorizedRetryCount = 0;
            while (url != null)
            {
                HttpResponseMessage response = null;
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        response = await client.SendAsync(request);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        WinGraphGernericResponse genericAnswer = JsonSerializer.Deserialize<WinGraphGernericResponse>(result);
                        if (genericAnswer?.value == null)
                        {
                            logger.Debug("GraphWinScanner.GetAllOf: Response has no value array.");
                            return resultList;
                        }
                        logger.Debug("GraphWinScanner.GetAllOf: {0} elements in response", genericAnswer.value.Length);

                        foreach (var entry in genericAnswer.value)
                        {
                            try
                            {
                                var resultParsed = JsonSerializer.Deserialize<T>(entry.ToString());
                                resultList.Add(resultParsed);
                            }
                            catch (Exception e)
                            {
                                logger.Debug("GraphWinScanner.GetAllOf: DeserializationFailed");
                                logger.Debug(e.Message);
                                logger.Debug(entry.ToString());
                                return resultList;
                            }
                        }

                        if (genericAnswer.odatanextLink != null)
                        {
                            url = genericAnswer.odatanextLink;
                            unauthorizedRetryCount = 0;
                        }
                        else
                        {
                            url = null;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized && unauthorizedRetryCount < 1)
                    {
                        logger.Debug("GraphWinScanner.GetAllOf: 401 on {0}, refreshing token and retrying.", url);
                        unauthorizedRetryCount++;
                        accessToken = await Authenticator.GetAccessToken(this.Scope);
                        if (accessToken != null)
                        {
                            continue;
                        }
                        return resultList;
                    }
                    else
                    {
                        try
                        {
                            logger.Debug("GraphWinScanner.GetAllOf: {0}|{1} was not successful", typeof(T).ToString(), usedEndpoint);
                            logger.Debug("GraphWinScanner.GetAllOf: Status Code {0}", response.StatusCode);
                            logger.Debug(await response.Content.ReadAsStringAsync());
                        }
                        catch (Exception ex) { logger.Debug("GraphWinScanner.GetAllOf: Failed to read error body: {0}", ex.Message); }
                        return resultList;
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Dispose();
                    }
                }
            }
            return resultList;
        }
    }


    public class WinGraphGernericResponse
    {
        [JsonPropertyName("odata.metadata")]
        public string odatametadata { get; set; }
        [JsonPropertyName("odata.nextLink")]
        public string odatanextLink { get; set; }
        public object[] value { get; set; }
    }


    public class StrongAuthenticationDetailAndObjectId
    {
        [JsonPropertyName("odata.type")]
        public string odatatype { get; set; }
        public Guid objectId { get; set; }
        public StrongAuthenticationDetail strongAuthenticationDetail { get; set; }
    }
}
