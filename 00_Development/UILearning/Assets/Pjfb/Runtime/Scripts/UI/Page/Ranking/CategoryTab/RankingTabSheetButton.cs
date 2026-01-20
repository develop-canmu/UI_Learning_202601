using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    
    public class RankingTabSheetButton : SheetSwitchButton<RankingTabSheetManager,RankingTabSheetType>
    {
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private GameObject buttonFilter = null;
        
        public void ShowFilter(bool isShow)
        {
            buttonFilter.SetActive(isShow);
            button.interactable = !isShow;
        }
    }
}