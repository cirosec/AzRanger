using AzRanger.Checks;
using AzRanger.Models;
using AzRanger.Models.AdminCenter;
using AzRanger.Models.AzMgmt;
using AzRanger.Models.ComplianceCenter;
using AzRanger.Models.ExchangeOnline;
using AzRanger.Models.Generic;
using AzRanger.Models.MainIAM;
using AzRanger.Models.MSGraph;
using AzRanger.Models.Provision;
using AzRanger.Models.Teams;
using AzRanger.Utilities;
using AzRanger.Utilities.EnrichmentEngine;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public class MainCollector
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal string Username { get; }
        internal String Proxy { get; }

        internal IAuthenticator AADPowerShellAuthenticator;
        internal IAuthenticator PowerAutomateAuthenticator;
        internal IAuthenticator SPOMgmtShellAuthenticator;
        internal IAuthenticator SPOMGMShell;
        internal String Domain;
        internal String TenantId;
        internal bool HasP1License = false;
        internal bool HasP2License = false;


        internal AdminCenterCollector AdminCenterCollector;
        internal MSGraphCollector MSGraphCollector;
        internal ProvisionAPICollector ProvisionAPICollector;
        internal ExchangeOnlineCollector ExchangeOnlineScanner;
        internal MainIamCollector MainIamCollector;
        internal ComplianceCenterCollector ComplianceCenterScanner;
        internal GraphWinCollector GraphWinCollector;
        internal TeamsCollector TeamsCollector;
        internal AzMgmtCollector AzMgmtCollector;

        private MainCollector(IAuthenticator aadPowerShellUserAuthenticator, IAuthenticator powerAutomateUserAuthenticator, IAuthenticator spoMgmtShellAuthenticator, String proxy, String tenant, String username)
        {
            this.Proxy = proxy;
            this.TenantId = tenant;
            this.AADPowerShellAuthenticator = aadPowerShellUserAuthenticator;
            this.PowerAutomateAuthenticator = powerAutomateUserAuthenticator;
            this.SPOMgmtShellAuthenticator = spoMgmtShellAuthenticator;
            this.Username = username;
        }

        public static async Task<MainCollector> CreateAsync(IAuthenticator aadPowerShellUserAuthenticator, IAuthenticator powerAutomateUserAuthenticator, IAuthenticator spoMgmtShellAuthenticator, String proxy, String tenant = null)
        {
            String tenantId = tenant ?? await aadPowerShellUserAuthenticator.GetTenantId();
            String username = await aadPowerShellUserAuthenticator.GetUsername();
            return new MainCollector(aadPowerShellUserAuthenticator, powerAutomateUserAuthenticator, spoMgmtShellAuthenticator, proxy, tenantId, username);
        }

        public async Task<Tenant> ScanTenant(List<ScopeEnum> scopes)
        {
            if (this.TenantId == null)
            {
                logger.Warn("Scanner.ScanTenant: Cannot retrieve TenantId. Aborting!");
                return null;
            }

            Tenant Result = new Tenant
            {
                TenantId = this.TenantId,
                Username = this.Username
            };
            String currentUserId = await this.AADPowerShellAuthenticator.GetUserId();
            bool isGlobalAdmin = false;
            bool isGlobalReader = false;
            bool isSharePointAdmin = false;
            bool scanAzureOnly = false;
            Task redirectionTask = null;

            AdminCenterCollector = new AdminCenterCollector(AADPowerShellAuthenticator, Proxy);
            MSGraphCollector = new MSGraphCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            ProvisionAPICollector = new ProvisionAPICollector(AADPowerShellAuthenticator, TenantId, Proxy);
            ExchangeOnlineScanner = new ExchangeOnlineCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            MainIamCollector = new MainIamCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            ComplianceCenterScanner = new ComplianceCenterCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            GraphWinCollector = new GraphWinCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            TeamsCollector = new TeamsCollector(AADPowerShellAuthenticator, TenantId, Proxy);
            AzMgmtCollector = new AzMgmtCollector(AADPowerShellAuthenticator, TenantId, Proxy);

            if (scopes.Count == 1 && scopes.Contains(ScopeEnum.Azure))
            {
                scanAzureOnly = true;
            }

            Result.TenantSettings = new M365Settings();
            if (!scanAzureOnly)
            {
                if (this.AADPowerShellAuthenticator.IsUserContext)
                {
                    Result.DirectoryRoles = await MSGraphCollector.GetAllDirectoryRoles();
                    if ((Result.DirectoryRoles != null && currentUserId != null))
                    {
                        Console.WriteLine("[+] You are using {0} roles in your tenant", Result.DirectoryRoles.Count);
                        foreach (DirectoryRole role in Result.DirectoryRoles.Values)
                        {
                            if (role.roleTemplateId == DirectoryRoleTemplateID.GlobalAdministrator)
                            {
                                if (role.PricipalIsInActiveMembers(Guid.Parse(currentUserId)))
                                {
                                    isGlobalAdmin = true;
                                }
                            }

                            if (role.roleTemplateId == DirectoryRoleTemplateID.GlobalReader)
                            {
                                if (role.PricipalIsInActiveMembers(Guid.Parse(currentUserId)))
                                {
                                    isGlobalReader = true;
                                }
                            }

                            if (role.roleTemplateId == DirectoryRoleTemplateID.SharePointAdministrator)
                            {
                                if (role.PricipalIsInActiveMembers(Guid.Parse(currentUserId)))
                                {
                                    isSharePointAdmin = true;
                                }
                            }
                        }
                        if (!isGlobalAdmin && !isGlobalReader)
                        {
                            Console.WriteLine("[-] The current user has not sufficient rights, please choose another one.");
                            return null;
                        }
                        else
                        {
                            Console.WriteLine("[+] Current user has sufficient rights, continue...");
                        }

                        if (!isGlobalAdmin)
                        {
                            if (!isSharePointAdmin)
                            {
                                Console.WriteLine("[-] The current user is no SharePointAdmin, so it cannot read data from SharePoint.");
                            }
                        }
                    }
                    else
                    {
                        logger.Debug("Scanner.ScanTenant: Cannot get User Id. Should not happen!");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("[+] Running AzRanger with a service principal.... Good luck!");
                }
            }

            if (scopes.Contains(ScopeEnum.AAD))
            {

                Result.TenantSettings.TenantSkuInfo = await MainIamCollector.GetTenantSkuInfo();
                if (Result.TenantSettings.TenantSkuInfo != null)
                {
                    if (Result.TenantSettings.TenantSkuInfo.aadPremium)
                    {
                        Console.WriteLine("[+] Tenant has a P1 license.");
                        this.HasP1License = true;
                        Result.HasP1License = true;
                    }
                    else
                    {
                        Console.WriteLine("[-] Tenant has no P1 license. Not all data can be gathered.");
                    }

                    if (Result.TenantSettings.TenantSkuInfo.aadPremiumP2)
                    {
                        Console.WriteLine("[+] Tenant has a P2 license.");
                        this.HasP2License = true;
                        Result.HasP2License = true;
                    }
                    else
                    {
                        Console.WriteLine("[-] Tenant has no P2 license. Not all data can be gathered.");
                    }
                }
                else
                {
                    logger.Warn("Scanner.ScanTenant: Cannot get Tenant License. This should not happen.");
                    return null;
                }

                if (Result.DirectoryRoles == null)
                {
                    Task<Dictionary<Guid, DirectoryRole>> getAllDirectoryRoles = MSGraphCollector.GetAllDirectoryRoles(); ;
                    Result.DirectoryRoles = await getAllDirectoryRoles;
                    if (Result.DirectoryRoles != null)
                    {
                        Console.WriteLine("[+] {0} used roles found in the tenant", Result.DirectoryRoles.Count);
                    }
                }
                Task<List<Domain>> getDomainTask = MSGraphCollector.GetAzDomains();
                Result.Domains = await getDomainTask;
                if (Result.Domains != null)
                {
                    Console.WriteLine("[+] {0} domains found in the tenant", Result.Domains.Count);
                }
                Task<Dictionary<Guid, User>> getUserTask = MSGraphCollector.GetAllUsers(HasP1License);
                Result.Users = await getUserTask;
                if (Result.Users != null)
                {
                    Console.WriteLine("[+] {0} users found in the tenant", Result.Users.Count);
                }
                Task<Dictionary<Guid, User>> getGuestTask = MSGraphCollector.GetAllGuests(HasP1License);
                Result.Guests = await getGuestTask;
                if (Result.Guests != null)
                {
                    Console.WriteLine("[+] {0} guests found in the tenant", Result.Guests.Count);
                }
                Task<Dictionary<Guid, Application>> getAllApplications = MSGraphCollector.GetAllApplications();
                Result.Applications = await getAllApplications;
                
                if (Result.Applications != null)
                {

                    Console.WriteLine("[+] {0} applications found in the tenant", Result.Applications.Count);
                    try
                    {
                        redirectionTask = CheckIfRedirectUriExist.Enrich(Result);
                    }
                    catch (Exception ex) {
                        logger.Warn("[-] MainCollector.CheckIfRedirectUriExist.Enrich failed.");
                        logger.Debug(ex.Message);
                    }
                }
                Task<Dictionary<Guid, ServicePrincipal>> getAllServicePrincipals = MSGraphCollector.GetAllServicePrincipals();
                Result.ServicePrincipals = await getAllServicePrincipals;
                if (Result.ServicePrincipals != null)
                {
                    Console.WriteLine("[+] {0} service principals found in the tenant", Result.ServicePrincipals.Count);
                }
                Task<Dictionary<Guid, Group>> getGroupTask = MSGraphCollector.GetAllGroups();
                Result.Groups = await getGroupTask;
                if (Result.Groups != null)
                {
                    Console.WriteLine("[+] {0} groups found in the tenant", Result.Groups.Count);
                }

                // Calculate role membership and if a user can add creds to an application
                if (Result.DirectoryRoles != null)
                {
                    if (HasP2License)
                    {
                        MSGraphCollector PIMCollector = new MSGraphCollector(PowerAutomateAuthenticator, TenantId, Proxy);

                        // Alle Rollen parallel abfragen — UserAuthenticator ist thread-safe (SemaphoreSlim)
                        var pimTasks = Result.DirectoryRoles.Values.Select(async role =>
                        {
                            role.pimRoleAssignments = await PIMCollector.GetDirectoryRoleAssignments(TenantId, role.roleTemplateId);
                            role.pimRoleAssignmentsEligible = await PIMCollector.GetDirectoryRoleAssignmentsEligible(TenantId, role.roleTemplateId);
                        });
                        await Task.WhenAll(pimTasks);
                    }
                    // If not Premium P2 ist much easier
                    else
                    {
                        foreach (DirectoryRole role in Result.DirectoryRoles.Values)
                        {
                            if (DirectoryRoleTemplateID.RolesAllowingAddCreds.Contains(role.roleTemplateId))
                            {
                                List<AzurePrincipal> cloudAdmins = new List<AzurePrincipal>();
                                foreach (AzurePrincipal user in role.GetMembers())
                                {
                                    AzurePrincipal u = new AzurePrincipal(user.id, user.PrincipalType);
                                    cloudAdmins.Add(u);
                                }
                                foreach (Application app in Result.Applications.Values)
                                {
                                    foreach (AzurePrincipal a in cloudAdmins)
                                    {
                                        app.AddUserAbleToAddCreds(a);
                                    }
                                }
                                foreach (ServicePrincipal principal in Result.ServicePrincipals.Values)
                                {
                                    if (principal.appOwnerOrganizationId == this.TenantId)
                                    {
                                        foreach (AzurePrincipal a in cloudAdmins)
                                        {
                                            principal.AddUserAbleToAddCreds(a);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }


                Result.TenantSettings.AdminCenterSettings = new AdminCenterSettings();
                String baseAdress = await this.ComplianceCenterScanner.GetBaseAddress();
                this.ComplianceCenterScanner.BaseAddress = baseAdress;

                List<Task> officeTasks = new List<Task>
                {
                    MSGraphCollector.GetSettings(),
                    MSGraphCollector.GetAuthorizationPolicy(),
                    MSGraphCollector.GetDeviceRegistrationPolicy(),
                    MSGraphCollector.GetAllCondtionalAccessPolicies(),
                    MSGraphCollector.GetAuthenticationMethodsPolicy(),
                    MSGraphCollector.GetActivityBasedTimeoutPolicies(),
                    MSGraphCollector.GetLegacyPolicies(),
                    MSGraphCollector.GetGroupSettings(),
                    MainIamCollector.GetSecurityDefaults(),
                    MainIamCollector.GetDirectoryProperties(),
                    MainIamCollector.GetPasswordResetPolicies(),
                    MainIamCollector.GetPasswordPolicy(),
                    MainIamCollector.GetADConnectStatus(),
                    MainIamCollector.GetB2BPolicy(),
                    MainIamCollector.GetLCMSettings(),
                    MainIamCollector.GetUserSettings(),
                    MainIamCollector.GetSsgmProperties(),
                    MainIamCollector.GetLoginTenantBrandings(),
                    MainIamCollector.GetOnPremisesPasswordResetPolicy(),
                    ProvisionAPICollector.GetDirSyncFeatures(),
                    ProvisionAPICollector.GetMsolCompanyInformation(),
                    AdminCenterCollector.GetSkypeTeamsSettings(),
                    AdminCenterCollector.GetOfficeFormsSettings(),
                    AdminCenterCollector.GetOfficeStoreSettings(),
                    AdminCenterCollector.GetO365PasswordPolicy(),
                    AdminCenterCollector.GetSwaySettings(),
                    AdminCenterCollector.GetCalendarSharing(),
                    AdminCenterCollector.GetDirsyncManagement(),
                    AdminCenterCollector.GetOfficeOnline(),
                    ComplianceCenterScanner.GetDLPPolicies(),
                    ComplianceCenterScanner.GetDLPLabels(),
                    ComplianceCenterScanner.GetDLPLabelPolicies(),
                    MSGraphCollector.GetOrganizationSettings()
                };

                while (officeTasks.Any())
                {
                    Task result = await Task.WhenAny(officeTasks);
                    if (result.IsFaulted)
                    {
                        logger.Warn("[-] An error occurred. Don't panic...");
                        logger.Debug("Scanner.ScanTenant: OfficeTasks failed.");
                        logger.Debug(result.Exception?.InnerException?.Message ?? result.Exception?.Message);
                        officeTasks.Remove(result);
                        continue;
                    }
                    switch (result)
                    {
                        case Task<List<EnterpriseApplicationUserSettings>> getEnterpriseApplicationUserSettingsTask:
                            Result.EnterpriseApplicationUserSettings = await getEnterpriseApplicationUserSettingsTask;
                            break;
                        case Task<AuthorizationPolicy> getAuthorizationPolicyTask:
                            Result.TenantSettings.AuthorizationPolicy = await getAuthorizationPolicyTask;
                            break;
                        case Task<DeviceRegistrationPolicy> getDeviceRegistrationPolicyTask:
                            Result.TenantSettings.DeviceRegistrationPolicy = await getDeviceRegistrationPolicyTask;
                            break;
                        case Task<Dictionary<Guid, ConditionalAccessPolicy>> getConditionalAccessPolicyTask:
                            Result.CAPolicies = await getConditionalAccessPolicyTask;
                            break;
                        case Task<SecurityDefaults> getSecurityDefaultsTask:
                            Result.TenantSettings.SecurityDefaults = await getSecurityDefaultsTask;
                            break;
                        case Task<List<LegacyPolicy>> getLegacyPolicies:
                            Result.TenantSettings.LegacyPolicies = await getLegacyPolicies;
                            break;
                        case Task<List<GroupSetting>> getGroupSettings:
                            Result.TenantSettings.GroupSettings = await getGroupSettings;
                            break;
                        case Task<DirectoryProperties> getDirectoryPropertiesTask:
                            Result.TenantSettings.DirectoryProperties = await getDirectoryPropertiesTask;
                            break;
                        case Task<PasswordResetPolicies> getPasswordResetPolicies:
                            Result.TenantSettings.PasswordResetPolicies = await getPasswordResetPolicies;
                            break;
                        case Task<AzureADPasswordPolicy> getAzureADPasswordPolicy:
                            Result.TenantSettings.PasswordPolicy = await getAzureADPasswordPolicy;
                            break;
                        case Task<ADConnectStatus> getADConnectStatusTask:
                            Result.TenantSettings.ADConnectStatus = await getADConnectStatusTask;
                            break;
                        case Task<B2BPolicy> getB2BPolicyTask:
                            Result.TenantSettings.B2BPolicy = await getB2BPolicyTask;
                            break;
                        case Task<LCMSettings> getLCMSettingsTask:
                            Result.TenantSettings.LCMSettings = await getLCMSettingsTask;
                            break;
                        case Task<UserSettings> getUserSettingsTask:
                            Result.TenantSettings.UserSettings = await getUserSettingsTask;
                            break;
                        case Task<SsgmProperties> getSsgmPropertiesTask:
                            Result.TenantSettings.SsgmProperties = await getSsgmPropertiesTask;
                            break;
                        case Task<List<LoginTenantBranding>> getLoginTenantBrandingTask:
                            Result.TenantSettings.LoginTenantBrandings = await getLoginTenantBrandingTask;
                            break;
                        case Task<List<ActivityBasedTimeoutPolicy>> getActivityBasedTimeoutPolicies:
                            Result.TenantSettings.ActivityBasedTimeoutPolicy = await getActivityBasedTimeoutPolicies;
                            break;
                        case Task<DirSyncFeatures> getDirSyncFeaturesTask:
                            Result.TenantSettings.DirSyncFeatures = await getDirSyncFeaturesTask;
                            break;
                        case Task<SkypeTeams> getSkypeTeamsTask:
                            Result.TenantSettings.AdminCenterSettings.SkypeTeams = await getSkypeTeamsTask;
                            break;
                        case Task<OfficeFormsSettings> getOfficeFormsSettingsTask:
                            Result.TenantSettings.AdminCenterSettings.OfficeFormsSettings = await getOfficeFormsSettingsTask;
                            break;
                        case Task<OfficeStoreSettings> getOfficeStoreSettingsTask:
                            Result.TenantSettings.AdminCenterSettings.OfficeStoreSettings = await getOfficeStoreSettingsTask;
                            break;
                        case Task<O365PasswordPolicy> getO365PasswordPolicyTask:
                            Result.TenantSettings.AdminCenterSettings.O365PasswordPolicy = await getO365PasswordPolicyTask;
                            break;
                        case Task<SwaySettings> getSwaySettingsTask:
                            Result.TenantSettings.AdminCenterSettings.SwaySettings = await getSwaySettingsTask;
                            break;
                        case Task<CalendarSharing> getCalendarSharingTask:
                            Result.TenantSettings.AdminCenterSettings.CalendarSharing = await getCalendarSharingTask;
                            break;
                        case Task<DirsyncManagement> getDirsyncManagementTask:
                            Result.TenantSettings.AdminCenterSettings.DirsyncManagement = await getDirsyncManagementTask;
                            break;
                        case Task<List<DlpCompliancePolicy>> getDlpCompliancePolicyTask:
                            Result.TenantSettings.OfficeDLPPolicies = await getDlpCompliancePolicyTask;
                            break;
                        case Task<List<DlpLabel>> getDlpLabelTask:
                            Result.TenantSettings.DlpLabels = await getDlpLabelTask;
                            break;
                        case Task<List<DlpLabelPolicy>> getDlpLabelPoliciesTask:
                            Result.TenantSettings.DlpLabelPolicies = await getDlpLabelPoliciesTask;
                            break;
                        case Task<AuthenticationMethodsPolicy> getAuthenticationMethodsPolicyTask:
                            Result.TenantSettings.AuthenticationMethodsPolicy = await getAuthenticationMethodsPolicyTask;
                            break;
                        case Task<MsolCompanyInformation> getMSOLCompanyInformationTask:
                            Result.TenantSettings.MSOLCompanyInformation = await getMSOLCompanyInformationTask;
                            break;
                        case Task<OfficeOnline> getOfficeOnline:
                            Result.TenantSettings.OfficeOnline = await getOfficeOnline;
                            break;
                        case Task<OnPremisesPasswordResetPolicy> getOnPremisesPasswordResetPolicyTask:
                            Result.TenantSettings.OnPremisesPasswordResetPolicy = await getOnPremisesPasswordResetPolicyTask;
                            break;
                        case Task<List<OrganizationSettings>> getOrganizationSettingsTask:
                            List<OrganizationSettings> settings = await getOrganizationSettingsTask;
                            if (settings != null && settings.Count > 0)
                            {
                                if (settings.Count > 1)
                                {
                                    logger.Warn("Scanner.ScanTenant: More than one OrganizationSettings found. Taking the first one.");
                                }
                                Result.OrganizationSettings = settings[0];
                            }
                            else
                            {
                                logger.Warn("Scanner.ScanTenant: OrganizationSettings is null. This should not happen.");
                                Result.OrganizationSettings = null;
                            }
                            break;
                        default:
                            Console.WriteLine("Scanner.ScanTenant: OfficeTask Default. This should not happen");
                            break;
                    }
                    officeTasks.Remove(result);
                }
            }
            if (scopes.Contains(ScopeEnum.Teams))
            {
                TeamsSettings settings = new TeamsSettings();

                Task<List<TeamsClientConfiguration>> getTeamsClientConfigurationTask = TeamsCollector.GetTeamsClientConfiguration();
                Task<List<TenantFederationSetting>> getTenantFederationSettingsTask = TeamsCollector.GetTenantFederationSettings();
                Task<List<TeamsMeetingPolicy>> getTeamsMeetingPolicyTask = TeamsCollector.GetTeamsMeetingPolicy();
                Task<List<TeamsExternalPolicy>> getTeamsExternalPolicyTask = TeamsCollector.GetTeamsExternalPolicies();
                Task<List<TeamsMessagingPolicy>> getTeamsMessagePolicyTask = TeamsCollector.GetTeamsMessagingPolicy();

                List<Task> teamsTasks = new List<Task> { getTeamsClientConfigurationTask, getTenantFederationSettingsTask, getTeamsMeetingPolicyTask, getTeamsMessagePolicyTask, getTeamsExternalPolicyTask };

                while (teamsTasks.Any())
                {
                    Task result = await Task.WhenAny(teamsTasks);
                    if (result.IsFaulted)
                    {
                        logger.Warn("[-] An error occurred. Don't panic...");
                        logger.Debug("Scanner.ScanTenant: TeamsTasks failed.");
                        logger.Debug(result.Exception?.InnerException?.Message ?? result.Exception?.Message);
                        teamsTasks.Remove(result);
                        continue;
                    }

                    if (result == getTeamsClientConfigurationTask)
                    {
                        settings.TeamsClientConfigurations = await getTeamsClientConfigurationTask;
                    }
                    if (result == getTenantFederationSettingsTask)
                    {
                        settings.TenantFederationSettings = await getTenantFederationSettingsTask;
                    }
                    if (result == getTeamsMeetingPolicyTask) {
                        settings.TeamsMeetingPolicies = await getTeamsMeetingPolicyTask;
                    }
                    if (result == getTeamsExternalPolicyTask)
                    {
                        settings.TeamsExternalPolicies = await getTeamsExternalPolicyTask;
                    }
                    if (result == getTeamsMessagePolicyTask)
                    {
                        settings.TeamsMessagePolicies = await getTeamsMessagePolicyTask;
                    }
                    teamsTasks.Remove(result);
                }
                Result.TeamsSettings = settings;
            }

            if (scopes.Contains(ScopeEnum.SPO))
            {

                if (Result.TenantSettings.SecurityDefaults == null)
                {
                    Result.TenantSettings.SecurityDefaults = await MainIamCollector.GetSecurityDefaults();
                }
                if (isGlobalAdmin || isSharePointAdmin)
                {
                    //Result.SharePointInformation = await ProvisionAPICollector.GetSharepointInformation();
                    String SPODomain = SPOBaseAddress.GetBaseAddress(Result);
                    if (SPODomain != null)
                    {
                        //Console.WriteLine("[+] Found SharePoint on: {0}", Result.SharePointInformation.SharePointUrl);
                        //Console.WriteLine("[+] Found SharePoint-Admin on: {0}", Result.SharePointInformation.AdminUrl);
                        String adminUrl = SPOBaseAddress.GetAdminAddress(SPODomain);
                        String sharePointUrl = SPOBaseAddress.GetSPOAddress(SPODomain);
                        Result.SharePointInformation = new SharePointInformation(adminUrl, sharePointUrl);
                        SharePointCollector sharePointScanner = new SharePointCollector(SPOMgmtShellAuthenticator, adminUrl, TenantId, Proxy);
                        var settingsTask = sharePointScanner.GetSharePointSettings();
                        var tenantTask = sharePointScanner.GetSPOTenant();
                        var pagesTask = sharePointScanner.GetSPOPages();

                        try
                        {
                            await Task.WhenAll(settingsTask, tenantTask, pagesTask);
                        }
                        catch (Exception ex)
                        {
                            logger.Warn("[-] One or more SharePoint tasks failed: {0}", ex.Message);
                        }

                        if (settingsTask.Status == TaskStatus.RanToCompletion)
                            Result.SharePointInformation.SharePointInternalInfos = settingsTask.Result;
                        if (tenantTask.Status == TaskStatus.RanToCompletion)
                            Result.SharePointInformation.SPOTenant = tenantTask.Result;
                        if (pagesTask.Status == TaskStatus.RanToCompletion)
                            Result.SharePointInformation.SPOPages = pagesTask.Result;
                    }
                }

            }

            if (scopes.Contains(ScopeEnum.EXO))
            {
                if (Result.TenantSettings.SecurityDefaults == null)
                {
                    Result.TenantSettings.SecurityDefaults = await MainIamCollector.GetSecurityDefaults();
                }
                if (Result.Domains == null)
                {
                    Result.Domains = await MSGraphCollector.GetAzDomains();
                }

                Result.ExchangeOnlineSettings = new ExchangeOnlineSettings();

                List<Task> exchangeTask = new List<Task>
                {
                    ExchangeOnlineScanner.GetAdminAuditLogConfig(),
                    ExchangeOnlineScanner.GetHostedOutboundSpamFilterPolicies(),
                    ExchangeOnlineScanner.GetMalwareFilterPolicies(),
                    ExchangeOnlineScanner.GetTransportRules(),
                    ExchangeOnlineScanner.GetAcceptedDomains(),
                    ExchangeOnlineScanner.GetDkimSigningConfig(),
                    AdminCenterCollector.GetExchangeModernAuthSettings(),
                    ExchangeOnlineScanner.GetMalwareFilterRules(),
                    ExchangeOnlineScanner.GeRoleAssignmentPolicies(),
                    ExchangeOnlineScanner.GetRemoteDomains(),
                    ExchangeOnlineScanner.GetMailboxes(),
                    ExchangeOnlineScanner.GetOrganizationConfig(),
                    ExchangeOnlineScanner.GetAuthenticationPolicies(),
                    ExchangeOnlineScanner.GetEXOUsers(),
                    ExchangeOnlineScanner.GetOwaMailboxPolicy(),
                    ExchangeOnlineScanner.GetMailboxAuditBypassAssociations(),
                    ExchangeOnlineScanner.GetExternalInOutlooks(),
                    ExchangeOnlineScanner.GetHostedConnectionFilterPolicy(),
                    ExchangeOnlineScanner.GetHostedContentFilterPolicy(),
                    ExchangeOnlineScanner.GetTransportConfig(),
                    ExchangeOnlineScanner.GetSafeLinksPolicy(),
                    ExchangeOnlineScanner.GetSafeAttachmentPolicies(),
                    ExchangeOnlineScanner.GetSafeAttachmentRules(),
                    ExchangeOnlineScanner.GetAtpPolicyForO365(),
                    ExchangeOnlineScanner.GetAntiPhishRule(),
                    ExchangeOnlineScanner.GetAntiPhishPolicy(),
                    ExchangeOnlineScanner.GetTeamsProtectionPolicies(),
                    ExchangeOnlineScanner.GetEmailTenantSettings()
                };

                while (exchangeTask.Any())
                {
                    Task result = await Task.WhenAny(exchangeTask);
                    if (result.IsFaulted)
                    {
                        logger.Warn("[-] An error occurred. Don't panic...");
                        logger.Debug("Scanner.ScanTenant: ExchangeTasks failed.");
                        logger.Debug(result.Exception?.InnerException?.Message ?? result.Exception?.Message);
                        exchangeTask.Remove(result);
                        continue;
                    }

                    switch (result)
                    {
                        case Task<AdminAuditLogConfig> getAdminAuditLogConfigTask:
                            Result.ExchangeOnlineSettings.AdminAuditLogConfig = await getAdminAuditLogConfigTask;
                            break;
                        case Task<List<HostedOutboundSpamFilterPolicy>> getHostedOutboundSpamFilterPolicyTask:
                            Result.ExchangeOnlineSettings.HostedOutboundSpamFilterPolicy = await getHostedOutboundSpamFilterPolicyTask;
                            break;
                        case Task<List<MalwareFilterPolicy>> getMalwareFilterPolicyTask:
                            Result.ExchangeOnlineSettings.MalwareFilterPolicy = await getMalwareFilterPolicyTask;
                            break;
                        case Task<List<TransportRule>> getTransportRuleTask:
                            Result.ExchangeOnlineSettings.TransportRules = await getTransportRuleTask;
                            break;
                        case Task<List<AcceptedDomain>> getAcceptedDomainTask:
                            Result.ExchangeOnlineSettings.AcceptedDomains = await getAcceptedDomainTask;
                            break;
                        case Task<List<DkimSigningConfig>> getDkimSigningConfigTask:
                            Result.ExchangeOnlineSettings.DkimSigningConfigs = await getDkimSigningConfigTask;
                            break;
                        case Task<ExchangeModernAuthSettings> getExchangeModernAuthSettingsTask:
                            Result.ExchangeOnlineSettings.ExchangeModernAutheSettings = await getExchangeModernAuthSettingsTask;
                            break;
                        case Task<List<MalwareFilterRule>> getMalwareFilterRuleTask:
                            Result.ExchangeOnlineSettings.MalwareFilterRule = await getMalwareFilterRuleTask;
                            break;
                        case Task<List<Mailbox>> getMailboxTask:
                            Result.ExchangeOnlineSettings.Mailboxes = await getMailboxTask;
                            break;
                        case Task<List<RemoteDomain>> getRemoteDomainTask:
                            Result.ExchangeOnlineSettings.RemoteDomains = await getRemoteDomainTask;
                            break;
                        case Task<List<RoleAssignmentPolicy>> getRoleAssignmentPolicyTask:
                            Result.ExchangeOnlineSettings.RoleAssignmentPolicies = await getRoleAssignmentPolicyTask;
                            break;
                        case Task<OrganizationConfig> getOrganizationConfigTask:
                            Result.ExchangeOnlineSettings.OrganizationConfig = await getOrganizationConfigTask;
                            break;
                        case Task<List<AuthenticationPolicy>> getAuthenticationPolicyTask:
                            Result.ExchangeOnlineSettings.AuthenticationPolicies = await getAuthenticationPolicyTask;
                            break;
                        case Task<List<EXOUser>> getEXOUserTask:
                            Result.ExchangeOnlineSettings.EXOUsers = await getEXOUserTask;
                            break;
                        case Task<OwaMailboxPolicy> getOwaMailboxPolicyTask:
                            Result.ExchangeOnlineSettings.OwaMailboxPolicy = await getOwaMailboxPolicyTask;
                            break;
                        case Task<List<MailboxAuditBypassAssociation>> getMailboxAuditBypassAssociationTask:
                            Result.ExchangeOnlineSettings.MailboxAuditBypassAssociations = await getMailboxAuditBypassAssociationTask;
                            break;
                        case Task<List<ExternalInOutlook>> getExternalInOutlooksTask:
                            Result.ExchangeOnlineSettings.ExternalInOutlooks = await getExternalInOutlooksTask;
                            break;
                        case Task<List<HostedConnectionFilterPolicy>> getHostedConnectionFilterPolicy:
                            Result.ExchangeOnlineSettings.HostedConnectionFilterPolicy = await getHostedConnectionFilterPolicy;
                            break;
                        case Task<List<HostedContentFilterPolicy>> getHostedContentFilterPolicy:
                            Result.ExchangeOnlineSettings.HostedContentFilterPolicies = await getHostedContentFilterPolicy;
                            break;
                        case Task<List<TransportConfig>> getTransportConfig:
                            Result.ExchangeOnlineSettings.TransportConfig = await getTransportConfig;
                            break;
                        case Task<List<SafeLinksPolicy>> getSafeLinksPolicy:
                            Result.ExchangeOnlineSettings.SafeLinksPolicies = await getSafeLinksPolicy;
                            break;
                        case Task<List<SafeAttachmentPolicy>> getSafeAttachmentPolicies:
                            Result.ExchangeOnlineSettings.SafeAttachmentPolicies = await getSafeAttachmentPolicies;
                            break;
                        case Task<List<SafeAttachmentRule>> getSafeAttachmentRules:
                            Result.ExchangeOnlineSettings.SafeAttachmentRules = await getSafeAttachmentRules;
                            break;
                        case Task<List<AtpPolicyForO365>> getAtpPolicyForO365:
                            Result.ExchangeOnlineSettings.AtpPolicyForO365s = await getAtpPolicyForO365;
                            break;
                        case Task<List<AntiPhishRule>> getAntiPhishRules:
                            Result.ExchangeOnlineSettings.AntiPhishRules = await getAntiPhishRules;
                            break;
                        case Task<List<AntiPhishPolicy>> getAntiPhishPolicies:
                            Result.ExchangeOnlineSettings.AntiPhishPolicies = await getAntiPhishPolicies;
                            break;
                        case Task<List<EmailTenantSettings>> getEmailTenantSettings:
                            Result.ExchangeOnlineSettings.EmailTenantSettings = await getEmailTenantSettings;
                            break;
                        case Task<List<TeamsProtectionPolicy>> getTeamsProtectionPolicy:
                            Result.ExchangeOnlineSettings.TeamsProtectionPolicies = await getTeamsProtectionPolicy;
                            break;
                        case Task<List<MailboxFolderStatistic>> getMailboxFolderStatistic:
                            Result.ExchangeOnlineSettings.MailboxFolderStatistics = await getMailboxFolderStatistic;
                            break;
                        default:
                            Console.WriteLine("Scanner.ScanTenant: Hit default in exchangeTasks. Should not happen.");
                            break;
                    }
                    exchangeTask.Remove(result);
                }

                if (Result.ExchangeOnlineSettings.EXOUsers != null)
                {
                    Console.WriteLine("[+] Found {0} Exchange-User.", Result.ExchangeOnlineSettings.EXOUsers.Count);
                }
                Console.WriteLine("[+] Start scanning Azure Resources.");
            }

            if (scopes.Contains(ScopeEnum.Azure))
            {
                Result.ManagementGroups = await AzMgmtCollector.GetAllManagementGroups();
                Result.ManagementGroupSettings = await AzMgmtCollector.GetManagementGroupSettings();
                Result.SubscriptionPolicy = await AzMgmtCollector.GetSubscriptionPolicy();
                Result.Subscriptions = await AzMgmtCollector.GetAllSubscriptions();
                Task scanSubscriptionTask = ScanSubscriptions(Result);
                await scanSubscriptionTask;
            }

            // Ignore infos not available
            /**if (scopes.Contains(ScopeEnum.MDM))
            {
                MDMScanner = new MDMScanner(this);
                if (MDMScanner.CheckIntunePowerShellAvailable())
                {
                    MDMSettings mdmSettings = new MDMSettings();
                    mdmSettings.MobileDeviceConfigurations = await MDMScanner.GetMobileDeviceConfigurations();
                    mdmSettings.ConfigurationPolicies = await MDMScanner.GetConfigurationPolicies();
                    mdmSettings.MobileDeviceCompliancePolicies = await MDMScanner.GetMobileDeviceCompliancePolicies();
                    Result.MDMSettings = mdmSettings;
                }
            }**/

            if (Result.Users != null && Result.Users.Count > 0 && Result.CAPolicies != null && Result.CAPolicies.Count > 0)
            {
                EnrichUserWithCAPolicies.Enrich(Result);
            }
            if (HasP2License)
            {
                await AssignUserToRole.Enrich(Result, MSGraphCollector);
                await AssignEligibleUserToRole.Enrich(Result, MSGraphCollector);
                AssignUserCanAddCreds.Enrich(Result);
            }
            if (redirectionTask != null)
            {
                await redirectionTask;
            }
            Console.WriteLine("[+] Finished collecting information.");
            return Result;
        }

        private const int SubscriptionConcurrency = 5;

        private async Task ScanSubscriptions(Tenant Result)
        {
            var semaphore = new System.Threading.SemaphoreSlim(SubscriptionConcurrency, SubscriptionConcurrency);
            var tasks = Result.Subscriptions.Values.Select(async sub =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var storageAccountsTask = AzMgmtCollector.GetStorageAccounts(sub.subscriptionId);
                    var keyVaultsTask = AzMgmtCollector.GetKeyVaults(sub.subscriptionId);
                    var activityLogAlertsTask = AzMgmtCollector.GetActivityLogAlerts(sub.subscriptionId);
                    var networkSecurityGroupsTask = AzMgmtCollector.GetNetworkSecurityGroups(sub.subscriptionId);
                    var sqlServersTask = AzMgmtCollector.GetSQLServers(sub.subscriptionId);
                    var autoProvisioningSettingsTask = AzMgmtCollector.GetProvisioningSettings(sub.subscriptionId);
                    var securityCenterBuiltInTask = AzMgmtCollector.GetSecurityCenterBuiltIn(sub.subscriptionId);
                    var securityContactsTask = AzMgmtCollector.GetSecurityContacts(sub.subscriptionId);
                    var virtualMachinesTask = AzMgmtCollector.GetVirtualMachines(sub.subscriptionId);
                    var postgreSQLsTask = AzMgmtCollector.GetPostgreSQLFlexibleServers(sub.subscriptionId);
                    var policyAssignmentTask = AzMgmtCollector.GetPolicyAssignment(sub.subscriptionId);

                    sub.Resources.StorageAccounts = await storageAccountsTask;
                    sub.Resources.KeyVaults = await keyVaultsTask;
                    sub.Resources.ActivityLogAlerts = await activityLogAlertsTask;
                    sub.Resources.NetworkSecurityGroups = await networkSecurityGroupsTask;
                    sub.Resources.SQLServers = await sqlServersTask;
                    sub.AutoProvisioningSettings = await autoProvisioningSettingsTask;
                    sub.SecurityCenterBuiltIn = await securityCenterBuiltInTask;
                    sub.SecurityContact = await securityContactsTask;
                    sub.Resources.VirtualMachines = await virtualMachinesTask;
                    sub.Resources.PostgreSQLs = await postgreSQLsTask;
                    sub.PolicyAssignment = await policyAssignmentTask;

                    if (sub.Resources.KeyVaults != null)
                    {
                        var keyVaultTasks = sub.Resources.KeyVaults.Select(async vault =>
                        {
                            KeyVaultCollector vaultScanner = new KeyVaultCollector(AADPowerShellAuthenticator, vault.properties.vaultUri, TenantId, Proxy);
                            var keysTask = vaultScanner.GetKeyVaultKeys();
                            var secretsTask = vaultScanner.GetKeyVaultSecrets();
                            vault.Keys = await keysTask;
                            vault.Secrets = await secretsTask;
                        });
                        await Task.WhenAll(keyVaultTasks);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
