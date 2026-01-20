using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Pjfb
{
    [CustomEditor(typeof(PageScrollRect))]
    public class PageScrollRectEditor : ScrollRectEditor
    {
        public override void OnInspectorGUI()
        {
            // PageScrollRectのフィールドをInspectorに表示
            serializedObject.Update();
            FieldInfo[] fieldInfos = typeof(PageScrollRect).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                SerializeField attribute = fieldInfo.GetCustomAttribute<SerializeField>();
                if(attribute == null) continue;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldInfo.Name));
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.LabelField("==== Unity ScrollRect ====");
            base.OnInspectorGUI();
        }
    }
}