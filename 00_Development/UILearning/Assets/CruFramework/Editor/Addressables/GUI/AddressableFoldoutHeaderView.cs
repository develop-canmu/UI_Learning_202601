using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public abstract class AddressableFoldoutHeaderView : Foldout
    {
        public AddressableFoldoutHeaderView()
        {
            style.marginBottom = 1;
            
            Toggle toggle = this.Q<Toggle>();
            toggle.style.height = 24;
            toggle.style.backgroundColor = new Color(60, 60, 60, 255) / 255;
            toggle.style.SetMargin(0);
            toggle.style.SetBorderColor(new Color(32, 32, 32, 255) / 255);
            toggle.style.SetBorderWidth(1, 0, 1, 0);

            VisualElement check = toggle.Q<VisualElement>("unity-checkmark");
            check.style.alignSelf = Align.Center;
            
            contentContainer.style.marginTop = contentContainer.style.marginBottom = 6;
            
            RegisterCallback<MouseEnterEvent>(evt =>
            {
                UpdateView();
            });
        }

        public abstract void UpdateView();
    }
}
