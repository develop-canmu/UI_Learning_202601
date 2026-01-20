using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.CampaignBanner
{
    public static class CampaignBannerManager
    {
        #region PublicStatic
        public static void TryShowCampaignBanner(NewsPopup[] newsPopupList, ShownPopupDataContainer shownData, Action onComplete)
        {
            var now = AppTime.Now;
            var popupDataList = newsPopupList == null ? new List<NewsPopup>() : newsPopupList.ToList();
            var showingPopupDataList = shownData
                .GetShowingNewsPopup(popupDataList, now)
                .Select(aData => new CampaignBannerModalWindow.BannerData(aData))
                .ToList();
            
            if (showingPopupDataList.Count > 0)
            {
                CampaignBannerModalWindow.Open(new CampaignBannerModalWindow.WindowParams
                {
                    popupDataList = showingPopupDataList,
                    onShownPopup = (popupData) =>
                    {
                        shownData.OnFinishShowPopup(popupData, now);
                        shownData.Save();
                    },
                    onComplete = (windowParam) => onComplete?.Invoke()
                    
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        #endregion

        #region ShownPopupData
        [Serializable]
        public class ShownPopupData
        {
            [SerializeField] public string endAt;
            [SerializeField] public string imagePath;

            public bool Equals(NewsPopup popupData) => popupData.endAt == endAt && popupData.imagePath == imagePath;
            public override string ToString() => $"endAt:{endAt} imagePath:{imagePath}";
        }

        [Serializable]
        public class ShownPopupDataContainer
        {
            #region SerializeFields
            [SerializeField] public List<ShownPopupData> shownPopupDataList = new();
            #endregion

            #region PublicMethods
            public void OnFinishShowPopup(CampaignBannerModalWindow.BannerData shownPopup, DateTime now)
            {
                shownPopupDataList.Add(new ShownPopupData { endAt = shownPopup.endAt, imagePath = shownPopup.imagePath });
                ClearEndedPopups(now);
            }

            public List<NewsPopup> GetShowingNewsPopup(List<NewsPopup> popupDataList, DateTime now)
            {
                return popupDataList
                    .Where(aPopupData => aPopupData.endAt.TryConvertToDateTime() > now && // 期間内チェック
                                         !shownPopupDataList.Exists(aShownData => aShownData.Equals(aPopupData))) // 表示されたことないチェック
                    .ToList();
            }

            public void ClearEndedPopups(DateTime now)
            {
                shownPopupDataList = shownPopupDataList.FindAll(aData => now <= aData.endAt.TryConvertToDateTime()); // すでに終わったらplayerPrefsから削除されるように
            }
            
            public override string ToString()
            {
                var retVal = string.Empty;
                shownPopupDataList?.ForEach(aData => retVal += "\n" + aData);
                return retVal;
            }
            #endregion

            #region PlayerPrefs
            private static readonly string ShownPopupDataKey = "shownPopupDataKey";
            public void Save()
            {
                var json = ToJson();
                // Debug.Log($"ShownPopupDataContainer.Save json:{json}\n{ToString()}");
                ObscuredPrefs.Set<string>(ShownPopupDataKey, json);
            }

            public static ShownPopupDataContainer LoadFromPlayerPrefs()
            {
                var retVal = FromJson(ObscuredPrefs.Get<string>(ShownPopupDataKey, "{}"));
                // Debug.Log($"ShownPopupDataContainer.LoadFromPlayerPrefs retVal:{retVal}");
                return retVal;
            }
            #endregion

            #region PrivateMethods
            private string ToJson() => JsonUtility.ToJson(this);
            private static ShownPopupDataContainer FromJson(string json)
            {
                return JsonUtility.FromJson<ShownPopupDataContainer>(json);
            }
            #endregion
        }
        #endregion

    }
}