using System;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.InGame;
using Pjfb.Menu;
using Pjfb.Shop;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using UnityEngine;

namespace Pjfb.Colosseum
{

    public enum ColosseumState
    {
        Unknown,
        ExitSeason,
        NextSeason,
        OnSeason
    }

    public enum ScoreCorrectionType
    {
        Time = 1,
        DefenseCount = 2
    }
    
    public enum ColosseumGroupType
    {
        Club = 1,
        NPC = 2,
    }
    
    public enum ColosseumPlayerType
    {
        Player = 1,
        NPC = 2
    }

    public class ColosseumInGameParam
    {
        public long sColosseumEventId;
        public long useDeckId;

        public ColosseumInGameParam(long _sColosseumEventId, long _useDeckId)
        {
            sColosseumEventId = _sColosseumEventId;
            useDeckId = _useDeckId;
        }
    }
    
    public static class ColosseumManager
    {
        public const int RankMatchRewardPointId = 5;
        public const int ColosseumLockId = 310001;
        public const float ConsecutivePenaltyCoef = 0.1f;

        private static long cachedOpponentRanking;
        private static long cachedOpponentUserType;
        private static long cachedOpponentUserId;
        private static long cachedOpponentCostType;
        private static long cachedOpponentCostValue;
        
        // 結果（0 => 結果なし, 1 => 勝利, 2 => 敗北, 3 => 引き分け, 99 => 未実施）
        public const long None = 0;
        public const long ResultWin = 1;
        public const long ResultLose = 2;
        public const long ResultDraw = 3;
        public const long ResultNone = 99;
        
        // 最低ランク
        public const long MinGradeNumber = 1;
        // 1ランクあたりの最大ポイント
        public const long MaxRankPointPerGradeNumber = 100;

        #region StaticMethod

        public static bool IsUnLockColosseum()
        { 
            return UserDataManager.Instance.IsUnlockSystem(ColosseumLockId) || SystemUnlockDataManager.Instance.IsUnlockingSystem(ColosseumLockId);
        }

        public static bool GetHomeBadgeFlag()
        {
            if (!IsUnLockColosseum())
            {
                // 機能未解放
                return false;
            }

            var master = MasterManager.Instance.colosseumEventMaster.GetAvailableColosseumMaster();
            if (master == null)
            {
                // ColosseumEventなし
                return false;
            }
            
            var seasonHome = UserDataManager.Instance.GetAvailableSeasonHome(master.id);
            if (seasonHome == null)
            {
                // 集計中
                return false;
            }
            
            var colosseumState = GetColosseumState(seasonHome);
            if (colosseumState != ColosseumState.OnSeason)
            {
                // シーズン開催中ではない
                return false;
            }
            
            return StaminaUtility.GetStamina(master.mStaminaId) > 0;
        }

        public static string GetHomeColosseumButtonNotificationLabel()
        {
            if (!IsUnLockColosseum())
            {
                // 機能未解放
                return string.Empty;
            }
            
            var master = MasterManager.Instance.colosseumEventMaster.GetAvailableColosseumMaster();
            if (master == null)
            {
                // ColosseumEventなし
                return string.Empty;
            }
            
            var seasonHome = UserDataManager.Instance.GetAvailableSeasonHome(master.id);
            if (seasonHome == null)
            {
                // 集計中
                return string.Empty;
            }
            
            var colosseumState = GetColosseumState(seasonHome);

            var nextSeasonPeriod = string.Empty;
            if (colosseumState == ColosseumState.NextSeason)
            {
                var timeString = seasonHome.startAt.TryConvertToDateTime().GetDateTimeString();
                nextSeasonPeriod = string.Format(StringValueAssetLoader.Instance["pvp.home.notification"],timeString);
            }

            return nextSeasonPeriod;
        }

        public static void TrySeasonResult(ColosseumClientHandlingType handlingType, Action onFinish)
        {
            var colosseumUnreadFinishedList = UserDataManager.Instance.ColosseumSeasonDataList.Values
                .Where(seasonData => seasonData.IsHandlingType(handlingType) && seasonData.UnreadUserSeasonStatus != null)
                .Select(seasonData => seasonData.UnreadUserSeasonStatus).ToArray();
            
            if (colosseumUnreadFinishedList == null ||
                colosseumUnreadFinishedList.Length == 0)
            {
                onFinish.Invoke();
                return;
            }
            
            OpenColosseumResultModal(colosseumUnreadFinishedList, handlingType, onFinish);
        }

        /// <summary>
        /// 対人戦リザルトモーダルを開く
        /// </summary>
        public static void OpenColosseumResultModal(ColosseumUserSeasonStatus[] userSeasonStatusList, ColosseumClientHandlingType handlingType, Action onFinish)
        {
            // 各種演出の再生
            switch (handlingType)
            {
                case ColosseumClientHandlingType.PvP:
                    HomeEndOfSeasonModal.Open(new HomeEndOfSeasonModal.Parameters(userSeasonStatusList, onFinish));
                    break;
                case ColosseumClientHandlingType.ClubMatch:
                    HomeEndOfSeasonClubMatchModal.Open(new HomeEndOfSeasonClubMatchModal.Parameters{clubSeasonStatusList = userSeasonStatusList, onFinish = onFinish});
                    break;
                case ColosseumClientHandlingType.LeagueMatch:
                    // シーズン情報
                    ColosseumUserSeasonStatus seasonStatus = userSeasonStatusList.OrderByDescending(v => v.startAt).FirstOrDefault();
                    // シーズン終了結果
                    // JsonUtilityの仕様で2階層目以降のクラスはnullにならないっぽいので、sColosseumGroupStatusIdで判定
                    // 入れ替え戦が行われない||入れ替え戦は行われるが結果が出ていない
                    if(seasonStatus.groupSeasonStatus.shiftMatchInfo == null || seasonStatus.groupSeasonStatus.shiftMatchInfo.sColosseumGroupStatusId <= 0 || seasonStatus.groupSeasonStatus.shiftMatchInfo.result == 0 ||seasonStatus.groupSeasonStatus.shiftMatchInfo.result == ResultNone )
                    {
                        AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.HomeEndOfSeasonLeagueMatch, new Pjfb.LeagueMatch.HomeEndOfSeasonLeagueMatchModal.Parameters(userSeasonStatusList, onFinish) );
                    }
                    // 入れ替え戦結果
                    else
                    {
                        AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.HomeEndOfPromotionAndRelegation, new Pjfb.LeagueMatch.HomeEndOfPromotionAndRelegationModal.Parameters(userSeasonStatusList, onFinish) );
                    }
                    break;
                case ColosseumClientHandlingType.InstantTournament:
                    // 結果演出再生
                    AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.HomeEndOfInstantTournament, new LeagueMatch.HomeEndOfInstantTournamentModal.Parameters(userSeasonStatusList, onFinish) );
                    break;
                case ColosseumClientHandlingType.ClubRoyal:
                    AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.HomeEndOfSeasonClubRoyal, new ClubRoyal.HomeEndOfSeasonClubRoyalModal.Param(userSeasonStatusList,onFinish));
                    break;
                default:
                    CruFramework.Logger.LogError("そのリザルトは現在未対応です");
                    break;
            }
        }

        public static async void OnClickColosseumInGameButton(PageType openFrom, long opponentUserType, long opponentUserId,
            long opponentRanking, long costType, long costValue, long sColosseumEventId, long useDeckId)
        {
            var param = new ColosseumInGameParam(sColosseumEventId, useDeckId);
            var battleData = await RequestColosseumGetBattleData(opponentUserType, opponentUserId, opponentRanking, costType, costValue, sColosseumEventId);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, true,
                new NewInGameOpenArgs(openFrom, battleData, param));
        }

        public static ColosseumState GetColosseumState(ColosseumSeasonHome seasonHome)
        {
            if (seasonHome == null)
            {
                return ColosseumState.ExitSeason;
            }
            
            var now = AppTime.Now;
            var startAt = seasonHome.startAt.TryConvertToDateTime();
            var endAt = seasonHome.endAt.TryConvertToDateTime();

            if (startAt.IsPast(now) && endAt.IsFuture(now))
            {
                return ColosseumState.OnSeason;
            }

            if (startAt.IsFuture(now))
            {
                return ColosseumState.NextSeason;
            }

            return ColosseumState.ExitSeason;
        }
        
        public static ColosseumEventMasterObject GetNextColosseumEventMasterObject(ColosseumSeasonHome[] seasonHomeList)
        {
            var now = AppTime.Now;
            var seasonEvent = seasonHomeList.FirstOrDefault(seasonHome =>
                seasonHome.startAt.TryConvertToDateTime().IsFuture(now));

            if (seasonEvent == null) return null;

            return MasterManager.Instance.colosseumEventMaster.FindData(seasonEvent.id);
        }
        
        public static async UniTask GetUserSeasonStatusAsync(long seasonId)
        {
            var seasonData = UserDataManager.Instance.GetColosseumSeasonData(seasonId);
            var status = seasonData?.UserSeasonStatus;
            if (status == null)
            {
                await RequestJoinSeasonAsync(seasonId);
            }
            else
            {
                await RequestGetSeasonStatusAsync(seasonId);
                seasonData = UserDataManager.Instance.GetColosseumSeasonData(seasonId);
                // 状態がわからず更新できていない場合もあるため更新後のidチェック
                if (seasonData.UserSeasonStatus == null || seasonData.UserSeasonStatus.sColosseumEventId == 0)
                {
                    await RequestJoinSeasonAsync(seasonId);
                }               
            }
        }
        
        private static async UniTask RequestJoinSeasonAsync(long sColosseumEventId)
        {
            var request = new ColosseumJoinSeasonAPIRequest();
            var post = new ColosseumJoinSeasonAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            UserDataManager.Instance.UpdateColosseumSeasonStatus(response.userSeasonStatus);
            UserDataManager.Instance.UpdateColosseumDailyStatus(sColosseumEventId, response.dailyStatus);
        }
        
        private static async UniTask RequestGetSeasonStatusAsync(long sColosseumEventId)
        {
            var request = new ColosseumGetSeasonStatusAPIRequest();
            var post = new ColosseumGetSeasonStatusAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            UserDataManager.Instance.UpdateColosseumSeasonStatus(response.userSeasonStatus);
            UserDataManager.Instance.UpdateColosseumDailyStatus(sColosseumEventId, response.dailyStatus);
        }

        public static async UniTask<ColosseumRankingUser[]> RequestTargetDataAsync(long sColosseumEventId)
        {
            var request = new ColosseumGetTargetListAPIRequest();
            var post = new ColosseumGetTargetListAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            var targetData = response.targetData;
            UserDataManager.Instance.UpdateColosseumSeasonStatus(targetData.userSeasonStatus);
            return targetData.targetList;
        }
        
        public static async UniTask<ColosseumRankingUser[]> RequestGetRankingAsync(long sColosseumEventId,long roomCapacity)
        {
            var request = new ColosseumGetRankingAPIRequest();
            var post = new ColosseumGetRankingAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            UserDataManager.Instance.UpdateColosseumSeasonStatus(response.userSeasonStatus);
            return response.rankingList;
        }

        public static async UniTask<ColosseumDeck> RequestColosseumDeckAsync(long sColosseumEventId, long targetUMasterId, long userType)
        {
            var request = new ColosseumGetDeckAPIRequest();
            var post = new ColosseumGetDeckAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            post.targetUMasterId = targetUMasterId;
            post.userType = userType;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.deck;
        }

        public static async UniTask<BattleV2ClientData> RequestColosseumGetBattleData(long opponentUserType, long opponentUserId, long opponentRanking, long costType, long costValue,long sColosseumEventId)
        {
            var request = new ColosseumGetBattleDataAPIRequest();
            var post = new ColosseumGetBattleDataAPIPost();

            cachedOpponentUserId = opponentUserId;
            cachedOpponentUserType = opponentUserType;
            cachedOpponentRanking = opponentRanking;
            cachedOpponentCostType = costType;
            cachedOpponentCostValue = costValue;

            post.opponentUserType = opponentUserType;
            post.opponentUserId = opponentUserId;
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.clientData;
        }

        public static async UniTask<ColosseumAttackAPIResponse> RequestColosseumAttackAsync(
            BattleV2ClientData clientData, int resultType, int pointGet, int pointLost, long sColosseumEventId)
        {
            
            var log = new BattleV2BattleLog();
            log.result = resultType;
            log.pointGet = pointGet;
            log.pointLost = pointLost;

            var deckList = new List<ColosseumDeck>();
            
            foreach (var player in clientData.playerList)
            {
                var deck = new ColosseumDeck();
                var index = player.playerIndex;
                var charaVariableList = clientData.charaList.Where(chara => chara.playerIndex == index).ToArray();
                BigValue combatPower = charaVariableList.Sum(chara => new BigValue(chara.combatPower));
                deck.combatPower = combatPower.ToString();
                deck.rank = StatusUtility.GetPartyRank(combatPower);
                deck.player = player;
                deck.charaVariableList = charaVariableList;
                var charaList = new List<ColosseumDeckChara>();
                foreach (var charaVariable in charaVariableList)
                {
                    var chara = new ColosseumDeckChara();
                    chara.mCharaId = charaVariable.mCharaId;
                    chara.visualKey = charaVariable.visualKey;
                    chara.rank = charaVariable.rank;
                    chara.roleNumber = charaVariable.roleNumber;
                    chara.combatPower = charaVariable.combatPower;
                    charaList.Add(chara);
                }
                deck.charaList = charaList.ToArray();
                deckList.Add(deck);
            }

            log.deckList = deckList.ToArray();

            var request = new ColosseumAttackAPIRequest();
            var post = new ColosseumAttackAPIPost();

            post.sColosseumEventId = sColosseumEventId;
            post.opponentUserId = cachedOpponentUserId;
            post.opponentUserType = cachedOpponentUserType;
            post.opponentRanking = cachedOpponentRanking;
            post.costTypeForValidation = cachedOpponentCostType;
            post.costValueForValidation = cachedOpponentCostValue;
            post.battleLog = log;
            post.getTurn = 1;
            request.SetPostData(post);
            
            await APIManager.Instance.Connect(request);

            var response = request.GetResponseData();
            UserDataManager.Instance.UpdateColosseumSeasonStatus(response.userSeasonStatus);
            UserDataManager.Instance.UpdateColosseumDailyStatus(sColosseumEventId, response.dailyStatus);

            var cData = UserDataManager.Instance.GetColosseumSeasonData(response.userSeasonStatus.sColosseumEventId);
            cData.ScoreBattleTurn = response.scoreBattleTurn;
            
            if (response.deckList != null)
            {
                await DeckUtility.UpdateClubDeckFatigue(response.deckList);
            }
            
            return response;
        }

        public static async UniTask<ColosseumHistory[]> RequestGetHistoryListAsyncByMColosseumEventId(long mColosseumEventId)
        {
            return await RequestGetHistoryListAsync(mColosseumEventId, 0);
        }
        
        public static async UniTask<ColosseumHistory[]> RequestGetHistoryListAsyncBySColosseumEventId(long sColosseumEventId)
        {
            return await RequestGetHistoryListAsync(0, sColosseumEventId);
        }
        
        public static async UniTask<ColosseumHistory[]> RequestGetHistoryListAsync(long mColosseumEventId, long sColosseumEventId)
        {
            if (mColosseumEventId == 0 && sColosseumEventId == 0)
            {
                return Array.Empty<ColosseumHistory>();
            }
            
            var request = new ColosseumGetHistoryListAPIRequest();
            var post = new ColosseumGetHistoryListAPIPost();
            post.mColosseumEventId = mColosseumEventId;
            post.sColosseumEventId = sColosseumEventId;
            post.lastId = -1;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.historyList;
        }
        
        public static async UniTask<ColosseumDeck> RequestGetHistoryDeckAsync(long uColosseumBattleHistoryId)
        {
            var request = new ColosseumGetHistoryDeckAPIRequest();
            var post = new ColosseumGetHistoryDeckAPIPost();
            post.uColosseumBattleHistoryId = uColosseumBattleHistoryId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.deck;
        }
        
        public static async UniTask<ColosseumDeck> RequestBattleReserveFormationGetHistoryDeckAsync(long uBattleReserveFormationHistoryId)
        {
            var request = new BattleReserveFormationGetHistoryDeckAPIRequest();
            var post = new BattleReserveFormationGetHistoryDeckAPIPost();
            post.id = uBattleReserveFormationHistoryId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.deck;
        }

        public static async UniTask<ColosseumUserSeasonStatus[]> RequestGetUserSeasonStatusPastList(long mColosseumEventId, long lastSeasonId)
        {
            var request = new ColosseumGetUserSeasonStatusPastListAPIRequest();
            var post = new ColosseumGetUserSeasonStatusPastListAPIPost();
            post.mColosseumEventId = mColosseumEventId;
            post.lastSeasonId = lastSeasonId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response.userSeasonStatusList;
        }

        public static async UniTask RequestReadFinished(long sColosseumEventId)
        {
            var request = new ColosseumReadFinishedAPIRequest();
            var post = new ColosseumReadFinishedAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
        }

        public static async UniTask RequestReadFinished(long[] sColosseumEventIdList)
        {
            var request = new ColosseumReadFinishedListAPIRequest();
            var post = new ColosseumReadFinishedListAPIPost();
            post.sColosseumEventIdList = sColosseumEventIdList;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
        }

        public static async UniTask<ColosseumRankingGroup[]> ColosseumGetGroupBattleRanking(long sColosseumEventId = 0)
        {
            var request = new ColosseumGetGroupBattleRankingAPIRequest();
            var post = new ColosseumGetGroupBattleRankingAPIPost();

            post.sColosseumEventId = sColosseumEventId;
            post.getTurn = 1;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            var cData = UserDataManager.Instance.GetColosseumSeasonData(response.userSeasonStatus.sColosseumEventId);
            cData.UserSeasonStatus = response.userSeasonStatus;
            cData.BattleRankingGroups = response.rankingGroupList;
            cData.ScoreBattleTurn = response.scoreBattleTurn;
            return response.rankingGroupList;
        }

        public static bool IsBonusRanking(long sColosseumEventId, long ranking)
        {
            return GetBonusValue(sColosseumEventId,ranking) > 0;
        }

        public static long GetBonusValue(long sColosseumEventId, long ranking)
        {
            var seasonData = UserDataManager.Instance.GetColosseumSeasonData(sColosseumEventId);
            if (seasonData == null)
            {
                return 0;
            }
            
            var bonusRankList = seasonData.ScoreBattleTurn.bonusRankList;
            var rankBonusArray = bonusRankList?.FirstOrDefault(rankArray => rankArray.l[0] == ranking);
            return (rankBonusArray != null) ? rankBonusArray.l[1] : 0;
        }

        public static float GetPenaltyCoefficient(long defenseCount)
        {
            if (defenseCount <= 0) return 0f;

            var coefList = ConfigManager.Instance.colosseum.scoreMatchDefenseDebuffRateList.OrderByDescending(item => item.l[0]);
            var coef = coefList.FirstOrDefault(item => item.l[0] == defenseCount);
            if (coef == null) coef = coefList.First();

            return (float)coef.l[1] / 10000;
        }

        public static BigValue GetClubMatchTotalCombatPower(long defenseCount, BigValue combatPower)
        {
            var penaltyValue = (int)(GetPenaltyCoefficient(defenseCount) * 100);
            return (combatPower * (100 - penaltyValue) / 100);
        }

        /// <summary>
        /// クラブマッチの獲得予定スコア内訳データ
        /// </summary>
        public class ClubMatchExpectedScoreData
        {
            // 順位スコア
            public long baseScore;
            
            // ボーナスは100倍の値（100%なら10000）
            // 順位ボーナス
            public long rankBonusMultiplier;
            
            public long totalScore => baseScore * (10000 + rankBonusMultiplier) / 10000;
        }
        /// <summary>
        /// クラブマッチの獲得予定スコア内訳計算
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="scoreBattleTurn"></param>
        /// <returns></returns>
        public static ClubMatchExpectedScoreData GetExpectedScoreData(ColosseumRankingUser userData, ColosseumScoreBattleTurn scoreBattleTurn)
        {
            ClubMatchExpectedScoreData scoreData = new ClubMatchExpectedScoreData();
            // 順位スコア
            scoreData.baseScore = MasterManager.Instance.colosseumScoreBattleScoreBaseMaster.GetBattleScoreBase(userData.ranking, (int)ScoreBaseType.BaseScore);
            
            //順位ボーナス
            scoreData.rankBonusMultiplier = scoreBattleTurn?.bonusRankList?.FirstOrDefault(data=> data.l[0] == userData?.ranking)?.l[1] ?? 0;

            return scoreData;
        }
        
        public static void OpenUserProfileModal(long targetUMasterId, ColosseumPlayerType playerType)
        {
            // NPC
            if(playerType == ColosseumPlayerType.NPC)
            {
                OpenFailedToGetDataModal();
                return;
            }
            
            if(targetUMasterId == 0)
            {
                return;
            }
            
            // プロフィールウィンドウを開く
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(targetUMasterId, false);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        
        public static void OpenFailedToGetDataModal()
        {
            ConfirmModalData data = new ConfirmModalData();
            // エラー
            data.Title = StringValueAssetLoader.Instance["common.error"];
            // データの取得に失敗
            data.Message = StringValueAssetLoader.Instance["common.failed_to_get_data"];
            // 閉じる
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.ok"],
                window => window.Close()
            );
            // モーダル開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        public static long GetColosseumGroupId(long eventId)
        {
            ColosseumEventMasterObject eventMaster = MasterManager.Instance.colosseumEventMaster.FindData(eventId);
            return eventMaster.mColosseumGradeGroupId;
        }
        
        /// <summary>
        /// 最高ランクの取得
        /// </summary>
        public static long GetTopRank(long eventId)
        {
            ColosseumEventMasterObject mColosseumEvent = MasterManager.Instance.colosseumEventMaster.FindData(eventId);
        
            return MasterManager.Instance.colosseumGradeMaster.values
                .Where(m => m.mColosseumGradeGroupId == mColosseumEvent.mColosseumGradeGroupId)
                .Max(x => x.gradeNumber);
        }
        
        /// <summary>
        /// ランク名を取得
        /// </summary>
        public static string GetGradeName(long eventId, long gradeNumber)
        {
            ColosseumGradeMasterContainer gradeMasterContainer = MasterManager.Instance.colosseumGradeMaster;
            foreach (ColosseumGradeMasterObject gradeMaster in gradeMasterContainer.values)
            {
                if (gradeMaster.mColosseumGradeGroupId == ColosseumManager.GetColosseumGroupId(eventId) && gradeMaster.gradeNumber == gradeNumber)
                {
                    return gradeMaster.name;
                }
            }
            return string.Empty;
        }
        
        /// <summary>
        /// 入れ替え戦に参加するかどうかをチェックする
        /// </summary>
        /// <param name="seasonStatus"></param>
        /// <returns>入れ替え戦に参加する場合はtrue</returns>
        public static bool IsDecisionShiftMatch(ColosseumUserSeasonStatus seasonStatus)
        {
            return seasonStatus.groupSeasonStatus.shiftMatchInfo != null && seasonStatus.groupSeasonStatus.shiftMatchInfo.sColosseumGroupStatusId > 0;
        }

        /// <summary>
        /// 入れ替え戦が終わったかどうかをチェックする
        /// </summary>
        /// <param name="seasonStatus"></param>
        /// <returns>入れ替え戦が終わっている場合はtrue</returns>
        public static bool IsFinishShiftMatch(ColosseumUserSeasonStatus seasonStatus)
        {
            return IsDecisionShiftMatch(seasonStatus) && HasResult(seasonStatus.groupSeasonStatus.shiftMatchInfo.result);
        }

        //// <summary> 結果があるか </summary>
        public static bool HasResult(long result)
        {
            return result != None && result != ResultNone;
        }
        
        /// <summary>
        /// 入れ替え戦なしで昇格したかどうかをチェックする
        /// </summary>
        /// <param name="seasonStatus"></param>
        /// <returns>入れ替え戦なしで昇格していたらtrue</returns>
        public static bool IsGradeUpNoShiftMatch(ColosseumUserSeasonStatus seasonStatus)
        {
            return !IsDecisionShiftMatch(seasonStatus) && seasonStatus.groupSeasonStatus.gradeAfter > seasonStatus.groupSeasonStatus.gradeNumber;
        }

        /// <summary> エントリー済みのリストに含まれているか </summary>
        public static bool IsEntry(long id)
        {
            return UserDataManager.Instance.EntryColosseumIdList != null && UserDataManager.Instance.EntryColosseumIdList.Contains(id);
        }

        /// <summary> 既読リストに存在しないイベントか？ </summary>
        public static bool IsNewEvent(long id)
        {
            return LocalSaveManager.saveData.viewedMColosseumEventIdList.Contains(id) == false;
        }
        
        /// <summary> m_colosseum_EventIdを既読リストに追加 </summary>
        public static void SaveViewedEventId(long id)
        {
            // すでに追加済みなら無視
            if (LocalSaveManager.saveData.viewedMColosseumEventIdList.Contains(id)) return;
            
            // 既読リストに追加
            LocalSaveManager.saveData.viewedMColosseumEventIdList.Add(id);
            // 保存
            LocalSaveManager.Instance.SaveData();
        }
        #endregion
    }
}