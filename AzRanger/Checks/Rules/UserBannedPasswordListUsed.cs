using AzRanger.Models;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class UserBannedPasswordListUsed : BaseCheck
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

            // Check if EnableBannedPasswordCheck is True
            var enableBannedCheck = pwRuleSetting.values?
                .FirstOrDefault(v => v.name == "EnableBannedPasswordCheck");

            if (enableBannedCheck == null || enableBannedCheck.value != "True")
            {
                return CheckResult.Finding;
            }

            // Check if BannedPasswordList is populated
            var bannedPasswordList = pwRuleSetting.values?
                .FirstOrDefault(v => v.name == "BannedPasswordList");

            if (bannedPasswordList == null || string.IsNullOrWhiteSpace(bannedPasswordList.value))
            {
                return CheckResult.Finding;
            }

            return CheckResult.NoFinding;
        }
    }
}