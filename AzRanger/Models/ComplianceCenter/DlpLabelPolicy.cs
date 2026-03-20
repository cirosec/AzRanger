using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ComplianceCenter
{
    public class DlpLabelPolicy
    {
        public string Type { get; set; }
        public string Settingsodatatype { get; set; }
        public string[] Settings { get; set; }
        public string Labelsodatatype { get; set; }
        public string[] Labels { get; set; }
        public string ScopedLabelsodatatype { get; set; }
        public string[] ScopedLabels { get; set; }
        public string PolicySettingsBlob { get; set; }
        public string UPELabelRulesodatatype { get; set; }
        public string[] UPELabelRules { get; set; }
        public string SharePointLocationodatatype { get; set; }
        public object[] SharePointLocation { get; set; }
        public string SharePointLocationExceptionodatatype { get; set; }
        public object[] SharePointLocationException { get; set; }
        public string ExchangeLocationodatatype { get; set; }
        public string[] ExchangeLocation { get; set; }
        public string ExchangeLocationExceptionodatatype { get; set; }
        public object[] ExchangeLocationException { get; set; }
        public string PublicFolderLocationodatatype { get; set; }
        public object[] PublicFolderLocation { get; set; }
        public string SkypeLocationodatatype { get; set; }
        public object[] SkypeLocation { get; set; }
        public string SkypeLocationExceptionodatatype { get; set; }
        public object[] SkypeLocationException { get; set; }
        public string ModernGroupLocationodatatype { get; set; }
        public object[] ModernGroupLocation { get; set; }
        public string ModernGroupLocationExceptionodatatype { get; set; }
        public object[] ModernGroupLocationException { get; set; }
        public string OneDriveLocationodatatype { get; set; }
        public object[] OneDriveLocation { get; set; }
        public string OneDriveLocationExceptionodatatype { get; set; }
        public object[] OneDriveLocationException { get; set; }
        public string ExchangeAdaptiveScopesodatatype { get; set; }
        public object[] ExchangeAdaptiveScopes { get; set; }
        public string ExchangeAdaptiveScopesExceptionodatatype { get; set; }
        public object[] ExchangeAdaptiveScopesException { get; set; }
        public string SharePointAdaptiveScopesodatatype { get; set; }
        public object[] SharePointAdaptiveScopes { get; set; }
        public string SharePointAdaptiveScopesExceptionodatatype { get; set; }
        public object[] SharePointAdaptiveScopesException { get; set; }
        public string OneDriveAdaptiveScopesodatatype { get; set; }
        public object[] OneDriveAdaptiveScopes { get; set; }
        public string OneDriveAdaptiveScopesExceptionodatatype { get; set; }
        public object[] OneDriveAdaptiveScopesException { get; set; }
        public string TeamsAdaptiveScopesodatatype { get; set; }
        public object[] TeamsAdaptiveScopes { get; set; }
        public string TeamsAdaptiveScopesExceptionodatatype { get; set; }
        public object[] TeamsAdaptiveScopesException { get; set; }
        public object EndpointDlpAdaptiveScopes { get; set; }
        public object EndpointDlpAdaptiveScopesException { get; set; }
        public object ErrorMetadata { get; set; }
        public object UserAdministrativeUnitMembershipMap { get; set; }
        public bool ForceValidate { get; set; }
        public string PolicyRulesMetaData { get; set; }
        public string GlobalListType { get; set; }
        public string Locations { get; set; }
        public string PolicyConstraints { get; set; }
        public string Workload { get; set; }
        public string Prioritydatatype { get; set; }
        public int Priority { get; set; }
        public string ObjectVersiondatatype { get; set; }
        public string ObjectVersionodatatype { get; set; }
        public string ObjectVersion { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public bool ReadOnly { get; set; }
        public string ExternalIdentity { get; set; }
        public string Comment { get; set; }
        public bool Enabled { get; set; }
        public string Mode { get; set; }
        public string DistributionStatus { get; set; }
        public string DistributionSyncStatus { get; set; }
        public object DistributionResults { get; set; }
        public object LastStatusUpdateTime { get; set; }
        public string ModificationTimeUtcdatatype { get; set; }
        public DateTime ModificationTimeUtc { get; set; }
        public string CreationTimeUtcdatatype { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public object PolicyRBACScopes { get; set; }
        public string Identity { get; set; }
        public string Id { get; set; }
        public bool IsValid { get; set; }
        public string ExchangeVersion { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public object ObjectCategory { get; set; }
        public string ObjectClassodatatype { get; set; }
        public string[] ObjectClass { get; set; }
        public string WhenChangeddatatype { get; set; }
        public DateTime? WhenChanged { get; set; }
        public string WhenCreateddatatype { get; set; }
        public DateTime? WhenCreated { get; set; }
        public string WhenChangedUTCdatatype { get; set; }
        public DateTime? WhenChangedUTC { get; set; }
        public string WhenCreatedUTCdatatype { get; set; }
        public DateTime? WhenCreatedUTC { get; set; }
        public string ExchangeObjectIddatatype { get; set; }
        public string ExchangeObjectIdodatatype { get; set; }
        public string ExchangeObjectId { get; set; }
        public string OrganizationalUnitRoot { get; set; }
        public string OrganizationId { get; set; }
        public string Guiddatatype { get; set; }
        public string Guidodatatype { get; set; }
        public string Guid { get; set; }
        public string OriginatingServer { get; set; }
    }

}
