
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
    public class AdvCommandTutorial : IAdvCommand, IAdvCommandSkipBreak
    {
    
        public enum TutorialType
        {
            InitializeUserData
        }
        
        [SerializeField]
        [AdvDocument("実行するチュートリアル")]
        private TutorialType type = TutorialType.InitializeUserData;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            ExecuteAsync( (AppAdvManager)manager).Forget();
        }
        
        bool IAdvCommandSkipBreak.IsSkipBreak(AdvManager manager)
        {
            return true;
        }
        
        private async UniTask ExecuteAsync(AppAdvManager manager)
        {

#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
            if(manager.IsDebugMode)
            {
                await manager.OpenDebugModalAsync(this, "チュートリアル ユーザー名入力");
                return;
            }
#endif
            
            // コマンドを停止
            manager.IsStopCommand = true;
            
            switch(type)
            {
                case TutorialType.InitializeUserData:
                {
                    // ユーザー初期化
                    await AppManager.Instance.TutorialManager.InitializeUserDataAsync();
                    // ユーザー名や一人称に変更がかかるので置換を更新
                    manager.UpdateReplaceText();
                    break;
                }
            }
            
            // コマンドを再開
            manager.IsStopCommand = false;
        }
    }
}
