using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Pjfb
{
    public class UIFooterButton : MonoBehaviour
    {
        public UIBadgeBalloon BadgeBalloon => badgeBalloon;
        
        [SerializeField]
        private GameObject onImage = null;
        
        [SerializeField]
        private GameObject offImage = null;
        
        [SerializeField]
        private UIBadgeNotification badgeNotification = null;
        
        [SerializeField]
        private UIBadgeBalloon badgeBalloon = null;
        
        public void SetActive(bool value)
        {
            onImage.SetActive(value);
            offImage.SetActive(!value);
        }

        public void SetNotificationBadge(bool isActive)
        {
            badgeNotification.SetActive(isActive);
        }
        
        public void SetBalloonBadge(string text)
        {
            if(string.IsNullOrEmpty(text))
            {
                badgeBalloon.SetActive(false);
            }
            else
            {
                badgeBalloon.SetActive(true);
                badgeBalloon.SetText(text);
            }
        }

        public void SetBalloonBadge(string text1, string text2)
        {
            if(string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2))
            {
                badgeBalloon.SetActive(false);
            }
            else if( string.IsNullOrEmpty(text2) )
            {
                SetBalloonBadge(text1);
            }
            else if( string.IsNullOrEmpty(text1) )
            {
                SetBalloonBadge(text2);
            }
            else
            {
                badgeBalloon.SetActive(true);
                badgeBalloon.SetTextToCrossFade(text1, text2);
            }
        }
    }
}
