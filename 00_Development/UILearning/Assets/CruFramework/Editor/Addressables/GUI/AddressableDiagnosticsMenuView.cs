using System;
using System.Reflection;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableDiagnosticsMenuView : AddressableFoldoutHeaderView
    {
        private AddressableAssetSettings addressableAssetSettings = null;
        
        private Toggle logRuntimeExceptionsField = null;
        private Button openEventViewerButton = null;

        public AddressableDiagnosticsMenuView(AddressableAssetSettings addressableAssetSettings)
        {
            this.addressableAssetSettings = addressableAssetSettings;
            
            text = "Diagnostics";
            
            // EventViewerを開く
            openEventViewerButton = new Button();
            openEventViewerButton.text = "Open Event Viewer";
            openEventViewerButton.clicked += () =>
            {
                Type type = Assembly.Load("Unity.Addressables.Editor").GetType("UnityEditor.AddressableAssets.Diagnostics.ResourceProfilerWindow");
                MethodInfo mi = type.GetMethod("ShowWindow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                mi.Invoke(null, null);
            };
            Add(openEventViewerButton);

            // アセット読み込み実行時の例外を記録するか
            logRuntimeExceptionsField = new Toggle();
            logRuntimeExceptionsField.label = "Log Runtime Exceptions";
            logRuntimeExceptionsField.RegisterValueChangedCallback(evt =>
            {
                AddressableUtility.SetDirty(addressableAssetSettings, () => addressableAssetSettings.buildSettings.LogResourceManagerExceptions = evt.newValue);
            });
            Add(logRuntimeExceptionsField);
        }

        public override void UpdateView()
        {
            logRuntimeExceptionsField.value = addressableAssetSettings.buildSettings.LogResourceManagerExceptions;
        }
    }
}
