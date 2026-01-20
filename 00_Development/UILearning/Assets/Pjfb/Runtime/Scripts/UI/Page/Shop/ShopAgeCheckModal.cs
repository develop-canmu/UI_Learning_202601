using System;
using System.Globalization;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Shop
{
    public class ShopAgeCheckModal : ModalWindow
    {
        private const int MIN_BIRTHDAY_YEAR = 1900;
        
        public class Data
        {
            public Action OnUpdateUi;

            public Data(Action onUpdateUi)
            {
                OnUpdateUi = onUpdateUi;
            }
        }
        
        [SerializeField] private TMP_InputField yearInput;
        [SerializeField] private TMP_InputField monthInput;
        [SerializeField] private TMP_InputField dayInput;

        private Data modalData;
        private DateTime confirmBirthDay;

        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopAgeCheck, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            return base.OnPreOpen(args, token);
        }

        public void OnClickRegisterButton()
        {
            confirmBirthDay = GetInputBirthday();
            if (confirmBirthDay == DateTime.MinValue || confirmBirthDay.Year < MIN_BIRTHDAY_YEAR)
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["shop.age.title"], 
                    StringValueAssetLoader.Instance["shop.age.error"], 
                    string.Empty, 
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window =>
                    { 
                        window.Close();
                    })));
            }
            else
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["shop.age.title"], 
                    CreateConfirmString(confirmBirthDay), 
                    string.Empty, 
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], window =>
                    {
                        RequestUpdatePaymentLimitAsync().Forget();
                    }),
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window =>
                    { 
                        window.Close();
                    })));
            }
        }
        
        private async UniTask RequestUpdatePaymentLimitAsync()
        {
            try
            {
                var request = new ShopUpdatePaymentLimitAPIRequest();
                var post = new ShopUpdatePaymentLimitAPIPost();
                post.birthday = confirmBirthDay.ToString("yyyyMMdd");
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                var response = request.GetResponseData();
                UserDataManager.Instance.user.UpdateMonthPaymentData(response.monthPayment,response.monthPaymentLimit, response.hasParentalConsent);
                var data = new ConfirmModalData();
                data.Title = StringValueAssetLoader.Instance["shop.age.title"];
                data.Message = StringValueAssetLoader.Instance["shop.age.success"];
                data.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.close"], 
                    window => 
                    {
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window.GetType() != typeof(ShopPackModalWindow));
                        window.Close();
                        modalData.OnUpdateUi?.Invoke();
                    }
                );
                ConfirmModalWindow.Open(data);
            }
            catch (APIException)
            {
                // Apiエラーでタイトルへ戻った際に確認モーダルを閉じるとエラーになるので更新用の処理を行わないようにnullを代入する
                modalData.OnUpdateUi = null;
                // Apiエラーだった場合はログを出して後の処理を行わない
                CruFramework.Logger.LogError("ShopUpdatePaymentLimitAPI error");
            }
        }
        
        private string CreateConfirmString(DateTime birthday)
        {
            var confirmText = StringValueAssetLoader.Instance["shop.age.confirm"];
            var currentDate = AppTime.Now;
            var yearString = StringValueAssetLoader.Instance["shop.age.year"];
            var monthString = StringValueAssetLoader.Instance["shop.age.month"];
            var dayString = StringValueAssetLoader.Instance["shop.age.day"];
            confirmText += "<br><br>";
            confirmText += $"{birthday.Year}{yearString}　{birthday.Month}{monthString}　{birthday.Day}{dayString}<br>";
            
            var fixedAge = currentDate.Year - birthday.Year;
            if (currentDate.Month < birthday.Month ||
                currentDate.Month == birthday.Month && currentDate.Day < birthday.Day)
            {
                fixedAge--;
            }

            var limitAgeText = "";
            var limitValueText = "";
            var limitAnnotation = StringValueAssetLoader.Instance["shop.age.register.annotation"];
            
            if (fixedAge >= 18)
            {
                limitAgeText = StringValueAssetLoader.Instance["shop.age.older18"];
                limitValueText = StringValueAssetLoader.Instance["shop.age.older18.value"];
            }
            else if (fixedAge >= 16)
            {
                limitAgeText = StringValueAssetLoader.Instance["shop.age.under18"];
                limitValueText = StringValueAssetLoader.Instance["shop.age.under18.value"];
            }
            else
            {
                limitAgeText = StringValueAssetLoader.Instance["shop.age.under16"];
                limitValueText = StringValueAssetLoader.Instance["shop.age.under16.value"];
            }

            confirmText += $"{limitAgeText}　{limitValueText}<br><br>";
            confirmText += $"<color=red><size=26>{limitAnnotation}</size></color>";

            return confirmText;
        }
        
        private DateTime GetInputBirthday()
        {
            if ( string.IsNullOrEmpty(yearInput.text) || string.IsNullOrEmpty(monthInput.text) || string.IsNullOrEmpty(dayInput.text))
            {
                return DateTime.MinValue;
            }
            var datetime = DateTime.MinValue;
            DateTime.TryParseExact($"{yearInput.text}{monthInput.text.PadLeft(2, '0')}{dayInput.text.PadLeft(2, '0')}", "yyyyMMdd", null, DateTimeStyles.None, out datetime);
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            if (datetime > AppTime.Now)
            {
                return DateTime.MinValue;
            }
            return datetime;
        }
        
    }
}