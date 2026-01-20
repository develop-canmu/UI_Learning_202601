using CruFramework.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaProbabilityContentView : MonoBehaviour {
        
        [SerializeField]
        TextMeshProUGUI _contentName = null;

        [SerializeField]
        TextMeshProUGUI _percentage = null;

        [SerializeField]
        Image[] _bg = null;

        [SerializeField]
        Color[] _bgColor = null;
        
        [SerializeField]
        private string textColorKey = string.Empty;
        [SerializeField]
        private string highlightTextColorKey = string.Empty;

        public void Init( int index, GachaProbabilityContent content ){
            var textColor = content.picked >= 1 ? ColorValueAssetLoader.Instance[highlightTextColorKey] : ColorValueAssetLoader.Instance[textColorKey];
            
            _contentName.text = content.name;
            _percentage.text = string.Format( StringValueAssetLoader.Instance["common.percent_value"], content.percentage.ToString() );
            _contentName.color = textColor;
            _percentage.color = textColor;
            var color = _bgColor[index % 2];
            foreach( var bg in _bg ){
                bg.color = color;
            }
        }
        
    }
}
