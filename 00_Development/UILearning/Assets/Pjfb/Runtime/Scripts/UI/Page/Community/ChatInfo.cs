using Pjfb.Networking.App.Request;

namespace Pjfb.Community
{
    public class ChatInfo
    {
        public enum ChatType
        {
            None,
            Personal,
            Club,
            ClubLog,
        }

        public ChatType Type { get; private set; }
        public long UMasterId { get; private set; } = 0;
        public long ChatId { get; private set; } = 0;
        public string CreatedAt { get; private set; }
        public MessageType MessageType { get; private set; }
        public string MessageText { get; private set; }
        public string UserName { get; private set; }
        public long MIconId { get; private set; }
        
        public ChatInfo(ModelsUChat uChat, string userName, long mIconId)
        {
            Type = ChatType.Personal;
            
            ChatId = uChat.id;
            UMasterId = uChat.fromUMasterId;
            CreatedAt = uChat.createdAt;
            MessageType = (MessageType)uChat.type;
            MessageText = uChat.body;
            
            UserName = userName;
            MIconId = mIconId;
        }

        public ChatInfo(ModelsGChat gChat, UserChatUserStatus status)
        {
            Type = ChatType.Club;
            
            ChatId = gChat.id;
            UMasterId = gChat.uMasterId;
            CreatedAt = gChat.createdAt;
            MessageType = (MessageType)gChat.type;
            MessageText = gChat.message;

            UserName = status.name;
            MIconId = status.mIconId;
        }

        public ChatInfo(GuildLogGuildActionLog gActionLog)
        {
            Type = ChatType.ClubLog;

            CreatedAt = gActionLog.createdAt;
            MessageText = gActionLog.message;
        }
    }
}