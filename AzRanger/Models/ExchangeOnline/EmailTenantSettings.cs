using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ExchangeOnline
{
    public class EmailTenantSettings
    {
        public bool EnablePriorityAccountProtection { get; set; }
        public string Identity { get; set; }
        public string Id { get; set; }
        public bool IsValid { get; set; }
        public string ExchangeVersion { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public object ObjectCategory { get; set; }
        public string ObjectClassodatatype { get; set; }
        public string[] ObjectClass { get; set; }
        public object WhenChanged { get; set; }
        public object WhenCreated { get; set; }
        public object WhenChangedUTC { get; set; }
        public object WhenCreatedUTC { get; set; }
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
