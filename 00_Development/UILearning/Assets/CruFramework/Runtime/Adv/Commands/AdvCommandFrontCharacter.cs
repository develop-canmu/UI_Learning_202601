using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandFrontCharacter : IAdvCommand
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.CharacterDatas), AdvObjectIdAttribute.WindowType.SearchWindow)]
        public int[] ids = null;
        
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=ids.Length-1;i>=0;i--)
            {
                AdvCharacter c = manager.GetAdvCharacter<AdvCharacter>(ids[i]);
                c.ToFront();
            }
        }
    }
}