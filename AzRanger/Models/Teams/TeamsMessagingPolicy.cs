using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzRanger.Models.Teams
{
    public class TeamsMessagingPolicy
    {
        public object Description { get; set; }
        public bool AllowUrlPreviews { get; set; }
        public bool AllowOwnerDeleteMessage { get; set; }
        public bool AllowUserEditMessage { get; set; }
        public bool AllowUserDeleteMessage { get; set; }
        public bool UsersCanDeleteBotMessages { get; set; }
        public bool AllowUserDeleteChat { get; set; }
        public bool AllowUserChat { get; set; }
        public bool AllowRemoveUser { get; set; }
        public bool AllowGiphy { get; set; }
        public string GiphyRatingType { get; set; }
        public bool AllowGiphyDisplay { get; set; }
        public bool AllowPasteInternetImage { get; set; }
        public bool AllowMemes { get; set; }
        public bool AllowImmersiveReader { get; set; }
        public bool AllowStickers { get; set; }
        public bool AllowUserTranslation { get; set; }
        public string ReadReceiptsEnabledType { get; set; }
        public bool AllowPriorityMessages { get; set; }
        public bool AllowSmartReply { get; set; }
        public bool AllowSmartCompose { get; set; }
        public string ChannelsInChatListEnabledType { get; set; }
        public string AudioMessageEnabledType { get; set; }
        public string ChatPermissionRole { get; set; }
        public bool AllowFullChatPermissionUserToDeleteAnyMessage { get; set; }
        public bool AllowFluidCollaborate { get; set; }
        public bool AllowVideoMessages { get; set; }
        public bool AllowCommunicationComplianceEndUserReporting { get; set; }
        public bool AllowChatWithGroup { get; set; }
        public bool AllowSecurityEndUserReporting { get; set; }
        public string InOrganizationChatControl { get; set; }
        public bool AllowGroupChatJoinLinks { get; set; }
        public bool CreateCustomEmojis { get; set; }
        public bool UseB2BInvitesToAddExternalUsers { get; set; }
        public bool AllowProactiveSummaries { get; set; }
        public bool DeleteCustomEmojis { get; set; }
        public string AutoShareFilesInExternalChats { get; set; }
        public string DesignerForBackgroundsAndImages { get; set; }
        public bool AllowCustomGroupChatAvatars { get; set; }
        public Key Key { get; set; }
        public string Identity { get; set; }
        public Configmetadata ConfigMetadata { get; set; }
        public string ConfigId { get; set; }
    }
}
