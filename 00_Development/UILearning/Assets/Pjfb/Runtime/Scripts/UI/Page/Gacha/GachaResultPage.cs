using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Master;
using Pjfb.UserData;
using CruFramework.Audio;
using CruFramework.ResourceManagement;


namespace Pjfb.Gacha
{
    public class GachaResultPage : Page
    {
        /// <summary>
        /// 初期入手判別用の構造体
        /// </summary>
        private readonly struct NewItems
        {
            public long Id { get; }
            public ItemIconType Type { get; }

            // コンストラクタ
            public NewItems(long id, ItemIconType type)
            {
                Id = id;
                Type = type;
            }
        }

        readonly int startEffectInterval = 1000;
        readonly int rushEffectInterval = 400;

        [SerializeField]
        private int prizeCountPerLine = 3;
        
        [SerializeField]
        private GachaPrizeIconWrap prizeIconWrap = null;
        
        
        [SerializeField]
        private GameObject prizeLine = null;
        
        [SerializeField]
        private GachaResultPageView singleView = null;
        
        [SerializeField]
        private GachaResultPageView multiView = null;

        [SerializeField]
        private CharacterGetEffect getEffect = null;
        [SerializeField]
        private GachaResultRushEffect rushEffect = null;
        [SerializeField]
        GameObject skipTrigger = null;

        [SerializeField]
        GachaRetryAnimationController retryAnimation = null;
        [SerializeField]
        GachaResultPageAnimationController resultPageAnimator = null;

        [SerializeField]
        GachaResultPageCommonView commonView = null;

        [SerializeField]
        private AudioClip bgm = null;
        [SerializeField]
        private AudioClip spGachaBgm = null;


        private GameObject[] prizeLineList = null;
        private GachaPrizeIconWrap[] prizeIconWrapList = null;


        private GachaResultPageData pageData = null;
        private bool isBackTop = false;
        private List<GachaRushData> playRushEffectDataList = null;

        private long executeGachaSettingId = 0;
        private long executeGachaType = 0;
        
        // H2MD等でかいアセットを適宜解放する用
        private ResourcesLoader gachaResourcesLoader = new ResourcesLoader();

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            pageData = (GachaResultPageData)args;
            GachaResultPageView pageView = GetView();
            skipTrigger.SetActive(true);
            singleView.gameObject.SetActive(false);
            multiView.gameObject.SetActive(false);

            commonView.Init(pageData);
            if( pageData.IsComebackPage ){
                resultPageAnimator.OpenPendingPage();
                commonView.ViewPendingButtons();                

            } else {
                resultPageAnimator.OpenResultPage();
                commonView.HideButtonAll();
            }

            PlayBGM( pageData.PrizeList );


            //報酬作成
            CreatePrize(pageView);

            playRushEffectDataList = CreatePlayRushEffectDataList();

            await InitRush(pageView, commonView.GetCommonView());

            pageView.gameObject.SetActive(true);
            
            //実行ガチャ情報の保存
            if( pageData.GachaCategoryData != null ) {
                executeGachaSettingId = pageData.GachaCategoryData.SettingData.GachaSettingId;
                executeGachaType = (int)pageData.GachaCategoryData.SettingData.GachaType;
                pageData.GachaCategoryData.UpdatePointId();
            } else if( pageData.PendingData != null ) {
                executeGachaSettingId = pageData.PendingData.GachaSettingId;
                executeGachaType = pageData.PendingData.GachaSettingType;
            }

            await base.OnPreOpen(args, token);
        }

        protected override void OnClosed(){
            if( isBackTop ) {
                AppManager.Instance.UIManager.Header.Show();
                AppManager.Instance.UIManager.Footer.Show();
            }
        }

        protected override UniTask OnMessage(object value)
        {
            if (value is PageManager.MessageType type)
            {
                switch (type)
                {
                    case PageManager.MessageType.BeginFade:
                        // ヘッダーフッターは非表示
                        AppManager.Instance.UIManager.Header.Hide();
                        AppManager.Instance.UIManager.Footer.Hide();

                        break;
                }
            }
            return base.OnMessage(value);
        }


        protected override async UniTask<bool> OnPreLeave(CancellationToken token) {
            GC.Collect();
            await Resources.UnloadUnusedAssets().ToUniTask();
            gachaResourcesLoader.Release();
            GC.Collect();

            return true;
        }
        

        protected override void OnOpened(object args){
            PlayResultEffect().Forget();
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL

            // 重複してた場合は開発環境のみエラーモーダルを表示する
            for(int i = 0; i < pageData.PrizeList.Length; i++)
            {
                List<(long, ItemIconType)> idList = new List<(long, ItemIconType)>();
                // 1枠分のリスト
                for(int n = 0; n < pageData.PrizeList[i].ContentList.Length; n++)
                {
                    // idが重複してる
                    if(idList.Contains((pageData.PrizeList[i].ContentList[n].Id, pageData.PrizeList[i].ContentList[n].ItemIconType)))
                    {
                        ConfirmModalData modalData = new ConfirmModalData();
                        modalData.Title = "警告";
                        modalData.Message = $"ガチャ報酬{i+1}枠目で報酬idの重複があります。\ntype:{pageData.PrizeList[i].ContentList[n].ItemIconType}\nid:{pageData.PrizeList[i].ContentList[n].Id}";
                        modalData.Caution = "マスタデータが正しいか確認してください";
                        modalData.NegativeButtonParams = new ConfirmModalButtonParams("はい", window => window.Close());
                        AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, modalData);
                    }
                    // idが重複してない
                    else
                    {
                        idList.Add((pageData.PrizeList[i].ContentList[n].Id, pageData.PrizeList[i].ContentList[n].ItemIconType));
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 戻るボタンが押された
        /// </summary>
        public void OnClickBack(){
            // ページ引数
            var pageArgs = new GachaPageArgs();
            pageArgs.FocusGachaSettingId = executeGachaSettingId;
            pageArgs.FocusTicketGacha = executeGachaType == (int)GachaType.Ticket;

            isBackTop = true;
            if( pageData.PendingData != null ) {
                AddPendingFooterNotificationBadge();
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
            } else {
                GachaPage m = (GachaPage)Manager;
                m.GetGachaListAndOpenPageAsync(pageArgs).Forget();
            }
            
        }

        /// <summary>
        /// もう一度ひくボタンが押された
        /// </summary>
        public void OnClickOneMore(){
            isBackTop = false;
            ExecuteGachaOneMore().Forget();
        }

        /// <summary>
        /// 確定ボタンが押された
        /// </summary>
        public void OnClickPendingFix(){
            ConfirmModalData data = new ConfirmModalData(
            StringValueAssetLoader.Instance["common.confirm"],
            StringValueAssetLoader.Instance["gacha.pending_fix_confirm"],
            null,
            new ConfirmModalButtonParams(StringValueAssetLoader.Instance["gacha.pending_fix_finish"],(window)=>{
                ConnectPendingFixAPI(window).Forget();
            }),
            new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
        }

        /// <summary>
        /// スキップトリガーがタップされた
        /// </summary>
        public void OnClickSkipTrigger(){
            
            foreach(var icon in prizeIconWrapList ){
                if( icon.prizeIcon.IsOpened ){
                    continue;
                }
                if( icon.prizeIcon.IsPlayGetEffect ){
                    break;
                }
                icon.prizeIcon.Skip();
            }
        }

        /// <summary>
        /// 報酬作成
        /// </summary>
        /// <param name="pageView"></param>
        private void CreatePrize( GachaResultPageView pageView ){
            // 報酬表示の行数
            var prizeCount = pageData.PrizeList.Length;
            int contentLineCount = (prizeCount /  prizeCountPerLine);
            if( prizeCount % prizeCountPerLine != 0 ){
                contentLineCount += 1;
            }
            
            // 行を複製
            prizeLineList = new GameObject[contentLineCount];
            for(int i = 0; i < prizeLineList.Length; i++)
            {
                prizeLineList[i] = GameObject.Instantiate<GameObject>(prizeLine, pageView.Content.transform);
                prizeLineList[i].gameObject.SetActive(true);
            }
            
            prizeIconWrapList = new GachaPrizeIconWrap[pageData.PrizeList.Length];

            // 報酬生成
            var newItems = new List<NewItems>();
            //最初に初期入手となるIdを保存
            for(int i = 0; i < pageData.PrizeList.Length; i++){
                var content = pageData.PrizeList[i].ContentList[0];
                if( content.IsNew ){
                    newItems.Add(new NewItems(content.Id, content.ItemIconType));
                }
            }
            
            // 初入手のアイテムが複数出た場合に演出を一度だけ表示するための管理リスト
            var doActiveNewItems = new List<NewItems>();

            for(int i = 0; i < pageData.PrizeList.Length; i++)
            {
                // どの行に配置するか
                int lineIndex = i / prizeCountPerLine;
                // アイコン複製
                prizeIconWrapList[i] = GameObject.Instantiate<GachaPrizeIconWrap>(prizeIconWrap, prizeLineList[lineIndex].transform);
                // 初期表示
                var isNewActive = false;
                var content = pageData.PrizeList[i].ContentList[0];
                var currentItem = new NewItems(content.Id, content.ItemIconType);
                // 初期入手か
                bool isNew = CheckContainItem(newItems, currentItem);
                // 初期入手の同じアイテムが存在するか
                bool isDoActiveNew = CheckContainItem(doActiveNewItems, currentItem);
                if( isNew && !isDoActiveNew) {
                    isNewActive = true;
                    doActiveNewItems.Add(currentItem);
                }
                var prizeWrap = prizeIconWrapList[i];
                prizeWrap.CreateIcon();
                prizeWrap.prizeIcon.SetView(pageData.PrizeList[i].ContentList, isNewActive, isNew, pageData.IsComebackPage);
                prizeWrap.prizeIcon.gameObject.SetActive(true);
            }
        }
        
        // 一致するものがリストにあるかチェック
        private bool CheckContainItem(List<NewItems> list, NewItems currentItem)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (currentItem.Id == list[i].Id && currentItem.Type == list[i].Type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ラッシュ設定
        /// </summary>
        /// <param name="pageView"></param>
        private async UniTask InitRush( GachaResultPageView pageView, GachaResultPageCommonView.CommonView commonView ){
            if( pageData.GachaCategoryData == null ) {
                commonView.rushLogo.gameObject.SetActive(false);
                return;
            }

            //logo設定
            if( pageData.GachaCategoryData.RushData != null && pageData.GachaCategoryData.RushData.rushId > 0 && !pageData.GachaCategoryData.RushData.IsHiddenRush()) {
                await commonView.rushLogo.SetTextureAsync(pageData.GachaCategoryData.RushData.imageNumber);
            } else {
                commonView.rushLogo.gameObject.SetActive(false);
            }

            //演出画像事前読み込み
            await rushEffect.Init(pageData.GachaCategoryData ,playRushEffectDataList);
        }


        /// <summary>
        /// 結果演出
        /// </summary>
        private async UniTask PlayResultEffect( ){
            var token = this.GetCancellationTokenOnDestroy();
            
            if( !pageData.IsComebackPage ) {
                //Fadeアニメーションを待つために待機
                await UniTask.Delay(startEffectInterval, cancellationToken:token );
            }
            
            //カード演出
            await PlayOpenCardsEffect();            
    
            //Rush演出
            await PlayRushEffect(token);

            //メモリーボールチュートリアル
            await AppManager.Instance.TutorialManager.OpenMemoryBollTutorialAsync();
            
            //カードのアイコン切り替え再生
            PlayCardsChangeIconAnimation();

            //自動売却モーダル表示
            OpenAutoGotModal(pageData.AutoGotList);

            skipTrigger.SetActive(false);

            //ボタン表示
            commonView.ViewResultButtons();

            await PendingProcess();

            OnCompletedAction();
        }
        

        /// <summary>
        /// カードオープン演出
        /// </summary>
        /// <returns></returns>
        private async UniTask PlayOpenCardsEffect( ){
            foreach(var icon in prizeIconWrapList){
                await icon.prizeIcon.Open( getEffect, false, gachaResourcesLoader);
            }
        }

        /// <summary>
        /// ラッシュエフェクト
        /// </summary>
        private async UniTask PlayRushEffect( CancellationToken token ){
            if( rushEffect.IsPlayEffect() ) {
                await UniTask.Delay(rushEffectInterval, cancellationToken:token );
                await rushEffect.PlayEffect(token);
            }
            
        }

        /// <summary>
        /// カードのアイコン切り替え再生
        /// </summary>
        /// <returns></returns>
        private void PlayCardsChangeIconAnimation( ){
            foreach(var icon in prizeIconWrapList){
                icon.prizeIcon.PlayIconChangeAnimation();
            }
        }

        private void ResetCardsChangeIconAnimation( ){
            foreach(var icon in prizeIconWrapList){
                icon.prizeIcon.ResetCardsChangeIconAnimation();
            }
        }
        
        private void StopCardsChangeIconAnimation( ){
            foreach(var icon in prizeIconWrapList){
                icon.prizeIcon.StopIconChangeAnimation();
            }
        }

        

        /// <summary>
        /// 自動売却モーダル表示
        /// </summary>
        private void OpenAutoGotModal( PrizeJsonViewData[] autoGotList ){
            if( autoGotList == null || autoGotList.Length <= 0 ) {
                return;
            }

            var param = new GachaAutoGotModal.Param();
            param.prizeList = autoGotList;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaAutoGot, param);
        }

        private void OpenAutoGotModal( NativeApiAutoSell autoSell ){
            if( autoSell == null || autoSell.prizeListGot == null ||  autoSell.prizeListGot.Length <= 0 ) {
                return;
            }

            var autoGotList = new PrizeJsonViewData[autoSell.prizeListGot.Length];
            // データセット
            for(int i = 0; i < autoSell.prizeListGot.Length; i++)
            {
                autoGotList[i] = new PrizeJsonViewData(autoSell.prizeListGot[i]);
            }

            OpenAutoGotModal(autoGotList);
        }

        

        private GachaResultPageView GetView()
        {
            return pageData.PrizeList.Length > 1 ? multiView : singleView;
        }


        /// <summary>
        /// ラッシュ演出を再生するデータリストを取得
        /// </summary>
        /// <returns></returns>
        private List<GachaRushData> CreatePlayRushEffectDataList()
        {
            if( pageData.RushDataList == null || pageData.RushDataList.Count <= 0 ) {
                return null;
            }

            if( pageData.GachaCategoryData.RushData == null || pageData.GachaCategoryData.RushData.rushId <= 0 ) {
                return pageData.RushDataList;
            }
            
            var dataList = new List<GachaRushData>();
            foreach( var rushData in pageData.RushDataList ){
                
                if( rushData.rushId != pageData.GachaCategoryData.RushData.rushId || rushData.isFinished > 0 ) {
                    dataList.Add(rushData);
                }
            }
            return dataList;
        }

        /// <summary>
        /// ガチャをもう一度引く
        /// </summary>
        async UniTask ExecuteGachaOneMore(){
            // サポート器具上限チェック
            if (SupportEquipmentManager.ShowOverLimitModal()) 
            {
                return;
            }
            //ラッシュが発生していたらローカルのカテゴリデータを更新
            if( pageData.RushDataList != null ) {
                foreach( var rushData in pageData.RushDataList ){
                    if( pageData.GachaCategoryData.GachaCategoryId == rushData.gachaCategoryId ) {
                        pageData.GachaCategoryData.UpdateRushData(rushData);
                        break;
                    }
                }
            }
            var resultPageData = await GachaUtility.ConfirmAndExecuteGacha(pageData.GachaCategoryData, this.GetCancellationTokenOnDestroy());
            if( resultPageData != null ) {
                // 演出画面へ遷移
                GachaPage m = (GachaPage)Manager;
                m.OpenPage(GachaPageType.GachaEffect, false, resultPageData);
            }
        }

        /// <summary>
        /// 保留情報の表示更新
        /// </summary>
        private async UniTask PendingProcess( ){
            // 保留情報がない場合は終了
            if( pageData.PendingData == null ) {
                AllHidePendingView();
                return;  
            }

            // 保留情報をアイコンに反映
            for( int i=0; i<pageData.PrizeList.Length; ++i ){
                var prizeData = pageData.PrizeList[i];
                prizeIconWrapList[i].UpdatePendingView(prizeData.PendingData, OnClickPending);
            }

            
            if( !pageData.IsComebackPage ) {
                await resultPageAnimator.PlayOutResultPage();
                commonView.ViewPendingButtons();
                await resultPageAnimator.PlayInPendingPage();
            } else {
                // ホームから保留画面に戻ってきた場合
                commonView.ViewPendingButtons();
            }
            
            if( pageData.PendingData.ExpireAt < AppTime.Now ){
                //期限切れ
                OpenPendingPeriodEnd();
            }
        }

        private void AllHidePendingView( ){
            for( int i=0; i<prizeIconWrapList.Length; ++i ){
                prizeIconWrapList[i].prizeIcon.HidePendingView();
            }
        }

        // 引き直し実行
        private void OnExecutedPendingRetry( GachaRetryConfirmModal modal, GachaPrizeIconWrap icon, PrizeJsonViewData[] dataList, GachaPendingFrame pendingData){            
            PendingRetryProcess(modal, icon, dataList, pendingData).Forget();
        }

        private async UniTask PendingRetryProcess(GachaRetryConfirmModal modal, GachaPrizeIconWrap icon, PrizeJsonViewData[] dataList, GachaPendingFrame pendingData){
            
            var common = commonView.GetCommonView();
            //終了までボタンを押せないように
            foreach( var button in common.buttons ){
                button.interactable = false;
            }
            
            await modal.CloseAsync();

            // 入手するメインとなる0番目のデータで判定する
            var isFirstNew = dataList[0].IsNew;
            var iconId = dataList[0].Id;
            foreach( var iconWrap in prizeIconWrapList ){
                if( icon == iconWrap ) {
                    continue;
                }
                if ( iconId ==  iconWrap.prizeIcon.MainViewData.Id){
                    isFirstNew = false;
                    break;
                }
            }
            
            StopCardsChangeIconAnimation();

            //交換後ポイント取得
            if (pageData.PendingData != null)
            {
                var exchangePointId = pageData.PendingData.StoreMPointId;
                var afterPoint = GachaUtility.FetchPointValue(exchangePointId);
                pageData.SetPointData(afterPoint);
            }

            commonView.Init(pageData, false);

            var rarity = GachaUtility.GetGachaPrizeRarity(dataList[0]);
            await retryAnimation.PlayOpenAnimation( rarity );

            //iconを作成しなおし
            icon.CreateIcon();
            icon.prizeIcon.SetView( dataList, isFirstNew, dataList[0].IsNew, false );
            icon.prizeIcon.UpdateViewOpenFinishedIdle();

            //newIconの更新
            UpdateNewIcon();

            //獲得演出の調整
            if (icon.prizeIcon.MainViewData.ItemIconType == ItemIconType.Character)
            {
                await getEffect.PreLoadAsset(icon.prizeIcon.MainViewData.Id, gachaResourcesLoader);
                await getEffect.PlayVoiceWithPreLoad(icon.prizeIcon.MainViewData.Id);
            }
            var getEffectTask = getEffect.PlayAsyncWithPreLoad();
            await UniTask.Yield();

            if( !icon.prizeIcon.IsPlayEffect(icon.prizeIcon.MainViewData, isFirstNew) ) 
            {
                getEffect.OnSkip();
            }
            await retryAnimation.PlayCloseAnimation( );
            await getEffectTask;
            
            ResetCardsChangeIconAnimation();
            PlayCardsChangeIconAnimation();
            icon.UpdatePendingView( pendingData );

            if( !CanRetry() ) {
                //引き直しができなかったら確定させる
                await ConnectPendingFixAPI(null);
            }

            //終了までボタンを押せないように
            //終了処理を待つため１フレームディレイ
            await UniTask.Yield();
            foreach( var button in common.buttons ){
                button.interactable = true;
            }
        }


        // 保留の確定
        private async UniTask ConnectPendingFixAPI(ModalWindow window){
            // APIとアニメーション中に確定ボタンが押せるのでタッチガード表示する
            await AppManager.Instance.LoadingActionAsync(async () => {
                if( window != null ) {
                    await window.CloseAsync();
                }
                
                var request = new GachaPendingFixAPIRequest();
                var post = new GachaPendingFixAPIPost();
                post.uGachaResultPendingId = pageData.PendingData.GachaResultPendingId;
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                
                var response = request.GetResponseData();
                if( response.autoSell != null ){
                    //自動売却
                    OpenAutoGotModal( response.autoSell );
                }

                pageData.ClearPendingData();
                RemovePendingFooterNotificationBadge();

                AllHidePendingView();

                await resultPageAnimator.PlayOutPendingPage();
                commonView.ViewResultButtons();
                await resultPageAnimator.PlayInResultPage();
            });
        }

        private void OnClickPending( GachaPrizeIconWrap icon ){
            if( pageData.PendingData == null 
            || icon.pendingData == null
            || !icon.pendingData.CanRetry ) {
                //  引き直しができない
                return;
            }

            if( pageData.PendingData.ExpireAt < AppTime.Now ){
                //期限切れ
                OpenPendingPeriodEnd();
                return;
            }
            
            // 消費ポイントが足りているか
            if( IsEnoughPoint(icon.pendingData) ) {
                var param = new GachaRetryConfirmModal.Param();
                param.icon = icon;
                param.pendingData = icon.pendingData;
                param.gachaResultPendingId = pageData.PendingData.GachaResultPendingId;
                param.OnExecutedPendingRetry = OnExecutedPendingRetry;
                param.gachaCategoryId = pageData.PendingData.GachaCategoryId;
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaRetryConfirm, param);
            } else {
                var param = new GachaRetryPointShortageModal.Param();
                param.pendingData = icon.pendingData;
                param.gachaCategoryId = pageData.PendingData.GachaCategoryId;
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaRetryPointShortage, param);
            }
        }


        private bool IsEnoughPoint( GachaPendingFrameData data ){
            var pointMaster = MasterManager.Instance.pointMaster.FindData(data.RetryPointId);
            // 通常のポイント
            long possessionCount = 0;
            // 仮想ポイント
            long pointAlternativeCount = 0;
            if(UserDataManager.Instance.point.data.ContainsKey(data.RetryPointId)) {
                possessionCount = UserDataManager.Instance.point.Find(data.RetryPointId).value;
            }
            
            // 仮想ポイントの所持個数を取得(使えない場合は0が入る)
            pointAlternativeCount = GachaUtility.GetPointAlternativeCount(data.RetryPointId, pageData.PendingData.GachaCategoryId);
            
            // 通常のポイントと仮想ポイントの合計が必要数に足りているか
            return possessionCount + pointAlternativeCount >= data.RetryPrice;
        }

        /// <summary>
        /// 引き直しができるか
        /// </summary>
        /// <returns></returns>
        private bool CanRetry(){
            if( pageData.PendingData == null ) {
                return false;
            }
            foreach( var icon in prizeIconWrapList ) {
                if( icon.pendingData != null && icon.pendingData.CanRetry ) {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// フッターのガチャバッジ更新
        /// </summary>
        void AddPendingFooterNotificationBadge(){
            //保留中表示を表示
            var gachaFooterButton = AppManager.Instance.UIManager.Footer.GachaButton.GetComponent<UIGachaFooterButton>();
            if( gachaFooterButton != null ) {
                gachaFooterButton.UpdatePendingBallon(true);
            }
        }


        void RemovePendingFooterNotificationBadge(){
            //保留中表示を消す
            var gachaFooterButton = AppManager.Instance.UIManager.Footer.GachaButton.GetComponent<UIGachaFooterButton>();
            if( gachaFooterButton != null ) {
                gachaFooterButton.UpdatePendingBallon(false);
            }
        }


        void OpenPendingPeriodEnd( ){
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["gacha.pending_period_end_title"],
                StringValueAssetLoader.Instance["gacha.pending_period_end_text"],
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => window.Close())
                );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }


        void UpdateNewIcon(){
            var isNewIds = new List<long>();

            //最初に初期入手となるIdを保存
            foreach(var icon in prizeIconWrapList ){
                if( icon.prizeIcon.IsNewByServer ){
                    isNewIds.Add(icon.prizeIcon.MainViewData.Id);
                }
            }

            // 初入手の同アイテムが複数出た場合にラベルを一度だけ表示するための管理リスト
            var isDoActiveNewIds = new List<long>();
            foreach(var icon in prizeIconWrapList ){
                var isNewActive = false;
                var iconId = icon.prizeIcon.MainViewData.Id;

                var isNewId = isNewIds.Contains(iconId);
                if( isNewId && !isDoActiveNewIds.Contains( iconId ) ) {
                    isNewActive = true;
                    isDoActiveNewIds.Add( iconId );
                }
                icon.prizeIcon.SetNewIconActive( isNewActive );
            }   
        }

        void PlayBGM( GachaPrizeData[] prizeDatas ){
            if( prizeDatas == null || prizeDatas.Length <= 0 ) {
                return;
            }

            PrizeJsonViewData prize = prizeDatas[0].ContentList[0];
            if( prize == null ) {
                return;
            }

            if(prize.ItemIconType == ItemIconType.Character)
            {
                CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(prize.Id);
                if(mChara.cardType == CardType.SpecialSupportCharacter)
                {
                    AudioManager.Instance.PlayBGM(spGachaBgm);
                    return;
                }
            }
            
            AudioManager.Instance.PlayBGM(bgm);
        }
            

        protected virtual void OnCompletedAction()
        {
        }

    }
}
