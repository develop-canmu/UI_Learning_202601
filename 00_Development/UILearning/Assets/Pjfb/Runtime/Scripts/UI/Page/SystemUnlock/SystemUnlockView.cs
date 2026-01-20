using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UniRx;
using UnityEngine.UI;

namespace Pjfb.SystemUnlock
{
    public class SystemUnlockView : MonoBehaviour
    {
        public class Param
        {
            public long systemNumber;
            public SystemLockMasterObject systemLockMasterObject;
        }
        
        [SerializeField] private Transform focusObjectRoot;
        [SerializeField] private GameObject touchGuard;
        [SerializeField] private Transform tapEffectRoot;
        [SerializeField] private HomeContentUnlockedAnimationView homeContentUnlockedAnimationView;
        [SerializeField] private GameObject tapEffectPrefab;
        // ボタン誘導のないシステム
        [SerializeField] private List<SystemUnlockDataManager.SystemUnlockNumber> noGuideSystemUnlock;
        public List<SystemUnlockDataManager.SystemUnlockNumber> NoGuideSystemUnlock => noGuideSystemUnlock;
        private Param unlockInfo;
        private GameObject tapEffect;
        private GameObject copyObject;
        private CancellationToken token;
        private GameObject focusObject;
        
        public void Open(Param param)
        {
            unlockInfo = param;
            var labelText = string.Format(StringValueAssetLoader.Instance["tutorial.system.unlock"], unlockInfo.systemLockMasterObject.name);
            homeContentUnlockedAnimationView.SetTextLabel(labelText);
        }

        public void SetActiveTouchGuard(bool isActive)
        {
            touchGuard.SetActive(isActive);
        }

        public void EndEffectAndDestroy()
        {
            Destroy(gameObject);
        }

        public async UniTask WaitInputTapEffectAsync(SystemUnlockReference reference)
        {
            // 機能の仮解放
            UnlockSystem(reference);

            // スクロールするなら
            if (reference.IsScroll)
            {
                var scrollRect = reference.GetComponentInParent<ScrollRect>();
                if (scrollRect != null)
                {
                    RectTransform referenceTransform = (RectTransform) reference.transform;
                    
                    // ハイライトするオブジェクトのローカルy座標(pivotの設定を加味して高さを加算 *上がpivot1なので1から引く)
                    var targetPos = Mathf.Abs(referenceTransform.anchoredPosition.y) + referenceTransform.rect.height * (1.0f - referenceTransform.pivot.y);
                    // 描画領域からどのくらいハイライトオブジェクトがはみ出ているか
                    var diff = targetPos - scrollRect.viewport.rect.height;

                    // 描画領域からはみ出ている領域の高さ
                    var overAreaHeight = scrollRect.content.rect.height - scrollRect.viewport.rect.height;
                  
                    // ScrollRectの描画領域よりもContextの領域が下ならスクロール
                    if (overAreaHeight > 0)
                    {
                        // スクロールしていない状態が1なので1から引く
                        // ハイライトするオブジェクトの描画外領域に対してどの程度の割合の位置にあるか計算
                        scrollRect.verticalNormalizedPosition = 1.0f - diff/overAreaHeight;
                    } 
                }
            }
            
            focusObject = reference.UnlockButton;
            copyObject = Instantiate(focusObject, focusObjectRoot);
            copyObject.transform.position = focusObject.transform.position;
            var copyButton = copyObject.GetComponentInChildren<UIButton>();
            copyButton.ResetClickEvents();
            copyButton.ResetLongTapEvents();
            var focusUiButton = focusObject.GetComponentInChildren<UIButton>();

            SetActivePrefabRootObject(focusObject, false);
            // Pivot(0.5, 0.5)の時の中心座標取得
            RectTransform rectTransform = (RectTransform)copyObject.transform;
            Vector3 centerPosition = GetCenterPosition(rectTransform);
            UpdateTapEffect(centerPosition);
            
            copyButton.onClick.AddListener(() =>
            {
                SetActivePrefabRootObject(focusObject, true);
                // 非永続的な入力イベントのみだとSEならないのでこちらで再生
                if (focusUiButton.OnLongTap.GetPersistentEventCount() <= 0)
                {
                    SEManager.PlaySE(focusUiButton.SoundType);
                }
                focusUiButton.onClick.Invoke();
                copyObject.SetActive(false);
                tapEffect.SetActive(false);
                SetActiveTouchGuard(false);
            });

            // ボタンクリックを待機
            await copyButton.OnClickAsObservable().First();
        }

        public async UniTask StartEffect()
        {
            // 解放演出
            homeContentUnlockedAnimationView.gameObject.SetActive(true);
        
            // アニメーション待機。オーバーレイはアニメーションと重なるため再生中は非表示
            SetActiveTouchGuard(false);
            await homeContentUnlockedAnimationView.PlayAnimationAsync(token);
            SetActiveTouchGuard(true);
        }

        /// <summary>
        /// ボタンの復元。キャンセルされた時用。
        /// </summary>
        public void RestoreButtonWhenCancel()
        {
            if (focusObject != null)
            {
                SetActivePrefabRootObject(focusObject, true);
            }
        }

        private void InitializeUi()
        {
            tapEffect = Instantiate(tapEffectPrefab, tapEffectRoot);
            tapEffect.SetActive(false);
            SetActiveTouchGuard(false);

            homeContentUnlockedAnimationView.gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            InitializeUi();

            token = this.GetCancellationTokenOnDestroy();
        }

        private void SetActivePrefabRootObject(GameObject obj, bool state)
        {
            // ボタンを変更するとレイアウトに影響するため、直下のRootを調整
            obj.transform.GetChild(0).gameObject.SetActive(state);
        }

        private void UpdateTapEffect(Vector2 position)
        {
            if (tapEffect != null)
            {
                tapEffect.transform.position = position;
                tapEffect.SetActive(true);
            }
        }
        
        private void UnlockSystem(SystemUnlockReference reference)
        {
            // 表示上ロックされている場合は機能の仮解放
            if (reference.LockObject != null)
            {
                reference.LockObject.SetActive(false);    
            }
        }
        
        private Vector3 GetCenterPosition(RectTransform rectTransform)
        {
            Vector3 position = rectTransform.position;
            Vector3 diff = new Vector3(
                Mathf.Lerp(-rectTransform.rect.size.x / 2f, rectTransform.rect.size.x / 2f, rectTransform.pivot.x) * rectTransform.transform.lossyScale.x,
                Mathf.Lerp(-rectTransform.rect.size.y / 2f, rectTransform.rect.size.y / 2f, rectTransform.pivot.y) * rectTransform.transform.lossyScale.y
            );
            return position - diff;
        }
    }
}