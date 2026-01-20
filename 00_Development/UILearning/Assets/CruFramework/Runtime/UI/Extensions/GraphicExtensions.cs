using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.UI
{
    public static class GraphicExtensions
    {
        public static Rect GetScreenRect(this Graphic graphic)
        {
            Vector3[] corners = new Vector3[4];
            graphic.rectTransform.GetWorldCorners(corners);
            
            if(graphic.canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Camera camera = graphic.canvas.worldCamera;
                corners[0] = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
                corners[2] = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
            }
            
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }
}
