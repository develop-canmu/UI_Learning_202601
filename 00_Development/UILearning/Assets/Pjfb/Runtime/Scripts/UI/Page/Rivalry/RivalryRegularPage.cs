using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public class RivalryRegularPage : Page
    {
        #region PageParams
        public class PageParams
        {
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private StaminaView staminaView;
        #endregion

        #region PrivateProperties
        private PageParams _pageParams;
        private List<RivalryRegularPoolListItem.ItemParams> _listItemParams = new();
        private const int RegularMatchHuntId = 1;
        #endregion

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParams) args;
            poolListContainer.Clear();
            staminaView.InitWithoutUpdateAsync(StaminaUtility.StaminaType.RivalryBattle);
            await base.OnPreOpen(args, token);
        }

        protected override async void OnOpened(object args)
        {
            await ShowPoolList();
            base.OnOpened(args);
        }
        
        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            await poolListContainer.SlideOut();
            return await base.OnPreClose(token);
        }
        #endregion
        
        #region PrivateMethods
        private async UniTask ShowPoolList()
        {
            var huntDifficultyDictionary = MasterManager.Instance.huntDifficultyMaster.values.ToDictionary(aData => aData.id);
            var regularMatchHuntEnemies = MasterManager.Instance.huntEnemyMaster.FindByHuntId(mHuntId: RegularMatchHuntId);
            _listItemParams = regularMatchHuntEnemies
                .GroupBy(aData => aData.difficulty)
                .Where(aData => huntDifficultyDictionary.ContainsKey(aData.Key))
                .Select(aData => new RivalryRegularPoolListItem.ItemParams(
                    huntDifficultyMasterObject: huntDifficultyDictionary[aData.Key],
                    huntTimetableMasterObject: _pageParams.huntTimetableMasterObject,
                    huntMasterObject:_pageParams.huntMasterObject,
                    onClickItemParams: OnClickPoolListItem))
                .ToList();
            
            await poolListContainer.SetDataList(_listItemParams);
        }
        #endregion
        
        #region EventListeners
        private void OnClickPoolListItem(RivalryRegularPoolListItem.ItemParams itemParams)
        {
            RivalryPage m = (RivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryTeamSelect, true, new RivalryTeamSelectPage.PageParams(
                huntDifficultyMasterObject: itemParams.huntDifficultyMasterObject,
                huntMasterObject: itemParams.huntMasterObject,
                huntTimetableMasterObject: itemParams.huntTimetableMasterObject));
        }

        public void OnClickBack()
        {
            RivalryPage m = (RivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryTop, false, null);
        }
        
        #endregion
    }
}
