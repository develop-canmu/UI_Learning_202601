using CruFramework.UI;
using UnityEngine;
using TMPro;

namespace Pjfb.Gacha
{
    public class GachaProbabilityFrameView : MonoBehaviour {
        
        [SerializeField]
        TextMeshProUGUI _frameName = null;
        [SerializeField]
        TextMeshProUGUI _percentage = null;

        public void Init(GachaProbabilityFrame frame ){
            _frameName.text = frame.name;
            _percentage.text = string.Format( StringValueAssetLoader.Instance["gacha.probability_total_percentage"], frame.percentage );
        }
        
    }
}
