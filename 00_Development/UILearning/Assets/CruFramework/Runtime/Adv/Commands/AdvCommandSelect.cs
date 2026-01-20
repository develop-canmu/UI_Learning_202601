using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
    
    public enum AdvCommandSelectSkipMode
    {
        SkipIfOne = 0,
        CantSkip  = 1,
        Always    = 2
    }

    [System.Serializable]
    public class AdvCommandSelect : IAdvCommand, IAdvFastForward, IAdvCommandSkipBreak, IAdvCommandSelect
    {
    
        [SerializeField]
        private AdvCommandSelectSkipMode skipMode = AdvCommandSelectSkipMode.SkipIfOne;
    
        [SerializeField]
        private string[] messages = null;
        
        
        int IAdvCommandSelect.GetSelectCount()
        {
            return messages == null ? 0 : messages.Length;
        }
        
        private void AutoSelect(AdvManager manager)
        {
            switch(skipMode)
            {
                case AdvCommandSelectSkipMode.SkipIfOne:
                {
                    if(messages.Length == 1)
                    {
                        manager.SelectRoot.Select(1);
                    }
                    break;
                }
                
                case AdvCommandSelectSkipMode.Always:
                    manager.SelectRoot.Select(1);
                    break;
            }
        }
        
        private bool CanSkip(AdvManager manager)
        {
            switch(skipMode)
            {
                case AdvCommandSelectSkipMode.SkipIfOne:
                {
                    if(messages.Length == 1)
                    {
                        return true;
                    }
                    
                    return false;
                }
                
                case AdvCommandSelectSkipMode.Always:
                    return true;
                
                case AdvCommandSelectSkipMode.CantSkip:
                    return false;
                
            }
         
            return true;
        }
        
        bool IAdvFastForward.OnNext(AdvManager manager)
        {
            return CanSkip(manager);
        }
        
        bool IAdvCommandSkipBreak.IsSkipBreak(AdvManager manager)
        {
            return CanSkip(manager) == false;
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            ExecuteAsync(manager).Forget();
        }
        
        private async UniTask ExecuteAsync(AdvManager manager)
        {
            if(messages.Length <= 0)
            {
                manager.ErrorLog("メッセージの設定が0個です");
                return;
            }
            
            await manager.SelectRoot.OpenAsync(manager, messages, null);
            
            if(manager.IsSkip || manager.IsFastMode)
            {
                AutoSelect(manager);
            }
        }
    }
}