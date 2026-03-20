using AzRanger.Models;
using AzRanger.Models.ComplianceCenter;

namespace AzRanger.Checks.Rules
{
    class OfficeDLPLabels : BaseCheck
    {
        // TODO: Maybe we can check if they makes sense
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.DlpLabels != null)
            {
                foreach (DlpLabelPolicy policy in tenant.TenantSettings.DlpLabelPolicies)
                {
                    if (policy.Enabled & policy.Type.ToLower().Equals("publishedsensitivitylabel"))
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
