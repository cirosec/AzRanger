using AzRanger.Models.SharePoint;
using System;
using System.Collections.Generic;


namespace AzRanger.Models
{
    public class SharePointInformation
    {
        public string AdminUrl { get; private set; }
        public string SharePointUrl { get; private set; }
        public SPOInternalUseOnly SharePointInternalInfos { get; set; }
        public SPOTenant SPOTenant { get; set; }
        public List<SPOSite> SPOPages { get; set; } 

        public SharePointInformation(string AdminUrl, String SharePointUrl)
        {
            this.AdminUrl = AdminUrl;
            this.SharePointUrl = SharePointUrl;
        }
    }
}
