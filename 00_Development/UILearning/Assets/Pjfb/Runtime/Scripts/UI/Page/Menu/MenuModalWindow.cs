using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Community;
using Pjfb.Home;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;

namespace Pjfb.Menu
{
    public class MenuModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public Action onClosed;
        }

        #endregion
        
        [SerializeField] private GameObject profileBadge;
        private WindowParams _windowParams;
        private HomeGetDataAPIResponse currentHomeDataResponse;
        
        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Menu, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            Init();
            base.OnOpened();
        }

        private void Init()
        {
            profileBadge.SetActive(MenuManager.IsTrainerCardBadge());
        }
        #endregion
        

        #region PrivateMethods
       
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            //ホーム画面のコミュニティバッジ更新
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Home)
            {
                var homeTop = AppManager.Instance.UIManager.PageManager.CurrentPageObject.GetComponentInChildren<HomeTopPage>();
                if (homeTop != null)
                {
                    homeTop.UpdateCommunityBadge();
                    homeTop.UpdateRecommendCharaBadge();
                    homeTop.UpdateRankingBadge();
                }
            }

            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickProfileButton()
        {
            TrainerCardModalWindow.WindowParams param;
            switch (AppManager.Instance.UIManager.PageManager.CurrentPageType)
            {
                case PageType.Club:
                    param = new TrainerCardModalWindow.WindowParams(UserDataManager.Instance.user.uMasterId, onDissolution:ClubUtility.ChangeFindClubPage, onSecession:ClubUtility.ChangeFindClubPage);
                    break;
                default:
                    param = new TrainerCardModalWindow.WindowParams(UserDataManager.Instance.user.uMasterId);
                    break;
            }
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        
        public void OnClickInquiryButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Inquiry,null);
        }
        
        public void OnClickItemButton()
        {
            ItemListModalWindow.Open(new ItemListModalWindow.WindowParams());
        }

        public void OnClickTermsButton()
        {
            TermsModal.OpenAsync(TermsModal.DisplayType.Confirm,gameObject.GetCancellationTokenOnDestroy()).Forget();
        }
        
        public void OnClickConfigurationButton()
        {
            ConfigurationModalWindow.Open(new ConfigurationModalWindow.WindowParams());
        }
        
        public void OnClickUserTransferSettingButton()
        {
            UserTransferSettingModalWindow.Open(new UserTransferSettingModalWindow.WindowParams());
        }
        
        public void OnClickHelpButton()
        {
            HelpModalWindow.Open(new HelpModalWindow.WindowParams());
        }

        public void OnClickLicenseButton()
        {
            ShowLicenseWindow().Forget();
        }

        public static void OnClickBackTitleButton()
        {
            string title = StringValueAssetLoader.Instance["common.confirm"];
            string message = StringValueAssetLoader.Instance["menu.back_title_confirm"];
            string positiveButtonText = StringValueAssetLoader.Instance["common.ok"];
            string negativeButtonText = StringValueAssetLoader.Instance["common.cancel"];
            ConfirmModalWindow.Open(new ConfirmModalData
            (
                title,
                message,
                string.Empty, 
                new ConfirmModalButtonParams(positiveButtonText, confirmWindow =>
                {
                    //”はい”ボタンの押す処理
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                    confirmWindow.Close(() =>
                    {
                        AppManager.Instance.BackToTitle();
                    });
                }),new ConfirmModalButtonParams(negativeButtonText, window =>
                {
                    //”いいえ”ボタンの押す処理
                    window.Close();
                })
            ));
        }
        #endregion
       
        #region API

        private async UniTask<HomeGetDataAPIResponse> HomeGetDataAPI()
        {
            HomeGetDataAPIRequest request = new HomeGetDataAPIRequest();
            HomeGetDataAPIPost post = new HomeGetDataAPIPost();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }

        private async UniTask ShowLicenseWindow()
        {
            // API
            TermsGetLicenceAPIRequest request = new TermsGetLicenceAPIRequest();
            await APIManager.Instance.Connect(request);
            TermsGetLicenceAPIResponse response = request.GetResponseData();
            
            ArticleModalWindow.Open(new ArticleModalWindow.WindowParams
            {
                titleText = StringValueAssetLoader.Instance["menu.license"],
                bodyText = response.licence
            });
        }

        #endregion
        
    }
}
