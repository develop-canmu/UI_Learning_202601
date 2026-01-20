using CruFramework.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Gacha
{
    public class GachaProbabilityGroupView : MonoBehaviour {
        
        public float  height => _layout.preferredHeight;
        [SerializeField]
        TextMeshProUGUI _groupName = null;
        [SerializeField]
        GachaProbabilityFrameView _frameViewPrefab = null;
        [SerializeField]
        GachaProbabilityContentView _contentViewPrefab = null;

        [SerializeField]
        LayoutGroup _layout = null;

        public void Init(GachaProbabilityGroup groupData ){

            _groupName.text = string.Format( StringValueAssetLoader.Instance["gacha.probability_group_name"], groupData.name, groupData.count);

            foreach( var frame in groupData.frames ){
                //中身がない場合は何もしない
                if( frame.contents.Count <= 0 ) {
                    continue;
                }
                var frameView = Instantiate<GachaProbabilityFrameView>(_frameViewPrefab, transform);
                frameView.Init(frame);
                var index = 0;
                foreach( var content in frame.contents ){
                    var contentView = Instantiate<GachaProbabilityContentView>(_contentViewPrefab, transform);
                    contentView.Init(index, content);
                    index++;
                }
            }
            _layout.CalculateLayoutInputHorizontal();
            _layout.CalculateLayoutInputVertical();
            _layout.SetLayoutHorizontal();
            _layout.SetLayoutVertical();
        }
        
    }
}
