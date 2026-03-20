using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsCanUserReportConcerns : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsMessagingPolicy policy in tenant.TeamsSettings.TeamsMessagePolicies)
            {
                if (policy.Identity.Equals("Global"))
                {
                    if (policy.AllowSecurityEndUserReporting)
                    {
                        return CheckResult.NoFinding;
                    }
                } 
            }
            return CheckResult.Finding;
        }
    }
}