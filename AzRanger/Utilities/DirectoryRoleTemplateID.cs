using System.Collections.Generic;

namespace AzRanger.Utilities
{
    public static class DirectoryRoleTemplateID
    {
        // High Priv Accounts
        public const string PrivilegedAuthenticationAdministrator = "7be44c8a-adaf-4e2a-84d6-ab2649e08a13";
        public const string GlobalAdministrator = "62e90394-69f5-4237-9190-012177145e10";

        // Priv Accounts
        // See https://learn.microsoft.com/en-us/azure/active-directory/roles/permissions-reference
        public const string ApplicationAdministrator = "9b895d92-2cd3-44c7-9d02-a6ac2d5ea5c3";
        public const string ApplicationDeveloper = "cf1c38e5-3621-4004-a7cb-879624dced7c";
        public const string AuthenticationAdministrator = "c4e39bd9-1100-46d3-8c65-fb160da0071f";
        public const string AzureADJoinedDeviceLocalAdministrator = "9f06204d-73c1-4d4c-880a-6edb90606fd8";
        public const string AzureInformationProtectionAdministrator = "7495fdc4-34c4-4d15-a289-98788ce399fd";
        public const string BillingAdministrator = "b0f54661-2d74-4c50-afa3-1ec803f12efe";
        public const string CloudApplicationAdministrator = "158c047a-c907-4556-b7ef-446551a6b5f7";
        public const string CloudDeviceAdministrator = "7698a772-787b-4ac8-901f-60d6b08affd2";
        public const string ComplianceAdministrator = "17315797-102d-40b4-93e0-432062caca18";
        public const string ConditionalAccessAdministrator = "b1be1c3e-b65d-4f19-8427-f6fa0d97feb9";
        public const string CustomerLockBoxAccessApprover = "5c4f9dcd-47dc-4cf7-8c9a-9e4207cbfc91";
        public const string DirectorySyncAccounts = "d29b2b05-8046-44ba-8758-1e26182fcf32";
        public const string DirectoryWriters = "9360feb5-f418-4baa-8175-e2a00bac4301";
        public const string ExchangeAdministrator = "29232cdf-9323-42fd-ade2-1d097af3e4de";
        public const string FabricAdministrator = "a9ea8996-122f-4c74-9520-8edcd192826c";
        public const string GroupsAdministrator = "fdd7a751-b60b-444a-984c-02652fe8fa1c";
        public const string HelpdeskAdministrator = "729827e3-9c14-49f7-bb1b-9608f156bbb8";
        public const string HybridIdentityAdministrator = "8ac3fc64-6eca-42ea-9e69-59f4c7b60eb2";
        public const string IntuneAdministrator = "3a2c62db-5318-420d-8d74-23affee5d9d5";
        public const string KaizalaAdministrator = "74ef975b-6605-40af-a5d2-b9539d836353";
        public const string LicenseAdministrator = "4d6ac14f-3453-41d0-bef9-a3e0c569773a";
        public const string PartnerTier1Support = "4ba39ca4-527c-499a-b93d-d9b492c50246";
        public const string PartnerTier2Support = "e00e864a-17c5-4a4b-9c06-f5b95a8d5bd8";
        public const string PasswordAdministrator = "966707d0-3269-4727-9be2-8c3a10f19b9d";
        public const string PrivilegedRoleAdministrator = "e8611ab8-c189-46e8-94e1-60213ab1f814";
        public const string SecurityAdministrator = "194ae4cb-b126-40b2-bd5b-6091b380977d";
        public const string SharePointAdministrator = "f28a1f50-f6e7-4571-818b-6a12f2af6b6c";
        public const string SkypeForBusinessAdministrator = "75941009-915a-4869-abe7-691bff18279e";
        public const string TeamsAdministrator = "69091246-20e8-4a56-aa4d-066075b2a7a8";
        public const string UserAdministrator = "fe930be7-5e62-47db-91af-98c3a49a38b1";
        public const string GlobalReader = "f2ef992c-3afb-46b9-b7cf-a126ee74c451";

        // Others
        public const string DynamicsAdministrator = "44367163-eba1-44c3-98af-f5787879f96a";
        public const string PowerPlatformAdministrator = "11648597-926c-4cf3-9c36-bcebb0ba8dcc";

        //https://posts.specterops.io/azure-privilege-escalation-via-service-principal-abuse-210ae2be2a5
        public static readonly HashSet<string> RolesAllowingAddCreds = new HashSet<string> {
                CloudApplicationAdministrator,
                ApplicationAdministrator,
                HybridIdentityAdministrator
        };

        public static readonly HashSet<string> HighPrivRoles = new HashSet<string> {
            ApplicationAdministrator,
            AuthenticationAdministrator,
            AzureInformationProtectionAdministrator,
            BillingAdministrator,
            CloudApplicationAdministrator,
            CloudDeviceAdministrator,
            ComplianceAdministrator,
            CustomerLockBoxAccessApprover,
            ExchangeAdministrator,
            FabricAdministrator,
            GlobalAdministrator,
            HelpdeskAdministrator,
            IntuneAdministrator,
            KaizalaAdministrator,
            LicenseAdministrator,
            AzureADJoinedDeviceLocalAdministrator,
            PasswordAdministrator,
            PrivilegedAuthenticationAdministrator,
            PrivilegedRoleAdministrator,
            SecurityAdministrator,
            SharePointAdministrator,
            SkypeForBusinessAdministrator,
            TeamsAdministrator,
            UserAdministrator,
        };

        // Roles which are Global Admin or can become Global Admins
        public static readonly HashSet<string> GlobalAdmins = new HashSet<string> {
            GlobalAdministrator,
            PrivilegedAuthenticationAdministrator
        };

    }
}