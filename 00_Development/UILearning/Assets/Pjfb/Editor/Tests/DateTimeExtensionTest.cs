using System;
using Pjfb.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Pjfb.Editor.Tests
{
    public class DateTimeExtensionTest
    {
        [Test]
        public void GetRemainingString()
        {
            var now = DateTime.Now;
            var future = now;
            Assert.AreEqual(string.Empty, future.GetRemainingString(now));

            future = now.AddMilliseconds(1);
            Assert.AreEqual("1秒", future.GetRemainingString(now));
            
            future = now.AddSeconds(1);
            Assert.AreEqual("1秒", future.GetRemainingString(now));
            
            future = now.AddSeconds(1).AddMilliseconds(1);
            Assert.AreEqual("2秒", future.GetRemainingString(now));
            
            future = now.AddSeconds(1).AddMilliseconds(-1);
            Assert.AreEqual("1秒", future.GetRemainingString(now));
            
            future = now.AddMinutes(1);
            Assert.AreEqual("1分", future.GetRemainingString(now));
            
            future = now.AddMinutes(59);
            Assert.AreEqual("59分", future.GetRemainingString(now));
            
            future = now.AddHours(2);
            Assert.AreEqual("2時間", future.GetRemainingString(now));
            
            future = now.AddHours(2).AddSeconds(-1);
            Assert.AreEqual("1時間", future.GetRemainingString(now));
            
            future = now.AddDays(2);
            Assert.AreEqual("2日", future.GetRemainingString(now));
            
            future = now.AddDays(2).AddSeconds(-1);
            Assert.AreEqual("1日", future.GetRemainingString(now));
            
            future = now.AddSeconds(-1);
            Assert.AreEqual(string.Empty, future.GetRemainingString(now));
            
            future = now.AddDays(1);
            Assert.AreEqual(string.Empty, future.GetRemainingString(now, remainingDayLimit: 0));
            Assert.AreEqual(string.Empty, future.GetRemainingString(now, remainingDayLimit: 0, "あと{0}"));
            Assert.AreEqual("1日", future.GetRemainingString(now, remainingDayLimit: 1));
            Assert.AreEqual("あと1日", future.GetRemainingString(now, remainingDayLimit: 1, textFormat: "あと{0}"));
            
            future = now.AddYears(1);
            var daysInYear = (future - now).TotalDays;
            Assert.AreEqual(string.Empty, future.GetRemainingString(now, remainingDayLimit: (int)daysInYear - 1, textFormat: "あと{0}"));
            Assert.AreEqual($"あと{daysInYear}日", future.GetRemainingString(now, remainingDayLimit: (int)daysInYear, textFormat: "あと{0}"));
        }

        [Test]
        public void GetPreciseRemainingString()
        {
            var now = DateTime.Now;
            var future = now;
            Assert.AreEqual("終了", future.GetPreciseRemainingString(now, defaultString: "終了"));
            
            future = now.AddMilliseconds(-1);
            Assert.AreEqual("終了", future.GetPreciseRemainingString(now, defaultString: "終了"));
            
            future = now.AddSeconds(-1);
            Assert.AreEqual("終了", future.GetPreciseRemainingString(now, defaultString: "終了"));
            
            future = now.AddMilliseconds(1);
            Assert.AreEqual("1秒", future.GetPreciseRemainingString(now));
            
            future = now.AddSeconds(1);
            Assert.AreEqual("1秒", future.GetPreciseRemainingString(now));
            
            future = now.AddSeconds(1).AddMilliseconds(1);
            Assert.AreEqual("2秒", future.GetPreciseRemainingString(now));
            
            future = now.AddSeconds(1).AddMilliseconds(-1);
            Assert.AreEqual("1秒", future.GetPreciseRemainingString(now));
            
            future = now.AddMinutes(1);
            Assert.AreEqual("1分0秒", future.GetPreciseRemainingString(now));
            
            future = now.AddMinutes(59);
            Assert.AreEqual("59分0秒", future.GetPreciseRemainingString(now));
            
            future = now.AddHours(2);
            Assert.AreEqual("2時間0分", future.GetPreciseRemainingString(now));
            
            future = now.AddHours(2).AddSeconds(-1);
            Assert.AreEqual("1時間59分", future.GetPreciseRemainingString(now));
            
            future = now.AddDays(2);
            Assert.AreEqual("2日", future.GetPreciseRemainingString(now));
            
            future = now.AddDays(2).AddSeconds(-1);
            Assert.AreEqual("1日", future.GetPreciseRemainingString(now));
            
            future = now.AddSeconds(-1);
            Assert.AreEqual(string.Empty, future.GetPreciseRemainingString(now));
            
            future = now.AddDays(1);
            Assert.AreEqual(string.Empty, future.GetPreciseRemainingString(now, remainingDayLimit: 0));
            Assert.AreEqual(string.Empty, future.GetPreciseRemainingString(now, remainingDayLimit: 0, "あと{0}"));
            Assert.AreEqual("1日", future.GetPreciseRemainingString(now, remainingDayLimit: 1));
            Assert.AreEqual("あと1日", future.GetPreciseRemainingString(now, remainingDayLimit: 1, textFormat: "あと{0}"));
            
            future = now.AddYears(1);
            Assert.AreEqual(string.Empty, future.GetPreciseRemainingString(now, remainingDayLimit: 364, textFormat: "あと{0}"));
            Assert.AreEqual("あと365日", future.GetPreciseRemainingString(now, remainingDayLimit: 365, textFormat: "あと{0}"));
        }
        
        [Test]
        public void GetNextStepDate()
        {
            var startAt = new DateTime(year: 2022, month: 1, day: 1, hour: 4, minute: 0, second: 0);
            
            Assert.AreEqual(expected: startAt.AddDays(0), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(-1)));
            Assert.AreEqual(expected: startAt.AddDays(0), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(-5)));
            Assert.AreEqual(expected: startAt.AddDays(0), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(-23)));
            Assert.AreEqual(expected: startAt.AddDays(1), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt));
            Assert.AreEqual(expected: startAt.AddDays(1), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(1)));
            Assert.AreEqual(expected: startAt.AddDays(1), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(5)));
            Assert.AreEqual(expected: startAt.AddDays(1), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(23)));
            Assert.AreEqual(expected: startAt.AddDays(2), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(24)));
            Assert.AreEqual(expected: startAt.AddDays(2), actual: startAt.GetNextStepDate(stepDay: 1, now: startAt.AddHours(47)));
            
            Assert.AreEqual(expected: startAt.AddDays(0), actual: startAt.GetNextStepDate(stepDay: 2, now: startAt.AddHours(-1)));
            Assert.AreEqual(expected: startAt.AddDays(2), actual: startAt.GetNextStepDate(stepDay: 2, now: startAt));
            Assert.AreEqual(expected: startAt.AddDays(2), actual: startAt.GetNextStepDate(stepDay: 2, now: startAt.AddHours(1)));
            Assert.AreEqual(expected: startAt.AddDays(2), actual: startAt.GetNextStepDate(stepDay: 2, now: startAt.AddHours(47)));
        }
        
        [Test]
        public void GetDateTimeString()
        {
            var dateTime = new DateTime(year: 2022, month: 1, day: 1, hour: 23, minute: 59, second: 59);
            Assert.AreEqual("2022/1/1 23:59", dateTime.GetNewsDateTimeString());
            
            dateTime = new DateTime(year: 2022, month: 1, day: 1, hour: 9, minute: 9, second: 59);
            Assert.AreEqual("2022/1/1 9:9", dateTime.GetNewsDateTimeString());
        }

        [Test]
        public void IsPast()
        {
            var dateTime = new DateTime(year: 2022, month: 1, day: 1, hour: 23, minute: 59, second: 59);
            Assert.AreEqual(true, dateTime.IsPast(dateTime.AddSeconds(1)));
            Assert.AreEqual(false, dateTime.IsPast(dateTime.AddSeconds(-1)));
            Assert.AreEqual(false, dateTime.IsPast(dateTime));
        }
        
        [Test]
        public void IsFuture()
        {
            var dateTime = new DateTime(year: 2022, month: 1, day: 1, hour: 23, minute: 59, second: 59);
            Assert.AreEqual(false, dateTime.IsFuture(dateTime.AddSeconds(1)));
            Assert.AreEqual(true, dateTime.IsFuture(dateTime.AddSeconds(-1)));
            Assert.AreEqual(false, dateTime.IsPast(dateTime));
        }
        
        [Test]
        public void IsSameMinute()
        {
            var dateTime = new DateTime(year: 2022, month: 1, day: 1, hour: 23, minute: 59, second: 59);
            Assert.IsTrue(dateTime.IsSameMinute(dateTime));
            Assert.IsTrue(dateTime.AddSeconds(-dateTime.Second).IsSameMinute(dateTime));
            Assert.IsFalse(dateTime.AddSeconds(1).IsSameMinute(dateTime));
            
            Assert.IsFalse(dateTime.AddMinutes(1).IsSameMinute(dateTime));
            Assert.IsFalse(dateTime.AddMinutes(-1).IsSameMinute(dateTime));
            
            Assert.IsFalse(dateTime.AddHours(1).IsSameMinute(dateTime));
            Assert.IsFalse(dateTime.AddHours(-1).IsSameMinute(dateTime));
            
            Assert.IsFalse(dateTime.AddDays(1).IsSameMinute(dateTime));
            Assert.IsFalse(dateTime.AddDays(-1).IsSameMinute(dateTime));
        }
    }
}
