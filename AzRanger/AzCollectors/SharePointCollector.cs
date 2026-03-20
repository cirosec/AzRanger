using AzRanger.Models.SharePoint;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    class SharePointCollector : AbstractCollector
    {
        public const String SPOInternalUseOnly = "/_api/SPOInternalUseOnly.Tenant";
        public const String SPOQuerySearch = "/_api/search/query?";
        public const String SPOGetSiteProperties = "/_api/SPO.Tenant/sites('{0}')";
        public const String SPOTenant = "/_api/SPO.Tenant";
        private static readonly SemaphoreSlim semaphoreGetPageProperties = new SemaphoreSlim(500);
        public SharePointCollector(IAuthenticator authenticator, String baseAddress, String tenantId, String proxy)
        {
            this.Authenticator = authenticator;
            this.TenantId = tenantId;
            this.BaseAddress = baseAddress;
            this.additionalHeaders = new List<Tuple<string, string>>
            {
                Tuple.Create("Odata-Version", "4.0")
            };
            String baseScope = baseAddress + "/.default";
            this.Scope = new string[] { baseScope, "offline_access" };
            this.client = Helper.GetDefaultClient(additionalHeaders, proxy);
        }

        public Task<SPOInternalUseOnly> GetSharePointSettings()
        {
            return Get<SPOInternalUseOnly>(SPOInternalUseOnly);
        }

        public Task<SPOTenant> GetSPOTenant()
        {
            return Get<SPOTenant>(SPOTenant);
        }

        private async Task GetSitePropertiesAsync(SPOSite site)
        {
            await semaphoreGetPageProperties.WaitAsync();
            try
            {
                site.Properties = await Get<SPOSiteProperties>(String.Format(SPOGetSiteProperties, site.Id));
            }
            finally
            {
                semaphoreGetPageProperties.Release();
            }
        }

        public async Task<List<SPOSite>> GetSPOPages() {
            SPOSearchQueryResult result = await Get<SPOSearchQueryResult>(SPOQuerySearch + "querytext='contentclass:STS_Site contentclass:STS_Web'&selectproperties='Title,Path,Id'");
            if (result != null) { 
                List<SPOSite> pages = new List<SPOSite>();
                List<Task> getSitePropertiesTask = new List<Task>();
                foreach (SPOSearchQueryResultRow row in result.PrimaryQueryResult.RelevantResults.Table.Rows)
                {
                    SPOSite page = new SPOSite();
                    foreach (SPOSearchQueryResultCell cell in row.Cells)
                    {
                        try
                        {
                            switch (cell.Key)
                            {
                                case "Title":
                                    page.Title = cell.Value;
                                    break;
                                case "Path":
                                    page.Path = cell.Value;
                                    break;
                                case "SiteId":
                                    page.Id = Guid.Parse(cell.Value);
                                    break;
                            }
                            if (page.Title != null && page.Path != null && page.Id != null)
                            {
                                getSitePropertiesTask.Add(Task.Run(() => GetSitePropertiesAsync(page)));
                                pages.Add(page);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Debug("SharePointCollector.GetSPOPages: {0}", e.Message);
                            break;
                        }
                    }
                }
                await Task.WhenAll(getSitePropertiesTask);
                return pages;
            }
            else
            {
                return null;
            }
        }
    }
}
