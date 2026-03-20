using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class ODBSharing : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.SharePointInformation.SharePointInternalInfos.ODBSharingCapability == 0)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
