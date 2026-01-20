using System;
using System.Threading;
using Pjfb.Adv;
using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;
using CruFramework.Timeline;
using CruFramework.H2MD;

namespace Pjfb.Training
{
    /// <summary> トレーニング中のカットイン演出のabstractクラス </summary>
    public class TrainingEventCutInAnimationBase : MonoBehaviour
    {
        // PlayableDirector
        [SerializeField] private PlayableDirector playableDirector;
        // CancellationTokenSource
        private CancellationTokenSource cancellationTokenSource;
        
        // カットイン演出再生非同期
        public async UniTask PlayAnimationAsync(long mCharaId, AppAdvAutoMode autoMode)
        {
            // オート4の時は再生しない
            if (autoMode == AppAdvAutoMode.Skip4)
            {
                playableDirector.gameObject.SetActive(false);
                return;
            }
            
            // CancellationTokenSource初期化
            cancellationTokenSource = new CancellationTokenSource();
            
            // アニメーションオブジェクトをアクティブに
            playableDirector.gameObject.SetActive(true);
            
            // カットイン再生
            try
            {
                // 時間をリセット
                playableDirector.time = 0;
                // クリップを取得してH2MDをセット
                H2MDPlayableClip[] clips = playableDirector.GetTimelineClips<H2MDPlayableClip>("UIH2MD Playable Track", "UIH2MDPlayableClip");
                await PageResourceLoadUtility.LoadAssetAsync<H2MDAsset>(GetCutInPath(mCharaId), x => { clips[0].H2MDAsset = x; }, cancellationTokenSource.Token);
                
                // 再生
                playableDirector.Play();
                
                // 再生完了まで待機
                await UniTask.WaitUntil(() => playableDirector.state != PlayState.Playing, cancellationToken: cancellationTokenSource.Token);
            }
            // キャンセル時
            catch (OperationCanceledException)
            {
                // 再生停止
                if (playableDirector.state != PlayState.Playing)
                {
                    playableDirector.Stop();
                }
            }
            finally
            {
                // アニメーションオブジェクトを非アクティブに
                playableDirector.gameObject.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            // CancellationTokenSourceを破棄
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        /// <summary> カットインのパスを取得 </summary>
        protected virtual string GetCutInPath(long mCharaId)
        {
            string id = ((mCharaId / 1000) % 1000).ToString("0000");
            return $"H2MD/InGame/SkillCutIn/{id}_01_SkillCutIn.h2md";
        }
    }
}