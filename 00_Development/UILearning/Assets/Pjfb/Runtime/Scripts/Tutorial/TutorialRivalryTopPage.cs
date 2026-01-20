using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Rivalry;
using Pjfb.Master;

namespace Pjfb
{
    public class TutorialRivalryTopPage : RivalryTopPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // チュートリアルではAPIを叩きたくないので上書きする
            poolListContainer.Clear();
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.RivalryBattle);
        }

        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.EndFade:
                        AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }
        
        public void OnClickMatchBanner(RivalryMatchBanner data)
        {
            RivalryPage m = (RivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryRegular, true, data);
        }
        protected override async UniTask ShowPoolList()
        {
            var huntMasterList = MasterManager.Instance.huntMaster.values.ToList();
            var huntTimeTableList = MasterManager.Instance.huntTimetableMaster.values.ToDictionary(aData => aData.id);
            var challengedDictionary = RivalryManager.challengedEventMatchDataContainer.challengedDataList.GroupBy(aData => aData.mHuntTimetableId).ToDictionary(aData => aData.Key, aData => aData.ToList());
            var huntEnemyObjectList = MasterManager.Instance.huntEnemyMaster.values.GroupBy(aData => aData.mHuntId).ToDictionary(aData => aData.Key, aData => aData.ToList());
            var showingNewIconEventDataContainer = RivalryManager.showingNewIconEventDataContainer;
            
            _listItemParams = huntMasterList
                .Where(aData => 
                    huntTimeTableList.TryGetValue(aData.id, out var timeTable) &&
                    timeTable.type == 99 )
                .Select(aData => new RivalryMatchPoolListItem.ItemParams (
                    huntMasterObject: aData,
                    huntTimetableMasterObject: huntTimeTableList[aData.id],
                    huntEnemyObjectList: huntEnemyObjectList.TryGetValue(aData.id, out var huntEnemyObject) ? huntEnemyObject : null,
                    challengedEventMatchDataList: challengedDictionary.TryGetValue(huntTimeTableList[aData.id].id, out var challengedDataList) ? challengedDataList : null,
                    showingNewIconEventDataContainer: showingNewIconEventDataContainer,
                    onClickItemParams: OnClickRivalryMatchPoolListItem
                )).ToList();
            
            await poolListContainer.SetDataList(_listItemParams);
        }
    }
}