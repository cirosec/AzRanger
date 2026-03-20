using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsExternalCommunicationWithConsumer : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TenantFederationSetting setting in tenant.TeamsSettings.TenantFederationSettings)
            {
                if (setting.Identity.Equals("Global"))
                {
                    if (setting.AllowTeamsConsumer == false)
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
