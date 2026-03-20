using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class SPOCustomScriptExecution : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            bool passed = true;
            if (tenant.SharePointInformation.SPOPages != null)
            {
                foreach (var page in tenant.SharePointInformation.SPOPages)
                {
                    if (!page.Path.Contains("-my.sharepoint.com") & page.Properties.DenyAddAndCustomizePages == 1)
                    {
                        passed = false;
                        AddAffectedEntity(page);
                    }
                }
            }
            if(passed)
            {
                return CheckResult.NoFinding;
            }
            else
            {
                SetReason("Custom Script Execution is enabled on the following sites");
                return CheckResult.Finding;
            }
        }
    }
}
