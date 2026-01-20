using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Colosseum;
using Pjfb.Common;
using Pjfb.Community;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchMenuModal : ModalWindow
    {

        public class Arguments
        {
            private LeagueMatchInfo leagueMatchInfo = null;
            /// <summary>リーグマッチ情報</summary>
            public LeagueMatchInfo LeagueMatchInfo{get{return leagueMatchInfo;}}

            /// <summary>シーズンId</summary>
            public long SeasonId { get{return leagueMatchInfo.SeasonData.SeasonId;} }
            
            private long recentSeasonId;
            public long RecentSeasonId { get{return recentSeasonId;} }
            
            /// <summary>m_coloseum_event_id</summary>
            public long ColosseumEventId{get{return leagueMatchInfo.MColosseumEvent.id;}}
                 
            /// <summary>シーズン情報</summary>
            public ColosseumSeasonData SeasonData{get{return leagueMatchInfo.SeasonData;}}
            
            /// <summary>マスタ</summary>
            public ColosseumEventMasterObject MColosseumEvent{get{return leagueMatchInfo.MColosseumEvent;}}
            
            public Arguments(LeagueMatchInfo leagueMatchInfo, long recentSeasonId = -1)
            {
                this.leagueMatchInfo = leagueMatchInfo;
                this.recentSeasonId = recentSeasonId;
            }
        }

        private Arguments arguments = null;

        [SerializeField]
        private UIBadgeNotification chatButton;
        [SerializeField]
        private UIButton recordButton;
        [SerializeField]
        private UIButton resultSeasonButton;
        [SerializeField]
        private UIButton rewardConfirmButton;
        [SerializeField] 
        private UIButton rankingButton;

        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            arguments = (Arguments)args;
            
            bool hasSeasonData = arguments.SeasonData != null;
            bool isEntry = IsEntry();
            
            // シーズンデータがある時のみ有効
            recordButton.interactable = hasSeasonData && isEntry;
            resultSeasonButton.interactable = hasSeasonData && isEntry;
            // シーズンデータがある&&入れ替え戦をしてない場合有効
            rewardConfirmButton.interactable = hasSeasonData && arguments.LeagueMatchInfo.IsOnShiftBattle == false && isEntry;
            // チャットボタンのバッジ更新
            UpdateChatNotification();
            // ユーザー参加情報によってランキングボタンの有効無効を切り替える
            UpdateRankingButtonInteractable();
            await base.OnPreOpen(args, token);
        }

        /// <summary>
        /// UGUI
        /// 対戦履歴
        /// </summary>
        public void OnLeagueMatchRecordButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRecord, arguments);
        }
        
        /// <summary>
        /// UGUI
        /// 今季の戦績
        /// </summary>
        public void OnLeagueMatchResultSeasonButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchResultSeason, arguments);
        }
        
        /// <summary>
        /// UGUI
        /// 過去の戦績
        /// </summary>
        public void OnLeagueMatchResultPastSeasonButton()
        {
            // 過去戦績モーダルに必要なパラメータ作成
            ClubMatchPastRecordModal.PastRecordArguments args = new ClubMatchPastRecordModal.PastRecordArguments(
                new ColosseumClientHandlingType[]
                {
                    ColosseumClientHandlingType.LeagueMatch,
                    ColosseumClientHandlingType.InstantTournament
                });
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMatchPastRecord, args);
        }
        
        /// <summary>
        /// UGUI
        /// 報酬確認
        /// </summary>
        public void OnRewardConfirmButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRewardConfirm, arguments);
        }
        
        /// <summary>
        /// UGUI
        /// 交換所
        /// </summary>
        public void OnShopButton()
        {
            Shop.ShopExchangeModal.Open(arguments.MColosseumEvent.mCommonStoreCategoryId);
        }
        
        /// <summary>
        /// UGUI
        /// 遊び方
        /// </summary>
        public void OnHowToButton()
        {
            AppManager.Instance.TutorialManager.OpenLeagueMatchHowToPlayTutorialAsync().Forget();
        }
        
        /// <summary>
        /// UGUI
        /// チャット
        /// </summary>
        public void OnChatButton()
        {
            // モダールを閉じてページ移動
            Close();
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, null);
        }

        /// <summary>
        /// クラブチャットの更新チェック
        /// </summary>
        private void UpdateChatNotification()
        {
            var unViewedChatCount = CommunityManager.ShowClubChatBadge;
            chatButton.SetActive(unViewedChatCount);
        }
        
        /// <summary>
        /// UGUI
        /// ランキング確認
        /// </summary>
        public void OnRankingButton()
        {
            LeagueMatchRankingModal.Data data = new LeagueMatchRankingModal.Data();
            data.SeasonId = arguments.RecentSeasonId;
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRanking, data);
        }
        
        private void UpdateRankingButtonInteractable()
        {
            // ユーザー参加情報がない場合グレーアウト
            rankingButton.interactable = arguments.RecentSeasonId > 0 && IsEntry();
        }

        // エントリー済みかどうか
        private bool IsEntry()
        {
            // シーズンデータがない
            if(arguments.SeasonData == null) return false;
            
            // そもそも大会じゃないならエントリーの概念がない
            if(arguments.SeasonData.IsInstantTournament == false) return true;
            
            return arguments.LeagueMatchInfo.IsEntry;
        }
        
    }
}