using System.Linq;
using Pjfb.Community;
using Pjfb.Networking.API;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetChatListAPIRequest : AppAPIRequestBase<CommunityGetChatListAPIPost, CommunityGetChatListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetChatListAPIResponse response )
        {
            //未読個人チャット数の更新
            CommunityManager.unViewedChatCount = response.uChatList.Count(chat => chat.fromUMasterId != UserDataManager.Instance.user.uMasterId && chat.readFlg == (int)ReadFlgType.NotView);
        }
    }
}
