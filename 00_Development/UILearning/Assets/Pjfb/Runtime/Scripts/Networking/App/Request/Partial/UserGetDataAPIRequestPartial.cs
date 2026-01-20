using System.Collections.Generic;
using System.Linq;
using CruFramework;
using Pjfb.Community;
using Pjfb.Event;
using Pjfb.Networking.API;
using Pjfb.Shop;
using Pjfb.Storage;
using Pjfb.Story;
using Pjfb.UserData;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserGetDataAPIRequest : AppAPIRequestBase<UserGetDataAPIPost, UserGetDataAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserGetDataAPIResponse response ) {
            // 利用規約同意
            LocalSaveManager.saveData.isTermsAgreed = response.user.isTermsAgreed;
            // ユーザーデータ更新
            UserDataManager.Instance.user.Update(response.user);
            // コンフィグ更新
            ConfigManager.Instance.UpdateByResponseData(response.conf);
            // 初回のみConfig取得後所持ジェム数を設定
            UserDataManager.Instance.point.UpdateGemValue();
            //フォローリスト保存
            CommunityManager.followUserList = new HashSet<UserCommunityUserStatus>(response.followUMasterIdList.Select(id => new UserCommunityUserStatus{uMasterId = id}));
            //followedCanYellCount更新
            var followCanYell = response.followUMasterIdList.Where(v => response.todayYelledUMasterIdList.All(t => t != v));
            CommunityManager.yellDetail.todayYelledList =
                response.todayYelledUMasterIdList.Select(id => new ModelsUYell { uMasterId = id }).ToArray();
            CommunityManager.yellDetail.followedCanYellCount = followCanYell.Count();
            CommunityManager.guildMemberIdList = response.guildUMasterIdList.ToList();
            //guildCanYellCount更新
            CommunityManager.UpdateGuildCanYellCount();
            DeckUtility.SetDeckDataDictionary(response.deckList, response.useTypePartyNumberList);
            EventManager.Instance.SetFestivalEffectStatus(response.festivalEffectStatusList);
            UserDataManager.Instance.UpdatePushSettingList(response.pushSettingList);
            StoryManager.Instance.UpdateResumeData(response.huntPending);
            
            // プレイヤー強化状況を更新
            PlayerEnhanceManager.UpdateEnhanceData(response.playerEnhanceList);
            
            // セーブデータ保存
            LocalSaveManager.Instance.SaveData();
        }
    }
}
