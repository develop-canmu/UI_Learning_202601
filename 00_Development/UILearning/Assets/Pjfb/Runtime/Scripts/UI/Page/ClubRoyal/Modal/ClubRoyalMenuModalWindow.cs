using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Community;
using Pjfb.ClubMatch;
using Pjfb.LeagueMatch;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using UnityEngine;

namespace Pjfb.ClubRoyal
{
    /// <summary>クラブ・ロワイヤルのメニューモーダル</summary>
    public class ClubRoyalMenuModalWindow : ModalWindow
    {
        /// <summary>メニューモーダルから各モーダルへ渡すデータをまとめた型</summary>
        public class ClubRoyalInfo
        {
            private LeagueMatchInfo matchInfo = null;
            public LeagueMatchInfo MatchInfo => matchInfo;
            
            private GroupLeagueMatchGroupStatusDetail groupStatusDetail = null;
            public GroupLeagueMatchGroupStatusDetail GroupStatusDetail => groupStatusDetail;
            
            public ClubRoyalInfo(LeagueMatchInfo matchInfo, GroupLeagueMatchGroupStatusDetail groupStatusDetail)
            {
                this.matchInfo = matchInfo;
                this.groupStatusDetail = groupStatusDetail;
            }
        }
        
        [SerializeField]
        private UIBadgeNotification chatButton;
        
        [SerializeField]
        private UIButton rewardConfirmButton;
        
        /// <summary>キャッシュ</summary>
        private ClubRoyalInfo clubRoyalInfo = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // モーダルを開いた際にデータをキャッシュする
            clubRoyalInfo = (ClubRoyalInfo)args;
            
            bool hasSeasonData = clubRoyalInfo.MatchInfo.SeasonData != null;
            
            // チャットボタンのバッチ表示切り替え
            chatButton.SetActive(CommunityManager.ShowClubChatBadge);
            
            // シーズンデータがある、かつ入れ替え戦ではない場合は報酬ボタンをアクティブにする
            rewardConfirmButton.interactable = hasSeasonData && clubRoyalInfo.MatchInfo.IsOnShiftBattle == false;
            
            return base.OnPreOpen(args, token);
        }

        /// <summary>試合履歴を開くボタン</summary>
        public void OnClickMatchHistory()
        {
            // 試合履歴モーダルに必要なパラメータ作成
            long seasonId = clubRoyalInfo.MatchInfo.SeasonData.SeasonId;
            string myClubName = clubRoyalInfo.MatchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.name;
            long myClubEmblemId = clubRoyalInfo.MatchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.mGuildEmblemId;
            ClubRoyalMatchHistoryModalWindow.MatchHistoryArguments arguments = new ClubRoyalMatchHistoryModalWindow.MatchHistoryArguments(seasonId, myClubName, myClubEmblemId);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalRecord, arguments);
        }
        
        /// <summary>過去戦績を開くボタン</summary>
        public void OnClickPastRecord()
        {
            // 過去戦績モーダルに必要なパラメータ作成
            ClubMatchPastRecordModal.PastRecordArguments args = new ClubMatchPastRecordModal.PastRecordArguments(
                new ColosseumClientHandlingType[]
                {
                    ColosseumClientHandlingType.ClubRoyal
                });
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchPastRecord, args);
        }
        
        /// <summary>報酬確認を開くボタン</summary>
        public void OnClickRewardConfirmButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalRewardConfirm, clubRoyalInfo);
        }

        /// <summary>交換所を開くボタン</summary>
        public void OnClickShopExchangeButton()
        {
            // 交換所で表示するアイテムのカテゴリIDを渡す
            ShopExchangeModal.Open(clubRoyalInfo.MatchInfo.MColosseumEvent.mCommonStoreCategoryId);
        }

        /// <summary>遊び方を開くボタン</summary>
        public void OnClickHowToPlayButton()
        {
            HowToPlayUtility.OpenHowToPlayModal((long)HowToPlayUtility.TutorialType.ClubRoyal, StringValueAssetLoader.Instance["club-royal.tutorial.title"]).Forget();
        }
        
        /// <summary>チャットを開くボタン</summary>
        public void OnClickChatButton()
        {
            // Pageを開く前にモーダルを閉じる
            Close();
            
            // クラブ・ロワイヤルのページをスタックしてクラブチャットを開く
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, null);
        }
        
        /// <summary>閉じるボタン</summary>
        public void OnClickCloseButton()
        {
            Close();
        }
    }
}