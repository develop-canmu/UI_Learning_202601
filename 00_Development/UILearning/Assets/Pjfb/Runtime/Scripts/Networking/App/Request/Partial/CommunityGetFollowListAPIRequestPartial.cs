using System.Collections.Generic;
using System.Linq;
using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunityGetFollowListAPIRequest : AppAPIRequestBase<CommunityGetFollowListAPIPost, CommunityGetFollowListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunityGetFollowListAPIResponse response )
        {
            //フォローリスト保存
            CommunityManager.followUserList = new HashSet<UserCommunityUserStatus>(response.communityUserStatusList);
        }
    }
}
