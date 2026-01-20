using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using CruFramework.H2MD;

namespace CruFramework.Timeline
{
    public abstract class H2MDPlayableClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private H2MDAsset h2mdAsset = null;
        public H2MDAsset H2MDAsset
        {
            get { return h2mdAsset; }
            set { h2mdAsset = value; }
        }

        [SerializeField]
        private bool isFlip = false;
        /// <summary>反転するかどうか</summary>
        public bool IsFlip { get { return isFlip; } }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
    }
}
