using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableDownloadMenuView : AddressableFoldoutHeaderView
    {
        private AddressableAssetSettings addressableAssetSettings = null;
        
        private SliderInt maxConcurrentWebRequestsField = null;
        private IntegerField catalogDownloadTimeoutField = null;
        
        public AddressableDownloadMenuView(AddressableAssetSettings addressableAssetSettings)
        {
            this.addressableAssetSettings = addressableAssetSettings;
            
            text = "Downloads";
            
            maxConcurrentWebRequestsField = new SliderInt();
            maxConcurrentWebRequestsField.label = "Max Concurrent Web Requests";
            maxConcurrentWebRequestsField.showInputField = true;
            maxConcurrentWebRequestsField.lowValue = 1;
            maxConcurrentWebRequestsField.highValue = 1024;
            // 最大同時リクエスト数
            maxConcurrentWebRequestsField.RegisterValueChangedCallback(evt =>
            {
                AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.MaxConcurrentWebRequests = Mathf.Clamp(evt.newValue, 1, 1024));
            });
            Add(maxConcurrentWebRequestsField);
            
            catalogDownloadTimeoutField = new IntegerField();
            catalogDownloadTimeoutField.label = "Catalog Download Timeout";
            // カタログファイルのタイムアウト時間
            catalogDownloadTimeoutField.RegisterValueChangedCallback(evt =>
            {
                AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.CatalogRequestsTimeout = evt.newValue);
            });
            Add(catalogDownloadTimeoutField);
        }

        public override void UpdateView()
        {
            maxConcurrentWebRequestsField.value = addressableAssetSettings.MaxConcurrentWebRequests;
            catalogDownloadTimeoutField.value = addressableAssetSettings.CatalogRequestsTimeout;
        }
    }
}
