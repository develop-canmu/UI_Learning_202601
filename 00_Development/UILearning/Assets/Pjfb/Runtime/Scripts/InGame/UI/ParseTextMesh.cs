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
    public class ParseTextMesh : Text
    {
        [SerializeField, Range(0.0f, 10.0f)] private float xIntensity;
        [SerializeField, Range(0.0f, 10.0f)] private float yIntensity;
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            var vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);

            /*
             文字上下左右ガチャガチャバージョン
            var xDiff = 0.0f;
            var yDiff = 0.0f;
            if (i % 6 == 0)
            {
                xDiff = UnityEngine.Random.Range(-10.0f, 10.0f);
                yDiff = UnityEngine.Random.Range(-20.0f, 20.0f);
            }

            vertex.position.x += xDiff;
            vertex.position.y += yDiff;
            */
            
            for (var i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[i];
                vertex.position.x *= 1.0f + ((vertex.position.x + Mathf.Abs(vertex.position.y)) * xIntensity / rectTransform.rect.width);
                vertex.position.y *= 1.0f + ((vertex.position.x) * yIntensity / rectTransform.rect.height);
                vertices[i] = vertex;
            }
            
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ParseTextMesh))]
    [CanEditMultipleObjects]
    public class ParseTextMeshEditor : Editor
    {
    }
#endif
    
}