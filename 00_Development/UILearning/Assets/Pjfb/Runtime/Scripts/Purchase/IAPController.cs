using com.adjust.sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using Pjfb;
using Pjfb.Master;
using Logger = CruFramework.Logger;

/// <summary>
/// Step 1. UnityIAPの初期化 InitializePurchasing
/// Step 2. 購入処理 BuyProductID
/// -> ゲームAPIにレシートを送信する
/// Step 3. レシートの検証が完了 ConfirmPendingPurchase
/// Step 1.を呼び出した時に、GetPendingProductsを確認して
/// もしPendingしている購入トランザクションがある場合はゲームAPIにレシートを送信する
/// </summary>
namespace Pjfb
{
    public class IAPController : IDetailedStoreListener
    {
        private IStoreController _iStoreController;
        private IExtensionProvider _iExtensionProvider;

        /// <summary>
        /// 消費型アイテム
        /// </summary>
        public List<PurchaseProduct> ConsumableProducts = new List<PurchaseProduct>();

        /// <summary>
        /// UnityEditorのダミーストア名
        /// </summary>
        private const string UnityEditorStore = "dummy";

        /// <summary>
        /// 初期化成功時のデリゲート
        /// </summary>
        public delegate void InitializeSuccessEvent(Product[] products);

        /// <summary>
        /// 初期化完了通知イベント
        /// </summary>
        private InitializeSuccessEvent OnInitializeSuccess;
        
        /// <summary>
        /// 初期化失敗時のデリゲート
        /// </summary>
        public delegate void InitializeFailureEvent(InitializationFailureReason reason);

        /// <summary>
        /// 初期化失敗通知イベント
        /// </summary>
        private InitializeFailureEvent OnInitializeFailure;
        
        /// <summary>
        /// 課金完了通知イベント
        /// </summary>
        private Action<Product> OnPurchaseSuccessResultAction;

        /// <summary>
        /// 課金失敗通知イベント
        /// </summary>
        private Action<Product,PurchaseFailureReason> OnPurchaseFailureResultAction;

        /// <summary>
        /// Pendingとなったアイテム
        /// </summary>
        private Dictionary<string, Product> m_pendingProducts;

        /// <summary>
        /// インスタンス
        /// </summary>
        private static IAPController _instance;

        /// <summary>
        /// シングルトン
        /// </summary>
        /// <value>Purchaser</value>
        public static IAPController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IAPController();
                }
                return _instance;
            }
        }

        //課金プラットフォーム
        public enum PlatformType
        {
            iOS = 1,    // Apple
            Android = 2,    // Google
        }
        
        /// <summary>
        /// 購入失敗時の定数
        /// </summary>
        public enum BuyFailureReason
        {
            /// <summary>
            /// エラー無し
            /// </summary>
            None,
            /// <summary>
            /// 課金システムが初期化されていない
            /// </summary>
            NotInitialization,
            /// <summary>
            /// 課金システムが初期化中
            /// </summary>
            Initializing,
            /// <summary>
            /// 販売されていないアイテムを指定した
            /// </summary>
            UnknownItem,
            /// <summary>
            /// 課金メッセージを受け取れない場合
            /// </summary>
            NotReceiveMessage,
            /// <summary>
            /// 通信不可状態（課金システムの初期化は完了）
            /// </summary>
            NetworkUnavailable,
            /// <summary>
            /// 非サポート（リストアの場合）
            /// </summary>
            NotSupported,
            /// <summary>
            /// 不明なエラー
            /// </summary>
            Unknown
        }

        /// <summary>
        /// 課金アイテムの設定
        /// </summary>
        public class PurchaseProduct
        {
            /// <summary>
            ///  Unity上で扱うアイテムID
            /// </summary>
            public string UnityProductId;

            /// <summary>
            /// UnityEditor上で扱うアイテムID
            /// </summary>
            public string UnityEditorName;

            /// <summary>
            /// Appleストアで扱うアイテムID
            /// </summary>
            public string AppleName;

            /// <summary>
            /// GooglePlayで扱うアイテムID
            /// </summary>
            public string GooglePlayName;
        }
        
        #region Unity Action
        /// <summary>
        /// 課金システムの初期化を行います。
        /// Step 1. UnityIAPの初期化 InitializePurchasing
        /// </summary>
        public void InitializeIapController(InitializeSuccessEvent successResult, InitializeFailureEvent failureEvent)
        {
            Logger.Log("InitializeIapController called");
            // 既にIAPが初期化されている場合は何もしない
            if (IsInitialized())
            {
                Logger.Log("IsInitialized already so finish process");
                successResult?.Invoke(null);
                return;
            }

            ClearCallBacks();
            OnInitializeSuccess = successResult;
            OnInitializeFailure = failureEvent;
            OnPurchaseFailureResultAction = null;
            OnPurchaseSuccessResultAction = null;

            _iStoreController = null;
            _iExtensionProvider = null;
            
            m_pendingProducts = new Dictionary<string, Product>();
            
            AbstractPurchasingModule module = StandardPurchasingModule.Instance();
    #if !PJFB_REL
            ((StandardPurchasingModule)module).useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
            ((StandardPurchasingModule)module).useFakeStoreAlways = true;
    #endif

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

            var platformType = (int)GetStoreType();
            var purchaseModels = MasterManager.Instance.billingRewardMaster.values
                .Where(reward => reward.storeType == platformType);

            foreach (var obj in purchaseModels)
            {
                PurchaseProduct product = new PurchaseProduct();
                product.UnityProductId = obj.appProductId.ToString();
                switch ((PlatformType)obj.storeType) {
                    case PlatformType.iOS:
                        product.AppleName = obj.appleProductKey;
                        break;
                    case PlatformType.Android:
                        product.GooglePlayName = obj.googleProductKey;
                        break;
                    default:
#if UNITY_IOS
                        product.UnityEditorName = obj.appleProductKey;
#else
                        product.UnityEditorName = obj.googleProductKey;
#endif
                        break;
                }
                ConsumableProducts.Add(product);
            }
            
    #if !PJFB_REL && UNITY_EDITOR
            var logStr = "Available products. \n";
            var isAndroid = Application.platform == RuntimePlatform.Android; 
            ConsumableProducts.ForEach(product =>
            {
                logStr += $"{product.UnityProductId}:";
                logStr += (isAndroid ? product.GooglePlayName : product.AppleName) + "\n";
            });
            Logger.Log(logStr);
    #endif
            
            //// 消費型アイテム
            AddProduct(builder, ProductType.Consumable, ConsumableProducts);
            // 非同期の課金処理の初期化を開始
            UnityPurchasing.Initialize(this, builder);
        }

        /// <summary>
        /// アイテムの購入を行います。
        /// Step 2. 購入処理 BuyProductID
        /// </summary>
        /// <returns>BuyFailureReason(NotSupported以外全て)</returns>
        /// <param name="productId">アイテムID</param>
        /// <param name="successResult">Success result.</param>
        /// <param name="failureEvent">Failure event.</param>
        public void PurchaseConsumable(string productId, Action<Product> successResult, Action<Product,PurchaseFailureReason> failureEvent)
        {
            if (OnPurchaseFailureResultAction != null ||
                OnPurchaseSuccessResultAction != null)
            {
                // 購入が既に走っている際のFailSafe、進行中の購入でcallbackがかかるためここではreturnするのみ
                return;
            }
            
            Logger.Log("PurchaseConsumable called");
            Product product = _iStoreController.products.WithID(productId);
            OnPurchaseSuccessResultAction = successResult;
            OnPurchaseFailureResultAction = failureEvent;

            if (!IsInitialized())
            {
                Logger.Log("Not Initialized and cancel the process");
                OnPurchaseFailureResult(product, PurchaseFailureReason.Unknown);
                ClearCallBacks();
                return;
            }

            // コールバックが通知できない場合は何もしない
            if (successResult == null || failureEvent == null)
            {
                Logger.Log("successResult or failureEvent callback nulled");
                OnPurchaseFailureResult(product, PurchaseFailureReason.Unknown);
                ClearCallBacks();
                return;
            }
            if (!IsAvailableProduct(product))
            {
                Logger.Log("available Product not found " + product.ToString());
                OnPurchaseFailureResult(product, PurchaseFailureReason.ProductUnavailable);
                ClearCallBacks();
                return;
            }
            try
            {
                // 以降はエラーが無いのでコールバック設定
                _iStoreController.InitiatePurchase(product);
            }
            catch (Exception e)
            {
                Logger.Log("exception occured  " + e.StackTrace);
                OnPurchaseFailureResult(product, PurchaseFailureReason.ProductUnavailable);
                ClearCallBacks();
            }
        }
        
        /// <summary>
        /// IStoreControllerのConfirmPendingPurchaseを実行します
        /// Step 3. レシートの検証が完了 ConfirmPendingPurchase
        /// </summary>
        /// <returns>trueの場合は消費実行</returns>
        /// <param name="product">Product.</param>
        public bool ConfirmPendingPurchase(Product product)
        {
            if (!IsInitialized())
            {
                return false;
            }
            
            // 完了の通知とPendingアイテム情報の更新
            //たとえUpdatePendingProductで消費できなくとも、初期化後にまたアイテム情報がやってくる
            _iStoreController.ConfirmPendingPurchase(product);
            RemovePendingProduct(product);
            return true;
        }

        /// <summary>
        /// 課金完了通知イベント
        /// </summary>
        private void OnPurchaseSuccessResult(Product product)
        {
            OnPurchaseSuccessResultAction?.Invoke(product);
            OnPurchaseSuccessResultAction = null;
        }

        /// <summary>
        /// 課金失敗通知イベント
        /// </summary>
        private void OnPurchaseFailureResult(Product product, PurchaseFailureReason result)
        {
            OnPurchaseFailureResultAction?.Invoke(product,result);
            OnPurchaseFailureResultAction = null;
        }

        #endregion UnityAction
        
        #region IStoreListener
        /// <summary>
        /// UnityPurchasing.Initializeのhook関数
        /// IStoreListenerの初期化完了成功通知
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="extensions">Extensions.</param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Logger.Log("--- OnInitialized called ");
            _iStoreController = controller;
            _iExtensionProvider = extensions;

            if (OnInitializeSuccess != null)
            {
                var availableProducts = _iStoreController.products.all.Where(IsAvailableProduct);
                OnInitializeSuccess(availableProducts.ToArray());
            }
            ClearCallBacks();
        }
        
        /// <summary>
        /// UnityPurchasing.Initializeのhook関数
        /// IStoreListenerの初期化完了失敗通知
        /// </summary>
        /// <param name="error">Error.</param>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, String.Empty);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Logger.Log("Billing failed to initialize!");
            Logger.Log("Message : " + message);

            switch (error)
            {
                case InitializationFailureReason.AppNotKnown:
                {
                    Logger.LogError("Is your App correctly uploaded on the relevant publisher console?");
                    break;
                }
                case InitializationFailureReason.PurchasingUnavailable:
                {
                    // Ask the user if billing is disabled in device settings.
                    Logger.LogError("Billing disabled!");
                    break;
                }
                case InitializationFailureReason.NoProductsAvailable:
                {
                    // Developer configuration error; check product metadata.
                    Logger.LogError("No products available for purchase!");
                }
                    break;
            }
            var args = new Dictionary<string, object>() { { "error", error }, };

            OnInitializeFailure?.Invoke(error);
            ClearCallBacks();
        }

        /// <summary>
        /// iStoreController.InitiatePurchaseのhook関数
        /// IStoreListenerのアプリ内課金成功の通知です。
        /// 初期化時、未処理のトランザクションの場合もこちらから流れてくるので実装注意
        /// </summary>
        /// <returns>The purchase.</returns>
        /// <param name="args">E.</param>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Logger.Log("ProcessPurchase is hooked ");
            string unityProductId = args.purchasedProduct.definition.id;
            Product product = args.purchasedProduct;

            // このコールバックに入ってくるProductを全てPendingとして登録
            AddPendingProduct(product);
            
            // コールバックが通知できない場合はここで処理を終える
            // 
            // 未処理のトランザクションはこちらのhook関数で処理せず、webviewから明示的にGameManager.OnCheckPendingPurchase
            // を呼ばれることによって実行するためこちらでは対応せずPendingのままとする
            if (OnPurchaseSuccessResultAction == null || OnPurchaseFailureResultAction == null)
            {
                Logger.Log("OnPurchaseSuccessResult or OnPurchaseFailureResult are not found so cancel the process noop ");
                return PurchaseProcessingResult.Pending;
            }

            // 初期化時に登録されていないアイテムの場合（アプリの不具合・サーバの設定ミス等）
            if (unityProductId == null)
            {
                Logger.Log("unityProductId are not found so cancel the process noop ");
                OnPurchaseFailureResult(product, PurchaseFailureReason.ProductUnavailable);
                ClearCallBacks();
                return PurchaseProcessingResult.Pending;
            }

            try
            {
                if (ValidateDeferredPaymentProduct(product))
                {
                    throw new Exception();
                }
                // アイテムの購入完了処理
                // c->過去に購入した現在は販売していないアイテムが未消費の可能性があるため
                OnPurchaseSuccessResult(product);
            }
            catch (Exception)
            {
                // 不明なエラーが発生（成功のコールバックで強制終了している場合もここで通知されるので、レシートの有無で判断する）
                OnPurchaseFailureResult(product, PurchaseFailureReason.Unknown);
            }

            ClearCallBacks();
            return PurchaseProcessingResult.Pending;
        }
        
        /// <summary>
        /// A purchase failed with a detailed Failure Description.
        /// PurchaseFailureDescription contains : productId, PurchaseFailureReason and an error message
        /// </summary>
        /// <param name="product"> The product that was attempted to be purchased. </param>
        /// <param name="failureDescription"> The Purchase Failure Description. </param>
        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            OnPurchaseFailed(product, failureDescription.reason);
        }

        /// <summary>
        /// iStoreController.InitiatePurchaseのhook関数
        /// IStoreListenerアプリ内課金失敗通知です。
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="error">Error.</param>
        public void OnPurchaseFailed(Product item, PurchaseFailureReason error)
        {
            OnPurchaseFailureResult(item, error);

            ClearCallBacks();
            var errorMessage = "";
            switch (error)
            {
                case PurchaseFailureReason.Unknown:
                {
                    errorMessage = "OnPurchaseFailed: Unknown error";
                    Logger.LogError(errorMessage);
                    break;
                }

                case PurchaseFailureReason.ProductUnavailable:
                {
                    errorMessage = "OnPurchaseFailed: Product unavailable error";
                    break;
                }

                case PurchaseFailureReason.SignatureInvalid:
                {
                    errorMessage = "OnPurchaseFailed: Signature invalid error";
                    break;
                }

                case PurchaseFailureReason.UserCancelled:
                {
                    errorMessage = "OnPurchaseFailed: User cancelled error";
                    break;
                }

                case PurchaseFailureReason.PaymentDeclined:
                {
                    errorMessage = "OnPurchaseFailed: Product unavailable error";
                    break;
                }

                case PurchaseFailureReason.DuplicateTransaction:
                {
                    errorMessage = "OnPurchaseFailed: Product unavailable error";
                    break;
                }
            }

            Logger.LogError(errorMessage);
        }

        #endregion EventListener

        #region Member Behaviour
        /// <summary>
        /// Gets the type of the store.
        /// </summary>
        /// <returns>The store type.</returns>
        public PlatformType GetStoreType()
        {
    #if UNITY_EDITOR && UNITY_IOS
            return PlatformType.iOS;
    #elif UNITY_EDITOR && UNITY_ANDROID
            return PlatformType.Android;
    #else
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    return PlatformType.iOS;
                default:
                    return PlatformType.Android;
            }
    #endif
        }

        /// <summary>
        /// [iOS] アプリ内課金がデバイスの設定で制限されているか、iOS以外はtrueを返す
        /// </summary>
        /// <value>iOS以外はtrue</value>
        public bool CanMakePayments
        {
            get
            {
    #if UNITY_IOS
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                return builder.Configure<IAppleConfiguration>().canMakePayments;
    #else
                return true;
    #endif
            }
        }

        // <summary>
        /// InitializePurchasingで初期化を行った際に、前回の購入が中断されている場合は購入処理を続行する
        /// </summary>
        /// <returns>The pending products.</returns>
        public Product GetFirstPendingProduct()
        {
            if (!IsInitialized())
            {
                return null;
            }

            if (m_pendingProducts?.Values.Count == 0)
            {
                return null;
            }

            return m_pendingProducts.Values.FirstOrDefault();
        }

        public bool IsAvailableProduct(string productId)
        {
            if (!IsInitialized())
                return false;

            Product product = _iStoreController.products.WithID(productId);
            return IsAvailableProduct(product);
        }
        
        private void ClearCallBacks()
        {
            OnInitializeSuccess = null;
            OnInitializeFailure = null;
            OnPurchaseFailureResultAction = null;
            OnPurchaseSuccessResultAction = null;
        }

        /// <summary>
        /// UnityIAPの初期化を行っているか返します
        /// </summary>
        /// <returns><c>true</c> なら初期化済み、<c>false</c>なら未初期化</returns>
        public bool IsInitialized()
        {
            return _iStoreController != null && _iExtensionProvider != null;
        }

        private void AddPendingProduct(Product product)
        {
            DebugLogProduct(product, "AddPendingProduct");
            if (!product.hasReceipt)
            {
                return;
            }

            // 扱う商品が全てConsumableなのでStoreIdでのユニークチェックとする
            // SubscriptionとかNon-Consumableな商品が出てきたらそれらの状態を考慮する必要アリ
            var specificId = product.definition.storeSpecificId;
            if (m_pendingProducts.ContainsKey(specificId))
            {
                m_pendingProducts.Remove(specificId);
            }
            
            m_pendingProducts.Add(specificId, product);
        }
        
        private void RemovePendingProduct(Product product)
        {
            DebugLogProduct(product, "RemovePendingProduct");
            var specificId = product.definition.storeSpecificId;
            if (m_pendingProducts.ContainsKey(specificId))
            {
                m_pendingProducts.Remove(specificId);
            }

            AdjustManager.TrackPurchaseEvent(product.definition.storeSpecificId);
        }
        
        private bool IsAvailableProduct(Product product)
        {
            return IsInitialized() && product != null && product.availableToPurchase;
        }

        /// <summary>
        /// ProductTypeごとにアイテムを追加します。
        /// </summary>
        /// <param name="builder">ConfigurationBuilder.</param>
        /// <param name="productType">ProductType.</param>
        /// <param name="products">PurchaseProductの配列.</param>
        private void AddProduct(ConfigurationBuilder builder,
            ProductType productType,
            List<PurchaseProduct> products
        )
        {
            if (products == null)
            {
                return;
            }

            int length = products.Count;
            if (length == 0)
            {
                return;
            }

            // アイテムの追加
            for (int i = 0; i < length; i++)
            {
                IDs ids = new IDs();
                PurchaseProduct product = products[i];
                // UnityEditor
                if (!String.IsNullOrEmpty(product.UnityProductId))
                {
                    ids.Add(product.UnityProductId, UnityEditorStore);
                }
                // AppleAppStore
                if (!String.IsNullOrEmpty(product.AppleName))
                {
                    ids.Add(product.AppleName, AppleAppStore.Name);
                }
                // GooglePlay
                if (!String.IsNullOrEmpty(product.GooglePlayName))
                {
                    ids.Add(product.GooglePlayName, GooglePlay.Name);
                }

                builder.AddProduct(product.UnityProductId, productType, ids);
            }
        }

        public static void DebugLogProduct(Product product, string label = "")
        {
            Logger.Log($"PurchaseDebug {label}:\nStoreSpecificId: {product.definition.storeSpecificId}\nTransactionId: {product.transactionID}\nReceipt: {product.receipt}");
        }

        public bool ValidatePurchase(Product product)
        {
    #if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            return true;
    #else
            var isValid = true;
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(product.receipt);

                foreach (IPurchaseReceipt productReceipt in result)
                {
                    DebugLogProduct(product, $"ProductId: {productReceipt.productID}, PurchaseDate: {productReceipt.purchaseDate}, TransactionId: {productReceipt.transactionID}");
#if UNITY_ANDROID
                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (google != null)
                    {
                        // ここに Google のオーダー ID
                        // sandbox でテストする場合は null にするように注意
                        // なぜなら、Google の sandbox はオーダー IDを発行しないため
                        DebugLogProduct(product, $"PurchaseState: {google.purchaseState}, PurchaseToken: {google.purchaseToken}");
                        if (google.purchaseState != GooglePurchaseState.Purchased)
                        {
                            // コンビニ決済(後払い)の場合、ここでPurchasedになった場合にショップ遷移時に
                            DebugLogProduct(product, "This product is not purchased.");
                            isValid = false;
                        }
                    }
#endif
#if UNITY_IOS
                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (apple != null)
                    {
                        Logger.Log(apple.originalTransactionIdentifier);
                        Logger.Log(apple.subscriptionExpirationDate);
                        Logger.Log(apple.cancellationDate);
                        Logger.Log(apple.quantity);
                    }
#endif
                }

                // For informational purposes, we list the receipt(s)
                DebugLogProduct(product, "Receipt is valid. Contents:");
            } catch (IAPSecurityException e) {
                DebugLogProduct(product, $"Invalid receipt, not unlocking content {e.Message}");
                isValid = false;
            }

            return isValid;
    #endif
        }

        public bool ValidateDeferredPaymentProduct()
        {
#if UNITY_ANDROID            
　　　　　　　// Pendingの全件を確認してスロー決済中のproductがないかチェック
            var pendingProducts = m_pendingProducts.Values;
            return pendingProducts.Any(ValidateDeferredPaymentProduct);
#else
            return false;
#endif
        }

        public bool ValidateDeferredPaymentProduct(Product product)
        {
#if UNITY_ANDROID
            // Android コンビニ決済対応 
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(product.receipt);
                foreach (IPurchaseReceipt productReceipt in result)
                {

                    DebugLogProduct(product, $"ProductId: {productReceipt.productID}, PurchaseDate: {productReceipt.purchaseDate}, TransactionId: {productReceipt.transactionID}");
                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (google != null)
                    {
                        DebugLogProduct(product,$"PurchaseState: {google.purchaseState}, PurchaseToken: {google.purchaseToken}");
                        var state = (int)google.purchaseState;
                        if (state == 4)
                        {
                            DebugLogProduct(product, "This product is deferred payment.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CruFramework.Logger.LogError($"Validate Deferred Payment Error!! {e.Message}");
            }
#endif
            return false;
        }

        /// <summary>
        /// レシートの更新
        /// </summary>
        public void RefreshAppReceipt(Action<string> OnRefreshSuccess, Action OnRefreshFailure = null)
        {
#if UNITY_IOS
            _iExtensionProvider.GetExtension<IAppleExtensions>().RefreshAppReceipt(receipt =>
            {
                Logger.Log($"OnRefreshSuccess : {receipt}");
                OnRefreshSuccess?.Invoke(receipt);
            },
            () => 
            {
                CruFramework.Logger.LogError("OnRefreshFailure");
                OnRefreshFailure?.Invoke();
            });
#else
            OnRefreshFailure?.Invoke();
#endif
        }
        #endregion
    }
}