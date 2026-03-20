using AzRanger.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.SharePoint
{
    public class SPOSite : IReporting
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public Guid? Id = null;
        public SPOSiteProperties Properties { get; set; }

        public string PrintConsole()
        {
            return $"Title: {Title} - Path: {Path}";
        }

        public string PrintCSV()
        {
            return $"{Title};{Path}";
        }

        public AffectedItem GetAffectedItem()
        {
            return new AffectedItem(Title, Path);
        }
    }
}
