using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXOAntiPhishingPolicyEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (AntiPhishPolicy policy in tenant.ExchangeOnlineSettings.AntiPhishPolicies)
            {
                if (policy.Enabled)
                {
                    if (policy.PhishThresholdLevel <= 3 &
                        policy.EnableTargetedUserProtection &
                        policy.EnableOrganizationDomainsProtection &
                        policy.EnableMailboxIntelligence &
                        policy.EnableMailboxIntelligenceProtection &
                        policy.EnableSpoofIntelligence &
                        policy.TargetedUserProtectionAction.ToLower().Equals("quarantine") &
                        policy.TargetedDomainProtectionAction.ToLower().Equals("quarantine") &
                        policy.MailboxIntelligenceProtectionAction.ToLower().Equals("quarantine") &
                        policy.EnableFirstContactSafetyTips &
                        policy.EnableSimilarUsersSafetyTips &
                        policy.EnableSimilarDomainsSafetyTips &
                        policy.EnableUnusualCharactersSafetyTips &
                        policy.HonorDmarcPolicy &
                        policy.TargetedUsersToProtect.Length > 0)
                    {
                        foreach (AntiPhishRule rule in tenant.ExchangeOnlineSettings.AntiPhishRules)
                        {
                            if (rule.Identity == policy.Identity)
                            {
                                if (rule.State.ToLower().Equals("enabled"))
                                {
                                    return CheckResult.NoFinding;
                                }
                            }
                        }
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
