using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXOTeamsZAPEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {

            // We should only have one policy
            if (tenant.ExchangeOnlineSettings.TeamsProtectionPolicies[0].ZapEnabled)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
