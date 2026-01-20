using CruFramework.UI;
using UnityEngine;

namespace Pjfb
{ 
    
    public class TrainerCardDisplayCharacterIconScrollItem : ScrollGridItem
    {
        public class ScrollData
        {
            /// <summary>データのインデックス</summary>
            public int Index { get; }
            
            /// <summary>表示キャラクターID</summary>
            public long DisplayCharacterId { get; }
            
            public bool IsHave { get; }
            
            public bool IsNew { get; }
            
            /// <summary>選択状態</summary>
            public bool IsSelect { get; set; }
            
            public bool IsSetting { get; }
            
            public ScrollData(int index, long displayCharacterId, bool isHave, bool isNew, bool isSelect, bool isSetting)
            {
                Index = index;
                DisplayCharacterId = displayCharacterId;
                IsHave = isHave;
                IsNew = isNew;
                IsSelect = isSelect;
                IsSetting = isSetting;
            }
        }
        
        [SerializeField]
        private DisplayCharacterIcon displayCharacterIcon = null;
        [SerializeField]
        private GameObject cover;
        [SerializeField]
        private GameObject selectedImage;
        [SerializeField]
        private GameObject settingImage;
        [SerializeField]
        private UIBadgeNotification newBadge;
        
        private ScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (ScrollData)value;
            displayCharacterIcon.SetTexture(scrollData.DisplayCharacterId);
            cover.SetActive(!scrollData.IsHave);
            selectedImage.SetActive(scrollData.IsSelect);
            settingImage.SetActive(scrollData.IsSetting);
            newBadge.SetActive(scrollData.IsNew);
        }

        protected override void OnSelectedItem()
        {
            selectedImage.SetActive(true);
            TriggerEvent(scrollData.Index);
        }

        protected override void OnDeselectItem()
        {
            selectedImage.SetActive(false);
        }
    }
}