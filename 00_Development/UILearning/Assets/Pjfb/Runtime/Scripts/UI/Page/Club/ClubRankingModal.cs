using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Club
{
    public class ClubRankingModal : ModalWindow
    {
        const string rankingStrKey = "common.ranking";
        const string rewordStrKey = "common.reword";
        enum ViewMode {
            Club,
            Reword
        }

        [SerializeField]
        private GameObject _clubListView = null;
        [SerializeField]
        private GameObject _rewordListView = null;
        
        [SerializeField]
        private TextMeshProUGUI _changeViewButtonText = null;
        [SerializeField]
        private ScrollBanner _rankScroll = null;

        ViewMode _currentMode = ViewMode.Club;


        protected async override UniTask OnOpen(CancellationToken token)
        {
            //ランクの初期化
            //todo とりあえず固定
            var rankParams = new ClubRankingRankScrollItem.Param[]{ 
                new ClubRankingRankScrollItem.Param(3),
                new ClubRankingRankScrollItem.Param(2),
                new ClubRankingRankScrollItem.Param(1),
                new ClubRankingRankScrollItem.Param(0),
             };
            _rankScroll.SetBannerDatas(rankParams);

            UpdateView(ViewMode.Club);
            await base.OnOpen(token);;
        }
        
        
        public void OnClickChangeViewButton()
        {
            UpdateView( _currentMode == ViewMode.Club ? ViewMode.Reword : ViewMode.Club );   
        }


        public void OnClickCloseButton()
        {
            
            Close();
        }


        void UpdateView( ViewMode mode ) {

            _currentMode = mode;
            var textKey = _currentMode == ViewMode.Club ? rewordStrKey : rankingStrKey;
            _changeViewButtonText.text = StringValueAssetLoader.Instance[textKey];
            _clubListView.SetActive(_currentMode == ViewMode.Club );
            _rewordListView.SetActive(_currentMode == ViewMode.Reword );

        }
    }
}
