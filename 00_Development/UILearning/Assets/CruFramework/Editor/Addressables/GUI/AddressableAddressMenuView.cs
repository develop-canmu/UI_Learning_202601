using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableAddressMenuView : AddressableFoldoutHeaderView
    {
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private Toggle includeExtensionField = null;
        private AddressableListView excludePathListView = null;
        
        public AddressableAddressMenuView(AddressableCustomSettingsObject addressableCustomSettings)
        {
            this.addressableCustomSettings = addressableCustomSettings; 
            
            text = "Address";
            
            includeExtensionField = new Toggle();
            includeExtensionField.label = "Include Extension In Address";
            includeExtensionField.RegisterValueChangedCallback(evt =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.IncludeExtensionInAddress = evt.newValue);
            });
            Add(includeExtensionField);
            
            // スクロール要素の紐付け
            excludePathListView = new AddressableListView();
            excludePathListView.Header.text = "Address Replacement List";
            excludePathListView.Body.makeItem = () => new AddressableAddressReplacementView(addressableCustomSettings);
            // 要素の紐付け
            excludePathListView.Body.bindItem = (element, index) =>
            {
                AddressableAddressReplacementView replacementView = (AddressableAddressReplacementView)element;
                replacementView.RegisterCallback(index);
                replacementView.UpdateView(index);
            };
            // +ボタン押下
            excludePathListView.Footer.AddButton.clicked += () =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.AddressReplacementList.Add(new AddressableAddressReplacement()));
                UpdateView();
            };
            // -ボタン押下
            excludePathListView.Footer.RemoveButton.clicked += () =>
            {
                // 要素数が0
                if(addressableCustomSettings.AddressReplacementList.Count <= 0) return;
                
                int index = excludePathListView.Body.selectedIndex;
                // アイテムを選択してない場合は末尾の要素
                if(index < 0)
                {
                    index = addressableCustomSettings.AddressReplacementList.Count-1;
                }
                AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.AddressReplacementList.RemoveAt(index));
                UpdateView();
            };
            // 並び替え通知
            excludePathListView.Body.itemIndexChanged += (i1, i2) =>
            {
                AddressableUtility.SetDirty(addressableCustomSettings, () =>
                {
                    AddressableAddressReplacement temp = addressableCustomSettings.AddressReplacementList[i1];
                    // 削除
                    addressableCustomSettings.AddressReplacementList.RemoveAt(i1);
                    // 追加
                    addressableCustomSettings.AddressReplacementList.Insert(i2, temp);
                });
            };
            Add(excludePathListView);
        }
        
        public override void UpdateView()
        {
            includeExtensionField.value = addressableCustomSettings.IncludeExtensionInAddress;
            excludePathListView.Body.itemsSource = new List<AddressableAddressReplacement>(addressableCustomSettings.AddressReplacementList);
            excludePathListView.Body.RefreshItems();
        }
    }
}
