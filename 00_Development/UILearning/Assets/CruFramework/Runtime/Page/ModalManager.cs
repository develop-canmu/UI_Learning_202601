using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Utils;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PolyQA;

namespace CruFramework.Page
{
    
    public enum ModalOptions
    {
        None = 0,
        /// <summary>最前面のモーダルを維持したまま開く</summary>
        KeepFrontModal = 1 << 0,
        /// <summary>新しいモーダルが開いても閉じない</summary>
        DontClose = 1 << 1,
    }
    
    public abstract class ModalManager : MonoBehaviour
    {
        protected struct ModalData
        {
            public ModalWindow modalWindow;
            
            public ModalData(ModalWindow modalWindow)
            {
                this.modalWindow = modalWindow;
            }
        }
        
        [SerializeField]
        protected GameObject modalBackground = null;
        [SerializeField]
        protected GameObject modalRoot = null;
        
        protected int allocateModalId = 0;
        // 開いているモーダル
        protected Dictionary<int, ModalData> modalList = new Dictionary<int, ModalData>();

        protected List<int> modalStack = new List<int>();
        /// <summary>モーダルのスタック</summary>
        public IReadOnlyList<int> ModalStack{get{return modalStack;}}
        
        

        /// <summary>モーダルが閉じたときの通知</summary>
        internal void OnCloseModal(ModalWindow modalWindow, Action OnCompleted)
        {
            CloseModalAsync(modalWindow, OnCompleted, this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        /// <summary>モーダルが閉じたときの通知</summary>
        internal UniTask OnCloseModalAsync(ModalWindow modalWindow)
        {
            return CloseModalAsync(modalWindow, null, this.GetCancellationTokenOnDestroy());
        }
        
        /// <summary>モーダルウィンドウを取得</summary>
        public ModalWindow GetModalWindow(int id)
        {
            if(modalList.TryGetValue(id, out ModalData data))
            {
                return data.modalWindow;
            }
            return null;
        }

        /// <summary>一番上のモーダル取得</summary>
        [FrameworkDocument("一番上のモーダル取得")]
        public ModalWindow GetTopModalWindow()
        {
            // モーダルなし
            if(modalStack.Count <= 0)return null;
            // 一番上のモーダル
            int id = modalStack[modalStack.Count-1];
            return modalList[id].modalWindow;
        }
        
        /// <summary>一番上のモーダルを閉じる</summary>
        [FrameworkDocument("一番上のモーダルを閉じる")]
        public void CloseTopModalWindow()
        {
            ModalWindow topModalWindow = GetTopModalWindow();
            // モーダルなし
            if(topModalWindow == null)return;
            // 閉じる
            topModalWindow.Close();
        }

        /// <summary>スタックから削除する</summary>
        private void RemoveStack(ModalWindow modalWindow)
        {
            if (modalWindow == null)
            {
                return;
            }
            // リストから削除
            modalList.Remove(modalWindow.ModalId);
            modalStack.Remove(modalWindow.ModalId);
            // 削除
            GameObject.Destroy(modalWindow.gameObject);
        }
        
        private void CheckModalList()
        {
            // すべて閉じた
            if(modalList.Count <= 0)
            {
                // 背景を閉じる
                if(modalBackground != null)
                {
                    modalBackground.gameObject.SetActive(false);
                }

                // 最後のモーダルが閉じた通知
                OnCloseLastModalWindow();
            }
        }
        
        /// <summary>モーダルウィンドウを閉じて破棄する</summary>
        public virtual async UniTask CloseModalAsync(ModalWindow modalWindow, Action OnCompleted, CancellationToken token)
        {
            // 一番上のモーダル？
            bool isTopModal = GetTopModalWindow() == modalWindow;
            // 閉じる通知
            await modalWindow.OnCloseInternal(token);
            // スタックから削除
            RemoveStack(modalWindow);
            
            // 一番上のモーダルを閉じた場合は次のモーダルを開く
            if(isTopModal)
            {
                if( (modalWindow.Options & ModalOptions.KeepFrontModal) == ModalOptions.None)
                {
                    ModalWindow topModalWindow = GetTopModalWindow();
                    if(topModalWindow != null)
                    {
                        topModalWindow.gameObject.SetActive(true);
                        await topModalWindow.OnActiveInternal(token);
                    }
                }
            }
            
            // モーダルリストのチェック
            CheckModalList();
            
            // 閉じたときの通知
            if(OnCompleted != null)
            {
                OnCompleted();
            }
        }
        
        /// <summary>手前のモーダルをスタックから消す。一番上のモーダルは除く</summary>
        [FrameworkDocument("一番上のモーダルを除く手前のモーダルをスタックから消す。全て閉じるか条件がfalseになると終了")]
        public void RemoveTopModalsIgnoreTop(Func<ModalWindow, bool> getCloseFlag)
        {
            RemoveTopModals(getCloseFlag, 1);
        }
        
        /// <summary>手前のモーダルをスタックから消す</summary>
        [FrameworkDocument("手前のモーダルをスタックから消す。全て閉じるか条件がfalseになると終了")]
        public void RemoveTopModals(Func<ModalWindow, bool> getCloseFlag, int ignoreCount = 0)
        {
            IReadOnlyList<int> ids = ModalStack;
            for(int i = ids.Count - 1 - ignoreCount;i >= 0;i--)
            {
                // 一番手前のモーダルを取得
                ModalWindow modal = GetModalWindow(ids[i]);
                // 取得失敗
                if(modal == null)break;
                // 閉じるかチェック
                bool isClose = getCloseFlag(modal);
                // 閉じる場合
                if(isClose)
                {
                    RemoveStack(modal);
                }
                else
                {
                    break;
                }
            }
                        
            // モーダルリストのチェック
            CheckModalList();
        }

        [FrameworkDocument("モーダルをスタックから消す。全てチェックしてtrueのものを削除")]
        public void RemoveModals(Func<ModalWindow, bool> getCloseFlag)
        {
            IReadOnlyList<int> ids = ModalStack;
            for(int i = ids.Count-1;i >= 0;i--)
            {
                // モーダルを取得
                ModalWindow modal = GetModalWindow(ids[i]);
                // 取得失敗
                if(modal == null){
                    continue;
                }
                // 閉じるかチェック
                bool isClose = getCloseFlag(modal);
                // 閉じる場合
                if(isClose)
                {
                    RemoveStack(modal);
                }
            }
                        
            // モーダルリストのチェック
            CheckModalList();
        }

        /// <summary>最後のモーダルが閉じた時の通知</summary>
        protected virtual void OnCloseLastModalWindow()
        {
        }

        public bool IsModalOpened<T>() where T : ModalWindow
        {
            foreach (var modalData in modalList.Values)
            {
                var component = modalData.modalWindow.GetComponent<T>();
                if (component != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    [FrameworkDocument("Page", nameof(ModalManager), "モーダル管理用クラス")]
    public abstract class ModalManager<T> : ModalManager where T : System.Enum
    {

        private bool isRunOpen = false;
        public bool IsRunOpen => isRunOpen;

        /// <summary>ページの読み込み</summary>
        protected abstract UniTask<ModalWindow> OnLoadModalResource(T modal, CancellationToken token);
        
        public void CloseAllModalWindow()
        {
            RemoveTopModals((m)=>true);
            isRunOpen = false;
        }
        
        /// <summary>モーダルウィンドウを開く</summary>
        [FrameworkDocument("モーダルウィンドウを開く")]
        public void OpenModal(T modal, object args, ModalOptions options = ModalOptions.None)
        {
            OpenModalAsync(modal, args, this.GetCancellationTokenOnDestroy(), options).Forget();
        }
        
        /// <summary>モーダルウィンドウを開く</summary>
        public UniTask<ModalWindow> OpenModalAsync(T modal, object args, ModalOptions options = ModalOptions.None)
        {
            return OpenModalAsync(modal, args, this.GetCancellationTokenOnDestroy(), options);
        }
       
        /// <summary>モーダルウィンドウを開く</summary>
        public virtual async UniTask<ModalWindow> OpenModalAsync(T modal, object args, CancellationToken token, ModalOptions options = ModalOptions.None)
        {
            // 同時に開く処理は許容しない
            await UniTask.WaitWhile(()=>isRunOpen);
            // 開く処理中に
            isRunOpen = true;
            
            // 既にモーダルを開いている場合は閉じる
            if( (options & ModalOptions.KeepFrontModal) == ModalOptions.None)
            {
                ModalWindow topModalWindow = GetTopModalWindow();
                if(topModalWindow != null) 
                {
                    if( (topModalWindow.Options & ModalOptions.DontClose) == ModalOptions.None)
                    {
                        // 閉じる通知
                        await topModalWindow.OnCloseInternal(token);
                        // アクティブを切る
                        topModalWindow.gameObject.SetActive(false);
                    }
                }
            }

            // Modalの生成
            ModalWindow modalTemp = await OnLoadModalResource(modal, token);
            ModalWindow modalWindow = GameObject.Instantiate<ModalWindow>(modalTemp, modalRoot == null ? transform : modalRoot.transform);
            // Id
            int id = ++allocateModalId;
            modalWindow.Manager = this;
            modalWindow.Options = options;
            modalWindow.SetId(id);
            modalWindow.SetArguments(args);
            
            // リストに追加
            modalList.Add(id, new ModalData(modalWindow) );
            // リストに追加
            modalStack.Add(id);
            
            // 背景をアクティブ化
            if(modalBackground != null)
            {
                modalBackground.SetActive(true);
            }
            
            try
            {
                // パラメータ渡す
                await modalWindow.OnPreOpenInternal(args, token);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                if(modalWindow != null)
                {
                    modalWindow.OnErrorInternal(e);
                    await modalWindow.CloseAsync();
                }
                // 開く処理終わり
                isRunOpen = false;
                return modalWindow;
            }

            // アクティブ化
            modalWindow.gameObject.SetActive(true);
            // QAイベント
            DataSender.Send("Modal", modalWindow.name.Replace("(Clone)", "").Trim());
            // モーダルに通知
            await modalWindow.OnActiveInternal(token);
            
            // 開く処理終わり
            isRunOpen = false;
            // 開いたモーダルを返す
            return modalWindow;
        }
    }
}