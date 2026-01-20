using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    [System.Serializable]
    public class AdvCommandStopBgm : IAdvCommand
    { 
    
        [SerializeField][HideInInspector]
        private bool isEnable = true;
        public bool IsEnable{get{return isEnable;}}
    
        void IAdvCommand.Execute(AdvManager manager)
        {
            manager.StopBgm();
        }
    }
}