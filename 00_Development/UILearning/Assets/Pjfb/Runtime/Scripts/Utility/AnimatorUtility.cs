using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    public static class AnimatorUtility
    {
        public static async UniTask WaitStateAsync(Animator animator, string paramName, CancellationToken token = default)
        {
            // 現在ステート
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            // トリガーセット
            animator.SetTrigger(paramName);
            // ステートが変わるの待機
            await UniTask.WaitWhile(() => animator != null && currentInfo.Equals(animator.GetCurrentAnimatorStateInfo(0)), cancellationToken: token);
            // ステート終わるまで待機
            if(animator != null)
            {
                float length = animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.Delay(TimeSpan.FromSeconds(length), cancellationToken: token);
            }
        }
        
        //// <summary> アニメーター内の全てのトリガーをリセット </summary>
        public static void ResetAllTriggers(this Animator animator)
        {
            foreach (var parameter in animator.parameters)
            {
                // トリガータイプのパラメータならトリガーをリセットする
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.ResetTrigger(parameter.name);
                }
            }
        }
        
        public static void SkipToEnd(this Animator animator, string stateName)
        {
            animator.Play(stateName, -1, 1);
        }
        
        /// <summary>
        /// SkipToEndとほぼ同等
        /// stateNameの存在チェックが追加してある
        /// </summary>
        public static async UniTask<bool> SkipToEnd(this Animator animator, string stateName, int layer, CancellationToken token = default, bool ignoreWarning = false)
        {
            // 指定したステートが存在しない場合は何もしない
            if (!animator.HasState(stateName, layer))
            {
                if (!ignoreWarning)
                {
                    CruFramework.Logger.LogWarning($"State {stateName} not found in Animator {animator.name}");
                }
                return false;
            }

            // ステートを再生
            animator.Play(stateName, layer, 1);
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1, cancellationToken: token);
            return true;
        }

        //// <summary> 指定したStateに変わるまで待つ(トリガーのセットは行わない) </summary>
        public static async UniTask WaitStateChangeAsync(Animator animator, string stateName, CancellationToken token = default, bool ignoreWarning = false)
        {
            // 指定したステートが存在しない場合は何もしない
            if (!animator.HasState(stateName))
            {
                if (!ignoreWarning)
                {
                    CruFramework.Logger.LogWarning($"State {stateName} not found in Animator {animator.name}");
                }
                return;
            }
            
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName), cancellationToken: token);
        }

        //// <summary> 指定したStateのアニメーションが完了するまで待つ </summary>
        public static async UniTask WaitStateFinishAsync(Animator animator, string stateName, CancellationToken token = default, bool ignoreWarning = false)
        {
            // 指定したステートが存在しない場合は何もしない
            if (!animator.HasState(stateName))
            {
                if (!ignoreWarning)
                {
                    CruFramework.Logger.LogWarning($"State {stateName} not found in Animator {animator.name}");
                }
                return;
            }

            await WaitStateChangeAsync(animator, stateName, token);
            // アニメーションの再生が完了するまで待つ
            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1, cancellationToken: token);
        }

        /// <summary>
        /// 指定した名前のステートが存在するか
        /// 実装経緯：Unity標準のAnimation.HasStateが使いづらいため
        /// </summary>
        public static bool HasState(this Animator animator, in string stateName, in int layer = 0)
        {
            return animator.HasState(layer, Animator.StringToHash(stateName));
        }
    }
}
