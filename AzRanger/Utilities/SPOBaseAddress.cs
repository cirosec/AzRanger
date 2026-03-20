using AzRanger.Models;
using AzRanger.Models.MSGraph;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Utilities
{
    public static class SPOBaseAddress
    {
        internal static Logger logger = LogManager.GetCurrentClassLogger();
        public static string GetBaseAddress(Tenant tenant)
        {
            if (tenant.OrganizationSettings != null && tenant.OrganizationSettings.verifiedDomains != null)
            {
                foreach(OrganizationSettingsVerifieddomain verifiedDomain in tenant.OrganizationSettings.verifiedDomains)
                {
                    if(verifiedDomain.capabilities != null)
                    {
                        if (verifiedDomain.capabilities.ToLower().Contains("officecommunicationsonline"))
                        {
                            return verifiedDomain.name;
                        }
                    }
                }
                logger.Warn("[-] No domain found with OfficeCommunicationsOnline... cannot check SharePoint");
                return null;
            }
            else
            {
                logger.Warn("[-] No verified domain found... cannot check SharePoint");
                return null;    
            }
        }

        public static String GetAdminAddress(string baseUrl)
            => BuildSPOUrl(baseUrl, ".onmicrosoft.com", "-admin.sharepoint.com");

        public static String GetSPOAddress(string baseUrl)
            => BuildSPOUrl(baseUrl, ".onmicrosoft.com", ".sharepoint.com");

        private static string BuildSPOUrl(string baseUrl, string oldSuffix, string newSuffix)
        {
            int idx = baseUrl.IndexOf(oldSuffix, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                return "https://" + baseUrl.Substring(0, idx) + newSuffix;
            }
            logger.Warn("[-] Base Url is not " + oldSuffix);
            return null;
        }
    }
}
