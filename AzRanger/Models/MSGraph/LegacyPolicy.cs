using System;
using System.Text.Json;

namespace AzRanger.Models.MSGraph
{
    public class LegacyPolicy
    {
        public string id { get; set; }
        public object deletedDateTime { get; set; }
        public object alternativeIdentifier { get; set; }
        public DateTime? createdDateTime { get; set; }
        public string[] definition { get; set; }
        public string displayName { get; set; }
        public object isManagementRestricted { get; set; }
        public bool isOrganizationDefault { get; set; }
        public string type { get; set; }
        public object[] keyCredentials { get; set; }

        // Parsed version of the definition JSON string
        public B2BManagementPolicyDefinition B2BManagementPolicyDefinition
        {
            get
            {
                if (definition == null || definition.Length == 0) return null;
                try
                {
                    return JsonSerializer.Deserialize<B2BManagementPolicyDefinition>(definition[0]);
                }
                catch
                {
                    return null;
                }
            }
        }
    }

    public class B2BManagementPolicyDefinition
    {
        public B2BManagementPolicy B2BManagementPolicy { get; set; }
    }

    public class B2BManagementPolicy
    {
        public InvitationsAllowedAndBlockedDomainsPolicy InvitationsAllowedAndBlockedDomainsPolicy { get; set; }
        public AutoRedeemPolicy AutoRedeemPolicy { get; set; }
    }

    public class InvitationsAllowedAndBlockedDomainsPolicy
    {
        public string[] AllowedDomains { get; set; }
        public string[] BlockedDomains { get; set; }
    }

    public class AutoRedeemPolicy
    {
        public string[] AdminConsentedForUsersIntoTenantIds { get; set; }
        public string[] NoAADConsentForUsersFromTenantsIds { get; set; }
    }
}