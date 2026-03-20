using AzRanger.Models;
using AzRanger.Models.MSGraph;

namespace AzRanger.Checks.Rules
{
    class AzB2BPolicy : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            // If not set it can be null
            if (tenant.TenantSettings.LegacyPolicies != null)
            {
                foreach (LegacyPolicy policy in tenant.TenantSettings.LegacyPolicies) {
                    if (policy.displayName.Equals("B2BManagementPolicy"))
                    {
                        if(policy.B2BManagementPolicyDefinition.B2BManagementPolicy.InvitationsAllowedAndBlockedDomainsPolicy == null || 
                            policy.B2BManagementPolicyDefinition.B2BManagementPolicy.InvitationsAllowedAndBlockedDomainsPolicy.AllowedDomains == null)
                        {
                            return CheckResult.Finding;
                        }
                        if(policy.B2BManagementPolicyDefinition.B2BManagementPolicy.InvitationsAllowedAndBlockedDomainsPolicy.AllowedDomains.Length > 0)
                        return CheckResult.NoFinding;
                    }
                } 
            }
            return CheckResult.Finding;
        }
    }
}
