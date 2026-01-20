using System;
using UnityEngine;
using System.Collections.Generic;
using Pjfb.Extensions;
using Pjfb.UserData;
using System.Linq;
using Pjfb.ClubMatch;
using Pjfb.LeagueMatch;
using Pjfb.LeagueMatchTournament;
using Pjfb.Master;
using WrapperIntList = Pjfb.Networking.App.Request.WrapperIntList;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace Pjfb.Runtime.Scripts.Utility
{
    public class LocalPushNotificationUtility : MonoBehaviour
    {
        /// <summary>GvG系の通知登録用Param</summary>
        private class GvgRegisterParam
        {
            private PushMasterObject pushMaster;
            public PushMasterObject PushMaster => pushMaster;
            private int registerCount;
            public int RegisterCount => registerCount;
            public GvgRegisterParam(PushMasterObject pushMaster)
            {
                this.pushMaster = pushMaster;
                this.registerCount = 0;
            }
            public void AddCount()
            {
                registerCount++;
            }
        }
        
        /// <summary>プッシュ通知のフラグ設定</summary>
        private enum PushSettingState
        {
            On = 1,
            Off = 2,
        }
        
        /// <summary>Android用チャンネルID</summary>
        private const string CHANNEL_ID = "bluelock";
        /// <summary>Android用タイトル</summary>
        private const string TITLE = "bluelock";
        /// <summary>Android用詳細</summary>
        private const string DESCRIPTION = "bluelock";
        /// <summary>クラブマッチ開始(m_pushのkeyName)</summary>
        private const string CLUB_MATCH_BATTLE_START_KEY_NAME = "colosseumGroupBattleStart";
        /// <summary>リーグマッチエントリー終了予告(m_pushのkeyName)</summary>
        private const string LEAGUE_MATCH_TEAM_ENTRY_END_KEY_NAME = "leagueMatchTeamEntryEnd";
        /// <summary>リーグマッチ開始(m_pushのkeyName)</summary>
        private const string LEAGUE_MATCH_BATTLE_START_KEY_NAME = "leagueMatchBattleStart";
        /// <summary>簡易大会エントリー終了予告(m_pushのkeyName)</summary>
        private const string CHAMPIONS_SHIP_LEAGUE_MATCH_TEAM_ENTRY_END_KEY_NAME = "championshipLeagueMatchTeamEntryEnd";
        /// <summary>簡易大会開始(m_pushのkeyName)</summary>
        private const string CHAMPIONS_SHIP_LEAGUE_MATCH_BATTLE_START_KEY_NAME = "championshipLeagueMatchBattleStart";
        /// <summary>クラブロワイヤル入場開始</summary>>
        private const string CLUB_ROYAL_ACCESS_START_KEY_NAME = "battleGameliftAccessStart";
        /// <summary>クラブロワイヤル試合開始</summary>>
        private const string CLUB_ROYAL_BATTLE_START_KEY_NAME = "battleGameliftBattleStart";
        /// <summary>登録件数</summary>
        private static int registCount = 0;
        /// <summary>登録可能最大件数</summary>
        private const int REGIST_MAX_COUNT = 64;
        /// <summary>GVGの登録可能最大件数</summary>
        private const int GVG_REGIST_MAX_COUNT = 5;
        
        /// <summary>priorityが特定の値の場合反映しないっぽいのでその判定用…</summary>
        private const int PRIORITY_IGNORE_VALUE = 0;
        
        public static string AndroidChannelId => CHANNEL_ID;
        public static string AndroidTitle => TITLE;
        public static string AndroidDescription => DESCRIPTION;
        public static readonly string NotificationLargeIconName = "large_icon";
        public static readonly string NotificationSmallIconName = "notification_icon";
        
        /// <summary>Androidで使用するプッシュ通知用のチャンネルを登録</summary>
        public static void RegisterChannel()
        {
#if UNITY_ANDROID
            // チャンネルの登録
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = TITLE,
                Importance = Importance.High,
                Description = DESCRIPTION,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
        }

        /// <summary>ローカル通知をすべて削除</summary>
        public static void AllClear()
        {
#if UNITY_ANDROID
            // Androidの通知をすべて削除
            AndroidNotificationCenter.CancelAllScheduledNotifications();
            AndroidNotificationCenter.CancelAllNotifications();
#endif
#if UNITY_IOS
            // iOSの通知をすべて削除
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
            // バッジを消す
            iOSNotificationCenter.ApplicationBadge = 0;
#endif
            registCount = 0;
        }

        /// <summary>プッシュ通知を登録</summary>
        public static void AddSchedule(string title, string message, int badgeCount, int elapsedTime)
        {
            if (elapsedTime <= 0)
            {
                return;
            }
            if (registCount >= REGIST_MAX_COUNT)
            {
                CruFramework.Logger.LogError($"push通知の登録可能数を超過しています");
                return;
            }
            DateTime pushDate = AppTime.Now.AddSeconds(elapsedTime);
            CruFramework.Logger.Log($"[PUSH]{title}_{message}_{badgeCount}_{pushDate.ToString()}");
#if UNITY_ANDROID
            SetAndroidNotification(title, message, badgeCount, elapsedTime);
#endif
#if UNITY_IOS
            SetIOSNotification(title, message, badgeCount, elapsedTime);
#endif
            registCount++;
        }

#if UNITY_IOS
        /// <summary>通知を登録(iOS)</summary>
        private static void SetIOSNotification(string title, string message, int badgeCount, int elapsedTime)
        {
            // 通知を作成
            iOSNotificationCenter.ScheduleNotification(new iOSNotification()
            {
                //プッシュ通知を個別に取り消しなどをする場合はこのIdentifierを使用
                Identifier = $"_notification_{elapsedTime}",
                Title = title,
                Body = message,
                ShowInForeground = false,
                Badge = badgeCount,
                Trigger = new iOSNotificationTimeIntervalTrigger()
                {
                    TimeInterval = new TimeSpan(0, 0, elapsedTime),
                    Repeats = false
                }
            });
        }
#endif

#if UNITY_ANDROID
        /// <summary>通知を登録(Android) </summary>
        private static void SetAndroidNotification(string title, string message, int badgeCount, int elapsedTime)
        {
            // 通知を作成します
            var notification = new AndroidNotification
            {
                Title = title,
                Text = message,
                Number = badgeCount,
                SmallIcon = NotificationSmallIconName,
                LargeIcon = NotificationLargeIconName,
                FireTime = AppTime.Now.AddSeconds(elapsedTime)
            };

            // 通知を送信します
            AndroidNotificationCenter.SendNotification(notification, CHANNEL_ID);
        }
#endif

        /// <summary>ローカル通知を登録</summary>
        public static void SetLocalNotification()
        {
            if (UserDataManager.Instance.registeredLocalPushNotification)
            {
                return;
            }

            // ローカル通知の登録
            RegisterChannel();
            AllClear();

            // クラブマッチローカル通知登録
            SetClubMatchNotifications();
            
            // リーグマッチローカル通知登録
            // 辞書型で各スケジュールごとにkeyを設定しておく
            Dictionary<LeagueMatchInfoSchedule.Schedule, string> gvgKeyNameDict = new Dictionary<LeagueMatchInfoSchedule.Schedule, string>();
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.FormationLock, LEAGUE_MATCH_TEAM_ENTRY_END_KEY_NAME);
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.Battle, LEAGUE_MATCH_BATTLE_START_KEY_NAME);
            SetGvGNotifications(ColosseumClientHandlingType.LeagueMatch, gvgKeyNameDict);
            // 簡易大会ローカル通知登録
            gvgKeyNameDict.Clear();
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.FormationLock, CHAMPIONS_SHIP_LEAGUE_MATCH_TEAM_ENTRY_END_KEY_NAME);
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.Battle, CHAMPIONS_SHIP_LEAGUE_MATCH_BATTLE_START_KEY_NAME);
            SetGvGNotifications(ColosseumClientHandlingType.InstantTournament, gvgKeyNameDict);
            // クラブロワイヤルローカル通知登録
            gvgKeyNameDict.Clear();
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.BattleEntrance, CLUB_ROYAL_ACCESS_START_KEY_NAME);
            gvgKeyNameDict.Add(LeagueMatchInfoSchedule.Schedule.Battle, CLUB_ROYAL_BATTLE_START_KEY_NAME);
            SetGvGNotifications(ColosseumClientHandlingType.ClubRoyal, gvgKeyNameDict);
            UserDataManager.Instance.UpdateRegisteredLocalPushNotification(true);
        }

        /// <summary>
        /// クラブマッチの通知登録
        /// </summary>
        private static void SetClubMatchNotifications()
        {
            var gvgData = ClubMatchUtility.GetClubMatchBannerData();
            // クラブ未参加の場合は登録しない
            if (gvgData == null || 
                UserDataManager.Instance.user.gMasterId == 0)
            {
                return;
            }

            // 通知設定の取得
            var pushSettingResponse = UserDataManager.Instance.pushSettingList;
            var pushSettingList = pushSettingResponse.OrderBy(s => s.l[0]).ToArray();
            var pushMasterList = Master.MasterManager.Instance.pushMaster.values.Where(v => v.priority > 0).OrderBy(v=> v.priority);
            foreach( var pushMaster in pushMasterList)
            {
                var pushSetting = pushSettingList.FirstOrDefault(v => v.l[0] == pushMaster.id);
                
                // 通知オフ設定の場合
                if (pushSetting == null || pushSetting.l[1] == 2) continue;

                // クラブマッチ開始通知
                if (pushMaster.keyName == CLUB_MATCH_BATTLE_START_KEY_NAME) 
                {
                    DateTime startAt = gvgData.NextSeasonStartAt;
                    if (gvgData.HasSeasonHome && gvgData.SeasonData.IsOnSeason) 
                    {
                        startAt = gvgData.SeasonStartAt;
                    }
                    if (startAt.IsPast(AppTime.Now))
                    {
                        // 次回開催日時の取得
                        startAt+= TimeSpan.FromDays(gvgData.SeasonData.MColosseumEvent.cycleDays);
                    }
                    TimeSpan ts = startAt - AppTime.Now;
                    if (ts.TotalSeconds > 0)
                    {
                        // クラブマッチ開始の通知登録
                        int pushTime = GetPushTime(pushMaster, ts);
                        AddSchedule(pushMaster.titleText, pushMaster.bodyBaseText, 1, pushTime);
                    }
                }
            }
        }


        /// <summary>
        /// GvGの通知設定
        /// </summary>
        private static void SetGvGNotifications(ColosseumClientHandlingType handlingType, Dictionary<LeagueMatchInfoSchedule.Schedule, string> keyNameDict)
        {
            List<LeagueMatchInfo> leagueMatchInfoList;
            
            // 大会が複数開催されるためリストで取得
            if(handlingType == ColosseumClientHandlingType.InstantTournament)
            {
                leagueMatchInfoList = new List<LeagueMatchInfo>(LeagueMatchTournamentManager.GetTournamentList());
            }
            else
            {
                leagueMatchInfoList = new List<LeagueMatchInfo> { LeagueMatchUtility.GetLeagueMatchInfo(handlingType) };
            }

            // 取得したgvgごとに通知登録
            foreach (LeagueMatchInfo leagueMatchInfo in leagueMatchInfoList)
            {
                // クラブ未参加やマッチングしていない場合は登録しない
                if (leagueMatchInfo == null || 
                    leagueMatchInfo.SeasonData == null || 
                    leagueMatchInfo.SeasonData.IsMatchingFailed ||
                    leagueMatchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.groupId == 0)
                {
                    continue;
                }
                RegisterGvGNotifications(leagueMatchInfo, keyNameDict);
            }

        }

        /// <summary>
        /// GvGの通知登録
        /// </summary>
        private static void RegisterGvGNotifications(LeagueMatchInfo leagueMatchData, Dictionary<LeagueMatchInfoSchedule.Schedule, string> keyNameDict)
        {
            // 通知登録用の辞書型
            Dictionary<LeagueMatchInfoSchedule.Schedule, GvgRegisterParam> pushMasterDict = new Dictionary<LeagueMatchInfoSchedule.Schedule, GvgRegisterParam>();
            foreach (WrapperIntList setting in UserDataManager.Instance.pushSettingList)
            {
                if(setting.l[(int)UserDataManager.PushIndexType.OnOff] == (int)PushSettingState.Off) continue;
                PushMasterObject pushMaster = MasterManager.Instance.pushMaster.FindData(setting.l[(int)UserDataManager.PushIndexType.Id]);
                // priorityが特定の数値の場合は使用しない設定らしい…
                if(pushMaster.priority == PRIORITY_IGNORE_VALUE) continue;
                foreach (KeyValuePair<LeagueMatchInfoSchedule.Schedule, string> keyData in keyNameDict)
                {
                    if (pushMaster.keyName == keyData.Value)
                    {
                        pushMasterDict.Add(keyData.Key, new GvgRegisterParam(pushMaster));
                    }
                }
            }
            SetGvGNotificationsFromScheduleList(leagueMatchData.TimeSchedule, pushMasterDict);
        }
        
        /// <summary>
        /// スケジュールリストから通知登録
        /// </summary>
        private static void SetGvGNotificationsFromScheduleList(IReadOnlyList<LeagueMatchInfoSchedule> scheduleList, Dictionary<LeagueMatchInfoSchedule.Schedule,GvgRegisterParam> pushMasterDict)
        {
            foreach( LeagueMatchInfoSchedule schedule in scheduleList)
            {
                // スケジュールごとに通知登録数をカウントし、gvgの登録可能最大件数を超えた場合は登録しない
                // 試合開始については総試合数分登録するとそれだけで総合の登録可能最大件数を超えるため、gvgの登録可能最大数に一旦合わせる
                if(pushMasterDict.ContainsKey(schedule.CurrentSchedule) == false) continue;
                if(pushMasterDict[schedule.CurrentSchedule].RegisterCount >= GVG_REGIST_MAX_COUNT) continue;
                TimeSpan ts = schedule.StartAt - AppTime.Now;
                if (ts.TotalSeconds > 0)
                {
                    PushMasterObject pushMaster = pushMasterDict[schedule.CurrentSchedule].PushMaster;
                    int pushTime = GetPushTime(pushMaster, ts);
                    // 通知登録
                    AddSchedule(pushMaster.titleText, pushMaster.bodyBaseText, 1, pushTime);
                    pushMasterDict[schedule.CurrentSchedule].AddCount();
                }
            }
        }

        /// <summary>
        ///  通知時間の取得
        /// </summary>
        private static int GetPushTime(Master.PushMasterObject pushMaster, TimeSpan ts)
        {
            long delayMinutes = 0;
            if (pushMaster.optionJson.delayMinutes != 0)
            {
                delayMinutes = pushMaster.optionJson.delayMinutes * -1;
            }
            long delaySeconds = 60 * delayMinutes;
            int pushTime = (int)(ts.TotalSeconds - delaySeconds);
            return pushTime;
        }
    }
}