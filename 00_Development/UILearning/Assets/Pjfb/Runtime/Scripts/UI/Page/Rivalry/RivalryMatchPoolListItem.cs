using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Pjfb.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using System.Linq;

namespace Pjfb.Rivalry
{
    public enum RivalryMatchType
    {
        Regular,
        Event
    }

    public class RivalryMatchPoolListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public List<HuntEnemyMasterObject> huntEnemyObjectList;
            public Action<ItemParams> onClickItemParams;
            
            public RivalryMatchType matchType { get; }
            public DateTime endDateTime { get; }
            public bool showBadge { get; }
            public bool IsFarFuture { get; }
            public ItemParams(HuntMasterObject huntMasterObject, 
                            HuntTimetableMasterObject huntTimetableMasterObject, 
                            List<HuntEnemyMasterObject> huntEnemyObjectList, 
                            List<RivalryManager.ChallengedEventMatchData> challengedEventMatchDataList, 
                            RivalryManager.ShowingNewIconEventDataContainer showingNewIconEventDataContainer, 
                            Action<ItemParams> onClickItemParams)
            {
                this.huntMasterObject = huntMasterObject;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.huntEnemyObjectList = huntEnemyObjectList;
                this.onClickItemParams = onClickItemParams;

                matchType = RivalryManager.GetMatchType(huntTimetableMasterObject, huntMasterObject);
                if (huntMasterObject.isClosedOnceWin) {
                    showBadge = showingNewIconEventDataContainer.ShowBadge(mHuntId: huntMasterObject.id, mHuntTimetableId: huntTimetableMasterObject.id);
                } else {
                    var challengedCount = challengedEventMatchDataList?.Count ?? 0; 
                    var showingCount = huntEnemyObjectList?.Count ?? 0; 
                    showBadge = matchType == RivalryMatchType.Event && challengedCount < showingCount;
                }
                endDateTime = huntTimetableMasterObject.endAt.TryConvertToDateTime();
                IsFarFuture = endDateTime.GetTimeSpanRemaining(AppTime.Now).TotalDays >= 365;
            }

            public override int GetItemHeight(PoolListItemBase poolListItemBase, int prefabHeight)
            {
                return prefabHeight - (matchType == RivalryMatchType.Regular || IsFarFuture ? 40 : 0);
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text titleText, scheduleText, coverText;
        [SerializeField] private GameObject badgeGameObject, maskGameObject;
        [SerializeField] private Animator towerCompletedAnimator;
        [SerializeField] private RivalryTopBannerImage bannerImage;
        [SerializeField] private UIBadgeBalloonCrossFade balloonCrossFade;
        #endregion

        private ItemParams _itemParams;

        public override void Init(ItemParamsBase value)
        {
            _itemParams = (ItemParams) value;
            var now = AppTime.Now;
            var hasEnded = _itemParams.endDateTime.IsPast(now);
            badgeGameObject.SetActive(_itemParams.showBadge && !hasEnded);
            maskGameObject.SetActive(false);
            titleText.text = _itemParams.huntMasterObject.name;
            bannerImage.SetTexture(_itemParams.huntMasterObject.id);
            
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, _itemParams.itemHeight);

            // 吹き出しバッジ表示
            // 1. 報酬ブースト
            // 2. 回数制限
            var boostCondition = RivalryManager.GetRewardBoost(_itemParams.huntTimetableMasterObject.id) != null;
            var boostString = string.Format(StringValueAssetLoader.Instance["rivalry.rewardboost.activated"]);
            var usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == _itemParams.huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            var remainingLimit = _itemParams.huntTimetableMasterObject.dailyPlayCount - usedLimit;
            var limitCondition = remainingLimit > 0;
            var limitString = string.Format(StringValueAssetLoader.Instance[_itemParams.huntTimetableMasterObject.playCountType == (long)HuntPlayCountType.Win ? "rivalry.match_limit.win" : "rivalry.match_limit.challenge"], remainingLimit);            
            var noLimitLeft = _itemParams.huntTimetableMasterObject.dailyPlayCount > 0 && remainingLimit <= 0;
            maskGameObject.SetActive(noLimitLeft || hasEnded);
            coverText.text = string.Format(StringValueAssetLoader.Instance[noLimitLeft ? "rivalry.match_limit.over" : "rivalry.top.event_end"]);
            balloonCrossFade.gameObject.SetActive(!hasEnded);
            balloonCrossFade.SetView(boostCondition, limitCondition, boostString, limitString);

            if (_itemParams.matchType == RivalryMatchType.Regular) 
            {
                scheduleText.gameObject.SetActive(false);
            } else 
            {
                scheduleText.gameObject.SetActive(true);
                scheduleText.gameObject.SetActive(!_itemParams.IsFarFuture);
                scheduleText.text = hasEnded ? StringValueAssetLoader.Instance["rivalry.top.event_end_period"] : GetDateTimeString(_itemParams.huntTimetableMasterObject);
            }

            bool isTowerEvent = _itemParams.huntMasterObject.isClosedOnceWin;
            bool isComplete = RivalryManager.towerCompleteEventMatchDataContainer.towerCompleteDataList.Exists(aData => aData.Equals(_itemParams.huntTimetableMasterObject.id));
            if (isTowerEvent)
            {
                var d = RivalryManager.towerCompleteEventMatchDataContainer.towerCompleteDataList;
            }
            towerCompletedAnimator.gameObject.SetActive(isTowerEvent && isComplete);
            if (isTowerEvent && isComplete)
            {
                towerCompletedAnimator.SetTrigger("Open");
            }
        }

        private string GetDateTimeString(HuntTimetableMasterObject huntTimetableMasterObject)
        {
            return string.Format(StringValueAssetLoader.Instance["rivalry.top.event_period"],
                huntTimetableMasterObject.startAt.TryConvertToDateTime().GetNewsDateTimeString(),
                huntTimetableMasterObject.endAt.TryConvertToDateTime().GetNewsDateTimeString());
        }

        #region EventListener
        public void OnClickListItem()
        {
            if (maskGameObject.activeSelf) return;
            _itemParams?.onClickItemParams?.Invoke(_itemParams);
        }
        #endregion
    }
}