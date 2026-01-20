using System.Collections;
using System.Collections.Generic;
using Pjfb.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pjfb.Runtime.Scripts.Timeline
{
    public class TrackTimeDilationPlayableBehaviour : PlayableBehaviour
    {
        internal ITrackTimeDilation Target = null;
        internal TrackTimeDilationPlayableClip Clip { get; set; }
        
        internal TimelineClip timelineClip { get; set; }
        internal PlayableDirector playableDirector { get; set; }

        private DirectorUpdateMode origUpdateMode; 

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var value = Vector3.zero;
            var time = playable.GetTime();
            var duration = playable.GetDuration();
            var targetDilationTime = 0f;
            if (Clip.UseTimeRemapper)
            {
                targetDilationTime = Clip.SpeedMultiplier.Evaluate(Clip.TimeReMapper) * Clip.ManificationFactor;   
            }
            else
            {
                var evaluatedTime = (float)time;
                if (Clip.IsTimeStrechedToClip)
                {
                    var min = Clip.SpeedMultiplier.keys[0];
                    var max = Clip.SpeedMultiplier.keys[^1];
                    evaluatedTime = (float)math.remap(timelineClip.start, timelineClip.end, (double)min.time, (double)max.time, (double)time);
                }

                targetDilationTime = Clip.SpeedMultiplier.Evaluate(evaluatedTime) * Clip.ManificationFactor;
            }
             
            if (Clip.DirectMode)
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(targetDilationTime);
            }

            Target.OnDilationTimeChange(targetDilationTime);
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (Application.isPlaying) Target?.OnRemoveCurrentClip(Clip);
        }
    }
}
