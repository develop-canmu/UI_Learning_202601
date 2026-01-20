using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.News
{
    public class CommonToggleItem : ToggleItemBase
    {
        #region SerializeFields
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI text;
        
        [SerializeField] private Sprite selectedBackgroundSprite;
        [SerializeField] private Sprite deselectedBackgroundSprite;
        [SerializeField] private Color selectedTextColor;
        [SerializeField] private Color deselectedTextColor;
        #endregion
        
        #region OverrideMethods
        public override void SetDisplay(bool isSelected)
        {
            background.sprite = isSelected ? selectedBackgroundSprite : deselectedBackgroundSprite;
            text.color = isSelected ? selectedTextColor : deselectedTextColor;

            base.SetDisplay(isSelected);
        }
        #endregion
    }
}