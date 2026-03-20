using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsBypassLobby : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsMeetingPolicy policy in tenant.TeamsSettings.TeamsMeetingPolicies)
            {
                if (policy.Identity.Equals("Global"))
                {
                    if (policy.AutoAdmittedUsers.ToLower().Equals("everyoneincompanyexcludingguests") |
                        policy.AutoAdmittedUsers.ToLower().Equals("organizeronly"))
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}