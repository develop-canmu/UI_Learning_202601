using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableCacheMenuView : AddressableFoldoutHeaderView
    {
	    private CacheInitializationSettings cacheInitializationSettings = null;
	    
        private Toggle compressBundlesField = null;
        private TextField cacheDirectoryOverrideField = null;
        private Toggle limitCacheSizeField = null;
        private LongField maximumCacheSizeField = null;

        public AddressableCacheMenuView(CacheInitializationSettings cacheInitializationSettings)
        {
			this.cacheInitializationSettings = cacheInitializationSettings;
			
	        text = "Cache";
            
            compressBundlesField = new Toggle();
            compressBundlesField.label = "Compress Bundles";
            // キャッシュに保存されるアセットバンドルのLZ4への再圧縮を有効にする
			compressBundlesField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(cacheInitializationSettings, () => cacheInitializationSettings.Data.CompressionEnabled = evt.newValue);
			});
            Add(compressBundlesField);
            
            cacheDirectoryOverrideField = new TextField();
            cacheDirectoryOverrideField.label = "Cache Directory Override";
            // キャッシュの保存場所
			cacheDirectoryOverrideField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(cacheInitializationSettings, () => cacheInitializationSettings.Data.CacheDirectoryOverride = evt.newValue);
			});
            Add(cacheDirectoryOverrideField);
            
            limitCacheSizeField = new Toggle();
            limitCacheSizeField.label = "Limit Cache Size";
            // キャッシュの最大サイズを指定するか
			limitCacheSizeField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(cacheInitializationSettings, () => cacheInitializationSettings.Data.LimitCacheSize = evt.newValue);
				maximumCacheSizeField.style.display = cacheInitializationSettings.Data.LimitCacheSize ? DisplayStyle.Flex : DisplayStyle.None;
			});
            Add(limitCacheSizeField);
            
            maximumCacheSizeField = new LongField();
            maximumCacheSizeField.label = "Maximum Cache Size (in MB)";
            // キャッシュの最大サイズ
			maximumCacheSizeField.RegisterValueChangedCallback(evt =>
			{
				AddressableUtility.SetDirty(cacheInitializationSettings, () => cacheInitializationSettings.Data.MaximumCacheSize = evt.newValue * (1024 * 1024));
			});
            Add(maximumCacheSizeField);
        }

		public override void UpdateView()
		{
			compressBundlesField.value = cacheInitializationSettings.Data.CompressionEnabled;
			cacheDirectoryOverrideField.value = cacheInitializationSettings.Data.CacheDirectoryOverride;
			limitCacheSizeField.value = cacheInitializationSettings.Data.LimitCacheSize;
			maximumCacheSizeField.style.display = cacheInitializationSettings.Data.LimitCacheSize ? DisplayStyle.Flex : DisplayStyle.None;
			maximumCacheSizeField.value = cacheInitializationSettings.Data.MaximumCacheSize / (1024 * 1024);
		}
    }
}
