using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;

namespace Pjfb.Club
{
    public class ClubRankingRankScrollItem : ScrollGridItem
    {
        public class Param {
            public int id = 0;
            public Param( int id ){
                this.id = id;
            }
        }
        
        [SerializeField]
        private Image _icon = null;
        [SerializeField]
        private TextMeshProUGUI _text = null;
        
        protected override void OnSetView(object value){
            var param = (Param)value;
            UpdateView(param.id).Forget();
        }

        async UniTask UpdateView(int id){
            _text.text = StringValueAssetLoader.Instance["common.rank"];
            var sprite = await ClubUtility.LoadRankIcon(id, this.GetCancellationTokenOnDestroy());
            _icon.sprite = sprite;
        }
    }
}
