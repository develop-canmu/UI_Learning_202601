using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CruFramework;
using CruFramework.Addressables;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Logger = CruFramework.Logger;

namespace Pjfb.Community
{
    public static class CommunityManager
    {
        #region PublicProperties
        public static bool showHomeBadge { get { return ShowClubChatBadge || ShowPersonalChatBadge || ShowFollowBadge; } }
        public static long yellCount = 0;
        public static long unViewedYellCount = 0;
        public static long unViewedClubChatCount = 0;
        public static long unViewedClubInfoCount = 0;
        public static long unViewedChatCount = 0;
        
        
        public static CommunityGetTodayYellDetailAPIResponse yellDetail = new();
        public static HashSet<UserCommunityUserStatus> followUserList = new();
        public static HashSet<UserCommunityUserStatus> blockUserList = new();
        public static List<long> guildMemberIdList = new();
        public static bool ShowFollowBadge { get { return ShowYellAllBadge || ShowYellHistoryBadge; } }
        public static bool ShowYellHistoryBadge { get {  return unViewedYellCount > 0; } }
        public static bool ShowPersonalChatBadge { get { return unViewedChatCount > 0; } }
        public static bool ShowClubChatBadge { get { return unViewedClubChatCount > 0; } }
        public static bool ShowYellAllBadge { get { return yellCount < ConfigManager.Instance.yellLimit && (yellDetail?.followedCanYellCount > 0 || yellDetail?.guildCanYellCount > 0); } }
        public static bool isGetBlockUserList = false;  //ブロックリストを取得したか

        #endregion

        #region StaticMethods
        
        public static void OnClickCommunityButton()
        {
            Logger.Log("CommunityManager.OnClickCommunityButton");
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, null);
        }
        
        public static DateTime GetDateTimeByString(string dateTimeString, CultureInfo ci = null)
        {
            var dateTime = DateTime.MinValue;
            var parseSuccess = DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", ci, DateTimeStyles.None, out dateTime);
            if (!parseSuccess) DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd", ci, DateTimeStyles.None, out dateTime);
            return dateTime;
        }
        
        public static string GetDateTimeDiffByString(string dateTimeString)
        {
            var lastDate = GetDateTimeByString(dateTimeString);
            // サーバー時間との差分を取得
            var diff = AppTime.Now - lastDate;
            var result = string.Empty;
            if (diff.Days >= 365) result = string.Format(StringValueAssetLoader.Instance["community.chat.over_one_year"]);//１年以上前
            else if (diff.Days > 0) result = string.Format(StringValueAssetLoader.Instance["community.chat.days_ago"],diff.Days);//{0}日前
            else if (diff.Hours > 0) result = string.Format(StringValueAssetLoader.Instance["community.chat.hours_ago"],diff.Hours);//{0}時間前
            else if (diff.Minutes >= 0) result = string.Format(StringValueAssetLoader.Instance["community.chat.minutes_ago"],diff.Minutes);//{0}分
            return result;
        }

        public static async UniTask UpdateActionInterval(float interval,Action action,CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(interval),cancellationToken: cancellationTokenSource.Token);
                action.Invoke();
            }
        }

        public static bool CheckCanYell(long targetUMasterId)
        {
            return  yellDetail != null &&
                    yellCount < ConfigManager.Instance.yellLimit &&
                    yellDetail.todayYelledList.All(yell => yell.uMasterId != targetUMasterId);
        }

        public static void UpdateGuildCanYellCount()
        {
            Func<long,bool> memberCanYell = (id) =>
            {
                return id != UserDataManager.Instance.user.uMasterId && yellDetail.todayYelledList.All(yell => yell.uMasterId != id);
            };
            yellDetail.guildCanYellCount = guildMemberIdList.Count(memberCanYell);
        }

        /// <summary>
        /// エール情報を取得するAPI呼び出し
        /// </summary>
        public static async UniTask GetTodayYellDetailAPI()
        {
            CommunityGetTodayYellDetailAPIRequest request = new CommunityGetTodayYellDetailAPIRequest();
            await APIManager.Instance.Connect(request);
        }

        /// <summary>
        /// エール一括送信モーダルを開く
        /// </summary>
        public static void OpenYellAllModal()
        {
            YellAllModalWindow.Open(new YellAllModalWindow.WindowParams
            {
                showNotification = message => AppManager.Instance.UIManager.System.UINotification.ShowNotification(message),
                onYellSent = ()=> GetTodayYellDetailAPI().Forget()
            });
        }

        #endregion
    }
}