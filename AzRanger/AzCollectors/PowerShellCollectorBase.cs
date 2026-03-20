using AzRanger.Models.Generic;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public abstract class PowerShellCollectorBase : AbstractCollector
    {
        protected String EndPoint;

        internal async Task<List<T>> GetAllOf<T>(string command, List<Tuple<string, string>> parameters = null, List<Tuple<string, string>> additionalHeaders = null)
        {
            String accessToken = await this.Authenticator.GetAccessToken(this.Scope);
            if (accessToken == null)
            {
                return new List<T>();
            }

            logger.Debug("PowerShellCollectorBase.GetAllOf: {0}|{1}", typeof(T).ToString(), command);
            String url = this.BaseAddress + this.EndPoint;
            List<T> resultList = new List<T>();
            int unauthorizedRetryCount = 0;
            while (url != null)
            {
                HttpResponseMessage response = null;
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        if (additionalHeaders != null)
                        {
                            foreach (var header in additionalHeaders)
                            {
                                request.Headers.Add(header.Item1, header.Item2);
                            }
                        }
                        request.Content = CreateMessage(command, parameters);
                        response = await client.SendAsync(request);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        GenResponse genericAnswer = null;
                        try
                        {
                            genericAnswer = JsonSerializer.Deserialize<GenResponse>(result);
                        }
                        catch (Exception e)
                        {
                            logger.Debug("PowerShellCollectorBase.GetAllOf: Failed to parse response.");
                            logger.Debug(e.Message);
                            logger.Debug(result.ToString());
                            return resultList;
                        }
                        if (genericAnswer?.value == null)
                        {
                            logger.Debug("PowerShellCollectorBase.GetAllOf: Response has no value array.");
                            return resultList;
                        }
                        logger.Debug("PowerShellCollectorBase.GetAllOf: Response has {0} entries.", genericAnswer.value.Length);
                        foreach (var entry in genericAnswer.value)
                        {
                            try
                            {
                                var resultParsed = JsonSerializer.Deserialize<T>(entry.ToString());
                                resultList.Add(resultParsed);
                            }
                            catch (Exception e)
                            {
                                logger.Debug("PowerShellCollectorBase.GetAllOf: Failed to parse entry.");
                                logger.Debug(e.Message);
                                logger.Debug(entry.ToString());
                                return resultList;
                            }
                        }

                        if (genericAnswer.odatanextLink != null)
                        {
                            logger.Debug("PowerShellCollectorBase.GetAllOf: Odatanextlink is: {0}", genericAnswer.odatanextLink);
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
                        logger.Debug("PowerShellCollectorBase.GetAllOf: 401 on {0}, refreshing token and retrying.", url);
                        unauthorizedRetryCount++;
                        accessToken = await this.Authenticator.GetAccessToken(this.Scope);
                        if (accessToken == null)
                        {
                            return resultList;
                        }
                        continue;
                    }
                    else
                    {
                        try
                        {
                            logger.Debug("PowerShellCollectorBase.GetAllOf: {0} was not successful", typeof(T).ToString());
                            logger.Debug("PowerShellCollectorBase.GetAllOf: Status Code {0}", response.StatusCode);
                            logger.Debug(await response.Content.ReadAsStringAsync());
                        }
                        catch (Exception ex) { logger.Debug("PowerShellCollectorBase.GetAllOf: Failed to read error body: {0}", ex.Message); }
                        url = null;
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

        protected HttpContent CreateMessage(string command, List<Tuple<string, string>> parameters)
        {
            var sb = new StringBuilder();
            sb.Append(@"{""CmdletInput"":{""CmdletName"":");
            sb.Append(JsonSerializer.Serialize(command));
            sb.Append(@",""Parameters"":{");
            if (parameters != null && parameters.Count > 0)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(JsonSerializer.Serialize(parameters[i].Item1));
                    sb.Append(':');
                    sb.Append(JsonSerializer.Serialize(parameters[i].Item2));
                }
            }
            sb.Append("}}}");
            return new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
