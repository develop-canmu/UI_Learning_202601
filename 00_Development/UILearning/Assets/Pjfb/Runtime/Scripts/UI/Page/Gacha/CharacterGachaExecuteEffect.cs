using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Audio;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace Pjfb
{
    public class CharacterGachaExecuteEffect : GachaExecuteEffectBase
    {
        public enum Cut1
        {
            Isagi = 1,
            Kunigami = 2,
            Barou = 3
        }
    
        public enum Cut2
        {
            Normal = 1,
            Gold = 2
        }
    
        public enum Cut3
        {
            Left = 1,
            Right = 2
        }
    
        public enum Cut4
        {
            Normal = 1,
            Break = 2
        }
        
        [SerializeField]
        private GameObject touchToNextRoot = null;
        
        [SerializeField]
        private AudioClip se = null;
        
        private AudioPlayer sePlayer = null;
        
        public override void Play(Func<UniTask<bool>> task)
        {
            sePlayer = AudioManager.Instance.CreateSEPlayer(se);
            base.Play(task);
        }

        /// <summary>Signal</summary>
        public void OnTouchToNextSignalReceived()
        {
            // 一時停止
            PlayableDirector.Pause();
            // 入力待機
            touchToNextRoot.SetActive(true);
            // SE再生
            sePlayer.AudioSource.Play();
        }
        
        /// <summary>uGUI</summary>
        public void OnClickTouchToNext()
        {
            touchToNextRoot.SetActive(false);
            PlayableDirector.Play();
        }

        private void OnDestroy()
        {
            if(sePlayer != null)
            {
                AudioManager.Instance.ReleaseAudioPlayer(sePlayer);
            }
        }
    }
}