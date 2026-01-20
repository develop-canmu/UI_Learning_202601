using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Utility;
using UnityEngine;

namespace Pjfb.CampaignBanner
{
    public class CampaignBannerModalWindow : ModalWindow
    {
        #region Params
        public class BannerData
        {
            public string imagePath = string.Empty;
            public string onClick = string.Empty;
            public string endAt = string.Empty;
            public long masterId = 0;
            
            public BannerData(NewsPopup newsPopup)
            {
                imagePath = newsPopup.imagePath;
                onClick = newsPopup.onClick;
                endAt = newsPopup.endAt;
            }
        }
        public class WindowParams
        {
            public List<BannerData> popupDataList;
            public Action<BannerData> onShownPopup;
            public Action<WindowParams> onComplete;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private Animator animator;
        [SerializeField] private CancellableWebTexture campaignBannerImage;
        [SerializeField] private CanvasGroup canvasGroup;
        #endregion

        #region PrivateFields
        private WindowParams _windowParams;
        private BannerData nullableShowingPopup;
        private List<BannerData> _showingPopupDataList;

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        private const string DebugMenuKey = "キャンペーンバナー";
#endif
        #endregion

        #region StaticMethods
        public static void Open(WindowParams windowParams)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CampaignBanner, args: windowParams);
        }
        #endregion
        
        #region OverrideMethods
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Init((WindowParams) args);
            return base.OnPreOpen(args, token);
        }

        protected override UniTask OnOpen(CancellationToken token)
        {
            OnOpened();
            return UniTask.CompletedTask;
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            canvasGroup.alpha = 0;
            ShowNextPopupOrClose();
            
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            CruFramework.DebugMenu.AddOption(DebugMenuKey,"スキップ", DebugSkipAllBanner);
#endif
        }
        #endregion

        #region PrivateMethods
        private void ShowNextPopupOrClose()
        {
            if (_showingPopupDataList.Any())
            {
                canvasGroup.alpha = 1;
                var showingPopupData = _showingPopupDataList[0];
                _showingPopupDataList.RemoveAt(0);
                ShowBannerAnimation(showingPopupData);
            }
            else
            {
                Close(onCompleted: ModalClose);
            }
        }

        private void ModalClose()
        {
            _windowParams?.onComplete?.Invoke(_windowParams);
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
            // デバッグメニューの削除
            CruFramework.DebugMenu.RemoveOption(DebugMenuKey);
#endif            
        }

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        // デバッグ用：全キャンペーンバナーを既読扱いにして閉じる
        private void DebugSkipAllBanner()
        {
            foreach (var showData in _showingPopupDataList)
            {
                // 既読処理
                _windowParams?.onShownPopup?.Invoke(showData);
            }
            
            Close(ModalClose);
        }
#endif
        
        private async void ShowBannerAnimation(BannerData popupData)
        {
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            nullableShowingPopup = popupData;

            await SetBanner(popupData.imagePath);
            
            animator.SetTrigger("Open");
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            _windowParams?.onShownPopup?.Invoke(popupData);
        }
        
        private void Init(WindowParams windowParams)
        {
            _windowParams = windowParams;
            _showingPopupDataList = _windowParams.popupDataList.ToList();
            nullableShowingPopup = null;
        }

        private async UniTask SetBanner(string imagePath)
        {
            await campaignBannerImage.SetTextureAsync($"{AppEnvironment.AssetBrowserURL}/{imagePath}");
        }
        #endregion
        
        #region EventListeners
        public void OnClickClose()
        {
            ShowNextPopupOrClose();
        }

        public void OnClickShowDetailButton()
        {
            if (nullableShowingPopup != null)
            {
                ServerActionCommandUtility.ProceedAction(nullableShowingPopup.onClick);    
            }
        }
        #endregion
    }
}