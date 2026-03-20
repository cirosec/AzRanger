using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsRecordingIsOff : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsMeetingPolicy policy in tenant.TeamsSettings.TeamsMeetingPolicies)
            {
                if (policy.Identity.Equals("Global"))
                {
                    if (!policy.AllowCloudRecording)
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}