using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CruFramework.Adv
{

    public abstract class AdvUICharacter : AdvCharacter
    {
        
        public RectTransform UITransform{get{return (RectTransform)transform;}}
        
        public override void ToFront()
        {
            UITransform.SetAsLastSibling();
        }
    }
}
