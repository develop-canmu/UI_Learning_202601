using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    /// <summary> アニメーション同期コンポーネント </summary>
    public class AnimationSyncComponent : MonoBehaviour
    {
        // 同期制御コントローラー
        [SerializeField]
        private AnimationSyncController syncController = null;
        
        [SerializeField]
        private Animator animator = null;
        
        public Animator Animator => animator;

        // 同期完了済みか
        private bool isCompleteSync = false;
        public bool IsCompleteSync => isCompleteSync;
        
        // タイマー
        private float timer = 0;
        // 更新間隔
        private float updateInterval = 0.5f;
        
        private void Awake()
        {
            timer = 0f;
            syncController.Register(this);
        }
        
        private void OnEnable()
        {
            // アニメーションを同期
            syncController.SetAnimationSyncTime(this);
        }

        private void OnDisable()
        {
            timer = 0f;
            isCompleteSync = false;
        }

        private void Update()
        {
            // 同期完了しているなら無視
            if (isCompleteSync)
            {
                return;
            }

            timer += Time.deltaTime;

            // 更新時間がまだ
            if (timer < updateInterval)
            {
                return;
            }
            
            timer = 0f;
            // 同期が完了しているか確認
            if (syncController.IsCompleteSync(animator))
            {
                isCompleteSync = true;
            }
        }
        
        private void OnDestroy()
        {
            syncController.UnRegister(this);
        }
    }
}