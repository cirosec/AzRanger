using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ExchangeOnline
{
   public class SafeAttachmentPolicy
    {
        public string Action { get; set; }
        public string AdminDisplayName { get; set; }
        public bool Enable { get; set; }
        public bool EnableBlockingEncryptedAttachments { get; set; }
        public bool EnableOrganizationBranding { get; set; }
        public string ExcludedTypesFromBlockingEncryptedAttachmentsodatatype { get; set; }
        public object[] ExcludedTypesFromBlockingEncryptedAttachments { get; set; }
        public bool IsBuiltInProtection { get; set; }
        public bool IsDefault { get; set; }
        public string QuarantineTag { get; set; }
        public string QuarantineTagForBlockingEncryptedAttachments { get; set; }
        public string RecommendedPolicyType { get; set; }
        public bool Redirect { get; set; }
        public string RedirectAddress { get; set; }
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
