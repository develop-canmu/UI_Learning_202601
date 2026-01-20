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

namespace Pjfb.InGame
{
    public enum NewInGamePageType
    {
        StartPage,
        SimplePage,
        ResultPage,
    }

    public class NewInGameOpenArgs
    {
        public PageType OpenFrom;
        public BattleV2ClientData BattleData;
        public string CompressedBattleData;
        public string CompressedLogData;
        public long PlayerIndex;
        public object OtherData;
        // 敵戦力上書き用
        public BigValue FixedEnemyCombatPower = BigValue.Zero;
        
        public NewInGameOpenArgs(PageType openFrom, BattleV2ClientData battleData, object otherData)
        {
            OpenFrom = openFrom;
            BattleData = battleData;
            OtherData = otherData;
        }
        
        public NewInGameOpenArgs(PageType openFrom, BattleV2ClientData battleData,  BigValue fixedEnemyCombatPower, object otherData)
        {
            OpenFrom = openFrom;
            BattleData = battleData;
            FixedEnemyCombatPower = fixedEnemyCombatPower;
            OtherData = otherData;
        }

        
        public NewInGameOpenArgs(PageType openFrom, string compressedBattleData, string compressedLogData, long playerIndex, object otherData)
        {
            OpenFrom = openFrom;
            CompressedBattleData = compressedBattleData;
            CompressedLogData = compressedLogData;
            PlayerIndex = playerIndex;
            OtherData = otherData;
        }
    }

    public class NewInGamePage : PageManager<NewInGamePageType>
    {

        private LogParse log;
        
        protected BattleDataMediator battleDataMediator;
        protected BattleLogMediator battleLogMediator;
        protected BattleEventDispatcher battleEventDispatcher;
        protected BattleUIMediator battleUIMediator;

        protected NewInGameOpenArgs openArgs = null;
        protected BattleBase battleBase;
        protected PageType OpenFrom;
        private object OtherData;

        protected override string GetAddress(NewInGamePageType page)
        {
            switch (page)
            {
                case NewInGamePageType.StartPage: return "Prefabs/UI/Page/NewInGame/NewInGameStartPage.prefab";
                case NewInGamePageType.SimplePage: return "Prefabs/UI/Page/NewInGame/NewInGameSimplePage.prefab";
                case NewInGamePageType.ResultPage: return "Prefabs/UI/Page/NewInGame/NewInGameResultPage.prefab";
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
            AddManagers();
            battleBase = new SingleBattle();
            openArgs = (NewInGameOpenArgs)args;

            BattleManager.Instance.Initialize(battleBase);

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            var i = 0;
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "味方に加点", () =>
            {
                BattleDataMediator.Instance.Score[0]++;
                BattleDataMediator.Instance.ScoreLog.Add(BattleDataMediator.Instance.Decks[0][BattleGameLogic.GetNonStateRandomValue(0, 5)].id);
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "相手に加点", () =>
            {
                BattleDataMediator.Instance.Score[1]++;
                BattleDataMediator.Instance.ScoreLog.Add(BattleDataMediator.Instance.Decks[1][BattleGameLogic.GetNonStateRandomValue(0, 5)].id);
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "引き分けで終了", () =>
            {
                BattleDataMediator.Instance.Score[0] = 9;
                BattleDataMediator.Instance.Score[1] = 9;
                for (var i = 0; i < 9; i++)
                {
                    BattleDataMediator.Instance.ScoreLog.Add(BattleDataMediator.Instance.Decks[0][i % 5].id);
                    BattleDataMediator.Instance.ScoreLog.Add(BattleDataMediator.Instance.Decks[1][i % 5].id);
                }
                OnBattleEnd();
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "味方全員スタミナ1割回復", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[0])
                {
                    chara.CurrentStamina = BigValue.Clamp(chara.CurrentStamina + chara.MaxStamina / 10, BigValue.Zero, chara.MaxStamina);
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "味方全員スタミナ1割消費", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[0])
                {
                    chara.CurrentStamina = BigValue.Clamp(chara.CurrentStamina - chara.MaxStamina / 10, BigValue.Zero, chara.MaxStamina);
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "敵全員スタミナ1割回復", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[1])
                {
                    chara.CurrentStamina = BigValue.Clamp(chara.CurrentStamina + chara.MaxStamina / 10, BigValue.Zero, chara.MaxStamina);
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "敵全員スタミナ1割消費", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[1])
                {
                    chara.CurrentStamina = BigValue.Clamp(chara.CurrentStamina - chara.MaxStamina / 10, BigValue.Zero, chara.MaxStamina);
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "味方全員ステータス表示", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[0])
                {
                    Debug.Log($"Name: {chara.Name}, Physical: {chara.GetCurrentPhysical()}, Speed: {chara.GetCurrentSpeed()}, Tech: {chara.GetCurrentTechnique()}, Kick: {chara.GetCurrentKick()}, Wise: {chara.GetCurrentWise()}, StaminaRate: {chara.GetStaminaRate()}, ShootRange: {chara.GetCurrentShootRange()}");
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "敵全員ステータス表示", () =>
            {
                foreach (var chara in BattleDataMediator.Instance.Decks[1])
                {
                    Debug.Log($"Name: {chara.Name}, Physical: {chara.GetCurrentPhysical()}, Speed: {chara.GetCurrentSpeed()}, Tech: {chara.GetCurrentTechnique()}, Kick: {chara.GetCurrentKick()}, Wise: {chara.GetCurrentWise()}, StaminaRate: {chara.GetStaminaRate()}, ShootRange: {chara.GetCurrentShootRange()}");
                }
            }, i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "強制等速",
                () => { return BattleDataMediator.Instance.ForceFlatPlaySpeed; },
                (value) => { BattleDataMediator.Instance.ForceFlatPlaySpeed = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "ログ一時停止",
                () => { return BattleDataMediator.Instance.StopLogPlay; },
                (value) => { BattleDataMediator.Instance.StopLogPlay = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "突破/シュート(絶対成功)",
                () => { return BattleDataMediator.Instance.ForceSuccessMatchUp; },
                (value) => { BattleDataMediator.Instance.ForceSuccessMatchUp = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "突破/シュート(絶対失敗)",
                () => { return BattleDataMediator.Instance.ForceFailMatchUp; },
                (value) => { BattleDataMediator.Instance.ForceFailMatchUp = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "スタミナ消費なし",
                () => { return BattleDataMediator.Instance.DontConsumeStamina; },
                (value) => { BattleDataMediator.Instance.DontConsumeStamina = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "パッシブ発動率表示",
                () => { return BattleDataMediator.Instance.ShowPassiveInvokeRate; },
                (value) => { BattleDataMediator.Instance.ShowPassiveInvokeRate = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "パッシブ発動率無視",
                () => { return BattleDataMediator.Instance.ForceSuccessPassiveInvokeByRate; },
                (value) => { BattleDataMediator.Instance.ForceSuccessPassiveInvokeByRate = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "エフェクト発動率無視",
                () => { return BattleDataMediator.Instance.ForceSuccessPassiveEffectInvoke; },
                (value) => { BattleDataMediator.Instance.ForceSuccessPassiveEffectInvoke = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "アクティブ発動率無視",
                () => { return BattleDataMediator.Instance.ForceSuccessActiveInvokeByRate; },
                (value) => { BattleDataMediator.Instance.ForceSuccessActiveInvokeByRate = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "発動コンディション無視アビリティId(\",区切り入力\")",
                () =>
                {
                    var s = string.Empty;
                    foreach (var id in BattleDataMediator.Instance.ForceActivateAbilityIds)
                    {
                        s += $"{id}, ";
                    }
                    return s;
                },
                (value) =>
                {
                    var ids = value.Split(",");
                    BattleDataMediator.Instance.ForceActivateAbilityIds.Clear();
                    foreach (var id in ids)
                    {
                        BattleDataMediator.Instance.ForceActivateAbilityIds.Add(long.Parse(id));
                    }
                }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "上記Id以外アビリティ発動させない",
                () => { return BattleDataMediator.Instance.ForceDontActivateAbilities; },
                (value) => { BattleDataMediator.Instance.ForceDontActivateAbilities = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "シュートレンジ無視",
                () => { return BattleDataMediator.Instance.IgnoreShootRange; },
                (value) => { BattleDataMediator.Instance.IgnoreShootRange = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "ラウンドに全員参加",
                () => { return BattleDataMediator.Instance.ForceJoinFullMemberToRound; },
                (value) => { BattleDataMediator.Instance.ForceJoinFullMemberToRound = value; }
            , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "突破選択率爆上げ",
                () => { return BattleDataMediator.Instance.AddMatchUpActionWeightThrough; },
                (value) => { BattleDataMediator.Instance.AddMatchUpActionWeightThrough = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "パス選択率爆上げ",
                () => { return BattleDataMediator.Instance.AddMatchUpActionWeightPass; },
                (value) => { BattleDataMediator.Instance.AddMatchUpActionWeightPass = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "シュート選択率爆上げ",
                () => { return BattleDataMediator.Instance.AddMatchUpActionWeightShoot; },
                (value) => { BattleDataMediator.Instance.AddMatchUpActionWeightShoot = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "クロス選択率爆上げ",
                () => { return BattleDataMediator.Instance.AddMatchUpActionWeightCross; },
                (value) => { BattleDataMediator.Instance.AddMatchUpActionWeightCross = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "強制マッチアップ演出タイプ(0:なし,1:あり,2:文字少なめ)",
                () => { return BattleDataMediator.Instance.ForceMatchUpType; },
                (value) => { BattleDataMediator.Instance.ForceMatchUpType = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "時間消費なし",
                () => { return BattleDataMediator.Instance.DontWasteTime; },
                (value) => { BattleDataMediator.Instance.DontWasteTime = value; }
                , i++);
            CruFramework.DebugMenu.AddOption(PageType.NewInGame.ToString(), "スコア加算なし",
                () => { return BattleDataMediator.Instance.DontAddScore; },
                (value) => { BattleDataMediator.Instance.DontAddScore = value; }
                , i++);
#endif

            if (!string.IsNullOrEmpty(openArgs.CompressedBattleData))
            {
                SetReplayData(ref openArgs);
                BattleDataMediator.Instance.IsAutoSwipe = true;
            }

            OpenFrom = openArgs.OpenFrom;
            BattleDataMediator.Instance.SetBattleType(OpenFrom);
            battleBase.SetData(openArgs.BattleData);
            OtherData = openArgs.OtherData;

            if (log != null)
            {
                SetReplayStatsData();
            }

            SetEvent();
            
            // PvPで入ってきたときは結果確定まで処理後に内容の再生を行う.
            if (BattleDataMediator.Instance.BattleType == BattleConst.BattleType.VersusPlayerBattle &&
                !BattleDataMediator.Instance.IsReplayGameMode)
            {
                BattleDataMediator.Instance.IsSkipToFinish = true;
                BattleDataMediator.Instance.IsSkipToFinishWithoutView = true;
                return StartBattle();
            }

            // リーグマッチリプレイはスキップなし(任意で見ようとしているため)
            if (BattleDataMediator.Instance.BattleType != BattleConst.BattleType.ReplayLeagueMatch &&
                LocalSaveManager.saveData.skipMatchData)
            {
                BattleDataMediator.Instance.IsSkipToFinish = true;
                BattleDataMediator.Instance.IsSkipToFinishWithoutView = true;
                return StartBattle();
            }

            var startParam = new NewInGameStartPage.Param(OpenFrom, openArgs.FixedEnemyCombatPower);
            return OpenPageAsync(NewInGamePageType.StartPage, true, startParam, token);
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

            BGMManager.PlayBGMAsync(BGM.bgm_game_01).Forget();
        }

        protected override void OnClosed()
        {
            BattleManager.Instance.Release();

            Resources.UnloadUnusedAssets();
            GC.Collect();

#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.RemoveOption(PageType.NewInGame.ToString());
#endif
            base.OnClosed();
        }

        protected void SetEvent()
        {
            BattleEventDispatcher.Instance.OnBattleStartAction = OnBattleStart;
            BattleEventDispatcher.Instance.OnBattleEndAction = OnBattleEnd;
            BattleEventDispatcher.Instance.OnCalledNextMatchUpAction = DoNextMatchUp;
            BattleEventDispatcher.Instance.ReplayDigestAction = ReplayDigestScene;
        }

        protected void OnBattleStart()
        {
            switch (BattleDataMediator.Instance.BattleType)
            {
                case BattleConst.BattleType.None:
                case BattleConst.BattleType.StoryBattle:
                case BattleConst.BattleType.TrainingBattle:
                case BattleConst.BattleType.RivalryBattle:
                    StartBattle().Forget();
                    break;
                case BattleConst.BattleType.VersusPlayerBattle:
                    StartBattleReplay().Forget();
                    break;
                case BattleConst.BattleType.ReplayLeagueMatch:
                    StartBattleReplay(log.logData).Forget();
                    break;
            }
        }

        protected virtual async UniTask StartBattle()
        {
            await OpenPageAsync(NewInGamePageType.SimplePage, false, null);
        }

        protected virtual async UniTask StartBattleReplay()
        {
            await OpenPageAsync(NewInGamePageType.SimplePage, false, -1);
        }
        
        protected virtual async UniTask StartBattleReplay(List<BattleLog> logs)
        {
            await OpenPageAsync(NewInGamePageType.SimplePage, false, logs);
        }

        
        protected virtual void DoNextMatchUp()
        {
            if (BattleDataMediator.Instance.IsReplayMode)
            {
                return;
            }
            
            BattleManager.Instance.DoNextMatchUp();
        }

        protected virtual void OnBattleEnd()
        {
            // failsafe
            AudioManager.Instance.SetVolume(AudioGroup.BGM, BattleDataMediator.Instance.DefaultBgmVolume / 10.0f);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();

            if (BattleDataMediator.Instance.IsReplayMode)
            {
                BattleLogMediator.Instance.BattleLogs.Clear();
                BattleUIMediator.Instance.DialogueUi.CancelPlayVoice();
                BattleUIMediator.Instance.LogMessageScroller.Cancel();
                OpenPageAsync(NewInGamePageType.ResultPage, false, BattleDataMediator.Instance.ResultParam);
                return;
            }
            
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            
            switch (OpenFrom, OtherData)
            {
                case (PageType.Story, null):
                case (PageType.Rivalry, null):
                    EndStoryBattle();
                    break;
                case (PageType.Training, null):
                    EndTrainingBattle();
                    break;
                case (PageType.ClubMatch, ColosseumInGameParam):
                case (PageType.Colosseum, ColosseumInGameParam):
                    EndColosseumBattle();
                    break;
                default:
                    EndFriendMatch();
                    break;
            }

        }

        private async void EndStoryBattle()
        {
            var battleResult = (int)BattleDataMediator.Instance.GetBattleResult();
            var res = await StoryManager.Instance.CallHuntFinishApi(battleResult);
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, res, battleResult);
            BattleDataMediator.Instance.ResultParam = resultParam;
            await OpenPageAsync(NewInGamePageType.ResultPage, false, resultParam);
        }

        private async void EndTrainingBattle()
        {
            var battleResult = (int)BattleDataMediator.Instance.GetBattleResult();
            var postArgs = new TrainingProgressArgs();
            
            // 活躍度を取得
            postArgs.activityRank = 0;
            if(BattleDataMediator.Instance.Decks.Count == 2)
            {
                var playerCharacters = BattleDataMediator.Instance.Decks[0].OrderByDescending(chara => chara.Stats.ActivityPoint).ToArray();
                long mCharId = BattleDataMediator.Instance.OriginalBattleV2ClientData.charaList[0].mCharaId;
                for(int i=0;i<playerCharacters.Length;i++)
                {
                    if(playerCharacters[i].CharaMaster.id == mCharId)
                    {
                        postArgs.activityRank = (i + 1);
                        break;
                    }
                }
            }

            var trainingResult = await TrainingUtility.ProgressAPI(battleResult, postArgs);
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, trainingResult, battleResult);
            BattleDataMediator.Instance.ResultParam = resultParam;
            await OpenPageAsync(NewInGamePageType.ResultPage, false, resultParam);
        }

        private async void EndFriendMatch()
        {
            var battleResult = (int)BattleDataMediator.Instance.GetBattleResult();
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, battleResult, OtherData);
            BattleDataMediator.Instance.ResultParam = resultParam;
            if (LocalSaveManager.saveData.skipMatchData)
            {
                await OpenPageAsync(NewInGamePageType.ResultPage, false, resultParam);                
            }
            else
            {
                var startParam = new NewInGameStartPage.Param(OpenFrom, openArgs.FixedEnemyCombatPower);
                await OpenPageAsync(NewInGamePageType.StartPage, true, startParam);
            }
        }

        private async void EndColosseumBattle()
        {
            var battleResult = (int)BattleDataMediator.Instance.GetBattleResult();
            var colosseumInGameParam = (ColosseumInGameParam)OtherData;
            var sColosseumEventId = colosseumInGameParam.sColosseumEventId;
            var res = await ColosseumManager.RequestColosseumAttackAsync(
                BattleDataMediator.Instance.OriginalBattleV2ClientData,
                battleResult,
                BattleDataMediator.Instance.Score[(int)BattleConst.TeamSide.Left],
                BattleDataMediator.Instance.Score[(int)BattleConst.TeamSide.Right],
                sColosseumEventId);
            var useDeckId = colosseumInGameParam.useDeckId;
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, res, battleResult, useDeckId);
            BattleDataMediator.Instance.ResultParam = resultParam;
            if (LocalSaveManager.saveData.skipMatchData)
            {
                await OpenPageAsync(NewInGamePageType.ResultPage, false, resultParam);                
            }
            else
            {
                var startParam = new NewInGameStartPage.Param(OpenFrom, openArgs.FixedEnemyCombatPower);
                await OpenPageAsync(NewInGamePageType.StartPage, true, startParam);
            }
        }

        private async void ReplayDigestScene(int index)
        {
            await OpenPageAsync(NewInGamePageType.SimplePage, false, index);
        }

        private void AddManagers()
        {
            battleDataMediator = new BattleDataMediator();
            battleLogMediator = new BattleLogMediator();
            battleEventDispatcher = new BattleEventDispatcher();
            battleUIMediator = new BattleUIMediator();
            
            gameObject.AddComponent<BattleDigestController>();
        }

        private void SetReplayData(ref NewInGameOpenArgs openArgs)
        {
            var battleDataString = BattleDataMediator.GetRawStringByCompressedString(openArgs.CompressedBattleData);
            var logString = BattleDataMediator.GetRawStringByCompressedString(openArgs.CompressedLogData);

            openArgs.BattleData = JsonUtility.FromJson<BattleV2ClientData>(battleDataString);
            log = JsonUtility.FromJson<LogParse>(logString);
            battleDataMediator.SetPlayerSide(openArgs.PlayerIndex);
            
            SetReplayScoreData(openArgs.OpenFrom, openArgs.OtherData);
        }

        private void SetReplayScoreData(PageType OpenFrom, object args)
        {
            foreach (var battleLog in log.logData)
            {
                if (battleLog.DigestLog.Score.Count > 0)
                {
                    var leftScore = battleLog.DigestLog.Score[0];
                    var rightScore = battleLog.DigestLog.Score[1];
                    if (leftScore != battleDataMediator.Score[0] || rightScore != battleDataMediator.Score[1])
                    {
                        battleDataMediator.Score[0] = leftScore;
                        battleDataMediator.Score[1] = rightScore;
                        battleDataMediator.ScoreLog.Add(battleLog.DigestLog.MainCharacterData.CharaId);
                    }
                }
            }

            var battleResult = (int)StoryUtility.BattleResultType.Draw;
            if (BattleDataMediator.Instance.Score[0] != BattleDataMediator.Instance.Score[1])
            {
                if (BattleDataMediator.Instance.PlayerSide == BattleConst.TeamSide.Left)
                {
                    battleResult = battleDataMediator.Score[0] > battleDataMediator.Score[1]
                        ? (int)StoryUtility.BattleResultType.Win
                        : (int)StoryUtility.BattleResultType.Lose;
                }
                else
                {
                    battleResult = battleDataMediator.Score[0] > battleDataMediator.Score[1]
                        ? (int)StoryUtility.BattleResultType.Lose
                        : (int)StoryUtility.BattleResultType.Win;
                }
            }
            
            var resultParam = new NewInGameResultPage.ResultParam(OpenFrom, battleResult, args);
            BattleDataMediator.Instance.ResultParam = resultParam;
        }

        private void SetReplayStatsData()
        {
            foreach (var stat in log.stats)
            {
                var charaId = stat.CharacterId;
                var target = battleDataMediator.GetBattleCharacter(charaId);
                target.Stats = stat;
            }
        }
    }
}