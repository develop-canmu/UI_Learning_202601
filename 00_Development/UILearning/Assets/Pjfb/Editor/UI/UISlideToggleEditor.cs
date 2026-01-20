using UnityEditor;
using UnityEngine;

namespace Pjfb
{
    [CustomEditor(typeof(UISlideToggle))]
    public class UISlideToggleEditor : UnityEditor.UI.SliderEditor
    {
        public override void OnInspectorGUI()
        {
            UISlideToggle toggle = (UISlideToggle)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isOn"), new GUIContent("m_isOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onSoundType"), new GUIContent("onSoundType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("offSoundType"), new GUIContent("offSoundType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handleImage"), new GUIContent("handleImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bgImage"), new GUIContent("bgImage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("imageOn"), new GUIContent("imageOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("imageOff"), new GUIContent("imageOff"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("textOn"), new GUIContent("textOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("textOff"), new GUIContent("textOff"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colorOn"), new GUIContent("colorOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("colorOff"), new GUIContent("colorOff"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("switchDuration"), new GUIContent("switchDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveDuration"), new GUIContent("moveDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onValueChanged"), new GUIContent("onValueChanged"));

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.LabelField("==== Unity Slider ====");
            base.OnInspectorGUI();
        }
    }
}
