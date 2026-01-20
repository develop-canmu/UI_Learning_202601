using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.ResourceManagement;
using Pjfb.Battle;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.InGame.ClubRoyal;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameBattlePage : Page
    {
        public class Args
        {
            public string HostUrl;
            public long HostPort;
            public long MatchingId;
            public int GuildIdA;
            public int GuildIdB;
            public int DeckType;
            
            public Args(string hostUrl, int hostPort, int matchingId)
            {
                HostUrl = hostUrl;
                HostPort = hostPort;
                MatchingId = matchingId;
            }
            
            public Args(string hostUrl, int hostPort, int guildIdA, int guildIdB, int deckType)
            {
                HostUrl = hostUrl;
                HostPort = hostPort;
                GuildIdA = guildIdA;
                GuildIdB = guildIdB;
                DeckType = deckType;
            }

            public Args(ClubRoyalInGameOpenArgs args)
            {
                HostUrl = args.HostUrl;
                HostPort = args.HostPort;
                MatchingId = args.MatchingId;
                GuildIdA = args.GuildIdA;
                GuildIdB = args.GuildIdB;
                DeckType = args.DeckType;
            }
        }
        
        [SerializeField] private PjfbGameHubClient gameHubClient;
        [SerializeField] private GameObject characterModelRender;

        private Args OpenArgs;
        private ResourcesLoader resourcesLoader;
        protected BattleLogMediator battleLogMediator;
        protected BattleEventDispatcher battleEventDispatcher;

        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            resourcesLoader = new ResourcesLoader();
            ClubRoyalInGameUIMediator.Instance.SetResourceLoader(resourcesLoader);

            // サポートスキルのログ表示用
            battleLogMediator = new BattleLogMediator();
            battleLogMediator.ClearLogs();
            
            battleEventDispatcher = new BattleEventDispatcher();
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
        }

        private async UniTask LoadEssentialAsset(Action callback)
        {
            await BattleUIMediator.Instance.RadarUI.LoadCharacterModel(BattleDigestController.Instance.BattleResourcesLoader, BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.PlayerSide], BattleDataMediator.Instance.Decks[(int)BattleDataMediator.Instance.EnemySide]);
            callback?.Invoke();
        }

        protected override void OnOpened(object args)
        {
            OpenArgs = (Args)args;
            ClubRoyalInGameUIMediator.Instance.StartDirectionUI.Open();
            JoinGuildBattle().Forget();
            base.OnOpened(args);
        }

        private async UniTask JoinGuildBattle()
        {
            var hostAddr = GuildBattleCommonLogic.GetHostAddress(OpenArgs.HostUrl, OpenArgs.HostPort);
            var joinResult = await gameHubClient.JoinAsync(hostAddr, GuildBattleCommonConst.DummyRoomName, OpenArgs.MatchingId, OnAfterConnect);
            if (joinResult != GuildBattleCommonConst.JoinResult.Success)
            {
                PjfbGameHubClient.OpenDisconnectedModal();
                return;
            }
            
            var tokenSource = new CancellationTokenSource();
            CancelDataReceiveAsync(tokenSource).Forget();
            CruFramework.Logger.Log($"WaitDataReceived Start");
            await UniTask.WaitUntil(() => PjfbGuildBattleDataMediator.Instance.IsBattleDataInitialized(), cancellationToken: tokenSource.Token);
            CruFramework.Logger.Log($"DataReceived Start");
            tokenSource.Cancel();

            ClubRoyalInGameUIMediator.Instance.HeaderUI.InitializeUI(PjfbGuildBattleDataMediator.Instance.OriginalBattleData);
            ClubRoyalInGameUIMediator.Instance.FooterUI.Initialize();
            ClubRoyalInGameUIMediator.Instance.FieldUI.Initialize(PjfbGuildBattleDataMediator.Instance.BattleField);
            ClubRoyalInGameUIMediator.Instance.BeforeFightTimerUI.StartTimer(PjfbGuildBattleDataMediator.Instance.UtcStartAt);
            ClubRoyalInGameUIMediator.Instance.Initialize();

            CruFramework.Logger.Log($"SendReady Start");
            await gameHubClient.SendOnReady();
            CruFramework.Logger.Log($"SendReady Finish");
            characterModelRender.SetActive(true);
            ClubRoyalInGameUIMediator.Instance.StartDirectionUI.StartAnimation(PjfbGuildBattleDataMediator.Instance.OriginalBattleData).Forget();
            SetEvent();
        }

        private async UniTask CancelDataReceiveAsync(CancellationTokenSource tokenSource)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(GuildBattleCommonConst.WaitReceiveBattleDataTimeOutSeconds), cancellationToken: tokenSource.Token);
            if (tokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            
            tokenSource.Cancel();
        }

        protected override void OnClosed()
        {
            ClubRoyalInGameUIMediator.Instance.Closed();
            base.OnClosed();
        }

        private void SetReference()
        {
        }

        private void SetEvent()
        {
            BattleEventDispatcher.Instance.OnSetBattleData = OnSetBattleData;
            BattleEventDispatcher.Instance.OnAddLogAction = OnAddLog;
        }

        private void OnSetBattleData()
        {
            BattleDataMediator.Instance.GameTime = 0;
            BattleUIMediator.Instance.HeaderUI.SetScore(0, 0);
            BattleUIMediator.Instance.HeaderUI.SetRemainTime(0);
        }

        private void OnAddLog()
        {
            if (ClubRoyalInGameUIMediator.Instance.LogMessageScroller != null)
            {
                ClubRoyalInGameUIMediator.Instance.LogMessageScroller.SetLogMessage();
            }
            else 
            {
                CruFramework.Logger.LogError("LogMessageScroller is null");
            }
        }

        private void OnAfterConnect()
        {
            CruFramework.Logger.Log("OnAfterConnect");
            WaitReady().Forget();
        }

        private async UniTask WaitReady()
        {
            CruFramework.Logger.Log("WaitReady");
            var tokenSource = new CancellationTokenSource();
            CancelDataReceiveAsync(tokenSource).Forget();
            CruFramework.Logger.Log($"WaitDataReceived Start");
            await UniTask.WaitUntil(() => PjfbGuildBattleDataMediator.Instance.IsBattleDataInitialized(), cancellationToken: tokenSource.Token);
            CruFramework.Logger.Log($"DataReceived Start");
            tokenSource.Cancel();

            ClubRoyalInGameUIMediator.Instance.HeaderUI.InitializeUI(PjfbGuildBattleDataMediator.Instance.OriginalBattleData);
            ClubRoyalInGameUIMediator.Instance.FieldUI.Initialize(PjfbGuildBattleDataMediator.Instance.BattleField);

            CruFramework.Logger.Log($"SendReady Start");
            await gameHubClient.SendOnReady();
        }
    }
}