using System;
using System.Globalization;
using com.adjust.sdk;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class AdjustManager
    {
        
        private const string LAST_LOGIN_DATE_KEY = "lastLoginDate";
        public enum TrackEventType
        {
            None,
            UserCreate,
            ClearTutorialPatternA,
            ClearTutorialPatternB,
            JoinGuild,
            FirstPurchase,
            LoginDay1,
            LoginDay3,
            LoginDay7,
            LoginDay14,
            LoginDay30,
            LoginReturn7,
            LoginReturn14,
            LoginReturn30,
        }

        public static void TrackEvent(TrackEventType eventType)
        {
            Logger.Log($"Adjust track event.{eventType}");
            
            var eventToken = string.Empty;
            switch (eventType)
            {
                case TrackEventType.UserCreate:
                    eventToken = "7780k9";
                    break;
                case TrackEventType.ClearTutorialPatternA:
                    eventToken = "jpfk0k";
                    break;
                case TrackEventType.ClearTutorialPatternB:
                    eventToken = "fioyyt";
                    break;
                case TrackEventType.JoinGuild:
                    eventToken = "wacxxq";
                    break;
                case TrackEventType.FirstPurchase:
                    eventToken = "9dwymj";
                    break;
                case TrackEventType.LoginDay1:
                    eventToken = "2pzmi2";
                    break;
                case TrackEventType.LoginDay3:
                    eventToken = "1ptu9n";
                    break;
                case TrackEventType.LoginDay7:
                    eventToken = "n1rgqh";
                    break;
                case TrackEventType.LoginDay14:
                    eventToken = "1g3ve6";
                    break;
                case TrackEventType.LoginDay30:
                    eventToken = "rmtxug";
                    break;
                case TrackEventType.LoginReturn7:
                    eventToken = "vg0nd6";
                    break;
                case TrackEventType.LoginReturn14:
                    eventToken = "p54bic";
                    break;
                case TrackEventType.LoginReturn30:
                    eventToken = "joslnl";
                    break;
                default:
                    Logger.LogError("Invalid adjust event type.");
                    return;
            }

            var adjustEvent = new AdjustEvent(eventToken);
            Adjust.trackEvent(adjustEvent);
        }
        
        public static void TrackPurchaseEvent(string productKey, string receiptId = "")
        {
            Logger.Log($"Adjust track event purchase.{productKey}");
            
            var eventToken = string.Empty;
            var amountToken = string.Empty;
            var price = 0;
            var isOver1000 = false;
            switch (productKey)
            {
                case "bluelock_pwc.product21":
                    eventToken = "i4urvl";
                    amountToken = "frrkb5";
                    price = 160;
                    break;
                case "bluelock_pwc.product22":
                    eventToken = "krp0eu";
                    amountToken = "87eqog";
                    price = 320;
                    break;
                case "bluelock_pwc.product23":
                    eventToken = "odj02x";
                    amountToken = "glg8ie";
                    price = 480;
                    break;
                case "bluelock_pwc.product25":
                    eventToken = "khtrw2";
                    amountToken = "auknu1";
                    price = 800;
                    break;
                case "bluelock_pwc.product28":
                    eventToken = "w0xizb";
                    amountToken = "i3cjzn";
                    isOver1000 = true;
                    price = 1200;
                    break;
                case "bluelock_pwc.product40":
                    eventToken = "4k6zos";
                    amountToken = "fvb56v";
                    isOver1000 = true;
                    price = 3200;
                    break;
                case "bluelock_pwc.product51":
                    eventToken = "wlv21k";
                    amountToken = "h04t72";
                    isOver1000 = true;
                    price = 4900;
                    break;
                case "bluelock_pwc.product73":
                    eventToken = "e07mmd";
                    amountToken = "btfpz7";
                    isOver1000 = true;
                    price = 10000;
                    break;
                case "bluelock_pwc.product75":
                    eventToken = "jbpepo";
                    amountToken = "h49d4e";
                    isOver1000 = true;
                    price = 11800;
                    break;
                case "bluelock_pwc.product80":
                    eventToken = "ibanh3";
                    amountToken = "kcckz7";
                    isOver1000 = true;
                    price = 15800;
                    break;
                default:
                    Logger.LogError("Adjust track revenue: Invalid product key.");
                    return;
            }
            
            // 念の為, 重複計測を避けるためにレシートのId+トークンとかでトランザクションIdセットしといたほうがいいと思う.
            // AdjustEvent.setTransactionId("レシートIDかなんか.");

            // 課金毎のイベント
            var everyPurchaseEvent = new AdjustEvent("5oy0wu");
            Adjust.trackEvent(everyPurchaseEvent);

            // 1000円以上課金
            if (isOver1000)
            {
                var over730YenEvent = new AdjustEvent("4eijjk");
                Adjust.trackEvent(over730YenEvent);
            }

            // 価格毎の課金イベント
            var purchaseEvent = new AdjustEvent(eventToken);
            Adjust.trackEvent(purchaseEvent);
            
            // 価格毎の合計額更新イベント
            var amountEvent = new AdjustEvent(amountToken);
            amountEvent.setRevenue(price, "JPY");
            Adjust.trackEvent(amountEvent);
            
        }

        private static DateTime ParseDateStringToDateTime(string dateString)
        {
            var dateTime = DateTime.MinValue;
            DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out dateTime);
            // mhsは考慮しない
            dateTime = dateTime.AddHours(-dateTime.Hour);
            dateTime = dateTime.AddMinutes(-dateTime.Minute);
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            return dateTime;
        }
        
        public static void TrackLoginEvent(string registerDateString, string lastLoginDateString)
        {
            // Dgsm準拠でログイン日数と復帰日数イベント、ロジックはAdjustManager内で完結させる
            var now = AppTime.Now;
            var dateTime = ParseDateStringToDateTime(registerDateString);
            
            if (dateTime != DateTime.MinValue)
            {
                var adjustEvent = TrackEventType.None;
                var elapsedDateFromRegistered = (now - dateTime).TotalDays;
                Logger.Log($"registerDate: {registerDateString} / {dateTime.ToString()} / 経過日数:{elapsedDateFromRegistered}");
                if (elapsedDateFromRegistered >= 30)
                {
                    adjustEvent = TrackEventType.LoginDay30;
                }else if (elapsedDateFromRegistered >= 14)
                {
                    adjustEvent = TrackEventType.LoginDay14;
                }else if (elapsedDateFromRegistered >= 7)
                {
                    adjustEvent = TrackEventType.LoginDay7;
                }else if (elapsedDateFromRegistered >= 3)
                {
                    adjustEvent = TrackEventType.LoginDay3;
                }else if (elapsedDateFromRegistered >= 1)
                {
                    adjustEvent = TrackEventType.LoginDay1;
                }
                if (adjustEvent != TrackEventType.None)
                {
                    TrackEvent(adjustEvent);
                }
            }

            var lastLoginDate = ParseDateStringToDateTime(lastLoginDateString);
            
            if (lastLoginDate != DateTime.MinValue)
            {
                var adjustEvent = TrackEventType.None;
                var elapsedDateFromLastLogin = (now - lastLoginDate).TotalDays;
                Logger.Log($"lastLoginDate {lastLoginDateString} / {lastLoginDate.ToString()} / 復帰日数:{elapsedDateFromLastLogin}");
                if (elapsedDateFromLastLogin >= 30)
                {
                    adjustEvent = TrackEventType.LoginReturn30;
                }else if (elapsedDateFromLastLogin >= 14)
                {
                    adjustEvent = TrackEventType.LoginReturn14;
                }else if (elapsedDateFromLastLogin >= 7)
                {
                    adjustEvent = TrackEventType.LoginReturn7;
                }
                if (adjustEvent != TrackEventType.None)
                {
                    TrackEvent(adjustEvent);
                }
            }

        }
    }
}