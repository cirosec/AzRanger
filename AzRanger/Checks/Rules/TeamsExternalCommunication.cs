using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    // TODO
    class TeamsExternalCommunication : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach(TenantFederationSetting setting in tenant.TeamsSettings.TenantFederationSettings)
            {
                if (setting.Identity.Equals("Global"))
                {
                    if (setting.AllowPublicUsers == false && setting.AllowTeamsConsumer == false)
                    {

                        if (setting.AllowFederatedUsers == false)
                        {
                            return CheckResult.NoFinding;
                        }
                        else
                        {
                            AllowedDomain[] allowedDOmains = (AllowedDomain[])setting.AllowedDomains;
                            if (allowedDOmains != null && allowedDOmains.Length > 0)
                            {
                                return CheckResult.NoFinding;
                            }
                        }
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}