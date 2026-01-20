using System;
using UnityEngine;

namespace Pjfb.SubscribeInfo
{
    public class SubscribeInfoExpireDateView : MonoBehaviour
    {
        #region Params
        public class ViewParams
        {
            public DateTime expireDateTime;

            public ViewParams(DateTime expireDateTime)
            {
                this.expireDateTime = expireDateTime;
            }
        }
        #endregion

        #region SerializeField
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMPro.TMP_Text expireDateText;
        #endregion

        #region PrivateFields
        private ViewParams viewParams;
        #endregion

        #region PublicMethods
        public void SetDisplay(ViewParams viewParams)
        {
            expireDateText.text = viewParams.expireDateTime.ToString(StringValueAssetLoader.Instance["subscribe.remain_time_format"]);
            SetActive(true);
        }

        public float GetPreferredHeight(ViewParams viewParams)
        {
            return rectTransform.rect.height;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        #endregion
    }
}
