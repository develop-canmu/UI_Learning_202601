using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.Adv
{
    public abstract class AdvTextObject : MonoBehaviour
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Texts))]
        private int id = 0;
        /// <summary>Id</summary>
        public int Id{get{return id;}}
        
        /// <summary>文字列の設定</summary>
        public  abstract void SetText(string text, Color color);
    }
}