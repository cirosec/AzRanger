using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXOOutboundAntiSpamLimits : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (HostedOutboundSpamFilterPolicy policy in tenant.ExchangeOnlineSettings.HostedOutboundSpamFilterPolicy)
            {
                if (policy.Enabled &
                    policy.RecipientLimitExternalPerHour < 500 & 
                    policy.RecipientLimitInternalPerHour < 1000 &
                    policy.RecipientLimitPerDay < 1000 &
                    policy.ActionWhenThresholdReached.ToLower().Equals("blockuser"))
                {
                    return CheckResult.NoFinding;
                }
            }
            return CheckResult.Finding;
        }
    }
}
