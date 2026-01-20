using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableEditorWindow : EditorWindow
    {
        private AddressableAssetSettings addressableAssetSettings = null;
        private CacheInitializationSettings cacheInitializationSettings = null;
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private AddressableDiagnosticsMenuView diagnosticsMenuView = null;
        private AddressableCatalogMenuView catalogMenuView = null;
        private AddressableDownloadMenuView downloadMenuView = null;
        private AddressableBuildMenuView buildMenuView = null;
        private AddressableCacheMenuView cacheMenuView = null;
        private AddressableLabelMenuView labelMenuView = null;
        private AddressableAddressMenuView addressMenuView = null;
        private AddressableGroupMenuView groupMenuView = null;
        // TODO Profile, Hosting, Analyze
        
        [MenuItem("CruFramework/Addressables/Editor")]
        public static AddressableEditorWindow Open()
        {
            return GetWindow<AddressableEditorWindow>(nameof(AddressableEditorWindow), true, typeof(SceneView));
        }

        private void OnEnable()
        {
            addressableAssetSettings = AddressableUtility.GetAddressableAssetSettings();
            cacheInitializationSettings = AddressableUtility.GetCacheInitializationSettings();
            addressableCustomSettings = AddressableUtility.GetAddressableCustomSettings();

            // スクロールできるようにする
            ScrollView scrollView = new ScrollView();
            // 横方向のスクロールはできないないように
            scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            // 要素を追加
            rootVisualElement.Add(scrollView);
            
            // デバッグ関連
            diagnosticsMenuView = new AddressableDiagnosticsMenuView(addressableAssetSettings);
            scrollView.Add(diagnosticsMenuView);
            
            // ダウンロード関連
            downloadMenuView = new AddressableDownloadMenuView(addressableAssetSettings);
            scrollView.Add(downloadMenuView);

            // ビルド関連
            buildMenuView = new AddressableBuildMenuView(addressableAssetSettings);
            scrollView.Add(buildMenuView);

            // キャッシュ関連
            cacheMenuView = new AddressableCacheMenuView(cacheInitializationSettings);
            scrollView.Add(cacheMenuView);
            
            // カタログ関連
            catalogMenuView = new AddressableCatalogMenuView(addressableAssetSettings, addressableCustomSettings);
            scrollView.Add(catalogMenuView);

            // アドレス関連
            addressMenuView = new AddressableAddressMenuView(addressableCustomSettings);
            scrollView.Add(addressMenuView);
            
            // ラベル関連
            labelMenuView = new AddressableLabelMenuView();
            scrollView.Add(labelMenuView);

            // グループ関連
            groupMenuView = new AddressableGroupMenuView(addressableAssetSettings, addressableCustomSettings);
            scrollView.Add(groupMenuView);

            UpdateView();
            Undo.undoRedoPerformed += UndoRedoCallback; 
        }
        
        private void OnDestroy()
        {
            Undo.undoRedoPerformed -= UndoRedoCallback;
        }

        private void UndoRedoCallback()
        {
            UpdateView();
        }
        
        private void UpdateView()
        {
            // diagnostics 
            diagnosticsMenuView.UpdateView();
            // catalog
            catalogMenuView.UpdateView();
            // download
            downloadMenuView.UpdateView();
            // build
            buildMenuView.UpdateView();
            // cache
            cacheMenuView.UpdateView();
            // label
            labelMenuView.UpdateView();
            // address
            addressMenuView.UpdateView();
            // group
            groupMenuView.UpdateView();
        }
    }
}
