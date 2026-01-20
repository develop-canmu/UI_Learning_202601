using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchInfoSchedule
    {
        /// <summary>スケジュール</summary>
        public enum Schedule
        {
            /// <summary>シーズン戦エントリ終了</summary>
            FormationLock,
            //// <summary> バトル準備画面に入場できる状態 </summary>
            BattleEntrance,
            /// <summary>シーズン戦バトル</summary>
            Battle,
        }

        private Schedule currentSchedule;
        /// <summary>ステート</summary>
        public Schedule CurrentSchedule
        {
            get { return currentSchedule; }
        }
        
        private DateTime startAt;
        /// <summary>開始時間</summary>
        public DateTime StartAt
        {
            get { return startAt; }
        }
        
        private DateTime nextScheduleStartAt;
        /// <summary>次のスケジュールの開始時間</summary>
        public DateTime NextScheduleStartAt
        {
            get { return nextScheduleStartAt; }
        }
        
        private long roundNumber = 0;
        /// <summary>バトルのラウンド数</summary>
        public long RoundNumber
        {
            get { return roundNumber; }
        }
        
        public LeagueMatchInfoSchedule(Schedule currentSchedule, DateTime startAt, DateTime nextScheduleStartAt, long roundNumber = 0)
        {
            this.currentSchedule = currentSchedule;
            this.startAt = startAt;
            this.nextScheduleStartAt = nextScheduleStartAt;
            this.roundNumber = roundNumber;
        }
    }
    
    public class LeagueMatchInfo
    {
        private ColosseumSeasonData seasonData = null;
        /// <summary>シーズン情報</summary>
        public ColosseumSeasonData SeasonData
        {
            get { return seasonData; }
        }
        
        private ColosseumEventMasterObject mColosseumEvent = null;
        /// <summary>イベントマスタ</summary>
        public ColosseumEventMasterObject MColosseumEvent
        {
            get { return mColosseumEvent; }
        }
    
        private List<LeagueMatchInfoSchedule> timeSchedule = new List<LeagueMatchInfoSchedule>();
        /// <summary>タイムスケジュール</summary>
        public IReadOnlyList<LeagueMatchInfoSchedule> TimeSchedule
        {
            get { return timeSchedule; }
        }
        
        private DateTime seasonStartAt;
        /// <summary>シーズン開始時間</summary>
        public DateTime SeasonStartAt
        {
            get { return seasonStartAt; }
        }
        
        private DateTime seasonEndAt;
        /// <summary>シーズン終了時間</summary>
        public DateTime SeasonEndAt
        {
            get { return seasonEndAt; }
        }
        
        private DateTime seasonBattleStartAt;
        /// <summary>シーズン戦開始時間</summary>
        public DateTime SeasonBattleStartAt
        {
            get { return seasonBattleStartAt; }
        }
        
        private DateTime seasonBattleEndAt;
        /// <summary>シーズン戦終了時間</summary>
        public DateTime SeasonBattleEndAt
        {
            get { return seasonBattleEndAt; }
        }
        
        private DateTime shiftBattleStartAt;
        /// <summary>入れ替え戦が開始時間</summary>
        public DateTime ShiftBattleStartAt
        {
            get { return shiftBattleStartAt; }
        }
        
        private DateTime shiftBattleEndAt;
        /// <summary>入れ替え戦終了時間</summary>
        public DateTime ShiftBattleEndAt
        {
            get { return shiftBattleEndAt; }
        }

        private DateTime nextSeasonStartAt;
        /// <summary>次シーズンの開始時間</summary>
        public DateTime NextSeasonStartAt
        {
            get { return nextSeasonStartAt; }
        }
        
        private DateTime nextSeasonEndAt;
        /// <summary>次シーズンの終了時間</summary>
        public DateTime NextSeasonEndAt
        {
            get { return nextSeasonEndAt; }
        }

        private DateTime entryStartAt;
        /// <summary> エントリー受付開始時間 </summary>
        public DateTime EntryStartAt
        {
            get { return entryStartAt; }
        }
        
        private DateTime entryEndAt;
        /// <summary> エントリー受付期限 </summary>
        public DateTime EntryEndAt
        {
            get { return entryEndAt; }
        }

        private DateTime displayEndAt;

        /// <summary> 表示終了時間 </summary>
        public DateTime DisplayEndAt
        {
            get { return displayEndAt; }
        }
        
        private long battleDays = 0;
        
        public bool HasSeasonHome
        {
            get
            {
                if(seasonData == null) return false;
                if(seasonData.SeasonHome == null) return false;
                return true;
            }
        }
        
        /// <summary>入れ替え戦に参加できる</summary>
        public bool CanShiftBattle
        {
            get
            {
                // 入れ替え戦ルールではない
                if(mColosseumEvent.gradeShiftType != ColosseumGradeShiftType.ShiftBattle) return false;
                // データがない
                if(seasonData == null) return false;
                if(seasonData.UserSeasonStatus == null) return false;
                if(seasonData.UserSeasonStatus.groupSeasonStatus == null) return false;
                if(seasonData.UserSeasonStatus.groupSeasonStatus.shiftMatchInfo == null) return false;
                if(seasonData.UserSeasonStatus.groupSeasonStatus.shiftMatchInfo.sColosseumGroupStatusId <= 0) return false;
                // 入れ替え戦に参加できる
                return true;
            }
        }

        public LeagueMatchInfo(ColosseumEventMasterObject mColosseumEvent)
        {
            // マスタ
            this.mColosseumEvent = mColosseumEvent; 
            // 次シーズンの期間を計算する
            CalculateNextSeasonTerm();
            // シーズン期間は次回の期間を設定しておく
            seasonStartAt = nextSeasonStartAt;
            seasonEndAt = nextSeasonEndAt;
            // マスタの日付設定を取得
            SetMasterDateTime(mColosseumEvent);
            // シーズン戦日数計算
            CalculateNumberOfSeasonBattleDays();
            // スケジュール
            CalculateTimeSchedule();
        }
        
        public LeagueMatchInfo(ColosseumSeasonData seasonData)
        {
            this.seasonData = seasonData;
            // シーズンの期間を取得
            if(seasonData.SeasonHome != null)
            {
                seasonStartAt = seasonData.SeasonHome.startAt.TryConvertToDateTime();
                seasonEndAt = seasonData.SeasonHome.endAt.TryConvertToDateTime();
            }
            // マスタ
            mColosseumEvent = seasonData.MColosseumEvent;
            // マスタの日付設定を取得
            SetMasterDateTime(mColosseumEvent);
            // 次シーズンの期間を計算する
            CalculateNextSeasonTerm();
            // シーズン戦日数計算
            CalculateNumberOfSeasonBattleDays();
            // スケジュール
            CalculateTimeSchedule();
        }

        /// <summary> マスタに設定されている日付周りの設定をセット</summary>
        private void SetMasterDateTime(ColosseumEventMasterObject master)
        {
            // エントリー受付開始時間
            entryStartAt = master.entryStartAt.TryConvertToDateTime();
            // エントリー受付期限
            entryEndAt = master.entryEndAt.TryConvertToDateTime();
            // 表示期限
            displayEndAt = master.displayEndAt.TryConvertToDateTime();
        }
        
        /// <summary>マスタの日付で次シーズン期間を上書き</summary>
        public void OverwriteNextSeasonTerm(ColosseumEventMasterObject mColosseumEvent)
        {
            // シーズン開始は1時から
            nextSeasonStartAt = mColosseumEvent.startAt.TryConvertToDateTime() + TimeSpan.FromHours(1);
            nextSeasonEndAt = mColosseumEvent.endAt.TryConvertToDateTime();
        }

        /// <summary>次シーズンの期間を計算する</summary>
        private void CalculateNextSeasonTerm()
        {
            if(mColosseumEvent.cycleDays < 0)
            {
                // cycleDaysが0以下の場合はマスタを見て設定
                OverwriteNextSeasonTerm(mColosseumEvent);
            }
            else
            {
                // マスタの開始時間
                DateTime startAt = mColosseumEvent.startAt.TryConvertToDateTime();
                // 未来に設定されてる
                if(startAt.IsFuture(AppTime.Now))
                {
                    nextSeasonStartAt = startAt;
                }
                // 既に開催済み
                else
                {
                    TimeSpan diff = AppTime.Now - startAt;
                    // 今までに開催した回数
                    int count = (int)(diff.TotalDays / mColosseumEvent.cycleDays);

                    // シーズン期間中 || 休止期間に入ってたら+1回
                    if(DateTimeExtensions.IsWithinPeriod(AppTime.Now, seasonStartAt, seasonEndAt) || diff.TotalDays - mColosseumEvent.cycleDays * count - (mColosseumEvent.cycleDays - mColosseumEvent.intervalMarginDays) >= 0)
                    {
                        count++;
                    }
                    // 開催回数から次シーズンの開始時間を求める
                    nextSeasonStartAt = startAt + TimeSpan.FromDays(mColosseumEvent.cycleDays * count);
                }
                // シーズン開始は1時から
                nextSeasonStartAt += TimeSpan.FromHours(1);
                
                // 次シーズンの終了時間
                nextSeasonEndAt = nextSeasonStartAt - TimeSpan.FromHours(1) + TimeSpan.FromDays(mColosseumEvent.cycleDays - mColosseumEvent.intervalMarginDays);
                // 59分59秒まで
                nextSeasonEndAt -= TimeSpan.FromSeconds(1);
            }
        }
        
        /// <summary>シーズン戦がいつまで行われるかの計算</summary>
        private void CalculateNumberOfSeasonBattleDays()
        {
            ColosseumGradeMasterObject mColosseumGrade = MasterManager.Instance.colosseumGradeMaster.values.Where(m => m.mColosseumGradeGroupId == mColosseumEvent.mColosseumGradeGroupId).FirstOrDefault();
            if(mColosseumGrade == null)
            {
                throw new Exception($"mColosseumGradeにmColosseumGradeGroupId={mColosseumEvent.mColosseumGradeGroupId}が存在しません。");
            }
            // 日数
            battleDays = mColosseumGrade.roomCapacity;
            // 偶数の場合-1する
            if(mColosseumGrade.roomCapacity % 2 == 0)
            {
                battleDays -= 1;
            }
            // シーズン戦開始時間
            seasonBattleStartAt = seasonStartAt;
            // 入れ替え戦開始時間(シーズン開始時間+シーズン戦日数-1時間)
            shiftBattleStartAt = seasonBattleStartAt + TimeSpan.FromDays(battleDays) - TimeSpan.FromHours(1); 
            // シーズン戦終了時間(入れ替え戦開始時間-1秒)
            seasonBattleEndAt = shiftBattleStartAt - TimeSpan.FromSeconds(1);
            // 入れ替え戦終了時間(シーズン戦終了時間+1日)
            shiftBattleEndAt = seasonBattleEndAt + TimeSpan.FromDays(1);
        }
        
        public long GetTopRank()
        {
            return MasterManager.Instance.colosseumGradeMaster.values
                .Where(m => m.mColosseumGradeGroupId == mColosseumEvent.mColosseumGradeGroupId)
                .Max(x => x.gradeNumber);
        }
        
        public BattleReserveFormationMasterObject GetBattleReserveFormationMasterObject()
        {
            // mBattleReserveFormationId取得
            long mBattleReserveFormationId = 0;
            if(mColosseumEvent.inGameType == (long)ColosseumInGameType.BattleReserveFormation)
            {
                mBattleReserveFormationId = mColosseumEvent.inGameSystemId;
            }
            // マスタ取得
            BattleReserveFormationMasterObject mBattleReserveFormation = MasterManager.Instance.battleReserveFormationMaster.FindData(mBattleReserveFormationId);
            
            // マスタ取得できないのでエラー
            if(mBattleReserveFormation == null)
            {
                CruFramework.Logger.LogError($"mColosseumEventId:{mColosseumEvent.id}からmBattleReserveFormationIdを取得できませんでした.");
            }
            return mBattleReserveFormation;
        }

        //// <summary> BattleGameLiftのマスタを取得して返す </summary>
        public BattleGameliftMasterObject GetBattleGameLiftMasterObject()
        {
            // mBattleGameLiftId取得
            long mBattleGameLiftId = 0;
            if(mColosseumEvent.inGameType == (long)ColosseumInGameType.BattleGameLift)
            {
                mBattleGameLiftId = mColosseumEvent.inGameSystemId;
            }
            // マスタ取得
            BattleGameliftMasterObject mBattleReserveFormation = MasterManager.Instance.battleGameliftMaster.FindData(mBattleGameLiftId);
            
            // マスタ取得できないのでエラー
            if(mBattleReserveFormation == null)
            {
                CruFramework.Logger.LogError($"mColosseumEventId:{mColosseumEvent.id}からmBattleGameLiftIdを取得できませんでした.");
            }
            return mBattleReserveFormation;
        }
        
        private long GetBattleRoundCount(BattleReserveFormationMasterObject mBattleReserveFormation)
        {
            long battleRoundCount = MasterManager.Instance.battleReserveFormationRoundMaster.values
                .Count(m => m.mBattleReserveFormationRoundGroupId == mBattleReserveFormation.mBattleReserveFormationRoundGroupId);
            return battleRoundCount;
        }

        //// <summary> １試合にラウンドが何回あるかを取得 </summary>
        private long GetBattleRoundCount()
        {
            // 編成予約戦はRoundが複数ある
            if (mColosseumEvent.inGameType == (long)ColosseumInGameType.BattleReserveFormation)
            {
                // マスタ取得
                BattleReserveFormationMasterObject mBattleReserveFormation = GetBattleReserveFormationMasterObject();
                return GetBattleRoundCount(mBattleReserveFormation);
            }

            // それ以外は１試合のみ
            return 1;
        }
        
        /// <summary>直近のシーズンのスケジュールを計算</summary>
        private void CalculateTimeSchedule()
        {
            // 綺麗にする
            timeSchedule.Clear();

            // 入れ替え戦の設定の場合は1日追加
            if(mColosseumEvent.gradeShiftType == ColosseumGradeShiftType.ShiftBattle)
            {
                battleDays++;
            }
            
            // 編成予約戦
            if (mColosseumEvent.inGameType == (long)ColosseumInGameType.BattleReserveFormation)
            {
                CalculateTimeScheduleByReserveFormation();
            }
            // ゲームサーバー上
            else if (mColosseumEvent.inGameType == (long)ColosseumInGameType.BattleGameLift)
            {
                CalculateTimeScheduleByGameLift();
            }
            
        }

        //// <summary> 編成予約戦の時のスケジュール計算 </summary>
        private void CalculateTimeScheduleByReserveFormation()
        {
            // マスタ取得
            BattleReserveFormationMasterObject mBattleReserveFormation = GetBattleReserveFormationMasterObject();

            // 編成終了時間をマスタから取得
            DateTime formationLockAt = mBattleReserveFormation.formationLockAt.TryConvertToDateTime();
            formationLockAt = new DateTime(seasonStartAt.Year, seasonStartAt.Month, seasonStartAt.Day, formationLockAt.Hour, formationLockAt.Minute, formationLockAt.Second);
            // バトルラウンド数
            long battleRoundCount = GetBattleRoundCount(mBattleReserveFormation);

            // シーズン戦スケジュール
            for(int i = 0; i < battleDays; i++)
            {
                // 入れ替え戦設定
                if(mColosseumEvent.gradeShiftType == ColosseumGradeShiftType.ShiftBattle)
                {
                    // 最終日かつ入れ替え戦に参加できない場合は無視
                    if(i >= battleDays-1 && !CanShiftBattle)
                    {
                        continue;
                    }
                }
                
                // 編成終了時間
                DateTime startAt = formationLockAt + TimeSpan.FromDays(i);
                timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.FormationLock, startAt, startAt.AddMinutes(mBattleReserveFormation.battleStartIntervalMinutes)));
                // 第1試合開始時間
                startAt += TimeSpan.FromMinutes(mBattleReserveFormation.battleStartIntervalMinutes);
                timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.Battle, startAt, startAt.AddMinutes(mBattleReserveFormation.roundIntervalMinutes),1));
                // 第n試合
                for(int n = 1; n < battleRoundCount; n++)
                {
                    // ラウンド間隔
                    startAt += TimeSpan.FromMinutes(mBattleReserveFormation.roundIntervalMinutes);
                    timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.Battle, startAt, startAt.AddMinutes(mBattleReserveFormation.roundIntervalMinutes),n+1));
                }
            }
        }

        //// <summary> GameTypeがゲームサーバー上の場合のスケジュール計算 </summary>
        private void CalculateTimeScheduleByGameLift()
        {
             // マスタ取得
            BattleGameliftMasterObject mBattleGameLift = GetBattleGameLiftMasterObject();

            // 編成終了時間をマスタから取得
            DateTime formationLockAtBase = mBattleGameLift.deckLockAt.TryConvertToDateTime();
            formationLockAtBase = new DateTime(seasonStartAt.Year, seasonStartAt.Month, seasonStartAt.Day, formationLockAtBase.Hour, formationLockAtBase.Minute, formationLockAtBase.Second);
            // 対戦開始時間
            DateTime battleStartAtBase = mBattleGameLift.battleStartAt.TryConvertToDateTime();
            battleStartAtBase = new DateTime(seasonStartAt.Year, seasonStartAt.Month, seasonStartAt.Day, battleStartAtBase.Hour, battleStartAtBase.Minute, battleStartAtBase.Second);
            // サブ対戦開始時間
            DateTime subBattleStartAtBase = mBattleGameLift.battleStartAtSub.TryConvertToDateTime();
            subBattleStartAtBase = new DateTime(seasonStartAt.Year, seasonStartAt.Month, seasonStartAt.Day, subBattleStartAtBase.Hour, subBattleStartAtBase.Minute, subBattleStartAtBase.Second);
            // 対戦終了時間
            DateTime battleEndAtBase = mBattleGameLift.battleFinishAt.TryConvertToDateTime();
            battleEndAtBase = new DateTime(seasonStartAt.Year, seasonStartAt.Month, seasonStartAt.Day, battleEndAtBase.Hour, battleEndAtBase.Minute, battleEndAtBase.Second);
            
            // シーズン戦スケジュール
            for(int i = 0; i < battleDays; i++)
            {
                // 入れ替え戦設定
                if(mColosseumEvent.gradeShiftType == ColosseumGradeShiftType.ShiftBattle)
                {
                    // 最終日かつ入れ替え戦に参加できない場合は無視
                    if(i >= battleDays-1 && !CanShiftBattle)
                    {
                        continue;
                    }
                }
                
                // 編成終了時間
                DateTime formationLockAt = formationLockAtBase + TimeSpan.FromDays(i);
                // バトル準備画面入場開始時間
                DateTime battleEntranceStartAt = battleStartAtBase + TimeSpan.FromDays(i);
                // 対戦開始時間(クラブロワイヤルではサブ対戦開始時間が実際の対戦開始時間となる)
                DateTime battleStartAt = subBattleStartAtBase + TimeSpan.FromDays(i);
                // 対戦終了時間
                DateTime battleEndAt = battleEndAtBase + TimeSpan.FromDays(i);
                
                // 編成確定からバトル準備画面入場開始までのスケジュール
                timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.FormationLock, formationLockAt, battleEntranceStartAt));
                // バトル準備画面入場開始から戦闘開始までのスケジュール
                timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.BattleEntrance, battleEntranceStartAt, battleStartAt));
                // 戦闘開始から戦闘終了までのスケジュール
                timeSchedule.Add(new LeagueMatchInfoSchedule(LeagueMatchInfoSchedule.Schedule.Battle, battleStartAt, battleEndAt, 1));
            }
        }
        
        /// <summary>次のスケジュール更新</summary>
        public LeagueMatchInfoSchedule GetNextSchedule()
        {
            return timeSchedule.FirstOrDefault(v => v.StartAt.IsFuture(AppTime.Now));
        }
        
        //// <summary> 指定したスケジュールが次に行われる物を取得 </summary>
        public LeagueMatchInfoSchedule GetNextSchedule(LeagueMatchInfoSchedule.Schedule scheduleType)
        {
            if (timeSchedule == null)
            {
                return null;
            }

            for (int i = 0; i < timeSchedule.Count; i++)
            {
                LeagueMatchInfoSchedule schedule = timeSchedule[i];
                if (schedule.CurrentSchedule != scheduleType)
                {
                    continue;
                }

                // スケジュール開始がまだ先のものを取得
                if (schedule.StartAt.IsFuture(AppTime.Now))
                {
                    return schedule;
                }
            }

            return null;
        }

        //// <summary> 本日の指定されたスケジュールを取得 </summary>
        public LeagueMatchInfoSchedule GetTodaySchedule(LeagueMatchInfoSchedule.Schedule scheduleType)
        {
            if (timeSchedule == null)
            {
                return null;
            }

            for (int i = 0; i < timeSchedule.Count; i++)
            {
                LeagueMatchInfoSchedule schedule = timeSchedule[i];
                if (schedule.CurrentSchedule != scheduleType)
                {
                    continue;
                }

                // スケジュール開始が本日のものを取得
                if (DateTimeExtensions.IsSameDay(schedule.StartAt, AppTime.Now))
                {
                    return schedule;
                }
            }

            return null;
        }
        
        /// <summary>シーズン中</summary>
        public bool IsOnSeason => DateTimeExtensions.IsWithinPeriod(AppTime.Now, SeasonStartAt, SeasonEndAt);

        /// <summary>シーズン戦中</summary>
        public bool IsOnSeasonBattle => DateTimeExtensions.IsWithinPeriod(AppTime.Now, SeasonBattleStartAt, SeasonBattleEndAt);
        
        /// <summary>入れ替え戦中</summary>
        public bool IsOnShiftBattle => CanShiftBattle && DateTimeExtensions.IsWithinPeriod(AppTime.Now, ShiftBattleStartAt, ShiftBattleEndAt);

        /// <summary> エントリー受付期間か </summary>
        public bool IsEntryTerm => DateTimeExtensions.IsWithinPeriod(AppTime.Now, EntryStartAt, EntryEndAt);

        /// <summary> エントリー済みデータか </summary>
        public bool IsEntry => ColosseumManager.IsEntry(MColosseumEvent.id);
        
        public DateTime GetNextOrCurrentBattleStartDateTime()
        {
            var firstBattleSchedule = GetBattleScheduleOfToday(1);
            var finalBattleSchedule = GetFinalBattleScheduleOfToday();
            if (firstBattleSchedule == null || finalBattleSchedule == null)
            {
                return default;
            }
            
            var firstBattleStartTimeToday = firstBattleSchedule.StartAt;
            var allBattleEndTimeToday = finalBattleSchedule.StartAt;
            
            if (DateTimeExtensions.IsFuture(firstBattleStartTimeToday, AppTime.Now))
            {
                return firstBattleStartTimeToday;
            }
            
            // デイリーマッチ中
            if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, firstBattleStartTimeToday, allBattleEndTimeToday))
            {
                LeagueMatchInfoSchedule currentSchedule = GetCurrentSchedule();
                if (currentSchedule == null)
                {
                    currentSchedule = GetNextSchedule();
                }
                
                var processedBattleRound = currentSchedule.RoundNumber;
                if (processedBattleRound == finalBattleSchedule.RoundNumber)
                {
                    return allBattleEndTimeToday;
                }
                else
                {
                    var nextBattleSchedule = GetBattleScheduleOfToday(currentSchedule.RoundNumber + 1);
                    return nextBattleSchedule?.StartAt ?? default;
                }
            }
            
            // デイリーマッチ全行程終了
            if (DateTimeExtensions.IsPast(allBattleEndTimeToday, AppTime.Now))
            {
                if (!DateTimeExtensions.IsSameDay(AppTime.Now, seasonBattleEndAt))
                {
                    var firstFightTomorrow = GetBattleScheduleOfToday(1, 1);
                    return firstFightTomorrow?.StartAt ?? default;
                }
            }
            return default;
        }
        
        public LeagueMatchInfoSchedule GetFinalBattleScheduleOfToday()
        {
            // マスタ取得
            BattleReserveFormationMasterObject mBattleReserveFormation = GetBattleReserveFormationMasterObject();
            long totalRound = GetBattleRoundCount(mBattleReserveFormation);
            for (var i = timeSchedule.Count -1; i >= 0; i--)
            {
                var leagueMatchBannerSchedule = timeSchedule[i];
                if (DateTimeExtensions.IsSameDay(leagueMatchBannerSchedule.StartAt, AppTime.Now) &&
                    leagueMatchBannerSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle &&
                    leagueMatchBannerSchedule.RoundNumber == totalRound)
                {
                    return leagueMatchBannerSchedule;
                }
            }
            return default;
        }
        
        LeagueMatchInfoSchedule GetBattleScheduleOfToday(long round, long plusDay = 0)
        {
            if (timeSchedule == null)
            {
                return null;
            }
            for (var i = timeSchedule.Count -1; i >= 0; i--)
            {
                var leagueMatchBannerSchedule = timeSchedule[i];
                if (DateTimeExtensions.IsSameDay(leagueMatchBannerSchedule.StartAt, AppTime.Now.AddDays(plusDay)) &&
                    leagueMatchBannerSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle &&
                    leagueMatchBannerSchedule.RoundNumber == round)
                {
                    return leagueMatchBannerSchedule;
                }
            }
            return null;
        }

        //// <summary> クラブロワイヤルで本日行われるバトルスケジュールを取得 </summary>
        public LeagueMatchInfoSchedule GetBattleRoyalScheduleOfToday()
        {
            if (timeSchedule == null)
            {
                return null;
            }
            // クラブロワイヤルは１日１戦のみ
            for (var i = timeSchedule.Count -1; i >= 0; i--)
            {
                LeagueMatchInfoSchedule leagueMatchBannerSchedule = timeSchedule[i];
                if (DateTimeExtensions.IsSameDay(leagueMatchBannerSchedule.StartAt, AppTime.Now) &&
                    leagueMatchBannerSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle)
                {
                    return leagueMatchBannerSchedule;
                }
            }
            return null;
        }

        /// <summary> 本日の試合終了時間 </summary>
        public DateTime GetBattleEndTimeOfToday()
        {
            LeagueMatchInfoSchedule finalBattleScheduleOfToday = GetFinalBattleScheduleOfToday();
            return finalBattleScheduleOfToday.StartAt;
        }

        public DateTime GetAggregatingEndTimeOfToday()
        {
            LeagueMatchInfoSchedule finalBattleScheduleOfToday = GetFinalBattleScheduleOfToday();
            return finalBattleScheduleOfToday.StartAt.AddMinutes(LeagueMatchUtility.RefreshInterval);
        }
        
        //// <summary> チームエントリー受付開始時間(対戦相手決定時間) </summary>
        public DateTime GetTeamEntryStartTime()
        {
            DateTime entryStartDate = SeasonStartAt + TimeSpan.FromDays((AppTime.Now - SeasonStartAt).Days + 1);
            // シーズン開始が1時開始で、エントリ開始が0時からなので-1時間する
            // (サーバ側では特にエントリー開始時間などは設けていないのでクライアント側で定義)
            entryStartDate -= TimeSpan.FromHours(1);
            return entryStartDate;
        }

        public bool IsWithEntryTermOfToday()
        {
            var todayFormationLockSchedule = GetFormationLockScheduleOfToday();
            DateTime formationLockTimeToday = todayFormationLockSchedule.StartAt;
            return DateTimeExtensions.IsFuture(formationLockTimeToday, AppTime.Now);
        }

        //// <summary> 全ての対戦が終了したか(引数のフラグで入れ替え戦を含むかを設定する) </summary>
        public bool IsAllBattleComplete(bool isIgnoreShiftBattle = false)
        {
            // ラウンド数
            long roundCount = GetBattleRoundCount();
            // 入れ替え戦のチェックをしたか
            bool isCheckShiftBattle = false;
            
            // 最終試合の時間から判定するので後ろから見ていく
            for (int i = timeSchedule.Count() - 1; i >= 0; i--)
            {
                // 対戦以外のスケジュールはとばす
                if (timeSchedule[i].CurrentSchedule != LeagueMatchInfoSchedule.Schedule.Battle)
                {
                    continue;
                }

                // 該当のラウンド数のもの以外はとばす
                if (timeSchedule[i].RoundNumber != roundCount)
                {
                    continue;
                }
                
                // 入れ替え戦を無視する場合でまだ入れ替え戦のチェックがされてないなら、もうひとつ前の試合が最終試合に該当するのでとばす
                if (isIgnoreShiftBattle && isCheckShiftBattle == false && CanShiftBattle)
                {
                    // 入れ替え戦チェック済み
                    isCheckShiftBattle = true;
                    continue;
                }

                // 対戦最終日情報を取得(時間は切り捨て)
                DateTime battleEndDate = timeSchedule[i].NextScheduleStartAt.Date;
                
                // 今日が最終対戦日なら本日の試合が終わっているかも判定に加える(試合の早期終了考慮のため)
                if (DateTimeExtensions.IsSameDay(battleEndDate, AppTime.Now))
                {
                    // 最終日以降で試合が終わっているなら全ての対戦が終了
                    return battleEndDate.Date.IsPast(AppTime.Now) && HasTodayMatchResult();
                }
                // 最終日と同日でないなら
                else
                {
                    return battleEndDate.Date.IsPast(AppTime.Now);
                }
                
            }

            return false;
        }

        //// <summary> 今日の試合結果がでているか(マスタの予定時間の比較ではなくサーバー側からのデータが来ているかで判定する) </summary>
        public bool HasTodayMatchResult()
        {
            // データがない
            if(seasonData == null) return false;
            if(seasonData.UserSeasonStatus == null) return false;

            long result = seasonData.UserSeasonStatus.todayResultQuick;
            // 試合結果があるか
            return ColosseumManager.HasResult(result);
        }
        
        public string GetMatchBoardTransitionButtonText()
        {
            string text = String.Empty;
            var todayFormationLockSchedule = GetFormationLockScheduleOfToday();
            if (DateTimeExtensions.IsFuture( todayFormationLockSchedule.StartAt, AppTime.Now))
            {
                text = StringValueAssetLoader.Instance["league.match_board.enter.entry"];
            }
            else
            {
                text = StringValueAssetLoader.Instance["league.match_board.enter.top"];
            }
            return text;
        }
        
        string GetRankName(long rank)
        {
            return StringValueAssetLoader.Instance[$"club.rank.{rank}"];
        }
        
        /// <summary>
        /// 対戦表 補足説明表示
        /// </summary>
        /// <returns></returns>
        public string GetBoardUnderText(long rank)
        {
            string returnValue = string.Empty;
            var todayFormationLockSchedule = GetFormationLockScheduleOfToday();
            var finalBattleSchedule = GetFinalBattleScheduleOfToday();
            var formationLockTimeToday = todayFormationLockSchedule.StartAt;
            var allBattleEndTimeToday = GetAggregatingEndTimeOfToday();
            
            // チームエントリー受付中
            if (DateTimeExtensions.IsFuture(formationLockTimeToday, AppTime.Now))
            {
                returnValue = StringValueAssetLoader.Instance["league.match_board.enter.entry"] + StringValueAssetLoader.Instance["registration_open"] + "<br>" +
                StringValueAssetLoader.Instance["common.remain"] + formationLockTimeToday.GetRemainingString(AppTime.Now);
                return returnValue;
            }
            
            // デイリーマッチ中
            if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, formationLockTimeToday, allBattleEndTimeToday))
            {
                LeagueMatchInfoSchedule currentSchedule = GetCurrentSchedule();
                if (currentSchedule == null)
                {
                    currentSchedule = GetNextSchedule();
                }
                
                var processedBattleRound = currentSchedule.RoundNumber;
                if (processedBattleRound == finalBattleSchedule.RoundNumber)
                {
                    returnValue = string.Empty;
                }
                else
                {
                    var battleConfigInfo = MasterManager.Instance.battleReserveFormationRoundMaster.FindData(currentSchedule.RoundNumber + 1);
                    var nextBattleSchedule = GetBattleScheduleOfToday(battleConfigInfo.roundNumber);
                    returnValue = StringValueAssetLoader.Instance["league.match_board.enter.entry"] + StringValueAssetLoader.Instance["registration_closed"] + 
                            "<br>"+ string.Format(StringValueAssetLoader.Instance["league.match_board.til_start"], battleConfigInfo.nameLabel) + nextBattleSchedule.StartAt.GetRemainingString(AppTime.Now);
                }
                return returnValue;
            }
            
            // デイリーマッチ全行程終了
            if (DateTimeExtensions.IsPast(allBattleEndTimeToday, AppTime.Now) && DateTimeExtensions.IsSameDay(AppTime.Now, seasonBattleEndAt))
            {
                if (!CanShiftBattle) // 入れ替え戦なし
                {
                    if (seasonData.UserSeasonStatus.groupSeasonStatus.gradeAfter > seasonData.UserSeasonStatus.groupSeasonStatus.gradeBefore)
                    {
                        returnValue = string.Format(StringValueAssetLoader.Instance["league.match.season_result"], rank) + 
                                "<br>" + string.Format(StringValueAssetLoader.Instance["league.match.season_result_up_without_shift"]);
                    }
                    else
                    {
                        // すべての試合が終了し、最終順位は｛n｝位でした！
                        returnValue = string.Format(StringValueAssetLoader.Instance["league.match.season_result"], rank);
                    }
                }
                else // 入れ替え戦あり
                {
                    if (seasonData.UserSeasonStatus.groupSeasonStatus.gradeBefore < seasonData.UserSeasonStatus.groupSeasonStatus.shiftMatchInfo.gradeNumber) // 昇格戦
                    {
                        returnValue = string.Format(StringValueAssetLoader.Instance["league.match.season_result_up_shift_reason"], rank);
                    }
                    else　// 降格戦
                    {
                        returnValue = string.Format(StringValueAssetLoader.Instance["league.match.season_result_down_shift_reason"], rank);
                    }
                }
                return returnValue;
            }
            return returnValue;
        }
        
        // リーグマッチトップ Headerの下
        public string GetLeagueMatchTopCurrentStatusInfoText(Progress progress, BattleResult battleResult)
        {
            string text = null;

            if (!HasSeasonHome)
            {
                return text;
            }
            
            ColosseumGroupSeasonStatus groupSeasonStatus = SeasonData.UserSeasonStatus.groupSeasonStatus;
            if (progress == Progress.MatchEnd && CanShiftBattle)
            {
                if ((BattleResult)groupSeasonStatus.shiftMatchInfo.result == BattleResult.Win)
                {
                    if (groupSeasonStatus.gradeBefore < groupSeasonStatus.gradeAfter) // 昇格
                    {
                        text = String.Format(StringValueAssetLoader.Instance["league.match.top_result_rank_win_up"],GetRankName(groupSeasonStatus.gradeAfter));
                    }
                    else
                    {
                        text = String.Format(StringValueAssetLoader.Instance["league.match.top_result_rank_win_keep"],GetRankName(groupSeasonStatus.gradeAfter));
                    }
                }
                else if ((BattleResult)groupSeasonStatus.shiftMatchInfo.result == BattleResult.Lose)
                {
                    if (groupSeasonStatus.gradeBefore > groupSeasonStatus.gradeAfter) // 降格
                    {
                        text = String.Format(StringValueAssetLoader.Instance["league.match.top_result_rank_lose_down"],GetRankName(groupSeasonStatus.gradeAfter));
                    }
                    else
                    {
                        text = String.Format(StringValueAssetLoader.Instance["league.match.top_result_rank_lose_keep"],GetRankName(groupSeasonStatus.gradeAfter));
                    }
                }
                else
                {
                    text = StringValueAssetLoader.Instance["league.match.all_battle_ended"];
                }
                
                return text;
            }
            
            if( DateTimeExtensions.IsWithinPeriod(AppTime.Now, SeasonStartAt, SeasonEndAt))
            {
                var todayFormationLockSchedule = GetFormationLockScheduleOfToday();
                var firstBattleSchedule = GetBattleScheduleOfToday(1);
                
                var formationLockTimeToday = todayFormationLockSchedule.StartAt;
                var firstBattleStartTimeToday = firstBattleSchedule.StartAt;
                var allBattleEndTimeToday = GetFinalBattleScheduleOfToday().StartAt;
                
                double currentDayOfSeason = Math.Floor((AppTime.Now -　SeasonStartAt).TotalDays) + 1;
                
                // チームエントリー受付中
                if (DateTimeExtensions.IsFuture(formationLockTimeToday, AppTime.Now))
                {
                    text = StringValueAssetLoader.Instance["league.match_board.enter.entry"] + StringValueAssetLoader.Instance["registration_open"];
                }
                
                // チームエントリー受付終了
                if (DateTimeExtensions.IsWithinPeriod(AppTime.Now,formationLockTimeToday, firstBattleStartTimeToday))
                {
                    text = string.Format(StringValueAssetLoader.Instance["league.match.season_day"], currentDayOfSeason) + 
                           StringValueAssetLoader.Instance["league.match.entry_ended"];
                }
                
                // デイリーマッチ中
                if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, firstBattleStartTimeToday, allBattleEndTimeToday))
                {
                    text = string.Format(StringValueAssetLoader.Instance["league.match.season_day"], currentDayOfSeason) + 
                           StringValueAssetLoader.Instance["league.match.battling"];
                }
                
                // デイリーマッチ全行程終了
                if (DateTimeExtensions.IsPast(allBattleEndTimeToday, AppTime.Now))
                {
                    text = string.Format(StringValueAssetLoader.Instance["league.match.season_day"], currentDayOfSeason) + 
                           StringValueAssetLoader.Instance["league.match.all_battle_ended"];
                }
            }
            return text;
        }
        
        // リーグマッチトップ Headerの下
        public string GetTilEntryEndText()
        {
            var todayFormationLockSchedule = GetFormationLockScheduleOfToday();
            var formationLockTimeToday = todayFormationLockSchedule.StartAt;
            if (formationLockTimeToday > AppTime.Now)
                return StringValueAssetLoader.Instance["league.match_entry.term_remains"] +
                       formationLockTimeToday.GetRemainingString(AppTime.Now);
            return string.Empty;
        }
        
        LeagueMatchInfoSchedule GetFormationLockScheduleOfToday(long plusDay = 0)
        {
            for (var i = timeSchedule.Count -1 ; i >= 0 ; i--)
            {
                var leagueMatchBannerSchedule = timeSchedule[i];
                if (DateTimeExtensions.IsSameDay(AppTime.Now.AddDays(plusDay), leagueMatchBannerSchedule.StartAt) && 
                    leagueMatchBannerSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.FormationLock)
                {
                    return leagueMatchBannerSchedule;
                }
            }
            return null;
        }
        
        public LeagueMatchInfoSchedule GetCurrentSchedule(bool isIgnoreFreeTime = true)
        {
            for (var i = 0; i < timeSchedule.Count; i++)
            {
                var leagueMatchBannerSchedule = timeSchedule[i];
                LeagueMatchInfoSchedule nextSchedule = null;
                
                // スケジュール間の空き時間を無視するならその空き時間にもスケジュールがあるように
                if (isIgnoreFreeTime)
                {
                    if (i != timeSchedule.Count - 1)
                    {
                        nextSchedule = timeSchedule[i + 1];
                        if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, leagueMatchBannerSchedule.StartAt, nextSchedule.StartAt))
                            return leagueMatchBannerSchedule;
                    }
                    else
                    {
                        if (DateTimeExtensions.IsSameDay(AppTime.Now, leagueMatchBannerSchedule.StartAt) &&
                            DateTimeExtensions.IsPast(leagueMatchBannerSchedule.StartAt, AppTime.Now))
                            return leagueMatchBannerSchedule;
                    }
                }
                else
                {
                    // 現在の時刻がスケジュールの期間内なら
                    if (DateTimeExtensions.IsWithinPeriod(AppTime.Now, leagueMatchBannerSchedule.StartAt, leagueMatchBannerSchedule.NextScheduleStartAt))
                    {
                        return leagueMatchBannerSchedule;
                    }
                }
            }
            return null;
        }
    }
}