using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{ 
    public class TrainerCardCustomizeImageScrollItem : ScrollGridItem
    {
        public class ScrollData
        {
            public ScrollData(long id, bool isNew, bool isSelect, bool isSetting, int index)
            {
                Id = id;
                IsNew = isNew;
                IsSelect = isSelect;
                IsSetting = isSetting;
                Index = index;
            }

            public long Id { get; }
            public bool IsNew { get; }
            public bool IsSelect { get; set; }
            public bool IsSetting { get; }
            public int Index { get; }
        }
        
        [SerializeField]
        private TrainerCardCustomizeThumbnailImage trainerCardCustomizeImage;

        [SerializeField]
        private GameObject newBadge;
        [SerializeField]
        private GameObject settingImage;
        [SerializeField]
        private GameObject selectImage;
        
        private ScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (ScrollData)value;
            
            newBadge.SetActive(scrollData.IsNew);
            settingImage.SetActive(scrollData.IsSetting);
            selectImage.SetActive(scrollData.IsSelect);
            
            // サムネイル画像設定
            var mProfileFrame = MasterManager.Instance.profileFrameMaster.FindData(scrollData.Id);
            trainerCardCustomizeImage.SetTexture(mProfileFrame.thumbnailImageId);
        }

        protected override void OnSelectedItem()
        {
            selectImage.SetActive(true);
            TriggerEvent(scrollData.Index);
        }
        
        protected override void OnDeselectItem()
        {
            selectImage.SetActive(false);
        }
    }
}