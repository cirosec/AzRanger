using AzRanger.Models;
using AzRanger.Models.MSGraph;
using DnsClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzRanger.Utilities.EnrichmentEngine
{
    internal static class CheckIfRedirectUriExist
    {
        internal static Logger logger = LogManager.GetCurrentClassLogger();
        internal static LookupClient lookup = new LookupClient();

        private static readonly HashSet<string> NonPublicTlds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "local", "internal", "corp", "lan", "home", "intranet", "private", "localdomain"
        };

        private static bool IsLoopbackIp(string host)
        {
            if (System.Net.IPAddress.TryParse(host, out var ip))
            {
                return System.Net.IPAddress.IsLoopback(ip);
            }
            return false;
        }

        private static bool HasNonPublicTld(string host)
        {
            var lastDot = host.LastIndexOf('.');
            if (lastDot < 0) return false;
            var tld = host.Substring(lastDot + 1);
            return NonPublicTlds.Contains(tld);
        }

        public async static Task Enrich(Tenant tenant)
        {
            List<Task> dnsTasks = new List<Task>();
            foreach (Application application in tenant.Applications.Values)
            {
                if (application.web != null)
                {
                    foreach (string redirectUri in application.web.redirectUris)
                    {
                        Uri uri;
                        string host;
                        try {
                            uri = new Uri(redirectUri.Replace("*.", ""));
                            host = uri.Host.ToLower();
                            if (host.Equals("localhost")
                                || host.Equals("visualstudio")
                                || IsLoopbackIp(host)
                                || HasNonPublicTld(host))
                            {
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Debug("CheckIfRedirectUriExist.enrich: Creating record {0} failed.", redirectUri);
                            logger.Debug(e.Message);
                            application.web.allRedirectUrisAreRegistered = false;
                            continue;
                        }
                        dnsTasks.Add(GetDNSData(application, host));
                    }
                }
            }
            await Task.WhenAll(dnsTasks);
        }

        private async static Task GetDNSData(Application app, string host){
            try
            {
                var result = await lookup.QueryAsync(host, QueryType.A);
                if (result != null)
                {
                    if (result.HasError)
                    {
                        app.web.allRedirectUrisAreRegistered = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("CheckIfRedirectUriExist.enrich: Checking record {0} failed.", host);
                logger.Debug(e.Message);
                app.web.allRedirectUrisAreRegistered = false;
            }
        }
    }
}
