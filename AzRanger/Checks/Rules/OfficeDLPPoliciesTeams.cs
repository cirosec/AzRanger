using AzRanger.Models;
using AzRanger.Models.ComplianceCenter;

namespace AzRanger.Checks.Rules
{
    class OfficeDLPPoliciesTeams : BaseCheck
    {
        // TODO: Maybe we can check if we can do better
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.OfficeDLPPolicies != null)
            {
                foreach (DlpCompliancePolicy policy in tenant.TenantSettings.OfficeDLPPolicies)
                {
                    if (policy.Enabled & policy.Workload.ToLower().Contains("teams"))
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
