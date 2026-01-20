using System.Collections.Generic;
using System.Linq;
using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetBlockListAPIRequest : AppAPIRequestBase<CommunityGetBlockListAPIPost, CommunityGetBlockListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetBlockListAPIResponse response ) {
            //ブロックリスト保存
            CommunityManager.blockUserList = new HashSet<UserCommunityUserStatus>(response.communityUserStatusList);
            CommunityManager.isGetBlockUserList = true;
        }
    }
}
