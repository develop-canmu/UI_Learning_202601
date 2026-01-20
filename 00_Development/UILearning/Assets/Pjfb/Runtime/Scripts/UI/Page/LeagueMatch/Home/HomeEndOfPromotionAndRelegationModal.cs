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
    public class HomeEndOfPromotionAndRelegationModal : ModalWindow
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
        private static readonly string AnimationOpen = "OpenEndOfPromotionAndRelegation";
        // 勝ち
        private static readonly string AnimationWin = "OpenResultMatchWin";
        // 負け
        private static readonly string AnimationLose = "OpenResultMatchLose";
        // 昇格
        private static readonly string AnimationPromotion = "OpenResultLeagueRankPromotion";
        // 降格
        private static readonly string AnimationRelegation = "OpenResultLeagueRankRelegation";
        // 残留
        private static readonly string AnimationRankKeepUpper = "OpenResultLeagueRankKeepUpper";
        // 残留
        private static readonly string AnimationRankKeepLower = "OpenResultLeagueRankKeepLower";
        
        private enum StateType
        {
            Open,
            Result,
            RankResult,
            Close
        }

        [SerializeField]
        private Animator targetAnimator = null;
        [SerializeField]
        private ClubRankImage currentRankImage = null;
        [SerializeField]
        private ClubRankImage afterRankImage = null;
        
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
            // 開く
            await PlayAnimation(AnimationOpen);
            
            state = StateType.Result;
            // 結果でアニメーション分岐
            switch(seasonStatus.groupSeasonStatus.shiftMatchInfo.result)
            {
                case ColosseumManager.ResultWin:
                    await PlayAnimation(AnimationWin);
                    break;
                case ColosseumManager.ResultLose:
                    await PlayAnimation(AnimationLose);
                    break;
                default:
                    Debug.LogError($"LeagueMatch result is {seasonStatus.groupSeasonStatus.shiftMatchInfo.result}.");
                    Close();
                    break;
            }
            OnOpened();
        }
        
        
        private void UpdateView()
        {
            // ランクアイコン
            currentRankImage.SetTexture( seasonStatus.groupSeasonStatus.gradeBefore );
            afterRankImage.SetTexture( seasonStatus.groupSeasonStatus.gradeAfter );
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
            // アニメーション再生中
            AppManager.Instance.UIManager.System.TouchGuard.Show();

            try
            {
                switch(state)
                {
                    // 結果
                    case StateType.Result:
                    {
                        state = StateType.RankResult;
                        // 相手のが上
                        bool isGradeUpper = seasonStatus.gradeNumber < seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber;
                    
                        // 結果でアニメーション分岐
                        switch(seasonStatus.groupSeasonStatus.shiftMatchInfo.result)
                        {
                            case ColosseumManager.ResultWin:
                            {
                                // 格上勝利
                                if(isGradeUpper)
                                {
                                    await PlayAnimation(AnimationPromotion);
                                }
                                else
                                {
                                    await PlayAnimation(AnimationRankKeepUpper);
                                }
                                break;
                            }
                            case ColosseumManager.ResultLose:
                            {
                                // 格上に敗北
                                if(isGradeUpper)
                                {
                                    await PlayAnimation(AnimationRankKeepLower);
                                }
                                else
                                {
                                    await PlayAnimation(AnimationRelegation);
                                }
                                break;
                            }
                        }
                        break;
                    }
                
                    // 閉じる
                    case StateType.RankResult:
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