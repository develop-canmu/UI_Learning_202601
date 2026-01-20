using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Colosseum;
using Pjfb.Community;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using Pjfb.Extensions;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Training;

namespace Pjfb.LeagueMatch
{
    public enum SeasonEndResultType
    {
        PromotionBattleForUp = 1,
        PromotionBattleForDown = 2,
        Keep = 3,
        KeepTop = 4,
        PromotionDirectly = 5
    }

    public enum BattleImportanceLabelNumber
    {
        Normal = 1,
        Bronze = 2,
        Silver = 3,
        Gold = 4
    }
    
    public static class LeagueMatchUtility
    {
        public static BattleReserveFormationMatchInfo MatchInfoCache { get; private set; } = null;
        public static void SetMatchInfo(BattleReserveFormationMatchInfo matchInfo)
        {
            MatchInfoCache = matchInfo;
        }
        
        public static long SelectedRoundNumber { get; private set; } = 0;
        public static void SetSelectedRoundNumber(long selectedRoundNumber)
        {
            SelectedRoundNumber = selectedRoundNumber;
        }
        
        public const long RefreshInterval = 60;
        
        public static long GetEntryCount()
        {
            if (MatchInfoCache == null)
            {
                return 0;
            }

            long entryCount = 0;
            foreach (var matchLineup in MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId)
                {
                    entryCount++;
                }
            }

            return entryCount;
        }

        //// <summary> 指定したデッキが登録されている試合番号を取得 (登録されていないなら０を返す)</summary>
        public static long GetEntryRoundNumber(long deckId)
        {
            foreach (var matchLineup in LeagueMatchUtility.MatchInfoCache.matchLineupList)
            {
                // スロットが空ならとばす
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                // スロットに登録されているデッキか
                if (matchLineup.playerInfo.partyNumber != deckId) continue;
                // 自分のパーティのみ
                if (matchLineup.playerInfo.player.playerId != UserDataManager.Instance.user.uMasterId) continue;
                return matchLineup.roundNumber;
            }
            return 0;
        }

        public static long GetRemainingEntryCount(long entryCount, ColosseumClientHandlingType handlingType)
        {
            // 残りの登録可能数
            long remainingEntryCount = 0;
            var mBattleReserveFormationMasterObject = MasterManager.Instance.battleReserveFormationMaster.FindData(LeagueMatchUtility.GetLeagueMatchInfo(handlingType).SeasonData.MColosseumEvent.inGameSystemId);
            remainingEntryCount = mBattleReserveFormationMasterObject.oneUserReserveCount - entryCount;
            
            return remainingEntryCount;
        }

        public static bool GetMyRegisteredDeck(DeckData data)
        {
            if (MatchInfoCache == null)
            {
                return false;
            }
            
            foreach (var matchLineup in MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.roundNumber == data.Deck.partyNumber) continue;
                return true;
            }

            return false;
        }

        public static async UniTask<ColosseumGetGroupLeagueBoardAPIResponse> GetLeagueBoardInfo(long seasonId)
        {
            // API実行
            ColosseumGetGroupLeagueBoardAPIRequest request = new ColosseumGetGroupLeagueBoardAPIRequest();
            ColosseumGetGroupLeagueBoardAPIPost post = new ColosseumGetGroupLeagueBoardAPIPost();
            post.sColosseumEventId = seasonId;
            request.SetPostData(post);
            
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            return response;
        }
        
        public static LeagueMatchInfo GetLeagueMatchInfo(ColosseumClientHandlingType handlingType)
        {
            try
            {
                // シーズン中の情報取得
                foreach (ColosseumSeasonData seasonData in UserDataManager.Instance.ColosseumSeasonDataList.Values)
                {
                    // シーズン中じゃない 
                    if(!seasonData.IsOnSeason) continue;
                    // マスタがない
                    if(seasonData.MColosseumEvent == null) continue;
                    // リーグマッチじゃない
                    if(seasonData.MColosseumEvent.clientHandlingType != handlingType) continue;
                    // データを返す
                    return new LeagueMatchInfo(seasonData);
                }
                
                // シーズン期間外の情報取得
                ColosseumEventMasterObject[] sortedMasters = MasterManager.Instance.colosseumEventMaster.values.OrderBy(m => m.id).ToArray();
                foreach (ColosseumEventMasterObject mColosseumEvent in sortedMasters)
                {
                    // リーグマッチじゃない
                    if(mColosseumEvent.clientHandlingType != handlingType) continue;
                    // 期間外
                    if(mColosseumEvent.endAt.TryConvertToDateTime().IsPast(AppTime.Now)) continue;
                    // データを返す
                    return new LeagueMatchInfo(mColosseumEvent);
                }
            }
            catch (Exception e)
            {
                CruFramework.Logger.LogError(e.Message);
                return null;
            }
            
            return null;
        }
        
        public static async UniTask<BattleReserveFormationMatchInfo> GetMatchInfo(long id)
        {
            BattleReserveFormationGetMatchInfoAPIRequest request = new BattleReserveFormationGetMatchInfoAPIRequest();
            BattleReserveFormationGetMatchInfoAPIPost post = new BattleReserveFormationGetMatchInfoAPIPost();
            
            request.SetPostData(post);
            post.id = id;
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            MatchInfoCache = response.matchInfo;
            return response.matchInfo;
        }

        public static async UniTask<BattleReserveFormationMatchInfo> GetMatchInfoByEventInfo(long eventType, long eventId)
        {
            BattleReserveFormationGetMatchInfoByEventInfoAPIRequest request =
                new BattleReserveFormationGetMatchInfoByEventInfoAPIRequest();

            BattleReserveFormationGetMatchInfoByEventInfoAPIPost post =
                new BattleReserveFormationGetMatchInfoByEventInfoAPIPost();
            
            request.SetPostData(post);
            post.eventType = eventType;
            post.eventId = eventId;
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            MatchInfoCache = response.matchInfo;
            return response.matchInfo;
        }
        
        public static async UniTask<ColosseumGetGroupLeagueMatchHistoryAPIResponse> GetMatchHistory(long sColosseumEventId)
        {
            // Post
            ColosseumGetGroupLeagueMatchHistoryAPIPost post = new ColosseumGetGroupLeagueMatchHistoryAPIPost();
            post.sColosseumEventId = sColosseumEventId;
            // Request
            ColosseumGetGroupLeagueMatchHistoryAPIRequest request = new ColosseumGetGroupLeagueMatchHistoryAPIRequest();
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            // レスポンス取得
            ColosseumGetGroupLeagueMatchHistoryAPIResponse response = request.GetResponseData();
            
            return response;
        }
        
        public static SeasonEndResultType GetSeasonResult(LeagueMatchInfo leagueMatchInfo)
        {
            ColosseumGroupSeasonStatus groupSeasonStatus = leagueMatchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus;
            if (!leagueMatchInfo.CanShiftBattle) // 入れ替え戦なし
            {
                if (groupSeasonStatus.gradeAfter > groupSeasonStatus.gradeBefore) // 不戦勝で昇格決定！
                {
                    return SeasonEndResultType.PromotionDirectly;
                }
                else // 残留決定！
                {
                    if (groupSeasonStatus.gradeAfter == leagueMatchInfo.GetTopRank())
                    {
                        return SeasonEndResultType.KeepTop;
                    }
                    else
                    {
                        return SeasonEndResultType.Keep;
                    }
                }
            }
            else // 入れ替え戦あり
            {
                if (groupSeasonStatus.gradeBefore < groupSeasonStatus.shiftMatchInfo.gradeNumber) // 昇格入れ替え戦進出決定！
                {
                    return SeasonEndResultType.PromotionBattleForUp;
                }
                else　// 降格入れ替え戦参加決定！
                {
                    return SeasonEndResultType.PromotionBattleForDown;
                }
            }
        }
        
        public static UniTask OpenClubInfo(long groupId, long groupType)
        {
            return ClubMatchUtility.OpenClubInfo(groupId, 0, groupType);
        }
        
        public static void UpdateMenuNotification(UIBadgeNotification notification)
        {
            var unViewedChatCount = CommunityManager.ShowClubChatBadge;
            notification.SetActive(unViewedChatCount);
        }
        
        public static long GetNpcIconId(string name)
        {
            // 名前が空文字
            if(string.IsNullOrEmpty(name)) return 0;
            
            // エフェクト表示のないアイコン一覧
            IconMasterObject[] mIcons = MasterManager.Instance.iconMaster.values
                .Where(v => v.imageEffectId <= 0)
                .ToArray();
            // ハッシュ値の絶対値
            int hashCode = UnityEngine.Mathf.Abs(name.GetHashCode());
            // 名前のハッシュ値とアイコン一覧の長さの余り
            long index = hashCode % mIcons.Length;
            // アイコンID
            return mIcons[index].id;
        }
    }
    
    public class LeagueMatchManager : CruFramework.Utils.Singleton<LeagueMatchManager>
    {
                
        /// <summary>シーズンIdが取得できなかった時の例外</summary>
        private static long recentSeasonId;
        #region StaticMethods
        
        public async UniTask OnReplayButtonAsync(long previewId)
        {
            // 閉じる
            // AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            
            BattleReserveFormationGetBattlePreviewDataFromLogAPIRequest request =
                new BattleReserveFormationGetBattlePreviewDataFromLogAPIRequest();

            BattleReserveFormationGetBattlePreviewDataFromLogAPIPost post =
                new BattleReserveFormationGetBattlePreviewDataFromLogAPIPost
                {
                    id = previewId
                };
            request.SetPostData(post);
            
            // API
            await APIManager.Instance.Connect(request);
            BattleReserveFormationGetBattlePreviewDataFromLogAPIResponse response = request.GetResponseData();
            
            // 現在のページと引数
            PageType pageType = AppManager.Instance.UIManager.PageManager.CurrentPageType;
            object openArguments = AppManager.Instance.UIManager.PageManager.CurrentPageObject.OpenArguments;
            
            // インゲームへ
            NewInGameOpenArgs args = new NewInGameOpenArgs(pageType, response.srcData, response.destData, response.previewIndex, openArguments);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false, args);
        }

        public static void OnClickLeagueMatchMenu(LeagueMatchInfo leagueMatchInfo)
        {
            // 渡すシーズンIDを決定
            long seasonId = leagueMatchInfo.SeasonData != null ? leagueMatchInfo.SeasonData.SeasonId : recentSeasonId;
            LeagueMatchMenuModal.Arguments arguments = new LeagueMatchMenuModal.Arguments(leagueMatchInfo, seasonId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchMenu, arguments);
        }
        
        public static void OnClickLeagueMatchDeckEditButton(long id, BattleReserveFormationMatchLineup matchLineup, ColosseumEventMasterObject mColosseumEvent)
        {
            var param = new LeagueMatchDeckEntryPage.PageParams
            {
                InitialPartyNumber = matchLineup.playerInfo.partyNumber, OpenFrom = PageType.LeagueMatch, Id = id, RoundNumber = matchLineup.roundNumber, ColosseumEventMaster = mColosseumEvent
            };
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatchDeckEntry, true, param);
        }
        
        public static string GetDateTimeString(DateTime startAt, DateTime endAt)
        {
            return string.Format(StringValueAssetLoader.Instance["event.period"], startAt.GetDateTimeString(), endAt.GetDateTimeString());
        }
        
        // ◆自クラブメンバーの登録チーム確認ポップアップ（管理者）_新規
        // ◆自クラブメンバーの登録チーム確認ポップアップ_新規
        public static void ConfirmClubMateEntry(long inGameMatchId, long SideNumber, long RoundNumber, bool admin = false, Action onCancelMateEntry = null)
        {
            var param = new LeagueMatchRegisteredTeamConfirmModal.
                Arguments(admin ? LeagueMatchRegisteredTeamConfirmModal.Type.Unregistration: LeagueMatchRegisteredTeamConfirmModal.Type.MyTeamConfirm, 
                    inGameMatchId, SideNumber, RoundNumber,
                    () =>
                    {
                        onCancelMateEntry?.Invoke();
                    });
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRegisteredTeamConfirm, param);
        }
        
        // ◆他クラブメンバーの登録チーム確認ポップアップ（戦力非公開）_新規
        public static void ConfirmOtherClubEntry(long inGameMatchId, long SideNumber, long RoundNumber)
        {
            var param = new LeagueMatchRegisteredTeamConfirmModal.
                Arguments(LeagueMatchRegisteredTeamConfirmModal.Type.OpponentTeamConfirm, inGameMatchId, SideNumber, RoundNumber);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.LeagueMatchRegisteredTeamConfirm, param);
        }
        
        // リーグマッチシーズン外ならrecentSeasonIdを更新
        public static async void UpdateRecentSeasonId(long mColosseumEventId)
        {
                ColosseumGetRecentSeasonInfoAPIResponse response = await ConnectColosseumGetRecentSeasonInfoAPI(mColosseumEventId);
                // データが取得できているか確認し、recentSeasonIdを更新
                recentSeasonId = response.userSeasonStatus != null ? response.userSeasonStatus.sColosseumEventId : 0;
        }

        // リーグマッチのシーズン情報を取得
        private static async UniTask<ColosseumGetRecentSeasonInfoAPIResponse> ConnectColosseumGetRecentSeasonInfoAPI(long mColosseumEventId)
        {
            ColosseumGetRecentSeasonInfoAPIPost post = new ColosseumGetRecentSeasonInfoAPIPost();
            post.mColosseumEventId = mColosseumEventId;
            ColosseumGetRecentSeasonInfoAPIRequest request = new ColosseumGetRecentSeasonInfoAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        #endregion
    }
}
