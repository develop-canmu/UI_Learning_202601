using System;
using UnityEngine;

namespace Pjfb.Common
{
    public class UIBadgeBalloonButton : MonoBehaviour
    {
        #region Params
        public class ButtonParams
        {
            public string badgeString;
            public bool showBadgeNotification;
            public Action onClick;

            public ButtonParams(string badgeString, bool showBadgeNotification, Action onClick)
            {
                this.badgeString = badgeString;
                this.showBadgeNotification = showBadgeNotification;
                this.onClick = onClick;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private UIBadgeBalloon badgeBalloon;
        [SerializeField] private UIBadgeNotification badgeNotification;
        #endregion

        #region PrivateFields
        private ButtonParams _buttonParams;
        #endregion

        #region PublicMethods
        public void Init(ButtonParams buttonParams)
        {
            _buttonParams = buttonParams;
            UpdateBadge(_buttonParams.badgeString, _buttonParams.showBadgeNotification);
        }

        public void UpdateBadge(string badgeString, bool showBadgeNotification)
        {
            _buttonParams.badgeString = badgeString;
            if (badgeBalloon != null) badgeBalloon.SetText(badgeString);
            if (badgeNotification != null) badgeNotification.SetActive(showBadgeNotification);
            
            ShowBadge(!string.IsNullOrEmpty(badgeString));
        }
        
        public void ShowBadge(bool showBadge)
        {
            if (badgeBalloon != null) badgeBalloon.SetActive(showBadge);
        }
        #endregion

        #region EventListeners
        public void OnClickButton()
        {
            _buttonParams?.onClick?.Invoke();
        }
        #endregion
    }
}