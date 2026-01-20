using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableGroupMenuView : AddressableFoldoutHeaderView
    {
        private AddressableAssetSettings addressableAssetSettings = null;
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private AddressableBundledAssetSchemaSettingsView bundledAssetSchemaSettingsView = null;
        private AddressableListView groupSettingsListView = null;

        private ToolbarSearchField searchField = null;
        
        public AddressableGroupMenuView(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings)
        {
            this.addressableAssetSettings = addressableAssetSettings;
            this.addressableCustomSettings = addressableCustomSettings;
            
            text = "Group";
            
            Foldout foldout = new Foldout();
            foldout.text = "Default Bundled Asset Schema";
            Add(foldout);
            
            bundledAssetSchemaSettingsView = new AddressableBundledAssetSchemaSettingsView(addressableCustomSettings);
            bundledAssetSchemaSettingsView.RegisterCallback(addressableCustomSettings.DefaultBundledAssetSchemaSettings);
            bundledAssetSchemaSettingsView.UpdateView(addressableCustomSettings.DefaultBundledAssetSchemaSettings);
            foldout.Add(bundledAssetSchemaSettingsView);
            
            groupSettingsListView = new AddressableListView();
            
            // 検索
            searchField = new ToolbarSearchField();
            searchField.RegisterValueChangedCallback(x => UpdateView());
            // 右寄せ
            searchField.style.marginLeft = new StyleLength(StyleKeyword.Auto);
            searchField.style.minWidth = 200f;
            groupSettingsListView.Header.Insert(0, searchField);
            
            groupSettingsListView.Header.text = "Group Settings List";
            // 最大表示サイズセット
            groupSettingsListView.Body.style.maxHeight = 800f;
            // スクロール要素の生成
            groupSettingsListView.Body.makeItem = () => new AddressableGroupRuleViewContainer(addressableAssetSettings, addressableCustomSettings);
            // スクロール要素の紐付け
            groupSettingsListView.Body.bindItem = (element, index) =>
            {
                // 設定ファイル
                AddressableGroupRule groupRule = (AddressableGroupRule) groupSettingsListView.Body.itemsSource[index];
                // ビューコンテナー
                AddressableGroupRuleViewContainer viewContainer = (AddressableGroupRuleViewContainer)element;

                // 別データが変換されないように一応変えとく
                index = -1;
                
                // 元のリストのIndexに変換
                for (int i = 0; i < addressableCustomSettings.GroupRuleList.Count; i++)
                {
                    if (this.addressableCustomSettings.GroupRuleList[i] == groupRule)
                    {
                        index = i;
                        break;
                    }
                }
                
                // コンテナ内のビューを全て非表示
                foreach (VisualElement childElement in viewContainer.Children())
                {
                    childElement.style.display = DisplayStyle.None;
                }
                
                // 型によって表示内容切り替え
                AddressableGroupRuleView view = null;
                if(groupRule is AddressableGroupRuleByDirectory)
                {
                    view = viewContainer.DirectoryRuleView;
                }
                else if(groupRule is AddressableGroupRuleByRegex)
                {
                    view = viewContainer.RegexRuleView;
                }
                view.style.display = DisplayStyle.Flex;
                view.RegisterCallback(index);
                view.UpdateView(index);
            };
            // +ボタン押下
            groupSettingsListView.Footer.AddButton.clicked += () =>
            {
                GenericDropdownMenu genericDropdownMenu = new GenericDropdownMenu();
                // AddressableGroupSettingsBaseを実装しているクラスを取得
                Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(v => v.IsSubclassOf(typeof(AddressableGroupRule))).ToArray();
                // メニューに追加
                foreach (Type type in types)
                {
                    genericDropdownMenu.AddItem(type.Name, false, obj =>
                    {
                        Type type = (Type)obj;
                        int selectedIndex = groupSettingsListView.Body.selectedIndex;
                        // 何も選択されてない時、要素数がオーバーしているなら末尾に追加
                        if (selectedIndex < 0 || selectedIndex > addressableCustomSettings.GroupRuleList.Count)
                        {
                            selectedIndex = addressableCustomSettings.GroupRuleList.Count;
                        }
                        // 指定位置に追加
                        AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList.Insert(selectedIndex, (AddressableGroupRule)Activator.CreateInstance(type)));
                        UpdateView();
                    },
                    type);
                }
                Rect position = groupSettingsListView.Footer.AddButton.worldBound;
                position.y -= types.Length * 40;
                genericDropdownMenu.DropDown(position, this, false);
            };
            // -ボタン押下
            groupSettingsListView.Footer.RemoveButton.clicked += () =>
            {
                // 要素数が0
                if(addressableCustomSettings.GroupRuleList.Count <= 0) return;
                
                int index = groupSettingsListView.Body.selectedIndex;
                // アイテムを選択してない場合は末尾の要素
                if(index < 0)
                {
                    index = addressableCustomSettings.GroupRuleList.Count-1;
                }
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.GroupRuleList.RemoveAt(index));
                UpdateView();
            };
            // 要素入れ替え
            groupSettingsListView.Body.itemIndexChanged += (i1, i2) =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () =>
                {
                    // キャッシュ
                    AddressableGroupRule temp = addressableCustomSettings.GroupRuleList[i1];
                    // 削除
                    addressableCustomSettings.GroupRuleList.RemoveAt(i1);
                    // 追加
                    addressableCustomSettings.GroupRuleList.Insert(i2, temp);
                });
            };
            Add(groupSettingsListView);
        }

        public override void UpdateView()
        {
            // 検索を実行するか
            bool hasSearch = searchField.value.Length > 0;

            if (hasSearch == false)
            {
                groupSettingsListView.Body.itemsSource = new List<AddressableGroupRule>(addressableCustomSettings.GroupRuleList);
            }
            else
            {
                List<AddressableGroupRule> searchResult = new List<AddressableGroupRule>();
                
                // 小文字に統一
                string searchText = searchField.value.ToLower();
                foreach (var groupRule in addressableCustomSettings.GroupRuleList)
                {
                    if (groupRule.Description.ToLower().Contains(searchText))
                    {
                        searchResult.Add(groupRule);
                    }
                }

                groupSettingsListView.Body.itemsSource = searchResult;
            }

            groupSettingsListView.Body.RefreshItems();
        }
    }
}
