using AzRanger.Models;
using System.Text.Json;

namespace AzRanger.Checks.Rules
{
    class SPOSharingExternally : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.SharePointInformation.SharePointInternalInfos.SharingCapability == 0)
            {
                return CheckResult.NoFinding;
            }
            if (tenant.SharePointInformation.SharePointInternalInfos.SharingDomainRestrictionMode != 0)
            {
                var domains = tenant.SharePointInformation.SharePointInternalInfos.SharingAllowedDomainList;
                if (domains != null && domains is JsonElement elem)
                {
                    bool hasDomains = (elem.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(elem.GetString()))
                                   || (elem.ValueKind == JsonValueKind.Array && elem.GetArrayLength() > 0);
                    if (hasDomains)
                        return CheckResult.NoFinding;
                }
            }
            return CheckResult.Finding;
        }
    }
}
