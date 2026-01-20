using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.Editor.Adv;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;

namespace CruFramework.Adv
{
    
    [CustomPropertyDrawer(typeof(AdvObjectIdAttribute))]
    public class AdvObjectIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Editor
            if(AdvEditor.Instance == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent("AdvEditorがみつかりません"));
                return;
            }

            // Configファイル
            AdvConfig config = AdvEditor.Instance.AdvConfigAsset;
            
            if(config == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent("AdvConfigがみつかりません"));
                return;
            }
            
            // 属性
            AdvObjectIdAttribute advObjectAttribute = (AdvObjectIdAttribute)attribute;
            // タイプ
            Type configType = config.GetType();
            // フィールド取得
            PropertyInfo field = configType.GetProperty(advObjectAttribute.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            if(field == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent($"{advObjectAttribute.Name}がみつかりません"));
                return;
            }
            
            AdvObjectIds objectIds = field.GetValue(config) as AdvObjectIds;

            if(objectIds == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent($"AdvObjectIdsがみつかりません") );
                return;
            }
            
            // 文字列定義の取得
            List<int> ids = new List<int>(objectIds.GetIds());
            List<string> names = new List<string>(objectIds.GetNames());
            
            // Id0に固定で値を入れる
            ids.Insert(0, 0);
            names.Insert(0, "None");

            // 選択位置
            int selected = -1;
            int selectedIndex = 0;
            // Index
            List<int> index = new List<int>();
            // メニュー文字列
            List<GUIContent> menuString = new List<GUIContent>();
            // データ生成
            for(int i=0;i<ids.Count;i++)
            {
                if(ids[i] == property.intValue)
                {
                    selected = ids[i];
                    selectedIndex = i;
                }
                
                if(objectIds.GetValueObject(ids[i]) is IAdvObjectCategory c)
                {
                    menuString.Add( new GUIContent(config.ObjectCategories[c.Category].Name + "/" + names[i]) );
                }
                else
                {
                    menuString.Add( new GUIContent(names[i]) );
                }
                
                
                index.Add( ids[i] );
            }
            
            // Idが見つからなかった場合
            if(selected == -1)
            {
                selected = property.intValue;
                index.Insert(0, selected);
                menuString.Insert(0, new GUIContent("Not found id : " + selected));
            }
            
            switch(advObjectAttribute.WinType)
            {
                case AdvObjectIdAttribute.WindowType.Generic:
                {
                    property.intValue = EditorGUI.IntPopup(position, label, selected, menuString.ToArray(), index.ToArray());
                    break;
                }
                case AdvObjectIdAttribute.WindowType.SearchWindow:
                {
                    Rect labelRect = position;
                    labelRect.width = EditorGUIUtility.labelWidth;
                    GUI.Label(labelRect, label);
                    position.x += EditorGUIUtility.labelWidth;
                    position.width -= EditorGUIUtility.labelWidth;
                    
                    if(GUI.Button(position, menuString[selectedIndex]))
                    {
                        AdvObjectIdSearchWindow window = AdvObjectIdSearchWindow.Create($"Select {advObjectAttribute.Name}", menuString, index.ToArray(), (selectIndex)=>
                        {
                            property.intValue = selectIndex;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                        window.Open( EditorGUIUtility.GUIToScreenPoint(Event.current.mousePosition) );
                    }
                    break;
                }
            }
        }
    }
}
