using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.GUI;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableLabelMenuView : AddressableFoldoutHeaderView
    {
        private Button openLabelWindowButton = null;

        public AddressableLabelMenuView()
        {
            text = "Labels";
            
            openLabelWindowButton = new Button();
            openLabelWindowButton.text = "Open Label Window";
            // ラベルウィンドウを開く
            openLabelWindowButton.clicked += () =>
            {
                EditorWindow.GetWindow<LabelWindow>(true).Intialize(AddressableUtility.GetAddressableAssetSettings());
            };
            Add(openLabelWindowButton);
        }
        
        public override void UpdateView()
        {
        }
    }
}
