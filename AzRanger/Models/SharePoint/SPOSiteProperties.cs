using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzRanger.Models.SharePoint
{
    public class SPOSiteProperties
    {
        [JsonPropertyName("@odata.context")]
        public string odatacontext { get; set; }
        [JsonPropertyName("@odata.type")]
        public string odatatype { get; set; }
        [JsonPropertyName("@odata.id")]
        public string odataid { get; set; }
        [JsonPropertyName("@odata.editLink")]
        public string odataeditLink { get; set; }
        public bool AllowDownloadingNonWebViewableFiles { get; set; }
        public bool AllowEditing { get; set; }
        public bool AllowSelfServiceUpgrade { get; set; }
        public int AnonymousLinkExpirationInDays { get; set; }
        public bool ApplyToExistingDocumentLibraries { get; set; }
        public bool ApplyToNewDocumentLibraries { get; set; }
        public string ArchivedBy { get; set; }
        public DateTime ArchivedTime { get; set; }
        public string ArchiveStatus { get; set; }
        public object AuthContextStrength { get; set; }
        public bool AuthenticationContextLimitedAccess { get; set; }
        public object AuthenticationContextName { get; set; }
        public float AverageResourceUsage { get; set; }
        public int BlockDownloadLinksFileType { get; set; }
        public object BlockDownloadMicrosoft365GroupIds { get; set; }
        public bool BlockDownloadPolicy { get; set; }
        public object BlockDownloadPolicyFileTypeIds { get; set; }
        public int BlockGuestsAsSiteAdmin { get; set; }
        public int BonusDiskQuota { get; set; }
        public bool ClearGroupId { get; set; }
        public bool ClearRestrictedAccessControl { get; set; }
        public bool CommentsOnSitePagesDisabled { get; set; }
        public int CompatibilityLevel { get; set; }
        public int ConditionalAccessPolicy { get; set; }
        public DateTime CreatedTime { get; set; }
        public float CurrentResourceUsage { get; set; }
        public int DefaultLinkPermission { get; set; }
        public bool DefaultLinkToExistingAccess { get; set; }
        public bool DefaultLinkToExistingAccessReset { get; set; }
        public int DefaultShareLinkRole { get; set; }
        public int DefaultShareLinkScope { get; set; }
        public int DefaultSharingLinkType { get; set; }
        // 1 = Allowed
        // 2 = Blocked
        public int DenyAddAndCustomizePages { get; set; }
        public string Description { get; set; }
        public int DisableAppViews { get; set; }
        public int DisableCompanyWideSharingLinks { get; set; }
        public int DisableFlows { get; set; }
        public bool EnableAutoExpirationVersionTrim { get; set; }
        public bool ExcludeBlockDownloadPolicySiteOwners { get; set; }
        public object[] ExcludeBlockDownloadSharePointGroups { get; set; }
        public object[] ExcludedBlockDownloadGroupIds { get; set; }
        public int ExpireVersionsAfterDays { get; set; }
        public int ExternalUserExpirationInDays { get; set; }
        public string GroupId { get; set; }
        public string GroupOwnerLoginName { get; set; }
        public bool HasHolds { get; set; }
        public bool HidePeoplePreviewingFiles { get; set; }
        public bool HidePeopleWhoHaveListsOpen { get; set; }
        public string HubSiteId { get; set; }
        public string IBMode { get; set; }
        public object[] IBSegments { get; set; }
        public object IBSegmentsToAdd { get; set; }
        public object IBSegmentsToRemove { get; set; }
        public bool InheritVersionPolicyFromTenant { get; set; }
        public bool IsGroupOwnerSiteAdmin { get; set; }
        public bool IsHubSite { get; set; }
        public bool IsTeamsChannelConnected { get; set; }
        public bool IsTeamsConnected { get; set; }
        public DateTime LastContentModifiedDate { get; set; }
        public int Lcid { get; set; }
        public int LimitedAccessFileType { get; set; }
        public bool ListsShowHeaderAndNavigation { get; set; }
        public object LockIssue { get; set; }
        public int LockReason { get; set; }
        public string LockState { get; set; }
        public int LoopDefaultSharingLinkRole { get; set; }
        public int LoopDefaultSharingLinkScope { get; set; }
        public int MajorVersionLimit { get; set; }
        public int MajorWithMinorVersionsLimit { get; set; }
        public int MediaTranscription { get; set; }
        public int OverrideBlockUserInfoVisibility { get; set; }
        public bool OverrideSharingCapability { get; set; }
        public bool OverrideTenantAnonymousLinkExpirationPolicy { get; set; }
        public bool OverrideTenantExternalUserExpirationPolicy { get; set; }
        public string Owner { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerLoginName { get; set; }
        public string OwnerName { get; set; }
        public int PWAEnabled { get; set; }
        public bool ReadOnlyAccessPolicy { get; set; }
        public bool ReadOnlyForBlockDownloadPolicy { get; set; }
        public bool ReadOnlyForUnmanagedDevices { get; set; }
        public string RelatedGroupId { get; set; }
        public bool RequestFilesLinkEnabled { get; set; }
        public int RequestFilesLinkExpirationInDays { get; set; }
        public bool RestrictContentOrgWideSearch { get; set; }
        public bool RestrictedAccessControl { get; set; }
        public object[] RestrictedAccessControlGroups { get; set; }
        public object RestrictedAccessControlGroupsToAdd { get; set; }
        public object RestrictedAccessControlGroupsToRemove { get; set; }
        public int RestrictedToRegion { get; set; }
        public int SandboxedCodeActivationCapability { get; set; }
        public string SensitivityLabel { get; set; }
        public string SensitivityLabel2 { get; set; }
        public bool SetOwnerWithoutUpdatingSecondaryAdmin { get; set; }
        public string SharingAllowedDomainList { get; set; }
        public string SharingBlockedDomainList { get; set; }
        public int SharingCapability { get; set; }
        public int SharingDomainRestrictionMode { get; set; }
        public bool SharingLockDownCanBeCleared { get; set; }
        public bool SharingLockDownEnabled { get; set; }
        public bool ShowPeoplePickerSuggestionsForGuestUsers { get; set; }
        public int SiteDefinedSharingCapability { get; set; }
        public string SiteId { get; set; }
        public bool SocialBarOnSitePagesDisabled { get; set; }
        public object Status { get; set; }
        public int StorageMaximumLevel { get; set; }
        public object StorageQuotaType { get; set; }
        public int StorageUsage { get; set; }
        public int StorageWarningLevel { get; set; }
        public int TeamsChannelType { get; set; }
        public string Template { get; set; }
        public int TimeZoneId { get; set; }
        public string Title { get; set; }
        public object TitleTranslations { get; set; }
        public string Url { get; set; }
        public float UserCodeMaximumLevel { get; set; }
        public float UserCodeWarningLevel { get; set; }
        public int WebsCount { get; set; }
    }

}
