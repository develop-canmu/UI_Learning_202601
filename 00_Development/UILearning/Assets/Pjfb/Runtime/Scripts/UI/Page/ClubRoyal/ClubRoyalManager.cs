using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb
{
    public static class ClubRoyalManager
    {
        //// <summary> クラブロワイヤルの開催状況のState </summary>
        public enum State
        {
            //// <summary> 準備中 </summary>
            InPreparation,

            //// <summary> 入場前(試合準備中) </summary>
            BeforeEntry,

            //// <summary> 入場可能(試合準備受付中) </summary>
            BattleEntry,
            
            //// <summary> 試合開催中 </summary>
            BattleStart,

            //// <summary> 本日の試合終了 </summary>
            BattleEnd,

            //// <summary> 全試合終了 </summary>
            AllBattleEnd,
        }

        //// <summary> クラブロワイヤルのスケジュールを返す(開催状況と更新が必要になる時間) </summary>
        public struct Schedule
        {
            // 開催状況
            private State state;
            public State State => state;
            
            // 次の更新が必要になる時間
            private DateTime nextUpdateTime;
            public DateTime NextUpdateTime => nextUpdateTime;

            // 次のスケジュール開始時間
            private DateTime nextScheduleTime;
            public DateTime NextScheduleTime => nextScheduleTime;
            
            public Schedule(State state, DateTime nextUpdateTime, DateTime nextScheduleTime)
            {
                this.state = state;
                this.nextUpdateTime = nextUpdateTime;
                this.nextScheduleTime = nextScheduleTime;
            }
        }

        //// <summary> クラブロワイヤルの現在のStateを返す </summary>
        public static State GetClubRoyalState(LeagueMatchInfo matchInfo, bool isCalculateBattleEarlyFinish = true)
        {
            // 開催中
            if (matchInfo.IsOnSeason)
            {
                // 次のスケジュールを取得
                LeagueMatchInfoSchedule nextSchedule = matchInfo.GetNextSchedule();
                // 現在のスケジュール(空き時間も考慮する)
                LeagueMatchInfoSchedule currentSchedule = matchInfo.GetCurrentSchedule(false);

                // 今日の試合がすでに終了しているか
                bool isTodayMatchFinish = isCalculateBattleEarlyFinish && matchInfo.HasTodayMatchResult();
                
                // 現在のスケジュールがあるなら
                if (currentSchedule != null)
                {
                    // バトル中(終了予定時刻よりも早く対戦が終わった場合は除く)   
                    if (currentSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle && isTodayMatchFinish == false)
                    {
                        return State.BattleStart;
                    }

                    // バトル準備画面に入場できる
                    if (currentSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.BattleEntrance)
                    {
                        return State.BattleEntry;
                    }
                }
                
                // 次のスケジュールがないなら全てのスケジュールが終わっている
                if (nextSchedule == null)
                {
                    // 全バトル終了
                    return State.AllBattleEnd;
                }

                // 編成確定前
                if (nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.FormationLock)
                {
                    // 今日行われる対戦スケジュールを取得
                    LeagueMatchInfoSchedule todayBattleSchedule = matchInfo.GetBattleRoyalScheduleOfToday();
                    // 対戦が終了する時間
                    DateTime battleEndAt = DateTime.MaxValue;
                    // 対戦終了時間をセット
                    if (todayBattleSchedule != null)
                    {
                        battleEndAt = todayBattleSchedule.NextScheduleStartAt;
                    }

                    // すでに対戦が終わっているなら(予定終了時刻よりも早く終わっている場合も含める)
                    if (battleEndAt.IsPast(AppTime.Now) || isTodayMatchFinish)
                    {
                        // シーズン戦終了日情報を取得(時間は切り捨て)
                        DateTime seasonBattleEndDate = matchInfo.SeasonBattleEndAt.Date;
                        
                        // シーズン戦最終日の試合が終わっている
                        // 入れ替え戦以外がすでに終わっているなら全試合終了に
                        if (seasonBattleEndDate.IsPast(AppTime.Now))
                        {
                            return State.AllBattleEnd;
                        }
                        
                        // まだなら一日の試合終了
                        return State.BattleEnd;
                    }
                    // まだなら入場前
                    else
                    {
                        return State.BeforeEntry;
                    }
                }
                // 次が入場なら試合準備中
                else if (nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.BattleEntrance)
                {
                    return State.BeforeEntry;
                }
                // 次のスケジュールが対戦なら入場が可能に
                else if (nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle)
                {
                    // 入場可能
                    return State.BattleEntry;
                }
            }

            // 準備中
            return State.InPreparation;
        }

        //// <summary> クラブロワイヤルのスケジュールを取得 </summary>
        public static Schedule GetSchedule(LeagueMatchInfo matchInfo)
        {
            // 現在の開催状況を取得
            State currentState = GetClubRoyalState(matchInfo);
            // 次の更新時間
            DateTime nextUpdateTime = DateTime.MaxValue;
            // 次のスケジュール開始時間
            DateTime nextScheduleTime = DateTime.MaxValue;
            
            // 開催状況ごとに更新をかける時間を取得
            switch (currentState)
            {
                case State.InPreparation:
                {
                    // 次回開催予定の日付が過去でないなら次のシーズン開始時間を更新時間にする
                    if (matchInfo.NextSeasonStartAt.IsFuture(AppTime.Now))
                    {
                        // 次のシーズンが開始する時間
                        nextUpdateTime = matchInfo.NextSeasonStartAt;
                    }
                    nextScheduleTime = matchInfo.NextSeasonStartAt;
                    break;
                }
                case State.AllBattleEnd:
                {
                    // まだ入れ替え戦が始まってないなら入れ替え戦が開始する時間を更新時間に
                    if (matchInfo.ShiftBattleStartAt.IsFuture(AppTime.Now))
                    {
                        nextUpdateTime = matchInfo.ShiftBattleStartAt;
                    }
                    else
                    {
                        // 今のシーズンが終了する時間
                        nextUpdateTime = matchInfo.SeasonEndAt;
                    }
                    // 次のスケジュール時間を次シーズン開始時間に
                    nextScheduleTime = matchInfo.NextSeasonStartAt;
                    
                    break;
                }
                case State.BeforeEntry:
                {
                    // 次に行われる入場開始のスケジュールを取得
                    LeagueMatchInfoSchedule schedule = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.BattleEntrance);
                    if (schedule != null)
                    {
                        // 入場開始の時間を次の更新時間に
                        nextUpdateTime = schedule.StartAt;
                        nextScheduleTime = schedule.StartAt;
                    }
                    break;
                }
                case State.BattleEntry:
                {
                    // 次に行われる戦闘開始のスケジュールを取得
                    LeagueMatchInfoSchedule schedule = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.Battle);
                    if (schedule != null)
                    {
                        // 戦闘開始の時間を次の更新時間に
                        nextUpdateTime = schedule.StartAt;
                        nextScheduleTime = schedule.StartAt;
                    }
                    break;
                }
                case State.BattleStart:
                {
                    // 今日行われる戦闘終了のスケジュールを取得
                    LeagueMatchInfoSchedule schedule = matchInfo.GetTodaySchedule(LeagueMatchInfoSchedule.Schedule.Battle);
                    if (schedule != null)
                    {
                        // 戦闘終了の時間を次の更新時間に
                        nextUpdateTime = schedule.NextScheduleStartAt;
                        nextScheduleTime = schedule.NextScheduleStartAt;
                    }
                    break;
                }
                case State.BattleEnd:
                {
                    // 次の受付開始時間(対戦相手決定時間)を更新時間に
                    DateTime entryStartTime = matchInfo.GetTeamEntryStartTime();
                    nextUpdateTime = entryStartTime;
                    nextScheduleTime = entryStartTime;
                    break;
                }
            }

            // スケジュールを返す
            return new Schedule(currentState, nextUpdateTime, nextScheduleTime);
        }

        //// <summary> 現在、自動配置の変更ができるか </summary>
        public static bool CanChangeAutoFormationSetting(LeagueMatchInfo matchInfo)
        {
            // 現在の開催状況を取得
            // 自動配置は試合の早期終了を考慮しない
            State state = GetClubRoyalState(matchInfo, false);
            
            // 入場可能、試合開始中の時は変更ができない
            if (state == State.BattleEntry || state == State.BattleStart)
            {
                return false;
            }

            return true;
        }
        
        //// <summary> 自動配置の変更が出来ない旨を表示するモーダルを開く </summary>
        public static async UniTask OpenCantChangeAutoFormationModal(LeagueMatchInfo matchInfo)
        {
            BattleGameliftMasterObject master = matchInfo.GetBattleGameLiftMasterObject();
            // 本日の入場開始時間
            DateTime battleEntranceStartAt = master.battleStartAt.TryConvertToDateTime();
            // 試合終了時間
            DateTime battleEndAt = master.battleFinishAt.TryConvertToDateTime();
            
            // 変更出来ない時間帯を表示するモーダルを開く
            ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
            string title = StringValueAssetLoader.Instance["club-royal.auto_formation_modal.caution.title"];
            // 時間表示フォーマット
            string dateTimeFormat = StringValueAssetLoader.Instance["common.datetime_format.1"];
            string message = string.Format(StringValueAssetLoader.Instance["club-royal.auto_formation_modal.cannot_change.message"], battleEntranceStartAt.ToString(dateTimeFormat), battleEndAt.ToString(dateTimeFormat));
            ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, negativeButton);
            
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
        }
    }
}