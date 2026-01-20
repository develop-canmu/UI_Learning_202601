using System.Linq;
using Pjfb.Common;
using Pjfb.Community;
using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CommunitySendYellAPIRequest : AppAPIRequestBase<CommunitySendYellAPIPost, CommunitySendYellAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CommunitySendYellAPIResponse response ) 
        {
            //エール情報更新
            CommunityManager.yellCount += response.yellResult.successCount;
            var oldList = CommunityManager.yellDetail.todayYelledList.ToList();
            var addList = response.yellResult.yelledUMasterIdList.Select(id => new ModelsUYell{uMasterId = id}).ToList();
            CommunityManager.yellDetail.todayYelledList = oldList.Union(addList).ToArray();
            CommunityManager.yellDetail.followedCanYellCount = CommunityManager.followUserList.Count(f =>
                CommunityManager.yellDetail.todayYelledList.All(yell => yell.uMasterId != f.uMasterId));
            //guildCanYellCount更新
            CommunityManager.UpdateGuildCanYellCount();
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            MissionManager.Instance.OnCommunitySendYellFinished();
        }
    }
}
