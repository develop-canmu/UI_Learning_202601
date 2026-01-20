using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CruFramework.Adv;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Adv
{
    public class AdvConfigEditor : EditorWindow
    {
        public static void Open(AdvConfig config)
        {
            AdvConfigEditor e = GetWindow<AdvConfigEditor>();
            e.SetConfig(config);
        }
        
        private class ListViewElements
        {
            public Button deleteButton = null;
            public PropertyField idField = null;
            public PropertyField valueField = null;
        }
        
        private class PropertyData
        {
            public SerializedProperty serializedProperty = null;
            public int index = 0;
        }
        
        [SerializeField]
        private AdvConfig config = null;
        
        // パラメータルート
        private VisualElement parameterElement = null;
        
        // リストビュー
        private ListView listView = null;
        // 表示するプロパティリスト
        private List<PropertyData> properties = new List<PropertyData>();
        // 表示するオブジェクト
        private SerializedObject serializedObject = null;
        // 表示する値
        private SerializedProperty valueList = null;
        // Config
        private object configReflectionValue = null;
        private IList configReflectionValueList = null;
        // 選択中のカテゴリ
        private int currentCategory = -1;
        // 
        private List<FieldInfo> valueFieldInfoList = new List<FieldInfo>();

        private void OnEnable()
        {
            SetConfig(config);
            
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            // UndoRedoした時にリストを更新
            if(listView != null)
            {
                UpdatePropertyList();
            }
        }

        private void SetConfig(AdvConfig config)
        {
            this.config = config;
            UpdateUI();
        }
        
        private void UpdatePropertyList()
        {
            UpdatePropertyList(currentCategory);
        }
        
        private void UpdatePropertyList(int category)
        {
            currentCategory = category;
            // リスト初期化
            properties.Clear();
            
            // Apply
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            
            // リストを取得
            for(int n=0;n<valueList.arraySize;n++)
            {
                // カテゴリをチェック
                object v = AdvEditorUtility.GetField(configReflectionValueList[n].GetType(), "value").GetValue(configReflectionValueList[n]);
                if(v is IAdvObjectCategory c)
                {
                    if(c.Category != category)continue;
                }

                PropertyData p = new PropertyData();
                p.serializedProperty = valueList.GetArrayElementAtIndex(n);
                p.index = n;
                properties.Add(p);
            }
            // ListViewの更新
            listView.RefreshItems();
        }
        
        private void UpdateUI()
        {
            rootVisualElement.Clear();
            
            if(config == null)
            {
                return;
            }
            
            valueFieldInfoList.Clear();
            
            rootVisualElement.style.flexDirection = FlexDirection.Row;
            
            // Configのタイプ
            Type type = config.GetType();
            // フィールドを取得
            List<FieldInfo> idFieldList = new List<FieldInfo>();
            while(true)
            {
                if(type == null || type == typeof(object))break;
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach(FieldInfo field in fields)
                {
                    // SerializeField
                    if(field.GetCustomAttribute<SerializeField>() == null)continue;
                    // AdvId
                    if(field.FieldType.IsSubclassOf(typeof(AdvObjectIds)) == false)
                    {
                        valueFieldInfoList.Add(field);
                    }
                    else
                    {
                        idFieldList.Add(field);
                    }
                }
                
                type = type.BaseType;
            }
            
            // その他フィールド表示用
            idFieldList.Add(null);
            
            
            // リストビュー
            ListView listView = new ListView();
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            listView.style.width = 200.0f;
            listView.style.alignContent = Align.FlexStart;
            rootVisualElement.Add(listView);
            
            listView.itemsSource = idFieldList;
            
            listView.makeItem += ()=>
            {
                Button button = new Button();
                button.style.unityTextAlign = TextAnchor.MiddleLeft;
                button.clicked += ()=>OnClick(idFieldList[(int)button.userData]);
                return button;
            };
            
            listView.bindItem += (e, i)=>
            {
                Button button = (Button)e;
                button.userData = i;
                button.text = idFieldList[i] != null ? idFieldList[i].Name : "Ohter";
            };
            
            parameterElement = new VisualElement();
            parameterElement.style.flexGrow = 1.0f;
            rootVisualElement.Add(parameterElement);
        }
        
        private void SetIdField(FieldInfo field)
        {
            // FieldType
            Type type = field.FieldType;
            
            // AdvConfig
            serializedObject = new SerializedObject(config);
            // Update
            serializedObject.Update();
            // Config取得
            SerializedProperty configValue = serializedObject.FindProperty(field.Name);
            // ValueList
            valueList = configValue.FindPropertyRelative("values");
            properties = new List<PropertyData>();
            
            
            // Configを取得
            configReflectionValue = field.GetValue(config);
            // ValueListを取得
            configReflectionValueList = (IList)AdvEditorUtility.GetField(configReflectionValue.GetType(), "values").GetValue(configReflectionValue);
            
                      // カテゴリを持っている？
            bool hasCategory = configReflectionValue is IHasAdvObjectCategory;
            // 最初に開くカテゴリId
            int firstOpenCategoryId = -1;
            
            // カテゴリの表示
            if(hasCategory)
            {
                // ツールバー
                Toolbar toolbar = new Toolbar();
                parameterElement.Add(toolbar);

                // ツールバーにボタンを追加
                foreach(KeyValuePair<int, AdvObjectCategoryId> category in config.ObjectCategories.GetDictionary())
                {
                    if(firstOpenCategoryId == -1)firstOpenCategoryId = category.Key;
                    ToolbarButton button = new ToolbarButton();
                    button.text = category.Value.Name;
                    button.clicked += ()=>
                    {
                        UpdatePropertyList(category.Key);
                    };
                    toolbar.Add(button);
                }
            }


            // リストビュー
            if(listView != null)listView.Clear();
            listView = new ListView();
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            parameterElement.Add(listView);
            
            
            listView.makeItem += ()=>
            {
                ListViewElements elements = new ListViewElements();
                
                VisualElement v = new VisualElement();
                v.style.flexDirection = FlexDirection.Row;
                v.userData = elements;
                // 削除ボタン
                elements.deleteButton = new Button();
                elements.deleteButton.style.width = EditorGUIUtility.singleLineHeight;
                elements.deleteButton.style.height = EditorGUIUtility.singleLineHeight;
                elements.deleteButton.text = "✗";
                elements.deleteButton.clicked += ()=>
                {
                    PropertyData p = properties[(int)elements.deleteButton.userData];
                    valueList.DeleteArrayElementAtIndex(p.index);
                    UpdatePropertyList();
                };
                v.Add(elements.deleteButton);
                
                // Id入力フィールド
                elements.idField = new PropertyField();
                elements.idField.style.width = 60.0f;
                elements.idField.style.height = EditorGUIUtility.singleLineHeight;
                elements.idField.label = string.Empty;
                elements.idField.Bind(serializedObject);
                elements.idField.SetEnabled(false);
                v.Add(elements.idField);
                
                // 値の入力フィールド
                elements.valueField = new PropertyField();
                elements.valueField.style.flexGrow = 1.0f;
                elements.valueField.Bind(serializedObject);
                v.Add( elements.valueField );
                
                return v;
            };
            
            listView.bindItem += (e, i)=>
            {
                VisualElement parent = (VisualElement)e;
                ListViewElements elements = (ListViewElements)parent.userData;
                // 背景色
                parent.style.backgroundColor = i % 2 == 0 ? new Color(0.3f, 0.3f, 0.3f, 1.0f) : new Color(0.2f, 0.2f, 0.2f, 1.0f);
                elements.deleteButton.userData = i;
                elements.idField.BindProperty(properties[i].serializedProperty.FindPropertyRelative("id"));
                elements.valueField.BindProperty(properties[i].serializedProperty.FindPropertyRelative("value"));
            };
            
            listView.itemsSource = properties;
            listView.RefreshItems();
            
            // 追加ボタン
            Button addButton = new Button();
            addButton.style.width = EditorGUIUtility.singleLineHeight;
            addButton.style.height = EditorGUIUtility.singleLineHeight;
            addButton.text = "+";
            addButton.clicked += ()=>
            {
                // Undo登録
                Undo.RecordObject(serializedObject.targetObject, "AdvConfig");

                // 値追加メソッドを取得
                MethodInfo m = configReflectionValue.GetType().GetMethod(nameof(AdvObjectIds.AddNewValue));
                // 実行
                object value = m.Invoke(configReflectionValue, null);
                // カテゴリを追加
                if(value is IAdvObjectCategory c)
                {
                    c.SetCategory(currentCategory);
                }

                // リスト更新
                UpdatePropertyList();
            };
            parameterElement.Add(addButton);
            
            // リスト更新
            UpdatePropertyList(firstOpenCategoryId);
        }
        
        private void SetOtherField()
        {
            // AdvConfig
            serializedObject = new SerializedObject(config);
            // Update
            serializedObject.Update();
            
            foreach(FieldInfo field in valueFieldInfoList)
            {
                SerializedProperty p = serializedObject.FindProperty(field.Name);
                PropertyField propertyField = new PropertyField(p);
                propertyField.Bind(serializedObject);
                parameterElement.Add(propertyField);
            }
        }
        
        private void OnClick(FieldInfo field)
        {
            parameterElement.Clear();
  
            if(field != null)
            {
                SetIdField(field);
            }
            else
            {
                SetOtherField();
            }
  
        }
    }
}
