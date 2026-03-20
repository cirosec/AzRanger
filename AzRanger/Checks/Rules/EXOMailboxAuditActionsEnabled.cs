using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class EXOMailboxAuditActionsEnabled : BaseCheck
    {
        private static readonly string[] AdminActions = {
            "ApplyRecord", "Copy", "Create", "FolderBind", "HardDelete",
            "MailItemsAccessed", "Move", "MoveToDeletedItems", "SendAs",
            "SendOnBehalf", "Send", "SoftDelete", "Update", "UpdateCalendarDelegation",
            "UpdateFolderPermissions", "UpdateInboxRules"
        };

        private static readonly string[] DelegateActions = {
            "ApplyRecord", "Create", "FolderBind", "HardDelete", "Move",
            "MailItemsAccessed", "MoveToDeletedItems", "SendAs", "SendOnBehalf",
            "SoftDelete", "Update", "UpdateFolderPermissions", "UpdateInboxRules"
        };

        private static readonly string[] OwnerActions = {
            "ApplyRecord", "Create", "HardDelete", "MailboxLogin", "Move",
            "MailItemsAccessed", "MoveToDeletedItems", "Send", "SoftDelete", "Update",
            "UpdateCalendarDelegation", "UpdateFolderPermissions", "UpdateInboxRules"
        };

        public override CheckResult Audit(Tenant tenant)
        {
            if (tenant.ExchangeOnlineSettings?.Mailboxes == null)
            {
                return CheckResult.NoFinding;
            }

            bool passed = true;

            foreach (Mailbox mailbox in tenant.ExchangeOnlineSettings.Mailboxes)
            {
                // Only check UserMailbox type
                if (mailbox.RecipientTypeDetails != "UserMailbox")
                {
                    continue;
                }

                // Check if AuditEnabled is true
                if (!mailbox.AuditEnabled)
                {
                    this.AddAffectedEntity(mailbox);
                    passed = false;
                    continue;
                }

                // Verify Admin actions
                var adminMissing = AdminActions.Where(a =>
                    mailbox.AuditAdmin == null || !mailbox.AuditAdmin.Contains(a)).ToList();

                // Verify Delegate actions
                var delegateMissing = DelegateActions.Where(a =>
                    mailbox.AuditDelegate == null || !mailbox.AuditDelegate.Contains(a)).ToList();

                // Verify Owner actions
                var ownerMissing = OwnerActions.Where(a =>
                    mailbox.AuditOwner == null || !mailbox.AuditOwner.Contains(a)).ToList();

                // If any actions are missing, mailbox is non-compliant
                if (adminMissing.Any() || delegateMissing.Any() || ownerMissing.Any())
                {
                    this.AddAffectedEntity(mailbox);
                    passed = false;
                }
            }

            return passed ? CheckResult.NoFinding : CheckResult.Finding;
        }
    }
}