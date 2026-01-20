using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public class RivalryTeamSelectPage : Page
    {
        #region PageParams
        public class PageParams
        {
            public HuntDifficultyMasterObject huntDifficultyMasterObject;
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;

            public PageParams(HuntDifficultyMasterObject huntDifficultyMasterObject, HuntMasterObject huntMasterObject, HuntTimetableMasterObject huntTimetableMasterObject)
            {
                this.huntDifficultyMasterObject = huntDifficultyMasterObject;
                this.huntMasterObject = huntMasterObject;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        [SerializeField] private StaminaView staminaView;
        #endregion

        #region PrivateProperties
        private PageParams _pageParams;
        private List<RivalryTeamSelectPoolListItem.ItemParams> _listItemParams = new();
        #endregion
        
		protected HuntGetTimetableDetailAPIResponse _response;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParams) args;
            staminaView.InitWithoutUpdateAsync(StaminaUtility.StaminaType.RivalryBattle);
            await GetHuntGetTimetableDetailAPI(_pageParams.huntTimetableMasterObject.id);
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            // シークレットセール表示
            ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.RivalryTeamSelect);
            base.OnOpened(args);
        }

        protected override async void OnEnablePage(object args)
        {
            await ShowPoolList();
            base.OnEnablePage(args);
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
            var progress = _response.enemyHistory.progress;
            _listItemParams = MasterManager.Instance.huntEnemyMaster.values
                .Where(aData =>
                    _response.mHuntEnemyIdList.Contains(aData.id) &&
                    aData.difficulty == _pageParams.huntDifficultyMasterObject.difficulty)
                .Select(aData => new RivalryTeamSelectPoolListItem.ItemParams(
                    huntEnemyMasterObject: aData,
                    huntDifficultyMasterObject: _pageParams.huntDifficultyMasterObject,
                    huntTimetableMasterObject: _pageParams.huntTimetableMasterObject,
                    huntMasterObject: _pageParams.huntMasterObject,
                    onClickItemParams: OnClickPoolListItem))
                .OrderBy(aData => aData.huntEnemyMasterObject.id)
                .ToList();
            
            await poolListContainer.SetDataList(_listItemParams);
        }
        #endregion

        
        #region API
        protected virtual async UniTask GetHuntGetTimetableDetailAPI(long mHuntTimetableId)
        {
            HuntGetTimetableDetailAPIRequest request = new HuntGetTimetableDetailAPIRequest();
            HuntGetTimetableDetailAPIPost post = new HuntGetTimetableDetailAPIPost();
            post.mHuntTimetableId = mHuntTimetableId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            _response = request.GetResponseData();
        }

        #endregion
        
        #region EventListeners
        protected virtual void OnClickPoolListItem(RivalryTeamSelectPoolListItem.ItemParams itemParams)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.TeamConfirm, 
                true, 
                new TeamConfirmPage.PageParams(
                    PageType.Rivalry, 
                    new RivalryPage.Data(RivalryPageType.RivalryTeamSelect, _pageParams), 
                    itemParams.huntMasterObject,
                    itemParams.huntTimetableMasterObject,
                    itemParams.huntEnemyMasterObject)
            );
        }

        public void OnClickBack()
        {
            RivalryPage m = (RivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryRegular, false, new RivalryRegularPage.PageParams{huntMasterObject = _pageParams.huntMasterObject, huntTimetableMasterObject = _pageParams.huntTimetableMasterObject});
        }
        
        #endregion
    }
}
