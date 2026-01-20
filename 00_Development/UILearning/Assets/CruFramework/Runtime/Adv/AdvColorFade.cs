using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace CruFramework.Adv
{
    [RequireComponent(typeof(Image))]
    public class AdvColorFade : AdvFade
    {
        private Image image = null;
        private Tween tween = null;
        
        private void Awake()
        {
            image = gameObject.GetComponent<Image>();
        }

        public void FadeIn(float duration)
        {
            ForceComplete();
            Color color = image.color;
            color.a = 0;
            tween = image.DOColor(color , duration);
        }
        
        public void FadeOut(Color color, float duration)
        {
            ForceComplete();
            image.color = new Color(color.r, color.g, color.b, 0);
            tween = image.DOColor(color, duration);
        }

        public override void ForceComplete()
        {
            if(tween != null)
            {
                tween.Complete();
                tween = null;
            }
        }
    }
}