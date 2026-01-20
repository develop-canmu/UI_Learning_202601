using System;
using System.Linq;
using Pjfb.Extensions;
using UnityEngine;
using Pjfb.UI;
using UnityEngine.UI;

namespace Pjfb.Common
{
    public class MissionModalListItem : PoolListItemBase
    {
        #region InnerClass
        public class ItemParams : ItemParamsBase
        {
            public MissionProgressPairData missionProgressPairData;
            public Action<ItemParams> onClickReceiveButton;
            public Action<ItemParams> onClickChallengeButton;
        }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text countText;
        [SerializeField] private OmissionTextSetter countOmissionTextSetter;
        [SerializeField] private UIProgress progress;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject receiveButton, challengeButton, maskGameObject, maskCompleteGameObject, maskReceivedButton;
        [SerializeField] private TMPro.TMP_Text challengeButtonText;
        #endregion

        #region PrivateFields
        private ItemParams _itemParams;
        #endregion

        #region OverrideMethods
        public override void Init(ItemParamsBase itemParams)
        {
            _itemParams = (ItemParams)itemParams;
            UpdateDisplay(_itemParams);
            base.Init(itemParams);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            var missionData = itemParams.missionProgressPairData.missionData;
            var nullableMissionProgressData = itemParams.missionProgressPairData.nullableMissionProgressData;
            
            titleText.text = missionData.description;
            countText.text = string.Empty;
            BigValue currentProgress = new BigValue(nullableMissionProgressData?.passingCount ?? 0);
            BigValue requireProgressCount = new BigValue(missionData.requireCount);
            progress.SetProgress(min: BigValue.Zero, max: requireProgressCount , value: currentProgress);
            countText.text = $"{currentProgress.ToDisplayString(countOmissionTextSetter.GetOmissionData())}/{requireProgressCount.ToDisplayString(countOmissionTextSetter.GetOmissionData())}";
            
            if (missionData.prizeJson.Any())
            {
                var prizeData = missionData.prizeJson[0];
                prizeJsonView.SetView(prizeData);
                prizeJsonView.gameObject.SetActive(true);
            }
            else
            {
                prizeJsonView.gameObject.SetActive(false);
            }
            
            var now = AppTime.Now;
            var progressStatus = nullableMissionProgressData != null ? (MissionProgressStatus)nullableMissionProgressData.progressStatus : MissionProgressStatus.Progressing;
            var isOpenSchedule = _itemParams.missionProgressPairData.missionCategory.endAt.TryConvertToDateTime().IsFuture(now);
            var showChallengeButton = isOpenSchedule && progressStatus == MissionProgressStatus.Progressing &&
                                      itemParams.missionProgressPairData.missionData.linkEx.Any();
            if (showChallengeButton) {
                challengeButton.SetActive(true);
                challengeButtonText.text = StringValueAssetLoader.Instance[itemParams.missionProgressPairData.missionData.linkEx.StartsWith("openNews") ? "mission.open_news" : "mission.challenge"] ;
            } else challengeButton.SetActive(false);

            receiveButton.SetActive(progressStatus == MissionProgressStatus.ReceivingReward);
            maskGameObject.SetActive(progressStatus == MissionProgressStatus.End || progressStatus == MissionProgressStatus.Progressing && !isOpenSchedule);
            maskCompleteGameObject.SetActive(progressStatus == MissionProgressStatus.End);
            maskReceivedButton.SetActive(progressStatus == MissionProgressStatus.End);
        }
        #endregion
        
        #region EventListeners
        public void OnClickReceiveButton()
        {
            _itemParams?.onClickReceiveButton?.Invoke(_itemParams);
        }

        public void OnClickChallengeButton()
        {
            _itemParams?.onClickChallengeButton?.Invoke(_itemParams);
        }
        #endregion
    }
}