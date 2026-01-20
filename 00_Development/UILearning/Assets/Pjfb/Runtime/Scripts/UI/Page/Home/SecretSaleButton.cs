using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class SecretSaleButton : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI badgeText;
        
        [SerializeField]
        private GameObject badgeRoot;
        
        [SerializeField]
        private TextMeshProUGUI timeText;
        
        [SerializeField]
        private Animator buttonAnimator;
        
        [SerializeField]
        private UIButton button;
        
        private DateTime currentSaleEndTime = DateTime.MinValue;
        private int currentSaleIndex = 0;
        // 更新用タイマー(最初に表示ない状態がちらつくので、UpdateIntervalに初期化しておく)
        private float timeSinceLastUpdate = UpdateInterval;
        private const int MaxBadgeCount = 9;
        private const float UpdateInterval = 0.5f;
        private string saleCountdownFormat = null;
        
        
        // アニメーションのトリガー
        private static readonly string CloseAnimationName = "Close";
        
        private NativeApiSaleIntroduction[] saleIntroductionsCache = null;

        public void SetSecretButton()
        {
            button.interactable = true;
            saleCountdownFormat = StringValueAssetLoader.Instance["common.timespan_format_1"];
            UpdateSecretButton();
        }
        
        private void UpdateSecretButton()
        {
            saleIntroductionsCache = ShopManager.GetSaleIntroductionsOrderPriority();
            //期間内のセールがあるか
            bool isExistSale = false;
            // 期間内のセールを取得
            for (int i = 0; i < saleIntroductionsCache.Length; i++)
            {
                DateTime saleExpireTime = ShopManager.GetDateTimeByString(saleIntroductionsCache[i].expireAt); 
                TimeSpan saleEndTime = saleExpireTime - AppTime.Now;
                if(saleEndTime.TotalSeconds > 0)
                {
                    currentSaleIndex = i;
                    currentSaleEndTime = saleExpireTime;
                    isExistSale = true;
                    break;
                }
            }
            
            // 表示するセールが無ければアニメーションして閉じる
            if (isExistSale == false)
            {
                saleIntroductionsCache = null;
                currentSaleIndex = 0;
                button.interactable = false;
                PlayCloseAnimationAsync().Forget();
                return;
            }
            
            // バッジ表示
            // 有効なセール数
            int validSaleCount = saleIntroductionsCache.Length - currentSaleIndex;

            // セールが複数有効な場合のみバッジ表示する
            if (validSaleCount > 1)
            {
                if (validSaleCount > MaxBadgeCount)
                {
                    // 発生しているセール発生数が基準値以上の場合は+〇表記
                    badgeText.text = String.Format(StringValueAssetLoader.Instance["common.above_limit"], MaxBadgeCount);
                }
                else
                {
                    // 発生しているセール数をバッジ表示
                    badgeText.text = validSaleCount.ToString();
                }
                badgeRoot.SetActive(true);
            }
            else
            {
                // バッジテキスト非表示
                badgeRoot.SetActive(false);
            }
        }

        private void Update()
        {
            if(saleIntroductionsCache == null) return;
            // 一定時間ごとに更新
            timeSinceLastUpdate += Time.deltaTime;;
            if (timeSinceLastUpdate < UpdateInterval) return;
            
            // 残り時間の更新
            TimeSpan saleEndTime = currentSaleEndTime - AppTime.Now;
            // 開催中のセール数が変化しているまたは現在のセールが終了している場合は情報を更新する
            if (saleIntroductionsCache.Length == ShopManager.GetSaleIntroductionsOrderPriority().Length && saleEndTime.TotalSeconds > 0)
            {
                timeText.gameObject.SetActive(true);
                timeText.text = saleEndTime.ToString(saleCountdownFormat);
            }
            else
            {
                // 時間表示のオブジェクトを一度非表示に
                timeText.gameObject.SetActive(false);
                UpdateSecretButton();
            }
            timeSinceLastUpdate = 0f;
        }

        // ボタンが消える演出の再生
        private async UniTask PlayCloseAnimationAsync()
        {
            badgeRoot.SetActive(false);
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            await AnimatorUtility.WaitStateAsync(buttonAnimator, CloseAnimationName, token);
            gameObject.SetActive(false);
        }

        public void OnClickButton()
        {
            // 発生している全てのシークレットセールを表示
            ShopManager.ShowAllSaleIntroduction(onClose: SetSecretButton);
        }
    }
}