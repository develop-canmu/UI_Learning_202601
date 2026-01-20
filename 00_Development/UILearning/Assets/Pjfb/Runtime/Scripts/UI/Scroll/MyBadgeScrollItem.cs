using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pjfb
{ 
    
    public class MyBadgeScrollItem : ScrollGridItem
    {
        public class ScrollData
        {
            public long Id { get; }
            
            public int DataIndex { get; }
            
            public bool IsNew { get; }
            
            public int SelectedIndex { get; set; }
            
            public ScrollData(long id, int dataIndex, bool isNew, int selectedIndex)
            {
                Id = id;
                DataIndex = dataIndex;
                IsNew = isNew;
                SelectedIndex = selectedIndex;
            }
        }
        
        [SerializeField]
        private MyBadgeImage myBadgeImage;

        [SerializeField]
        private GameObject newBadgeImage;
        
        [SerializeField]
        private GameObject selectImage;

        [SerializeField]
        private GameObject selectRoot;
        [SerializeField]
        private TextMeshProUGUI selectIndexText;
        
        private ScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (ScrollData)value;
            
            myBadgeImage.SetTexture(scrollData.Id);
            
            newBadgeImage.SetActive(scrollData.IsNew);
            
            RefreshSelectView();
        }
        
        private void RefreshSelectView()
        {
            // 選択中の場合は選択中の画像を表示する.
            bool isSelecting = scrollData.SelectedIndex != -1;
            selectImage.SetActive(isSelecting);
            selectRoot.SetActive(isSelecting);
            
            if (isSelecting)
            {
                // indexは0から始まるので+1する.
                selectIndexText.text = (scrollData.SelectedIndex + 1).ToString();
            }
        }
        
        public void OnSelected()
        {
            TriggerEvent(scrollData.DataIndex);
            RefreshSelectView();
        }
    }
}