using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace Pjfb
{
    [CustomEditor(typeof(UIToggle))]
    public class UIToggleEditor : UnityEditor.UI.ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            UIToggle toggle = (UIToggle)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            SerializedProperty onSoundType = serializedObject.FindProperty("onSoundType");
            EditorGUILayout.PropertyField(onSoundType);
            
            SerializedProperty offSoundType = serializedObject.FindProperty("offSoundType");
            EditorGUILayout.PropertyField(offSoundType);

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.LabelField("==== Unity Toggle ====");
            base.OnInspectorGUI();
        }
    }
}
