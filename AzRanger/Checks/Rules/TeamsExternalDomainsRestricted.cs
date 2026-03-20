using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsExternalDomainsRestricted : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsExternalPolicy policy in tenant.TeamsSettings.TeamsExternalPolicies)
            {
                if (policy.Identity.ToLower().Equals("global"))
                {
                    if (!policy.EnableFederationAccess)
                        return CheckResult.NoFinding;
                }
            }
            foreach (TenantFederationSetting setting in tenant.TeamsSettings.TenantFederationSettings)
            {
                if (setting.Identity.Equals("Global"))
                {
                    AllowedDomain[] allowedDOmains = (AllowedDomain[])setting.AllowedDomains;
                    if (!setting.AllowFederatedUsers ||
                        (setting.AllowFederatedUsers &
                        allowedDOmains != null && 
                        allowedDOmains.Length > 0)
                        )
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
