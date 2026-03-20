using AzRanger.Models;
using AzRanger.Models.AzMgmt;

namespace AzRanger.Checks.Rules
{
    internal class AzActNSGSUnrestrictedDestinationPort : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            bool passed = true;

            foreach (Subscription sub in tenant.Subscriptions.Values)
            {

                foreach (NetworkSecurityGroup nsg in sub.Resources.NetworkSecurityGroups)
                {
                    foreach (NetworkSecurityGroupSecurityRule rule in nsg.properties.securityRules)
                    {
                        if (rule.properties.direction == "Inbound")
                        {
                            if (rule.properties.destinationPortRange == "*")
                            {
                                passed = false;
                                this.AddAffectedEntity(nsg);
                            }
                        }
                    }
                }
            }

            if (passed)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
