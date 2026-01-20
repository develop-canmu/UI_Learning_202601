using UnityEditor;
using System;
using UnityEngine;
using System.Linq;

namespace Pjfb
{
    /// <summary> 継承先の型をドロップダウンで選択できるPropertyDrawer </summary>
    [CustomPropertyDrawer(typeof(SerializeReferenceDropdownAttribute))]
    public class SerializeReferenceTypeDrawer : PropertyDrawer
    {
        // 基底の型
        private Type baseType = null;
        // 継承先の型
        private Type[] derivedTypes = null;
        // 型名
        private string[] typeNames = null;
        
        // インデックスのキャッシュ
        private int cachedIndex = -1;
        
        // インスペクターに描画する処理
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (baseType == null)
            {
                // アトリビュートから基底の型を取得
                baseType = fieldInfo.FieldType;
            }
            
            if (derivedTypes == null)
            {
                // 継承先の型を取得
                derivedTypes = TypeCache.GetTypesDerivedFrom(baseType).Where(t => !t.IsAbstract && !t.IsInterface).ToArray();
                typeNames = derivedTypes.Select(t => t.Name).ToArray();
            }
            
            // ドロップダウン枠描画
            Rect line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            // 最初だけインデックスを取得
            if (property.managedReferenceValue != null && cachedIndex < 0)
            {
                Type currentType = property.managedReferenceValue.GetType();
                // キャッシュ情報更新
                cachedIndex = Array.IndexOf(derivedTypes, currentType);
            }
            
            // ポップアップ描画
            int nextIndex = EditorGUI.Popup(line, label.text, Math.Max(0, cachedIndex), typeNames);
            // 選択項目が変わったらインスタンスを生成して代入
            if (nextIndex != cachedIndex)
            {
                property.serializedObject.Update();
                property.managedReferenceValue = Activator.CreateInstance(derivedTypes[nextIndex]);
                // キャッシュ情報更新
                cachedIndex = nextIndex;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            // インスタンスのフィールドを表示
            if (!string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                Rect propertyPosition = EditorGUI.PrefixLabel(position, GUIContent.none);
                propertyPosition.y += EditorGUIUtility.singleLineHeight;
                propertyPosition.height = position.height - EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(propertyPosition, property, new GUIContent("Property Fields"), true);
            }
        }
        
        // インスペクター上で表示される高さを取得
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // managedReferenceFullTypenameが空の場合は高さを1行分にする
            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            // プロパティの高さ + ドロップダウン分の高さを返す
            return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight;
        }
    }
}
