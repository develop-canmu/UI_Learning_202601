using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;
using Pjfb.UI;

namespace Pjfb.Common
{
    public class MissionSheetEvent : Sheet
    {
        #region Parameters
        public class Parameters
        {
            public Dictionary<DailyMissionCategoryMasterObject, List<MissionProgressPairData>> eventMissionDataDictionary;
            public Action<MissionModalEventListItem.ItemParams> onListItemClickEventButton;
            public Action<List<DailyMissionCategoryMasterObject>, List<List<MissionProgressPairData>>, DateTime, float> onClickReceiveAllButton;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private UIButton allReceiveButton;
        [SerializeField] private GameObject listEmptyNotification;
        #endregion

        #region PublicFields
        public Parameters parameters { get; private set; }
        public float scrollValue => poolListContainer.scrollValue;
        #endregion
        
        #region PrivateFields
        private const long ClosedOrderNumber = 10000000; 
        #endregion
        
        #region OverrideMethods
        protected override async UniTask OnOpen(object args)
        {
            var initialScrollValue = 1f;
            if (args is MissionModal.MissionSheetOpenParams @params) initialScrollValue = @params.initialScroll;
            await UpdateDisplay(initialScrollValue);
            await base.OnOpen(args);
        }

        protected override async UniTask OnPreClose()
        {
            await poolListContainer.SlideOut();
            await base.OnPreClose();
        }
        #endregion

        #region PublicMethods
        public void Init()
        {
            
        }

        public void SetDisplay(Parameters parameters)
        {
            this.parameters = parameters;
        }
        
        public async UniTask UpdateDisplay(float initialScrollValue = 1f)
        {
            allReceiveButton.interactable = parameters.eventMissionDataDictionary
                .SelectMany(aData => aData.Value)
                .Any(aMissionProgressPairData => aMissionProgressPairData.hasReceivingReward);

            var now = AppTime.Now;
            var showingItemParamsList = parameters.eventMissionDataDictionary
                .Select(aData => new MissionModalEventListItem.ItemParams(
                    missionEventData: aData.Key,
                    missionProgressPairDataList: aData.Value,
                    onClickEventButton: parameters.onListItemClickEventButton
                )).OrderBy(aData => 
                    (aData.endAt.IsPast(now) ? ClosedOrderNumber : 0) // 受け取り期間は下に並ぶように
                    + aData.missionEventData.sortNumber).ToList();
            
            if (showingItemParamsList.Count == 0) {
                listEmptyNotification.SetActive(true);
                poolListContainer.Clear();
                poolListContainer.gameObject.SetActive(false);
            } else {
                listEmptyNotification.SetActive(false);
                poolListContainer.gameObject.SetActive(true);
                await poolListContainer.SetDataList(showingItemParamsList, scrollValue: initialScrollValue);    
            }
        }
        #endregion

        #region EventListener
        public void OnClickReceiveAllButton()
        {
            var now = AppTime.Now;
            var missionCategories = parameters.eventMissionDataDictionary
                .Where(aPairData => aPairData.Value.Any(aMissionProgressPairData => aMissionProgressPairData.hasReceivingReward) &&
                                    aPairData.Key.receiveEndAt.TryConvertToDateTime().IsFuture(now))
                .Select(aPairData => aPairData.Key).ToList();
            var pairDatas = parameters.eventMissionDataDictionary
                .Where(aPairData => aPairData.Value.Any(aMissionProgressPairData => aMissionProgressPairData.hasReceivingReward) &&
                                    aPairData.Key.receiveEndAt.TryConvertToDateTime().IsFuture(now))
                .Select(aPairData => aPairData.Value).ToList();
            var receiveEndAt = DateTime.MaxValue; // この画面の「全て受け取る」は期間に縛らない
            var lastScrollValue = poolListContainer.scrollValue;
            parameters.onClickReceiveAllButton.Invoke(missionCategories, pairDatas, receiveEndAt, lastScrollValue);
        }
        #endregion
    }
}