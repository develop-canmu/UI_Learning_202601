using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace Pjfb
{
    [CustomEditor(typeof(UIToggleMember), true)]
    public class UIToggleValueEditor : UIToggleEditor
    {
        public override void OnInspectorGUI()
        {
            UIToggle toggle = (UIToggle)target;
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            SerializedProperty value = serializedObject.FindProperty("value");
            EditorGUILayout.PropertyField(value);

            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }

            base.OnInspectorGUI();
        }
    }
}
