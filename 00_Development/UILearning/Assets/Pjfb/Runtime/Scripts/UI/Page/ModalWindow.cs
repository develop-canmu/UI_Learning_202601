using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Pjfb
{
    public class ModalWindow : CruFramework.Page.ModalWindow
    {
        private static readonly string OpenKey = "Open";
        private static readonly string CloseKey = "Close";
        
        [SerializeField]
        private RectTransform backKeyObject = null;
        /// <summary>バックキーオブジェクト</summary>
        public RectTransform BackKeyObject 
        { 
            get { return backKeyObject; }
            set { backKeyObject = value; } 
        }
        public bool DisableBackKey = false;
        
        [SerializeField]
        private bool isH2MDControl = false;
        public bool ISH2MDControl
        {
            get { return isH2MDControl; }
        }

        private Animator animator = null;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if(isH2MDControl)
            {
                // H2MDの再生を停止
                AppManager.Instance.UIManager.H2MDUIManager.StopEffect();
            }
            OnAwake();    
        }
        
        protected virtual void OnAwake()
        {
        }
        
        protected async override UniTask OnOpen(CancellationToken token)
        {
            // 開くアニメーション
            if(animator != null)
            {
                await AnimatorUtility.WaitStateAsync(animator, OpenKey, token);
            }
            
            // アニメーション完了通知
            OnOpened();
        }

        protected async override UniTask OnPreClose(CancellationToken token)
        {
            // 閉じるアニメーション
            if(animator != null && animator.gameObject.activeInHierarchy)
            {
                await AnimatorUtility.WaitStateAsync(animator, CloseKey, token);
            }
            // アニメーション完了通知
            OnClosed();
        }

        private void OnDestroy()
        {
            if(AppManager.Instance == null) return;
            // H2MDの再生
            if(isH2MDControl)
            {
                AppManager.Instance.UIManager.H2MDUIManager.PlayEffect();
            }
        }

        /// <summary>開くアニメーション完了通知</summary>
        protected virtual void OnOpened()
        {
        }
        
        /// <summary>閉じるアニメーション完了通知</summary>
        protected virtual void OnClosed()
        {
        }

        protected override void OnError(Exception e)
        {
            gameObject.SetActive(false);
            base.OnError(e);
        }
    }
}
