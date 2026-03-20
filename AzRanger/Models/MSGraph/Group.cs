using AzRanger.Models.Generic;
using AzRanger.Output;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzRanger.Models
{
    public class Group : IReporting
    {
        [JsonPropertyName("@odata.nextLink")]
        public String odatanextLink;
        public Guid id { get; set; }
        public object deletedDateTime { get; set; }
        public object classification { get; set; }
        public DateTime? createdDateTime { get; set; }
        public object createdByAppId { get; set; }
        public string organizationId { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public object expirationDateTime { get; set; }
        public string[] groupTypes { get; set; }
        public object[] infoCatalogs { get; set; }
        public object isAssignableToRole { get; set; }
        public object isManagementRestricted { get; set; }
        public string mail { get; set; }
        public bool mailEnabled { get; set; }
        public string mailNickname { get; set; }
        public string membershipRule { get; set; }
        public string membershipRuleProcessingState { get; set; }
        public object onPremisesDomainName { get; set; }
        public object onPremisesLastSyncDateTime { get; set; }
        public object onPremisesNetBiosName { get; set; }
        public object onPremisesSamAccountName { get; set; }
        public object onPremisesSecurityIdentifier { get; set; }
        public object onPremisesSyncEnabled { get; set; }
        public object preferredDataLocation { get; set; }
        public object preferredLanguage { get; set; }
        public string[] proxyAddresses { get; set; }
        public DateTime? renewedDateTime { get; set; }
        public object[] resourceBehaviorOptions { get; set; }
        public object[] resourceProvisioningOptions { get; set; }
        public bool securityEnabled { get; set; }
        public string securityIdentifier { get; set; }
        public object theme { get; set; }
        public string visibility { get; set; }
        public object uniqueName { get; set; }
        public object onPremisesExtensionAttributes { get; set; }
        public object[] onPremisesProvisioningErrors { get; set; }
        public object[] serviceProvisioningErrors { get; set; }
        public GroupWritebackconfiguration writebackConfiguration { get; set; }
        public List<AzurePrincipal> members = new List<AzurePrincipal>();

        public string PrintConsole()
        {
            return String.Format(@"{0} - {1}", this.displayName, this.id);
        }

        public string PrintCSV()
        {
            return String.Format(@"{0};{1}", this.id, this.displayName);
        }
        public AffectedItem GetAffectedItem()
        {
            return new AffectedItem(this.id.ToString(), this.displayName);
        }
    }

    public class GroupWritebackconfiguration
    {
        public object isEnabled { get; set; }
        public object onPremisesGroupType { get; set; }
    }

}
