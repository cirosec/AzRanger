using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ExchangeOnline
{
    public class SafeAttachmentRule
    {
        public string SafeAttachmentPolicy { get; set; }
        public string State { get; set; }
        public string Prioritydatatype { get; set; }
        // Priority 0 is the highest
        public int Priority { get; set; }
        public object Comments { get; set; }
        public string Description { get; set; }
        public string RuleVersion { get; set; }
        public string SentToodatatype { get; set; }
        public string[] SentTo { get; set; }
        public object SentToMemberOf { get; set; }
        public object RecipientDomainIs { get; set; }
        public object ExceptIfSentTo { get; set; }
        public object ExceptIfSentToMemberOf { get; set; }
        public object ExceptIfRecipientDomainIs { get; set; }
        public string Conditionsodatatype { get; set; }
        public string[] Conditions { get; set; }
        public object Exceptions { get; set; }
        public string Identity { get; set; }
        public string DistinguishedName { get; set; }
        public string Guiddatatype { get; set; }
        public string Guidodatatype { get; set; }
        public string Guid { get; set; }
        public string ImmutableIddatatype { get; set; }
        public string ImmutableIdodatatype { get; set; }
        public string ImmutableId { get; set; }
        public string OrganizationId { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public string WhenChangeddatatype { get; set; }
        public DateTime WhenChanged { get; set; }
    }

}
