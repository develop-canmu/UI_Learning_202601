using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public class RivalryTopPage : Page
    {
        #region PageParams
        public class PageParams
        {
            
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] protected PoolListContainer poolListContainer;
        [SerializeField] protected StaminaView staminaView;
        #endregion

        #region PrivateProperties
        private PageParams _pageParams;
        protected List<RivalryMatchPoolListItem.ItemParams> _listItemParams = new();
        #endregion
        
        #region OverrideMethods
        protected override async void OnOpened(object args)
        {
            await ShowPoolList();
            base.OnOpened(args);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParams) args;
            poolListContainer.Clear();
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.RivalryBattle);
            await RivalryManager.Instance.GetHuntGetDataAPI();
            await base.OnPreOpen(args, token);
        }
        
        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            await poolListContainer.SlideOut();
            return await base.OnPreClose(token);
        }
        #endregion

        #region PrivateMethods
        protected virtual async UniTask ShowPoolList()
        {
            var cachedData = RivalryManager.rivalryCacheData;
            var showingNewIconEventDataContainer = RivalryManager.showingNewIconEventDataContainer;
            
            var now = AppTime.Now;
            var listItemParams = new List<RivalryMatchPoolListItem.ItemParams>();
            foreach (var huntMasterObjectData in cachedData.huntMasterObjectDataList)
            {
                if (!cachedData.huntTimeTableDictionary.TryGetValue(huntMasterObjectData.id, out var timeTables)) continue;
                var openTimeTableData = timeTables.Find(timeTable =>
                    timeTable.type == 1 && 
                    timeTable.startAt.TryConvertToDateTime().IsPast(now) && 
                    timeTable.viewEndAt.TryConvertToDateTime().IsFuture(now));
                if (openTimeTableData != null) {
                    listItemParams.Add(new RivalryMatchPoolListItem.ItemParams (
                        huntMasterObject: huntMasterObjectData,
                        huntTimetableMasterObject: openTimeTableData,
                        huntEnemyObjectList: cachedData.huntEnemyObjectList.TryGetValue(huntMasterObjectData.id, out var huntEnemyObject) ? huntEnemyObject : null,
                        challengedEventMatchDataList: cachedData.challengedDictionary.TryGetValue(openTimeTableData.id, out var challengedDataList) ? challengedDataList : null,
                        showingNewIconEventDataContainer: showingNewIconEventDataContainer,
                        onClickItemParams: OnClickRivalryMatchPoolListItem));
                }
            }
            _listItemParams = listItemParams.OrderByDescending(aData => aData.huntTimetableMasterObject.priority).ToList();
            await poolListContainer.SetDataList(_listItemParams);
        }
        #endregion
        
        #region EventListeners
        protected void OnClickRivalryMatchPoolListItem(RivalryMatchPoolListItem.ItemParams itemParams)
        {
            var m = (RivalryPage)Manager;
            if (itemParams.matchType == RivalryMatchType.Event)
            {
                if (itemParams.endDateTime.IsPast(AppTime.Now))
                {
                    ConfirmModalWindow.Open(new ConfirmModalData(
                        StringValueAssetLoader.Instance["rivalry.event_end.title"], 
                        StringValueAssetLoader.Instance["rivalry.event_end.body"], 
                        string.Empty, 
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => window.Close())));
                    return;
                }
                
                m.OpenPage(RivalryPageType.RivalryEvent, true, new RivalryEventPage.PageParams{huntMasterObject = itemParams.huntMasterObject, huntTimetableMasterObject = itemParams.huntTimetableMasterObject, HuntEnemyMasterObjectList = itemParams.huntEnemyObjectList});
            }
            else
            {
                m.OpenPage(RivalryPageType.RivalryRegular, true, new RivalryRegularPage.PageParams{huntMasterObject = itemParams.huntMasterObject, huntTimetableMasterObject = itemParams.huntTimetableMasterObject});
            }
        }

        public void OnClickBack()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
        }
        
        
        #endregion
    }
}
