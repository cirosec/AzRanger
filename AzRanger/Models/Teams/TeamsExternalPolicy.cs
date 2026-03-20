using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.Teams
{
    public class TeamsExternalPolicy
    {
        public object[] AllowedExternalDomains { get; set; }
        public object[] BlockedExternalDomains { get; set; }
        public object Description { get; set; }
        public bool EnableFederationAccess { get; set; }
        public bool EnableXmppAccess { get; set; }
        public bool EnablePublicCloudAudioVideoAccess { get; set; }
        public bool EnableTeamsSmsAccess { get; set; }
        public bool EnableOutsideAccess { get; set; }
        public bool EnableAcsFederationAccess { get; set; }
        public bool EnableTeamsConsumerAccess { get; set; }
        public bool EnableTeamsConsumerInbound { get; set; }
        public bool RestrictTeamsConsumerAccessToExternalUserProfiles { get; set; }
        public bool FederatedBilateralChats { get; set; }
        public string CommunicationWithExternalOrgs { get; set; }
        public string Identity { get; set; }
        public string SchemaVersion { get; set; }
        public string ConfigId { get; set; }
        public TeamsExternalPolicyConfigmetadata ConfigMetadata { get; set; }
    }

    public class TeamsExternalPolicyConfigmetadata
    {
        public string Authority { get; set; }
    }

}
