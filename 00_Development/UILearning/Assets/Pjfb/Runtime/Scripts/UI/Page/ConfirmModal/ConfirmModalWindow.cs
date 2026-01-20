using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    public class ConfirmModalWindow : ModalWindow
    {
        public static void Open(ConfirmModalData data, ModalOptions modalOptions = ModalOptions.None)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data, modalOptions);
        }
        public static async UniTask<CruFramework.Page.ModalWindow> OpenAsync(ConfirmModalData data, CancellationToken token)
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data, token);
        }
        
        [SerializeField]
        private TextMeshProUGUI titleText = null;

        [SerializeField]
        private TextMeshProUGUI messageText = null;
        
        [SerializeField]
        private TextMeshProUGUI cautionText = null;

        [SerializeField]
        private TextMeshProUGUI positiveButtonText = null;
        
        [SerializeField]
        private TextMeshProUGUI redPositiveButtonText = null;
        
        [SerializeField]
        private TextMeshProUGUI negativeButtonText = null;
        
        [SerializeField]
        protected UIButton positiveButton = null;
        
        [SerializeField]
        protected UIButton redPositiveButton = null;

        [SerializeField]
        private UIButton negativeButton = null;

        [SerializeField] private GameObject cautionRoot;

        private Action<ModalWindow> positiveButtonCallback = null;
        private Action<ModalWindow> negativeButtonCallback = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            if(args == null)
            {
                Logger.LogError("ConfirmModalWindowのargsがnullです");
            }
            
            // データセット
            ConfirmModalData data = (ConfirmModalData)args;
            // タイトル
            titleText.text = data.Title;
            // メッセージ
            messageText.text = data.Message;
            // 注意
            cautionText.text = data.Caution;
            if(string.IsNullOrEmpty(cautionText.text)) cautionRoot.gameObject.SetActive(false); 
            
            if(data.PositiveButtonParams != null)
            {
                // オレンジのボタン
                if (!data.PositiveButtonParams.isRed)
                {
                    positiveButton.gameObject.SetActive(true);
                    redPositiveButton.gameObject.SetActive(false);
                    positiveButtonText.text = data.PositiveButtonParams.text;
                }
                // レッドのボタン
                else
                {
                    positiveButton.gameObject.SetActive(false);
                    redPositiveButton.gameObject.SetActive(true);
                    redPositiveButtonText.text = data.PositiveButtonParams.text;
                }
                positiveButtonCallback = data.PositiveButtonParams.onClick;
            }
            else
            {
                positiveButton.gameObject.SetActive(false);
                redPositiveButton.gameObject.SetActive(false);
            }
            
            // 白いボタン
            if(data.NegativeButtonParams != null)
            {
                negativeButtonText.gameObject.SetActive(true);
                negativeButtonText.text = data.NegativeButtonParams.text;
                negativeButtonCallback = data.NegativeButtonParams.onClick;
                negativeButton.SoundType = data.PositiveButtonParams == null ? SE.se_common_tap : SE.se_common_cancel;
            }
            else
            {
                negativeButton.gameObject.SetActive(false);
            }
            DisableBackKey = data.DisableBackKey;

            return base.OnPreOpen(args, token);
        }

        /// <summary>uGUI</summary>
        public void OnClickFirstButton()
        {
            positiveButtonCallback?.Invoke(this);
        }
        
        /// <summary>uGUI</summary>
        public void OnClickSecondButton()
        {
            negativeButtonCallback?.Invoke(this);
        }
    }
}
