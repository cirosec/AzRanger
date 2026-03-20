using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsExternalCommunicationTrial : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TenantFederationSetting setting in tenant.TeamsSettings.TenantFederationSettings)
            {
                if (setting.Identity.Equals("Global"))
                {
                    if (setting.ExternalAccessWithTrialTenants.ToLower().Equals("blocked"))
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
