using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    [System.Serializable]
    public class AdvCommandStopSe : IAdvCommand
    { 
        
        [SerializeField]
        [AdvDocument("停止するSE。")]
        private string id = string.Empty;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            manager.StopSe(manager.Config.SeResourcePathId, id);
        }
    }
}