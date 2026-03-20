using AzRanger.Models;
using AzRanger.Models.Generic;
using AzRanger.Models.MSGraph;
using AzRanger.Utilities;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class UserHighPrivUsePIM : BaseCheck
    {


        public override CheckResult Audit(Tenant tenant)
        {
            if (!tenant.HasP2License)
            {
                SetReason("Has no P2 license");
                return CheckResult.NotApplicable;
            }
            bool passed = true;
            foreach (DirectoryRole role in tenant.DirectoryRoles.Values.ToList())
            {
                if (DirectoryRoleTemplateID.HighPrivRoles.Contains(role.id.ToString()))
                {
                    if(role.eligibleMembers.Count == 0 && role.activeMembers.Count > 0)
                    {
                        passed = false;
                        AddAffectedEntity(role);
                    }
                }
            }

            if (passed)
            {
                return CheckResult.NoFinding;
            }
            return CheckResult.Finding;
        }
    }
}
