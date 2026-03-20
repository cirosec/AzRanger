using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsDisableChatAnonymUsers : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsMeetingPolicy policy in tenant.TeamsSettings.TeamsMeetingPolicies)
            {
                if (policy.Identity.Equals("Global"))
                {
                    if (policy.MeetingChatEnabledType.ToLower().Equals("enabledexceptanonymous"))
                    {
                        return CheckResult.NoFinding;
                    }   
                }
            }
            return CheckResult.Finding;
        }
    }
}