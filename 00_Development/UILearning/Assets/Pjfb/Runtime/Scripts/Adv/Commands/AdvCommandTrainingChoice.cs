
using System;
using System.Collections.Generic;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using UnityEngine;

namespace Pjfb.Adv
{
    [Serializable]
    public class AdvCommandTrainingChoice : IAdvCommand, IAdvFastForward, IAdvCommandSkipBreak, IAdvCommandSelect
    {
        [Serializable]
        private class TextData
        {
            [SerializeField]
            [AdvDocument("チョイスId。")]
            public int id = 0;
        
            [SerializeField]
            [AdvDocument("表示するテキスト。")]
            public string text = string.Empty;
        }
        
        [SerializeField]
        private AdvCommandSelectSkipMode skipMode = AdvCommandSelectSkipMode.SkipIfOne;

        [SerializeField]
        [AdvDocument("選択肢に表示するデータ。")]
        private TextData[] textDatas = null;
        
        private void AutoSelect(AdvManager manager)
        {
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugOverrideSkipMode();
#endif
            
            switch(skipMode)
            {
                case AdvCommandSelectSkipMode.SkipIfOne:
                {
                    if(textDatas.Length == 1)
                    {
                        manager.SelectRoot.Select(textDatas[0].id);
                    }
                    break;
                }
                
                case AdvCommandSelectSkipMode.Always:
                    manager.SelectRoot.Select(textDatas[0].id);
                    break;
            }
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugOverrideSkipMode()
        {
            if (TrainingChoiceDebugMenu.EnabledAutoChoiceAdv)
            {
                skipMode = AdvCommandSelectSkipMode.Always;
            }
        }
#endif
        
        int IAdvCommandSelect.GetSelectCount()
        {
            return textDatas.Length;
        }
        
        private bool CanSkip(AdvManager manager)
        {
            switch(skipMode)
            {
                case AdvCommandSelectSkipMode.SkipIfOne:
                {
                    if(textDatas.Length == 1)
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
            
#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
            if(manager.IsDebugMode)
            {
                List<string> debugList = new List<string>();
                List<int> debugNoList = new List<int>();
                foreach(TextData data in textDatas)
                {
                    debugNoList.Add(data.id);
                    debugList.Add(data.text);
                }
                await manager.SelectRoot.OpenAsync(manager, debugList.ToArray(), debugNoList.ToArray());
                if(manager.IsSkip || manager.IsFastMode)
                {
                    AutoSelect(manager);
                }
                return;
            }
#endif
            
            AppAdvManager m = (AppAdvManager)manager;
            List<string> list = new List<string>();
            List<int> noList = new List<int>();
            foreach(long id in m.ChoiceList)
            {
                foreach(TextData data in textDatas)
                {
                    if(data.id == id)
                    {
                        noList.Add(data.id);
                        list.Add(data.text);
                    }
                }
            }
            
            await manager.SelectRoot.OpenAsync(manager, list.ToArray(), noList.ToArray());
            if(manager.IsSkip || manager.IsFastMode)
            {
                AutoSelect(manager);
            }
        }
    }
}
