using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CruFramework.Timeline
{
    [ExecuteAlways]
    public class TimelineSkipController : MonoBehaviour
    {
        [SerializeField]
        internal PlayableDirector playableDirector = null;
        
        [SerializeField]
        internal int[] frames = Array.Empty<int>();
        
        private TimelineAsset timelineAsset = null;
        
        /// <summary>スキップ</summary>
        public void Skip()
        {
            if(timelineAsset == null) return;
            // 現在のフレーム数を計算
            int currentFrame = (int)(playableDirector.time * timelineAsset.editorSettings.frameRate);
            for(int i = 0; i < frames.Length; i++)
            {
                if(frames[i] > currentFrame)
                {
                    Skip(frames[i]);
                    return;
                }
            }
        }
        
        /// <summary>スキップ</summary>
        public void Skip(int frame)
        {
            if(timelineAsset == null) return;
            // 時間取得
            double time = frame / timelineAsset.editorSettings.frameRate;
            playableDirector.time = time;
            if(Application.isPlaying)
            {
                playableDirector.Evaluate();
            }
            else
            {
                playableDirector.DeferredEvaluate();
            }
        }

        private void Update()
        {
            if(playableDirector == null) return;
            if(timelineAsset != playableDirector.playableAsset)
            {
                timelineAsset = (TimelineAsset)playableDirector.playableAsset;
            }
        }
    }
}
