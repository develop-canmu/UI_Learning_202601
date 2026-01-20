using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;
using PrizeJsonWrap = Pjfb.Networking.App.Request.PrizeJsonWrap;

namespace Pjfb.ClubRoyal
{
    public class HomeEndOfSeasonClubRoyalModal : ModalWindow
    {
        public class Param
        {
            private ColosseumUserSeasonStatus[] seasonStatusList = null;
            /// <summary>シーズンデータ</summary>
            public ColosseumUserSeasonStatus[] SeasonStatusList{get{return seasonStatusList;}}
            
            private Action onFinish = null;
            /// <summary>終了</summary>
            public Action OnFinish{get{return onFinish;}}
            
            public Param(ColosseumUserSeasonStatus[] seasonStatusList, Action onFinish)
            {
                this.seasonStatusList = seasonStatusList;
                this.onFinish = onFinish;
            }
        }
        
        private static readonly string AnimationOpen = "OpenEndOfSeason";
        private static readonly string AnimationOpenFinalRanking = "OpenFinalRanking";
        private static readonly string AnimationOpenInfoChangeBattle = "OpenInfoChangeBattle";
        private static readonly string AnimationOpenResultClubRankUp = "OpenResultClubRankUp";
        private static readonly string AnimationOpenResultClubRankKeep = "OpenResultClubRankKeep";
        private static readonly string AnimationOpenResultClubRankDown = "OpenResultClubRankDown";
        
        private enum StateType
        {
            Open,
            OpenFinalRanking,
            OpenInfoChangeBattle,
            OpenResultChangeBattle,
            Close,
        }
        
        [SerializeField]
        private Animator targetAnimator = null;
        
        [SerializeField]
        private ScrollGrid clubRewardScrollGrid = null;
        [SerializeField]
        private ScrollGrid personalRewardScrollGrid = null;
        [SerializeField]
        private TextMeshProUGUI textSeasonDates = null;
        [SerializeField]
        private GameObject[] rewardsClubRanking = null;
        [SerializeField]
        private GameObject rewardsClubNoneItem = null;
        [SerializeField]
        private GameObject rewardsClubMemberNoneItem = null;
        [SerializeField]
        private GameObject rewardsPresent = null;
        [SerializeField]
        private GameObject rankUpBattle = null;
        [SerializeField]
        private GameObject rankDownBattle = null;
        [SerializeField]
        private ClubRankImage currentRankImage = null;
        [SerializeField]
        private ClubRankImage changeRankImage = null;
        [SerializeField]
        private ClubRankImage afterRankImage = null;
        [SerializeField]
        private TextMeshProUGUI changeBattleText = null;
        [SerializeField]
        private TextMeshProUGUI changeBattleResultText = null;
        [SerializeField]
        private GameObject changeBattleRankUp = null;
        [SerializeField]
        private GameObject changeBattleRankDown = null;
        [SerializeField]
        private GameObject changeBattleRankKeep = null;
        [SerializeField]
        private GameObject changeBattleRankKeepSS = null;
        
        // パラメータのキャッシュ
        private Param param = null;
        // モーダルの状態
        private StateType state = StateType.Open;
        
        // 最新のシーズンデータのキャッシュ
        private ColosseumUserSeasonStatus seasonStatus = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Param)args;
            
            // イベントId
            List<long> eventIdList = new List<long>();
            foreach(ColosseumUserSeasonStatus season in param.SeasonStatusList)
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
            seasonStatus = param.SeasonStatusList.OrderByDescending(v => v.startAt).FirstOrDefault();
            
            // ビューの更新
            UpdateView();
            
            // ステート初期化
            state = StateType.Open;
            
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
            // シーズンの期間を設定
            DateTime startAt = seasonStatus.startAt.TryConvertToDateTime();
            DateTime endAt = seasonStatus.endAt.TryConvertToDateTime();
            textSeasonDates.text = string.Format(StringValueAssetLoader.Instance["club-royal.result"], startAt.GetNewsDateTimeString(), endAt.GetNewsDateTimeString());
            
            // クラブランキングの表示
            for(int i = 0; i < rewardsClubRanking.Length; i++)
            {
                rewardsClubRanking[i].SetActive(i == seasonStatus.groupSeasonStatus.ranking - 1);
            }
            // クラブの報酬設定
            long groupId = MasterManager.Instance.colosseumEventMaster.FindData(seasonStatus.groupSeasonStatus.mColosseumEventId).mColosseumRankingPrizeGroupId;
            ColosseumRankingPrizeMasterContainer prizeMaster = MasterManager.Instance.colosseumRankingPrizeMaster;
            Master.PrizeJsonWrap[] clubRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatus.groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.GuildRanking, seasonStatus.groupSeasonStatus.ranking);
            // クラブ報酬
            if(clubRankPrize != null)
            {
                clubRewardScrollGrid.SetItems(clubRankPrize);
                rewardsClubNoneItem.SetActive(false);
            }
            else
            {
                rewardsClubNoneItem.SetActive(true);
            }
            // 個人の報酬設定
            Master.PrizeJsonWrap[] personalRankPrize = prizeMaster.GetRankingPrizeJson(groupId, seasonStatus.groupSeasonStatus.gradeNumber, ColosseumRankingPrizeCauseType.PersonalGuildRanking, seasonStatus.groupSeasonStatus.ranking);
                        
            if(personalRankPrize != null)
            {
                personalRewardScrollGrid.SetItems(personalRankPrize.Select(prize => new PrizeJsonGridItem.Data(prize)).ToList());
                rewardsClubMemberNoneItem.SetActive(false);
                rewardsPresent.SetActive(true);
            }
            else
            {
                rewardsClubMemberNoneItem.SetActive(true);
                rewardsPresent.SetActive(false);
            }

            // 入れ替え戦に参加する場合は昇格戦か降格戦かセットする
            if (ColosseumManager.IsDecisionShiftMatch(seasonStatus))
            {
                rankUpBattle.SetActive(seasonStatus.gradeNumber < seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber);
                rankDownBattle.SetActive(seasonStatus.gradeNumber > seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber);
                
                // 入れ替え戦のランクアイコンをセットする
                currentRankImage.SetTexture(seasonStatus.gradeNumber);
                changeRankImage.SetTexture(seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber);
                
                // 入れ替え戦のメッセージをセットする
                if (seasonStatus.gradeNumber > seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber)
                {
                    changeBattleText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-down_shift_battle.message"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.gradeNumber));
                }
                else
                {
                    changeBattleText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.grade-up_shift_battle.message"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber));
                }
            }
            
            // 最終的なランクアイコンをセットする
            afterRankImage.SetTexture(seasonStatus.groupSeasonStatus.gradeAfter);
        }
        
        /// <summary>
        /// 次へボタンが押された際の処理
        /// </summary>
        public void OnNextButton()
        {
            NextAsync().Forget();
        }
        
        /// <summary>
        /// アニメーションを再生して次の状態に遷移する
        /// </summary>
        private async UniTask NextAsync()
        {
            // アニメーション再生中はボタンを押せないようにする
            AppManager.Instance.UIManager.System.TouchGuard.Show();

            try
            {
                switch (state)
                {
                    // シーズン報酬か入れ替え戦の結果を表示
                    case StateType.Open:
                        // 入れ替え戦の結果がある場合は結果を表示
                        if (ColosseumManager.IsFinishShiftMatch(seasonStatus))
                        {
                            // リザルト表示
                            state = StateType.OpenResultChangeBattle;
                            await ShowResultChangeBattle();
                        }
                        else
                        {
                            // シーズン報酬表示
                            state = StateType.OpenFinalRanking;
                            await PlayAnimation(AnimationOpenFinalRanking);
                        }
                
                        break;
                    
                    // 入れ替え戦に参加する場合は情報を表示
                    // 入れ替え戦なしで昇格していたら結果を表示
                    case StateType.OpenFinalRanking:
                        if(ColosseumManager.IsDecisionShiftMatch(seasonStatus))
                        {
                            await PlayAnimation(AnimationOpenInfoChangeBattle);
                            state = StateType.OpenInfoChangeBattle;
                        }
                        else if (ColosseumManager.IsGradeUpNoShiftMatch(seasonStatus))
                        {
                            // 入れ替え戦なしで昇格
                            changeBattleRankUp.SetActive(true);
                            changeBattleResultText.text = StringValueAssetLoader.Instance["club-royal.result.grade-up_no_shift_battle"];
                            await PlayAnimation(AnimationOpenResultClubRankUp);
                            state = StateType.OpenResultChangeBattle;
                        }
                        else
                        {
                            state = StateType.Close;
                            await CloseAsync();
                        }
                        break;
                    
                    // すでに入れ替え戦の結果が出ている場合は結果を表示
                    case StateType.OpenInfoChangeBattle:
                        if(ColosseumManager.IsFinishShiftMatch(seasonStatus))
                        {
                            await ShowResultChangeBattle();
                            state = StateType.OpenResultChangeBattle;
                        }
                        else
                        {
                            state = StateType.Close;
                            await CloseAsync();
                        }
                        break;
                    
                    // 閉じる
                    case StateType.OpenResultChangeBattle:
                        state = StateType.Close;
                        await CloseAsync();
                        break;
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

        /// <summary>
        /// 入れ替え戦の結果表示
        /// </summary>
        private async UniTask ShowResultChangeBattle()
        {
            // 相手が格上かどうか
            bool isGradeUpper = seasonStatus.gradeNumber < seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber;
            bool isWin = seasonStatus.groupSeasonStatus.shiftMatchInfo.result == ColosseumManager.ResultWin;
            bool isLose = seasonStatus.groupSeasonStatus.shiftMatchInfo.result == ColosseumManager.ResultLose;
            
            // 最高ランクの取得
            long topRank = ColosseumManager.GetTopRank(seasonStatus.mColosseumEventId);
            
            // 昇格・降格・維持の出しわけ
            changeBattleRankUp.SetActive(isWin && isGradeUpper);
            changeBattleRankDown.SetActive(isLose && !isGradeUpper);
            changeBattleRankKeep.SetActive(seasonStatus.gradeNumber == seasonStatus.groupSeasonStatus.gradeAfter && seasonStatus.gradeNumber != topRank);
            changeBattleRankKeepSS.SetActive(seasonStatus.gradeNumber == seasonStatus.groupSeasonStatus.gradeAfter && seasonStatus.gradeNumber == topRank);
            
            // 結果でアニメーション分岐
            if(isWin)
            {
                // 格上に勝利した場合はランクアップ
                if (isGradeUpper)
                {
                    changeBattleResultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.result.grade-up_shift_battle.win"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber));
                    await PlayAnimation(AnimationOpenResultClubRankUp);
                }
                else
                {
                    changeBattleResultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.result.grade-down_shift_battle.win"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.gradeNumber));
                    await PlayAnimation(AnimationOpenResultClubRankKeep);
                }
            }
            else if(isLose)
            {
                // 格下に敗北した場合はランクダウン
                if (isGradeUpper)
                {
                    changeBattleResultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.result.grade-up_shift_battle.lose"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber));
                    await PlayAnimation(AnimationOpenResultClubRankKeep);
                }
                else
                {
                    changeBattleResultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.result.grade-down_shift_battle.lose"], ColosseumManager.GetGradeName(seasonStatus.mColosseumEventId, seasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber));
                    await PlayAnimation(AnimationOpenResultClubRankDown);
                }
            }
            // 引き分け
            else
            {
                changeBattleResultText.text = string.Format(StringValueAssetLoader.Instance["club-royal.announce.shift_battle.draw.message"]);
                await PlayAnimation(AnimationOpenResultClubRankKeep);
            }
        }

        /// <summary>
        /// モーダルが閉じられる際の処理
        /// </summary>
        protected override void OnClosed()
        {
            if(param.OnFinish != null)
            {
                param.OnFinish();
            }
            base.OnClosed();
        }
        
        /// <summary>
        /// アニメーション再生
        /// </summary>
        private UniTask PlayAnimation(string name)
        {
            return AnimatorUtility.WaitStateAsync(targetAnimator, name);
        }
    }
}