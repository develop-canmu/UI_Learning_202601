using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    /// <summary> アニメーション同期制御コントローラー </summary>
    public class AnimationSyncController : MonoBehaviour
    {
        // 同期制御下のコンポーネントリスト
        private List<AnimationSyncComponent> syncComponentList = new List<AnimationSyncComponent>();
        
        // アニメーションレイヤー（通常は0）
        public const int ANIMATION_LAYER = 0;

        // 同期対象ステート
        private int targetStateHash = 0;
        public int TargetStateHash => targetStateHash;

        // 同期対象ステートがセットされてるか
        private bool hasTargetState = false;
        
        /// <summary> コンポーネントを登録 </summary>
        public void Register(AnimationSyncComponent component)
        {
            // まだ追加されてないなら登録
            if (syncComponentList.Contains(component) == false)
            {
                syncComponentList.Add(component);
            }
        }

        /// <summary> 登録解除 </summary>
        public void UnRegister(AnimationSyncComponent component)
        {
            if (syncComponentList.Contains(component))
            {
                syncComponentList.Remove(component);
            }
        }

        /// <summary> 他のアニメーションと同期させる(同ステートのみ対応) </summary>
        public void SetAnimationSyncTime(AnimationSyncComponent component)
        {
            foreach (AnimationSyncComponent syncComponent in syncComponentList)
            {
                // コンポーネントが破棄されているものと非アクティブのもの、同期未完了のものはスキップ
                if (syncComponent == null || syncComponent.gameObject.activeInHierarchy == false || syncComponent.IsCompleteSync == false)
                {
                    continue;
                }
                
                // 現在の再生しているアニメーションから同期させるアニメーター情報を取得
                AnimatorStateInfo stateInfo = syncComponent.Animator.GetCurrentAnimatorStateInfo(ANIMATION_LAYER);
                
                // ターゲットステート更新
                targetStateHash = stateInfo.fullPathHash;
                
                // アニメーションを同期させる(normalizeTimeは増え続けるので小数部を利用)
                float normalizeTime = stateInfo.normalizedTime % 1;
                component.Animator.Play(targetStateHash, ANIMATION_LAYER, normalizeTime);
                return;
            }
        }

        /// <summary> アニメーターが同期済みか </summary>
        public bool IsCompleteSync(Animator animator)
        {
            int stateHash = animator.GetCurrentAnimatorStateInfo(ANIMATION_LAYER).fullPathHash;
            
            // 同期対象ステートがまだセットされてないならセット
            if (hasTargetState == false)
            {
                targetStateHash = stateHash;
                hasTargetState = true;
                return true;
            }

            return stateHash == targetStateHash;
        }
    }
}