using System;
using TMPro;
using UnityEngine;

namespace Pjfb.Common
{
    public class CommonBadgeButton : MonoBehaviour
    {
        #region Params
        public class ButtonParams
        {
            public string badgeString;
            public Action onClick;

            public ButtonParams(string badgeString, Action onClick)
            {
                this.badgeString = badgeString;
                this.onClick = onClick;
            }
            
            public ButtonParams(bool showBadge, Action onClick)
            {
                this.badgeString = GetShowBadgeString(showBadge);
                this.onClick = onClick;
            }

            public static string GetShowBadgeString(bool showBadge)
            {
                return showBadge ? " " : string.Empty;
            }
        }
        #endregion
        
        #region SerializeFields
        // TODO: badgeTextが伸びるほど背景も伸びるように実装する
        [SerializeField] private TextMeshProUGUI badgeText;
        [SerializeField] private GameObject badgeParent;
        #endregion

        #region PrivateFields
        private ButtonParams _buttonParams;
        #endregion

        #region PublicMethods
        public void Init(ButtonParams buttonParams)
        {
            _buttonParams = buttonParams;
            UpdateBadge(_buttonParams.badgeString);
        }

        public void UpdateBadge(string badgeString)
        {
            _buttonParams.badgeString = badgeString;
            badgeText.text = badgeString;
            badgeParent.SetActive(!string.IsNullOrEmpty(badgeString));
        }
        
        public void ShowBadge(bool showBadge)
        {
            UpdateBadge(ButtonParams.GetShowBadgeString(showBadge));
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