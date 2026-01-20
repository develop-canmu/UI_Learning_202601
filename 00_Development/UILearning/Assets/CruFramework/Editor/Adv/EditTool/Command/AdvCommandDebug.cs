using System.Collections;
using System.Collections.Generic;
using CruFramework.Adv;
using UnityEngine;

namespace CruFramework.Editor.Adv
{
    public class AdvCommandDebug : IAdvCommandObject
    {
        [SerializeField]
        private ulong nodeId = 0;
        /// <summary>NodeId</summary>
        public ulong NodeId{get{return nodeId;}}
        
        public AdvCommandDebug(ulong nodeId)
        {
            this.nodeId = nodeId;
        }
    }
}