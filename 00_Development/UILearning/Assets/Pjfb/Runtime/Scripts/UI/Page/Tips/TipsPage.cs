using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Pjfb
{
    public class TipsPageArgs
    {
        public PageType PageType { get; set; }
        public bool Stack { get; set; }
        public object Args { get; set; }
        public Func<UniTask> PreTask { get; set; }
        
        public TipsPageArgs()
        {
        }
        
        public TipsPageArgs(PageType pageType, bool stack, object args, Func<UniTask> preTask = null)
        {
            PageType = pageType;
            Stack = stack;
            Args = args;
            PreTask = preTask;
        }
    }
    
    public class TipsPage : Page
    {
        [SerializeField] 
        private Animator animator;
        
        [SerializeField] 
        private ScrollBanner scroll;
        
        [SerializeField]
        private GameObject arrowButtonObj = null;
        
        [SerializeField]
        private string[] tipsTextList = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            // ヘッダーは非表示
            AppManager.Instance.UIManager.Header.Hide();
            // フッターは非表示
            AppManager.Instance.UIManager.Footer.Hide();
            
            animator.SetTrigger("Open");
        }

        protected override void OnOpened(object args)
        {
            OnOpenAsync(args).Forget();
        }
        
        private async UniTask OnOpenAsync(object args)
        {
            TipsPageArgs pageArgs = (TipsPageArgs)args;
            scroll.SetBannerDatas(tipsTextList);
            arrowButtonObj.SetActive(true);

            // 前画面で処理したい処理
            if(pageArgs.PreTask != null)
            {
                // 後ろが重いとTipsUIが表示されないのでアクティブを待つ
                await UniTask.Yield();
                
                // 実行
                await UniTask.Lazy(pageArgs.PreTask);
            }

            // Tipsページが破棄されていたら止める
            if(this == null) return;
            AppManager.Instance.UIManager.PageManager.OpenPage(pageArgs.PageType, pageArgs.Stack, pageArgs.Args);
        }

        public static void Open(TipsPageArgs args)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Tips, false, args);
        }
    }
}
