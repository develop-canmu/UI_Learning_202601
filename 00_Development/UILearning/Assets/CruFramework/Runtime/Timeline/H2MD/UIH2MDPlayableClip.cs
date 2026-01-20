using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class UIH2MDPlayableClip : H2MDPlayableClip
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<UIH2MDPlayableBehaviour> playable = ScriptPlayable<UIH2MDPlayableBehaviour>.Create(graph);
            return playable;
        }
    }
}
