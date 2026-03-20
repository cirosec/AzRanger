using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;

namespace AzRanger.Checks.Rules
{
    class EXOSafeLinksEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.ExchangeOnlineSettings.SafeLinksPolicies != null)
            {
                foreach (SafeLinksPolicy policy in tenant.ExchangeOnlineSettings.SafeLinksPolicies)
                {
                    if (policy.EnableSafeLinksForEmail &
                        policy.EnableSafeLinksForTeams &
                        policy.EnableSafeLinksForOffice &
                        policy.TrackClicks &
                        !policy.AllowClickThrough &
                        policy.ScanUrls &
                        policy.EnableForInternalSenders &
                        policy.DeliverMessageAfterScan &
                        !policy.DisableUrlRewrite)
                    {
                        return CheckResult.NoFinding;
                    }
                }
                return CheckResult.Finding;
            }
            else
            {
                SetReason("SafeLink policies not found.");
                return CheckResult.Error;
            }
        }
    }
}
