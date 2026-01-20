using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace CruFramework.Timeline
{
    [TrackBindingType(typeof(RawImage))]
    [TrackClipType(typeof(UIH2MDPlayableClip))]
    public class UIH2MDPlayableTrack : H2MDPlayableTrack
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            ScriptPlayable<UIH2MDPlayableBehaviour> playable = ScriptPlayable<UIH2MDPlayableBehaviour>.Create(graph);
            RawImage trackBinding = (RawImage)gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            UIH2MDPlayableBehaviour behaviour = playable.GetBehaviour();
            behaviour.target = trackBinding;
            behaviour.clip = (UIH2MDPlayableClip)clip.asset;
            return playable;
        }
    }
}
