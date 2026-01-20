using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public class AdvObjectLayer : MonoBehaviour
    {

        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.ObjectLayers))]
        private int id = 0;
        /// <summary>Id</summary>
        public int Id{get{return id;}}
    }
}