using AzRanger.Models;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class AzB2BCollaborationDomainRestriction : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.LegacyPolicies == null)
            {
                return CheckResult.Finding;
            }

            var b2bPolicy = tenant.TenantSettings.LegacyPolicies
                .FirstOrDefault(p => p.type == "B2BManagementPolicy");

            if (b2bPolicy == null)
            {
                return CheckResult.Finding;
            }

            var allowedDomains = b2bPolicy.ParsedDefinition?
                .B2BManagementPolicy?
                .InvitationsAllowedAndBlockedDomainsPolicy?
                .AllowedDomains;

            if (allowedDomains != null && allowedDomains.Length > 0)
            {
                return CheckResult.NoFinding;
            }

            return CheckResult.Finding;
        }
    }
}