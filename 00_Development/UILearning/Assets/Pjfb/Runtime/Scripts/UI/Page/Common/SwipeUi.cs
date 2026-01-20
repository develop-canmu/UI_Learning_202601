using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pjfb
{
    public class SwipeUi : MonoBehaviour
    {
        // Todo: 挙動を見つつ調整
        [SerializeField] private GameObject arrowRoot;
        private static float SwipeOffset = 100f;

        private float clickPositionX;
        
        public Action OnNextAction { get; set; }
        public Action OnPrevAction { get; set; }

        public bool EnableSwipe
        {
            get => enableSwipe;
            set
            {
                enableSwipe = value;
                arrowRoot.SetActive(enableSwipe);
            }
        }

        private bool enableSwipe;
        
        private void Awake()
        {
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            CruFramework.DebugMenu.AddOption("Swipe Offset", "Offset",
                () => { return SwipeOffset; },
                (value) => { SwipeOffset = value; }
                , 1);
#endif
        }

        public void OnPointerDown(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointData)
            {
                clickPositionX = pointData.position.x;
            }
        }

        public void OnPointerUp(BaseEventData eventData)
        {
            if(!enableSwipe) return;
            if (eventData is not PointerEventData pointData) return;
            var swipeLength = pointData.position.x - clickPositionX;
            if (swipeLength < -SwipeOffset)
            {
                SEManager.PlaySE(SE.se_common_icon_tap);
                OnClickNextButton();
            }
            else if (swipeLength > SwipeOffset)
            {
                SEManager.PlaySE(SE.se_common_icon_tap);
                OnClickPrevButton();
            }
        }

        public void OnClickNextButton()
        {
            if(!enableSwipe) return;
            OnNextAction?.Invoke();
        }

        public void OnClickPrevButton()
        {
            if(!enableSwipe) return;
            OnPrevAction?.Invoke();
        }

        
    }
}