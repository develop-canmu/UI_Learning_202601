using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Text;
using CruFramework.Document;
using CruFramework.Editor.Adv;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace CruFramework.Editor.Document
{
    public abstract class DocumentEditor<T> : EditorWindow where T : DocumentAttribute
    {
        // ツールバー
        private Toolbar toolbar = null;
        
        // 説明文
        private ScrollView descriptionScrollView = null;
        
        // 表示中のリスト
        private ListView currentListView = null;
        
        // カテゴリルート
        private VisualElement categoryBody = null;
        // リストビュー
        private Dictionary<string, ListView> categoryListView = new Dictionary<string, ListView>();
        
        
        public void ExportText(string category)
        {
            Dictionary<string, List<Type>> types = GetDocumentTypes();
            
            if(types.TryGetValue(category, out List<Type> typeList) == false)return;

            StringBuilder sb = new StringBuilder();
            foreach(Type type in typeList)
            {
                // 属性
                T attribute = type.GetCustomAttribute<T>();
                if(attribute == null)return;
                sb.AppendLine("* # " + attribute.Name);
                sb.AppendLine(attribute.Text);
                ExportText(type, sb, 1);
                sb.AppendLine("<br>");
            }
            
            GUIUtility.systemCopyBuffer = sb.ToString();
        }
        
        public void ExportText(Type type, StringBuilder sb, int indent)
        {
            // フィールドの収集
            MemberInfo[] members = GetMembers(type);

            foreach(MemberInfo member in members)
            {
                // 属性取得
                T fieldAttribute = member.GetCustomAttribute<T>();
                if(fieldAttribute == null)continue;
                
                // 表示名
                string name = string.IsNullOrEmpty(fieldAttribute.Name) ? member.Name : fieldAttribute.Name;
                
                if(indent != 1 || name != "command")
                {
                    for(int i=0;i<indent;i++)sb.Append(">");
                    sb.AppendLine("* ### " + name);
                    sb.AppendLine(fieldAttribute.Text);
                    sb.AppendLine("<br>");
                }
                
                Type valueType = null;

                switch(member)
                {
                    case FieldInfo fi:
                        valueType = fi.FieldType;
                        break;
                }
                
                if(valueType == null)continue;
                ExportText(valueType, sb, indent + 1);
            }
        }

        private void OnEnable()
        {
            // ツールバー
            toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);
            
            VisualElement body = new VisualElement();
            body.style.flexDirection = FlexDirection.Row;
            rootVisualElement.Add(body);
            
            categoryBody = new VisualElement();
            categoryBody.style.width = 200.0f;
            body.Add(categoryBody);
            
            // 説明
            descriptionScrollView = new ScrollView();
            descriptionScrollView.style.flexGrow = 1.0f;
            body.Add(descriptionScrollView);
            
            // ドキュメント
            CreateDocument();
        }
        
        public void SetCategory(string category)
        {
            // 現在開いているビューを非表示に
            if(currentListView != null)
            {
                currentListView.style.display = DisplayStyle.None;
            }
            
            // ビューを開く
            currentListView = GetCategoryListView(category);
            currentListView.style.display = DisplayStyle.Flex;
        }
        
        private ListView GetCategoryListView(string category)
        {
            // 既に生成済み
            if(categoryListView.ContainsKey(category))
            {
                return categoryListView[category];
            }
            
            // ツールバーにボタンを追加
            ToolbarButton button = new ToolbarButton();
            button.text = category;
            // 押下時
            button.clicked += ()=>
            {
                SetCategory(category);
            };
            button.RegisterCallback<MouseDownEvent>((e)=>{if(Event.current.button == 1)OnRightClickedDocument(category);});
            toolbar.Add(button);
            
            // リストビュー
            ListView listView = new ListView();
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            listView.makeItem = ()=>
            {
                Button button = new Button();
                button.clicked += ()=>OnSelectedDocument(button);
                button.style.unityTextAlign = TextAnchor.MiddleLeft;
                return button;
            };
            
            // 非表示に
            listView.style.display = DisplayStyle.None;
            
            categoryBody.Add(listView);
            // リストに追加
            categoryListView.Add(category, listView);
            
            return listView;
        }
        
        private void OnRightClickedDocument(string category)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Exeport Text"), false, ()=>
            {
                ExportText(category);
            });
            menu.ShowAsContext();
        }

        private Dictionary<string, List<Type>> GetDocumentTypes()
        {
            Dictionary<string, List<Type>> types = new Dictionary<string, List<Type>>();
            
            // コマンドをリストに追加
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes())
                {
                    // 抽象クラスは省く
                    //if(type.IsAbstract)continue;
                    // 属性
                    T attribute = type.GetCustomAttribute<T>(false);
                    if(attribute == null)continue;
                    
                    if(types.TryGetValue( attribute.Category, out List<Type> typeList ) == false)
                    {
                        typeList = new List<Type>();
                        types.Add(attribute.Category, typeList);
                    }
                    
                    // ドキュメント追加
                    typeList.Add(type);
                }
            }
            
            return types;
        }
        
        private void CreateDocument()
        {
            Dictionary<string, List<Type>> types = GetDocumentTypes();
            CreateDocument(types);
        }
        
        private void CreateDocument(Dictionary<string, List<Type>> types)
        {

            foreach(KeyValuePair<string, List<Type>> type in types)
            {
                ListView listView = GetCategoryListView(type.Key);
                
                listView.itemsSource = type.Value;
            
                listView.bindItem = (e, i)=>
                {
                    Button button = (Button)e;
                    button.userData = type.Value[i];
                    T attribute = type.Value[i].GetCustomAttribute<T>();
                    button.text = attribute.Name;
                };
                
                if(currentListView == null)
                {
                    SetCategory(type.Key);
                }
            }
        }
        
        private Label CreateLabel(int fontSize, int index, string label)
        {
            // タイプ名表示
            Label labelElement = new Label();
            labelElement.style.fontSize = fontSize;
            labelElement.text = label;
            labelElement.style.marginLeft = index * 10.0f;
            return labelElement;
        }
        
        private void OnSelectedDocument(Button button)
        {
            // タイプ
            Type type = (Type)button.userData;
            // ドキュメントセット
            SetDocumentType(type);
        }
        
        public void SetDocumentType(Type type)
        {
            // 子要素を破棄
            descriptionScrollView.Clear();
            // 属性
            T attribute = type.GetCustomAttribute<T>();
            
            VisualElement parent = new VisualElement();
            descriptionScrollView.Add(parent);
            
            if(attribute == null)
            {
                parent.Add( CreateLabel(16, 0, type.Name + "のドキュメントがみつかりません") );
                return;
            }
            
            // ドキュメントの作成
            int fieldCount = 0;
            CreateDocument(parent, type, attribute.Name, attribute.Text, 20, 12, 0, ref fieldCount);
        }
        
        private void CreateDocument(VisualElement parent, Type type, string title, string description, int titleSize, int descriptionSize, int indent, ref int fieldCount)
        {
            
            // タイプ名表示
            parent.Add( CreateLabel(titleSize, indent, title) );
            // 説明
            if(string.IsNullOrEmpty(description) == false)
            {
                parent.Add( CreateLabel(descriptionSize, indent + 1, description) );
            }
            
            if(type == null)return;

            // フィールドの収集
            MemberInfo[] members = GetMembers(type);
            
            // 各フィールドを表示
            foreach(MemberInfo member in members)
            {
                // 属性取得
                T fieldAttribute = member.GetCustomAttribute<T>();
                if(fieldAttribute == null)continue;
                
                VisualElement fieldParent = new VisualElement();
                fieldParent.style.borderTopWidth = 1.0f;
                fieldParent.style.borderTopColor = Color.gray;
                parent.Add(fieldParent);
                fieldParent.style.backgroundColor = fieldCount % 2 == 0 ? new Color(0.2f, 0.2f, 0.2f, 1.0f) : new Color(0.1f, 0.1f, 0.1f, 1.0f);
                // 表示名
                string name = string.IsNullOrEmpty(fieldAttribute.Name) ? member.Name : fieldAttribute.Name;

                // 生成
                fieldCount++;
                
                Type valueType = null;

                switch(member)
                {
                    case FieldInfo fi:
                        valueType = fi.FieldType;
                        if(fi.IsStatic)name += " [static]";
                        break;
                    case PropertyInfo pi:
                        break;
                    case MethodInfo mi:
                        if(mi.IsStatic)name += " [static]";
                        break;
                }

                CreateDocument(fieldParent, valueType, name, fieldAttribute.Text, 12, 10, indent + 1, ref fieldCount);
            }
        }
        
        private MemberInfo[] GetMembers(Type type)
        {
            // フィールドの収集
            List<MemberInfo> members = new List<MemberInfo>();
            Type checkType = type;
            while(true)
            {
                if(checkType == null || checkType == typeof(object))break;
                members.AddRange(checkType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
                checkType = checkType.BaseType;
            }
            
            return members.ToArray();
        }
    }
}