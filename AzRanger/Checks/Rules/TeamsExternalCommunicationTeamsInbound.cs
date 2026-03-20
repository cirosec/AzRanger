using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsExternalCommunicationInbound : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TenantFederationSetting setting in tenant.TeamsSettings.TenantFederationSettings)
            {
                if (setting.Identity.Equals("Global"))
                {
                    if (setting.AllowTeamsConsumerInbound == false)
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
