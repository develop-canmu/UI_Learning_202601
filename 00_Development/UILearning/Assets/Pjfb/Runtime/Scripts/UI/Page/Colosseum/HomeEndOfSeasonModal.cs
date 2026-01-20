using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using Pjfb.Extensions;

namespace Pjfb.Colosseum
{
    public class HomeEndOfSeasonModal : ModalWindow
    {
        public class Parameters
        {
            public ColosseumUserSeasonStatus[] userSeasonStatus;
            public Action onFinish;

            public Parameters(ColosseumUserSeasonStatus[] userSeasonStatus, Action onFinish)
            {
                this.userSeasonStatus = userSeasonStatus;
                this.onFinish = onFinish;
            }
        }

        private enum AnimationState
        {
            Non,
            EndOfSeason,
            Ranking,
            Rewards,
            Grade,
            Close,
        }         

        private const string OpenEndOfSeasonKey = "OpenEndOfSeason";
        private const string OpenRankingKey = "OpenResultRanking";
        private const string OpenRewardsKey = "OpenRewards";
        private const string OpenResultGradeUpKey = "OpenResultGradeUp";
        private const string OpenResultGradeDownKey = "OpenResultGradeDown";
        private const string OpenResultGradeKeepKey = "OpenResultGradeKeep";
        private const string OpenRewardIdleKey = "IdleRewards";
        private const string CloseKey = "Close";

        [SerializeField] TMP_Text periodText;
        [SerializeField] TMP_Text userNameText;
        [SerializeField] TMP_Text rankingText;
        [SerializeField] ScrollGrid scrollGrid;
        [SerializeField] ColosseumRankImage resultRankImage;
        [SerializeField] ColosseumRoomImage resultRoomImage;
        [SerializeField] ColosseumRoomImage beforeRoomImage;
        [SerializeField] ColosseumRoomImage afterRoomImage;
        [SerializeField] Animator animator;

        ColosseumUserSeasonStatus currentUserStatus;
        AnimationState animationState = AnimationState.Non;
        List<PrizeJsonGridItem.Data> rewardList;
        string currentAnimatorKey = "";
        Parameters parameters;

        public static void Open(Parameters parameters)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.HomeEndOfSeason, parameters);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (args != null)
            {
                this.parameters = (Parameters) args;
                var seasonResultList = parameters.userSeasonStatus;

                var sColosseumEventIdArray = seasonResultList.Select(season => season.sColosseumEventId).ToArray();
                await ColosseumManager.RequestReadFinished(sColosseumEventIdArray);
                currentUserStatus = seasonResultList[seasonResultList.Length - 1];

                UpdateView();

                SetAnimatorKey();
            }
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            animator.SetTrigger(currentAnimatorKey);
            base.OnOpened();
        }

        private void UpdateView()
        {

            var startAt = currentUserStatus.startAt.TryConvertToDateTime().GetDateTimeString();
            var endAt = currentUserStatus.endAt.TryConvertToDateTime().GetDateTimeString(); 
            periodText.text = string.Format(StringValueAssetLoader.Instance["pvp.period.result"],startAt,endAt);

            userNameText.text = UserDataManager.Instance.user.name;

            var ranking = currentUserStatus.ranking;
            rankingText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"],ranking);

            var gradeNumber = currentUserStatus.gradeNumber;
            var gradeAfter = currentUserStatus.gradeAfter;
            resultRoomImage.SetTexture(gradeNumber);
            beforeRoomImage.SetTexture(gradeNumber);
            afterRoomImage.SetTexture(gradeAfter);

            var eventMaster = MasterManager.Instance.colosseumEventMaster.FindData(currentUserStatus.mColosseumEventId);

            var rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(eventMaster.mColosseumGradeGroupId, gradeNumber, ranking);
            resultRankImage.SetTexture(rankNumber);

            var prizeMasterList = MasterManager.Instance.colosseumRankingPrizeMaster.GetColosseumRankingPrize(eventMaster.mColosseumRankingPrizeGroupId, gradeNumber);
            var prizeMaster = prizeMasterList.FirstOrDefault(data => data.rankTop <= ranking && data.rankBottom >= ranking);

            rewardList = new List<PrizeJsonGridItem.Data>();
            if (prizeMaster != null)
            {                
                foreach (var prize in prizeMaster.prizeJson)
                {
                    var data = new PrizeJsonGridItem.Data(prize);
                    rewardList.Add(data);
                }
                scrollGrid.SetItems(rewardList);
            }

        }

        private void SetAnimatorKey()
        {
            switch(animationState)
            {
                case AnimationState.Non:
                    animationState = AnimationState.EndOfSeason;
                    currentAnimatorKey = OpenEndOfSeasonKey;
                    break;
                case AnimationState.EndOfSeason:
                    animationState = AnimationState.Ranking;
                    currentAnimatorKey = OpenRankingKey;
                    break;
                case AnimationState.Ranking:                    
                    animationState = AnimationState.Rewards;
                    currentAnimatorKey = OpenRewardsKey;
                    if (rewardList.Count == 0)
                    {
                        OnClickNext();
                        return;
                    }
                    break;
                case AnimationState.Rewards:
                    var beforeGrade = currentUserStatus.gradeNumber;
                    var afterGrade = currentUserStatus.gradeAfter;
                    animationState = AnimationState.Grade;
                    if (beforeGrade < afterGrade)
                    {
                        currentAnimatorKey = OpenResultGradeUpKey;
                    }
                    else if (beforeGrade > afterGrade)
                    {
                        currentAnimatorKey = OpenResultGradeDownKey;
                    }
                    else
                    {
                        currentAnimatorKey = OpenResultGradeKeepKey;
                    }
                    break;
                default:
                    Close();
                    break;
            }
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            parameters?.onFinish?.Invoke();
        }

        private async UniTask PlayAnimation()
        {
            // 連打防止
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            try
            {
                SetAnimatorKey();
                await AnimatorUtility.WaitStateAsync(animator, currentAnimatorKey);
                if (animationState == AnimationState.Rewards)
                {
                    currentAnimatorKey = OpenRewardIdleKey;
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

        #region EventListener
        public void OnClickNext()
        {            
            PlayAnimation().Forget();
        }
        #endregion
    }
}