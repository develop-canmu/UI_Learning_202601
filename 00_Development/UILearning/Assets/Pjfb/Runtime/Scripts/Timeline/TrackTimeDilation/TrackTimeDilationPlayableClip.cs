using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pjfb.Runtime.Scripts.Timeline
{
    public class TrackTimeDilationPlayableClip : PlayableAsset, ITimelineClipAsset
    {
        public AnimationCurve SpeedMultiplier
        {
            get => speedMultiplier;
        }

        public float ManificationFactor
        {
            get => manificationFactor;
            set => manificationFactor = value;
        }

        public float TimeReMapper
        {
            get => timeReMapper;
            set => timeReMapper = value;
        }

        public bool IsTimeStrechedToClip
        {
            get => isTimeStretchedToClip;
            set => isTimeStretchedToClip = value;
        }

        public bool DirectMode
        {
            get => directMode;
            set => directMode = value;
        }

        public PlayableDirector Director
        {
            get => director;
            set => director = value;
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public bool UseTimeRemapper
        {
            get => useTimeRemapper;
            set => useTimeRemapper = value;
        }

        public string clipName;
        public TrackTimeDilationPlayableTrack activeTrack;
        [SerializeField] private AnimationCurve speedMultiplier = AnimationCurve.Constant(0f, 1f, 1);
        [SerializeField] private bool isTimeStretchedToClip = true;
        [SerializeField] private float timeReMapper = 1f;
        [SerializeField] private bool directMode = true;
        private PlayableDirector director;

        [SerializeField] private float manificationFactor = 1f;
        [SerializeField] private bool useTimeRemapper;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TrackTimeDilationPlayableBehaviour>.Create(graph);
            return playable;
        }

        private void OnDisable()
        {
            if (director == null) return;
        }

        private void OnEnable()
        {
            if (director == null) return;
        }
    }
}
