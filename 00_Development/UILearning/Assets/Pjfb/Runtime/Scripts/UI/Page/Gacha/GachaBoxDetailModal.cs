using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxDetailModal : ModalWindow
    {
        public class Param{
            public long gachaCategoryId = 0; // ガチャカテゴリID
            public Action<long, bool> onReset = null;
        }
        

        [SerializeField]
        ScrollGrid _scrollGrid = null;

        [SerializeField]
        TextMeshProUGUI _titleText = null;

        [SerializeField] 
        TextMeshProUGUI _remainingNumberText = null;
        
        [SerializeField]
        UIButton _resetButton = null;

        Param _param = null;
        BoxInfo _boxInfo = null;


        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);

            _param = (Param)args;

            var request = new GachaBoxDetailAPIRequest();
            var post = new GachaBoxDetailAPIPost();
            post.mGachaCategoryId = _param.gachaCategoryId;
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();

            _boxInfo = response.boxGachaInfo;
            
            _titleText.text = string.Format(StringValueAssetLoader.Instance["gacha.box_laps_title"], _boxInfo.currentBox.boxNumber);
            _resetButton.interactable = _boxInfo.currentBox.canReset;

            //Boxガチャの総数残数計算
            long boxTotalValue = _boxInfo.currentBox.contentList.Select(content => content.initialCount).Sum();
            long boxRemainingValue = _boxInfo.currentBox.contentList.Select(content => content.count).Sum();
            _remainingNumberText.text = String.Format( "{0:#,0}/{1:#,0}",boxRemainingValue,boxTotalValue);
            

            var items = new List<GachaBoxRewardItem.Param>();
            foreach( var content in _boxInfo.currentBox.contentList ){
                var itemParam = new GachaBoxRewardItem.Param();
                itemParam.prizeJsonData = new PrizeJsonViewData( content.prizeList[0] );
                itemParam.quantity = content.count;
                itemParam.totalaquantity = content.initialCount;
                itemParam.isHighlight = content.isFeatured;
                items.Add(itemParam);
            }

            _scrollGrid.SetItems(items);
        }
        
        public void OnClickHighlight()
        {
            OpenHighlightModal().Forget();
        }

        public void OnClickReset()
        {
            OpenResetModal().Forget();
        }


        public void OnClickClose()
        {
            Close();
        }


        async UniTask OpenHighlightModal()
        {
            var param = new GachaBoxHighlightRewardConfirmModal.Param();
            param.boxInfo = _boxInfo;
            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.GachaBoxHighlightRewardConfirm, param);
            await modal.WaitCloseAsync();
        }

        async UniTask OpenResetModal()
        {
            var param = new GachaBoxDetailResetConfirmModal.Param();
            param.gachaCategoryId = _param.gachaCategoryId;
            param.onReset = _param.onReset;

            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.GachaBoxDetailResetConfirm, param);
            await modal.WaitCloseAsync();
        }
        
    }
}
