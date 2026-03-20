using AzRanger.Models.Teams;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzRanger.AzScanner
{
    class TeamsCollector : AbstractCollector
    {
        private const String TeamsClientConfiguration = "/Skype.Policy/configurations/TeamsClientConfiguration";
        private const String TenantFederationSettings = "/Skype.Policy/configurations/TenantFederationSettings";
        private const String TeamsMeetingPolicy = "/Skype.Policy/configurations/TeamsMeetingPolicy";
        private const String TeamsExternalPolicy = "/Skype.Policy/configurations/ExternalAccessPolicy";
        private const String TeamsMessagingPolicy = "/Skype.Policy/configurations/TeamsMessagingPolicy";

        public TeamsCollector(IAuthenticator authenticator, String tenantId, String proxy)
        {
            this.Authenticator = authenticator;
            this.TenantId = tenantId;
            this.BaseAddress = "https://api.interfaces.records.teams.microsoft.com";
            this.Scope = new string[] { "48ac35b8-9aa8-4d74-927d-1f4a14a0b239/.default", "offline_access" };
            this.client = Helper.GetDefaultClient(additionalHeaders, proxy);
        }
        public Task<List<TeamsClientConfiguration>> GetTeamsClientConfiguration()
        {
            return Get<List<TeamsClientConfiguration>>(TeamsClientConfiguration);
        }

        public Task<List<TenantFederationSetting>> GetTenantFederationSettings()
        {
            return Get<List<TenantFederationSetting>>(TenantFederationSettings);
        }

        public Task<List<TeamsMeetingPolicy>> GetTeamsMeetingPolicy()
        {
            return Get<List<TeamsMeetingPolicy>>(TeamsMeetingPolicy);
        }

        public Task<List<TeamsExternalPolicy>> GetTeamsExternalPolicies()
        {
            return Get<List<TeamsExternalPolicy>>(TeamsExternalPolicy);
        }

        public Task<List<TeamsMessagingPolicy>> GetTeamsMessagingPolicy()
        {
            return Get<List<TeamsMessagingPolicy>>(TeamsMessagingPolicy);
        }
    }
}
