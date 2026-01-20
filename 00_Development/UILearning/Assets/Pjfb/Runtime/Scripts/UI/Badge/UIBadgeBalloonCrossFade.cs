using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Pjfb
{
    public class UIBadgeBalloonCrossFade : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI firstBalloonText;
        [SerializeField] private TextMeshProUGUI secondBalloonText;
        [SerializeField] private GameObject firstBalloonObject;
        [SerializeField] private GameObject secondBalloonObject;

        public void SetView(bool firstCondition, bool secondCondition)
        {
            var isActive = firstCondition && secondCondition;
            if (isActive)
            {
                gameObject.SetActive(true);
            }
            animator.enabled = isActive;
            firstBalloonObject.SetActive(firstCondition);
            secondBalloonObject.SetActive(secondCondition);
            var firstCanvasGroup = firstBalloonObject.GetComponent<CanvasGroup>();
            var secondCanvasGroup = secondBalloonObject.GetComponent<CanvasGroup>();
            if (firstCanvasGroup != null)
            {
                firstCanvasGroup.alpha = firstCondition ? 1f : 0f;
            }
            if (secondCanvasGroup != null)
            {
                secondCanvasGroup.alpha = secondCondition ? 1f : 0f;
            }
        }
        
        public void SetView(bool firstCondition, bool secondCondition, string firstText, string secondText)
        {
            SetView(firstCondition, secondCondition);
            firstBalloonText.text = firstText;
            secondBalloonText.text = secondText;
        }
    }
}
