using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.MSGraph
{
    public class ActivityBasedTimeoutPolicy
    {
        public ActivityBasedTimeoutPolicyDefinition[] definition { get; set; }
        public object deletedDateTime { get; set; }
        public string displayName { get; set; }
        public string id { get; set; }
        public bool isOrganizationDefault { get; set; }
    }

    public class ActivityBasedTimeoutPolicyDefinition
    {
        public ActivityBasedTimeoutPolicyDefinitionActivitybasedtimeoutpolicy ActivityBasedTimeoutPolicy { get; set; }
    }

    public class ActivityBasedTimeoutPolicyDefinitionActivitybasedtimeoutpolicy
    {
        public ActivityBasedTimeoutPolicyDefinitionActivitybasedtimeoutpolicyApplicationpolicy[] ApplicationPolicies { get; set; }
        public int Version { get; set; }
    }

    public class ActivityBasedTimeoutPolicyDefinitionActivitybasedtimeoutpolicyApplicationpolicy
    {
        public string ApplicationId { get; set; }
        public string WebSessionIdleTimeout { get; set; }
    }


}
