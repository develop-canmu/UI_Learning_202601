using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Common;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using Pjfb.UserData;
using Pjfb.Master;

namespace Pjfb.MatchResult
{
    public class MatchResultWinColosseumPage : Page
    {
        #region Params
        public class Data
        {
            public ColosseumAttackAPIResponse colosseumAttackAPIResponse;

            public Data(ColosseumAttackAPIResponse _colosseumAttackAPIResponse)
            {
                colosseumAttackAPIResponse = _colosseumAttackAPIResponse;
            }
        }
        #endregion

        #region SerializeFields
		[SerializeField] private Animator animator;
        [SerializeField] private GameObject middleLayerRoot;
        [SerializeField] private GameObject rewardPointRoot;
        [SerializeField] private PossessionItemUi rewardPointPossessionItemUi;
        [SerializeField] private ColosseumRankImage beforeRankImage;
        [SerializeField] private TMP_Text beforeColosseumRankingText;
        [SerializeField] private ColosseumRankImage afterRankImage;
        [SerializeField] private TMP_Text afterColosseumRankingText;
        [SerializeField] private Animator rankUpAnimator;
		[SerializeField] private GameObject passEffectBadge;
        #endregion

        protected Data data;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            SetView();
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            
           
            base.OnEnablePage(args);
        }
        
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        var historyLite = data.colosseumAttackAPIResponse.historyLite;
                        if (historyLite.rankBefore > historyLite.rankAfter)
                        {
                            rankUpAnimator.gameObject.SetActive(true);
                            rankUpAnimator.SetTrigger("Open");
                        }
                        else
                        {
                            middleLayerRoot.SetActive(true);
                            animator.SetTrigger("Open");
                        }
                        break;
                }
            }
            return base.OnMessage(value);
        }
        #endregion
        
        #region PrivateMethods

        private void SetView()
        {
            
            middleLayerRoot.SetActive(false);
            var targetPointId = ColosseumManager.RankMatchRewardPointId;
            long rewardValue = 0;
            var isActivePassEffect = false;
            foreach (var reward in data.colosseumAttackAPIResponse.prizeJsonList)
            {
                if (reward.args.mPointId == targetPointId)
                {
                    rewardValue += reward.args.value;
                    // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                    if (!isActivePassEffect && reward.args.correctRate > 0 && reward.args.valueOriginal > 0)
                    {
                        isActivePassEffect = true;
                    }
                }
            }
            
            rewardPointRoot.SetActive(true);
            rewardPointPossessionItemUi.SetCount(targetPointId, rewardValue);
            if (isActivePassEffect)
            {
                rewardPointPossessionItemUi.SetColor(ColorValueAssetLoader.Instance["highlight.orange"]);
            }
            
            TryShowAutoSellConfirmModal(data.colosseumAttackAPIResponse.autoSell);
            var result = data.colosseumAttackAPIResponse.historyLite;

            var mColosseumEventId = data.colosseumAttackAPIResponse.userSeasonStatus.mColosseumEventId;
            var colosseumEvent = MasterManager.Instance.colosseumEventMaster.FindData(mColosseumEventId);
            var seasonStatus = data.colosseumAttackAPIResponse.userSeasonStatus;
            
            var rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(
                colosseumEvent.mColosseumGradeGroupId, seasonStatus.gradeNumber, result.rankBefore);
            beforeColosseumRankingText.text = String.Format(StringValueAssetLoader.Instance["pvp.result.ranking"],result.rankBefore);
            beforeRankImage.SetTexture(rankNumber);

            rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(
                colosseumEvent.mColosseumGradeGroupId, seasonStatus.gradeNumber, result.rankAfter);
            afterColosseumRankingText.text = String.Format(StringValueAssetLoader.Instance["pvp.result.ranking"],result.rankAfter);
            afterRankImage.SetTexture(rankNumber);
            // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
            passEffectBadge.SetActive(data.colosseumAttackAPIResponse.prizeJsonList.Any(prize => prize.args.correctRate > 0 && prize.args.valueOriginal > 0));
        }

        private async void TryShowAutoSellConfirmModal(NativeApiAutoSell autoSell)
        {
            // 自動売却があるかどうかチェック
            if (autoSell?.prizeListGot != null && autoSell.prizeListSold != null &&
                (autoSell.prizeListGot.Length > 0 || autoSell.prizeListSold.Length > 0))
            {
                var autoSellModalData = new AutoSellConfirmModal.Data(autoSell);
                var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoSellConfirm, autoSellModalData);
                await modal.WaitCloseAsync();
            }
        }
        #endregion
        
        #region EventListeners
        public void OnClickNext()
        {
            ColosseumPage.OpenPage(true);
        }
        #endregion
    }
}
