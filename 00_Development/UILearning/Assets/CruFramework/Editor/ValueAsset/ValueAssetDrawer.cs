using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CruFramework.Editor
{
    [CustomPropertyDrawer(typeof(ValueAssetRefAttribute), true)]
    public class ValueAssetDrawer: PropertyDrawer
    {
        
        // ローダー
        private ValueAssetLoader loader = null;
        // アセットの取得
        private ValueAsset valueAsset = null;
        
        // Serialize Cache
        private SerializedObject serializedObject = null;
        private SerializedProperty serializedProperty = null;
        private SerializedProperty valueProperty = null;
        
        // チェックしたId
        private int checkedIntId = -1;
        private string checkedStrId = string.Empty;
        // 初期化済み
        private bool isInitialized = false;
        
        // 初期化
        private void Initialize()
        {
            // すでに初期化済み
            if(isInitialized)return;
            isInitialized = true;
            // 属性
            ValueAssetRefAttribute a = (ValueAssetRefAttribute)attribute;
            // ローダーのタイプ
            Type loaderType = a.GetLoaderType();
            // ローダー
            loader = (ValueAssetLoader)Activator.CreateInstance(loaderType);
            // アセットの取得
            valueAsset = loader.GetValueAsset();
            // SerializeObject
            serializedObject = new SerializedObject(valueAsset);
            serializedProperty = serializedObject.FindProperty("values");
        }
        
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 初期化
            Initialize();
            // 値
            Rect valueRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - 32.0f, position.height);
            // フィールドタイプ
            SerializedPropertyType propertyType = property.propertyType;
            
            switch(propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    //
                    int id = property.intValue;
                    // ラベル
                    EditorGUI.LabelField(position, label + $"[{id}]");
                    
                    if(checkedIntId != id)
                    {
                        checkedIntId = id;

                        // Idの物を探す
                        for(int i=0;i<serializedProperty.arraySize;i++)
                        {
                            SerializedProperty value = serializedProperty.GetArrayElementAtIndex(i);
                            if(value.FindPropertyRelative("id").intValue == id)
                            {
                                valueProperty = value.FindPropertyRelative("value");
                                break;
                            }
                        }
                    }
                    break;
                }
                    
                case SerializedPropertyType.String:
                {
                    //
                    string key = property.stringValue;
                    
                    // ラベル
                    EditorGUI.LabelField(position, label + $"[{key}]");
                    
                    if(checkedStrId != key)
                    {
                        checkedStrId = key;

                        // Idの物を探す
                        for(int i=0;i<serializedProperty.arraySize;i++)
                        {
                            SerializedProperty value = serializedProperty.GetArrayElementAtIndex(i);
                            if(value.FindPropertyRelative("key").stringValue == key)
                            {
                                valueProperty = value.FindPropertyRelative("value");
                                break;
                            }
                        }
                    }
                    break;
                } 
            }
            
            if(valueProperty == null)
            {
                EditorGUI.LabelField(valueRect, "None");
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent(string.Empty));
                EditorGUI.EndDisabledGroup();
            }
                        
            // 変更ボタン
            Rect buttonRect = new Rect(position.x + position.width - 32.0f, position.y, 32.0f, position.height);
            if(GUI.Button(buttonRect, "◯"))
            {
                ValueAssetSelectWindow w = EditorWindow.CreateInstance<ValueAssetSelectWindow>();
                Rect rect = w.position;
                rect.width *= 600.0f;
                w.position = rect;
                w.ShowAuxWindow();
                w.Set(valueAsset, serializedObject, serializedProperty, (s)=> 
                {
                    
                    switch(propertyType)
                    {
                        case SerializedPropertyType.Integer:
                        {
                            property.intValue = s == null ? 0 : s.FindPropertyRelative("id").intValue;
                            
                            break;
                        }
                        case SerializedPropertyType.String:
                        {
                            property.stringValue = s == null ? string.Empty : s.FindPropertyRelative("key").stringValue;
                            break;
                        }
                    }
                    property.serializedObject.ApplyModifiedProperties();

                });
            }
        }
    }
    
    
    public class ValueAssetSelectWindow : EditorWindow
    {
        
        private class Element
        {
            public Label nameField = null;
            public PropertyField propertyField = null;
        }
        
        private string searchText = string.Empty;
        
        public void Set(ValueAsset valueAsset, SerializedObject serializeObject, SerializedProperty values, Action<SerializedProperty> onSelect)
        {
            rootVisualElement.Clear();
            
            ObjectField objectField = new ObjectField();
            rootVisualElement.Add(objectField);
            objectField.value = valueAsset;
            
            // 検索
            VisualElement searchElements = new VisualElement();
            rootVisualElement.Add(searchElements);
            searchElements.style.flexDirection = FlexDirection.Row;
            searchElements.style.height = 20.0f;
            searchElements.style.minHeight = 20.0f;
            // アイコン
            Image searchIcon = new Image();
            searchElements.Add(searchIcon);
            searchIcon.image = EditorGUIUtility.IconContent("Search Icon").image;
            searchIcon.style.width = 40.0f;
            // テキスト
            TextField searchTextField = new TextField();
            searchElements.Add(searchTextField);
            searchTextField.value = searchText;
            searchTextField.style.flexGrow = 1.0f;

            ListView listView = new ListView();
            rootVisualElement.Add(listView);
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight; 
            // 配列化
            SerializedProperty[] propertyList = new SerializedProperty[values.arraySize + 1];

            for(int i=0;i<values.arraySize;i++)
            {
                propertyList[i + 1] = values.GetArrayElementAtIndex(i);
            }
            
            List<SerializedProperty> drawList = new List<SerializedProperty>(propertyList);
            
            // リスト登録
            listView.itemsSource = drawList;
            // 選択タイプ
            listView.selectionType = SelectionType.Single;
            // 
            listView.makeItem += ()=>
            {
                VisualElement v = new VisualElement();
                v.style.flexDirection = FlexDirection.Row;
                Element e = new Element();
                
                e.nameField = new Label();
                e.nameField.style.width = 200.0f;
                v.Add(e.nameField);
                
                e.propertyField = new PropertyField();
                e.propertyField.SetEnabled(false);
                e.propertyField.label = string.Empty;
                e.propertyField.Bind(serializeObject);
                e.propertyField.style.flexGrow = 1.0f;
                v.Add(e.propertyField);
                
                v.userData = e;
                
                return v;
            };
            
            listView.bindItem += (e, i)=>
            {
                Element element = (Element)e.userData;
                
                if(i == 0)
                {
                    element.nameField.text = "None";
                    element.propertyField.style.display = DisplayStyle.None;  
                }
                else
                {
                    SerializedProperty v = drawList[i];
                    element.nameField.text = v.FindPropertyRelative("key").stringValue;
                    element.propertyField.BindProperty( v.FindPropertyRelative("value") );
                    element.propertyField.style.display = DisplayStyle.Flex; 
                }

            };
            
            listView.onSelectionChange += (s)=>
            {
                if(listView.selectedIndex == 0)
                {
                    onSelect(null);
                }
                else
                {
                    onSelect(drawList[listView.selectedIndex]);
                }
                
                Close();
            };
            
            
            searchTextField.RegisterValueChangedCallback((newValue)=>
            {
                searchText = newValue.newValue;
                // 表示するリストの再構築
                drawList.Clear();
                if(string.IsNullOrEmpty(searchText))
                {
                    drawList.AddRange(propertyList);
                }
                else
                {
                    drawList.Add(propertyList[0]);
                    for(int i=1;i<propertyList.Length;i++)
                    {
                        SerializedProperty v = propertyList[i];
                        string key = v.FindPropertyRelative("key").stringValue;
                        if(key.Contains( searchText ))
                        {
                            drawList.Add(v);
                            continue;
                        }
                        
                        string value = v.FindPropertyRelative("value").stringValue;
                        if(string.IsNullOrEmpty(value) == false)
                        {
                            if(value.Contains(searchText))
                            {
                                drawList.Add(v);
                                continue;
                            }
                        }

                    }
                }
                
                listView.RefreshItems();
            });
            
            searchTextField.Focus();
        }
    }
}
