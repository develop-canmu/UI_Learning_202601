using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandDeleteCharacter : IAdvCommand
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
        [AdvDocument("削除するキャラクタId。複数指定可能。")]
        private int[] ids = null;
        /// <summary>Id</summary>
        public int[] Ids{get{return ids;}}
        
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<ids.Length;i++)
            {
                manager.DeleteCharacter(ids[i]);
            }
        }
    }
}