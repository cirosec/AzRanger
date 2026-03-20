using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;
using AzRanger.Utilities;

namespace AzRanger.Checks.Rules
{
    class EXOSafeAttachmentForM365Enabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach(AtpPolicyForO365 policy in tenant.ExchangeOnlineSettings.AtpPolicyForO365s)
            {
                if(policy.EnableATPForSPOTeamsODB &
                    policy.EnableSafeDocs &
                    policy.AllowSafeDocsOpen)
                {
                    return CheckResult.NoFinding;
                }
            }
            return CheckResult.Finding;
        }
    }
}
