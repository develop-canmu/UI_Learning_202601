using Pjfb.Community;
using Pjfb.Common;
using Pjfb.LockedItem;
using Pjfb.Networking.API;
using Pjfb.PresentBox;
using Pjfb.Training;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {
    
    public partial class HomeGetDataAPIRequest : AppAPIRequestBase<HomeGetDataAPIPost, HomeGetDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HomeGetDataAPIResponse response ) {
            //コミュニティー設定
            CommunityManager.yellCount = response.todayYelledCount;
            CommunityManager.unViewedYellCount = response.unViewedYellCount;
            CommunityManager.unViewedChatCount = response.unViewedChatCount;
            CommunityManager.unViewedClubChatCount = response.unViewedGuildChatCount;
            CommunityManager.unViewedClubInfoCount = response.unViewedGuildInfoCount;
            //ミッション
            MissionManager.finishedMissionCount = response.finishedMissionCount;
            //プレゼント
            PresentBoxManager.unreceivedGiftCount = response.unreceivedGiftCount;
            LockedItemManager.unreceivedGiftCount = response.unreceivedGiftLockedCount;
            // 自動トレーニングの完了済みの数の更新
            TrainingManager.UpdateAutoTrainingCount(response.trainingAutoPendingStatusList);
                
            //メニューバッジ更新
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();

            Pjfb.UserData.UserDataManager.Instance.user.UpdateGuildData(response);
            
            // コロシアム
            UserDataManager.Instance.UpdateColosseumSeasonDataList(response.colosseum);
            // クラブロワイヤルバナー更新
            AppManager.Instance.UIManager.Header.UpdateClubRoyalBanner();

            Pjfb.Storage.LocalSaveManager.saveData.clubCheckNotificationData.UpdateDateTime(response);
            Pjfb.Storage.LocalSaveManager.Instance.SaveData();
            
        }
    }
}
