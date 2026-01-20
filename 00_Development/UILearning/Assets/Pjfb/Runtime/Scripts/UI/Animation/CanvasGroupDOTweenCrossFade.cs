using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Pjfb
{
    public class CanvasGroupDOTweenCrossFade : MonoBehaviour, ISyncTargetDOTween
    {
        public class SyncParameter : DOTweenSyncParameter
        {
            private int currentIndex = 0;
            public int CurrentIndex => currentIndex;
          
            /// <summary> パラメータセット </summary>
            public void SetValue(int currentIndex)
            {
                this.currentIndex = currentIndex;
            }
        }
        
        // クロスフェード対象オブジェクト
        [SerializeField]
        private CanvasGroup[] fadeTargetList = null;

        // アニメーションの遷移時間
        [SerializeField]
        private float transitionTime = 0.1f;

        // クロスフェード実行間隔
        [SerializeField]
        private float intervalTime = 1.0f;

        // Awake時に再生させるか
        [SerializeField]
        private bool playOnAwake = false;
        
        private Sequence sequence = null;

        private int currentTargetIndex = 0;
        private int nextTargetIndex = 0;
        
        // 再生中
        private bool isPlaying = false;
        // アクティブオブジェクト数
        private int activeCount = 0;
        
        private SyncParameter syncParameter = null;
        
        
        private void Awake()
        {
            if (playOnAwake == false)
            {
                return;
            }
            Play();
        }

        public Tween Tween()
        {
            return sequence;
        }

        public GameObject GetAnimationTarget()
        {
            return this.gameObject;
        }

        public DOTweenSyncParameter GetParameter()
        {
            if (syncParameter == null)
            {
                syncParameter = new SyncParameter();
            }
            
            syncParameter.SetValue(currentTargetIndex);
            
            return syncParameter;
        }

        public void ApplySyncParameter(DOTweenSyncParameter parameter)
        {
           SyncParameter param = (SyncParameter)parameter;
           
           currentTargetIndex = param.CurrentIndex;
           
           // 同期対象の再生Indexがフェードリストを超えているか非表示の場合は有効なIndexを探す
           if (param.CurrentIndex >= fadeTargetList.Length || fadeTargetList[currentTargetIndex].gameObject.activeSelf == false)
           {
               currentTargetIndex = FindNextTargetIndex();
           }

           // 次に表示するIndexを探す
           nextTargetIndex = FindNextTargetIndex();
           
           // Alphaセット(アニメーションの開始点の初期化)
           fadeTargetList[currentTargetIndex].alpha = 1.0f;
           fadeTargetList[nextTargetIndex].alpha = 0f;
        }

        private void Init()
        {
            // 再生中なら止める
            Stop();
            
            // 再生開始前の初期化
            currentTargetIndex = -1;
            activeCount = 0;
            
            for (int i = 0; i < fadeTargetList.Length; i++)
            {
                if (fadeTargetList[i].gameObject.activeSelf)
                {
                    // 初期設定前ならセット
                    if (currentTargetIndex < 0)
                    {
                        currentTargetIndex = i;
                    }

                    activeCount++;
                    // 初期表示のものは表示、それ以外は非表示に
                    fadeTargetList[i].alpha = currentTargetIndex == i ? 1.0f : 0.0f;
                }
            }

            // 初期Index確定後に次のIndexを探す
            nextTargetIndex = FindNextTargetIndex();
        }
        
        /// <summary> クロスフェード開始 </summary>
        public bool Play()
        {
            Init();
            
            // アクティブなオブジェクト数が１つ以下に場合はクロスフェード出来ないのでリターン
            if (activeCount <= 1)
            {
                return false;
            }

            CreateAnimation();
            return true;
        }
        
        /// <summary> アニメーション同期再生 </summary>
        public bool PlaySync(string key)
        {
            // アニメーションを再生するか
            bool isPlayAnimation = Play();

            if (isPlayAnimation == false)
            {
                return false;
            }
            
            DOTweenSyncManager.ApplySync(this, key);
            return true;
        }

        private void CreateAnimation()
        {
            isPlaying = true;
            sequence = DOTween.Sequence();
            sequence.SetLoops(-1);
            // 実行間隔設定
            sequence.AppendInterval(intervalTime);
            sequence.AppendCallback(() =>
            {
                fadeTargetList[currentTargetIndex].alpha = 1;
                fadeTargetList[nextTargetIndex].alpha = 0;
            });
            
            // DoFadeだとアニメーション対象がSequenceに追加した段階のもので固定されるので
            // フェードアウト
            sequence.Append(DOTween.To(() => fadeTargetList[currentTargetIndex].alpha, value => fadeTargetList[currentTargetIndex].alpha = value, 0.0f, transitionTime));
            // フェードイン
            sequence.Join(DOTween.To(() => fadeTargetList[nextTargetIndex].alpha, value => fadeTargetList[nextTargetIndex].alpha = value, 1.0f, transitionTime));
            
            // クロスフェード完了後の処理
            sequence.AppendCallback(() =>
            {
                // 完了後に完全な終了値にならないのでセットする
                fadeTargetList[currentTargetIndex].alpha = 0;
                fadeTargetList[nextTargetIndex].alpha = 1;
                // 次のIndexへ
                currentTargetIndex = nextTargetIndex;
                nextTargetIndex = FindNextTargetIndex();
            });
        }

        /// <summary> 次に表示させるオブジェクトのIndexを返す </summary>
        private int FindNextTargetIndex()
        {
            int nextIndex = currentTargetIndex;

            for (int i = 1; i < fadeTargetList.Length; i++)
            {
                nextIndex = (nextIndex + 1) % fadeTargetList.Length;
                
                // アクティブなもののみIndexを返す
                if (fadeTargetList[nextIndex].gameObject.activeSelf)
                {
                    return nextIndex;
                }
            }

            return -1;
        }

        /// <summary> クロスフェード停止 </summary>
        public void Stop()
        {
            // 再生中の時のみ停止させる
            if (isPlaying == false) return;

            ClearSequence();
            isPlaying = false;
        }

        /// <summary> Sequence破棄 </summary>
        private void ClearSequence()
        {
            if (sequence == null) return;
            sequence.Kill();
            sequence = null;
        }

        /// <summary> 明示的に破棄する </summary>
        private void OnDestroy()
        {
            ClearSequence();
        }
    }
}