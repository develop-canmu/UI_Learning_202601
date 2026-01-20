using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class CharacterPieceIcon : ItemIconBase
    {
        public override ItemIconType IconType { get { return ItemIconType.CharacterPiece; } }
        
        [SerializeField] private TextMeshProUGUI countText = null;
        [SerializeField] private OmissionTextSetter omissionTextSetter = null;
        [SerializeField] private bool openDetailModal = true;
        [SerializeField] private GameObject extraRoot = null;
        private long mCharaId;
        public void SetCount(long value)
        {
            countText.text = string.Format(StringValueAssetLoader.Instance["item.count_1"], value); 
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

        protected override void OnSetId(long id)
        {
            base.OnSetId(id);
            mCharaId = id;
            
            // mChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(id);
            // Ex
            extraRoot.SetActive(mChar.isExtraSupport);
            
        }
        
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            CharacterPieceDetailModal.Open(new CharacterPieceDetailModal.WindowParams{MCharaId = mCharaId});
        }
    }
}