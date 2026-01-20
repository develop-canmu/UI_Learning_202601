using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CruFramework.Editor.Tools
{
    internal class DefineSymbolEditor : EditorWindow
    {
        private static readonly string FileDirectory = "CruFramework/Tools";
        private static readonly string FilePath = FileDirectory + "/DefineSymbols.txt";

        [MenuItem(EditorMenu.ToolMenuPath + "/" + nameof(DefineSymbolEditor))]
        public static void Open()
        {
            EditorWindow window = GetWindow<DefineSymbolEditor>(nameof(DefineSymbolEditor));
            window.Focus();
        }

        [Serializable]
        private class DefineData
        {
            public string DefineSymbol = string.Empty;
            public bool Enable = false;
        }

        [Serializable]
        private class DefineDatas
        {
            public DefineData[] Datas = Array.Empty<DefineData>();
        }

        private class ListViewItem : VisualElement
        {
            public TextField DefineField = new TextField();
            public Toggle EnableToggle = new Toggle();

            public ListViewItem()
            {
                style.flexDirection = FlexDirection.Row;
                // 要素を中央に寄せる
                style.paddingTop = 3;
                style.paddingBottom = 3;
                DefineField.style.flexGrow = 1;

                Add(DefineField);
                Add(EnableToggle);
            }
        }

        [SerializeField]
        private DefineDatas defineDatas = new DefineDatas();

        ListView listView = null;

        private void OnEnable()
        {
            // 保存データ取得
            LoadDefineDatas();

            // ツールバー
            Toolbar toolbar = new Toolbar();
            toolbar.style.alignItems = Align.Center;
            rootVisualElement.Add(toolbar);

            // ボタンを右に配置したいのでSpacer追加
            toolbar.Add(new ToolbarSpacer() { flex = true });

            // 適用ボタン
            ToolbarButton applyButton = new ToolbarButton();
            applyButton.text = "Apply";
            applyButton.clicked += () =>
            {
                string json = JsonUtility.ToJson(defineDatas);

                // ディレクトリ作成
                if (!Directory.Exists(FileDirectory))
                {
                    Directory.CreateDirectory(FileDirectory);
                }
                // 保存
                File.WriteAllText(FilePath, json);
                // エディタに適用
                string symbols = string.Join(";", defineDatas.Datas.Where(data => data.Enable).Select(data => data.DefineSymbol));
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            };
            toolbar.Add(applyButton);

            toolbar.Add(new ToolbarSpacer());

            listView = new ListView();
            listView.itemsSource = defineDatas.Datas;
            // リストビューで表示する要素を生成
            listView.makeItem = () =>
            {
                ListViewItem item = new ListViewItem();
                return item;
            };
            // データと紐づける
            listView.bindItem = (e, i) =>
            {
                ListViewItem item = (ListViewItem)e;
                // 要素にデータを適用
                item.DefineField.value = defineDatas.Datas[i].DefineSymbol;
                item.EnableToggle.value = defineDatas.Datas[i].Enable;
                // 変更コールバック登録
                item.DefineField.UnregisterCallback<ChangeEvent<string>, int>(OnDefineChanged);
                item.DefineField.RegisterCallback<ChangeEvent<string>, int>(OnDefineChanged, i);
                // 変更コールバック登録
                item.EnableToggle.UnregisterCallback<ChangeEvent<bool>, int>(OnDefineEnableChanged);
                item.EnableToggle.RegisterCallback<ChangeEvent<bool>, int>(OnDefineEnableChanged, i);
            };
            // 追加
            listView.itemsAdded += indexes =>
            {
                Undo.RecordObject(this, "Add Defines");
                foreach (int index in indexes)
                {
                    ArrayUtility.Add(ref defineDatas.Datas, new DefineData());
                }
            };
            // 削除
            listView.itemsRemoved += indexes =>
            {
                Undo.RecordObject(this, "Remove Defines");
                foreach (int index in indexes)
                {
                    ArrayUtility.RemoveAt(ref defineDatas.Datas, index);
                }
            };
            // 並べ替え
            listView.itemIndexChanged += (index1, index2) =>
            {
                Undo.RecordObject(this, "Reorder Defines");
                DefineData temp = defineDatas.Datas[index2];
                defineDatas.Datas[index2] = defineDatas.Datas[index1];
                defineDatas.Datas[index1] = temp;
            };
            // 並び替え可能に
            listView.reorderable = true;
            listView.reorderMode = ListViewReorderMode.Animated;
            // 追加削除ボタン表示
            listView.showAddRemoveFooter = true;
            // リストビューに描画命令飛ばす
            listView.RefreshItems();
            rootVisualElement.Add(listView);

            Undo.undoRedoPerformed += OnUndoRedoCallback;
        }

        private void OnDefineChanged(ChangeEvent<string> evt, int index)
        {
            Undo.RecordObject(this, "Change Define");
            TextField defineField = (TextField)evt.target;
            defineDatas.Datas[index].DefineSymbol = defineField.value;
        }

        private void OnDefineEnableChanged(ChangeEvent<bool> evt, int index)
        {
            Undo.RecordObject(this, "Change DefineEnable");
            Toggle enableToggle = (Toggle)evt.target;
            defineDatas.Datas[index].Enable = enableToggle.value;
        }

        /// <summary>保存されているデータ取得</summary>
        private void LoadDefineDatas()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    defineDatas = JsonUtility.FromJson<DefineDatas>(json);
                }
                catch
                {
                }
            }

            if (defineDatas == null)
            {
                defineDatas = new DefineDatas();
            }
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoCallback;
        }

        private void OnUndoRedoCallback()
        {
            listView.itemsSource = defineDatas.Datas;
            listView.RefreshItems();
        }
    }
}
