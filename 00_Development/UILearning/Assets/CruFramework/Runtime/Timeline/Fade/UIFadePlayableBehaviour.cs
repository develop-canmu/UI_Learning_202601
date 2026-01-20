using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class UIFadePlayableBehaviour : FadePlayableBehaviour
    {
        internal Image target = null;

        protected override void OnAlphaChanged(float alpha)
        {
            Color color = target.color;
            color.a = alpha;
            target.color = color;
        }
    }
}
