using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.SubscribeInfo
{
    public class SubscribeInfoDescriptionView : MonoBehaviour
    {
        #region Params
        public class ViewParams
        {
            public string title;
            public string description;

            public ViewParams(string title, string description)
            {
                this.title = title;
                this.description = description;
            }
        }
        
        [Serializable] public class ExpireDateData { public DateTime expireDate; }
        [Serializable] public class DescriptionData { public string description; }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private int minimumHeight = 110;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        #endregion

        #region PrivateFields
        private ViewParams viewParams;
        #endregion

        #region PublicMethods
        public void SetDisplay(ViewParams viewParams)
        {
            titleText.text = viewParams.title;
            descriptionText.text = viewParams.description;
            SetActive(true);
        }

        public float GetPreferredHeight(ViewParams viewParams)
        {
            SetDisplay(viewParams);
            return titleText.preferredHeight + descriptionText.preferredHeight + minimumHeight;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void OnEnable()
        {
            // メモ：PoolListItem内にcontentSizeFitterを使うとうまく処理しない場合があるため、OnEnableにcontentSizeFitterを動かす
            contentSizeFitter.SetLayoutVertical();
        }
        #endregion
    }
}
