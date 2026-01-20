using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using TMPro;

namespace Pjfb.Shop
{
    public class SecretBannerModalWindow : ModalWindow
    {
        public class SecretBannerData
        {
            private BillingRewardBonusDetail bonusDetail;
            public BillingRewardBonusDetail BonusDetail => bonusDetail;
            private string expiresAt;
            public string ExpiresAt => expiresAt;
            private bool isRemind;
            public bool IsRemind => isRemind;
            public SecretBannerData(BillingRewardBonusDetail bonusDetail, bool isRemind, string expiresAt = "")
            {
                this.bonusDetail = bonusDetail;
                this.expiresAt = expiresAt;
                this.isRemind = isRemind;
            }
        }
        
        public class ModalData
        {
            private List<SecretBannerData> bannerDataList;
            public List<SecretBannerData> BannerDataList => bannerDataList;
            private long paymentPenaltyLevel;
            public long PaymentPenaltyLevel => paymentPenaltyLevel;
            private Action onClose;
            public Action OnClose => onClose;

            public ModalData(List<SecretBannerData> bannerDataList,long paymentPenaltyLevel, Action onClose = null)
            {
                this.bannerDataList = bannerDataList;
                this.paymentPenaltyLevel = paymentPenaltyLevel;
                this.onClose = onClose;
            }
        }
        
        [SerializeField] 
        private Animator modalAnimator;
        
        [SerializeField] 
        private ScrollBanner scrollBanner;
        
        // 購入期限テキスト
        [SerializeField]
        private TextMeshProUGUI limitText;
        
        // 表示中のセール情報辞書(表示順をキーとして、セール情報を保持する)
        List<SecretBannerData> displayedBonusDetailList = new List<SecretBannerData>(); 
        
        private int bannerIndex = 0;
        // 最大表示数
        private const int maxBannerCount = 5;
        
        private ModalData modalData = null;
        
        // 更新用タイマー
        private const float UpdateInterval = 0.5f;
        private float timeSinceLastUpdate = 0;
        
        // スキップされたインデックス数
        private int skippedIndexCount = 0;
        
        // ScrollGridのアイテムリスト
        private List<ScrollGridItem> bannerItems;
        
        // ScrollBannerにセットするバナーデータリスト
        private List<SecretBannerItem.BannerData> bannerDataList = new List<SecretBannerItem.BannerData>();

        public static async UniTask OpenModal(bool isConfirm, Action onClose, List<NativeApiSaleIntroduction> triggeredSaleIntroductions = null)
        {
            // ショップの情報を取得
            ShopGetBillingRewardBonusListAPIResponse response = await ShopManager.GetShopGetBillingRewardBonusListAPI();
            
            // 現在のセール情報を取得し、セール中の商品がある場合はモーダルを開く
            List<SecretBannerData> saleDataList = GetActiveSaleData(response.billingRewardBonusList, triggeredSaleIntroductions);
            // 期間順にソートされているので最後の要素がセール中であればモーダルを開く
            if(saleDataList.Count > 0)
            {
                ModalData modalData = new ModalData(saleDataList, response.paymentPenaltyLevel, onClose);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SecretBanner, modalData);
                return;
            }
            
            // 説明なしで閉じる場合はここで終了
            if(!isConfirm)return;
            
            // セール中の商品がない場合は説明を入れる
            string title = StringValueAssetLoader.Instance["common.confirm"];
            string message = StringValueAssetLoader.Instance["shop.no_purchasable_items"];
            ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
            ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, negativeButton);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        
        // 現在有効なセールを取得する
        private static List<SecretBannerData> GetActiveSaleData(BillingRewardBonusDetail[] detailArray, List<NativeApiSaleIntroduction> targetSaleIntroductions = null)
        {
            // シークレットセール一覧取得
            NativeApiSaleIntroduction[] saleList;
            if (targetSaleIntroductions == null)
            {
                // 発生中を全件
                saleList = ShopManager.GetSaleIntroductionsOrderPriority();
            }
            else
            {
                // 部分的に発生したものを優先ソート
                saleList = ShopManager.GetSpecifySaleIntroductionsOrderedByPriority(targetSaleIntroductions);
            }
            
            List<SecretBannerData> bannerDataList = new List<SecretBannerData>();
            foreach (NativeApiSaleIntroduction saleData in saleList)
            {
                // 期限外の場合はスキップ
                string expireAt = saleData.expireAt;
                TimeSpan remainTime = ShopManager.GetRemainTimeSpan(expireAt, AppTime.Now);
                if (remainTime <= TimeSpan.Zero) continue;

                // 商品情報を取得
                BillingRewardBonusDetail bonusDetail = detailArray.FirstOrDefault(v => v.mSaleIntroductionId == saleData.mSaleIntroductionId);
                
                // 商品情報がないまたは購入回数が上限に達している、セールが有効でない場合はスキップ
                if (bonusDetail == null || (bonusDetail.buyLimit != 0 && bonusDetail.buyCount >= bonusDetail.buyLimit) || bonusDetail.saleIntroductionActiveFlg == false)
                {
                    // リストから削除しておく
                    ShopManager.RemoveSaleIntroduction(saleData.mSaleIntroductionId);
                    continue;
                }
                
                // 表示用のセール情報を作成
                SecretBannerData bannerData = new SecretBannerData(bonusDetail, saleData.isRemind, expireAt);
                bannerDataList.Add(bannerData);
            }
            return bannerDataList;
        }
        
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        public static async UniTask OpenModalTest()
        {
            // ショップの情報を取得
            ShopGetBillingRewardBonusListAPIResponse response = await ShopManager.GetShopGetBillingRewardBonusListAPI();
            List<SecretBannerData> saleDataList = new List<SecretBannerData>();
            foreach (var bonus in response.billingRewardBonusList)
            {
                // 適当なIDをセット
                bonus.mSaleIntroductionId = MasterManager.Instance.saleIntroductionMaster.values.First().id;
                // 表示用のセール情報を作成
                SecretBannerData bannerData = new SecretBannerData(bonus, true, DateTime.MaxValue.ToString());
                saleDataList.Add(bannerData);
            }
            
            ModalData modalData = new ModalData(saleDataList, 0);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SecretBanner, modalData);
        }
#endif

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (ModalData)args;
            await base.OnPreOpen(args, token);
        }

        protected override async UniTask OnOpen(CancellationToken token)
        {
            InitializeUI();
            ShopManager.RegistrationOpenSecretBannerModal();
            await base.OnOpen(token);
        }

        protected override void OnOpened()
        {
            // アニメーション完了したので、非表示にしていたバナーを表示する
            foreach (var item in bannerItems)
            {
                item.gameObject.SetActive(true);
            }
        }

        private void InitializeUI()
        {
            // バナーのデータを取得
            RefreshBannerData();
            scrollBanner.ScrollGrid.OnChangedPage -= OnSetCurrentData;
            scrollBanner.ScrollGrid.OnChangedPage += OnSetCurrentData;
            
            // 初期選択データを設定
            OnSetCurrentData(0);
            // 最初に表示するバナー以外は非表示にする
            bannerItems = new List<ScrollGridItem>();
            for (int i = 0; i < bannerDataList.Count; i++)
            {
                if (i == bannerIndex) continue;
                ScrollGridItem item = scrollBanner.ScrollGrid.GetItem(bannerDataList[i]);
                if (item != null)
                {
                    item.gameObject.SetActive(false);
                    bannerItems.Add(item);
                }
            }
            
        }

        private void OnSetCurrentData(int index)
        {
            // indexを更新
            bannerIndex = index;

            SecretBannerData bannerData = displayedBonusDetailList[bannerIndex];
            // 時間表示
            string expireAt = bannerData.ExpiresAt;
            DateTime remainTime = ShopManager.GetDateTimeByString(expireAt);
            limitText.text = StringValueAssetLoader.Instance["shop.buy.remaintime"] + remainTime.GetRemainingString(AppTime.Now);
        }

        private void Update()
        {
            if(displayedBonusDetailList.Count == 0) return;
            // 一定時間ごとに更新
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate < UpdateInterval) return;
            
            // セール情報の更新
            UpdateBonusDetails();
            // 残り時間の更新
            OnSetCurrentData(bannerIndex);
            // タイマーリセット
            timeSinceLastUpdate = 0;
        }

        // リスト内全てのセール情報の残り時間を見る
        private void UpdateBonusDetails()
        {
            bool isUpdate = false;
            foreach (SecretBannerData bannerData in displayedBonusDetailList)
            {
                bool isInTime = ShopManager.IsInRemainTime(bannerData.ExpiresAt, AppTime.Now);
                if (!isInTime)
                {
                    isUpdate = true;
                    break;
                }
            }
            
            // 期限切れのセールがある場合は再取得してUIを更新
            if (isUpdate)
            {
                RefreshBannerData();
            }
        }


        private void RefreshBannerData()
        {
            // バナーのデータを取得
            bannerDataList = new List<SecretBannerItem.BannerData>();
            
            SecretBannerData cache = null;
            if (displayedBonusDetailList.Count > 0)
            { 
                cache = displayedBonusDetailList[bannerIndex];
            }
            displayedBonusDetailList.Clear();
            int index = 0;
            
            // 期限内のセール情報を取得
            for(int i = skippedIndexCount; i < modalData.BannerDataList.Count; i++)
            {
                SecretBannerData data = modalData.BannerDataList[i];
                // 時間切れのセールはスキップ
                bool isInTime = ShopManager.IsInRemainTime(data.ExpiresAt, AppTime.Now);
                if (!isInTime)
                {
                    skippedIndexCount++;
                    continue;
                }
                
                var saleIntroduction = MasterManager.Instance.saleIntroductionMaster.FindData(data.BonusDetail.mSaleIntroductionId);
                // 表示用のバナーデータを作成
                SecretBannerItem.BannerData bannerData = new SecretBannerItem.BannerData(saleIntroduction.imageFilePath, data.IsRemind);
                bannerDataList.Add(bannerData);
                // データをキャッシュ
                displayedBonusDetailList.Add(data);
                index++;
                // 最大表示数に達したら終了
                if (index >= maxBannerCount) break;
            }
            
            // 表示するセールが無い場合は閉じる
            if (displayedBonusDetailList.Count == 0)
            {
                if (cache != null)
                {
                    // 完全に閉じるまで最後に表示していたものを表示する
                    displayedBonusDetailList.Add(cache);
                    bannerIndex = 0;
                }
                CloseModal();
                return;
            }
            
            // インデックス調整
            if (bannerIndex > 0)
            {
                if (bannerIndex >= displayedBonusDetailList.Count)
                {
                    bannerIndex = displayedBonusDetailList.Count - 1;
                }
                else
                {
                    // 前回表示していたセールがある場合はそのインデックスをセット
                    bannerIndex -= skippedIndexCount;
                    if (bannerIndex < 0)
                    {
                        bannerIndex = 0;
                    }
                }
            }
            
            // スクロールバナーにデータをセット
            scrollBanner.SetBannerDatas(bannerDataList);
        }

        public void OnClickPurchaseButton()
        {
            string expireAt = displayedBonusDetailList[bannerIndex].ExpiresAt;
            TimeSpan remainTime = ShopManager.GetRemainTimeSpan(expireAt, AppTime.Now);
            
            // 期限切れの場合は購入エラーのモーダルを出す
            if(remainTime <= TimeSpan.Zero)
            {
                string title = StringValueAssetLoader.Instance["shop.purchase_error_title"];
                string message = StringValueAssetLoader.Instance["shop.purchase_error_body"];
                ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, negativeButton);
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            
            // 購入確認モーダルの購入ボタン押下時の処理
            Action onPurchaseClick = () =>
            {
                // 購入確認モーダル以外を閉じる
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window is not ShopProductPurchaseModal);
            };
            // 購入確認モーダルを開く
            ShopProductPurchaseModal.Open(
                new ShopProductPurchaseModal.Data(
                    displayedBonusDetailList[bannerIndex].BonusDetail,
                    LocalSaveManager.saveData.shopPurchaseType, 
                    null, 
                    true, 
                    onPurchaseClick, 
                    modalData.PaymentPenaltyLevel >= ShopManager.BanPaymentPenaltyLevel 
                )
            );
        }
        
        public void OnClickCloseButton()
        {
            CloseModal();
        }

        private void CloseModal()
        {
            Close(modalData.OnClose);
        }

    }
}