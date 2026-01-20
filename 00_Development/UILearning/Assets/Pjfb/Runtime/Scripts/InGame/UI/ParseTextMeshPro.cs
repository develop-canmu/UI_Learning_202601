using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pjfb
{
    public class ParseTextMeshPro : TextMeshProUGUI
    {
        [SerializeField, Range(0.0f, 10.0f)] private float xIntensity;
        [SerializeField, Range(0.0f, 10.0f)] private float yIntensity;

        public override void Rebuild(CanvasUpdate update)
        {
            if (update == CanvasUpdate.LatePreRender)
            {
                var verts = mesh.vertices;
                for (var i = 0; i < verts.Length; i++)
                {
                    verts[i].x *= 1.0f + ((verts[i].x + Mathf.Abs(verts[i].y)) * xIntensity / rectTransform.rect.width);
                    verts[i].y *= 1.0f + ((verts[i].x) * yIntensity / rectTransform.rect.height);
                }
                mesh.vertices = verts;
                canvasRenderer.SetMesh(mesh);
            }
            
            base.Rebuild(update);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ParseTextMeshPro))]
    [CanEditMultipleObjects]
    public class ParseTextMeshProEditor : Editor
    {
    }
#endif

}