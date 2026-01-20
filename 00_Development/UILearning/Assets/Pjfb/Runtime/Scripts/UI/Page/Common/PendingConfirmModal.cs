using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using System;

namespace Pjfb
{
    public class PendingConfirmModal : ModalWindow
    {
        public class Arguments
        {
            private string title = string.Empty;
            /// <summary>タイトル</summary>
            public string Title{get{return title;}}
        
            private string message = string.Empty;
            /// <summary>メッセージ</summary>
            public string Message{get{return message;}}
            
            private Action onContinue = null;
            /// <summary>続行</summary>
            public Action OnContinue{get{return onContinue;}}
            
            private Action onDelete = null;
            /// <summary>削除</summary>
            public Action OnDelete{get{return onDelete;}}
            
            private Action onAutoTrainingButton = null;
            /// <summary>自動トレーニングボタン</summary>
            public Action OnAutoTrainingButton{get{return onAutoTrainingButton;}}
            
            public Arguments(string title, string message, Action onContinue, Action onDelete, Action onAutoTrainingButton)
            {
                this.title = title;
                this.message = message;
                this.onContinue = onContinue;
                this.onDelete = onDelete;
                this.onAutoTrainingButton = onAutoTrainingButton;
            }
        }
        
        [SerializeField]
        private TMPro.TMP_Text titleText = null;
        [SerializeField]
        private TMPro.TMP_Text messageText = null;

        [SerializeField]
        private GameObject gotoAutoTrainingButton = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            titleText.text = arguments.Title;
            messageText.text = arguments.Message;
            
            gotoAutoTrainingButton.SetActive( arguments.OnAutoTrainingButton != null );

            return base.OnPreOpen(args, token);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnGotoAutoTraining()
        {
            // 閉じる
            Close();
            Arguments arguments = (Arguments)ModalArguments;
            if(arguments.OnAutoTrainingButton != null)
            {
                arguments.OnAutoTrainingButton();
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnOkButton()
        {
            // 閉じる
            Close();
            Arguments arguments = (Arguments)ModalArguments;
            
            if(arguments.OnContinue != null)
            {
                arguments.OnContinue();
            }
        }
        
        /// <summary>UGUI</summary>
        public void OnDeleteButton()
        {
            // 確認モーダル
            ConfirmModalButtonParams button2 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], async (modal)=> 
            {
                // モーダルを閉じる
                await modal.CloseAsync();
            });
                
            ConfirmModalButtonParams button1 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.pending_delete"], async (modal)=> 
            {
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
                await modal.CloseAsync();
                Arguments arguments = (Arguments)ModalArguments;
                if(arguments.OnDelete != null)
                {
                    arguments.OnDelete();
                }
            });
            
            // 確認モーダルを出す
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.pending_modal.title"],
                StringValueAssetLoader.Instance["common.pending_modal.message1"],
                string.Empty,
                button1,
                button2,
                false
            );
            
            AppManager.Instance.UIManager.ModalManager.OpenModal( ModalType.Confirm, data );
        }
    }
}