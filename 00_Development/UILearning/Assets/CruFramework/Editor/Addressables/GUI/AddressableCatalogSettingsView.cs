using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableCatalogSettingsView : VisualElement
    {
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private TextField catalogNameField = null;
        private Toggle optimizeCatalogSizeField = null;
        
        public AddressableCatalogSettingsView(AddressableCustomSettingsObject addressableCustomSettings)
        {
            this.addressableCustomSettings = addressableCustomSettings;
            
            catalogNameField = new TextField();
            catalogNameField.label = "Catalog Name";
            Add(catalogNameField);
            
            optimizeCatalogSizeField = new Toggle();
            optimizeCatalogSizeField.label = "Optimize Catalog Size";
            Add(optimizeCatalogSizeField);
        }
        
        public void RegisterCallback(int index)
        {
            UnregisterCallback<ChangeEvent<string>, int>(OnCatalogNameChanged);
            RegisterCallback<ChangeEvent<string>, int>(OnCatalogNameChanged, index);
            
            UnregisterCallback<ChangeEvent<bool>, int>(OnOptimizeCatalogSize);
            RegisterCallback<ChangeEvent<bool>, int>(OnOptimizeCatalogSize, index);
        }
        
        public void UpdateView(int index)
        {
            AddressableCatalogSettings settings = addressableCustomSettings.CatalogSettingsList[index]; 
            catalogNameField.value = settings.CatalogName;
            optimizeCatalogSizeField.value = settings.OptimizeCatalogSize;
        }
        
        private void OnCatalogNameChanged(ChangeEvent<string> evt, int index)
        {
            AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.CatalogSettingsList[index].CatalogName = evt.newValue);
        }
        
        private void OnOptimizeCatalogSize(ChangeEvent<bool> evt, int index)
        {
            AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.CatalogSettingsList[index].OptimizeCatalogSize = evt.newValue);
        }
    }
}
