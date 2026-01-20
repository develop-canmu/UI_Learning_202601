using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CruFramework.Audio
{
    [FrameworkDocument("Audio", nameof(AudioPlayer), "Audio再生用クラス")]
    public class AudioPlayer : MonoBehaviour
    {
        internal AudioGroup audioGroup;
        
        internal AudioSource audioSource = null;
        /// <summary>オーディオソース</summary>
        [FrameworkDocument("AudioSource")]
        public AudioSource AudioSource
        {
            get { return audioSource; }
        }
        
        /// <summary>重複再生</summary>
        [FrameworkDocument("重複再生")]
        public void PlayOneShot(float volumeScale = 1.0f)
        {
            audioSource.PlayOneShot(audioSource.clip, volumeScale);
        }
        
        /// <summary>クリップデータ取得</summary>
        public float[] GetClipData()
        {
            float[] data = new float[audioSource.clip.channels * audioSource.clip.samples];
            audioSource.clip.GetData(data, 0);
            return data;
        }
    }
}
