using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using PolyQA;

namespace CruFramework.Page
{
    
    public enum PageTransitionType
    {
        Open, Back, Next
    }
    
    public enum PageBackType
    {
        Default, AndroidBackKey
    }
    
    public enum PageMoveResult
    {
        Success, // 成功
        NonStack, // ページスタックがないので戻れない
        Cancel, // 遷移がキャンセルされた
    }
    
    public abstract class PageManager : Page
    {
        public abstract UniTask<PageMoveResult> PrevPageAsync();
        public abstract UniTask<PageMoveResult>  NextPageAsync();
        public abstract UniTask SendMessageAsync(object value);
        public abstract Page CurrentPageObject{get;}
        public abstract void ClearAllPageStack();
    }
    
    [FrameworkDocument("Page", nameof(PageManager), "ページ管理用クラス。各ページでオーバーライドして使用")]
    public abstract class PageManager<T> : PageManager where T : System.Enum
    {
        private struct PageData
        {
            public T page;
            public object args;
            
            public PageData(T page, object args)
            {
                this.page = page;
                this.args = args;
            }
        }

        [SerializeField]
        private bool openOnAwake = false;

        [SerializeField]
        // 最初に開くページ
        private T firstPage = default(T);
        
        [SerializeField]
        private GameObject pageRoot = null;

        /// <summary>表示/非表示フェード間隔</summary>
        [SerializeField] private float fadeDuration = 0.3f;

        private T currentPageType = default(T);
        /// <summary>現在のページ</summary>
        public T CurrentPageType{get{return currentPageType;}}
        
        private Page currentPageObject = null;
        /// <summary>現在開いているページのオブジェクト</summary>
        public override Page CurrentPageObject{get{return currentPageObject;}}
        
        private T loadingPageType = default(T);
        /// <summary>現在のページ</summary>
        public T LoadingPageType{get{return loadingPageType;}}
        
        // ページオブジェクトのキャッシュ
        private Dictionary<T, Page> pageObjectCache = new Dictionary<T, Page>();
        
        // ページスタック
        private List<PageData> pageStack = new List<PageData>();
        // ページスタック位置
        private int pageStackIndex = 0;

        /// <summary>ページの読み込み</summary>
        protected abstract UniTask<Page> OnLoadPageResource(T page, CancellationToken token);
        /// <summary>ページオブジェクトをインスタンス化する</summary>
        protected virtual bool IsCreateInstancePage{get{return true;}}
        
        /// <summary>ページを開く前ののコールバック</summary>
        protected virtual UniTask OnPreOpenPage(T page, CancellationToken token)
        {
            return default;
        }
        
        /// <summary>ページが閉じる前のコールバック</summary>
        protected virtual UniTask OnPreClosePage(T page, CancellationToken token)
        {
            return default;
        }
        
        /// <summary>ページを開いたあとのコールバック</summary>
        protected virtual UniTask OnOpenPage(T page, CancellationToken token)
        {
            return default;
        }
        
        /// <summary>ページオブジェクトが破棄された</summary>
        protected virtual void OnDestroyPageObject(T page){ }

        private void Awake()
        {
            if (openOnAwake)
            {
                // 最初のページを開く
                OpenPage(firstPage, true, null);
            }
        }

        /// <summary>前のページへ</summary>
        [FrameworkDocument("前のページへ")]
        public void PrevPage()
        {
            PrevPageAsync().Forget();            
        }
        
        public override async UniTask<PageMoveResult> PrevPageAsync()
        {        
            // サブページを前のページに戻る
            if(currentPageObject is PageManager m)
            {
                PageMoveResult subPageResult = await m.PrevPageAsync();
                if(subPageResult != PageMoveResult.NonStack)
                {
                    return subPageResult;
                }
            }
        
            // 前のページなし
            if(pageStackIndex <= 1)return PageMoveResult.NonStack;
            // ページ情報
            PageData pageData = pageStack[pageStackIndex - 2];
            // ページ移動
            pageStackIndex--;
            bool openResult = await OpenPageAsync( pageData.page, false, pageData.args, PageTransitionType.Back);
            // 遷移キャンセル
            if(openResult == false)
            {
                pageStackIndex++;
                return PageMoveResult.Cancel;
            }
            
            
            return PageMoveResult.Success;            
        }
        
        /// <summary>次のページへ</summary>
        [FrameworkDocument("次のページへ")]
        public void NextPage()
        {
            NextPageAsync().Forget();
        }
        
        /// <summary>次のページへ</summary>
        public override async UniTask<PageMoveResult> NextPageAsync()
        {
            // サブページを次のページに戻る
            if(currentPageObject is PageManager m)
            {
                PageMoveResult subPageResult = await m.NextPageAsync();
                if(subPageResult != PageMoveResult.NonStack)
                {
                    return subPageResult;
                }
            }
            
            // 次のページなし
            if(pageStackIndex >= pageStack.Count)return PageMoveResult.NonStack;
            // ページ情報
            PageData pageData = pageStack[pageStackIndex];
            pageStackIndex++;
            // ページ移動
            bool openResult = await OpenPageAsync( pageData.page, false, pageData.args, PageTransitionType.Next);
            // 遷移キャンセル
            if(openResult == false)
            {
                pageStackIndex--;
                return PageMoveResult.Cancel;
            }
            
                    
            return PageMoveResult.Success;
        }

        /// <summary>
        /// キャッシュしているページオブジェクトを削除
        /// </summary>
        [FrameworkDocument("キャッシュしているページオブジェクトを削除")]
        public void ClearPageObjectCache()
        {
            foreach(KeyValuePair<T, Page> pageObject in pageObjectCache)
            {
                GameObject.Destroy(pageObject.Value.gameObject);
                // ページが破棄された通知
                OnDestroyPageObject(pageObject.Key);
            }
            pageObjectCache.Clear();
        }
        
        /// <summary>
        /// ページスタックの削除
        /// </summary>
        [FrameworkDocument("ページスタックの削除")]
        public void ClearPageStack()
        {
            pageStack.Clear();
            pageStackIndex = 0;
        }
        
        public void AddPageStack(T page, object args)
        {
            // 不要なスタックを削除
            pageStack.RemoveRange(pageStackIndex, pageStack.Count - pageStackIndex);
                
            // 同じページ
            if(pageStack.Count >= 1 && pageStack[pageStack.Count-1].page.Equals(page))
            {
                // 引数を更新
                PageData data = pageStack[pageStack.Count-1];
                data.args = args;
                pageStack[pageStack.Count-1] = data;
            }
            else
            {
                pageStack.Add( new PageData(page, args) );
                pageStackIndex++;
            }
        }
        
        public void RemovePageStack(T page)
        {
            for(int i=pageStack.Count-1; i>=0; i--)
            {
                if(pageStack[i].page.Equals(page))
                {
                    if(pageStackIndex > i) pageStackIndex--;
                    pageStack.RemoveAt(i);
                    break;
                }
            }
        }
        
        public override void ClearAllPageStack()
        {
            ClearPageStack();
            foreach(Page page in pageObjectCache.Values)
            {
                if(page is PageManager m)
                {
                    m.ClearAllPageStack();
                }
            }
        }
        
        /// <summary>
        /// ページオブジェクトの破棄
        /// </summary>
        [FrameworkDocument("ページオブジェクトの破棄")]
        public void DestroyPageObject(T page)
        {
            if(pageObjectCache.TryGetValue(page, out Page pageObject))
            {
                // リストから削除
                pageObjectCache.Remove(page);
                // GameObjectの破棄
                GameObject.Destroy(pageObject.gameObject);
                // ページが破棄された通知
                OnDestroyPageObject(page);
            }
        }
        
        /// <summary>開いているページにメッセージを送る</summary>
        public override async UniTask SendMessageAsync(object value)
        {            
            if(currentPageObject == null)return;
            
            // 子ページにも送る
            if(currentPageObject is PageManager m)
            {
                await m.SendMessageAsync(value);
            }
            
            await currentPageObject.OnSendMessageInternal(value);
        }
        
        internal override async UniTask<bool> OnPreLeaveInternal(CancellationToken token)
        {
            // 結果
            bool result = true;
            // 子ページのチェック
            if(CurrentPageObject != null)
            {
                result &= await CurrentPageObject.OnPreLeaveInternal(token);
            }
            
            result &= await base.OnPreLeaveInternal(token);            
            return result;
        }

        public void OpenPage(string page, bool stack, object args)
        {
            if(System.Enum.TryParse(typeof(T), page, out object pageTypeValue) && pageTypeValue is T pageType)
            {
                OpenPage(pageType, stack, args);
            }
        }
        
        /// <summary>ページを開く</summary>
        [FrameworkDocument("ページを開く")]
        public void OpenPage(T page, bool stack, object args)
        {
            // 一旦削除時のみ止まるように
            OpenPageAsync(page, stack, args, this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        /// <summary>ページを開く</summary>
        private void OpenPage(T page, bool stack, object args, PageTransitionType transitionType)
        {
            // 一旦削除時のみ止まるように
            OpenPageAsync(page, stack, args, transitionType, this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        /// <summary>ページを開く</summary>
        public async UniTask OpenPageAsync(T page, bool stack, object args, CancellationToken token = default)
        {
            await OpenPageAsync(page, stack, args, PageTransitionType.Open, token);
        }
        
        /// <summary>ページを開く</summary>
        protected virtual async UniTask<bool> OpenPageAsync(T page, bool stack, object args, PageTransitionType transitionType, CancellationToken token = default)
        {
            // 同じページを開こうとしてる
            bool isSameOpenPage = false;
            // 同じページに移動しようとした場合
            if(currentPageObject != null && currentPageType.Equals(page))
            {
                isSameOpenPage = true;
            } 
            
            // ロード中のページをセット
            loadingPageType = page;
            
            Page pageObject = null;
            
            if(isSameOpenPage == false)
            {
            
                // 開いているページに閉じる通知
                // ページ遷移がキャンセルされるかチェック
                if(currentPageObject != null)
                {
                    // 離れる前の通知
                    bool leaveResult = await currentPageObject.OnPreLeaveInternal(token);
                    // ページ遷移をキャンセル
                    if(leaveResult == false)
                    {
                        return false;
                    }
                    
                    // ページに閉じる前の通知
                    bool closeResult = await currentPageObject.OnPreCloseInternal(token);
                    // ページ遷移をキャンセル
                    if(closeResult == false)
                    {
                        return false;
                    }
                }
                
                // ページのオブジェクトを生成
                if(pageObjectCache.TryGetValue(page, out pageObject) == false)
                {
                    Page pageTemp = await OnLoadPageResource(page, token);
                    if (pageTemp == null)
                    {
                        return false;
                    }
                    
                    // ページオブジェクトを取得
                    if(IsCreateInstancePage)
                    {
                        pageObject = GameObject.Instantiate<Page>(pageTemp, pageRoot == null ? transform : pageRoot.transform);
                    }
                    else
                    {
                        pageObject = pageTemp;
                    }
                    
                    // マネージャーを登録
                    pageObject.Manager = this;
                    // アクティブは切っておく
                    pageObject.gameObject.SetActive(false);
                    // キャッシュに追加
                    pageObjectCache.Add(page, pageObject);
                }
            }
            else
            {
                pageObject = currentPageObject;
            }
            
            // 暫定対応
            pageObject.transform.SetAsFirstSibling();
            // アクティブはOnにしておく
            pageObject.gameObject.SetActive(true);
            // 表示を隠す
            pageObject.HideImmediate();
            // 引数登録
            pageObject.SetArguments(args);
            // 遷移タイプ
            pageObject.TransitionType = transitionType;

            try
            {
                // ページを開く前の通知
                await pageObject.OnPreOpenInternal(args, token);
            }
            catch(Exception e)
            {
                CruFramework.Logger.LogError(e.ToString());
                if(pageObject != null && pageObject.gameObject != null)
                {
                    pageObject.gameObject.SetActive(false);
                }
                return false;
            }


            // ページを開くている場合は閉じる
            if(currentPageObject != null && isSameOpenPage == false)
            {
                // ページを閉じる前の処理
                await OnPreClosePage(currentPageType, token);
                await currentPageObject.Hide(fadeDuration, token);
                // ページに閉じた通知
                currentPageObject.OnClosedInternal();
                
                // GameObjectの破棄
                if(currentPageObject.IsDestroyOnClosed)
                {
                    // GameObjectの破棄
                    GameObject.Destroy(currentPageObject.gameObject);
                    // ページが破棄された通知
                    OnDestroyPageObject(currentPageType);
                    // キャッシュから削除
                    pageObjectCache.Remove(currentPageType);
                }
                else
                {
                    currentPageObject.gameObject.SetActive(false);
                }
            }
            
            // 現在のページをセット
            currentPageType = page;
            // 現在のページをセット
            currentPageObject = pageObject;
            
            // スタック処理
            if(stack) 
            {
                AddPageStack(page, args);
            }

            // 表示On
            await OnPreOpenPage(currentPageType, token);
            // Enable
            currentPageObject.OnEnablePageInternal(args);
            await currentPageObject.Show(fadeDuration, token);

            // ページを開いた通知
            await OnOpenPage(currentPageType, token);
            
            // 開いた通知
            await currentPageObject.OnOpenInternal(args);
            currentPageObject.OnOpenedInternal(args);
            
            return true;
        }
    }
}