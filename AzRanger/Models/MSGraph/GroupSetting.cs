using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.MSGraph
{
    public class GroupSetting
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string templateId { get; set; }
        public Value[] values { get; set; }
    }

    public class GroupSettingValue
    {
        public string name { get; set; }
        public string value { get; set; }
    }

}
