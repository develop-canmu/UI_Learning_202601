using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandActiveMessageWindow : IAdvCommand
    {
        
        [System.Serializable]
        private class ActiveData
        {
            [SerializeField]
            [AdvDocument("表示フラグ。")]
            public bool isActive = true;
            
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.MessageWindows))]
            [AdvDocument("メッセージウィンドウId。複数指定可能。")]
            public int id = 0;
        }
        
        [SerializeField]
        [AdvDocument("アクシティを切り替えるメッセージウィンドウ。複数指定可能。")]
        private ActiveData[] activeDatas = null;


        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<activeDatas.Length;i++)
            {
                manager.GetMessageWindow(activeDatas[i].id).gameObject.SetActive(activeDatas[i].isActive);
            }
        }
    }
}