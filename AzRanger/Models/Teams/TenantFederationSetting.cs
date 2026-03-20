namespace AzRanger.Models.Teams
{
    // https://admin.teams.microsoft.com/company-wide-settings/external-communications
  
    public class TenantFederationSetting
    {
        public object AllowedDomains { get; set; }
        public object BlockedDomains { get; set; }
        public object[] AllowedTrialTenantDomains { get; set; }
        // "Teams and Skype for Business users in external organizations"
        // True => If, all, allowlist or denylist
        // False => All external is blocked
        public bool AllowFederatedUsers { get; set; }
        // "Allow users in my organization to communicate with Skype users."
        public bool AllowPublicUsers { get; set; }
        public bool AllowTeamsSms { get; set; }
        // "People in my organization can communicate with Teams users whose accounts aren't managed by an organization."
        public bool AllowTeamsConsumer { get; set; }
        // "External users with Teams accounts not managed by an organization can contact users in my organization." => No effect if AllowTeamsConsumer=False
        public bool AllowTeamsConsumerInbound { get; set; }
        public bool TreatDiscoveredPartnersAsUnverified { get; set; }
        public bool SharedSipAddressSpace { get; set; }
        public bool RestrictTeamsConsumerToExternalUserProfiles { get; set; }
        public bool BlockAllSubdomains { get; set; }
        public string ExternalAccessWithTrialTenants { get; set; }
        public string DomainBlockingForMDOAdminsInTeams { get; set; }
        public string SecurityTeamAllowBlockListDelegation { get; set; }
        public Key Key { get; set; }
        public string Identity { get; set; }
        public Configmetadata ConfigMetadata { get; set; }
        public string ConfigId { get; set; }
    }

    public class Alloweddomains
    {
        public string Domain { get; set; }
    }

    public class BlockedDomains
    {
        public string Domain { get; set; }
    }
    public class TenantFederationSettingsKey
    {
        public string ScopeClass { get; set; }
        public Schemaid SchemaId { get; set; }
        public Authorityid AuthorityId { get; set; }
        public Defaultxml DefaultXml { get; set; }
        public Xmlroot1 XmlRoot { get; set; }
    }


    public class Schemaid1
    {
        public Xname1 XName { get; set; }
    }

    public class Xname1
    {
        public string name { get; set; }
    }

    public class Tenantfederationsettings
    {
        public string xmlns { get; set; }
        public Alloweddomains1 AllowedDomains { get; set; }
        public object BlockedDomains { get; set; }
        public object AllowedTrialTenantDomains { get; set; }
    }

    public class Alloweddomains1
    {
        public object AllowAllKnownDomains { get; set; }
    }

    public class Xmlroot1
    {
        public string name { get; set; }
    }

    public class Configmetadata
    {
        public string Authority { get; set; }
    }

}