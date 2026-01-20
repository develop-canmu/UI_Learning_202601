using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandWaitClick : IAdvCommand
    {
        [SerializeField]
        private  bool isEnable = true;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            if(isEnable)
            {
                manager.WaitClick();
            }
        }
    }
}
