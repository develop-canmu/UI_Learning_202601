using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Timeline;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Pjfb.Title
{
    public class OpMovie : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector playableDirector = null;
        
        [SerializeField]
        private TimelineSkipController skipController = null;
        
        private bool isCompleted = false;

        /// <summary>OP再生</summary>
        public async UniTask PlayAsync(CancellationToken token)
        {
            // 終了検知
            isCompleted = false;
            // イベント登録
            playableDirector.stopped -= OnStopped;
            playableDirector.stopped += OnStopped;
            // 再生
            playableDirector.Play();
            // 再生完了まで待機
            await UniTask.WaitUntil(() => isCompleted, cancellationToken: token);
            // 非表示
            gameObject.SetActive(false);
        }

        /// <summary>動画再生終了通知</summary>
        private void OnStopped(PlayableDirector director)
        {
            isCompleted = true;
        }
        
        /// <summary>uGUI</summary>
        public void OnClickSkipButton()
        {
            skipController.Skip();
        }
    }
}
