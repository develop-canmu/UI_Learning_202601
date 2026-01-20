using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetGuildChatAPIRequest : AppAPIRequestBase<CommunityGetGuildChatAPIPost, CommunityGetGuildChatAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetGuildChatAPIResponse response )
        {
            //未読ギルドチャット数の更新
            CommunityManager.unViewedClubChatCount = 0;
            CommunityManager.unViewedClubInfoCount = 0;
        }
    }
}
