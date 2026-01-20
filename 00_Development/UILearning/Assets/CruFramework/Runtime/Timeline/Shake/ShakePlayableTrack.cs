using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(ShakePlayableClip))]
    public class ShakePlayableTrack : TrackAsset
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            ScriptPlayable<ShakePlayableBehaviour> playable = ScriptPlayable<ShakePlayableBehaviour>.Create(graph);
            GameObject trackBinding = (GameObject)gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            ShakePlayableBehaviour behaviour = playable.GetBehaviour();
            behaviour.target = trackBinding;
            behaviour.clip = (ShakePlayableClip)clip.asset;
            return playable;
        }
    }
}
