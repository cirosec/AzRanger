using AzRanger.AzScanner;
using AzRanger.Models;
using System.Threading.Tasks;

namespace AzRanger.Utilities.EnrichmentEngine
{
    public static class AssignUserToRole
    {
        public static async Task<bool> Enrich(Tenant tenant, MSGraphCollector collector)
        {
            await RoleAssignmentHelper.ProcessAssignments(
                tenant, collector,
                role => role.pimRoleAssignments,
                (role, p) => role.AddActiveMember(p),
                (role, t) => role.AddActiveMemberScopes(t),
                nameof(AssignUserToRole));
            return true;
        }
    }
}
