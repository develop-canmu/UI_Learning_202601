using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.LeagueMatchTournament
{
    public static class LeagueMatchTournamentManager
    {
        // 大会ステータスグループタイプ
        public enum BannerGroupType
        {
            // 表示なし
            None = 0,
            // 進行中
            Progress = 1,
            // エントリー中
            EntryAccept = 2,
            // エントリー受付中
            EntryStart = 3,
            // 不参加
            NotEntry = 4,
        }

        // 更新時間の間隔
        public const float UpdateTimeInterval = 0.5f;

        // 初回フェーズ
        public const int firstPhase = 1;
        
        /// <summary> 大会情報リスト取得 </summary>
        public static List<LeagueMatchTournamentInfo> GetTournamentList()
        {
            List<LeagueMatchTournamentInfo> tournamentInfoList = new List<LeagueMatchTournamentInfo>();
            
            ColosseumClientHandlingType colosseumType = ColosseumClientHandlingType.InstantTournament;
            
            // シーズン開始~結果確認終了までのデータを取得
            foreach (ColosseumSeasonData seasonData in UserDataManager.Instance.ColosseumSeasonDataList.Values)
            {
                // マスタがない
                if(seasonData.MColosseumEvent == null) continue;
                // タイプが一致していない
                if(seasonData.MColosseumEvent.clientHandlingType != colosseumType) continue;
                // シーズン中、終了後~結果確認終了のどちらのデータでも無いなら無視する
                if(seasonData.IsOnSeason == false && DateTimeExtensions.IsWithinPeriod(AppTime.Now, seasonData.MColosseumEvent.endAt.TryConvertToDateTime(), seasonData.MColosseumEvent.displayEndAt.TryConvertToDateTime()) == false) continue;
                // すでに同じマスタが追加されているなら無視
                if (tournamentInfoList.Any(x => x.MColosseumEvent.id == seasonData.MColosseumEvent.id)) continue;
                // データを追加
                tournamentInfoList.Add(new LeagueMatchTournamentInfo(seasonData));
            }
                
            // シーズン外で表示するデータ取得(エントリー受付中データを取得)
            IOrderedEnumerable<ColosseumEventMasterObject> sortedMasters = MasterManager.Instance.colosseumEventMaster.values.OrderBy(m => m.id);
            foreach (ColosseumEventMasterObject mColosseumEvent in sortedMasters)
            {
                // タイプが一致していない
                if(mColosseumEvent.clientHandlingType != colosseumType) continue;
                // 初回フェーズ以外はエントリーが存在しないので表示しない
                if (mColosseumEvent.seriesOption.phaseNumber > firstPhase) continue;
                // エントリー受付が始まっていて結果確認の対象データを取得
                if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, mColosseumEvent.entryStartAt.TryConvertToDateTime(), mColosseumEvent.displayEndAt.TryConvertToDateTime()) == false) continue;
                // すでに同じマスタが追加されているなら無視
                if (tournamentInfoList.Any(x => x.MColosseumEvent.id == mColosseumEvent.id)) continue;
                // データを追加
                tournamentInfoList.Add(new LeagueMatchTournamentInfo(mColosseumEvent));
            }

            return tournamentInfoList;
        }
        
         /// <summary> バナーのステータス取得 </summary>
        public static LeagueMatchTournamentBanner.Status GetBannerStatus(LeagueMatchTournamentInfo matchInfo)
        {
            // エントリー期間中
            if (matchInfo.IsEntryTerm)
            {
                // エントリーしているならエントリー済み、それ以外ならエントリー受付中
                return matchInfo.IsEntry ? LeagueMatchTournamentBanner.Status.EntryAccept : LeagueMatchTournamentBanner.Status.EntryStart;
            }
            
            // シーズンデータがまだないなら(エントリー期間外の際の表示)
            if (matchInfo.SeasonData == null || matchInfo.SeasonData.IsMatchingFailed)
            {
                // エントリーしているならエントリー済み、それ以外なら不参加
                return matchInfo.IsEntry ? LeagueMatchTournamentBanner.Status.EntryAccept : LeagueMatchTournamentBanner.Status.NotEntry;
            }

            // エントリーはしていない場合は未参加
            if (matchInfo.IsEntry == false)
            {
                return LeagueMatchTournamentBanner.Status.NotEntry;
            }
            
            // これ以降はシーズンデータあり,かつエントリー済みの状態が通る
            
            // 現在のスケジュール
            LeagueMatchInfoSchedule currentSchedule = matchInfo.GetCurrentSchedule(false);
            // 次のスケジュール
            LeagueMatchInfoSchedule nextSchedule = matchInfo.GetNextSchedule();

            // 次のスケジュールがない
            if (nextSchedule == null)
            {
                // 大会シーズンが終わっているなら大会終了済み
               if (matchInfo.SeasonEndAt.IsPast(AppTime.Now) && matchInfo.SeasonData.UserSeasonStatus != null)
               {
                   return LeagueMatchTournamentBanner.Status.EndTournament;
               }
               // まだなら全ての試合終了
               else
               {
                   return LeagueMatchTournamentBanner.Status.FinishAllBattle;
               }
            }

            // 今日の最終試合のスケジュールを取得
            LeagueMatchInfoSchedule todayFinalBattleSchedule = matchInfo.GetFinalBattleScheduleOfToday();
            
            if (todayFinalBattleSchedule != null)
            {
                // 今日の試合がすでに終わっている(今日の最終試合終了時間を超えているか)
                if (todayFinalBattleSchedule.StartAt.IsPast(AppTime.Now))
                {
                    // 本日の試合が終了
                    return LeagueMatchTournamentBanner.Status.FinishTodayBattle;
                }
            }

            // 現在のスケジュール
            if (currentSchedule != null)
            {
                // 試合中
                if (currentSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle)
                {
                    return LeagueMatchTournamentBanner.Status.BattleStart;
                }
                
                // 編成確定後
                else if (currentSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.FormationLock)
                {
                    // 初回の試合開始まで
                    return LeagueMatchTournamentBanner.Status.BeforeFirstBattleStart;
                }
            }

            // 次のスケジュールが編成確定ならチームエントリー受付中
            if (nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.FormationLock)
            {
                return LeagueMatchTournamentBanner.Status.TeamEntryStart;
            }

            // どれにも該当しない場合は不参加にしておく
            return LeagueMatchTournamentBanner.Status.NotEntry;
        }

        /// <summary> 次の更新時間 </summary>
        public static DateTime GetNextUpdateTime(LeagueMatchTournamentInfo matchInfo)
        {
            DateTime updateTime = DateTime.MaxValue;

            switch (matchInfo.BannerStatus)
            {
                // エントリー受付中
                case LeagueMatchTournamentBanner.Status.EntryStart:
                {
                    // エントリー受付終了時間
                    updateTime = matchInfo.EntryEndAt;
                    break;
                }
                // エントリー受付済み
                case LeagueMatchTournamentBanner.Status.EntryAccept:
                {
                    // エントリー期間中はエントリー受付終了時間が更新時間
                    if (matchInfo.IsEntryTerm)
                    {
                        updateTime = matchInfo.EntryEndAt;
                    }
                    // シーズン開始前ならシーズン開始時間が更新時間
                    else if (matchInfo.SeasonStartAt.IsFuture(AppTime.Now))
                    {
                        updateTime = matchInfo.SeasonStartAt;
                    }

                    break;
                }
                // 不参加
                case LeagueMatchTournamentBanner.Status.NotEntry:
                {
                    // 特に更新時間はないので最大値で渡しておく
                    updateTime = DateTime.MaxValue;
                    break;
                }
                // 進行中
                case LeagueMatchTournamentBanner.Status.TeamEntryStart:
                {
                    // 編成確定時間
                    LeagueMatchInfoSchedule formationLock = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.FormationLock);
                    if (formationLock != null)
                    {
                        updateTime = formationLock.StartAt;
                    }
                    break;
                }
                // 初戦
                case LeagueMatchTournamentBanner.Status.BeforeFirstBattleStart:
                // 試合進行中
                case LeagueMatchTournamentBanner.Status.BattleStart:
                {
                    // 次の試合のスケジュール開始時間を更新時間に
                    LeagueMatchInfoSchedule battle = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.Battle);
                    if (battle != null)
                    {
                        updateTime = battle.StartAt;
                    }
                    break;
                }
                // 本日の試合終了
                case LeagueMatchTournamentBanner.Status.FinishTodayBattle:
                {
                    // 次のチームエントリー時間(対戦相手決定時間)を更新時間に
                    updateTime = matchInfo.GetTeamEntryStartTime();
                    break;
                }
                // 全試合終了
                case LeagueMatchTournamentBanner.Status.FinishAllBattle:
                {
                    // 試合結果確定時間は特定出来ないので特に更新時間の設定はなし
                    break;
                }
                // 大会終了
                case LeagueMatchTournamentBanner.Status.EndTournament:
                {
                    // 表示期間中なら結果確認終了時間を更新時間に
                    if (matchInfo.DisplayEndAt.IsFuture(AppTime.Now))
                    {
                        updateTime = matchInfo.DisplayEndAt;   
                    }
                    break;
                }
            }

            return updateTime;
        }

        /// <summary> バナーのグループタイプ取得 </summary>
        public static BannerGroupType GetBannerGroupType(LeagueMatchTournamentBanner.Status status)
        {
            switch (status)
            {
                // エントリー受付中
                case LeagueMatchTournamentBanner.Status.EntryStart:
                {
                    return BannerGroupType.EntryStart;
                }
                // エントリー受付済み
                case LeagueMatchTournamentBanner.Status.EntryAccept:
                {
                    return BannerGroupType.EntryAccept;
                }
                // 不参加
                case LeagueMatchTournamentBanner.Status.NotEntry:
                {
                    return BannerGroupType.NotEntry;
                }
                // 進行中
                case LeagueMatchTournamentBanner.Status.TeamEntryStart:
                case LeagueMatchTournamentBanner.Status.BeforeFirstBattleStart:
                case LeagueMatchTournamentBanner.Status.BattleStart:
                case LeagueMatchTournamentBanner.Status.FinishTodayBattle:
                case LeagueMatchTournamentBanner.Status.FinishAllBattle:
                {
                    return BannerGroupType.Progress;
                }
                // 終了済み大会はラベルグループは表示しない
                case LeagueMatchTournamentBanner.Status.EndTournament:
                {
                    return BannerGroupType.None;
                }
            }
            
            // グループラベルに分類できないならエラー
            CruFramework.Logger.LogError($"Not Find GroupLabelType : {status}");
            return BannerGroupType.None;
        }

        /// <summary> エントリー受付期間終了モーダル </summary>
        public static async UniTask OpenFinishEntryTernModal()
        {
            ConfirmModalButtonParams negative = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m) =>m.Close());
            string title = StringValueAssetLoader.Instance["league.match.tournament.end_entry_modal.title"];
            string message = StringValueAssetLoader.Instance["league.match.tournament.end_entry_modal.message"];
            ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, negative);
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
        }
    }
}