using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Master;

namespace Pjfb.Gacha
{
    public class GachaRetryConfirmModal : ModalWindow
    {
        public class Param {
            public GachaPrizeIconWrap icon  = null;
            public GachaPendingFrameData pendingData = null;
            public System.Action<GachaRetryConfirmModal, GachaPrizeIconWrap, PrizeJsonViewData[], GachaPendingFrame> OnExecutedPendingRetry = null;
            public long gachaResultPendingId = 0;
            public long gachaCategoryId = 0;
        }

        [SerializeField]
        private PossessionItemUi _mainPointUi = null;
        [SerializeField]
        private PossessionItemUi _alternativePointUi = null;
        [SerializeField]
        private TextMeshProUGUI periodText = null;
        [SerializeField]
        private PrizeJsonView _prizeIcon = null;
        [SerializeField]
        private IconImage _useImage = null;
        [SerializeField]
        private TextMeshProUGUI _useCountText = null;
        [SerializeField]
        private UIBadgeBalloon _retryCountBalloon = null;
        
        Param _param = null;
        
        private long _usingPointAlternativeId = 0;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            /* パターン
             * 1.引き直しが無料の場合
             * 2.仮想ポイントを所持していて充足する場合
             * 3.仮想ポイントを所持していて充足しない・所持ポイントは充足する場合
             * 4.仮想ポイントを所持していない場合
             */
            
            _param = (Param)args;
            var pointMaster = MasterManager.Instance.pointMaster.FindData(_param.pendingData.RetryPointId);
            
            // ポイント画像セット
            _useImage.SetTexture(pointMaster.id);
            
            var uPointValue = GachaUtility.FetchPointValue(pointMaster.id);
            var uPointAlternative = GachaUtility.GetPointAlternative(pointMaster.id, _param.gachaCategoryId);
            
            // 仮想ポイントの残数
            var uPointAlternativeValue = uPointAlternative?.UserData.value ?? 0;
            var hasAlternativePoint = uPointAlternative != null && uPointAlternativeValue > 0;
            
            if (_param.pendingData.RetryPrice <= 0)
            {
                // 引き直しが無料の場合
                _alternativePointUi.gameObject.SetActive(false);
                _mainPointUi.gameObject.SetActive(true);
                _mainPointUi.SetAfterCount(pointMaster.id, uPointValue, uPointValue);
            }
            else if (hasAlternativePoint && uPointAlternativeValue >= _param.pendingData.RetryPrice)
            {
                // prizeが0より大きい・仮想ポイントのみを使用する
                // 使用する仮想ポイントIDをキャッシュ
                _usingPointAlternativeId = uPointAlternative.UserData.pointId;
                _mainPointUi.gameObject.SetActive(false);
                
                _alternativePointUi.gameObject.SetActive(true);
                _alternativePointUi.SetAfterCount(uPointAlternative.UserData.pointId, uPointAlternativeValue, uPointAlternativeValue - _param.pendingData.RetryPrice);
                SetPeriodText(uPointAlternative.AlternativeMasterObject);
            }
            else if (hasAlternativePoint && uPointAlternativeValue > 0)
            {
                // 仮想ポイントと通常ポイントを併用
                var priceRemain = _param.pendingData.RetryPrice - uPointAlternativeValue;
                // 使用する仮想ポイントIDをキャッシュ
                _usingPointAlternativeId = uPointAlternative.UserData.pointId;
                
                _alternativePointUi.gameObject.SetActive(true);
                _alternativePointUi.SetAfterCount(uPointAlternative.UserData.pointId, uPointAlternativeValue, 0);
                SetPeriodText(uPointAlternative.AlternativeMasterObject);
                
                _mainPointUi.gameObject.SetActive(true);
                _mainPointUi.SetAfterCount(pointMaster.id, uPointValue, uPointValue - priceRemain);
            }
            else
            {
                // 仮想ポイントを未所持
                _alternativePointUi.gameObject.SetActive(false);
                
                _mainPointUi.gameObject.SetActive(true);
                _mainPointUi.SetAfterCount(pointMaster.id, uPointValue, uPointValue - _param.pendingData.RetryPrice);
            }
            
            _prizeIcon.SetView( _param.icon.prizeIcon.MainViewData );
            
            _useCountText.text = _param.pendingData.RetryPrice.ToString();

            var ballonText = string.Format(StringValueAssetLoader.Instance["gacha.retry_count"], _param.pendingData.RetryLimit- _param.pendingData.RetryCount, _param.pendingData.RetryLimit );
            _retryCountBalloon.SetText(ballonText);
        }
        
        /// <summary> 使用期限を反映する </summary>
        private void SetPeriodText(PointAlternativeMasterObject pointAlternativeMaster)
        {
            periodText.text = pointAlternativeMaster.endAt.TryConvertToDateTime().ToString(StringValueAssetLoader.Instance["point.expire_date_format"]);
        }
        
        /// <summary>
        /// 引き直し実行
        /// </summary>
        public void OnClickPositiveButton()
        {
            ExecutePendingRetry().Forget();
        }

        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }

        public async UniTask ExecutePendingRetry()
        {
            var request = new GachaPendingRetryAPIRequest();
            var post = new GachaPendingRetryAPIPost();
            post.uGachaResultPendingId = _param.gachaResultPendingId;
            post.index = _param.pendingData.Index;
            if (_usingPointAlternativeId > 0)
            {
                post.mPointIdAlternativeList = new long[] { _usingPointAlternativeId };
            }
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            var response = request.GetResponseData();
            
            var dataList = new PrizeJsonViewData[response.contentList.Length];
            for(int i=0; i<response.contentList.Length; ++i){
                dataList[i] = new PrizeJsonViewData(response.contentList[i]);
            }
            _param.OnExecutedPendingRetry?.Invoke( this, _param.icon, dataList, response.frame);
            

        }
        
    }
}
