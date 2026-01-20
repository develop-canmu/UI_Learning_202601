using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;


namespace CruFramework.Page
{

    /// <summary>ページ</summary>
    [RequireComponent(typeof(CanvasGroup))]
    [FrameworkDocument("Page", nameof(Page), "ページ表示用クラス。各ページでオーバーライドして使用。(現)OnPreClose > (次)OnPreOpen > (現)OnClosed > (次)OnEnable > (次)OnOpned")]
    public abstract class Page : MonoBehaviour
    {
        [SerializeField]
        private bool isDestroyOnClosed = false;
        /// <summary>閉じた時に破棄する</summary>
        public bool IsDestroyOnClosed{get{return isDestroyOnClosed;}}
        
        private PageManager manager = null;
        /// <summary>マネージャー</summary>
        public PageManager Manager{get{return manager;}internal set{manager = value;}}
        
        private PageTransitionType transitionType = PageTransitionType.Open;
        /// <summary>遷移した種類</summary>
        public PageTransitionType TransitionType{get{return transitionType;}internal set{transitionType = value;}}
        
        
        private object arguments = null;
        /// <summary>開いたときの引数</summary>
        public object OpenArguments{get{return arguments;}}

        private CanvasGroup _canvasGroup = null;
        /// <summary>
        /// キャンバスグループ
        /// 表示を隠すために使う
        /// </summary>
        public CanvasGroup PageCanvasGroup
        {
            get
            {
                if(_canvasGroup == null && this != null)
                {
                    _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        /// <summary>引数の設定</summary>
        public void SetArguments(object args)
        {
                arguments = args;
        }

        [FrameworkDocument("メッセージを受け取ったときの処理。")]
        /// <summary>開いた瞬間の通知</summary>
        protected virtual UniTask OnMessage(object value)
        {
            return default;
        }
        
        [FrameworkDocument("ページが完全に開いたあとの初期化処理はここで。フェード等の演出がある場合それが終わったあとに呼ばれる")]
        /// <summary>開らき終わったときの通知</summary>
        protected virtual UniTask OnOpen(object args)
        {
            return default;
        }
        
        [FrameworkDocument("ページが完全に開いたあとの初期化処理はここで。フェード等の演出がある場合それが終わったあとに呼ばれる")]
        /// <summary>開らき終わったときの通知</summary>
        protected virtual void OnOpened(object args){}
        
        [FrameworkDocument("ページが閉じたあとに呼ばれる")]
        /// <summary>閉じたときの通知</summary>
        protected virtual void OnClosed(){}
        

        
        [FrameworkDocument("ページが開いた時の初期化処理はここで。フェード等の演出がある場合それの前に呼ばれる")]
        /// <summary>開いた瞬間の通知</summary>
        protected virtual void OnEnablePage(object args){}
        
        [FrameworkDocument("ページを開く前の処理。API等はここで投げる")]
        /// <summary>開く前の処理</summary>
        protected virtual UniTask OnPreOpen(object args, CancellationToken token)
        {
            return default;
        }
        
        [FrameworkDocument("ページを閉じる前の処理。ページを閉じた時に保存したりする。falseを返すとページ遷移をキャンセルします")]
        /// <summary>閉じる前の処理</summary>
        protected virtual UniTask<bool> OnPreClose(CancellationToken token)
        {
            return new UniTask<bool>(true);
        }
        
        [FrameworkDocument("ページを離れる前の処理。ページを離れた時に保存したりする。falseを返すとページ遷移をキャンセルします")]
        /// <summary>離れるときの通知</summary>
        protected virtual UniTask<bool> OnPreLeave(CancellationToken token)
        {
            return new UniTask<bool>(true);
        }
        
        /// <summary>閉じた後の処理</summary>
        internal virtual void OnClosedInternal()
        {
            OnClosed();
        }
        
        
        /// <summary>開いたあとの処理</summary>
        internal void OnOpenedInternal(object args)
        {
            OnOpened(args);
        }
        
        /// <summary>開いたあとの処理</summary>
        internal UniTask OnOpenInternal(object args)
        {
            return OnOpen(args);
        }
        
        /// <summary>開いたあとの処理</summary>
        internal void OnEnablePageInternal(object args)
        {
            OnEnablePage(args);
        }
        
        /// <summary>
        /// 閉じる前の処理
        /// falseを返すことでページ遷移をキャンセルできる
        /// </summary>
        internal virtual UniTask<bool> OnPreCloseInternal(CancellationToken token)
        {
            return OnPreClose(token);
        }
        
        /// <summary>
        /// 離れる前の処理
        /// falseを返すことでページ遷移をキャンセルできる
        /// </summary>
        internal virtual UniTask<bool> OnPreLeaveInternal(CancellationToken token)
        {
            return OnPreLeave(token);
        }
        
        /// <summary>
        /// 画面を開く前の準備等
        /// </summary>
        internal UniTask OnPreOpenInternal(object args, CancellationToken token)
        {
            return OnPreOpen(args, token);
        }
        
        /// <summary>メッセージ処理</summary>
        internal UniTask OnSendMessageInternal(object value)
        {
            return OnMessage(value);
        }
        
        /// <summary>即座に非表示にする</summary>
        public void HideImmediate()
        {
            PageCanvasGroup.alpha = 0f;
        }
        
        /// <summary>非表示にする</summary>
        public async UniTask Hide(float duration, CancellationToken token)
        {
            await PageCanvasGroup.DOFade(0f, duration).ToUniTask(cancellationToken: token);
        }

        /// <summary>即座に表示する</summary>
        public void ShowImmediate()
        {
            PageCanvasGroup.alpha = 1f;
        }

        /// <summary>表示する</summary>
        public async UniTask Show(float duration, CancellationToken token)
        {
            await PageCanvasGroup.DOFade(1.0f, duration).ToUniTask(cancellationToken: token);
        }
    }
}
