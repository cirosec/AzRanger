using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXOPriorirtyProtectionEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach(EmailTenantSettings setting in tenant.ExchangeOnlineSettings.EmailTenantSettings)
            {
                if (setting.EnablePriorityAccountProtection)
                {
                    return CheckResult.NoFinding;
                }
            }
            return CheckResult.Finding;
        }
    }
}
