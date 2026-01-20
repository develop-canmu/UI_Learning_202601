using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.RecommendChara
{
    public class RecommendCharaListPage : Page
    {
        [SerializeField] private RecommendCharaSheetManager sheetManager;
        [SerializeField] private RecommendCharaSheetButton allSheetButton;
        [SerializeField] private RecommendCharaSheetButton clubSheetButton;

        #region Life Cycle

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (RecommendCharaPage.ClubDataList.Any())
            {
                ShowClubSheetButton();
            }
            else
            {
                HideClubSheetButton();
            }
            
            await sheetManager.OpenSheetAsync(RecommendCharaTabSheetType.All, null);
            await base.OnPreOpen(args, token);
        }

        private void ShowClubSheetButton()
        {
            allSheetButton.SetMultiTabView();
            clubSheetButton.gameObject.SetActive(true);
        }

        private void HideClubSheetButton()
        {
            allSheetButton.SetSingleTabView();
            clubSheetButton.gameObject.SetActive(false);
        }

        #endregion
        
        public void OnClickBack()
        {
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
    }
}
