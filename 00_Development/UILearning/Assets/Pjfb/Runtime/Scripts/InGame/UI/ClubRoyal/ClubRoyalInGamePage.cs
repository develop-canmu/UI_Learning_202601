using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Audio;
using Cysharp.Threading.Tasks;
using Pjfb.Battle;
using Pjfb.Colosseum;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Story;
using Pjfb.Training;
using UnityEngine;
using System.Linq;

namespace Pjfb.InGame.ClubRoyal
{
    public enum ClubRoyalInGamePageType
    {
        BattlePage,
    }

    public class ClubRoyalInGameOpenArgs
    {
        public string HostUrl;
        public long HostPort;
        public long MatchingId;
        public int GuildIdA;
        public int GuildIdB;
        public int DeckType;
        
        public ClubRoyalInGameOpenArgs(string hostUrl, long hostPort, long matchingId)
        {
            HostUrl = hostUrl;
            HostPort = hostPort;
            MatchingId = matchingId;
        }
        
        public ClubRoyalInGameOpenArgs(string hostUrl, long hostPort, int guildIdA, int guildIdB, int deckType)
        {
            HostUrl = hostUrl;
            HostPort = hostPort;
            GuildIdA = guildIdA;
            GuildIdB = guildIdB;
            DeckType = deckType;
        }
    }

    public class ClubRoyalInGamePage : PageManager<ClubRoyalInGamePageType>
    {

        private LogParse log;
        
        protected BattleDataMediator battleDataMediator;
        protected BattleLogMediator battleLogMediator;
        protected BattleEventDispatcher battleEventDispatcher;
        protected BattleUIMediator battleUIMediator;
        
        protected BattleBase battleBase;
        protected PageType OpenFrom;
        private object OtherData;

        protected override string GetAddress(ClubRoyalInGamePageType page)
        {
            switch (page)
            {
                case ClubRoyalInGamePageType.BattlePage: return "Prefabs/UI/Page/ClubRoyalInGame/ClubRoyalInGameBattlePage.prefab";
                default: throw new Exception("PageTypeが定義されてません。");
            }
        }

        protected override void OnEnablePage(object args)
        {
            // ヘッダーとフッタを非表示に
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();

            base.OnEnablePage(args);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var openArgs = (ClubRoyalInGameOpenArgs)args;
            
            PjfbGuildBattleDataMediator.Initialize();
            var inGamePageArgs = new ClubRoyalInGameBattlePage.Args(openArgs);
            return OpenPageAsync(ClubRoyalInGamePageType.BattlePage, true, inGamePageArgs, token);
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
        }

        protected override void OnClosed()
        {
            BattleManager.Instance.Release();
            PjfbGuildBattleDataMediator.Instance.Release();
            PjfbGameHubClient.Instance.Release();
            
            Resources.UnloadUnusedAssets();
            GC.Collect();

#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            base.OnClosed();
        }
    }
}