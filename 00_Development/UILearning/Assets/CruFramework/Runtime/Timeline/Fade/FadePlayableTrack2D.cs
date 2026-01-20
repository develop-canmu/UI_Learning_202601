using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    [TrackBindingType(typeof(SpriteRenderer))]
    [TrackClipType(typeof(FadePlayableClip2D))]
    public class FadePlayableTrack2D : FadePlayableTrack
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            ScriptPlayable<FadePlayableBehaviour2D> playable = ScriptPlayable<FadePlayableBehaviour2D>.Create(graph);
            SpriteRenderer trackBinding = (SpriteRenderer)gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            FadePlayableBehaviour2D behaviour = playable.GetBehaviour();
            behaviour.target = trackBinding;
            behaviour.clip = (FadePlayableClip)clip.asset;
            return playable;
        }
    }
}
