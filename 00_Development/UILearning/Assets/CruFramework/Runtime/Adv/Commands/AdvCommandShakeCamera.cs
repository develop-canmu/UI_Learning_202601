using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandShakeCamera : AdvCommandShake
    {
        protected override Transform GetTargetTransform(AdvManager manager)
        {
            return manager.World.transform;
        }
    }
}