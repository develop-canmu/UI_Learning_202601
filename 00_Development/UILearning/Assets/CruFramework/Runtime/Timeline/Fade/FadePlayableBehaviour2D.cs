using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class FadePlayableBehaviour2D : FadePlayableBehaviour
    {
        internal SpriteRenderer target = null;

        protected override void OnAlphaChanged(float alpha)
        {
            Color color = target.color;
            color.a = alpha;
            target.color = color;
        }
    }
}
