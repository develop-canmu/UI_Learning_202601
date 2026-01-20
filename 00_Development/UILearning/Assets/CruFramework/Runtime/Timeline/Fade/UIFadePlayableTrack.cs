using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace CruFramework.Timeline
{
    [TrackBindingType(typeof(Image))]
    [TrackClipType(typeof(UIFadePlayableClip))]
    public class UIFadePlayableTrack : FadePlayableTrack
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            ScriptPlayable<UIFadePlayableBehaviour> playable = ScriptPlayable<UIFadePlayableBehaviour>.Create(graph);
            Image trackBinding = (Image)gameObject.GetComponent<PlayableDirector>().GetGenericBinding(this);
            UIFadePlayableBehaviour behaviour = playable.GetBehaviour();
            behaviour.target = trackBinding;
            behaviour.clip = (FadePlayableClip)clip.asset;
            return playable;
        }
    }
}
