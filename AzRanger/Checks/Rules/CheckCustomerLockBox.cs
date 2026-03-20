using AzRanger.Models;
using AzRanger.Models.MSGraph;

namespace AzRanger.Checks.Rules
{

    class CheckCustomerLockBox : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.ExchangeOnlineSettings.OrganizationConfig.CustomerLockboxEnabled)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
