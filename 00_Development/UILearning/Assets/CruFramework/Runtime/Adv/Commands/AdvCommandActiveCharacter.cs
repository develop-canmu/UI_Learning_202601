using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandActiveCharacter : IAdvCommand
    {
        
        [System.Serializable]
        private class ActiveData
        {
            [SerializeField]
            [AdvDocument("表示フラグ。")]
            public bool isActive = true;
            
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
            [AdvDocument("オブジェクトId。複数指定可能。")]
            public int id = 0;
        }
        
        [SerializeField]
        [AdvDocument("アクティブを切り替えるオブジェクトデータ。複数指定可能。")]
        private ActiveData[] activeDatas = null;


        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<activeDatas.Length;i++)
            {
                manager.ShowCharacter(activeDatas[i].id , activeDatas[i].isActive);
            }
        }
    }
}