using AzRanger.Models.ExchangeOnline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    public class ExchangeOnlineCollector : PowerShellCollectorBase
    {
        private const String MalwareFilterPolicy = "Get-MalwareFilterPolicy";
        private const String MalwareFilterRule = "Get-MalwareFilterRule";
        private const String HostedOutboundSpamFilterPolicy = "Get-HostedOutboundSpamFilterPolicy";
        private const String TransportRule = "Get-TransportRule";
        private const String AcceptedDomain = "Get-AcceptedDomain";
        private const String DkimSigningConfig = "Get-DkimSigningConfig";
        private const String AdminAuditLogConfig = "Get-AdminAuditLogConfig";
        private const String RemoteDomain = "Get-RemoteDomain";
        private const String Mailbox = "Get-Mailbox";
        private const String RoleAssignmentPolicy = "Get-RoleAssignmentPolicy";
        private const String OrganizationConfig = "Get-OrganizationConfig";
        private const String Users = "Get-User";
        private const String AuthenticationPolicy = "Get-AuthenticationPolicy";
        private const String OwaMailboxPolicy = "Get-OwaMailboxPolicy";
        private const String MailboxAuditBypassAssociation = "Get-MailboxAuditBypassAssociation";
        private const String ExternalInOutlook = "Get-ExternalInOutlook";
        private const String HostedConnectionFilterPolicy = "Get-HostedConnectionFilterPolicy";
        private const String HostedContentFilterPolicy = "Get-HostedContentFilterPolicy";
        private const String TransportConfig = "Get-TransportConfig";
        private const String SafeLinksPolicy = "Get-SafeLinksPolicy";
        private const String SafeAttachmentPolicy = "Get-SafeAttachmentPolicy";
        private const String SafeAttachmentRule = "Get-SafeAttachmentRule";
        private const String AtpPolicyForO365 = "Get-AtpPolicyForO365";
        private const String AntiPhishRule = "Get-AntiPhishRule";
        private const String AntiPhishPolicy = "Get-AntiPhishPolicy";
        private const String EmailTenantSettings = "Get-EmailTenantSettings";
        private const String TeamsProtectionPolicy = "Get-TeamsProtectionPolicy";
        private const String TeamsProtectionPolicyRule = "Get-TeamsProtectionPolicyRule";
        private const String MailboxFolderStatistics = "Get-MailboxFolderStatistics";

        public ExchangeOnlineCollector(IAuthenticator authenticator, String tenantId, String proxy)
        {
            this.Authenticator = authenticator;
            this.TenantId = tenantId;
            this.BaseAddress = "https://outlook.office365.com";
            this.EndPoint = "/adminapi/beta/" + tenantId + "/InvokeCommand";
            this.Scope = new String[] { "https://outlook.office365.com/.default", "offline_access" };
            this.client = Helper.GetDefaultClient(this.additionalHeaders, proxy);
        }

        public Task<OrganizationConfig> GetOrganizationConfig()
        {
            return GetSingle<OrganizationConfig>(OrganizationConfig);
        }

        public Task<OwaMailboxPolicy> GetOwaMailboxPolicy()
        {
            return GetSingle<OwaMailboxPolicy>(OwaMailboxPolicy);
        }

        public Task<List<EXOUser>> GetEXOUsers()
        {
            return GetAllOf<EXOUser>(Users);
        }

        public Task<List<AuthenticationPolicy>> GetAuthenticationPolicies()
        {
            return GetAllOf<AuthenticationPolicy>(AuthenticationPolicy);
        }

        public Task<List<RemoteDomain>> GetRemoteDomains()
        {
            return GetAllOf<RemoteDomain>(RemoteDomain);
        }

        public Task<List<RoleAssignmentPolicy>> GeRoleAssignmentPolicies()
        {
            return GetAllOf<RoleAssignmentPolicy>(RoleAssignmentPolicy);
        }

        private const int MailboxConcurrency = 10;

        public async Task<List<Mailbox>> GetMailboxes()
        {
            List<Tuple<String, String>> parameters = new List<Tuple<String, String>>();
            parameters.Add(new Tuple<String, String>("ResultSize", "unlimited"));
            List<Mailbox> mailBoxes = await GetAllOf<Mailbox>(Mailbox, parameters);

            // Folder-Statistiken parallel abrufen (Exchange Online drosselt aggressiv — Semaphore begrenzt Gleichzeitigkeit)
            var semaphore = new SemaphoreSlim(MailboxConcurrency, MailboxConcurrency);
            var folderTasks = mailBoxes.Select(async mailbox =>
            {
                await semaphore.WaitAsync();
                try
                {
                    mailbox.mailboxFolderStatistics = await GetMailboxFolderStatistic(mailbox.Identity);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(folderTasks);

            return mailBoxes;
        }

        public Task<List<MailboxFolderStatistic>> GetMailboxFolderStatistic(String id)
        {
            List<Tuple<String, String>> parameters = new List<Tuple<String, String>>();
            parameters.Add(new Tuple<String, String>("Identity", id));
            List<Tuple<String, String>> headers = new List<Tuple<String, String>>();
            headers.Add(new Tuple<String, String>("X-Clientapplication", "ExoManagementModule"));
            return GetAllOf<MailboxFolderStatistic>(ExchangeOnlineCollector.MailboxFolderStatistics, parameters, headers);
        }

        public Task<List<MalwareFilterRule>> GetMalwareFilterRules()
        {
            return GetAllOf<MalwareFilterRule>(MalwareFilterRule);
        }

        public async Task<List<AcceptedDomain>> GetAcceptedDomains()
        {
            List<AcceptedDomain> allDomains = await GetAllOf<AcceptedDomain>(AcceptedDomain);
            if (allDomains == null)
            {
                logger.Debug(String.Format("ExchangeOnlineScanner.GetAcceptedDomains() is null."));
                return null;
            }
            foreach (AcceptedDomain domain in allDomains)
            {
                domain.HasSPF = DNSCollector.HasSPF(domain.DomainName);
                domain.HasDMARC = DNSCollector.HasDMARC(domain.DomainName);
            }
            return allDomains;
        }

        public Task<AdminAuditLogConfig> GetAdminAuditLogConfig()
        {
            return GetSingle<AdminAuditLogConfig>(AdminAuditLogConfig);
        }

        public Task<List<DkimSigningConfig>> GetDkimSigningConfig()
        {
            return GetAllOf<DkimSigningConfig>(DkimSigningConfig);
        }

        public Task<List<HostedOutboundSpamFilterPolicy>> GetHostedOutboundSpamFilterPolicies()
        {
            return GetAllOf<HostedOutboundSpamFilterPolicy>(HostedOutboundSpamFilterPolicy);
        }

        public Task<List<MalwareFilterPolicy>> GetMalwareFilterPolicies()
        {
            return GetAllOf<MalwareFilterPolicy>(ExchangeOnlineCollector.MalwareFilterPolicy);
        }

        public Task<List<TransportRule>> GetTransportRules()
        {
            return GetAllOf<TransportRule>(ExchangeOnlineCollector.TransportRule);
        }

        public Task<List<MailboxAuditBypassAssociation>> GetMailboxAuditBypassAssociations()
        {
            return GetAllOf<MailboxAuditBypassAssociation>(ExchangeOnlineCollector.MailboxAuditBypassAssociation);
        }

        public Task<List<ExternalInOutlook>> GetExternalInOutlooks()
        {
            return GetAllOf<ExternalInOutlook>(ExchangeOnlineCollector.ExternalInOutlook);
        }

        public Task<List<HostedConnectionFilterPolicy>> GetHostedConnectionFilterPolicy()
        {
            List<Tuple<String, String>> parameters = new List<Tuple<String, String>>();
            parameters.Add(new Tuple<String, String>("Identity", "Default"));
            return GetAllOf<HostedConnectionFilterPolicy>(ExchangeOnlineCollector.HostedConnectionFilterPolicy, parameters);
        }
        public Task<List<HostedContentFilterPolicy>> GetHostedContentFilterPolicy()
        {
            return GetAllOf<HostedContentFilterPolicy>(ExchangeOnlineCollector.HostedContentFilterPolicy);
        }

        public Task<List<TransportConfig>> GetTransportConfig()
        {
            return GetAllOf<TransportConfig>(ExchangeOnlineCollector.TransportConfig);
        }

        public Task<List<SafeLinksPolicy>> GetSafeLinksPolicy()
        {
            return GetAllOf<SafeLinksPolicy>(ExchangeOnlineCollector.SafeLinksPolicy);
        }
        public Task<List<SafeAttachmentPolicy>> GetSafeAttachmentPolicies()
        {
            return GetAllOf<SafeAttachmentPolicy>(ExchangeOnlineCollector.SafeAttachmentPolicy);
        }

        public Task<List<SafeAttachmentRule>> GetSafeAttachmentRules()
        {
            return GetAllOf<SafeAttachmentRule>(ExchangeOnlineCollector.SafeAttachmentRule);
        }

        public Task<List<AtpPolicyForO365>> GetAtpPolicyForO365()
        {
            return GetAllOf<AtpPolicyForO365>(ExchangeOnlineCollector.AtpPolicyForO365);
        }

        public Task<List<AntiPhishRule>> GetAntiPhishRule()
        {
            return GetAllOf<AntiPhishRule>(ExchangeOnlineCollector.AntiPhishRule);
        }

        public Task<List<EmailTenantSettings>> GetEmailTenantSettings()
        {
            return GetAllOf<EmailTenantSettings>(ExchangeOnlineCollector.EmailTenantSettings);
        }

        public Task<List<AntiPhishPolicy>> GetAntiPhishPolicy()
        {
            return GetAllOf<AntiPhishPolicy>(ExchangeOnlineCollector.AntiPhishPolicy);
        }

        public Task<List<TeamsProtectionPolicy>> GetTeamsProtectionPolicies()
        {
            return GetAllOf<TeamsProtectionPolicy>(ExchangeOnlineCollector.TeamsProtectionPolicy);
        }

        private async Task<T> GetSingle<T>(string command) where T : class
        {
            List<T> result = await GetAllOf<T>(command);
            if (result == null || result.Count == 0)
            {
                logger.Debug("ExchangeOnlineScanner.GetSingle<{0}>({1}): no results.", typeof(T).Name, command);
                return null;
            }
            return result[0];
        }

    }
}
