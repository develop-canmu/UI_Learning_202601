using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using TMPro;
using UnityEngine;

namespace Pjfb
{

    public class ItemIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.Item; } }
        
        [SerializeField]
        private bool openDetailModal = true;
        
        [SerializeField]
        private TextMeshProUGUI countText = null;
        
        [SerializeField]
        private OmissionTextSetter omissionTextSetter = null;
        
        private long itemId = 0;

        protected override void OnSetId(long id)
        {
            base.OnSetId(id);
            itemId = id;
        }

        public void SetCount(long value)
        {
            BigValue bigValue = new BigValue(value);
            countText.text = string.Format(StringValueAssetLoader.Instance["item.count_1"], bigValue.ToDisplayString(omissionTextSetter.GetOmissionData())); 
        }
        
        public void SetCountDigitString(long value)
        {
            BigValue bigValue = new BigValue(value);
            countText.text = StringValueAssetLoader.Instance["item.count_1"].Format(bigValue.ToDisplayString(omissionTextSetter.GetOmissionData())); 
        }
        
        public void SetCount(long possessionCount, long necessaryCount)
        {
            if(possessionCount >= necessaryCount)
            {
                countText.text = string.Format(StringValueAssetLoader.Instance["item.count_2"], possessionCount, necessaryCount);
            }
            else
            {
                countText.text = string.Format(StringValueAssetLoader.Instance["item.count_3"], possessionCount, necessaryCount);
            }
        }
        
        public void SetCountDigitString(long possessionCount, long necessaryCount)
        {
            BigValue possessionCountBigValue = new BigValue(possessionCount);
            BigValue necessaryCountBigValue = new BigValue(necessaryCount);
            var possessionCountDigitString = possessionCountBigValue.ToDisplayString(omissionTextSetter.GetOmissionData());
            var necessaryCountDigitString = necessaryCountBigValue.ToDisplayString(omissionTextSetter.GetOmissionData());
            if(possessionCount >= necessaryCount)
            {
                countText.text = string.Format(StringValueAssetLoader.Instance["item.count_2"], possessionCountDigitString, necessaryCountDigitString);
            }
            else
            {
                countText.text = string.Format(StringValueAssetLoader.Instance["item.count_3"], possessionCountDigitString, necessaryCountDigitString);
            }
        }

        public void SetCountTextColor(Color color)
        {
            countText.color = color;
        }
        
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            ItemConfirmModalWindow.WindowParams data = new ItemConfirmModalWindow.WindowParams()
            {
                Id = itemId,
                onClosed = null,
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ItemConfirm, data);
        }
    }
}