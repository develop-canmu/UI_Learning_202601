using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

using System;
using System.Linq;
using Pjfb.Colosseum;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine.UI;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;

namespace Pjfb.LeagueMatch
{
    public class HomeEndOfSeasonLeagueMatchModal : ModalWindow
    {
    
        public class Parameters
        {
            private ColosseumUserSeasonStatus[] seasonStatusList = null;
            /// <summary>シーズンデータ</summary>
            public ColosseumUserSeasonStatus[] SeasonStatusList{get{return seasonStatusList;}}
            
            private Action onFinish = null;
            /// <summary>終了</summary>
            public Action OnFinish{get{return onFinish;}}
            
            public Parameters(ColosseumUserSeasonStatus[] seasonStatusList, Action onFinish)
            {
                this.seasonStatusList = seasonStatusList;
                this.onFinish = onFinish;
            }
        }
    
        // 演出を開く
        private static readonly string AnimationOpen = "OpenEndOfSeason";
        // 報酬を開く
        private static readonly string AnimationOpenRewardsClub = "OpenRewardsClub";
        // メンバー報酬を開く
        private static readonly string AnimationOpenRewardsClubMember = "OpenRewardsClubMember";
        
        // 上位との入れ替え戦
        private static readonly string AnimationOpenResultPromotionMatch = "OpenResultLeagueRankPromotionMatch";
        // 下位との入れ替え戦
        private static readonly string AnimationOpenResultRelegationMatch = "OpenResultLeagueRankRelegationMatch";
        // SSランク時の残留
        private static readonly string AnimationOpenResultLeagueRankKeepSS = "OpenResultLeagueRankKeepSS";
        // 残留
        private static readonly string AnimationOpenResultLeagueRankKeepOther = "OpenResultLeagueRankKeepOther";


        private enum StateType
        {
            Open,
            OpenRewardsClub,
            OpenRewardsClubMember,
            OpenRankBattleNotification,
            Close
        }

        [SerializeField]
        private Animator targetAnimator = null;
        
        [SerializeField]
        private TMPro.TextMeshProUGUI rankText = null;
        
        [SerializeField]
        private ScrollGrid clubRewardScrollGrid = null;
        [SerializeField]
        private ScrollGrid personalRewardScrollGrid = null;
        
        [SerializeField]
        private ClubRankImage currentRankImage = null;
        [SerializeField]
        private ClubRankImage afterRankImage = null;
        
        [SerializeField]
        private ClubEmblemImage clubEmblemImage = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI clubNameText = null;
        
        // ステート
        private StateType state = StateType.Open;
        // パラメータ
        private Parameters parameters = null;
        
        // シーズンデータ
        private ColosseumUserSeasonStatus seasonStatus = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            parameters = (Parameters)args;
            // ステート初期化
            state = StateType.Open;
            
            // イベントId
            List<long> eventIdList = new List<long>();
            foreach(ColosseumUserSeasonStatus season in parameters.SeasonStatusList)
            {
#if UNITY_EDITOR
                if(season.sColosseumEventId < 0)continue;
#endif
                eventIdList.Add(season.sColosseumEventId);
            }
            // 既読状態にする
            if(eventIdList.Count > 0)
            {
                await ColosseumManager.RequestReadFinished(eventIdList.ToArray());
            }
            
            // 最新の情報を保持
            seasonStatus = parameters.SeasonStatusList.OrderByDescending(v => v.startAt).FirstOrDefault();
            
            // ビューの更新
            UpdateView();
        
            await base.OnPreOpen(args, token);
        }
        
        protected override async UniTask OnOpen(CancellationToken token)
        {
            // 開くアニメーション
            await PlayAnimation(AnimationOpen);
            OnOpened();
        }
        
        private void UpdateView()
        {
            // ランキング
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], seasonStatus.groupSeasonStatus.ranking.ToString("N0"));
            
            // 報酬設定
            long groupId = MasterManager.Instance.colosseumEventMaster.FindData(seasonStatus.groupSeasonStatus.mColosseumEventId).mColosseumRankingPrizeGroupId;
            ColosseumRankingPrizeMasterContainer prizeMaster = MasterManager.Instance.colosseumRankingPrizeMaster;
            PrizeJsonWrap[] clubRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatus.groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.GuildRanking, seasonStatus.groupSeasonStatus.ranking);
            // クラブ報酬
            if(clubRankPrize != null)
            {
                clubRewardScrollGrid.SetItems(clubRankPrize);
            }
            // 個人報酬
            PrizeJsonWrap[] personalRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatus.groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.PersonalGuildRanking, seasonStatus.groupSeasonStatus.ranking);
                        
            if(personalRankPrize != null)
            {
                personalRewardScrollGrid.SetItems(personalRankPrize.Select(prize => new PrizeJsonGridItem.Data(prize)).ToList());
            }

            // ランクアイコン
            currentRankImage.SetTexture( seasonStatus.gradeNumber );
            afterRankImage.SetTexture( seasonStatus.gradeAfter );
            
            // 入れ替え戦の情報
            if(seasonStatus.groupSeasonStatus.shiftMatchInfo != null)
            {
                clubNameText.text = seasonStatus.groupSeasonStatus.shiftMatchInfo.name;
                clubEmblemImage.SetTexture( seasonStatus.groupSeasonStatus.shiftMatchInfo.mGuildEmblemId );
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            NextAsync().Forget();
        }
        
        private async UniTask NextAsync()
        {
            // アニメーション再生中はボタンを押せないようにする
            AppManager.Instance.UIManager.System.TouchGuard.Show();

            try
            {
                switch(state)
                {
                    // クラブ報酬を開く
                    case StateType.Open:
                    {
                        if(clubRewardScrollGrid.GetItems().Count > 0)
                        {
                            state = StateType.OpenRewardsClub;
                            await PlayAnimation(AnimationOpenRewardsClub);
                        }
                        else
                        {
                            state = StateType.OpenRewardsClubMember;
                            await PlayAnimation(AnimationOpenRewardsClubMember);
                        }
                        break;
                    }
                    
                    // メンバー報酬を開く
                    case StateType.OpenRewardsClub:
                    {
                        state = StateType.OpenRewardsClubMember;
                        await PlayAnimation(AnimationOpenRewardsClubMember);
                        break;
                    }
                    
                    // 昇格戦降格線の通知
                    case StateType.OpenRewardsClubMember:
                    {
                        state = StateType.OpenRankBattleNotification;
                        
                        // 残留
                        if(seasonStatus.groupSeasonStatus.shiftMatchInfo == null || seasonStatus.groupSeasonStatus.shiftMatchInfo.sColosseumGroupStatusId <= 0)
                        {
                            // 最大ランク
                            if(seasonStatus.gradeNumber == ColosseumManager.GetTopRank(seasonStatus.mColosseumEventId))
                            {
                                await PlayAnimation(AnimationOpenResultLeagueRankKeepSS);
                            }
                            // その他
                            else
                            {
                                await PlayAnimation(AnimationOpenResultLeagueRankKeepOther);
                            }
                        }
                        // 昇格戦
                        else if(seasonStatus.gradeNumber < seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber)
                        {
                            await PlayAnimation(AnimationOpenResultPromotionMatch);
                        }
                        // 降格戦
                        else if(seasonStatus.gradeNumber > seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber)
                        {
                            await PlayAnimation(AnimationOpenResultRelegationMatch);
                        }
                
                        break;
                    }
                    
                    // 閉じる
                    case StateType.OpenRankBattleNotification:
                    {
                        state = StateType.Close;
                        await CloseAsync();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                CruFramework.Logger.LogError(e.Message);
                throw;
            }
            finally
            {
                // ボタンを押せるように
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
            }
        }

        protected override void OnClosed()
        {
            if(parameters.OnFinish != null)
            {
                parameters.OnFinish();
            }
            base.OnClosed();
        }

        private UniTask PlayAnimation(string name)
        {
            return AnimatorUtility.WaitStateAsync(targetAnimator, name);
        }
    }
}