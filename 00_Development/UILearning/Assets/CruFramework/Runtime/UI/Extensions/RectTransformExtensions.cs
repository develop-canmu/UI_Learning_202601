using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CruFramework.UI
{
    public static class RectTransformExtensions
    {
        public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            if(canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Camera camera = canvas.worldCamera;
                corners[0] = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
                corners[2] = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
            }
            
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }
}

