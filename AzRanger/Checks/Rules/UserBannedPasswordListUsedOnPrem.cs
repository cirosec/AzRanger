using AzRanger.Models;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class UserBannedPasswordListUsedOnPrem : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.TenantSettings.GroupSettings == null)
            {
                return CheckResult.Finding;
            }

            var pwRuleSetting = tenant.TenantSettings.GroupSettings
                .FirstOrDefault(s => s.templateId == "5cf42378-d67d-4f36-ba46-e8b86229381d");

            if (pwRuleSetting == null)
            {
                return CheckResult.Finding;
            }

            // Check if EnableBannedPasswordCheckOnPremises is True
            var enableBannedCheckOnPrem = pwRuleSetting.values?
                .FirstOrDefault(v => v.name == "EnableBannedPasswordCheckOnPremises");

            if (enableBannedCheckOnPrem == null || enableBannedCheckOnPrem.value != "True")
            {
                return CheckResult.Finding;
            }

            // Check if BannedPasswordCheckOnPremisesMode is set to Enforce
            var bannedPasswordCheckOnPremisesMode = pwRuleSetting.values?
                .FirstOrDefault(v => v.name == "BannedPasswordCheckOnPremisesMode");

            if (bannedPasswordCheckOnPremisesMode == null || bannedPasswordCheckOnPremisesMode.value != "Enforce")
            {
                return CheckResult.Finding;
            }

            return CheckResult.NoFinding;
        }
    }
}