using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pjfb.Extensions
{
    public static class UIButtonExtensions
    {
        public static bool TryClickWithRaycast(this UIButton button)
        {
            if (!CanRaycast(button)) return false;
            button.onClick.Invoke();
            return true;
        }

        public static bool CanRaycast(this UIButton button)
        {
            if (button == null) return false;
            var buttonRectTransform = button.GetComponent<RectTransform>();
            if (buttonRectTransform == null) return false;
            
            var resultList = RaycastAll(buttonRectTransform);
            if (resultList.Count <= 0) return false;
            
            var firstInteractableObject = resultList[0];
            return firstInteractableObject.gameObject.transform.IsChildOf(buttonRectTransform);
        }
        
        private static List<RaycastResult> RaycastAll(RectTransform rectTransform)
        {
            var eventSystem = EventSystem.current;
            var pointerPosition = RectTransformUtility.WorldToScreenPoint(AppManager.Instance.UIManager.UICamera, rectTransform.position);
            var pointerEventData = new PointerEventData(eventSystem) {position = pointerPosition};
            var resultList = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, resultList);
            return resultList;
        }
    }
}