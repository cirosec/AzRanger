using AzRanger.Models.Teams;
using System.Collections.Generic;

namespace AzRanger.Models
{
    public class TeamsSettings
    {
        public List<TeamsClientConfiguration> TeamsClientConfigurations { get; set; }
        public List<TenantFederationSetting> TenantFederationSettings { get; set; }
        public List<TeamsMeetingPolicy> TeamsMeetingPolicies { get; set; }
        public List<TeamsExternalPolicy> TeamsExternalPolicies { get; set; }
        public List<TeamsMessagingPolicy> TeamsMessagePolicies { get; internal set; }
    }
}
