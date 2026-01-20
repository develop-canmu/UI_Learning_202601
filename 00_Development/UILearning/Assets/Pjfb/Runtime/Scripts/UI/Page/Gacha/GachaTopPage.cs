using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;
using UnityEngine;


namespace Pjfb.Gacha
{
    public class GachaTopPage : Page
    {
        
        [SerializeField]
        private GachaTopPageView view = null;


        private GachaTopPageData topPageData = null;

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            topPageData = (GachaTopPageData)args;
            
            // バナーデータ
            List<GachaTopScrollBannerData> bannerDatas = new List<GachaTopScrollBannerData>();
            // 通常ガチャバナー
            List<GachaSettingData> normalGachaDatas = topPageData.GetGachaDatas(GachaType.Normal); 

            //画像の事前読み込み
            var tasks = new List<UniTask>();
            foreach( var gachaDatas in normalGachaDatas ){
                var id = gachaDatas.DesignNumber;
                
                tasks.Add(WebTextureManager.GetTextureAsync(GachaUtility.GetGachaLargeBannerURL(id), gameObject.GetCancellationTokenOnDestroy()));
                tasks.Add(WebTextureManager.GetTextureAsync(GachaUtility.GetGachaSmallBannerURL(id), gameObject.GetCancellationTokenOnDestroy()));
            }
            tasks.Add(WebTextureManager.GetTextureAsync(GachaUtility.GetGachaSmallBannerURL(GachaUtility.TicketGachaDesignNumber), gameObject.GetCancellationTokenOnDestroy()));
            await UniTask.WhenAll(tasks);

            for(int i = 0; i < normalGachaDatas.Count; i++)
            {
                bannerDatas.Add(new GachaTopScrollBannerData(normalGachaDatas[i].DesignNumber, OnClickBanner));
            }
            // チケットガチャバナーは必ず追加
            // チケットガチャはまとめて表示するので固定でバナーを設定
            bannerDatas.Add(new GachaTopScrollBannerData(GachaUtility.TicketGachaDesignNumber, OnClickBanner));
            // チケットスクロール
            view.TicketBannerScrollUI.Init(topPageData);
            
            // バナーデータセット
            view.GachaBannerScroll.SetBannerDatas(bannerDatas);
            // バナー変更通知登録
            view.GachaBannerScroll.ScrollGrid.OnChangedPage -= OnScrollBannerChanged;
            view.GachaBannerScroll.ScrollGrid.OnChangedPage += OnScrollBannerChanged;
            // スクロールセット
            if( topPageData.IsFocusTicketGacha ) {
                view.GachaBannerScroll.SetIndex(bannerDatas.Count - 1, false);
                OnScrollBannerChanged(bannerDatas.Count - 1);
            } else {
                view.GachaBannerScroll.SetIndex(topPageData.CurrentGachaData.Index, false);
            }
            

            // チケットバナー選択通知
            view.TicketBannerScrollUI.ScrollGrid.OnItemEvent -= OnTicketBannerSelected;
            view.TicketBannerScrollUI.ScrollGrid.OnItemEvent += OnTicketBannerSelected;
            RefreshBannerSelected();

            await view.RushUI.Init(topPageData, this.GetCancellationTokenOnDestroy());
            
            if( !topPageData.IsFocusTicketGacha ) {
                UpdateView( topPageData.CurrentGachaData );
            }

            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            // シークレットセール表示
            ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.GachaTop);
            
            base.OnOpened(args);
        }
        
        private void OnTicketBannerSelected(ScrollGridItem item, object data)
        {
            // 現在ガチャデータ更新
            topPageData.CurrentTicketGachaData = (GachaSettingData)data;
            topPageData.CurrentGachaData = topPageData.CurrentTicketGachaData;

            RefreshBannerSelected();
            // ビュー更新
            UpdateView(topPageData.CurrentGachaData);
        }

        private void RefreshBannerSelected()
        {
            if(!topPageData.ExistsGachaDatas(GachaType.Ticket)) {
                return;
            }
            // 現在選択中のチケットを非選択状態にする
            foreach( var itemParam in view.TicketBannerScrollUI.ScrollGrid.GetItems() ){
                var scrollParam =  (GachaTopTicketBannerScrollItem.Param)itemParam;
                GachaTopTicketBannerScrollItem scrollItem = (GachaTopTicketBannerScrollItem)view.TicketBannerScrollUI.ScrollGrid.GetItem(scrollParam);
                if( scrollItem != null ) {
                    scrollItem.Select(false);
                }
            }
            
            // 選択したチケットを選択状態にする
            var currentItem = (GachaTopTicketBannerScrollItem)view.TicketBannerScrollUI.ScrollGrid.GetItem(topPageData.CurrentTicketGachaData);
            currentItem?.Select(true);
        }

        /// <summary>バナー変更通知</summary>
        private void OnScrollBannerChanged(int index)
        {
            // 最後のページはチケットガチャ
            if(index == view.GachaBannerScroll.ScrollGrid.PageCount-1) {
                // チケットガチャが存在する
                if(topPageData.ExistsGachaDatas(GachaType.Ticket)) {
                    topPageData.CurrentGachaData = topPageData.CurrentTicketGachaData;
                    UpdateView(topPageData.CurrentGachaData);
                } else {
                    UpdateNoneTicketView();
                }
                return;
            }

            // バナーが変更されたのでUIを更新する
            topPageData.CurrentGachaData = topPageData.GetGachaDatas(GachaType.Normal)[index];
            UpdateView(topPageData.CurrentGachaData);
        }
        
        /// <summary>バナーの変更でUIを更新する</summary>
        private void UpdateView(GachaSettingData data)
        {
            switch (data.GachaType)
            {
                case GachaType.Normal:
                {
                    // ガチャチケットリスト非表示
                    view.TicketBannerScrollUI.gameObject.SetActive(false);
                    // キービジュアル表示
                    view.GachaKeyVisualUI.gameObject.SetActive(true);
                    view.GachaKeyVisualUI.SetData(data);
                    break;
                }
                case GachaType.Ticket:
                {
                    view.RushUI.UpdateViewTicket();
                    if( !view.TicketBannerScrollUI.Exist ) {
                        UpdateNoneTicketView();
                        return;
                    }
                    
                    // キービジュアル非表示
                    view.GachaKeyVisualUI.gameObject.SetActive(false);
                    // ガチャチケットリスト表示
                    view.TicketBannerScrollUI.gameObject.SetActive(true);
                    // ガチャ期間表示
                    view.TicketBannerScrollUI.UpdateTicketEndAtView();
                    
                    break;
                }
            }

            //実行ボタンの表示更新
            UpdateDrawView(data);
                        
            // 代替ポイント
            view.SubPointUI.gameObject.SetActive(false);
            for(int i = 0; i < data.CategoryDatas.Length; i++)
            {
                if(data.CategoryDatas[i] != null)
                {
                    // 表示する代替ポイントがあれば表示
                    if (data.CategoryDatas[i].FreeGachaPointData != null)
                    {
                        view.SubPointUI.gameObject.SetActive(true);
                        view.SubPointUI.SetData(data.CategoryDatas[i].FreeGachaPointData);
                        break;
                    }
                }
            }

            // 交換所  
            if( data.ExchangeStoreId != 0 && data.ExchangePointId != 0 ) {
                view.ExchangeUI.gameObject.SetActive(true);
                view.ExchangeUI.SetData(data.ExchangeStoreId, data.ExchangePointId);
            } else {
                view.ExchangeUI.gameObject.SetActive(false);
            }

            //ピックアップボタン
            view.PickUpButton.Init( data.ChoiceData );

            //ボタン表示
            if( data.GachaType == GachaType.Ticket ){
                view.ProbabilityButton.gameObject.SetActive(false);
                view.DetailButton.gameObject.SetActive(false);
            } else {
                view.ProbabilityButton.gameObject.SetActive(true);
                view.DetailButton.gameObject.SetActive(true);
                view.ProbabilityButton.interactable = data.DisableType != (long)GachaDisableType.ComingSoon;
                view.DetailButton.interactable = data.DisableType != (long)GachaDisableType.ComingSoon;;
            }

            //Boxガチャ
            view.BoxGachaDetailButton.gameObject.SetActive( data.IsBoxGacha );
            if( data.IsBoxGacha ) {
                view.ProbabilityButton.gameObject.SetActive( false );
            }
                    

            //ラッシュ
            if( data.GachaType == GachaType.Normal ) {
                view.RushUI.UpdateView(data);
            }
            
            // 仮想ポイントの所持個数の表示
            long pointId = 0;
            long categoryId = 0;
            bool isUsableAlternative = false;
            // 仮想ポイントが使えるガチャカテゴリを探す
            foreach (var categoryData in topPageData.CurrentGachaData.CategoryDatas)
            {
                // 単発ガチャと連続ガチャで別のポイントIdを設定できるがリリースタイミングでは考慮しなくていいので仮想ポイントが使える方のIdを使う
                if(GachaUtility.IsUsablePointAlternative(categoryData.PointId, categoryData))
                {
                    pointId = categoryData.PointId;
                    categoryId = categoryData.GachaCategoryId;
                    isUsableAlternative = true;
                    break;
                }
            }
            
            // 使用期限内の仮想ポイントを持っているか(単発か連続かで代替ポイント設定されているかが異なるのでどちらも見る)
            if (isUsableAlternative)
            {
                view.AlternativePointView.gameObject.SetActive(true);
                // 仮想ポイントのポイントを取得
                AlternativePointData alternativePoint = GachaUtility.GetPointAlternative(pointId, categoryId);
                view.AlternativePointView.SetView(alternativePoint.UserData.pointId, alternativePoint.UserData.value);
            }
            // 持ってないなら非表示
            else
            {
                view.AlternativePointView.gameObject.SetActive(false);
            }
        }


        private void UpdateDrawView(GachaSettingData data){
            if( data.IsBoxGacha  ){
                //Boxガチャの場合中身が空になった場合は非アクティブにしてバルーン表示
                var contentCount = data.BoxContentCount;
                    view.DrawGachaButtons[0].gameObject.SetActive(true);    
                    view.DrawGachaButtons[0].SetData(data.SingleGachaData);
                if( data.BoxContentCount <= 0 ) {
                    view.DrawGachaButtons[0].interactable = false;
                    if( data.CanResetBoxGacha ) {
                        view.DrawGachaButtons[0].SetDescriptionBalloonText( StringValueAssetLoader.Instance["gacha.box_reset_caution"]);
                    } else {
                        view.DrawGachaButtons[0].SetDescriptionBalloonText( "" );
                    }
                    
                } else {
                    view.DrawGachaButtons[0].interactable = true;
                }

                //連続ガチャ
                var drawCount = data.MultiGachaData.GachaCount; 
                if( drawCount >= 2 ) {
                    view.DrawGachaButtons[1].gameObject.SetActive(true);   
                    view.DrawGachaButtons[1].SetData(data.MultiGachaData);
                    view.DrawGachaButtons[1].SetCountText(drawCount);
                } else {
                    view.DrawGachaButtons[1].gameObject.SetActive(false);   
                }

            } else if( data.GachaType == GachaType.Ticket ){
                //単発ガチャ
                view.DrawGachaButtons[0].gameObject.SetActive(true);
                view.DrawGachaButtons[0].SetData(data.SingleGachaData);
                
                //連続ガチャ
                var drawCount = data.MultiGachaData.GachaCount; 
                if( drawCount >= 2 ) {
                    view.DrawGachaButtons[1].gameObject.SetActive(true);   
                    view.DrawGachaButtons[1].SetData(data.MultiGachaData);
                    view.DrawGachaButtons[1].SetCountText(drawCount);
                } else {
                    view.DrawGachaButtons[1].gameObject.SetActive(false);   
                }
                
            } else {
                for(int i = 0; i < data.CategoryDatas.Length; i++)
                {
                    if(data.CategoryDatas[i] != null && data.CategoryDatas[i].GachaCategoryId != 0 )
                    {
                        // 実行ボタン表示
                        view.DrawGachaButtons[i].gameObject.SetActive(true);
                        // 実行ボタンUIセット
                        view.DrawGachaButtons[i].SetData(data.CategoryDatas[i]);
                    }
                    else
                    {
                        // 実行ボタン非表示
                        view.DrawGachaButtons[i].gameObject.SetActive(false);
                    }
                }
            }
        
        }

        /// <summary>
        /// チケットガチャがない時のview更新
        /// </summary>
        private void UpdateNoneTicketView()
        {
            view.GachaKeyVisualUI.gameObject.SetActive(false);
            // ガチャチケットリスト表示
            view.TicketBannerScrollUI.gameObject.SetActive(true);
            //存在しなかったら非表示
            foreach( var button in view.DrawGachaButtons ) {
                button.gameObject.SetActive(false);
            }
            view.SubPointUI.gameObject.SetActive(false);
            view.ExchangeUI.gameObject.SetActive(false);
            view.PickUpButton.gameObject.SetActive(false);
            view.ProbabilityButton.gameObject.SetActive(false);
            view.DetailButton.gameObject.SetActive(false);
            view.BoxGachaDetailButton.gameObject.SetActive( false );
            view.AlternativePointView.gameObject.SetActive(false);
            //ラッシュ
            view.RushUI.UpdateViewTicket();
        }


        /// <summary>
        /// uGUI
        /// 提供割合
        /// </summary>
        public void OnClickProbabilityDetailButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaProbabilityList, topPageData.CurrentGachaData.GachaSettingId);
        }
        
        /// <summary>
        /// uGUI
        /// ピックアップボタン  
        /// </summary>
        public void OnClickChoiceButton()
        {
            OpenPickUpModal().Forget();
        }

        /// <summary>
        /// Boxガチャ詳細
        /// </summary>
        public void OnClickBoxGachaDetail()
        {
            OpenBoxGachaModal().Forget();
        }
        
        /// <summary>
        /// uGUI
        /// 単発ガチャ実行
        /// </summary>
        public void OnClickDrawSingleGachaButton()
        {
            if( !PreCheckExecuteGacha(topPageData.CurrentGachaData.SingleGachaData) ) {
                return;
            }
            OpenConfirmModalAsync(topPageData.CurrentGachaData.SingleGachaData).Forget();
        }
        
        /// <summary>
        /// uGUI
        /// 複数ガチャ実行
        /// </summary>
        public void OnClickDrawMultiGachaButton()
        {
            if( !PreCheckExecuteGacha(topPageData.CurrentGachaData.MultiGachaData) ) {
                return;
            }
            OpenConfirmModalAsync(topPageData.CurrentGachaData.MultiGachaData).Forget();
        }


        public void OnClickDetailButton(){
            Pjfb.News.NewsManager.TryShowNews(isClickNewsButton: true, isFromTitle: false, newsArticleForcedDisplayData: null, onComplete: null, openArticleUrl: topPageData.CurrentGachaData.DetailUrl);
        }

        /// <summary>
        /// ピックアップモーダル処理
        /// </summary>
        public async UniTask OpenPickUpModal()
        {
            var param = new GachaPickUpModal.Param();
            param.gachaId = topPageData.CurrentGachaData.ChoiceData.GachaChoiceId;
            param.onUpdatePickup = ()=>{
                 topPageData.CurrentGachaData.ChoiceData.UpdateSelectState(true);
            };
            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.GachaPickUp, param);
            await modal.WaitCloseAsync();

            //ピックアップボタン更新
            //モーダルでエラーが出た時ようにnullチェック
            if( this != null && view?.PickUpButton != null) {
                view.PickUpButton.UpdateViewNewBallon();
            }
            
        }

        /// <summary>
        /// Boxガチャ詳細モーダル表示
        /// </summary>
        public async UniTask OpenBoxGachaModal()
        {
            var param = new GachaBoxDetailModal.Param();
            param.gachaCategoryId = topPageData.CurrentGachaData.SingleGachaData.GachaCategoryId;
            param.onReset = (count, canReset)=>{ 
                // リセット後のBoxガチャ更新処理
                topPageData.CurrentGachaData.UpdateBoxContentCount(count);
                topPageData.CurrentGachaData.UpdateCanResetBoxGacha(canReset);
                topPageData.CurrentGachaData.UpdateBoxGachaMultiData();
                //実行ボタンの表示更新
                UpdateDrawView(topPageData.CurrentGachaData);
            };
            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.GachaBoxDetail, param);
            await modal.WaitCloseAsync();
        }

        
        

        bool PreCheckExecuteGacha( GachaCategoryData category ){
            if( !category.SettingData.IsPickUpSelected ) {
                // ピックアップが選択されていない
                ConfirmModalData data = new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"],
                    StringValueAssetLoader.Instance["gacha.none_pickup_select"],
                    null,
                    null,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (window) => {
                        window.Close();
                    })
                );
                
                AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, data);
                return false;  
            }
            // サポート器具上限チェック
            if (SupportEquipmentManager.ShowOverLimitModal()) 
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// バナーをタップした
        /// </summary>
        void OnClickBanner(ScrollGridItem item){
            
            //リストが一つの場合ItemのItemIdがおかしくなるので位置で判定
            if( item.transform.position.x >= 1.0f ) {
                view.GachaBannerScroll.ScrollGrid.NextPage();
            } else if( item.transform.position.x <= -1.0f ) {
                view.GachaBannerScroll.ScrollGrid.PrevPage();
            }
        }

        /// <summary>ガチャ実行</summary>
        protected virtual async UniTask OpenConfirmModalAsync(GachaCategoryData categoryData)
        {
            var resultPageData = await GachaUtility.ConfirmAndExecuteGacha(categoryData, this.GetCancellationTokenOnDestroy());
            if( resultPageData != null ) {
                UpdateFooterNotificationBadge();
                // 演出画面へ遷移
                GachaPage m = (GachaPage)Manager;
                m.OpenPage(GachaPageType.GachaEffect, false, resultPageData);
            }
        }
        

        /// <summary>
        /// フッターのガチャバッジ更新
        /// </summary>
        void UpdateFooterNotificationBadge(){
            var isActive = false;
            var normalGachaDatas = topPageData.GetAllGachaDatas(); 
            foreach( var data in normalGachaDatas ){
                if( CanDrawFreeGacha(data) ) {
                    isActive = true;
                    break;
                }
            }
            
            var gachaFooterButton = AppManager.Instance.UIManager.Footer.GachaButton.GetComponent<UIGachaFooterButton>();
            if( gachaFooterButton != null ) {
                gachaFooterButton.UpdateFreeBallon(isActive);
            }
        }

        /// <summary>
        /// 無料ガチャが引けるか
        /// </summary>
        bool CanDrawFreeGacha(  GachaSettingData data ){
        
            if( data.SingleGachaData != null && data.SingleGachaData.EnableDiscount ) {
                var discount = data.SingleGachaData.DiscountData;
                if ( discount.Price <= 0 ){
                    return true;
                }
            }

            if( data.MultiGachaData != null && data.MultiGachaData.EnableDiscount ) {
                var discount = data.MultiGachaData.DiscountData;
                if ( discount.Price <= 0 ){
                    return true;
                }
            }

            return false;
        }

        
    }
}
