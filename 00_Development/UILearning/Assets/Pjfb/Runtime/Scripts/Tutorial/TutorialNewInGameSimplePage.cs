using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Battle;
using Pjfb.InGame;
using UnityEngine;

namespace Pjfb
{
    public class TutorialNewInGameSimplePage : NewInGameSimplePage
    {
        [SerializeField] private GameObject[] enableGameObjects;
        [SerializeField] private GameObject[] disableGameObjects;
        [SerializeField] private UIButton skipButton;

        private bool scenarioInGameMode = false;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var tutorialDetail = (TutorialSettings.Detail)args;
            scenarioInGameMode = tutorialDetail.scenarioInGameMode;
            skipButton.interactable = tutorialDetail.stepId == TutorialSettings.Step.RivalryBattleInGame;
            AppManager.Instance.TutorialManager.OnExecuteTutorialAction = OnExecuteTutorialAction;
            AppManager.Instance.TutorialManager.OnCompletedExecuteTutorialAction = OnCompletedExecuteTutorialAction;
            return base.OnPreOpen(null, token);
        }

        protected override void OnOpened(object args)
        {
            SetTutorialUi();
            BattleDataMediator.Instance.IsReleaseAsset = false;
            // チュートリアル用のargs入ってる かつ スタッツのリプレイは不要なので基底にはnullを固定で渡す
            base.OnOpened(null);
        }

        protected override void OnDestroy()
        {
            if (AppManager.Instance == null) return;
            AppManager.Instance.TutorialManager.OnExecuteTutorialAction = null;
            AppManager.Instance.TutorialManager.OnCompletedExecuteTutorialAction = null;
        }

        protected override void OnMatchUpActivated(bool hideSomePhrase)
        {
            OnDigestStateChangedAction(TutorialSettings.DigestTriggerType.MatchUpActivated, BattleConst.DigestType.MatchUp).Forget();
            base.OnMatchUpActivated(hideSomePhrase);
        }

        protected override void OnDigestActivated(BattleConst.DigestType type)
        {
            base.OnDigestActivated(type);
        }

        protected override void OnDigestClosed(BattleConst.DigestType type)
        {
            if (type == BattleConst.DigestType.Goal && scenarioInGameMode)
            {
                AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            }
            else
            {
                OnDigestStateChangedAction(TutorialSettings.DigestTriggerType.Out, type).Forget();
            }
            base.OnDigestClosed(type);
        }

        private async UniTask OnDigestStateChangedAction(TutorialSettings.DigestTriggerType triggerType, BattleConst.DigestType type)
        {
            if (BattleDataMediator.Instance.IsSkipToFinish)
            {
                return;
            }
            if (!AppManager.Instance.TutorialManager.ValidateDigestTypeCondition(triggerType, type))
            {
                return;
            }
            await AppManager.Instance.TutorialManager.ExecuteTutorialAction();
        }

        protected override void OnClickFooterSkipButton()
        {
            ShowSkipModal().Forget();
        }

        private async UniTask ShowSkipModal()
        {
            if (BattleManager.Instance.CurrentState == BattleBase.BattleState.Finish)
            {
                return;
            }

            if (BattleDataMediator.Instance.IsSkipToFinish)
            {
                return;
            }
            
            var modal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            if (modal != null)
            {
                // チュートリアルのモーダルが開かれている場合は閉じられるまで待機
                await modal.WaitCloseAsync();
            }   

            modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.InGameSkip, null);
            await modal.WaitCloseAsync();
            if (BattleDataMediator.Instance.IsSkipToFinish)
            {
                AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                AppManager.Instance.TutorialManager.SkipTutorialAction();
            }
        }

        private void OnExecuteTutorialAction()
        {
            // ログと演出の停止
            BattleDigestController.Instance.Pause();
            BattleUIMediator.Instance.ActivateAbilityUI.Pause();
        }

        private void OnCompletedExecuteTutorialAction()
        {
            // ログと演出の再開
            BattleUIMediator.Instance.ActivateAbilityUI.Resume();
            BattleDigestController.Instance.Resume();
        }

        private void SetTutorialUi()
        {
            if (!scenarioInGameMode)
            {
                return;
            }

            foreach (var enableGameObject in enableGameObjects)
            {
                enableGameObject.SetActive(true);
            }

            foreach (var disableGameObject in disableGameObjects)
            {
                disableGameObject.SetActive(false);
            }
        }

    }
}