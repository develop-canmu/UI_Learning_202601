#if POLYQA_USE_UGUI
using UnityEngine;
using UnityEngine.EventSystems;

namespace PolyQA.Extensions
{
    public static class UIExtensions
    {
        public static Vector2 GetScreenPosition(this GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            if (!rect) throw new MissingComponentException("RectTransform is not found.");
            return rect.GetScreenPosition();
        }

        public static Rect GetScreenRect(this GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            if (!rect) throw new MissingComponentException("RectTransform is not found.");
            return rect.GetScreenRect();
        }

        public static Vector2 GetScreenPosition(this UIBehaviour ui)
        {
            return ui.GetComponent<RectTransform>().GetScreenPosition();
        }

        public static Camera FindCamera(this UIBehaviour ui)
        {
            return ui.GetComponent<RectTransform>().FindCamera();
        }

        public static Canvas FindCanvas(this UIBehaviour ui)
        {
            return ui.GetComponent<RectTransform>().FindCanvas();
        }

        public static Vector2 GetScreenPosition(this RectTransform rect)
        {
            var worldPosition = rect.TransformPoint(rect.rect.center);
            var camera = rect.FindCamera();
            return RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        }

        public static Rect GetScreenRect(this RectTransform rect)
        {
            var worldLeftBottom = rect.TransformPoint(rect.rect.min);
            var worldRightTop = rect.TransformPoint(rect.rect.max);
            var camera = rect.FindCamera();
            var leftBottom = RectTransformUtility.WorldToScreenPoint(camera, worldLeftBottom);
            var rightTop = RectTransformUtility.WorldToScreenPoint(camera, worldRightTop);
            return new Rect(leftBottom, rightTop - leftBottom);
        }

        public static Canvas FindCanvas(this RectTransform rect)
        {
            return rect.gameObject.GetComponentInParent<Canvas>(false);
        }

        public static Camera FindCamera(this RectTransform rect)
        {
            var canvas = rect.FindCanvas();
            if (!canvas) return null;

            var renderMode = canvas.renderMode;
            if (renderMode == RenderMode.ScreenSpaceOverlay
                || (renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
            {
                return null;
            }
            return canvas.worldCamera ? canvas.worldCamera : Camera.main;
        }

        public static void SetScreenPosition(this RectTransform rect, Vector2 point)
        {
            Camera camera = rect.FindCamera();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, point, camera, out var position);
            rect.position = position;
        }
    }
}
#endif
