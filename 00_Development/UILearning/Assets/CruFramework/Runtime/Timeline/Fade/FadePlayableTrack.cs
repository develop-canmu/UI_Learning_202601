using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    public abstract class FadePlayableTrack : TrackAsset
    {
        [SerializeField]
        private Color color = Color.black;
        /// <summary>カラー</summary>
        public Color Color
        {
            get { return color; }
        }
    }
}
