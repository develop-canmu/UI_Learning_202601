using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Audio;
using UnityEngine;
using UnityEngine.Playables;

namespace CruFramework.Timeline
{
    public class AudioAssistant : MonoBehaviour
    {
        [Serializable]
        internal class BindingData
        {
            public string trackName = string.Empty;
            public string audioGroup = string.Empty;
        }
    
        [SerializeField]
        internal PlayableDirector playableDirector = null;
        
        [SerializeField]
        internal BindingData[] bindingDatas = Array.Empty<BindingData>();
        
        private List<AudioPlayer> audioPlayerList = new List<AudioPlayer>(); 

        private void Awake()
        {
            if(playableDirector == null) return;
            if(!AudioManager.Exists()) return;

            foreach (BindingData bindingData in bindingDatas)
            {
                // オーディオグループ
                AudioGroup audioGroup = new AudioGroup(bindingData.audioGroup);
                // オーディオプレイヤーを生成
                AudioPlayer audioplayer = AudioManager.Instance.CreateAudioPlayer(audioGroup, null);
                // キャッシュに追加
                audioPlayerList.Add(audioplayer);

                foreach (PlayableBinding binding in playableDirector.playableAsset.outputs)
                {
                    if(binding.streamName != bindingData.trackName) continue;
                    playableDirector.SetGenericBinding(binding.sourceObject, audioplayer.AudioSource);
                }
            }
        }

        private void OnDestroy()
        {
            if(!AudioManager.Exists()) return;
            foreach (AudioPlayer audioPlayer in audioPlayerList)
            {
                AudioManager.Instance.ReleaseAudioPlayer(audioPlayer);                
            }
        }
    }
}
