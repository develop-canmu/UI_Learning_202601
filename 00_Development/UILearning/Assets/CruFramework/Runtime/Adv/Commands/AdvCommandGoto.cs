using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandGoto : IAdvCommand, IAdvCommandLocationIndex
    {
        [SerializeField]
        [AdvDocument("LocationコマンドのId。")]
        private ulong locationId = 0;
        /// <summary>移動先</summary>
        public ulong LocationId{get{return locationId;}}
        
        [SerializeField][HideInInspector]
        private int commandIndex = -1;
        /// <summary>Index</summary>
        public int CommandIndex{get{return commandIndex;}}
        
        
        public AdvCommandGoto(){}
        
        public AdvCommandGoto(ulong id)
        {
            locationId = id;
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            if(commandIndex >= 0)
            {
                manager.GotoIndex(commandIndex);
            }
            else
            {
                manager.Goto(locationId);
            }
        }
        
        
        void IAdvCommandLocationIndex.SetCommandIndex(int index)
        {
            commandIndex = index;
        }
    }
}