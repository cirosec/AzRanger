using AzRanger.Models;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class GroupForGuests : BaseCheck
    {

        public override CheckResult Audit(Tenant tenant)
        {
            foreach (Group group in tenant.Groups.Values.ToList())
            {
                if (group.groupTypes.Length > 0)
                {
                    foreach (string type in group.groupTypes)
                    {
                        if (type.ToLower().Equals("dynamicmembership"))
                        {
                            if(group.membershipRule.ToLower().Contains("user.userType -eq \"guest\""))
                            {
                                return CheckResult.NoFinding;
                            }
                        }
                    }
                }
            }
            return CheckResult.Finding;
        }
    }
}
