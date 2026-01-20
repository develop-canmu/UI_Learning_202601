using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandDeleteObject : IAdvCommand
    {
        [SerializeField]
        [AdvDocument("削除するオブジェクトId。複数指定可能。")]
        private ulong[] ids = null;
        
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<ids.Length;i++)
            {
                manager.DeleteObject(ids[i]);
            }
        }
    }
}