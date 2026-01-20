using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using NUnit.Framework;
using Pjfb.CampaignBanner;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Editor.Tests
{
    public class CampaignBannerManagerTest
    {
        [Test]
        public void ShownPopupDataContainer_BulkTestProcessFlow()
        {
            var shownPopupDataContainer = new CampaignBannerManager.ShownPopupDataContainer();
            var imagePathFormat = "Images/Home/{0}/Banner.png";
            var popupDataList = new List<NewsPopup>();

            var now = AppTime.Now;
            var today = now.Date;
            var showingNewsPopup = new List<CampaignBannerModalWindow.BannerData>();

            // 1.表示されたものはない状態、翌日まで開催されるポップアップを表示する
            popupDataList.AddRange(new List<NewsPopup>{
                new() {endAt = ToDateString(today.AddDays(1)), imagePath = string.Format(imagePathFormat, "home_popup_99999")},
            });
            showingNewsPopup = shownPopupDataContainer.GetShowingNewsPopup(popupDataList, now)
                .Select(aData => new CampaignBannerModalWindow.BannerData(aData)).ToList();
            Assert.AreEqual(expected: popupDataList.Count, actual: showingNewsPopup.Count);
            shownPopupDataContainer.OnFinishShowPopup(shownPopup: new CampaignBannerModalWindow.BannerData(popupDataList[0]), now);
            Assert.AreEqual(shownPopupDataContainer.shownPopupDataList.Count, showingNewsPopup.Count);
            
            // 2.すでに表示されて、表示するものがない状態
            showingNewsPopup = shownPopupDataContainer.GetShowingNewsPopup(popupDataList, now)
                .Select(aData => new CampaignBannerModalWindow.BannerData(aData)).ToList();
            Assert.AreEqual(expected: 0, actual: showingNewsPopup.Count);
            
            // 3.期間過ぎた状態（now時間がその翌日）。時間が1秒超えた際履歴情報が消える
            shownPopupDataContainer.ClearEndedPopups(now: today.AddDays(1));
            Assert.AreEqual(expected: 1, actual: shownPopupDataContainer.shownPopupDataList.Count);
            shownPopupDataContainer.ClearEndedPopups(now: today.AddDays(1).AddSeconds(1));
            Assert.AreEqual(expected: 0, actual: shownPopupDataContainer.shownPopupDataList.Count);
        }

        [Test]
        public void ShownPopupDataContainer_SaveLoadPlayerPrefs()
        {
            // 1.アプリインストール直後、PlayerPrefsが空状態
            ObscuredPrefs.DeleteAll();
            var shownPopupDataContainer = CampaignBannerManager.ShownPopupDataContainer.LoadFromPlayerPrefs();
            Assert.IsTrue(shownPopupDataContainer != null);
            Assert.IsTrue(shownPopupDataContainer.shownPopupDataList != null);

            // 2.空履歴をセーブし、ロードするテスト
            shownPopupDataContainer = new CampaignBannerManager.ShownPopupDataContainer();
            shownPopupDataContainer.Save();
            shownPopupDataContainer = CampaignBannerManager.ShownPopupDataContainer.LoadFromPlayerPrefs();
            Assert.AreEqual(expected: 0, actual: shownPopupDataContainer.shownPopupDataList.Count);
            
            // 3.履歴有りをセーブし、ロードするテスト
            shownPopupDataContainer.shownPopupDataList.AddRange(new List<CampaignBannerManager.ShownPopupData>
            {
                new(){endAt = "1.endAt", imagePath = "1.imagePath"},
                new(){endAt = "2.endAt", imagePath = "2.imagePath"},
            });
            shownPopupDataContainer.Save();
            shownPopupDataContainer = CampaignBannerManager.ShownPopupDataContainer.LoadFromPlayerPrefs();
            Assert.AreEqual(expected: 2, actual: shownPopupDataContainer.shownPopupDataList.Count);
            Assert.AreEqual(expected: "1.endAt", actual: shownPopupDataContainer.shownPopupDataList[0].endAt);
            Assert.AreEqual(expected: "2.imagePath", actual: shownPopupDataContainer.shownPopupDataList[1].imagePath);
        }

        private string ToDateString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}