
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandOpenModal : IAdvCommand
    {
        [SerializeField]
        [AdvDocument("開くモーダル。")]
        private ModalType modalType;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            ExecuteAsync( (AppAdvManager)manager).Forget();
        }
        
        private async UniTask ExecuteAsync(AppAdvManager manager)
        {
            // コマンドを停止
            manager.IsStopCommand = true;
            // 指定のモーダルを開く
            CruFramework.Page.ModalWindow modalWindow = await manager.ModalManager.OpenModalAsync(modalType, null);
            // モーダルが閉じるまで待つ
            await modalWindow.WaitCloseAsync();            
            // コマンドを再開
            manager.IsStopCommand = false;
        }
    }
}
