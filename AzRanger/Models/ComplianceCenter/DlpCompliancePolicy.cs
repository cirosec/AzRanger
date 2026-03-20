
using AzRanger.Output;
using System;

namespace AzRanger.Models.ComplianceCenter

{
    public class DlpCompliancePolicy : IReporting
    {
        public string Mode { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string ExchangeLocationodatatype { get; set; }
        public object[] ExchangeLocation { get; set; }
        public string SharePointLocationodatatype { get; set; }
        public object[] SharePointLocation { get; set; }
        public string SharePointLocationExceptionodatatype { get; set; }
        public object[] SharePointLocationException { get; set; }
        public string OneDriveLocationodatatype { get; set; }
        public object[] OneDriveLocation { get; set; }
        public string OneDriveLocationExceptionodatatype { get; set; }
        public object[] OneDriveLocationException { get; set; }
        public string ExchangeOnPremisesLocationodatatype { get; set; }
        public object[] ExchangeOnPremisesLocation { get; set; }
        public string SharePointOnPremisesLocationodatatype { get; set; }
        public object[] SharePointOnPremisesLocation { get; set; }
        public string SharePointOnPremisesLocationExceptionodatatype { get; set; }
        public object[] SharePointOnPremisesLocationException { get; set; }
        public string TeamsLocationodatatype { get; set; }
        public string[] TeamsLocation { get; set; }
        public string TeamsLocationExceptionodatatype { get; set; }
        public object[] TeamsLocationException { get; set; }
        public string EndpointDlpLocationodatatype { get; set; }
        public object[] EndpointDlpLocation { get; set; }
        public string EndpointDlpLocationExceptionodatatype { get; set; }
        public object[] EndpointDlpLocationException { get; set; }
        public string ThirdPartyAppDlpLocationodatatype { get; set; }
        public object[] ThirdPartyAppDlpLocation { get; set; }
        public string ThirdPartyAppDlpLocationExceptionodatatype { get; set; }
        public object[] ThirdPartyAppDlpLocationException { get; set; }
        public string OnPremisesScannerDlpLocationodatatype { get; set; }
        public object[] OnPremisesScannerDlpLocation { get; set; }
        public string OnPremisesScannerDlpLocationExceptionodatatype { get; set; }
        public object[] OnPremisesScannerDlpLocationException { get; set; }
        public string PowerBIDlpLocationodatatype { get; set; }
        public object[] PowerBIDlpLocation { get; set; }
        public string PowerBIDlpLocationExceptionodatatype { get; set; }
        public object[] PowerBIDlpLocationException { get; set; }
        public string Locations { get; set; }
        public string LocationInclusionsodatatype { get; set; }
        public object[] LocationInclusions { get; set; }
        public string LocationExclusionsodatatype { get; set; }
        public object[] LocationExclusions { get; set; }
        public string EndpointDlpExtendedLocations { get; set; }
        public string ExchangeSenderodatatype { get; set; }
        public object[] ExchangeSender { get; set; }
        public string ExchangeSenderExceptionodatatype { get; set; }
        public object[] ExchangeSenderException { get; set; }
        public string PolicyTemplateInfoodatatype { get; set; }
        public object[] PolicyTemplateInfo { get; set; }
        public string PolicyCategory { get; set; }
        public object MatchedItemsCount { get; set; }
        public object TotalItemsCount { get; set; }
        public object TopNLocationStatistics { get; set; }
        public object WorkloadStatistics { get; set; }
        public bool IsSimulationPolicy { get; set; }
        public object SimulationStatus { get; set; }
        public object AutoEnableAfter { get; set; }
        public object IsFromSmartInsights { get; set; }
        public bool IsColdDataSimulationPolicy { get; set; }
        public object ExtendedProperties { get; set; }
        public bool Summary { get; set; }
        public object EnforcementPlanes { get; set; }
        public object LogicalWorkload { get; set; }
        public string OneDriveSharedByodatatype { get; set; }
        public object[] OneDriveSharedBy { get; set; }
        public string ExceptIfOneDriveSharedByodatatype { get; set; }
        public object[] ExceptIfOneDriveSharedBy { get; set; }
        public string OneDriveSharedByMemberOfodatatype { get; set; }
        public object[] OneDriveSharedByMemberOf { get; set; }
        public string ExceptIfOneDriveSharedByMemberOfodatatype { get; set; }
        public object[] ExceptIfOneDriveSharedByMemberOf { get; set; }
        public string ExchangeSenderMemberOfodatatype { get; set; }
        public object[] ExchangeSenderMemberOf { get; set; }
        public string ExchangeSenderMemberOfExceptionodatatype { get; set; }
        public object[] ExchangeSenderMemberOfException { get; set; }
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
        public string EndpointDlpAdaptiveScopesodatatype { get; set; }
        public object[] EndpointDlpAdaptiveScopes { get; set; }
        public string EndpointDlpAdaptiveScopesExceptionodatatype { get; set; }
        public object[] EndpointDlpAdaptiveScopesException { get; set; }
        public string ExpectedLocationsdatatype { get; set; }
        public int ExpectedLocations { get; set; }
        public string CompletedLocationsdatatype { get; set; }
        public int CompletedLocations { get; set; }
        public string FailedLocationsdatatype { get; set; }
        public int FailedLocations { get; set; }
        public object ItemStatistics { get; set; }
        public object RuleMatchBlob { get; set; }
        public object ErrorMetadata { get; set; }
        public object UserAdministrativeUnitMembershipMap { get; set; }
        public bool ForceValidate { get; set; }
        public string PolicyRulesMetaData { get; set; }
        public string GlobalListType { get; set; }
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
        public string ModeMode { get; set; }
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
        public DateTime WhenChanged { get; set; }
        public string WhenCreateddatatype { get; set; }
        public DateTime WhenCreated { get; set; }
        public string WhenChangedUTCdatatype { get; set; }
        public DateTime WhenChangedUTC { get; set; }
        public string WhenCreatedUTCdatatype { get; set; }
        public DateTime WhenCreatedUTC { get; set; }
        public string ExchangeObjectIddatatype { get; set; }
        public string ExchangeObjectIdodatatype { get; set; }
        public string ExchangeObjectId { get; set; }
        public string OrganizationalUnitRoot { get; set; }
        public string OrganizationId { get; set; }
        public string Guiddatatype { get; set; }
        public string Guidodatatype { get; set; }
        public string Guid { get; set; }
        public string OriginatingServer { get; set; }

        public string PrintConsole()
        {
            return Identity;
        }

        public string PrintCSV()
        {
            return Identity + ";";
        }
        public AffectedItem GetAffectedItem()
        {
            return new AffectedItem(this.Identity, null);
        }
    }
}
