using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Logger = CruFramework.Logger;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.UserData;
using System.Threading;

namespace Pjfb
{
    public class TutorialBaseCharaListPage : BaseCharaListPage
    {
        
        private long tutorialBaseCharaId = 0;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            tutorialBaseCharaId = AppManager.Instance.TutorialManager.GetReleaseCharaId();
            characterScroll.Scroll.AlignmentReverse = true;
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            return base.OnPreOpen(args, token);
        }

        private void OnClickPieceToChara()
        {
            OpenConfirmAsync().Forget();
        }

        private async UniTask OpenConfirmAsync()
        {
            var piece = UserDataManager.Instance.charaPiece.Find(tutorialBaseCharaId)?.value ?? 0;
            var modalData = new PieceToCharaConfirmModal.Data(tutorialBaseCharaId, piece, 0, null);
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.PieceToCharaConfirm, modalData);
            AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickConfirm).Forget();
        }

        private void OnClickConfirm()
        {
            CharaPieceToChara().Forget();
        }
        
        private async UniTask CharaPieceToChara()
        {
            var stepIds = new []{(long)TutorialSettings.Step.Strengthen};
            await AppManager.Instance.TutorialManager.CharaPieceToCharaAsync(stepIds);
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            await WaitCloseTopModelAsync();
        }

        protected override async UniTask OpenDetailModal(CharacterScrollData data)
        {
            CruFramework.Page.ModalWindow modalWindow =
                await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.BaseCharacterDetail,
                    new BaseCharaDetailModalParams(data.SwipeableParams, false, true));
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction(OnClickPieceToChara);
            await modalWindow.WaitCloseAsync();
            characterScroll.SetUserCharacterList();
            characterScroll.SetCharacterList();
            Refresh();
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            await characterGetEffect.PlayAsync(tutorialBaseCharaId);
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction();
        }

        private async UniTask WaitCloseTopModelAsync()
        {
            var window = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            window.Close();
            await window.WaitCloseAsync();
        }
        
    }
}