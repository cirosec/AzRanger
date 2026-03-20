using AzRanger.Models;
using AzRanger.Models.Teams;

namespace AzRanger.Checks.Rules
{
    class TeamsExternalSharingProvider : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            foreach (TeamsClientConfiguration config in tenant.TeamsSettings.TeamsClientConfigurations)
            {
                if (config.Identity.Equals("Global"))
                {
                    if (config.AllowBox == false &&
                        config.AllowDropBox == false &&
                        config.AllowEgnyte == false &&
                        config.AllowGoogleDrive == false &&
                        config.AllowShareFile == false)
                    {
                        return CheckResult.NoFinding;
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
