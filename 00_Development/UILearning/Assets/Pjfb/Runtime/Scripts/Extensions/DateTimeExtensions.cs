using System;
using System.Globalization;

namespace Pjfb.Extensions
{
    public static class DateTimeExtensions
    {
        public enum DateTimeStringFormat
        {
            DateTime = 0,
            DateYearDay = 1,
        }
        
        /// <summary>
        /// テスト処理：DateTimeExtensionTest.GetRemainingString 
        /// </summary>
        public static string GetRemainingString(this DateTime futureDate, DateTime pastDate, int remainingDayLimit = -1, string textFormat = "{0}")
        {
            var remainTimeSpan = futureDate - pastDate;
            return -1 < remainingDayLimit && remainingDayLimit < remainTimeSpan.Days ? string.Empty : GetRemainingString(remainTimeSpan, textFormat);
        }

        private static string GetRemainingString(TimeSpan remainTimeSpan, string textFormat)
        {
            if (remainTimeSpan.Milliseconds > 0) remainTimeSpan = remainTimeSpan.Add(new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 1, milliseconds: -remainTimeSpan.Milliseconds));

            return
                remainTimeSpan.Days > 0 ? string.Format(textFormat, $"{remainTimeSpan.Days}日") :
                remainTimeSpan.Hours > 0 ? string.Format(textFormat, $"{remainTimeSpan.Hours}時間") :
                remainTimeSpan.Minutes > 0 ? string.Format(textFormat, $"{remainTimeSpan.Minutes}分") :
                remainTimeSpan.Seconds > 0 ? string.Format(textFormat, $"{remainTimeSpan.Seconds}秒") :
                string.Empty;
        }

        /// <summary>
        /// リアルタイムで時間の表示を更新するのにおすすめ
        /// </summary>
        public static string GetPreciseRemainingString(this DateTime futureDate, DateTime pastDate, int remainingDayLimit = -1, string textFormat = "{0}", string defaultString = "")
        {
            var remainTimeSpan = futureDate - pastDate;
            return -1 < remainingDayLimit && remainingDayLimit < remainTimeSpan.Days ? string.Empty : GetPreciseRemainingString(remainTimeSpan, textFormat, defaultString);
        }

        public static string GetPreciseRemainingString(TimeSpan remainTimeSpan, string textFormat, string defaultString)
        {
            if (remainTimeSpan.Milliseconds > 0) remainTimeSpan = remainTimeSpan.Add(new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 1, milliseconds: -remainTimeSpan.Milliseconds));
            
            return
                remainTimeSpan.Days > 0 ? string.Format(textFormat, $"{remainTimeSpan.Days}日") :
                remainTimeSpan.Hours > 0 ? string.Format(textFormat, $"{remainTimeSpan.Hours}時間{remainTimeSpan.Minutes}分") :
                remainTimeSpan.Minutes > 0 ? string.Format(textFormat, $"{remainTimeSpan.Minutes}分{remainTimeSpan.Seconds}秒") :
                remainTimeSpan.Seconds > 0 ? string.Format(textFormat, $"{remainTimeSpan.Seconds}秒") :
                defaultString;
        }
        
        public static DateTime GetNextStepDate(this DateTime startAt, long stepDay, DateTime now)
        {
            var timeSpan = now - startAt;
            if (timeSpan.TotalDays < 0) return startAt;
            
            var repeatedStepCount = timeSpan.TotalDays / stepDay;
            var nextStepCount = (long)repeatedStepCount + 1;
            return startAt.AddDays(nextStepCount * stepDay);
        }

        public static string GetNewsDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(format: "yyyy/MM/dd HH:mm");
        }
        
        public static string GetDateTimeString(this DateTime dateTime,DateTimeStringFormat stringFormat = DateTimeStringFormat.DateTime)
        {
            switch (stringFormat)
            {
                case DateTimeStringFormat.DateTime:
                    return dateTime.ToString(format: "MM/dd HH:mm");
                case DateTimeStringFormat.DateYearDay:
                    return dateTime.ToString(format: "yyyy/MM/dd");
            }
            return null;
        }
        
        /// <summary>
        /// サーバからもらったstring形の日付をDateTimeに変換する
        /// </summary>
        /// <returns>変換失敗したらDateTime.MinValueを返します</returns>
        public static DateTime TryConvertToDateTime (this string serverFormatDateTimeString, bool minValueDefault = true)
        {
            if(DateTime.TryParseExact(serverFormatDateTimeString, "yyyy-MM-dd HH:mm:ss", new DateTimeFormatInfo(), DateTimeStyles.None, out var result))
            {
                return result;
            }
            return DateTime.TryParseExact(serverFormatDateTimeString, "yyyy-MM-dd", new DateTimeFormatInfo(), DateTimeStyles.None, out result) ? result : minValueDefault ? DateTime.MinValue : DateTime.MaxValue;
        }

        public static TimeSpan GetTimeSpanRemaining(this DateTime dateTime, DateTime now)
        {
            return dateTime - now;
        }

        public static TimeSpan GetTimeSpanElapsed(this DateTime dateTime, DateTime now)
        {
            return now - dateTime;
        }

        public static bool IsPast(this DateTime dateTime, DateTime now)
        {
            return dateTime.CompareTo(now) == -1;
        }
        
        public static bool IsFuture(this DateTime dateTime, DateTime now)
        {
            return dateTime.CompareTo(now) == 1;
        }

        public static bool IsSame(this DateTime dateTime, DateTime now)
        {
            return dateTime.CompareTo(now) == 0;
        }

        public static bool IsSameMinute(this DateTime dateTime, DateTime now)
        {
            return 
                dateTime.Minute == now.Minute &&
                dateTime.Hour == now.Hour &&
                dateTime.DayOfYear == now.DayOfYear;
        }
        
        public static bool IsWithinPeriod(DateTime now, DateTime startAt, DateTime endAt)
        {
            return startAt <= now && now <= endAt;
        }
        
        public static bool IsSameDay(DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
        }
        
        public static DateTime GetDateTimeForSameRef(DateTime givenDateTime, DateTime ymdRef)
        {
            DateTime dateTimeToday = new DateTime(ymdRef.Year, ymdRef.Month, ymdRef.Day,
                givenDateTime.Hour, givenDateTime.Minute, givenDateTime.Second,
                givenDateTime.Millisecond);
            return dateTimeToday;
        }
    }
}