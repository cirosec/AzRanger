using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class SPODisallowInfectedFileDownload : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.SharePointInformation.SharePointInternalInfos.DisallowInfectedFileDownload)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
