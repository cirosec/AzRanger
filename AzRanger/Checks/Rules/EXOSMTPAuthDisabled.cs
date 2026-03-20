using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class EXOSMTPAuthDisabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            bool pass = true;
            foreach (var config in tenant.ExchangeOnlineSettings.TransportConfig)
            {
                if (!config.SmtpClientAuthenticationDisabled)
                {
                    pass = false;
                    this.AddAffectedEntity(config);
                }
            }
            if (pass)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
