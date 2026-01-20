using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using Pjfb.InGame;
using Pjfb.InGame.ClubRoyal;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class PjfbGameHubClient : MonoBehaviour, IPjfbGameHubReceiver
    {
        public static PjfbGameHubClient Instance;
        
        private IPjfbGameHub gameHubClient;
        private GrpcChannel channel;
        private bool isDisconnectingSelf;
        private readonly CancellationTokenSource shutdownCancellation = new CancellationTokenSource();
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        private string hostUrl;
        private string roomName;
        private long matchingId;
        
#if !PJFB_REL
        public static string DebugAddr = string.Empty;
        public static int DebugPort = -1;
        public static int DebugGuildIdA = -1;
        public static int DebugGuildIdB = -1;
        public static int DebugDeckId = -1;
#endif
        
        public CancellationTokenSource ConnectedCancellationSource => tokenSource;
        public CancellationToken ConnectedCancellationToken => tokenSource.Token;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            LeaveAsync().Forget();
        }

        public void Release()
        {
            LeaveAsync().Forget();
        }

        #region CommonCallback

        public void OnReadyServer()
        {
            //throw new NotImplementedException();
        }

        public void OnJoin(GuildBattleCommonHubPlayerData player)
        {
            Logger.Log($"OnJoin: {player.PlayerId}");
            // これいるか?
            //throw new System.NotImplementedException();
        }

        public void OnLeave(GuildBattleCommonHubPlayerData player)
        {
            //throw new System.NotImplementedException();
        }

        public void OnReceiveActivePlayerIds(List<long> playerIds)
        {
            ClubRoyalInGameUIMediator.Instance.HeaderUI.UpdateActivePlayerView(playerIds);
        }

        public void OnReceiveChat(GuildBattleCommonChatData commonChatData)
        {
            var isAdded = PjfbGuildBattleDataMediator.Instance.TryAddReceivedChatData(commonChatData);
            if (isAdded)
            {
                ClubRoyalInGameUIMediator.Instance.ChatView.AddReceivedChat(commonChatData);
            }
        }

        public void OnReceiveAllChat(List<GuildBattleCommonChatData> allChatData)
        {
            var isAdded = false;
            foreach (var chatData in allChatData)
            {
                isAdded |= PjfbGuildBattleDataMediator.Instance.TryAddReceivedChatData(chatData);
            }
            
            if (isAdded)
            {
                ClubRoyalInGameUIMediator.Instance.ChatView.InitializeUI(allChatData);
            }
        }
        
        public void OnReceiveCurrentGameState(GuildBattleCommonConst.GuildBattleGameState gameState)
        {
            // インゲーム滞在中に バトル前 -> バトル中 に移行時には演出再生
            if (PjfbGuildBattleDataMediator.Instance.GameState == GuildBattleCommonConst.GuildBattleGameState.BeforeFight &&
                gameState == GuildBattleCommonConst.GuildBattleGameState.InFight)
            {
                ClubRoyalInGameUIMediator.Instance.KickOffUI.StartAnimation();
            }
            
            PjfbGuildBattleDataMediator.Instance.UpdateGameState(gameState);
            ClubRoyalInGameUIMediator.Instance.HeaderUI.OnBattleStateChanged();
        }
        
#endregion

#region ApplicationCallback

        public void OnReceiveBattleData(BattleV2ClientData battleData, DateTime utcStartAt)
        {
            Logger.Log($"OnReceiveBattleData: {battleData} / {utcStartAt}");
            PjfbGuildBattleDataMediator.Instance.InitializeOnClient(battleData, utcStartAt);
        }

        public void OnReceiveBattlePlayerData(GuildBattlePlayerData battlePlayerData, bool isTurnResult)
        {
            Logger.Log($"OnReceiveBattlePlayerData: {battlePlayerData.PlayerId}");
            PjfbGuildBattleDataMediator.Instance.UpdateGuildBattlePlayerData(battlePlayerData);
            ClubRoyalInGameUIMediator.Instance.ActiveItemListUI.UpdateUI(battlePlayerData);
            ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.Invoke(battlePlayerData);

            if (!isTurnResult)
            {
                ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.UpdateUI(true, false);
            }
            
            var topModal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            if (topModal != null && topModal is ClubRoyalInGameSelectPartyModal modal)
            {
                modal.OnReceivePlayerData();
            }
        }

        public async void OnReceiveBattleTurnResult(GuildBattleFieldSituationModel fieldSituation)
        {
            Logger.Log($"OnReceiveBattleTurnResult: {fieldSituation.TurnNumber}, WinningPoint: {fieldSituation.WinningPoints[0]}/{fieldSituation.WinningPoints[1]}");
            
            if (tokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            // 途中参戦 or データが歯抜けになった場合. 強制的に現在ターンの最終状態で描画
            if (PjfbGuildBattleDataMediator.Instance.LatestSituation == null ||
                fieldSituation.TurnNumber - PjfbGuildBattleDataMediator.Instance.LatestSituation.TurnNumber > 1)
            {
                PjfbGuildBattleDataMediator.Instance.UpdateLocalData(fieldSituation);

                ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.UpdateUI(false, false);
                ClubRoyalInGameUIMediator.Instance.HeaderUI.UpdateRemainTurn(fieldSituation.RemainTurn);
                ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateBeforeFightUnitView(fieldSituation);
                ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateMapSpotView(PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary);
                return;
            }
            
            // BeforeTurnDirection
            ClubRoyalInGameUIMediator.Instance.HeaderUI.UpdateRemainTurn(fieldSituation.RemainTurn);
            
            ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateBeforeFightUnitView(fieldSituation);
            ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateMoveUnitView(fieldSituation);

            if (tokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            // 戦闘 or ゴールを決めたチームがあったら演出開始.
            if (fieldSituation.EachLaneMatchingPartyIds.Count > 0 || fieldSituation.ScoredPartyIds.Count > 0)
            {
                await ClubRoyalInGameUIMediator.Instance.FieldUI.PlayBattleEffect(fieldSituation);
                ClubRoyalInGameUIMediator.Instance.WinStreakUI.PlayEffect(fieldSituation);
            }

            if (tokenSource.Token.IsCancellationRequested)
            {
                return;
            }
            
            // AfterTurnDirection
            try
            {
                if (tokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.UpdateUI(true, false);
                PjfbGuildBattleDataMediator.Instance.UpdateLocalData(fieldSituation);
                ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateDeadUnitView(fieldSituation);
                ClubRoyalInGameUIMediator.Instance.FieldUI.UpdateMapSpotView(PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary);

                var allyPointIndex = (int)(PjfbGuildBattleDataMediator.Instance.PlayerSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right);
                var enemyPointIndex = (int)(PjfbGuildBattleDataMediator.Instance.PlayerSide == GuildBattleCommonConst.GuildBattleTeamSide.Left ? GuildBattleCommonConst.GuildBattleTeamSide.Right : GuildBattleCommonConst.GuildBattleTeamSide.Left);
                ClubRoyalInGameUIMediator.Instance.HeaderUI.UpdateSituationGaugeSlider(
                    PjfbGuildBattleDataMediator.Instance.WinningPoints[allyPointIndex],
                    PjfbGuildBattleDataMediator.Instance.WinningPoints[enemyPointIndex]);

                Logger.Log($"OnReceiveFieldSituationFinish. Turn:{fieldSituation.TurnNumber}");

                var topModal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
                if (topModal != null && topModal is ClubRoyalInGameSpotModal modal)
                {
                    modal.OnReceiveBattleTurnResult();
                }
            }
            catch (OperationCanceledException)
            {
                // 演出中に退出などでキャンセルするので特に問題ないです.
            }
        }
        
        public void OnReceiveBattleStandbyUnits(List<GuildBattleUnitStateModel> standbyUnits)
        {
            // PJFBでは利用しません.
            throw new NotImplementedException();
        }

        public void OnReceiveBattleResult(GuildBattleResultData resultData)
        {
            Logger.Log($"BattleResult: {resultData.WinTeam}");
            ClubRoyalInGameUIMediator.Instance.ResultUI.PlayAnimation(resultData).Forget();
        }

        public void OnReceivePlayerPartyData(List<GuildBattlePartyModel> parties)
        {
            PjfbGuildBattleDataMediator.Instance.UpdatePlayerBattleParties(parties);
            ClubRoyalInGameUIMediator.Instance.ActivePartyListUI.UpdateUI();
            
            var topModal = AppManager.Instance.UIManager.ModalManager.GetTopModalWindow();
            if (topModal != null && topModal is ClubRoyalInGameSelectPartyModal modal)
            {
                modal.OnReceivePartyData();
            }
        }

        #endregion

#region Other

        public async UniTask<GuildBattleCommonConst.JoinResult> JoinAsync(string _hostUrl, string _roomName, long _matchingId, Action onSuccessReconnect)
        {
            if (gameHubClient != null)
            {
                return GuildBattleCommonConst.JoinResult.Error;
            }

            hostUrl = _hostUrl;
            roomName = _roomName;
            matchingId = _matchingId;
            
            var isConnected = await EstablishChannel();
            if (!isConnected)
            {
                return GuildBattleCommonConst.JoinResult.Error;
            }
            
            RegisterDetectDisconnectEvent(onSuccessReconnect).Forget();
            var userId = UserDataManager.Instance.user.uMasterId;
            var ret = GuildBattleCommonConst.JoinResult.None;
#if !PJFB_REL
            if (DebugAddr != string.Empty && DebugAddr == "localhost")
            {
                ret = await gameHubClient.DebugJoinAsync(roomName, userId, DebugGuildIdA, DebugGuildIdB, DebugDeckId);
            }
            else
            {
                ret = await gameHubClient.JoinAsync(roomName, userId, matchingId);
            }
#else
            ret = await gameHubClient.JoinAsync(roomName, userId, matchingId);
#endif

            return ret;
        }

        private async UniTask<bool> EstablishChannel()
        {
            if (channel == null)
            {
                channel = ConnectionCommon.GetGrpcChannelWithCertificate(hostUrl);
            }

            Logger.Log($"接続開始: {DateTime.Now.Second}.{DateTime.Now.Millisecond}");
            var tokenSource = new CancellationTokenSource();
            // 接続自体のタイムアウト設定(Deadline)を利用したいが, 設定するとなぜか通信が成功しても強制的にクローズ&再接続されてしまう… ので, 接続自体のリトライとタイムアウトに関しては無理やり自前
            // API毎のタイムアウトとリトライは機能しているので, 上記channel生成時に内部で設定している.
            // var options = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5));
            try
            {
                CancelConnection(tokenSource, shutdownCancellation).Forget();
                gameHubClient = await StreamingHubClient.ConnectAsync<IPjfbGameHub, IPjfbGameHubReceiver>(channel, this, cancellationToken: shutdownCancellation.Token);
            }
            catch (Exception)
            {
                return false;
            }

            tokenSource.Cancel();
            Logger.Log($"接続完了: {DateTime.Now.Second}.{DateTime.Now.Millisecond}");
            return true;
        }

        public async UniTask LeaveAsync()
        {
            if (isDisconnectingSelf)
            {
                return;
            }
            
            tokenSource.Cancel();
            isDisconnectingSelf = true;
            
            if (gameHubClient != null)
            {
                await gameHubClient.LeaveAsync();
                await gameHubClient.DisposeAsync();
                gameHubClient = null;
            }

            if (channel != null)
            {
                await channel.ShutdownAsync();
            }
        }
        
        private async UniTask RegisterDetectDisconnectEvent(Action onSuccessReconnect)
        {
            try
            {
                Logger.Log($"Register disconnect event");
                await gameHubClient.WaitForDisconnect();
            }
            catch (Exception e)
            {
                Logger.LogError($"Disconnected from server. Reason: {e}");
            }
            finally
            {
                Logger.Log($"Finally register event.");
                if (!isDisconnectingSelf && !ConnectedCancellationToken.IsCancellationRequested)
                {
                    Logger.LogError($"Try reconnect to server.");
                    // UIへのアクセスがあるためメインスレッドでの実行にする
                    SynchronizationContext.Current.Post(_ => TryReconnectServer(onSuccessReconnect).Forget(), null);
                }
            }
        }

        private async UniTask CancelConnection(CancellationTokenSource source, CancellationTokenSource connection)
        {
            connection.Token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(GuildBattleCommonConst.ConnectTimeoutSeconds), cancellationToken: source.Token);
            if (source.IsCancellationRequested)
            {
                return;
            }
            
            Logger.LogError("Cancel connect");
            if (gameHubClient != null)
            {
                gameHubClient.DisposeAsync();
                gameHubClient = null;
            }

            if (channel != null)
            {
                channel.ShutdownAsync();
                channel = null;
            }
            
            connection.Cancel();
        }

        private async UniTask TryReconnectServer(Action onSuccessReconnect, int retryCount = 0)
        {
            gameHubClient?.DisposeAsync();
            gameHubClient = null;
            channel?.ShutdownAsync();
            channel = null;

            if (ConnectedCancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (retryCount == 0)
            {
                //AppManager.Instance.UIManager.System.TouchGuard.Show();
                //AppManager.Instance.UIManager.System.Loading.Show();
            }

            if(retryCount < GuildBattleCommonConst.MaxReConnectCount)
            {
                Logger.Log($"Try reconnect to server. RetryCount: {retryCount}");
                var joinResult = await JoinAsync(hostUrl, roomName, matchingId, onSuccessReconnect);
                Logger.Log($"Tried reconnect to server. IsSucceed: {joinResult}");
                
                if (ConnectedCancellationToken.IsCancellationRequested)
                {
                    return;
                }
                
                switch (joinResult)
                {
                    case GuildBattleCommonConst.JoinResult.None:
                        break;
                    case GuildBattleCommonConst.JoinResult.Success:
                        onSuccessReconnect?.Invoke();
                        break;
                    case GuildBattleCommonConst.JoinResult.Error:
                        TryReconnectServer(onSuccessReconnect, ++retryCount).Forget();
                        break;
                    case GuildBattleCommonConst.JoinResult.BattleStateError:
                        // 終了済みギルバトに接続しようとした場合はエラーにしてギルドトップに返す.
                        TryReconnectServer(onSuccessReconnect, GuildBattleCommonConst.MaxReConnectCount).Forget();
                        break;
                }
                
                return;
            }
            
            //AppManager.Instance.UIManager.System.TouchGuard.Hide();
            //AppManager.Instance.UIManager.System.Loading.Hide();
            Logger.LogError($"Failed to reconnect to server. RetryCount: {retryCount}");
            OpenDisconnectedModal();
        }

        public async UniTask SendOnReady()
        {
            await gameHubClient.OnReady();
        }
        
        public async UniTask<GuildBattlePlayerData> RequestSendUnit(int partyId, int targetSpotId, long[] unitIds, int militaryStrength, long tacticsId, long[] roleOperationIds)
        {
            var sendUnitCommand = new SendPartyCommand();
            sendUnitCommand.DepartedPartyId = partyId;
            sendUnitCommand.TargetSpotId = targetSpotId;
            sendUnitCommand.GroupUnitIdList = unitIds;
            sendUnitCommand.TotalMilitaryStrength = militaryStrength;
            sendUnitCommand.TacticsId = tacticsId;
            sendUnitCommand.RoleOperationIds = roleOperationIds;
            var ret = await gameHubClient.RequestSendParty(sendUnitCommand);
            if (ret != null)
            {
                ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.Invoke(ret);
            }
            return ret;
        }

        public async UniTask<bool> RequestDissolutionParty(int partyId)
        {
            var dissolutionPartyCommand = new DissolutionPartyCommand();
            dissolutionPartyCommand.TargetPartyId = partyId;
            var ret = await gameHubClient.RequestDissolutionParty(dissolutionPartyCommand);
            return ret;
        }
        
        public async UniTask<bool> SendChat(string body, long stampId)
        {
            var ret = await gameHubClient.SendChat(body, stampId);
            return ret;
        }

        public async UniTask<GuildBattlePlayerData> RequestUseItem(long itemId)
        {
            var updatedData = await gameHubClient.RequestUseItem(itemId);
            if (updatedData != null)
            {
                PjfbGuildBattleDataMediator.Instance.UpdateGuildBattlePlayerData(updatedData);
                ClubRoyalInGameUIMediator.Instance.ActiveItemListUI.UpdateUI(updatedData);
                ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.UpdateUI(true, false);
            }
            
            return updatedData;
        }
        
        public async UniTask<GuildBattlePlayerData> RequestUseYellAbility(long mCharaId, long abilityId)
        {
            GuildBattlePlayerData updatedData = await gameHubClient.RequestUseYellAbility(mCharaId, abilityId);

            if (updatedData != null)
            {
                // 関連する表示反映
                ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.Invoke(updatedData);
            }
            else
            {
                Logger.LogError($"RequestUseYellAbility failed. Updated data is null.");
                return null;
            }
            
            GuildBattlePlayerData prePlayerData = PjfbGuildBattleDataMediator.Instance.PjfbBattlePlayerData[PjfbGuildBattleDataMediator.Instance.PlayerIndex];
            long preMilitaryStrength = prePlayerData.AvailableMilitaryStrength;
            
            if( preMilitaryStrength < updatedData.AvailableMilitaryStrength)
            {
                PjfbGuildBattleDataMediator.Instance.UpdateGuildBattlePlayerData(updatedData);
                ClubRoyalInGameUIMediator.Instance.RemainBallCountUI.UpdateUI(true, true);
            }
            
            // クールタイムの減少があった場合はアイコンをキラリと光らせる
            var ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(v=>v.id == abilityId);
            if( ability != null )
            {
                foreach (BattleV2AbilityEffect effect in ability.abilityEffectList)
                {
                    switch ((BattleConst.AbilityEffectType)effect.effectType)
                    {
                        case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrement:
                        case BattleConst.AbilityEffectType.GuildBattleCoolTimeTurnDecrementMultiply:
                            // クールタイム系のエフェクトは表示更新のイベント登録を行う（登録済み判定などはメソッド内で行っている）
                            ClubRoyalInGameUIMediator.Instance.ActivePartyListUI.UpdateUI();
                            ClubRoyalInGameUIMediator.Instance.ActivePartyListUI.GrowEffectAtCoolTimePartyEntry(updatedData);
                            break;
                    }
                }
            }

            return updatedData;
        }
        
        

        public static void OpenDisconnectedModal()
        {
            Instance?.ConnectedCancellationSource.Cancel();
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            var args = new ConfirmModalData(
                StringValueAssetLoader.Instance["clubroyalingame.disconnected_from_server"],
                StringValueAssetLoader.Instance["clubroyalingame.disconnected_from_server_message"],
                string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], modal =>
                {
                    modal.Close();
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null);
                }));
            
            AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, args);
        }
        
#endregion

    }
}