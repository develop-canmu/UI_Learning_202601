using System;
using UnityEngine;

namespace Pjfb.Common
{
    public class UIBadgeNotificationButton : MonoBehaviour
    {
        #region Params
        public class ButtonParams
        {
            public bool showBadge;
            public Action onClick;

            public ButtonParams(bool showBadge, Action onClick)
            {
                this.showBadge = showBadge;
                this.onClick = onClick;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private UIBadgeNotification badgeNotification;
        #endregion

        #region PrivateFields
        private ButtonParams _buttonParams;
        #endregion

        #region PublicMethods
        public void Init(ButtonParams buttonParams)
        {
            _buttonParams = buttonParams;
            ShowBadge(_buttonParams.showBadge);
        }

        public void ShowBadge(bool showBadge)
        {
            badgeNotification.SetActive(showBadge);
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