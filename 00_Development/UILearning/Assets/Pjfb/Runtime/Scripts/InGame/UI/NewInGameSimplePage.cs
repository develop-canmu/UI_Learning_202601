using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Pjfb.Battle;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.InGame
{
    public class NewInGameSimplePage : Page
    {
        [SerializeField] protected BattleLogMessageScroll logScroller;
        [SerializeField] private NewInGameMatchUpUi matchUpUi;
        [SerializeField] private InGameActivateAbilityUI activateAbilityUI;
        [SerializeField] private NewInGameDigestUI digestUi;
        [SerializeField] private NewInGameDialogueUI dialogueUI;
        [SerializeField] private InGameFieldRadarUI radarUI;
        [SerializeField] private NewInGameHeaderUI headerUI;
        [SerializeField] private NewInGameFooterUI footerUI;
        [SerializeField] private GameObject blackOverLay;

        private void Awake()
        {
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetReference();
            if (args != null)
            {
                BattleDigestController.Instance.ForceQuitCurrentDigest();
                if (args is List<BattleLog>)
                {
                    BattleLogMediator.Instance.BattleLogs = (List<BattleLog>)args;
                    BattleLogMediator.Instance.AddDigestLogByReplayLogs(BattleLogMediator.Instance.BattleLogs);
                    OnSetBattleData();
                    BattleDataMediator.Instance.SetAsReplayGameMode();
                }
                else
                {
                    var index = (int)args;
                    BattleLogMediator.Instance.PrepareForReplay(index);
                    if (index >= 0)
                    {
                        BattleDataMediator.Instance.SetAsReplayDigestMode();
                        BattleUIMediator.Instance.HeaderUI.SetAsReplayMode();
                        BattleUIMediator.Instance.FooterUI.SetAsReplayMode();
                    }
                    else
                    {
                        OnSetBattleData();
                        BattleDataMediator.Instance.SetAsReplayGameMode();
                    }
                }

                BattleUIMediator.Instance.DialogueUi.gameObject.SetActive(true);
            }
            
            if (BattleDataMediator.Instance.IsSkipToFinishWithoutView)
            {
                BattleUIMediator.Instance.SetNonActiveAllElements();
            }
            else
            {
                BattleUIMediator.Instance.SetActiveAllElements();
            }

            // リーグマッチのリプレイはログからの再生になるのでオートスワイプボタンは隠す
            if (BattleDataMediator.Instance.BattleType == BattleConst.BattleType.ReplayLeagueMatch)
            {
                BattleUIMediator.Instance.FooterUI.HideAutoSwipeButton();
            }

            if (!BattleDataMediator.Instance.Is2DFieldViewMode)
            {
                BattleUIMediator.Instance.RadarUI.ResetCharacterData(BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.PlayerSide], BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.EnemySide]);                
            }
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);

            if (BattleDataMediator.Instance.IsReplayMode)
            {
                BattleEventDispatcher.Instance.OnAddLogCallback();
            }
        }

        private async UniTask LoadEssentialAsset(Action callback)
        {
            await BattleUIMediator.Instance.RadarUI.LoadCharacterModel(PageResourceLoadUtility.resourcesLoader, BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.PlayerSide], BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.EnemySide]);
            callback?.Invoke();
        }

        protected override void OnOpened(object args)
        {
            SetEvent();
            LoadEssentialAsset(() => BattleManager.Instance.StartBattle()).Forget();

            base.OnOpened(args);
        }

        private void SetReference()
        {
            BattleUIMediator.Instance.LogMessageScroller = logScroller;
            BattleUIMediator.Instance.MatchUpUi = matchUpUi;
            BattleUIMediator.Instance.ActivateAbilityUI = activateAbilityUI;
            BattleUIMediator.Instance.DialogueUi = dialogueUI;
            BattleUIMediator.Instance.RadarUI = radarUI;
            BattleUIMediator.Instance.HeaderUI = headerUI;
            BattleUIMediator.Instance.FooterUI = footerUI;
            BattleUIMediator.Instance.BlackOverLay = blackOverLay;

            BattleUIMediator.Instance.SetVisibleLowerElements(false);
        }

        private void SetEvent()
        {
            BattleEventDispatcher.Instance.OnSetBattleData = OnSetBattleData;
            BattleEventDispatcher.Instance.OnAddLogAction = OnAddLog;
            BattleEventDispatcher.Instance.OnAutoPlayActivatedAction = OnAutoPlayActivated;
            BattleEventDispatcher.Instance.OnDigestActivatedAction = OnDigestActivated;
            BattleEventDispatcher.Instance.OnDigestClosedAction = OnDigestClosed;
            BattleEventDispatcher.Instance.OnClickFooterSkipButtonAction = OnClickFooterSkipButton;
            BattleEventDispatcher.Instance.OnActivateUseAbilityUIAction = OnActivateUseAbilityUI;
            BattleEventDispatcher.Instance.OnActivateActiveAbilityAction = OnActivateActiveAbility;
            BattleEventDispatcher.Instance.OnSwipedAbilityAction = OnUseAbility;
            BattleEventDispatcher.Instance.OnMatchUpActivatedAction = OnMatchUpActivated;
        }

        private void OnSetBattleData()
        {
            BattleDataMediator.Instance.GameTime = 0;
            BattleUIMediator.Instance.HeaderUI.SetScore(0, 0);
            BattleUIMediator.Instance.HeaderUI.SetRemainTime(0);
        }

        private void OnAddLog()
        {
            BattleUIMediator.Instance.LogMessageScroller.SetLogMessage();
        }

        private void OnAutoPlayActivated()
        {
            digestUi.SetDigestUiActive(false);
        }

        protected virtual void OnDigestActivated(BattleConst.DigestType type)
        {
            digestUi.SetDigestUiActive(true);
        }

        protected virtual void OnDigestClosed(BattleConst.DigestType type)
        {
            if (type == BattleConst.DigestType.MatchUp)
            {
                BattleUIMediator.Instance.MatchUpUi.ClosePhraseUI();
            }
        }

        protected virtual void OnActivateUseAbilityUI(long abilityId, bool autoSwipe)
        {
            BattleUIMediator.Instance.DialogueUi.ClosedDialog();
            BattleUIMediator.Instance.ActivateAbilityUI.Open(abilityId, autoSwipe);
        }

        protected virtual void OnActivateActiveAbility()
        {
            if (!BattleDataMediator.Instance.IsReplayMode)
            {
                BattleLogMediator.Instance.AddActiveAbilityLog(BattleDataMediator.Instance.NextMatchUpResult);
            }
            BattleDigestController.Instance.ForceQuitCurrentDigest();
        }

        protected virtual void OnUseAbility(bool isSwiped)
        {
            BattleDigestController.Instance.SetCurrentDigestTimeAtStartAbilityDirectionTime();
            BattleManager.Instance.JudgeMatchUpFinalResult(isSwiped);
        }

        protected virtual void OnClickFooterSkipButton()
        {
            if (BattleDataMediator.Instance.IsReplayDigestMode)
            {
                BattleEventDispatcher.Instance.OnBattleEndCallback();
                return;
            }
            
            if (!BattleDataMediator.Instance.IsReplayGameMode &&
                BattleManager.Instance.CurrentState == BattleBase.BattleState.Finish)
            {
                return;
            }

            if (BattleDataMediator.Instance.IsSkipToFinish)
            {
                return;
            }

            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.InGameSkip, null);
        }

        protected virtual void OnMatchUpActivated(bool hideSomePhrase)
        {
            BattleUIMediator.Instance.MatchUpUi.OpenPhraseUI(hideSomePhrase);
        }
    }
}