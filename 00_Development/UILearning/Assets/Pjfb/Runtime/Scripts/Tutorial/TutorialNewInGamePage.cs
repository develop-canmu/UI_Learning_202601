using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Battle;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{
    public class TutorialNewInGamePage : NewInGamePage
    {
        private int progressIndex = 0;

        protected override string GetAddress(NewInGamePageType page)
        {
            return $"Prefabs/UI/Page/TutorialNewInGame/NewInGame{page}.prefab";
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialNewInGame);
            var tutorialDetail = (TutorialSettings.Detail)args;
            AddManagers(tutorialDetail.scenarioInGameMode);
            progressIndex = tutorialDetail.startProgressIndex;
            
            BattleV2ClientData clientData = AppManager.Instance.TutorialManager.GetBattleV2ClientData(progressIndex);
            clientData.playerList[0].name = UserDataManager.Instance.user.name;
            // 選定ロジック固定化のためSingleBattleServerをチュートリアル用に作成
            battleBase = new TutorialSingleBattle();
            BattleManager.Instance.Initialize(battleBase);
            battleBase.SetData(clientData);
            BattleDataMediator.Instance.BallOwnerCharacterId = tutorialDetail.scenarioInGameMode ? 1 : 0;
            OpenFrom = PageType.TutorialTraining;
            SetEvent();
            
            return OpenPageAsync(NewInGamePageType.SimplePage, true, args, token);
        }

        protected override UniTask OnMessage(object value)
        {
            if (value is PageManager.MessageType type)
            {
                switch (type)
                {
                    case PageManager.MessageType.EndFade:
                        BGMManager.PlayBGMAsync("bgm_game_01").Forget();
                        break;
                }
            }
            return base.OnMessage(value);
        }

        protected override void OnOpened(object args)
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();

            // インゲームでは電池消費量緩和の為30FPS
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 30;
#endif
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            BattleEventDispatcher.Instance.OnBattleStartCallback();
        }

        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialNewInGame);
            base.OnClosed();
        }

        protected override UniTask StartBattle()
        {
            // チュートリアルは開始前の情報画面表示がないのですでにゲームページにいるので空の関数でoverride
            return UniTask.CompletedTask;
        }

        protected override void OnBattleEnd()
        {
            switch (OpenFrom)
            {
                // TODO トレーニング or ライバルリーバトル (ダイジェストのみの場合は即次のチュートリアルへ)
                case PageType.TutorialTraining:
                    EndTrainingBattle().Forget();
                    break;
            }
        }

        private async UniTask EndTrainingBattle()
        {
            var battleResult = (int)BattleDataMediator.Instance.GetBattleResult();
            var response = new TrainingProgressAPIResponse();
            // TODO API叩いて受け取るトレーニングの進捗はチュートリアルでは不要
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, response, battleResult);
            await OpenPageAsync(NewInGamePageType.ResultPage, false, resultParam);
        }
        private void AddManagers(bool scenarioInGameMode)
        {
            battleDataMediator = new BattleDataMediator();
            battleLogMediator = new BattleLogMediator();
            battleEventDispatcher = new BattleEventDispatcher();
            battleUIMediator = new BattleUIMediator();

            var digestController = gameObject.AddComponent<TutorialBattleDigestController>();
            if (scenarioInGameMode) digestController.SetScenarioInGameMode();

        }
    }
}