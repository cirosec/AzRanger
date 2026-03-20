using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsMailIntoChannel : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsClientConfiguration config in tenant.TeamsSettings.TeamsClientConfigurations)
            {
                if (config.Identity.Equals("Global"))
                {
                    if (config.AllowEmailIntoChannel)
                    {
                        return CheckResult.Finding;
                    }
                }
            }
            return CheckResult.NoFinding;
        }
    }
}