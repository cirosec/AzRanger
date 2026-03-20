using AzRanger.AzScanner;
using AzRanger.Models;
using System.Threading.Tasks;

namespace AzRanger.Utilities.EnrichmentEngine
{
    public static class AssignEligibleUserToRole
    {
        public static async Task<bool> Enrich(Tenant tenant, MSGraphCollector collector)
        {
            await RoleAssignmentHelper.ProcessAssignments(
                tenant, collector,
                role => role.pimRoleAssignmentsEligible,
                (role, p) => role.AddEligibleMember(p),
                (role, t) => role.AddEligibleMemberScoped(t),
                nameof(AssignEligibleUserToRole));
            return true;
        }
    }
}
