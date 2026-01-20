using Pjfb.Utility;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pjfb.Runtime.Scripts.Timeline
{
    [TrackClipType(typeof(TrackTimeDilationPlayableClip))]
    public class TrackTimeDilationPlayableTrack : TrackAsset
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            var playableDirector = gameObject.GetComponent<PlayableDirector>();
            var target = gameObject.GetComponent<ITrackTimeDilation>();
            var playable = ScriptPlayable<TrackTimeDilationPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Target = target;
            behaviour.timelineClip = clip;
            behaviour.playableDirector = playableDirector;
            behaviour.Clip = (TrackTimeDilationPlayableClip)clip.asset;
            behaviour.Clip.activeTrack = this;
            behaviour.Clip.Director = playableDirector; 
            if (Application.isPlaying) target?.OnAddCurrentClip(behaviour.Clip);
            
            return playable;
        }
    }

    public interface ITrackTimeDilation
    {
        public void OnDilationTimeChange(float dilationTime);
        public void OnAddCurrentClip(TrackTimeDilationPlayableClip clip);
        public void OnRemoveCurrentClip(TrackTimeDilationPlayableClip clip);
    }
}
