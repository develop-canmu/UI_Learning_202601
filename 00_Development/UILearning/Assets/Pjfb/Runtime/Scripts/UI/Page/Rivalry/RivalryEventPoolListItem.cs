using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;
using static Pjfb.Rivalry.RivalryManager;

namespace Pjfb.Rivalry
{
    public class RivalryEventPoolListItem : PoolListItemBase
    {
        #region InnerClass
        public class ItemParams : ItemParamsBase
        {
            public HuntEnemyMasterObject huntEnemyMasterObject;
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public HuntDifficultyMasterObject huntDifficultyMasterObject;
            public HuntEnemyHistory huntEnemyHistory;
            public List<HuntEnemyMasterObject> huntEnemyList;
            public List<HuntEnemyPrizeMasterObject> huntEnemyPrizeList;
            public Action<ItemParams> onClickItemParams;
            public SeenUnlockedEventMatchDataContainer seenUnlockedEventMatchDataContainer;
            public Action onUnlockAnimFinished;
            
            public Status status { get; }
            public int huntEnemyIndex { get; }
            
            
            public bool IsUnlocked()
            {
                return 
                    huntMasterObject.isClosedOnceWin &&
                    status == Status.New &&
                    !seenUnlockedEventMatchDataContainer.seenUnlockedDataList.Exists(aData => aData.Equals(huntEnemyMasterObject.id, huntTimetableMasterObject.id));

            }
            
            public bool IsLocked()
            {
                return status == Status.Locked;
            }
            
            public ItemParams(HuntEnemyMasterObject huntEnemyMasterObject, HuntMasterObject huntMasterObject, HuntTimetableMasterObject huntTimetableMasterObject, HuntDifficultyMasterObject huntDifficultyMasterObject, HuntEnemyHistory huntEnemyHistory, ChallengedEventMatchDataContainer challengedEventMatchDataContainer, SeenUnlockedEventMatchDataContainer seenUnlockedEventMatchDataContainer, List<HuntEnemyMasterObject> huntEnemyList, List<HuntEnemyPrizeMasterObject> huntEnemyPrizeList, Action<ItemParams> onClickItemParams, Action onUnlockAnimFinished = null)
            {
                this.huntEnemyMasterObject = huntEnemyMasterObject;
                this.huntMasterObject = huntMasterObject;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.huntDifficultyMasterObject = huntDifficultyMasterObject;
                this.huntEnemyHistory = huntEnemyHistory;
                this.huntEnemyList = huntEnemyList;
                this.huntEnemyPrizeList = huntEnemyPrizeList;
                this.onClickItemParams = onClickItemParams;
                this.seenUnlockedEventMatchDataContainer = seenUnlockedEventMatchDataContainer;

                huntEnemyIndex = huntEnemyList.IndexOf(huntEnemyMasterObject);
                status = 
                    huntEnemyHistory.mHuntEnemyIdList.Contains(this.huntEnemyMasterObject.id) ? Status.Clear :
                    huntMasterObject.isClosedOnceWin && huntEnemyIndex > 0 && !huntEnemyHistory.mHuntEnemyIdList.Contains(huntEnemyList[huntEnemyIndex-1].id) ? Status.Locked :
                    challengedEventMatchDataContainer.challengedDataList.Exists(aData => aData.Equals(this.huntEnemyMasterObject.id, huntTimetableMasterObject.id)) ? Status.Losing :
                    Status.New;

                this.onUnlockAnimFinished = onUnlockAnimFinished;
            }
        }

        public enum Status
        {
            New, // 未挑戦
            Clear,
            Losing,
            Locked
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text titleText, descriptionText, powerText, weakenPowerText, prizeCount, towerLockConditionText;
        [SerializeField] private OmissionTextSetter powerOmissionTextSetter;
        [SerializeField] private OmissionTextSetter weakenPowerOmissionTextSetter;
        [SerializeField] private PrizeJsonView prizeJsonView, firstTimePrizeJsonView;
        [SerializeField] private GameObject newIconGameObject, clearIconGameObject, firstTimeGetIconGameObject, towerClearGameObject, towerLockGameObject, rewardRootGameObject, firstTimeRewardRootGameObject;
        [SerializeField] private RivalryEventFrameImage bannerImage;
        [SerializeField] private Animator towerLockAnimator;
        [SerializeField] private CallAnimationEventAction lockAnimationEventAction;
        
        // 弱体化時の戦力表示オブジェクト
        [SerializeField]
        private GameObject weakenPowerRoot = null;

        // 戦力表示切り替えクロスフェード
        [SerializeField]
        private CanvasGroupDOTweenCrossFade crossFade = null;
        
        #endregion

        private ItemParams _itemParams;

        /// <summary> アニメーション同期キー </summary>
        public static readonly string CrossFadeSyncKey = "Rivalry_RecommendPower";
        
        public override void Init(ItemParamsBase value)
        {
            _itemParams = (ItemParams) value;
            
            descriptionText.text = _itemParams.huntEnemyMasterObject.subName;
            
            // 通常の推奨戦力表示
            BigValue combatPowerRecommended = new BigValue(_itemParams.huntEnemyMasterObject.combatPowerRecommended);
            powerText.text = combatPowerRecommended.ToDisplayString(powerOmissionTextSetter.GetOmissionData());

            // 条件タイプ取得
            HuntDeckRegulationType regulationType = RivalryManager.GetRegulationType(_itemParams.huntEnemyMasterObject.mHuntDeckRegulationId);
            
            // 弱体化条件設定時
            if(regulationType == HuntDeckRegulationType.Weaken && string.IsNullOrEmpty(_itemParams.huntEnemyMasterObject.specialDisplay) == false)
            {
                // 弱体時の推奨戦力
                BigValue combatWeakenPowerRecommended = new BigValue(_itemParams.huntEnemyMasterObject.specialDisplay);
                weakenPowerText.text = combatWeakenPowerRecommended.ToDisplayString(weakenPowerOmissionTextSetter.GetOmissionData());
                weakenPowerRoot.SetActive(true);
            }
            // 弱体化条件以外は非表示
            else
            {
                weakenPowerRoot.SetActive(false);
            }
            
            // クロスフェード開始
            crossFade.PlaySync(CrossFadeSyncKey);
            
            
            bannerImage.SetTexture(_itemParams.huntDifficultyMasterObject.imageId);
            titleText.text = _itemParams.huntDifficultyMasterObject?.name ?? string.Empty;
            
            var isClear = _itemParams.status == Status.Clear;
            firstTimeGetIconGameObject.SetActive(isClear);
            clearIconGameObject.SetActive(isClear);
            newIconGameObject.SetActive(_itemParams.status == Status.New);
            towerClearGameObject.SetActive(_itemParams.huntMasterObject.isClosedOnceWin && isClear);
            towerLockConditionText.text = string.Format(StringValueAssetLoader.Instance["rivalry.event.unlock"]);
            var eventMPointId = _itemParams.huntTimetableMasterObject.mPointId;
            var firstTimePrize = _itemParams.huntEnemyPrizeList.FirstOrDefault(prize => prize.type == (int)HuntEnemyPrizeMasterObject.Type.FirstTime)?.prizeJson[0];
            var eventPointPrize = _itemParams.huntEnemyPrizeList.FirstOrDefault(prize =>
                prize.prizeJson.Any(prizeJsonWrap => prizeJsonWrap.args.mPointId == eventMPointId))?.prizeJson[0]
                ?? new PrizeJsonWrap{type = "point", args = new Master.PrizeJsonArgs{mPointId = eventMPointId}};
            if (firstTimePrize == null)
            {
                firstTimeRewardRootGameObject.SetActive(false);
            }
            else
            {
                firstTimeRewardRootGameObject.SetActive(true);
                firstTimePrizeJsonView.gameObject.SetActive(true);
                firstTimePrizeJsonView.SetView(new PrizeJsonViewData(firstTimePrize));   
            }
            if (eventPointPrize == null || eventMPointId <= 0)
            {
                rewardRootGameObject.SetActive(false);
            }
            else
            {
                rewardRootGameObject.SetActive(true);
                prizeJsonView.transform.parent.gameObject.SetActive(true);
                prizeCount.text = eventPointPrize.args.value.ToString();
                prizeJsonView.SetView(new PrizeJsonViewData(eventPointPrize));    
            }
            if (_itemParams.IsUnlocked())
            {
                towerLockGameObject.SetActive(true);
                lockAnimationEventAction.AnimationEventAction = _itemParams.onUnlockAnimFinished;
                towerLockAnimator.SetTrigger("Unlocked");
                seenUnlockedEventMatchDataContainer.OnSeenUnlockedAnimation(_itemParams.huntEnemyMasterObject.id, _itemParams.huntTimetableMasterObject.id);
            }
            else if (_itemParams.IsLocked())
            {
                towerLockGameObject.SetActive(true);
                towerLockAnimator.SetTrigger("Locked");
            }
            else
            {
                towerLockGameObject.SetActive(false);
            }
        }

        #region EventListener
        public void OnClickListItem()
        {
            _itemParams?.onClickItemParams?.Invoke(_itemParams);
        }
        #endregion
    }
}