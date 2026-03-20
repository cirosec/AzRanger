using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class OfficeCalenderSharing : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.AdminCenterSettings.CalendarSharing.EnableCalendarSharing == false)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
