using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using CruFramework.Addressables;
using CruFramework.Engine.CrashLog;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using CruFramework.Utils;
using Pjfb.Runtime.Scripts.Utility;
using PolyQA;
using UniRx;

namespace Pjfb
{
    public abstract class PageManager<T> : CruFramework.Page.PageManager<T> where T : System.Enum
    {
        // ページ削除時にハンドル解放するためDicで管理
        private Dictionary<T, List<AsyncOperationHandle>> pageHandleCache = new Dictionary<T, List<AsyncOperationHandle>>();

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>ページリソース読み込み</summary>
        protected async override UniTask<CruFramework.Page.Page> OnLoadPageResource(T page, CancellationToken token)
        {
            AsyncOperationHandle handle = new AsyncOperationHandle();
            
            bool isSucceeded = false;
            while (!isSucceeded)
            {
                try
                {
                    // ハンドル取得
                    handle = AddressablesManager.LoadAssetAsync<GameObject>(GetAddress(page));
                    
                    // 完了まで待機
                    await AddressablesManager.WaitHandle(handle, null, token);
                    
                    isSucceeded = true;
                }
                catch
                {
                    // QAイベント
                    DataSender.Send("AddressableError", "LoadPage");
                    // 失敗した場合はリトライ処理
                    var isRetry = true;
                    ConfirmModalData data = new ConfirmModalData();
                    data.Title = StringValueAssetLoader.Instance["common.confirm"];
                    data.Message = StringValueAssetLoader.Instance["asset.dl_error"];
                    data.PositiveButtonParams =
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.retry"],
                            window => window.Close());
                    data.NegativeButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.to_title"],
                        window =>
                        {
                            isRetry = false;
                            window.Close();
                        });
                    var window =
                        await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
                    await window.WaitCloseAsync();

                    // リトライしないならループ抜ける
                    if (!isRetry)
                    {
                        break;
                    }
                }
            }

            if (!isSucceeded)
            {
                // タイトルに戻る
                AppManager.Instance.BackToTitle();
                return null;
            }

            // キャッシュに追加
            if(!pageHandleCache.ContainsKey(page))
            {
                pageHandleCache.Add(page, new List<AsyncOperationHandle>());
            }
            pageHandleCache[page].Add(handle);
            
            // コールバック
            var obj = handle.Result as GameObject;
            return obj.GetComponent<CruFramework.Page.Page>();
        }
        
        /// <summary>ページのアドレスを取得</summary>
        protected abstract string GetAddress(T page);

        /// <summary>ページが削除された</summary>
        protected override void OnDestroyPageObject(T page)
        {
            if (pageHandleCache.ContainsKey(page))
            {
                // ハンドルが存在する
                if(pageHandleCache[page].Count > 0)
                {
                    // ハンドル解放
                    AsyncOperationHandle handle = pageHandleCache[page][0];
                    AddressablesManager.ReleaseHandle(handle);
                    // キャッシュから削除
                    pageHandleCache[page].RemoveAt(0);
                }
            }
        }
        
        protected virtual void OnDestroy()
        {
            // マネージャ削除時にキャッシュしたハンドルを解放する
            foreach (KeyValuePair<T, List<AsyncOperationHandle>> cache in pageHandleCache)
            {
                foreach (AsyncOperationHandle handle in cache.Value)
                {
                    AddressablesManager.ReleaseHandle(handle);
                }
            }
        }

        /// <summary>再起的にページオブジェクトを探す</summary>
        public Pjfb.Page GetCurrentPageRecursively()
        {
            return GetCurrentPageRecursively(CurrentPageObject);
        }
        
        /// <summary>再起的にページオブジェクトを探す</summary>
        private Pjfb.Page GetCurrentPageRecursively(CruFramework.Page.Page page)
        {
            if(page == null) return null;
                
            switch (page)
            {
                case Pjfb.Page p : return p;
                case CruFramework.Page.PageManager m : return GetCurrentPageRecursively(m.CurrentPageObject);
                default: throw new NotImplementedException();
            }
        }
        
        protected override UniTask<bool> OpenPageAsync(T page, bool stack, object args, PageTransitionType transitionType, CancellationToken token = default)
        {
            return AppManager.Instance.LoadingActionAsync(()=>
            {
                page.CrashLogFlow($"open.");
                return base.OpenPageAsync(page, stack, args, transitionType, token);
            });
        }
    }
    
    public class PageManager : PageManager<PageType>
    {
        private Dictionary<PageType, ResourcesLoader> resourcesLoaderCache = new Dictionary<PageType, ResourcesLoader>();

        protected override async UniTask<bool> OpenPageAsync(PageType page, bool stack, object args, PageTransitionType transitionType, CancellationToken token = default)
        {
            if(page == PageType.NewInGame)
            {
                ClearPageObjectCache();
                AppManager.Instance.UIManager.Header.Hide();
                AppManager.Instance.UIManager.Footer.Hide();
            }
            return await base.OpenPageAsync(page, stack, args, transitionType, token);
        }

        /// <summary>ResourcesLoader取得</summary>
        public ResourcesLoader GetResourcesLoader()
        {
            // dictionaryに存在しない
            if(!resourcesLoaderCache.ContainsKey(LoadingPageType))
            {
                // ResourcesLoader追加
                resourcesLoaderCache.Add(LoadingPageType, new ResourcesLoader());
            }
            return resourcesLoaderCache[LoadingPageType];
        }

        /// <summary>
        /// SendMessageで使うタイプ
        /// </summary>
        public enum MessageType
        {
            /// <summary>フェード開始時</summary>
            BeginFade,
            /// <summary>フェード終了時</summary>
            EndFade
        }

        /// <summary>ページを開いた後の処理</summary>
        protected async override UniTask OnOpenPage(PageType page, CancellationToken token)
        {
            await base.OnOpenPage(page, token);
            
            // インゲームでは30FPSにしているので60FPSに戻す
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            // インゲームではスリープ無効にさせているのでfailsafeとして.
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            // マルチタッチを無効にする
            Input.multiTouchEnabled = false;

            // フェード前にフッターボタンの更新をする
            AppManager.Instance.UIManager.Footer.SwitchButton(page);
            // フェード開始メッセージ
            await SendMessageAsync(MessageType.BeginFade);
            // フェード
            await AppManager.Instance.UIManager.FadeManager.FadeInAsync();
            // フェード終了メッセージ
            await SendMessageAsync(MessageType.EndFade);
        }

        /// <summary>ページを閉じる前の処理</summary>
        protected async override UniTask OnPreClosePage(PageType page, CancellationToken token)
        {
            await base.OnPreClosePage(page, token);
            await AppManager.Instance.UIManager.FadeManager.FadeOutAsync(FadeType.Color);  
            // 画面を閉じた時にヘッダー、フッターを表示する
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            // Title時はMasterの構築前なのでTitleはスキップする
            if (page != PageType.Title)
            {
                // 仮想ポイントのバルーン制御
                AppManager.Instance.UIManager.Footer.GachaFooterButton.UpdatePointAlternativeBalloon();
            }
        }

        protected override string GetAddress(PageType page)
        {
            switch (page)
            {
                // 特殊な配置はcaseで区切る
                default: return ResourcePathManager.GetPath("PageAddress", page.ToString());
            }
        }
        
        protected override void OnDestroyPageObject(PageType page)
        {
            base.OnDestroyPageObject(page);
            
            // ページ削除時にHandle解放
            if(resourcesLoaderCache.TryGetValue(page, out ResourcesLoader resourcesLoader))
            {
                resourcesLoader.Release();
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (ResourcesLoader resourcesLoader in resourcesLoaderCache.Values)
            {
                resourcesLoader.Release();
            }
        }
    }
}
