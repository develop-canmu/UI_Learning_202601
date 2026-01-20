using CruFramework.UI;
using UnityEngine;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaProbabilityListButton : ScrollGridItem {
        [SerializeField]
        TextMeshProUGUI _buttonText = null;

        GachaProbabilityData _data = null;

        protected override void OnSetView(object value){
            _data = (GachaProbabilityData)value;
            _buttonText.text = _data.name;
        }

        public void OnClickButton(){
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaProbabilityDetail, _data);
        }
        
    }
}
