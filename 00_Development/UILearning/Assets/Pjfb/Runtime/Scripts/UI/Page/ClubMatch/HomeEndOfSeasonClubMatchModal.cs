using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine.UI;

namespace Pjfb.ClubMatch
{
    public class HomeEndOfSeasonClubMatchModal : ModalWindow
    {
        #region Params
        public class Parameters
        {
            public ColosseumUserSeasonStatus[] clubSeasonStatusList;
            public Action onFinish;
        }

        private enum AnimationState
        {
            Non = 0,
            OpenEndOfSeason,
            OpenFinalRankingClub,
            OpenRewardsClubRank,
            OpenResultSeason,
            OpenResultClubRankDown,
            OpenResultClubRankKeep,
            OpenResultClubRankUp,
            OpenRewardsClubMember,
            OpenFinalRankingPersonal,
            OpenInformationNextSeason,
            Close
        }
        
        [SerializeField] TMP_Text seasonPeriodText;
        [SerializeField] TMP_Text clubRankText;
        [SerializeField] TMP_Text personalRankText;
        [SerializeField] TMP_Text personalScoreRankText;
        [SerializeField] TMP_Text nextSeasonPeriodText;
        [SerializeField] Image leftResultClubRankImage;
        [SerializeField] Image rightResultClubRankImage;
        [SerializeField] Image currentClubRankImage;
        [SerializeField] Image afterClubRankImage;
        [SerializeField] Image clubRankGauge;
        [SerializeField] ScrollGrid clubRankPrizeScrollGrid;
        [SerializeField] ScrollGrid clubPrizeScrollGrid;
        [SerializeField] Animator stateAnimator;
        [SerializeField] RectTransform rankPrizeContent;
        [SerializeField] HomeEndOfSeasonClubMatchPrizeIcon prizeObj;
        [SerializeField] Ease gaugeEaseType = Ease.InOutCirc;
        [SerializeField] TMP_Text buttonText = null;
        
        long MaxGrade => MasterManager.Instance.colosseumGradeMaster.values.Max(c => c.gradeNumber);
        long MinGrade => MasterManager.Instance.colosseumGradeMaster.values.Min(c => c.gradeNumber);
        AnimationState _animationState = AnimationState.Non;
        AnimationState _rankChangeState = AnimationState.OpenResultClubRankUp;
        Parameters _parameters;
        ColosseumUserSeasonStatus _clubSeasonStatus;
        Dictionary<long, Sprite> clubRankSpriteCache = new();
        bool _isSkipNextSeasonView = false;

        #endregion
        
        #region Life Cycle
        public static void Open(Parameters parameters)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HomeEndOfSeasonClubMatch, parameters);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (args != null)
            {
                _parameters = (Parameters) args;
                var sColosseumEventIdArray = _parameters.clubSeasonStatusList.Select(season => season.groupSeasonStatus.sColosseumEventId).ToArray();
                await ColosseumManager.RequestReadFinished(sColosseumEventIdArray);
                _clubSeasonStatus = _parameters.clubSeasonStatusList?.Last();
                UpdateView();
            }
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            OnClickNext();
            base.OnOpened();
        }
        
        protected override void OnClosed()
        {
            base.OnClosed();
            _parameters?.onFinish?.Invoke();
        }
        
        #endregion
        
        #region EventListener
        public async void OnClickNext()
        {            
            SetAnimatorKey();
            //次のシーズンの表示がスキップされる場合はそこで終了
            if( _isSkipNextSeasonView && _animationState == AnimationState.OpenInformationNextSeason ) {
                Close();
                return;
            }
            Debug.LogWarning(_animationState.ToString());
            stateAnimator.SetTrigger(_animationState.ToString());
            
            //アニメーション中ボタン押し防止
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            try
            {
                await AnimatorUtility.WaitStateAsync(stateAnimator, _animationState.ToString());
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
        /// Animation Event用、ゲージアニメーション動画
        /// </summary>
        public void ClubRankGaugeAnimation()
        {
            if (_clubSeasonStatus == null) return;
            var startGrade = (1 + _clubSeasonStatus.groupSeasonStatus.pointBefore / 100f);
            var endGrade =  (1 + _clubSeasonStatus.groupSeasonStatus.pointAfter / 100f);
            startGrade = Mathf.Clamp(startGrade, MinGrade, MaxGrade);
            endGrade = Mathf.Clamp(endGrade, MinGrade, MaxGrade);
            var durationPerLevel = 1 / Mathf.Abs(endGrade - startGrade);
            AnimateRanks(startGrade,endGrade,durationPerLevel,gaugeEaseType);
        }
        
        /// <summary>
        /// ゲージアニメーション処理
        /// </summary>
        private void AnimateRanks(float startRank,float endRank,float durationPerLevel,Ease easeType = Ease.Linear)
        {
            float distance = endRank - startRank;
            float direction = Mathf.Sign(distance);
            float startDecimal = startRank - (int)startRank;
            startDecimal = (direction < 0 && startDecimal % 1 == 0) ? 1 : startDecimal;
            float endDecimal = endRank - (int)endRank;
            int nextInt = (direction < 0) ? 0 : 1;
            float toInt = Mathf.Abs(nextInt - startDecimal);
            float toTarget =  Mathf.Abs(distance);
            float endValue = (toInt > toTarget) ? endDecimal : nextInt;
            float beginValue = (direction < 0 && startDecimal % 1 == 0) ? 1 : startDecimal;
            float diff = endValue - beginValue;
            float duration = Mathf.Abs(diff) * durationPerLevel;
            
            clubRankGauge.fillAmount = beginValue;
            clubRankGauge.DOFillAmount(endValue, duration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    startRank += diff;
                    long key = (long)startRank;
                    bool hasKey = clubRankSpriteCache.ContainsKey(key);
                    
                    if (hasKey)
                    {
                        rightResultClubRankImage.sprite = clubRankSpriteCache[key];
                    }

                    if (toInt < toTarget) AnimateRanks(startRank,endRank,durationPerLevel,easeType);
                });
        }
        
        /// <summary>
        /// ボタンの文字変更コールバック
        /// </summary>
        public void OnChangeButtonText()
        {
            //次のシーズンの表示がスキップされる場合はボタンの文字を変える
            if( _isSkipNextSeasonView ) {
                buttonText.text = StringValueAssetLoader.Instance["common.close"];
            }
        }

        #endregion
        
        #region Other
        
        /// <summary>
        /// 画面要素設定
        /// </summary>
        private void UpdateView()
        {
            if (_clubSeasonStatus == null) return;
            //Text設定
            clubRankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],_clubSeasonStatus.groupSeasonStatus.ranking.ToString("N0"));
            personalRankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],_clubSeasonStatus.ranking.ToString("N0"));
            personalScoreRankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],_clubSeasonStatus.scoreRanking.ToString("N0"));
            var eventMaster = MasterManager.Instance.colosseumEventMaster.FindData(_clubSeasonStatus.groupSeasonStatus.mColosseumEventId);
            var startAtDateTime = _clubSeasonStatus.startAt.TryConvertToDateTime();
            var endAtDateTime = _clubSeasonStatus.endAt.TryConvertToDateTime(); 
            var eventEndAt = eventMaster.endAt.TryConvertToDateTime();
            seasonPeriodText.text = string.Format(StringValueAssetLoader.Instance["pvp.period.result"],startAtDateTime.GetDateTimeString(),endAtDateTime.GetDateTimeString());
            if (eventMaster.cycleDays != 0)
            {
                //周期の次回開催期間設定
                var nextStartAtDateTime = startAtDateTime.AddDays(eventMaster.cycleDays);
                // PvPの仕様の名残でシーズン開始は1時から始まるがターンは0時から計算する
                var nextEndAtDateTime = nextStartAtDateTime.Date + TimeSpan.FromDays(eventMaster.cycleDays - eventMaster.intervalMarginDays) - TimeSpan.FromTicks(1);
                nextSeasonPeriodText.text = (eventEndAt >= nextEndAtDateTime) ? string.Format(StringValueAssetLoader.Instance["pvp.period.result"],nextStartAtDateTime.GetDateTimeString(),nextEndAtDateTime.GetDateTimeString()) : "";
            }
            else
            {
                nextSeasonPeriodText.text = "";
            }
            _isSkipNextSeasonView = string.IsNullOrEmpty(nextSeasonPeriodText.text);
            
            
            //After Rank State設定
            long gradeBefore = _clubSeasonStatus.groupSeasonStatus.gradeBefore;
            long gradeAfter = _clubSeasonStatus.groupSeasonStatus.gradeAfter;
            if (gradeBefore == gradeAfter) _rankChangeState = AnimationState.OpenResultClubRankKeep;
            else if(gradeBefore > gradeAfter) _rankChangeState = AnimationState.OpenResultClubRankDown;
            else _rankChangeState = AnimationState.OpenResultClubRankUp;

            //報酬設定
            var groupId = MasterManager.Instance.colosseumEventMaster.FindData(_clubSeasonStatus.groupSeasonStatus.mColosseumEventId).mColosseumRankingPrizeGroupId;
            var prizeMaster = MasterManager.Instance.colosseumRankingPrizeMaster;
            var clubRankPrize = prizeMaster.GetRankingPrizeJson(groupId, _clubSeasonStatus.groupSeasonStatus.gradeNumber,4, _clubSeasonStatus.groupSeasonStatus.ranking);
            if (clubRankPrize != null)
            {
                if (clubRankPrize.Length > 3)
                {
                    clubRankPrizeScrollGrid.SetItems(clubRankPrize.ToList());
                }
                else
                {
                    clubRankPrize.ForEach(prize => Instantiate(prizeObj, rankPrizeContent).Init(prize).Forget());
                }
            }

            var clubPrize = prizeMaster.GetRankingPrizeJson(groupId, _clubSeasonStatus.groupSeasonStatus.gradeNumber,(int)ColosseumClientHandlingType.ClubMatch, _clubSeasonStatus.groupSeasonStatus.ranking);
            if (clubPrize != null) clubPrizeScrollGrid.SetItems(clubPrize.Select(prize => new PrizeJsonGridItem.Data(prize)).ToList());

            //クラブランク画像設定
            GetClubRankSpriteCacheAndReset(gradeBefore, gradeAfter).Forget();
        }

        /// <summary>
        /// 次のアニメーションState更新
        /// </summary>
        private void SetAnimatorKey()
        {
            switch(_animationState)
            {
                case AnimationState.OpenFinalRankingClub:
                    if (clubRankPrizeScrollGrid.GetItems().Count > 0 || rankPrizeContent.childCount > 0)
                    {
                        _animationState = AnimationState.OpenRewardsClubRank;
                    }
                    else
                    {
                        _animationState = AnimationState.OpenResultSeason;
                    } 
                    break;
                case AnimationState.OpenResultSeason:
                    _animationState = _rankChangeState;
                    break;
                case AnimationState.OpenResultClubRankDown:
                case AnimationState.OpenResultClubRankKeep:
                    _animationState = AnimationState.OpenRewardsClubMember;
                    break;
                default:
                    _animationState++;
                    break;
            }
        }
        
        private async UniTask GetClubRankSpriteCacheAndReset(long gradeBefore, long gradeAfter)
        {
            long minValue = Math.Min(gradeBefore, gradeAfter);
            long maxValue = Math.Max(gradeBefore, gradeAfter);
            for(var i= minValue; i <= maxValue; i++)
            {
                long tempRank =  MasterManager.Instance.guildRankMaster.GetRankIdByGradeNumber(i);
                if (tempRank > 0 && !clubRankSpriteCache.ContainsKey(i))
                {
                    var rankIcon = await ClubUtility.LoadRankIcon(tempRank,this.GetCancellationTokenOnDestroy());
                    clubRankSpriteCache[i] = rankIcon;
                }
            }

            //クラブランク画像設定
            // シーズンリザルト
            currentClubRankImage.sprite = clubRankSpriteCache[gradeBefore];
            afterClubRankImage.sprite = clubRankSpriteCache[gradeAfter];
            // ランク結果（昇格等）
            leftResultClubRankImage.sprite = clubRankSpriteCache[gradeBefore];
            // Animationで変化される為初期値は前の状態
            rightResultClubRankImage.sprite = clubRankSpriteCache[gradeBefore];
        }
        #endregion
    }
}