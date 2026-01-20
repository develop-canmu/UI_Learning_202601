using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public abstract class FadePlayableBehaviour : PlayableBehaviour
    {
        internal FadePlayableClip clip;
        
        protected abstract void OnAlphaChanged(float alpha);
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            float alpha = Mathf.InverseLerp(0, (float)playable.GetDuration(), (float)playable.GetTime());
            if(clip.fadeType == FadePlayableClip.FadeType.FadeIn)
            {
                alpha = 1 - alpha;
            }
            alpha = Mathf.Round(alpha);
            OnAlphaChanged(alpha);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            float alpha = Mathf.InverseLerp(0, (float)playable.GetDuration(), (float)playable.GetTime());
            if(clip.fadeType == FadePlayableClip.FadeType.FadeIn)
            {
                alpha = 1 - alpha;
            }
            OnAlphaChanged(alpha);
        }
    }
}
