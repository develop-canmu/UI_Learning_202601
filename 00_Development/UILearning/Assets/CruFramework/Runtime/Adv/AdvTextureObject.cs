using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public abstract class AdvTextureObject : MonoBehaviour
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Textures))]
        private int id = 0;
        /// <summary>Id</summary>
        public int Id{get{return id;}}
        
        public abstract void SetTexture(Texture texture);

    }
}