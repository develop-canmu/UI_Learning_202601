using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    public class ShakePlayableClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private float strength = 1.0f;
        /// <summary>揺れの強さ</summary>
        public float Strength
        {
            get { return strength; }
        }
        
        [SerializeField]
        private float vibrato = 1.0f;
        /// <summary>どのくらい振動するか</summary>
        public float Vibrato
        {
            get { return vibrato; }
        }
        
        [SerializeField]
        private bool freezeX = false;
        public bool FreezeX
        {
            get { return freezeX; }
        }
        
        [SerializeField]
        private bool freezeY = false;
        public bool FreezeY
        {
            get { return freezeY; }
        }
        
        [SerializeField]
        private bool freezeZ = false;
        public bool FreezeZ
        {
            get { return freezeZ; }
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<ShakePlayableBehaviour> playable = ScriptPlayable<ShakePlayableBehaviour>.Create(graph);
            return playable;
        }
    }
}
