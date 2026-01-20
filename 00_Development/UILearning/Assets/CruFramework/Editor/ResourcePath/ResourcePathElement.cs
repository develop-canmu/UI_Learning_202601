using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace CruFramework
{
    public class ResourcePathElement : VisualElement
    {
        // 内部クラス定義
        public enum SortType
        {
            None,
            Key,
            Path
        }
        
        public class FilterSortState
        {
            public string searchText = "";
            public bool useRegex = false;
            public SortType sortType = SortType.None;
            public bool sortAscending = true;
            
            // パフォーマンス最適化用
            public string lastProcessedSearchText = null;
            public SortType lastProcessedSortType = (SortType)(-1);
            public bool lastProcessedSortAscending = false;
            public bool lastProcessedUseRegex = true;
            public bool isFirstRefresh = true;
            
            // 正規表現キャッシュ
            public string cachedSearchText = "";
            public System.Text.RegularExpressions.Regex cachedRegex = null;
            
            // データ変更フラグ（削除・追加時に強制リフレッシュ）
            public bool forceRefresh = false;
        }
        
        public class ListItemData
        {
            public Button deleteButton;
            public TextField keyField;
            public TextField pathField;
            public EventCallback<ChangeEvent<string>> keyCallback;
            public EventCallback<ChangeEvent<string>> pathCallback;
            public System.Action deleteAction;
        }
        
        // 内部状態管理
        private ResourcePathAsset asset;
        private List<ResourcePathAsset.PathData> filteredList = new List<ResourcePathAsset.PathData>();
        private FilterSortState filterState = new FilterSortState();
        
        // UI要素
        private TextField searchField;
        private Toggle regexToggle;
        private DropdownField sortDropdown;
        private Toggle sortOrderToggle;
        private Label countLabel;
        private ListView listView;
        
        // 設定
        private bool showHeader;
        private bool showUtilityButtons;
        
        public ResourcePathElement(ResourcePathAsset asset, bool showHeader = false, bool showUtilityButtons = false)
        {
            this.asset = asset;
            this.showHeader = showHeader;
            this.showUtilityButtons = showUtilityButtons;
            
            CreateUI();
            SetupCallbacks();
            RefreshViewImmediate();
            
            // ライフサイクル管理
            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }
        
        private void CreateUI()
        {
            style.flexDirection = FlexDirection.Column;
            style.flexGrow = 1;
            
            // ヘッダー（オプション）
            if (showHeader)
            {
                CreateHeaderSection();
            }
            
            // 検索バー
            CreateSearchBar();
            
            // ソートバー
            CreateSortBar();
            
            // リストビュー
            CreateListView();
            
            // 追加ボタン
            CreateAddButton();
            
            // ユーティリティボタン（オプション）
            if (showUtilityButtons)
            {
                CreateUtilitySection();
            }
        }
        
        private void CreateHeaderSection()
        {
            var headerContainer = new VisualElement();
            headerContainer.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            headerContainer.style.paddingTop = 5;
            headerContainer.style.paddingBottom = 5;
            headerContainer.style.paddingLeft = 10;
            headerContainer.style.paddingRight = 10;
            headerContainer.style.marginBottom = 10;
            
            var title = new Label("Resource Path Asset");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            headerContainer.Add(title);
            
            var description = new Label("リソースパスの管理とキー・パスの検索が可能です（キーワード、正規表現対応）");
            description.style.fontSize = 12;
            description.style.color = new Color(0.8f, 0.8f, 0.8f);
            headerContainer.Add(description);
            
            Add(headerContainer);
        }
        
        private void CreateSearchBar()
        {
            var searchContainer = new VisualElement();
            searchContainer.style.flexDirection = FlexDirection.Row;
            searchContainer.style.alignItems = Align.Center;
            searchContainer.style.minHeight = 25;
            searchContainer.style.marginBottom = 10;
            
            var searchIcon = new VisualElement();
            searchIcon.style.backgroundImage = EditorGUIUtility.IconContent("Search Icon").image as Texture2D;
            searchIcon.style.width = 16;
            searchIcon.style.height = 16;
            searchIcon.style.marginRight = 5;
            searchContainer.Add(searchIcon);
            
            var searchLabel = new Label("検索:");
            searchLabel.style.marginRight = 5;
            searchLabel.style.minWidth = 40;
            searchContainer.Add(searchLabel);
            
            searchField = new TextField();
            searchField.style.flexGrow = 1;
            searchField.style.marginRight = 10;
            searchContainer.Add(searchField);
            
            regexToggle = new Toggle();
            regexToggle.text = "正規表現";
            regexToggle.style.marginRight = 5;
            regexToggle.style.width = 80;
            regexToggle.style.fontSize = 12;
            searchContainer.Add(regexToggle);
            
            Add(searchContainer);
        }
        
        private void CreateSortBar()
        {
            var sortContainer = new VisualElement();
            sortContainer.style.flexDirection = FlexDirection.Row;
            sortContainer.style.alignItems = Align.Center;
            sortContainer.style.minHeight = 25;
            sortContainer.style.marginBottom = 10;
            
            var sortLabel = new Label("ソート:");
            sortLabel.style.marginRight = 5;
            sortLabel.style.width = 50;
            sortContainer.Add(sortLabel);
            
            sortDropdown = new DropdownField();
            sortDropdown.choices = new List<string> { "なし", "キー", "パス" };
            sortDropdown.value = "なし";
            sortDropdown.style.width = 80;
            sortDropdown.style.marginRight = 10;
            sortContainer.Add(sortDropdown);
            
            sortOrderToggle = new Toggle();
            sortOrderToggle.text = "昇順";
            sortOrderToggle.value = true;
            sortOrderToggle.style.marginRight = 10;
            sortOrderToggle.style.width = 60;
            sortOrderToggle.style.fontSize = 12;
            sortContainer.Add(sortOrderToggle);
            
            countLabel = new Label();
            countLabel.style.flexGrow = 1;
            countLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            countLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            sortContainer.Add(countLabel);
            
            Add(sortContainer);
        }
        
        private void CreateListView()
        {
            listView = new ListView();
            listView.style.minHeight = 300;
            listView.style.maxHeight = 600;
            listView.style.flexGrow = 1;
            listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            listView.showBorder = true;
            listView.itemsSource = filteredList;
            listView.makeItem = CreateListItem;
            listView.bindItem = BindListItem;
            
            Add(listView);
        }
        
        private void CreateAddButton()
        {
            var addButton = new Button();
            addButton.text = "新しいエントリを追加";
            addButton.style.height = 25;
            addButton.style.marginTop = showHeader ? 10 : 5;
            addButton.style.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            addButton.clicked += OnAddButtonClicked;
            
            Add(addButton);
        }
        
        private void CreateUtilitySection()
        {
            var utilityContainer = new VisualElement();
            utilityContainer.style.flexDirection = FlexDirection.Row;
            utilityContainer.style.marginTop = 10;
            
            var editorWindowButton = new Button();
            editorWindowButton.text = "専用エディタで開く";
            editorWindowButton.style.flexGrow = 1;
            editorWindowButton.clicked += OnOpenEditorWindowClicked;
            utilityContainer.Add(editorWindowButton);
            
            var clearSearchButton = new Button();
            clearSearchButton.text = "検索をクリア";
            clearSearchButton.style.marginLeft = 5;
            clearSearchButton.clicked += OnClearSearchClicked;
            utilityContainer.Add(clearSearchButton);
            
            Add(utilityContainer);
        }
        
        private void SetupCallbacks()
        {
            searchField.RegisterValueChangedCallback(evt => RefreshViewImmediate());
            regexToggle.RegisterValueChangedCallback(evt => OnRegexChanged());
            sortDropdown.RegisterValueChangedCallback(evt => OnSortChanged());
            sortOrderToggle.RegisterValueChangedCallback(evt => OnSortOrderChanged());
        }
        
        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            // パネルにアタッチされた時の処理（必要に応じて拡張）
        }
        
        private void OnDetachFromPanel(DetachFromPanelEvent evt)
        {
            // パネルからデタッチされた時の処理（必要に応じて拡張）
        }
        
        // イベントハンドラー
        private void OnRegexChanged()
        {
            filterState.useRegex = regexToggle.value;
            filterState.cachedRegex = null;
            filterState.cachedSearchText = "";
            RefreshViewImmediate();
        }
        
        private void OnSortChanged()
        {
            filterState.sortType = (SortType)sortDropdown.choices.IndexOf(sortDropdown.value);
            RefreshViewImmediate();
        }
        
        private void OnSortOrderChanged()
        {
            filterState.sortAscending = sortOrderToggle.value;
            sortOrderToggle.text = filterState.sortAscending ? "昇順" : "降順";
            RefreshViewImmediate();
        }
        
        private void OnAddButtonClicked()
        {
            var newData = new ResourcePathAsset.PathData();
            asset.GetPathDatas().Add(newData);
            filterState.forceRefresh = true;
            EditorUtility.SetDirty(asset);
            RefreshViewImmediate();
        }
        
        private void OnOpenEditorWindowClicked()
        {
            var window = EditorWindow.GetWindow<ResourcePathEditor>("ResourcePath Editor");
            window.Show();
            window.Focus();
        }
        
        private void OnClearSearchClicked()
        {
            searchField.value = "";
            regexToggle.value = false;
            sortDropdown.value = "なし";
            sortOrderToggle.value = true;
            RefreshViewImmediate();
        }
        
        // リフレッシュ処理
        private void RefreshViewImmediate()
        {
            if (asset == null) return;
            
            filterState.searchText = searchField?.value ?? "";
            FilterAndSort();
            UpdateCountLabel();
            
            listView?.RefreshItems();
        }
        
        // ListView用メソッド
        private VisualElement CreateListItem()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 2;
            container.style.paddingBottom = 2;
            container.style.paddingLeft = 5;
            container.style.paddingRight = 5;
            
            var itemData = new ListItemData();
            
            itemData.deleteButton = new Button();
            itemData.deleteButton.text = "✗";
            itemData.deleteButton.style.width = 25;
            itemData.deleteButton.style.height = 20;
            itemData.deleteButton.style.backgroundColor = new Color(0.8f, 0.4f, 0.4f);
            container.Add(itemData.deleteButton);
            
            itemData.keyField = new TextField();
            itemData.keyField.style.width = 180;
            itemData.keyField.style.marginLeft = 5;
            container.Add(itemData.keyField);
            
            itemData.pathField = new TextField();
            itemData.pathField.style.flexGrow = 1;
            itemData.pathField.style.marginLeft = 5;
            container.Add(itemData.pathField);
            
            container.userData = itemData;
            return container;
        }
        
        private void BindListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= filteredList.Count) return;
            
            var pathData = filteredList[index];
            var itemData = element.userData as ListItemData;
            if (itemData == null) return;
            
            // 既存のイベントをクリア
            if (itemData.deleteAction != null)
            {
                itemData.deleteButton.clicked -= itemData.deleteAction;
                itemData.deleteAction = null;
            }
            if (itemData.keyCallback != null)
            {
                itemData.keyField.UnregisterValueChangedCallback(itemData.keyCallback);
                itemData.keyCallback = null;
            }
            if (itemData.pathCallback != null)
            {
                itemData.pathField.UnregisterValueChangedCallback(itemData.pathCallback);
                itemData.pathCallback = null;
            }
            
            // 新しいイベントを設定
            itemData.deleteAction = () => {
                var allPaths = asset.GetPathDatas();
                allPaths.Remove(pathData);
                filterState.forceRefresh = true; // データ変更時は強制リフレッシュ
                EditorUtility.SetDirty(asset);
                RefreshViewImmediate();
            };
            itemData.deleteButton.clicked += itemData.deleteAction;
            
            itemData.keyField.SetValueWithoutNotify(pathData.Key ?? "");
            itemData.keyCallback = evt => {
                pathData.Key = evt.newValue;
                EditorUtility.SetDirty(asset);
            };
            itemData.keyField.RegisterValueChangedCallback(itemData.keyCallback);
            
            itemData.pathField.SetValueWithoutNotify(pathData.Path ?? "");
            itemData.pathCallback = evt => {
                pathData.Path = evt.newValue;
                EditorUtility.SetDirty(asset);
            };
            itemData.pathField.RegisterValueChangedCallback(itemData.pathCallback);
        }
        
        // ビジネスロジック
        private void FilterAndSort()
        {
            if (asset == null) return;
            
            string currentSearchText = filterState.searchText;
            
            // 初回以外で条件が同じ場合は処理をスキップ（ただし強制リフレッシュフラグが立っている場合は実行）
            if (!filterState.isFirstRefresh && !filterState.forceRefresh &&
                currentSearchText == filterState.lastProcessedSearchText &&
                filterState.sortType == filterState.lastProcessedSortType &&
                filterState.sortAscending == filterState.lastProcessedSortAscending &&
                filterState.useRegex == filterState.lastProcessedUseRegex)
            {
                return;
            }
            
            // 処理済み条件を更新
            filterState.lastProcessedSearchText = currentSearchText;
            filterState.lastProcessedSortType = filterState.sortType;
            filterState.lastProcessedSortAscending = filterState.sortAscending;
            filterState.lastProcessedUseRegex = filterState.useRegex;
            filterState.isFirstRefresh = false;
            filterState.forceRefresh = false; // 強制リフレッシュフラグをリセット
            
            var allPaths = asset.GetPathDatas();
            filteredList.Clear();
            
            // 正規表現のキャッシュ管理
            if (filterState.useRegex && currentSearchText != filterState.cachedSearchText)
            {
                filterState.cachedSearchText = currentSearchText;
                filterState.cachedRegex = null;
                
                if (!string.IsNullOrEmpty(currentSearchText))
                {
                    try
                    {
                        filterState.cachedRegex = new System.Text.RegularExpressions.Regex(
                            currentSearchText, 
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    }
                    catch (System.ArgumentException)
                    {
                        filterState.cachedRegex = null;
                    }
                }
            }
            else if (!filterState.useRegex)
            {
                filterState.cachedRegex = null;
                filterState.cachedSearchText = currentSearchText;
            }
            
            // フィルタリング
            if (string.IsNullOrEmpty(currentSearchText))
            {
                filteredList.AddRange(allPaths);
            }
            else
            {
                foreach (var pathData in allPaths)
                {
                    bool matches = false;
                    
                    if (filterState.useRegex && filterState.cachedRegex != null)
                    {
                        matches = filterState.cachedRegex.IsMatch(pathData.Key ?? "") || 
                                 filterState.cachedRegex.IsMatch(pathData.Path ?? "");
                    }
                    else
                    {
                        matches = (pathData.Key?.IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 (pathData.Path?.IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    
                    if (matches)
                    {
                        filteredList.Add(pathData);
                    }
                }
            }
            
            // ソート
            if (filterState.sortType != SortType.None)
            {
                switch (filterState.sortType)
                {
                    case SortType.Key:
                        if (filterState.sortAscending)
                            filteredList.Sort((a, b) => string.Compare(a.Key, b.Key, StringComparison.OrdinalIgnoreCase));
                        else
                            filteredList.Sort((a, b) => string.Compare(b.Key, a.Key, StringComparison.OrdinalIgnoreCase));
                        break;
                    case SortType.Path:
                        if (filterState.sortAscending)
                            filteredList.Sort((a, b) => string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase));
                        else
                            filteredList.Sort((a, b) => string.Compare(b.Path, a.Path, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }
        }
        
        private void UpdateCountLabel()
        {
            if (countLabel != null)
            {
                var allPaths = asset.GetPathDatas();
                countLabel.text = $"表示中: {filteredList.Count} / 全{allPaths.Count}エントリ";
            }
        }
        

        
        // 外部API（必要に応じて）
        public void FocusSearchField()
        {
            searchField?.Focus();
        }
    }
} 