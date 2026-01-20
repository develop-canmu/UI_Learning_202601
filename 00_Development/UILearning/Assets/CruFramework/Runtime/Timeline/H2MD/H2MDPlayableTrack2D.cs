using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    [TrackBindingType(typeof(SpriteRenderer))]
    [TrackClipType(typeof(H2MDPlayableClip2D))]
    public class H2MDPlayableTrack2D : H2MDPlayableTrack
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            ScriptPlayable<H2MDPlayableBehaviour2D> playable = ScriptPlayable<H2MDPlayableBehaviour2D>.Create(graph);
            SpriteRenderer trackBinding = (SpriteRenderer)gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            H2MDPlayableBehaviour2D behaviour = playable.GetBehaviour();
            behaviour.target = trackBinding;
            behaviour.clip = (H2MDPlayableClip2D)clip.asset;
            return playable;
        }
    }
}
