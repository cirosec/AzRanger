using AzRanger.Models;
using AzRanger.Models.Generic;
using AzRanger.Models.MSGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzRanger.Utilities.EnrichmentEngine
{
    public static class AssignUserCanAddCreds
    {
        public static void Enrich(Tenant tenant)
        {
            foreach (DirectoryRole role in tenant.DirectoryRoles.Values)
            {
                if (!DirectoryRoleTemplateID.RolesAllowingAddCreds.Contains(role.roleTemplateId))
                {
                    continue;
                }

                // Unscoped members (active + eligible) can add creds to ALL apps/SPs
                foreach (AzurePrincipal principal in role.activeMembers.Concat(role.eligibleMembers))
                {
                    foreach (Application a in tenant.Applications.Values)
                    {
                        a.AddUserAbleToAddCreds(principal);
                    }
                    foreach (ServicePrincipal s in tenant.ServicePrincipals.Values)
                    {
                        if (s.appOwnerOrganizationId == tenant.TenantId)
                        {
                            s.AddUserAbleToAddCreds(principal);
                        }
                    }
                }

                // Scoped members (active + eligible) can only add creds to their scoped target
                foreach (Tuple<AzurePrincipal, AzurePrincipal> entry in role.activeMembersScoped.Concat(role.eligibleMembersScoped))
                {
                    if (entry.Item2.PrincipalType == AzurePrincipalType.Application)
                    {
                        tenant.Applications[entry.Item2.id].AddUserAbleToAddCreds(new AzurePrincipal(entry.Item1.id, entry.Item1.PrincipalType));
                    }
                    if (entry.Item2.PrincipalType == AzurePrincipalType.ServicePrincipal)
                    {
                        tenant.ServicePrincipals[entry.Item2.id].AddUserAbleToAddCreds(new AzurePrincipal(entry.Item1.id, entry.Item1.PrincipalType));
                    }
                }
            }
        }
    }
}
