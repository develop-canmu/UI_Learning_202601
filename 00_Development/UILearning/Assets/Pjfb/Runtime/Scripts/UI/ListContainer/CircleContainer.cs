using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pjfb.UI
{
    [ExecuteAlways]
    public class CircleContainer : UIBehaviour, ILayoutGroup 
    {
        public float radius = 100;
        public float offsetAngle;

#if UNITY_EDITOR
        protected override void OnValidate ()
        {
            base.OnValidate ();
            Arrange ();
        }
#endif

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();
            Arrange ();
        }

        // 要素数が変わると自動的に呼ばれるコールバック
        #region ILayoutController implementation
        public void SetLayoutHorizontal (){}
        public void SetLayoutVertical ()
        {
            Arrange();
        }
        #endregion

        void Arrange()
        {
            if (transform.childCount <= 0) return;
            float splitAngle = 360 / transform.childCount;
            var rect = transform as RectTransform;

            for (int elementId = 0; elementId < transform.childCount; elementId++) {
                var child = transform.GetChild (elementId) as RectTransform;
                float currentAngle = splitAngle * elementId + offsetAngle;
                child.anchoredPosition = new Vector2 (
                    Mathf.Cos (currentAngle * Mathf.Deg2Rad), 
                    Mathf.Sin (currentAngle * Mathf.Deg2Rad)) * radius;
            }
        }
    }
}