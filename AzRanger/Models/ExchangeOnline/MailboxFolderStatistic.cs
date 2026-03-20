using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.ExchangeOnline
{
    public class MailboxFolderStatistic
    {
        public string Datedatatype { get; set; }
        public DateTime? Date { get; set; }
        public string CreationTimedatatype { get; set; }
        public DateTime? CreationTime { get; set; }
        public string LastModifiedTimedatatype { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string Name { get; set; }
        public string FolderPath { get; set; }
        public string FolderId { get; set; }
        public string ParentFolderId { get; set; }
        public string FolderType { get; set; }
        public bool ContentFolder { get; set; }
        public string ContentMailboxGuiddatatype { get; set; }
        public string ContentMailboxGuidodatatype { get; set; }
        public string ContentMailboxGuid { get; set; }
        public string RawContentMailboxGuiddatatype { get; set; }
        public string RawContentMailboxGuidodatatype { get; set; }
        public string RawContentMailboxGuid { get; set; }
        public bool Movable { get; set; }
        public bool RecoverableItemsFolder { get; set; }
        public object AssociatedIPMFolderPath { get; set; }
        public string ContainerClass { get; set; }
        public string Flags { get; set; }
        public string TargetQuota { get; set; }
        public string StorageQuota { get; set; }
        public string StorageWarningQuota { get; set; }
        public string VisibleItemsInFolderdatatype { get; set; }
        public int VisibleItemsInFolder { get; set; }
        public string HiddenItemsInFolderdatatype { get; set; }
        public int HiddenItemsInFolder { get; set; }
        public string ItemsInFolderdatatype { get; set; }
        public int ItemsInFolder { get; set; }
        public string DeletedItemsInFolderdatatype { get; set; }
        public int DeletedItemsInFolder { get; set; }
        public string FolderSize { get; set; }
        public string ItemsInFolderAndSubfoldersdatatype { get; set; }
        public int ItemsInFolderAndSubfolders { get; set; }
        public string DeletedItemsInFolderAndSubfoldersdatatype { get; set; }
        public int DeletedItemsInFolderAndSubfolders { get; set; }
        public string FolderAndSubfolderSize { get; set; }
        public string CurrentSchemaVersion { get; set; }
        public object OldestItemReceivedDate { get; set; }
        public object NewestItemReceivedDate { get; set; }
        public object OldestDeletedItemReceivedDate { get; set; }
        public object NewestDeletedItemReceivedDate { get; set; }
        public object OldestItemLastModifiedDate { get; set; }
        public object NewestItemLastModifiedDate { get; set; }
        public object OldestDeletedItemLastModifiedDate { get; set; }
        public object NewestDeletedItemLastModifiedDate { get; set; }
        public object ManagedFolder { get; set; }
        public object DeletePolicy { get; set; }
        public object ArchivePolicy { get; set; }
        public object CompliancePolicy { get; set; }
        public string RetentionFlags { get; set; }
        public string TopSubject { get; set; }
        public string TopSubjectSize { get; set; }
        public string TopSubjectCountdatatype { get; set; }
        public int TopSubjectCount { get; set; }
        public string TopSubjectClass { get; set; }
        public string TopSubjectPath { get; set; }
        public object TopSubjectReceivedTime { get; set; }
        public string TopSubjectFrom { get; set; }
        public string TopClientInfoForSubject { get; set; }
        public string TopClientInfoCountForSubjectdatatype { get; set; }
        public int TopClientInfoCountForSubject { get; set; }
        public object SearchFolders { get; set; }
        public string AuditAuxMailboxGuid { get; set; }
        public string AuditFolderStubSize { get; set; }
        public object LastMovedTimeStamp { get; set; }
        public object LowLatencyContainerId { get; set; }
        public object LowLatencyContainerFlags { get; set; }
        public string LowLatencyContainerQuota { get; set; }
        public bool SearchFolder { get; set; }
        public string Identity { get; set; }
        public object ConversationNamespace { get; set; }
        public object IsFailedMovedFolder { get; set; }
        public object LastScannedDocumentId { get; set; }
        public object ElcTeamsScanCompleted { get; set; }
        public string ElcRecrawlState { get; set; }
        public string ElcRecrawlStateReadMode { get; set; }
        public string WhenLabeleddatatype { get; set; }
        public DateTime? WhenLabeled { get; set; }
        public string SearchFolderState { get; set; }
        public object Diagnostics { get; set; }
        public object DiagnosticInfo { get; set; }
        public bool IsValid { get; set; }
        public string ObjectState { get; set; }
    }

}
