using AzRanger.Models.Generic;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public abstract class AbstractCollector
    {
        internal static Logger logger = LogManager.GetCurrentClassLogger();
        internal IAuthenticator Authenticator;
        internal String TenantId;
        internal String BaseAddress;
        internal String[] Scope;
        internal List<Tuple<string, string>> additionalHeaders = null;
        //https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        internal HttpClient client;

        internal async virtual Task<T> Get<T>(String endPoint, string query = null)
        {
            String accessToken = await Authenticator.GetAccessToken(this.Scope);
            if (accessToken == null)
            {
                return default;
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
            string url = BaseAddress + usedEndpoint;
            logger.Debug("IScanner.Get: {0}|{1}", typeof(T).ToString(), url);
            HttpResponseMessage response = null;
            try
            {
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        response = await client.SendAsync(request);
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("IScanner.Get: {0}|{1} failed...return", typeof(T).ToString(), url);
                    logger.Debug(e.Message);
                    return default;
                }
                String manipulatedResponse = null;
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        manipulatedResponse = this.ManipulateResponse(result, endPoint);
                        return JsonSerializer.Deserialize<T>(manipulatedResponse);
                    }
                    catch (Exception e)
                    {
                        logger.Debug("IScanner.Get: DeserializationFailed");
                        logger.Debug(e.Message);
                        logger.Debug(manipulatedResponse);
                        return default;
                    }
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Token may have expired — refresh once and retry
                    logger.Debug("IScanner.Get: 401 on {0}, refreshing token and retrying.", url);
                    response.Dispose();
                    response = null;
                    accessToken = await Authenticator.GetAccessToken(this.Scope);
                    if (accessToken != null)
                    {
                        try
                        {
                            using (var retryRequest = new HttpRequestMessage(HttpMethod.Get, url))
                            {
                                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                                response = await client.SendAsync(retryRequest);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Debug("IScanner.Get: retry request failed.");
                            logger.Debug(e.Message);
                            return default;
                        }
                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                manipulatedResponse = this.ManipulateResponse(result, endPoint);
                                return JsonSerializer.Deserialize<T>(manipulatedResponse);
                            }
                            catch (Exception e)
                            {
                                logger.Debug("IScanner.Get: DeserializationFailed after retry");
                                logger.Debug(e.Message);
                                return default;
                            }
                        }
                    }
                    logger.Debug("IScanner.Get: 401 retry failed for {0}.", url);
                    return default;
                }
                else
                {
                    try
                    {
                        logger.Debug("IScanner.Get: {0}|{1} was not successful.", typeof(T).ToString(), url);
                        logger.Debug("IScanner.Get: Status Code {0}.", response.StatusCode);
                        logger.Debug(await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex) { logger.Debug("IScanner.Get: Failed to read error body: {0}", ex.Message); }
                }
                return default;
            }
            finally
            {
                if (response != null)
                {
                    response.Dispose();
                }
            }
        }

        internal virtual String ManipulateResponse(String response, String endPoint)
        {
            return response;
        }
        internal async virtual Task<List<T>> GetAllOf<T>(string endPoint, string query = null, List<Tuple<string, string>> additionalHeaders = null)
        {
            String accessToken = await Authenticator.GetAccessToken(this.Scope);
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
            string url = BaseAddress + usedEndpoint;
            List<T> resultList = new List<T>();
            int taskCancelCounter = 0;
            int maxRetries = 3;
            int unauthorizedRetryCount = 0;
            while (url != null)
            {
                logger.Debug("IScanner.GetAllOf: {0}|{1}", typeof(T).ToString(), url);
                HttpResponseMessage response = null;
                try
                {
                    try
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                            response = await client.SendAsync(request);
                        }
                    }
                    catch (TaskCanceledException excancel)
                    {
                        logger.Debug("IScanner.GetAllOf: {0}|{1} canceled...", typeof(T).ToString(), url);
                        logger.Debug(excancel.Message);
                        logger.Info("Your network seems to be unreliable, please be patient.");
                        if (taskCancelCounter < maxRetries)
                        {
                            taskCancelCounter++;
                            continue;
                        }
                        return resultList;
                    }
                    catch (Exception e)
                    {
                        logger.Debug("IScanner.GetAllOf: {0}|{1} failed...return", typeof(T).ToString(), url);
                        logger.Debug(e.Message);
                        return resultList;
                    }
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        /// Parse the result in GenericObjects
                        var result = await response.Content.ReadAsStringAsync();
                        result = ManipulateResponse(result, endPoint);
                        GenResponse genericAnswer = JsonSerializer.Deserialize<GenResponse>(result);
                        if (genericAnswer?.value == null)
                        {
                            logger.Debug("IScanner.GetAllOf: Response has no value array.");
                            url = null;
                            continue;
                        }
                        logger.Debug("IScanner.GetAllOf: {0} elements in response", genericAnswer.value.Length);

                        /// Go through the generic object and parse the value field
                        foreach (var entry in genericAnswer.value)
                        {
                            try
                            {
                                var resultParsed = JsonSerializer.Deserialize<T>(entry.ToString());
                                resultList.Add(resultParsed);
                            }
                            catch (Exception e)
                            {
                                logger.Debug("IScanner.GetAllOf: DeserializationFailed");
                                logger.Debug(e.Message);
                                logger.Debug(entry.ToString());
                            }
                        }

                        // If the generic Answer has a nextLink, we have more items then responded in the first answer
                        if (genericAnswer.odatanextLink != null)
                        {
                            url = genericAnswer.odatanextLink;
                            taskCancelCounter = 0;
                            unauthorizedRetryCount = 0; // reset per-page retry counter for the next page
                        }
                        else
                        {
                            url = null;
                        }
                    }
                    else if (response != null && response.StatusCode == HttpStatusCode.Unauthorized && unauthorizedRetryCount < 1)
                    {
                        // Token may have expired mid-pagination — refresh once per page and retry
                        logger.Debug("IScanner.GetAllOf: 401 on {0}, refreshing token and retrying.", url);
                        unauthorizedRetryCount++;
                        accessToken = await Authenticator.GetAccessToken(this.Scope);
                        if (accessToken != null)
                        {
                            continue; // next iteration will create a fresh HttpRequestMessage with the refreshed token
                        }
                        // Token refresh failed — give up
                        return resultList;
                    }
                    else
                    {
                        if (response != null)
                        {
                            try
                            {
                                logger.Debug("IScanner.GetAllOf: {0}|{1} was not successful", typeof(T).ToString(), usedEndpoint);
                                logger.Debug("IScanner.GetAllOf: Status Code {0}", response.StatusCode);
                                logger.Debug(await response.Content.ReadAsStringAsync());
                            }
                            catch (Exception ex) { logger.Debug("IScanner.GetAllOf: Failed to read error body: {0}", ex.Message); }
                        }
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
}
