
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using CruFramework.H2MD;
using CruFramework.Timeline;
using Cysharp.Threading.Tasks;
using Pjfb.InGame;
using Pjfb.Training;
using UnityEngine.Playables;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandTrainingAwakeningCutin : IAdvCommand, IAdvResource
    {
        [SerializeField][HideInInspector]
        private bool isEnable = true;
        public bool IsEnable{get => isEnable;}
        
        private AppAdvManager manager = null;

        private string GetCutinPath(long mCharId)
        {
            string id = ((mCharId / 1000) % 1000).ToString("0000");
            return $"H2MD/InGame/SkillCutIn/{id}_01_SkillCutIn.h2md";
        }

        async UniTask IAdvResource.PreLoadResources(AdvManager manager)
        {
            if(manager is AppAdvManager m)
            {
                await m.PreLoadResourceAsync( GetCutinPath(m.TrainingCharacterId) );
                await m.PreloadAwakeningAnimation();
            }
        }

        void IAdvCommand.Execute(AdvManager manager)
        {
            this.manager = (AppAdvManager)manager;
            // コマンド停止
            manager.IsStopCommand = true;
            
            // タイムライン読み込み
            PlayableDirector playableDirector = this.manager.GetAwakeningAnimation();
            // 時間をリセット
            playableDirector.time = 0;
            // クリップを取得
            H2MDPlayableClip[] clips = playableDirector.GetTimelineClips<H2MDPlayableClip>("UIH2MD Playable Track", "UIH2MDPlayableClip");
            // H2MDをセット
            clips[0].H2MDAsset = manager.LoadResource<H2MDAsset>( GetCutinPath(this.manager.TrainingCharacterId) );
            // アクティブ
            playableDirector.gameObject.SetActive(true);
            // 再生
            playableDirector.Play();
            
            playableDirector.stopped += OnStopAnimation;
        }
        
        private void OnStopAnimation(PlayableDirector playableDirector)
        {
            playableDirector.stopped -= OnStopAnimation;
            manager.IsStopCommand = false;
            playableDirector.gameObject.SetActive(false);
            
        }
    }
}
