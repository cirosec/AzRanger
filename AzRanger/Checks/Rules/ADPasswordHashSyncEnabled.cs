using AzRanger.Models;

namespace AzRanger.Checks.Rules
{
    class ADPasswordHashSyncEnabled : BaseCheck
    {
        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.OrganizationSettings != null)
            {
                if (tenant.OrganizationSettings.onPremisesSyncEnabled != null && (bool)tenant.OrganizationSettings.onPremisesSyncEnabled == true)
                {
                    return CheckResult.NoFinding;
                }
                return CheckResult.Finding;
            }
            else {                 
                this.SetReason("Organization settings not found, could not determine if AD Password Hash Sync is enabled.");
                return CheckResult.Error; 
            }
        }
    }
}
