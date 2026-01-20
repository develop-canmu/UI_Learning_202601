using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.AddressableAssets.Settings;

namespace CruFramework.Editor.Addressable
{
    public class AddressableCatalogMenuView : AddressableFoldoutHeaderView
    {
        private AddressableAssetSettings addressableAssetSettings = null;
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private Toggle compressLocalCatalogField = null;
        private AddressableListView catalogSettingsListView = null;
        
        public AddressableCatalogMenuView(AddressableAssetSettings addressableAssetSettings, AddressableCustomSettingsObject addressableCustomSettings)
        {
            this.addressableAssetSettings = addressableAssetSettings;
            this.addressableCustomSettings = addressableCustomSettings;
            
            text = "Catalog";
            
            catalogSettingsListView = new AddressableListView();
            catalogSettingsListView.Header.text = "Catalog Settings List";
            // スクロール要素の生成
            catalogSettingsListView.Body.makeItem = () => new AddressableCatalogSettingsView(addressableCustomSettings);
            // スクロール要素の紐付け
            catalogSettingsListView.Body.bindItem = (element, index) =>
            {
                AddressableCatalogSettingsView view = (AddressableCatalogSettingsView)element;
                view.RegisterCallback(index);
                view.UpdateView(index);
            };
            catalogSettingsListView.Footer.AddButton.clicked += () =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.CatalogSettingsList.Add(new AddressableCatalogSettings()));
                UpdateView();
            };
            catalogSettingsListView.Footer.RemoveButton.clicked += () =>
            {
                // 要素数が0
                if(addressableCustomSettings.CatalogSettingsList.Count <= 0) return;
                
                int index = catalogSettingsListView.Body.selectedIndex;
                // アイテムを選択してない場合は末尾の要素
                if(index < 0)
                {
                    index = addressableCustomSettings.CatalogSettingsList.Count-1;
                }
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.CatalogSettingsList.RemoveAt(index));
                UpdateView();
            };
            // 要素入れ替え
            catalogSettingsListView.Body.itemIndexChanged += (i1, i2) =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () =>
                {
                    // キャッシュ
                    AddressableCatalogSettings temp = addressableCustomSettings.CatalogSettingsList[i1];
                    // 削除
                    addressableCustomSettings.CatalogSettingsList.RemoveAt(i1);
                    // 追加
                    addressableCustomSettings.CatalogSettingsList.Insert(i2, temp);
                });
            };
            Add(catalogSettingsListView);
        }

        public override void UpdateView()
        {
            catalogSettingsListView.Body.itemsSource = addressableCustomSettings.CatalogSettingsList;
            catalogSettingsListView.Body.RefreshItems();
        }
    }
}
