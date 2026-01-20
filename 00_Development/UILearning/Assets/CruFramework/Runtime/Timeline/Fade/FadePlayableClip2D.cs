using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class FadePlayableClip2D : FadePlayableClip
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<H2MDPlayableBehaviour2D> playable = ScriptPlayable<H2MDPlayableBehaviour2D>.Create(graph);
            return playable;
        }
    }
}
