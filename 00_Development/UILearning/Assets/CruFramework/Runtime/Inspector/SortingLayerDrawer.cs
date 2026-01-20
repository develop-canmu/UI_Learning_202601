using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CruFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SortingLayerDrawerAttribute : PropertyAttribute
    {
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SortingLayerDrawerAttribute))]
    public class SortingLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Label(position, label);
            position.x += EditorGUIUtility.labelWidth;
            // レイヤー一覧
            SortingLayer[] layers = SortingLayer.layers;
            
            int[] id = new int[layers.Length];
            string[] name = new string[layers.Length];
            
            for(int i=0;i<layers.Length;i++)
            {
                name[i] = layers[i].name;
                id[i] = layers[i].id;
            }
            
            property.intValue = EditorGUI.IntPopup(position, property.intValue, name, id);
        }
    }
#endif
}