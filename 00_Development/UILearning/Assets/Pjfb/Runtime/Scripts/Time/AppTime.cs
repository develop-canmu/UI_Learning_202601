using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Pjfb
{
    public static class AppTime
    {
        static DateTime serverTime = DateTime.Now;
        static double elapsedTime = 0.0;

        /// <summary>現在時間を取得する</summary>
        public static DateTime Now
        {
            get
            {
                // 最後に受け取ったサーバー時間に経過時間を追加
                return serverTime + TimeSpan.FromSeconds(Time.realtimeSinceStartupAsDouble - elapsedTime);
            }
        }
        
        public static DateTime Parse(string timeString)
        {
            return DateTime.Parse(timeString);
        }
        
        /// <summary>期限内かチェック</summary>
        public static bool IsInPeriod(string startAtString, string endAtString)
        {
            DateTime startAt = DateTime.Parse(startAtString);
            DateTime endAt = DateTime.Parse(endAtString);
            return IsInPeriod(startAt, endAt);
        }
        
        /// <summary>期限内かチェック</summary>
        public static bool IsInPeriod(DateTime startAt, DateTime endAt)
        {
            DateTime now = Now;
            return now >= startAt && now <= endAt;
        }
        
        /// <summary>期限内かチェック</summary>
        public static bool IsInPeriod(DateTime startAt)
        {
            DateTime now = Now;
            return now >= startAt;
        }
        
        /// <summary>期限内かチェック</summary>
        public static bool IsInPeriod(string startAtString)
        {
            DateTime startAt = DateTime.Parse(startAtString);
            return IsInPeriod(startAt);
        }

        //// <summary> サーバーと通信した最終時間を使っての期限内チェック </summary>
        public static bool IsInPeriodLatestAPITime(string startAtString, string endAtString)
        {
            DateTime startAt = DateTime.Parse(startAtString);
            DateTime endAt = DateTime.Parse(endAtString);
            return serverTime >= startAt && serverTime <= endAt;
        }
        
        /// <summary>EndAt基準で期限内かチェック</summary>
        public static bool IsInPeriodEndAt(string endAtString)
        {
            DateTime endAt = DateTime.Parse(endAtString);
            return IsInPeriodEndAt(endAt);
        }
        
        /// <summary>EndAt基準で期限内かチェック</summary>
        public static bool IsInPeriodEndAt(DateTime endAt)
        {
            DateTime now = Now;
            return now <= endAt;
        }

        static public void Reset( string dataTimeText ){
            // サーバー時間を保持
            serverTime = DateTime.ParseExact(dataTimeText, "yyyy-MM-dd HH:mm:ss", null);
            // 端末起動時からの経過時間を保持
            elapsedTime = Time.realtimeSinceStartupAsDouble;
        }
    }
}
