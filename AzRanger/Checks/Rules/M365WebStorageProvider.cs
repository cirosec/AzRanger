using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class M365WebStorageProvider : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.OfficeOnline != null && tenant.TenantSettings.OfficeOnline.Enabled == false)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
