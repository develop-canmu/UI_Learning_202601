#if !PJFB_REL

using CruFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.DebugPage
{
    public class DebugChara3dScrollItem : ScrollGridItem
    {
        public class Data
        {
            public long mCharaId { get; }
            public bool canMultipleSelect { get; private set; }
            public bool isSelected { get; private set; }
            public string charaName { get; }

            public Data(long mCharaId, bool canMultipleSelect, bool isSelected, string charaName)
            {
                this.mCharaId = mCharaId;
                this.canMultipleSelect = canMultipleSelect;
                this.isSelected = isSelected;
                this.charaName = charaName;
            }
            
            public void SetSelected(bool isSelected)
            {
                this.isSelected = isSelected;
            }
            public void SetSelectable(bool canMultipleSelect)
            {
                this.canMultipleSelect = canMultipleSelect;
            }
        }
        
        public class CommonData
        {
            public bool IsMultipleSelect = false;
        }
        
        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image highlightImage;
        
        private Data data;
        
        private readonly Color highlightColor = ColorUtility.TryParseHtmlString("#8e9ffa", out Color color) ? color : Color.cyan;
        private readonly Color defaultColor = Color.white;
        
        protected override void OnSetView(object value)
        {
            data = (Data)value;
            nameText.text = data.charaName;

            if (data.canMultipleSelect)
            {
                // ハイライト
                highlightImage.color = data.isSelected ? highlightColor : defaultColor;
            }
        }

        public void OnClick()
        {
            TriggerEvent(data);
        }
    }
}

#endif