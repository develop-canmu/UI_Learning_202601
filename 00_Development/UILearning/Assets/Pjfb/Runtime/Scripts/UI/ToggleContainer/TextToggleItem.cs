using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    public class TextToggleItem : ToggleItemBase
    {
        #region SerializeField
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI text;
        
        [SerializeField]
        [ColorValue]
        private string selectedBackgroundColorId = string.Empty;
        
        [SerializeField]
        [ColorValue]
        private string selectedTextColorId = string.Empty;

        [SerializeField]
        [ColorValue]
        private string deselectedBackgroundColorId = string.Empty;
        
        [SerializeField]
        [ColorValue]
        private string deselectedTextColorId = string.Empty;
        #endregion

        #region OverrideMethods
        public override void SetDisplay(bool isSelected)
        {
            background.color = ColorValueAssetLoader.Instance[isSelected ? selectedBackgroundColorId : deselectedBackgroundColorId];
            text.color = ColorValueAssetLoader.Instance[isSelected ? selectedTextColorId : deselectedTextColorId];
            
            base.SetDisplay(isSelected);
        }
        #endregion
    }
}