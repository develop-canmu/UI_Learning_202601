using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    public static class PlayableDirectorExtensions
    {
        public static T[] GetTimelineClips<T>(this PlayableDirector playableDirector, string trackName, string clipName) where T : UnityEngine.Object
        {
            List<T> result = new List<T>();
            // トラック検索
            IEnumerable<TrackAsset> tracks = ((TimelineAsset)playableDirector.playableAsset).GetOutputTracks();
            foreach (TrackAsset track in tracks)
            {
                // トラック名が一致しない
                if(track.name != trackName) continue;
                // クリップ検索
                IEnumerable<TimelineClip> clips = track.GetClips();
                foreach (TimelineClip clip in clips)
                {
                    // クリップ名が一致しない
                    if(clip.displayName != clipName) continue;
                    result.Add((T)clip.asset);
                }
            }
            return result.ToArray();
            
        } 
        
        public static TResult[] GetTimelineClips<T1,TResult>(this PlayableDirector playableDirector) where T1 : TrackAsset where TResult : Object
        {
            List<TResult> result = new List<TResult>();
            IEnumerable<TrackAsset> tracks = ((TimelineAsset)playableDirector.playableAsset).GetOutputTracks();
            var trackType = typeof(T1);
            foreach (TrackAsset track in tracks)
            {
                if (track.GetType() != trackType) continue;
                
                // クリップ検索
                IEnumerable<TimelineClip> clips = track.GetClips();
                foreach (TimelineClip clip in clips)
                {
                    var asset = (TResult)clip.asset;
                    if(asset == null) continue;
                    
                    result.Add(asset);
                }
            }
            
            return result.ToArray();
        } 
    }
}
