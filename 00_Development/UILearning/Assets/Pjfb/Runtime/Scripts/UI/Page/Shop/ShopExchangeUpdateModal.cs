using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Common;

namespace Pjfb.Shop
{
    public class ShopExchangeUpdateModal : ModalWindow
    {

        private class ModalParams
        {
            public CommonStoreCategoryMasterObject commonStoreCategory;
            public long updateCount;
            public Func<ShopLotStoreAPIResponse, UniTask> OnUpdate;
        }

        [SerializeField] private TMP_Text contentText;
        [SerializeField] private PossessionItemUi requiredItemUi;
        [SerializeField] private PossessionItemUi possessionItemUi;
        [SerializeField] private GameObject updateButton;
        
        private ModalParams modalParams;

        public static void Open(CommonStoreCategoryMasterObject commonStoreCategory, long updateCount,Func<ShopLotStoreAPIResponse, UniTask> onUpdate)
        {
            var param = new ModalParams
            {
                commonStoreCategory = commonStoreCategory,
                updateCount = updateCount,
                OnUpdate = onUpdate
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopExchangeUpdate,param);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalParams = (ModalParams)args;

            var updateCost = MasterManager.Instance.commonStoreLotteryCostMaster.GetCommonStoreLotteryCost(
                modalParams.commonStoreCategory.mCommonStoreLotteryCostCategoryId, modalParams.updateCount);
            var cost = updateCost.value;
            var pointId = updateCost.mPointId;
            
            bool canUpdate;
            if (cost == 0)
            {
                contentText.text = StringValueAssetLoader.Instance["shop.exchange.update_free"];
                possessionItemUi.gameObject.SetActive(false);
                requiredItemUi.gameObject.SetActive(false);
                canUpdate = true;
            }
            else
            {
                var point = UserDataManager.Instance.point.Find(updateCost.mPointId);
                
                var pointCount = point?.value ?? 0;
                var mPoint = MasterManager.Instance.pointMaster.FindData(pointId);
                var pointName = mPoint?.name ?? "";
                
                canUpdate = pointCount >= cost;

                if (canUpdate)
                {
                    contentText.color = ColorValueAssetLoader.Instance["red"];
                    contentText.text = string.Format(StringValueAssetLoader.Instance["shop.exchange.update"], pointName, cost.ToString());
                    requiredItemUi.gameObject.SetActive(false);
                }
                else
                {
                    contentText.color = ColorValueAssetLoader.Instance["default"];
                    contentText.text = string.Format(StringValueAssetLoader.Instance["gacha.not_enough_point"], mPoint.name);
                    requiredItemUi.SetCount(pointId, cost);
                    requiredItemUi.gameObject.SetActive(true);
                }
                
                possessionItemUi.gameObject.SetActive(true);
            }
            
            
            if (canUpdate)
            {
                possessionItemUi.SetAfterCountByAmount(pointId, -updateCost.value);
            }
            else
            {
                possessionItemUi.SetPossessionUi(pointId);
            }

            updateButton.SetActive(canUpdate);
            return base.OnPreOpen(args, token);
        }

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        
        public void OnClickUpdate()
        {
            UpdateAndCloseAsync().Forget();
        }
        
        private async UniTask UpdateAndCloseAsync()
        {
            try
            {
                var response = await GetShopLotStoreAPI();
                if (modalParams?.OnUpdate != null)
                    await modalParams.OnUpdate(response);
                Close();
            }
            catch (APIException)
            {
                
            }
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
        #endregion

        #region API
        private async UniTask<ShopLotStoreAPIResponse> GetShopLotStoreAPI()
        {
            ShopLotStoreAPIRequest request = new ShopLotStoreAPIRequest();
            //Token取得
            var token = await Pjfb.Networking.App.APIUtility.ConnectOneTimeTokeRequest();
            request.oneTimeToken = token.oneTimeToken;
            var post = new ShopLotStoreAPIPost();
            post.mCommonStoreCategoryId = modalParams.commonStoreCategory.id;
            request.SetPostData(post);
            try
            {
                await APIManager.Instance.Connect(request);
                return request.GetResponseData();
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                CruFramework.Logger.LogError("ShopLotStoreAPI error");
                throw;
            }
        }
        

        #endregion
    }
   
}