using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvPositionId : IAdvObjectId
    {
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        [SerializeField]
        private Vector3 position = Vector3.zero;
        /// <summary>座標</summary>
        public Vector3 Position{get{return position;}set{position = value;}}
        
        string IAdvObjectId.GetString(){return name;}
    }

    [System.Serializable]
    public class AdvPositionIds : AdvObjectIds<AdvPositionId>
    {

    }
}