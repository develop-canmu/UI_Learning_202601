using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.UI;
using UnityEngine;
using TMPro;

namespace Pjfb.LockedItem
{
    public class HomeLockedItemButton : MonoBehaviour
    {
        #region Params
        public class Parameters
        {
            public long unreceivedGiftLockedCount;
            public long unopenedGiftBoxCount;
            public DateTime newestGiftLockedAt;
            public Action<Parameters> onClickButton;

            public long totalGiftBoxCount => unopenedGiftBoxCount + unreceivedGiftLockedCount;
            
            public Parameters(long unreceivedGiftLockedCount, long unopenedGiftBoxCount, string newestGiftLockedAt, Action<Parameters> onClickButton)
            {
                this.unreceivedGiftLockedCount = unreceivedGiftLockedCount;
                this.unopenedGiftBoxCount = unopenedGiftBoxCount;
                this.newestGiftLockedAt = newestGiftLockedAt.TryConvertToDateTime();
                this.onClickButton = onClickButton;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private GameObject unlockedIconGameObject;
        [SerializeField] private GameObject badgeGameObject;
        [SerializeField] private AnimatorController animatorController;
        #endregion

        #region PublicFields
        public bool isPreparingForAnimation;
        #endregion
        
        #region PrivateFields
        private Parameters _parameters;
        private int _lastUpdateSecond = -1;
        private string RemainingTextFormat => StringValueAssetLoader.Instance["locked_item.home.remain_format"];
        private string CanReceiveText => StringValueAssetLoader.Instance["locked_item.home.can_receive"];
        #endregion

        #region PublicMethods
        public void Init(Parameters parameters)
        {
            var isNewestGiftUnlockedAtFuture = parameters.newestGiftLockedAt.IsFuture(AppTime.Now);
            var hasUnreceivedGift = parameters.unreceivedGiftLockedCount > 0;
            var showButton = isNewestGiftUnlockedAtFuture || hasUnreceivedGift;
            isPreparingForAnimation = showButton && !LockedItemManager.LockedItemLastShownFlagPlayerPrefs;
            
            _parameters = showButton ? parameters : null;
            _lastUpdateSecond = -1;
            unlockedIconGameObject.SetActive(!isNewestGiftUnlockedAtFuture || hasUnreceivedGift);

            if (showButton && !isPreparingForAnimation) {
                SetActive(true);
                LockedItemManager.LockedItemLastShownFlagPlayerPrefs = true;
            } else {
                SetActive(false);
                LockedItemManager.LockedItemLastShownFlagPlayerPrefs = false;
            }
        }

        public async UniTask TryShowFlashAnimation(Action onFinish = null)
        {
            if (isPreparingForAnimation)
            {
                isPreparingForAnimation = false;
                await ShowOpenAnimationAsync();
                LockedItemManager.LockedItemLastShownFlagPlayerPrefs = true;
            }
            else if(_parameters != null && LockedItemManager.LockedItemLastItemCountPlayerPrefs < _parameters.totalGiftBoxCount)
            {
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                await animatorController.Play("Flash");
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
            }
            
            if (_parameters != null) LockedItemManager.LockedItemLastItemCountPlayerPrefs = _parameters.totalGiftBoxCount;
            onFinish?.Invoke();
        }
        #endregion

        #region PrivateMethods
        private void UpdateBadge(long unreceivedGiftLockedCount, DateTime openDateTime, DateTime now)
        {
            var hasUnreceivedGift = unreceivedGiftLockedCount > 0;
            buttonText.text = hasUnreceivedGift ? CanReceiveText : openDateTime.GetPreciseRemainingString(now, textFormat: RemainingTextFormat, defaultString: CanReceiveText);
            unlockedIconGameObject.SetActive(hasUnreceivedGift || !openDateTime.IsFuture(now));
            badgeGameObject.SetActive(hasUnreceivedGift);
        }

        private void SetActive(bool isActive) 
        {
            gameObject.SetActive(isActive);
        }
        
        private void Update()
        {
            if (_parameters == null) return;
            
            var now = AppTime.Now;
            if (_lastUpdateSecond == now.Second) return;
            _lastUpdateSecond = now.Second;
            
            UpdateBadge(unreceivedGiftLockedCount: _parameters.unreceivedGiftLockedCount, openDateTime: _parameters.newestGiftLockedAt, now);
        }

        private async UniTask ShowOpenAnimationAsync ()
        {
            SetActive(false);
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            buttonText.text = string.Empty;

            await Task.Delay(millisecondsDelay: 500);
            SetActive(true);
            await animatorController.Play("Open");
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
        }
        #endregion

        #region EventListener
        public void OnClickButton()
        {
            _parameters?.onClickButton?.Invoke(_parameters);
        }
        #endregion
    }
}