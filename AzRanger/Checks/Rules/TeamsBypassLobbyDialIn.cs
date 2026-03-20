using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsBypassLobbyDialIn : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsMeetingPolicy policy in tenant.TeamsSettings.TeamsMeetingPolicies)
            {
                if (policy.Identity.Equals("Global"))
                {
                    if (policy.AllowPSTNUsersToBypassLobby == false)
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}