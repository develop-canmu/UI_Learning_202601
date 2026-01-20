using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.UI;
using Pjfb.UserData;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.Colosseum
{
    public class ColosseumRankingModal : ModalWindow
    {
        [SerializeField] private ColosseumStatusUi colosseumStatusUi;
        [SerializeField] private PoolListContainer rankingContainer;

        private ColosseumSeasonData currentColosseumSeasonData;
        
        public static void Open(ColosseumSeasonData colosseumSeasonData)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ColosseumRanking,colosseumSeasonData);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentColosseumSeasonData = (ColosseumSeasonData)args;
            colosseumStatusUi.SetView(currentColosseumSeasonData);
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            UpdateView().Forget();
            base.OnOpened();
        }

        private async UniTask UpdateView()
        {

            var season = currentColosseumSeasonData.UserSeasonStatus;
            if (season == null)
            {
                return;
            }

            var gradeMaster = MasterManager.Instance.colosseumGradeMaster.GetJoinedColosseumGradeMasterObject(
                currentColosseumSeasonData.MColosseumEvent.mColosseumGradeGroupId, season.gradeNumber);
            
            var rankingUserList = await ColosseumManager.RequestGetRankingAsync(season.sColosseumEventId,gradeMaster.roomCapacity);
            
            var data = new List<ColosseumRankingItem.Data>();
            if (rankingUserList == null || rankingUserList.Length == 0)
            {
                return;
            }
            
            var borderType = BorderLineType.Non;
            var borderRank = 0L;

            if (gradeMaster.promotionRankBottom == -1)
            {
                borderType = BorderLineType.ResidueLine;
                borderRank = (gradeMaster.demotionRankTop-1);
            }
            else 
            {
                borderType = BorderLineType.PromotionLine;
                borderRank = gradeMaster.promotionRankBottom;
            }
            
            foreach (var user in rankingUserList)
            {
                var cellBorderType = borderRank == user.ranking ? borderType : BorderLineType.Non;
                data.Add(new ColosseumRankingItem.Data
                {
                    userData = user,colosseumSeasonData = currentColosseumSeasonData, disableOnClickAction = true, borderLineType = cellBorderType,
                    OnSizeChanged = rankingContainer.RefreshView
                });
            }
            
            rankingContainer.SetDataList(data,slideIn:false).Forget();

        }

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        #endregion
    }
}
