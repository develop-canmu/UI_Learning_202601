using System;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class UINotification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private RectTransform rectTransform;

        private Action _onShowComplete;
        
        /// <summary>AnimationClipのTriggerから呼ばれてる</summary>
        public void OnShowComplete()
        {
            gameObject.SetActive(false);
            _onShowComplete?.Invoke();
            _onShowComplete = null;
        }
        
        public void ShowNotification(string body, Action onComplete = null, int yPos = 0)
        {
            contentText.text = body;
            rectTransform.offsetMin = new Vector2(0, yPos);
            rectTransform.offsetMax = new Vector2(0, yPos);
            gameObject.SetActive(true);
            _onShowComplete = onComplete;
        }
    }
}