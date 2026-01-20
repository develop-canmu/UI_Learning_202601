using System;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Gacha;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using CruFramework.ResourceManagement;

namespace Pjfb
{
    public class TutorialTrainingPreparation : TrainingPreparation
    {
        protected override string GetAddress(TrainingPreparationPageType page)
        {
            return $"Prefabs/UI/Page/TutorialTrainingPreparation/{page}Page.prefab";
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialTrainingPreparation);
            await TrainingUtility.LoadConfig();
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.Training);
            await OpenPageAsync(TrainingPreparationPageType.MenuSelect, true, new TrainingPreparationArgs());
        }
        protected override void OnClosed()
        {
            ResetTutorialDeckData().Forget();
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialTrainingPreparation);
            base.OnClosed();
        }
        
        private async UniTask ResetTutorialDeckData()
        {
            var deckList = await DeckUtility.GetDeckList(DeckType.Training);
            if (deckList != null)
            {
                deckList.DeckDataList[0].Friend = null;
            }
        }
    }
}