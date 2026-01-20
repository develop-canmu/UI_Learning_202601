using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace CruFramework.Adv
{
    public abstract class AdvFade : MonoBehaviour
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Fades))]
        private int id = 0;
        /// <summary>Id</summary>
        public int Id{get{return id;}}
        
        
        public abstract void ForceComplete();
    }
}