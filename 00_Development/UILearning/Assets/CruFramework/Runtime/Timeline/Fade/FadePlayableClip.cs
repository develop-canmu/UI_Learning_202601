using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    public abstract class FadePlayableClip : PlayableAsset, ITimelineClipAsset
    {
        public enum FadeType
        {
            FadeIn,
            FadeOut,
        }
        
        [SerializeField]
        internal FadeType fadeType = FadeType.FadeIn;
        
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
    }
}
