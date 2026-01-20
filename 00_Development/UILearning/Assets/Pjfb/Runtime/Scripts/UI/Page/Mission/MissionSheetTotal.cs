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
    public class MissionSheetTotal : Sheet
    {
        #region Parameters
        public class Parameters
        {
            public Dictionary<DailyMissionCategoryMasterObject, List<MissionProgressPairData>> dailyMissionDataDictionary;
            public Action<MissionModalListItem.ItemParams, DateTime, float> onListItemClickReceiveButton;
            public Action<MissionModalListItem.ItemParams> onListItemClickChallengeButton;
            public Action<List<DailyMissionCategoryMasterObject>, List<List<MissionProgressPairData>>, DateTime, float> onClickReceiveAllButton;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private UIButton allReceiveButton;
        #endregion

        #region PrivateFields
        private Parameters _parameters;
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
            poolListContainer.Clear();
        }

        public void SetDisplay(Parameters parameters)
        {
            _parameters = parameters;
        }
        #endregion
        
        #region PrivateMethods
        private async UniTask UpdateDisplay(float initialScrollValue)
        {
            var missionDataDictionary = _parameters.dailyMissionDataDictionary
                .SelectMany(aData => aData.Value)
                .ToDictionary(aData => aData.missionData.id); 
            var itemParamList = missionDataDictionary
                .Where(aData => aData.Value.ShowMission(missionDataDictionary))
                .Select(aData => new MissionModalListItem.ItemParams {
                    missionProgressPairData = aData.Value,
                    onClickReceiveButton = OnClickReceiveButton,
                    onClickChallengeButton = _parameters.onListItemClickChallengeButton,
                }).OrderBy(aData => aData.missionProgressPairData.sortOrder).ToList();
            allReceiveButton.interactable = itemParamList.Select(aData => aData.missionProgressPairData).Any(aProgressPairData => aProgressPairData.hasReceivingReward);
            await poolListContainer.SetDataList(itemParamList, scrollValue: initialScrollValue);
        }
        #endregion
        
        #region EventListener
        public void OnClickReceiveAllButton()
        {
            var missionCategories = _parameters.dailyMissionDataDictionary.Keys.ToList();
            var dataParams = _parameters.dailyMissionDataDictionary.Values.ToList();
            var receiveEndAtString = missionCategories.Count > 0 ? missionCategories[0].receiveEndAt : string.Empty;
            var receiveEndAt = receiveEndAtString.TryConvertToDateTime(minValueDefault: false);
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters.onClickReceiveAllButton.Invoke(missionCategories, dataParams, receiveEndAt, lastScrollValue);
        }

        private void OnClickReceiveButton(MissionModalListItem.ItemParams itemParams)
        {
            var missionCategory = itemParams.missionProgressPairData.missionCategory;
            var receiveEndAt = missionCategory.receiveEndAt.TryConvertToDateTime(minValueDefault: false);
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters.onListItemClickReceiveButton(itemParams, receiveEndAt, lastScrollValue);
        }
        #endregion
    }
}