using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandColorFade : AdvCommandFade
    {
        [SerializeField]
        private Color color = Color.white;
        
        protected override void FadeIn(AdvManager manager, AdvFade fade)
        {
            if(fade is AdvColorFade f)
            {
                f.FadeIn(duration);
            }
        }
        
        protected override void FadeOut(AdvManager manager, AdvFade fade)
        {
            if(fade is AdvColorFade f)
            {
                f.FadeOut(color, duration);
            }
        }
    }
}