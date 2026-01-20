using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Addressable
{
    public class AddressableListViewFooter : VisualElement
    {
        private Button addButton = null;
        public Button AddButton
        {
            get { return addButton; }
        }
        
        private Button removeButton = null;
        public Button RemoveButton
        {
            get { return removeButton; }
        }
        
        public AddressableListViewFooter()
        {
            style.height = 22;
            style.backgroundColor = new Color(70, 70, 70, 255) / 255f;
            style.alignSelf = Align.FlexEnd;
            style.flexDirection = FlexDirection.Row;
            style.SetMargin(0, 14, 0, 0);
            style.SetPadding(0, 4, 0, 4);
            style.SetBorderWidth(0, 1, 1, 1);
            style.SetBorderRadius(0, 0, 3, 3);
            style.SetBorderColor(new Color(26, 26, 26, 255) / 255f);
            
            addButton = new Button();
            addButton.text = "+";
            SetButtonStyle(addButton);    
            Add(addButton);
            
            removeButton = new Button();
            removeButton.text = "-";
            SetButtonStyle(removeButton);    
            Add(removeButton);
        }
        
        private void SetButtonStyle(VisualElement element)
        {
            Color defaultBackgroundColor = new Color(70, 70, 70, 255) / 255f;
            Color mouseOverBackgroundColor = new Color(88, 88, 88, 255) / 255f;
            element.style.width = 22;
            element.style.color = new Color(238, 238, 238, 255) / 255f;
            element.style.backgroundColor = defaultBackgroundColor;
            element.style.SetMargin(0);
            element.style.SetPadding(1, 6, 1, 6);
            element.style.SetBorderWidth(0);
            element.style.fontSize = 18;
            element.style.unityFontStyleAndWeight = FontStyle.Bold;
            element.style.unityTextAlign = TextAnchor.MiddleCenter;
            element.RegisterCallback<MouseOverEvent>(evt =>
            {
                element.style.backgroundColor = mouseOverBackgroundColor;
            });
            element.RegisterCallback<MouseOutEvent>(evt =>
            {
                element.style.backgroundColor = defaultBackgroundColor;
            });
        }
    }
}
