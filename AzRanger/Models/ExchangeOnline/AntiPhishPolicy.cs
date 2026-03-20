using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ExchangeOnline
{
    public class AntiPhishPolicy
    {
        public bool Enabled { get; set; }
        public string ImpersonationProtectionState { get; set; }
        public bool EnableTargetedUserProtection { get; set; }
        public bool EnableMailboxIntelligenceProtection { get; set; }
        public bool EnableTargetedDomainsProtection { get; set; }
        public bool EnableOrganizationDomainsProtection { get; set; }
        public bool EnableMailboxIntelligence { get; set; }
        public bool EnableFirstContactSafetyTips { get; set; }
        public bool EnableSimilarUsersSafetyTips { get; set; }
        public bool EnableSimilarDomainsSafetyTips { get; set; }
        public bool EnableUnusualCharactersSafetyTips { get; set; }
        public string TargetedUserProtectionAction { get; set; }
        public string TargetedUserQuarantineTag { get; set; }
        public string MailboxIntelligenceProtectionAction { get; set; }
        public string MailboxIntelligenceQuarantineTag { get; set; }
        public string TargetedDomainProtectionAction { get; set; }
        public string TargetedDomainQuarantineTag { get; set; }
        public string AuthenticationFailAction { get; set; }
        public string SpoofQuarantineTag { get; set; }
        public bool EnableSpoofIntelligence { get; set; }
        public bool EnableViaTag { get; set; }
        public bool EnableUnauthenticatedSender { get; set; }
        public bool EnableSuspiciousSafetyTip { get; set; }
        public bool HonorDmarcPolicy { get; set; }
        public string DmarcRejectAction { get; set; }
        public string DmarcQuarantineAction { get; set; }
        public string PhishThresholdLeveldatatype { get; set; }
        public int PhishThresholdLevel { get; set; }
        public string TargetedUsersToProtectodatatype { get; set; }
        public object[] TargetedUsersToProtect { get; set; }
        public string TargetedUserActionRecipientsodatatype { get; set; }
        public object[] TargetedUserActionRecipients { get; set; }
        public string MailboxIntelligenceProtectionActionRecipientsodatatype { get; set; }
        public object[] MailboxIntelligenceProtectionActionRecipients { get; set; }
        public string TargetedDomainsToProtectodatatype { get; set; }
        public object[] TargetedDomainsToProtect { get; set; }
        public string TargetedDomainActionRecipientsodatatype { get; set; }
        public object[] TargetedDomainActionRecipients { get; set; }
        public string ExcludedDomainsodatatype { get; set; }
        public object[] ExcludedDomains { get; set; }
        public string ExcludedSendersodatatype { get; set; }
        public object[] ExcludedSenders { get; set; }
        public string ExcludedSubDomainsodatatype { get; set; }
        public object[] ExcludedSubDomains { get; set; }
        public bool IsDefault { get; set; }
        public string AdminDisplayName { get; set; }
        public string PolicyTag { get; set; }
        public string RecommendedPolicyType { get; set; }
        public string Identity { get; set; }
        public string Id { get; set; }
        public bool IsValid { get; set; }
        public string ExchangeVersion { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string ObjectCategory { get; set; }
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
    }

}
