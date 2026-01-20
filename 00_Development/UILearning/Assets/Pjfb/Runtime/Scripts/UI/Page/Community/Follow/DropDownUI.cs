using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class DropDownUI: TMP_Dropdown
    {
        private Canvas _parentCanvas;
        private Transform arrowImage;
        protected override void Awake()
        {
            base.Awake();
            _parentCanvas = GetComponentInParent<Canvas>();
            arrowImage = transform.Find("Arrow");
        }
        protected override GameObject CreateDropdownList(GameObject templateObj)
        {
            var dropDown = base.CreateDropdownList(templateObj);
            dropDown.GetComponent<Canvas>().sortingLayerName = _parentCanvas.sortingLayerName;
            if (arrowImage != null) arrowImage.localScale = new Vector3(1,-1,0);
            return dropDown;
        }
        
        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            var blocker =  base.CreateBlocker(rootCanvas);
            blocker.GetComponent<Canvas>().sortingLayerName = _parentCanvas.sortingLayerName;
            return blocker;
        }

        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            if (arrowImage != null) arrowImage.localScale = Vector3.one;
        }
    }
}