using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

using System;
using System.Linq;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine.UI;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;

namespace Pjfb.LeagueMatch
{
    public class HomeEndOfInstantTournamentModal : ModalWindow
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
        // 進出演出を開く
        private static readonly string AnimationOpenResultAdvanceToNextLeague = "OpenResultAdvanceToNextLeague";
        // 敗退演出を開く
        private static readonly string AnimationOpenResultEndOfLeague = "OpenResultEndOfLeague";

        private enum StateType
        {
            Open,
            OpenRewardsClub,
            OpenRewardsClubMember,
            OpenResultNotification,
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
        private TMPro.TextMeshProUGUI nextLeagueTermText = null;
        
        // ステート
        private StateType state = StateType.Open;
        // パラメータ
        private Parameters parameters = null;
        
        // シーズンデータ
        private ColosseumUserSeasonStatus[] seasonStatusArray = null;
        
        // 再生中の配列番号
        private int seasonDataCount = 0;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            parameters = (Parameters)args;
            // ステート初期化
            state = StateType.Open;
            
            // イベントId
            List<long> eventIdList = new List<long>();
            foreach(ColosseumUserSeasonStatus season in parameters.SeasonStatusList)
            {
                eventIdList.Add(season.sColosseumEventId);
            }
            // 既読状態にする
            if(eventIdList.Count > 0)
            {
                await ColosseumManager.RequestReadFinished(eventIdList.ToArray());
            }
            
            // mColosseumEventIdでソート
            seasonStatusArray = parameters.SeasonStatusList.OrderBy(v => v.mColosseumEventId).ToArray();

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
            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], seasonStatusArray[seasonDataCount].groupSeasonStatus.ranking.ToString("N0"));
            // 報酬設定
            ColosseumEventMasterObject colosseumEventMaster = MasterManager.Instance.colosseumEventMaster.FindData(seasonStatusArray[seasonDataCount].groupSeasonStatus.mColosseumEventId);
            long groupId = colosseumEventMaster.mColosseumRankingPrizeGroupId;
            ColosseumRankingPrizeMasterContainer prizeMaster = MasterManager.Instance.colosseumRankingPrizeMaster;
            // クラブ報酬
            PrizeJsonWrap[] clubRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatusArray[seasonDataCount].groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.GuildRanking, seasonStatusArray[seasonDataCount].groupSeasonStatus.ranking);
            if(clubRankPrize != null)
            {
                clubRewardScrollGrid.SetItems(clubRankPrize);
            }
            // 個人報酬
            PrizeJsonWrap[] personalRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatusArray[seasonDataCount].groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.PersonalGuildRanking, seasonStatusArray[seasonDataCount].groupSeasonStatus.ranking);
            if(personalRankPrize != null)
            {
                personalRewardScrollGrid.SetItems(personalRankPrize.Select(prize => new PrizeJsonGridItem.Data(prize)).ToList());
            }
            
            // 最後の大会ではない
            if(!colosseumEventMaster.seriesOption.isLast)
            {
                // 次フェーズ番号
                long nextPhaseNumber = colosseumEventMaster.seriesOption.phaseNumber+1;
                ColosseumEventMasterObject nextColosseumEventMaster = null;
                foreach (ColosseumEventMasterObject master in MasterManager.Instance.colosseumEventMaster.values)
                {
                    // mColosseumEventSeriesIdが違う
                    if(colosseumEventMaster.mColosseumEventSeriesId != master.mColosseumEventSeriesId)
                    {
                        continue;
                    }
                    // phaseNumberが1つ大きいマスタ
                    if(master.seriesOption.phaseNumber == nextPhaseNumber)
                    {
                        nextColosseumEventMaster = master;
                        break;
                    }
                }

                // データが無い場合はログ出しておく
                if(nextColosseumEventMaster == null)
                {
                    CruFramework.Logger.LogError($"m_colosseum_eventにmColosseumEventSeriesId=={colosseumEventMaster.mColosseumEventSeriesId}かつphaseNumber=={nextPhaseNumber}が存在しません。");
                }
                else
                {
                    // 次回開催期間
                    nextLeagueTermText.text = string.Format(StringValueAssetLoader.Instance["common.term"], nextColosseumEventMaster.startAt.TryConvertToDateTime().ToString("yyyy/MM/dd HH:mm"), nextColosseumEventMaster.endAt.TryConvertToDateTime().ToString("yyyy/MM/dd HH:mm"));
                }
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
            // アニメーション再生中はボタンを押せなくする
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
                        state = StateType.OpenResultNotification;
                        
                        // 大会終了
                        ColosseumEventMasterObject mColosseumEvent = MasterManager.Instance.colosseumEventMaster.FindData(seasonStatusArray[seasonDataCount].groupSeasonStatus.mColosseumEventId);
                        if(mColosseumEvent.seriesOption.isLast)
                        {
                            // 次の大会があればそちらを表示する
                            if (seasonStatusArray.Length > seasonDataCount + 1)
                            {
                                seasonDataCount++;
                                UpdateView();
                                state = StateType.Open;
                                await PlayAnimation(AnimationOpen);
                                break;
                            }
                            await CloseAsync();
                            break;
                        }
                        
                        // 進出
                        if(seasonStatusArray[seasonDataCount].groupSeasonStatus.entryChainInfo.isChained)
                        {
                            await PlayAnimation(AnimationOpenResultAdvanceToNextLeague);
                            break;
                        }
                        // 敗退
                        else
                        {
                            await PlayAnimation(AnimationOpenResultEndOfLeague);
                            break;
                        }
                    }
                    
                    // 閉じる
                    case StateType.OpenResultNotification:
                    {
                        // 次の大会があればそちらを表示する
                        if (seasonStatusArray.Length > seasonDataCount + 1)
                        {
                            seasonDataCount++;
                            UpdateView();
                            state = StateType.Open;
                            await PlayAnimation(AnimationOpen);
                            break;
                        }
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