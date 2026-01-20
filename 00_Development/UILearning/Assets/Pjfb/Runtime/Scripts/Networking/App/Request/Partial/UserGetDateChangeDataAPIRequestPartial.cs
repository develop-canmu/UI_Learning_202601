using System.Linq;
using Pjfb.Community;
using Pjfb.Networking.API;
using UnityEngine;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserGetDateChangeDataAPIRequest : AppAPIRequestBase<UserGetDateChangeDataAPIPost, UserGetDateChangeDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserGetDateChangeDataAPIResponse response )
        {
            //followedCanYellCount更新
            var followCanYell = response.followUMasterIdList.Where(v => response.todayYelledUMasterIdList.All(t => t != v));
            CommunityManager.yellDetail.todayYelledList =
                response.todayYelledUMasterIdList.Select(id => new ModelsUYell { uMasterId = id }).ToArray();
            CommunityManager.yellDetail.followedCanYellCount = followCanYell.Count();
        }
    }
}
