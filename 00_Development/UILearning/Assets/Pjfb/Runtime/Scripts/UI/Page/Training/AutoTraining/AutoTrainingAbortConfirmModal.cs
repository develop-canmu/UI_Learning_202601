using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingAbortConfirmModal : ModalWindow
    {
        
        public class Arguments
        {
            private long slot = 0;
            /// <summary>スロット番号</summary>
            public long Slot{get{return slot;}}
            
            private IAutoTrainingReloadable reloadable = null;
            /// <summary>リロードインターフェース</summary>
            public IAutoTrainingReloadable Reloadable{get{return reloadable;}}
            
            public Arguments(long slot, IAutoTrainingReloadable reloadable)
            {
                this.slot = slot;
                this.reloadable = reloadable;
            }
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            return base.OnPreOpen(args, token);
        }

        public void CloseConfirmModal()
        {
            Close();
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnAbortButton()
        {
            AbortAsync().Forget();
        }
        
        private async UniTask AbortAsync()
        {

            Arguments args = (Arguments)ModalArguments;
            
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(_=>true);
            
            // API
            TrainingAbortAutoAPIRequest request = new TrainingAbortAutoAPIRequest();
            // Post
            TrainingAbortAutoAPIPost post = new TrainingAbortAutoAPIPost();
            // スロット
            post.slotNumber = args.Slot;
            
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            
            // 再読み込み
            if(args.Reloadable != null)
            {
                await args.Reloadable.OnReloadAsync();
            }
            
            // 確認モーダル
            ConfirmModalData modalData = new ConfirmModalData();
            modalData.Title = StringValueAssetLoader.Instance["auto_training.abort_modal.title"];
            modalData.Message = StringValueAssetLoader.Instance["auto_training.abort_modal.execute_message"];
            modalData.NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], m=>
            {
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(_=>true);
                m.Close();
            });
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, modalData);
        }
    }
}