using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace PolyQA.Editor
{
    public class AlwaysIncludedShaderList
    {
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _alwaysIncludedShaders;
        
        public AlwaysIncludedShaderList()
        {
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            _serializedObject = new SerializedObject(graphicsSettingsObj);
            _alwaysIncludedShaders = _serializedObject.FindProperty("m_AlwaysIncludedShaders");
        }
        
        public bool Contains(Shader shader)
        {
            for (var i = 0; i < _alwaysIncludedShaders.arraySize; ++i)
            {
                var elm = _alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (shader == elm.objectReferenceValue)
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool Add(Shader shader)
        {
            if (Contains(shader))
            {
                return false;
            }
            
            var arrayIndex = _alwaysIncludedShaders.arraySize;
            _alwaysIncludedShaders.InsertArrayElementAtIndex(arrayIndex);
            var elm = _alwaysIncludedShaders.GetArrayElementAtIndex(arrayIndex);
            elm.objectReferenceValue = shader;
 
            _serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            return true;
        }
        
        public bool Remove(Shader shader)
        {
            for (var i = 0; i < _alwaysIncludedShaders.arraySize; ++i)
            {
                var elm = _alwaysIncludedShaders.GetArrayElementAtIndex(i);
                if (shader != elm.objectReferenceValue)
                {
                    continue;
                }
                _alwaysIncludedShaders.DeleteArrayElementAtIndex(i);
                _serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                return true;
            }

            return false;
        }
    }
}