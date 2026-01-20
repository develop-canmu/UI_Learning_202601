using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Pjfb.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorController : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private Animator animator;
        #endregion

        #region PublicFields
        public bool isAnimating => animator.enabled;
        #endregion
        
        #region PrivateProperties
        private Dictionary<string, AnimationClip> _animationClipDictionary = new();
        [CanBeNull] private AnimationClip _currentClip;
        private List<CancellationTokenSource> cancellationTokenSources = new();
        #endregion
        
        #region PublicMethods
        public async UniTask Play(string stateName)
        {
            if(!_animationClipDictionary.TryGetValue(stateName, out var clip)) return;
            _currentClip = clip;
            
            await Play(clip.name, 0);
            await WaitWhileAnimating();
            animator.enabled = false;
        }

        /// <summary>
        /// 最初の状態に戻す
        /// </summary>
        [ContextMenu("StopAnimation")]
        public async void StopAnimation()
        {
            if (_currentClip != null) {
                await Play(stateName: _currentClip.name, normalizedTime: 0);
                await NextFrame();
            }
            animator.enabled = false;
        }
        
        /// <summary>
        /// 終わった状態にする
        /// </summary>
        [ContextMenu("FinishAnimation")]
        public async void FinishAnimation()
        {
            if (_currentClip != null) {
                await Play(stateName: _currentClip.name, normalizedTime: 1);
                await NextFrame();
            }
            animator.enabled = false;
        }

        [ContextMenu("Pause")]
        public void Pause()
        {
            animator.enabled = false;
        }

        [ContextMenu("Resume")]
        public async UniTask Resume()
        {
            animator.enabled = true;
            await WaitWhileAnimating();
            animator.enabled = false;
        }
        
        public void Init()
        {
            if (animator == null) animator = GetComponent<Animator>();
            _animationClipDictionary = animator.runtimeAnimatorController.animationClips.ToDictionary(aClip => aClip.name);
            Reset();
        }
        #endregion

        #region OverrideMethods
        private void Awake()
        {
            Init();
        }
        #endregion

        #region PrivateMethods
        private async UniTask Play(string stateName, float normalizedTime)
        {
            var source = GetCancellationTokenSource();
            await UniTask.NextFrame(source.Token);
            RemoveCancellationToken(source);
            animator.enabled = true;
            animator.Play(stateName: stateName, layer: -1, normalizedTime: normalizedTime);
        }
        
        private async UniTask WaitWhileAnimating()
        {
            var source = GetCancellationTokenSource();
            await UniTask.WaitWhile(() => animator.enabled && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f, cancellationToken: source.Token);
            RemoveCancellationToken(source);
        }

        private async UniTask NextFrame()
        {
            var source = GetCancellationTokenSource();
            await UniTask.NextFrame(source.Token);
            RemoveCancellationToken(source);
        }
        
        private void Reset()
        {
            if (_currentClip != null) FinishAnimation();
            _currentClip = null;
            animator.enabled = false;
        }
        #endregion

        #region CancellationToken処理
        private CancellationTokenSource GetCancellationTokenSource()
        {
            var source = new CancellationTokenSource();
            cancellationTokenSources.Add(source);
            return source;
        }

        private void RemoveCancellationToken(CancellationTokenSource source)
        {
            cancellationTokenSources.Remove(source);
            DisposeToken(source);
        }

        private void OnDestroy()
        {
            cancellationTokenSources.ForEach(DisposeToken);
            cancellationTokenSources.Clear();
        }
        
        private void DisposeToken(CancellationTokenSource source)
        {
            source.Cancel();
            source.Dispose();
        }
        #endregion
        
#if UNITY_EDITOR
        private int playIndex;
        [ContextMenu("PlayClips")]
        public void DebugPlayClips()
        {
            var clips = _animationClipDictionary.Values.ToList();
            if (clips.Count == 0) return;
            playIndex = (playIndex + 1) % clips.Count;
            Play(clips[playIndex].name).Forget();
        }
#endif
    }
}