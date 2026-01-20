using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableAddressReplacementView : VisualElement
    {
        private AddressableCustomSettingsObject addressableCustomSettings = null;
        
        private TextField srcField = null;
        private TextField destField = null;
        
        public AddressableAddressReplacementView(AddressableCustomSettingsObject addressableCustomSettings)
        {
            this.addressableCustomSettings = addressableCustomSettings;
            
            style.flexDirection = FlexDirection.Row;
            
            srcField = new TextField();
            srcField.style.flexGrow = 1;
            srcField.style.maxWidth = new StyleLength(new Length(50, LengthUnit.Percent));
            Add(srcField);
            
            Label label = new Label();
            label.text = ">";
            label.style.alignSelf = Align.Center;
            Add(label);
            
            destField = new TextField();
            destField.style.flexGrow = 1;
            destField.style.maxWidth = new StyleLength(new Length(50, LengthUnit.Percent));
            Add(destField);
        }
        
        public void RegisterCallback(int index)
        {
            srcField.UnregisterCallback<ChangeEvent<string>, int>(OnSrcChanged);
            srcField.RegisterCallback<ChangeEvent<string>, int>(OnSrcChanged, index);
            
            destField.UnregisterCallback<ChangeEvent<string>, int>(OnDestChanged);
            destField.RegisterCallback<ChangeEvent<string>, int>(OnDestChanged, index);
        }
        
        public void UpdateView(int index)
        {
            srcField.value = addressableCustomSettings.AddressReplacementList[index].SrcValue;
            destField.value = addressableCustomSettings.AddressReplacementList[index].DestValue;
        }
        
        private void OnSrcChanged(ChangeEvent<string> evt, int index)
        {
            AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.AddressReplacementList[index].SrcValue = evt.newValue);
        }
        
        private void OnDestChanged(ChangeEvent<string> evt, int index)
        {
            AddressableUtility.SetDirty(addressableCustomSettings, () => addressableCustomSettings.AddressReplacementList[index].DestValue = evt.newValue);
        }
    }
}
