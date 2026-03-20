using AzRanger.AzScanner;
using AzRanger.Models;
using AzRanger.Models.Generic;
using AzRanger.Models.MSGraph;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzRanger.Utilities.EnrichmentEngine
{
    internal static class RoleAssignmentHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal static async Task ProcessAssignments(
            Tenant tenant,
            MSGraphCollector collector,
            Func<DirectoryRole, List<DirectoryRoleAssignment>> getAssignments,
            Action<DirectoryRole, AzurePrincipal> addMember,
            Action<DirectoryRole, Tuple<AzurePrincipal, AzurePrincipal>> addMemberScoped,
            string callerName)
        {
            foreach (DirectoryRole role in tenant.DirectoryRoles.Values)
            {
                var assignments = getAssignments(role);
                if (assignments == null) continue;

                foreach (DirectoryRoleAssignment assignment in assignments)
                {
                    if (assignment.principal == null || assignment.principal.odatatype == null)
                    {
                        continue;
                    }

                    AzurePrincipalType aztype;
                    switch (assignment.principal.odatatype)
                    {
                        case ODataTypes.User:
                            aztype = AzurePrincipalType.User;
                            break;
                        case ODataTypes.ServicePrincipal:
                            aztype = AzurePrincipalType.ServicePrincipal;
                            break;
                        case ODataTypes.Application:
                            aztype = AzurePrincipalType.Application;
                            break;
                        case ODataTypes.Group:
                            aztype = AzurePrincipalType.Group;
                            break;
                        default:
                            continue;
                    }

                    List<AzurePrincipal> principals = new List<AzurePrincipal>();
                    if (aztype == AzurePrincipalType.Group)
                    {
                        principals.AddRange(await collector.GetAllGroupMemberTransitiv(Guid.Parse(assignment.principalId)));
                    }
                    else
                    {
                        principals.Add(new AzurePrincipal(Guid.Parse(assignment.principalId), aztype));
                    }

                    if (assignment.directoryScopeId == null || assignment.directoryScopeId.Equals("/"))
                    {
                        foreach (AzurePrincipal p in principals)
                        {
                            addMember(role, p);
                        }
                    }
                    else
                    {
                        if (assignment.directoryScopeId.Length < 2 || !Guid.TryParse(assignment.directoryScopeId.Substring(1), out Guid scopeId) || scopeId == Guid.Empty)
                        {
                            logger.Warn("[-] {0}: Pim Assignment invalid scopeId: {1}", callerName, assignment.directoryScopeId);
                            continue;
                        }
                        if (assignment.directoryScope == null || assignment.directoryScope.odatatype == null)
                        {
                            logger.Warn("[-] {0}: Pim Assignment has null directoryScope, scopeId={1}", callerName, assignment.directoryScopeId);
                            continue;
                        }
                        if (assignment.directoryScope.odatatype.Equals(ODataTypes.Application))
                        {
                            foreach (AzurePrincipal p in principals)
                            {
                                addMemberScoped(role, new Tuple<AzurePrincipal, AzurePrincipal>(p, new AzurePrincipal(scopeId, AzurePrincipalType.Application)));
                            }
                            continue;
                        }
                        if (assignment.directoryScope.odatatype.Equals(ODataTypes.ServicePrincipal))
                        {
                            if (tenant.ServicePrincipals.TryGetValue(scopeId, out ServicePrincipal sp) && sp.appOwnerOrganizationId == tenant.TenantId)
                            {
                                foreach (AzurePrincipal p in principals)
                                {
                                    addMemberScoped(role, new Tuple<AzurePrincipal, AzurePrincipal>(p, new AzurePrincipal(scopeId, AzurePrincipalType.ServicePrincipal)));
                                }
                            }
                            continue;
                        }
                        logger.Warn("[-] {0}: Pim Assignment unknown scope: {1}", callerName, assignment.directoryScope.odatatype);
                    }
                }
            }
        }
    }
}
