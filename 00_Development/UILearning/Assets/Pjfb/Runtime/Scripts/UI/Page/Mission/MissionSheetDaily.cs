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
    public class MissionSheetDaily : Sheet
    {
        #region InnerClass
        public class Parameters
        {
            public Dictionary<DailyMissionCategoryMasterObject, List<MissionProgressPairData>> dailyMissionDataDictionary;
            public Action<MissionModalListItem.ItemParams, DateTime, float> onListItemClickReceiveButton;
            public Action<MissionModalListItem.ItemParams> onListItemClickChallengeButton;
            public Action<List<DailyMissionCategoryMasterObject>, List<List<MissionProgressPairData>>, DateTime, float> onClickReceiveAllButton;
        }

        private class NextStepDateTimeCacheData
        {
            public DateTime dateTime;
            
            private int _lastCheckSecond = -1;
            private string _nextStepDateTimeString = string.Empty;
            
            public string GetNextStepDateTimeString(DateTime now)
            {
                if (_lastCheckSecond != now.Second) {
                    _nextStepDateTimeString = dateTime.GetRemainingString(now);
                    _nextStepDateTimeString = _nextStepDateTimeString.Any() ? string.Format(TimeTextFormat, _nextStepDateTimeString) : string.Empty;    
                } 
                
                return _nextStepDateTimeString;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TMPro.TMP_Text expireTimeText;
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private UIButton allReceiveButton;
        #endregion

        #region PrivateFields
        private Parameters _parameters;
        private NextStepDateTimeCacheData _nextStepDateTimeCacheData = null;
        
        private const string TimeTextFormat = "更新まであと {0}";
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

        private void Update()
        {
            expireTimeText.text = _nextStepDateTimeCacheData != null ? _nextStepDateTimeCacheData.GetNextStepDateTimeString(AppTime.Now) : string.Empty;
        }
        #endregion

        #region PublicMethods
        public void Init()
        {
            expireTimeText.text = string.Empty;
        }

        public void SetDisplay(Parameters parameters)
        {
            _parameters = parameters;
            if (_parameters.dailyMissionDataDictionary.Keys.Any())
            {
                var aDailyMissionData = _parameters.dailyMissionDataDictionary.Keys.ElementAt(0);
                var startAt = aDailyMissionData.startAt.TryConvertToDateTime();
                _nextStepDateTimeCacheData = new NextStepDateTimeCacheData {
                    dateTime = startAt.GetNextStepDate(aDailyMissionData.stepDay, AppTime.Now)
                };
            }
            expireTimeText.text = _parameters.dailyMissionDataDictionary.Keys.Any() ? _parameters.dailyMissionDataDictionary.Keys.ElementAt(0).endDescription : string.Empty;
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
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters.onClickReceiveAllButton.Invoke(_parameters.dailyMissionDataDictionary.Keys.ToList(), _parameters.dailyMissionDataDictionary.Values.ToList(), _nextStepDateTimeCacheData.dateTime, lastScrollValue);
        }

        private void OnClickReceiveButton(MissionModalListItem.ItemParams itemParams)
        {
            var lastScrollValue = poolListContainer.scrollValue;
            _parameters.onListItemClickReceiveButton(itemParams, _nextStepDateTimeCacheData.dateTime, lastScrollValue);
        }
        #endregion
    }
}