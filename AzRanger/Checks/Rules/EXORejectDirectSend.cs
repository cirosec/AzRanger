using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXORejectDirectSend : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.ExchangeOnlineSettings.OrganizationConfig.RejectDirectSend)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
