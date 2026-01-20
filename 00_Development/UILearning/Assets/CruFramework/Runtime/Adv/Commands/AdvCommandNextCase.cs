using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public class AdvCommandNextCase : IAdvCommandObject, IAdvCommandLocationIndex
    {
        [SerializeField]
        private ulong locationId = 0;
        /// <summary>位置</summary>
        public ulong LocationId{get{return locationId;}}
        
        [SerializeField]
        private int commandIndex = -1;
        /// <summary>Index</summary>
        public int CommandIndex{get{return commandIndex;}}
        
        
        public AdvCommandNextCase(ulong id)
        {
            locationId = id;
        }
        
        void IAdvCommandLocationIndex.SetCommandIndex(int index)
        {
            commandIndex = index;
        }
    }
}