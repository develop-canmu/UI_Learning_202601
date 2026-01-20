using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UI;
using Pjfb.UserData;

using UniRx;

namespace Pjfb.Colosseum
{
    public class ColosseumRewardModal : ModalWindow
    {
        [SerializeField] private ColosseumStatusUi colosseumStatusUi;
        [SerializeField] protected ScrollGrid scroller;

        private ColosseumSeasonData colosseumSeasonData;
        
        public static void Open(ColosseumSeasonData colosseumSeasonData)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ColosseumRewardConfirm,colosseumSeasonData);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            colosseumSeasonData = (ColosseumSeasonData)args;
            colosseumStatusUi.SetView(colosseumSeasonData);
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            UpdateView();
            base.OnOpened();
        }
        
        protected virtual void UpdateView()
        {
            var seasonStatus = colosseumSeasonData.UserSeasonStatus;
            var ranking = seasonStatus.ranking;

            var rewardList =
                MasterManager.Instance.colosseumRankingPrizeMaster.GetColosseumRankingPrize(
                    colosseumSeasonData.MColosseumEvent.mColosseumRankingPrizeGroupId, seasonStatus.gradeNumber);

            var rankingRewardList = new List<RewardScrollData>();
            foreach (var reward in rewardList)
            {
                var rankingReward = new RewardScrollData();
                rankingReward.rankTop = reward.rankTop;
                rankingReward.rankBottom = reward.rankBottom;
                rankingReward.myRank = reward.rankTop <= ranking && reward.rankBottom >= ranking;
                rankingReward.prizeList = reward.prizeJson;
                rankingRewardList.Add(rankingReward);
            }
            scroller.SetItems(rankingRewardList);
        }
        

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        #endregion
    }
}