using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;
using CruFramework.Audio;

namespace Pjfb
{
    public class GachaExecuteEffectBase : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector playableDirector = null;
        public PlayableDirector PlayableDirector { get { return playableDirector; } }
        
        [SerializeField]
        private AudioClip bgm = null;
        
        private Func<UniTask<bool>> task = null;
        
        public virtual void Play(Func<UniTask<bool>> task)
        {
            this.task = task;
            playableDirector.Play();
        }
        
        /// <summary>Signal</summary>
        public void OnEndSignalReceived()
        {
            GameObject.Destroy(gameObject);
        }
        
        /// <summary>Signal</summary>
        public void OnPlayBGMSignalReceived()
        {
            AudioManager.Instance.PlayBGM(bgm, 1, true);
        }
        
        /// <summary>Signal</summary>
        public async void OnFadeSignalReceived()
        {
            playableDirector.Pause();
            bool success = await UniTask.Lazy(task);
            if(success)
            {
                playableDirector.Play();
            }
        }
    }
}