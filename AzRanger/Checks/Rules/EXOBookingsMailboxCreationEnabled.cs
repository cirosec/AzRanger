using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class EXOBookingsMailboxCreationEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            // No finding if Bookings is disabled at the organization level
            if (tenant.ExchangeOnlineSettings.OrganizationConfig.BookingsEnabled == false)
            {
                return CheckResult.NoFinding;
            }
            // No finding if BookingsMailboxCreationEnabled is disabled in OwaMailboxPolicy
            if (tenant.ExchangeOnlineSettings.OwaMailboxPolicy.BookingsMailboxCreationEnabled == false)
            {
                return CheckResult.NoFinding;
            }
            // Finding if BookingsMailboxCreationEnabled is true
            return CheckResult.Finding;
        }
    }
}