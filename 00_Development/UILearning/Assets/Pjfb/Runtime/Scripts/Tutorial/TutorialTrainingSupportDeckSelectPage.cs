using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using CruFramework.Page;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class TutorialTrainingSupportDeckSelectPage : TrainingSupportDeckSelectPage
    {
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            
            if(TransitionType == PageTransitionType.Back)
            {
                return;
            }
            
            // 基底のAPIは叩かずにTutorialSettingからフレンド設定
            Arguments.DeckList = await DeckUtility.GetDeckList(DeckType.Training);
            Arguments.DeckList.DeckDataList[0].Friend = AppManager.Instance.TutorialManager.GetTutorialFriendData();
            Arguments.DeckList.DeckDataList[0].SetEmptyUCharId(0);
            
            Arguments.EquipmentDeckList = await DeckUtility.GetDeckList(DeckType.SupportEquipment);
            Arguments.EquipmentPartyNumber = AppManager.Instance.TutorialManager.GetTutorialTrainingSupportDeckNumber();

        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
        }

        public void OnSelectedCharacter()
        {
            TutorialTrainingPreparation m = (TutorialTrainingPreparation)Manager;
            m.OpenPage(TrainingPreparationPageType.SupportCharacterSelect, true, Arguments);
        }

        public new void OnNextButton()
        {
            NextAsync().Forget();
        }

        private async UniTask NextAsync()
        {
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingStartConfirm, Arguments);
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickStart);
        }

        private void OnClickStart()
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
        } 
    }
}