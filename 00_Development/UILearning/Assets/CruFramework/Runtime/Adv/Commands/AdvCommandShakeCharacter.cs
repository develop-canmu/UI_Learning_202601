using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandShakeCharacter : AdvCommandShake
    {
        [SerializeField]
        [AdvDocument("揺らす対象。")]
        [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
        public int id = 0;

        protected override Transform GetTargetTransform(AdvManager manager)
        {
            AdvCharacter c = manager.GetAdvCharacter<AdvCharacter>(id);
            return c.ShakeTarget;
        }
    }
}