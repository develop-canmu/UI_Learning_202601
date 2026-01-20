using System;
using NUnit.Framework;
using Pjfb.News;

namespace Pjfb.Editor.Tests
{
    public class NewsManagerTest
    {
        [Test]
        public void SetGetPlayerPrefs_NewsDisableAutomaticShow()
        {
            NewsManager.NewsDisableTodayAutomaticShowPlayerPrefs = true;
            Assert.AreEqual(true, NewsManager.NewsDisableTodayAutomaticShowPlayerPrefs);
        
            NewsManager.NewsDisableTodayAutomaticShowPlayerPrefs = false;
            Assert.AreEqual(false, NewsManager.NewsDisableTodayAutomaticShowPlayerPrefs);
        }
    
        [Test]
        public void SetGetPlayerPrefs_NewsLastShown()
        {
            var now = DateTime.Now.Date;
            NewsManager.NewsLastShownPlayerPrefs = now;
            Assert.AreEqual(now, NewsManager.NewsLastShownPlayerPrefs);
        }

        [Test]
        public void Logic_ShouldShowNews()
        {
            // メモ：
            // disableTodayAutomaticShowはNewsModalで表示されてる「今日は表示しない」のチェックボックスのこと
            // disableTodayAutomaticShow:false の場合、基本的にお知らせが表示される結果になります
            
            // 1. 同じ時間場合
            var now = DateTime.Now;
            var testResult = NewsManager.ShouldShowNews(lastShownDateTime: now, currentDateTime: now, disableTodayAutomaticShow: true, isFromTitle: false);
            Assert.IsFalse(testResult, $"1.1.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:false");
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: now, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: false);
            Assert.IsTrue(testResult, $"1.2.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:false isFromTitle:false");
            
            // 2. 昨日表示された場合
            var yesterday = now.AddDays(-1);
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: yesterday, currentDateTime: now, disableTodayAutomaticShow: true, isFromTitle: false);
            Assert.IsTrue(testResult, $"2.1.lastShownDateTime: yesterday({yesterday}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:false");
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: yesterday, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: false);
            Assert.IsTrue(testResult, $"2.2.lastShownDateTime: yesterday({yesterday}), currentDateTime: now({now}) disableTodayAutomaticShow:false isFromTitle:false");
            
            // 3. 1秒ずれの23:59:59に表示された場合
            now = now.Date; // 00:00:00に変換
            yesterday = now.AddSeconds(-1);
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: yesterday, currentDateTime: now, disableTodayAutomaticShow: true, isFromTitle: false);
            Assert.IsTrue(testResult, $"3.1.lastShownDateTime: yesterday({yesterday}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:false");
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: yesterday, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: false);
            Assert.IsTrue(testResult, $"3.2.lastShownDateTime: yesterday({yesterday}), currentDateTime: now({now}) disableTodayAutomaticShow:false isFromTitle:false");
            
            // 4. 同じ日１時間前場合
            now = now.Date.AddHours(2);
            var sameDayOneHourBefore = now.AddHours(-1); 
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: sameDayOneHourBefore, currentDateTime: now, disableTodayAutomaticShow: true, isFromTitle: false);
            Assert.IsFalse(testResult, $"4.1.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:false");
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: now, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: false);
            Assert.IsTrue(testResult, $"4.2.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:false isFromTitle:false");
            
            // 5. isFromTitleテスト、4.1条件から「今日表示しない」フラグがfalse状態
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: sameDayOneHourBefore, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: true);
            Assert.IsTrue(testResult, $"5.1.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:true");
            testResult = NewsManager.ShouldShowNews(lastShownDateTime: sameDayOneHourBefore, currentDateTime: now, disableTodayAutomaticShow: false, isFromTitle: false);
            Assert.IsFalse(testResult, $"5.2.lastShownDateTime: now({now}), currentDateTime: now({now}) disableTodayAutomaticShow:true isFromTitle:false");
        }

    }
}
