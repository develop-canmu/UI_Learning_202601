using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using TMPro;
using UniRx;

namespace Pjfb.Colosseum
{
    public class ColosseumHistoryModal : ModalWindow
    {
        [SerializeField] private ScrollGrid scrollGrid;
        [SerializeField] private GameObject notificationText;

        private ColosseumEventMasterObject currentColosseumEvent;
        private List<ColosseumHistoryItem.Data> cachedSeasonStatusList;

        public static void Open(ColosseumEventMasterObject colosseumEventMasterObject)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ColosseumHistory,colosseumEventMasterObject);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            scrollGrid.OnBeginPageSnap += ()=>
            {
                if (scrollGrid.verticalScrollbar.value > 0) return;
                UpdateView(false);
            };
            cachedSeasonStatusList = new List<ColosseumHistoryItem.Data>();
            currentColosseumEvent = (ColosseumEventMasterObject)args;
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            UpdateView(true); 
            base.OnOpened();
        }

        private async void UpdateView(bool isFirst)
        {

            var lastUpdate = cachedSeasonStatusList.LastOrDefault();
            var lastSeasonId = lastUpdate?.seasonStatus.sColosseumEventId ?? -1;
            
            var seasonStatusList = await ColosseumManager.RequestGetUserSeasonStatusPastList(currentColosseumEvent.id,lastSeasonId);

            if (cachedSeasonStatusList.Count == 0 && (seasonStatusList == null || seasonStatusList.Length == 0))
            {
                notificationText.gameObject.SetActive(true);
                return;
            }

            var dataList = new List<ColosseumHistoryItem.Data>();
            foreach (var seasonStatus in seasonStatusList)
            {
                var data = new ColosseumHistoryItem.Data();
                data.seasonStatus = seasonStatus;
                data.mColosseumGradeGroupId = currentColosseumEvent.mColosseumGradeGroupId;
                dataList.Add(data);
            }

            var beforeCount = cachedSeasonStatusList.Count;
            cachedSeasonStatusList.AddRange(dataList);
            scrollGrid.SetItems(cachedSeasonStatusList);

            if (!isFirst) scrollGrid.verticalNormalizedPosition = 1.0f - (beforeCount / (float)cachedSeasonStatusList.Count);

        }

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }

        #endregion
    }
}