using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    // Credits to https://github.com/soteria-security/365Inspect/blob/main/Inspectors/BypassingSafeAttachments.ps1    
    class EXOSafeAttachmentPolicyEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.ExchangeOnlineSettings.SafeAttachmentPolicies != null & tenant.ExchangeOnlineSettings.SafeAttachmentRules != null)
            {
                string policyToCheckIdentity = "Built-In Protection Policy";
                if (tenant.ExchangeOnlineSettings.SafeAttachmentRules.Count > 0) {
                    // Find the rule with the highest priority which is enabled. 
                    SafeAttachmentRule ruleLowestPrio = null;
                    foreach (SafeAttachmentRule rule in tenant.ExchangeOnlineSettings.SafeAttachmentRules)
                    {
                        if (ruleLowestPrio == null)
                        {
                            ruleLowestPrio = rule;
                        }
                        else
                        {
                            if (rule.State.ToLower().Equals("enabled"))
                            {
                                if (rule.Priority < ruleLowestPrio.Priority)
                                {
                                    ruleLowestPrio = rule;
                                }
                            }
                        }
                    }
                    if (ruleLowestPrio != null)
                    {
                        policyToCheckIdentity = ruleLowestPrio.Identity;
                    }
                }
                foreach(SafeAttachmentPolicy policy in tenant.ExchangeOnlineSettings.SafeAttachmentPolicies)
                {
                    if (policyToCheckIdentity.Equals(policy.Identity))
                    {
                        if (policy.Enable &
                            policy.Action.ToLower().Equals("block") &
                            policy.QuarantineTag.ToLower().Equals("adminonlyaccesspolicy"))
                        {
                            return CheckResult.NoFinding;
                        }
                        else
                        {
                            return CheckResult.Finding;
                        }
                    }
                }
                SetReason("Attachment policies not found - Should not happen");
                return CheckResult.Error;

            }
            else
            {
                SetReason("Attachment policies or rules not found");
                return CheckResult.Error;
            }

        }
    }
}
