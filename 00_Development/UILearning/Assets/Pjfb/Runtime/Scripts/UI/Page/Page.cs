using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.ResourceManagement;
using PolyQA;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Pjfb
{
    public class Page : CruFramework.Page.Page
    {
        [SerializeField]
        private BGM bgmType = BGM.bgm_home;
        
        [SerializeField]
        private bool ignorePlayBGM = false;
        
        [SerializeField]
        protected RectTransform backKeyObject = null;
        /// <summary>バックキーオブジェクト</summary>
        public RectTransform BackKeyObject { get { return backKeyObject; } }

        /// <summary>ページからリソースを取得する場合はこの関数を通す</summary>
        protected async UniTask LoadAssetAsync<T>(string key, Action<T> callback, CancellationToken token) where T : UnityEngine.Object
        {
            await PageResourceLoadUtility.LoadAssetAsync<T>(key, callback, token);
        }
        
        /// <summary>ページからリソースを取得する場合はこの関数を通す</summary>
        protected async UniTask LoadAssetAsync<T>(string key, Action<T> callback) where T : UnityEngine.Object
        {
            await PageResourceLoadUtility.LoadAssetAsync<T>(key, callback, this.GetCancellationTokenOnDestroy());
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            if(!ignorePlayBGM)
            {
                PlayBGMAsync().Forget();
            }
        }

        public virtual async UniTask PlayBGMAsync()
        {
            await BGMManager.PlayBGMAsync(bgmType);
        }

        protected virtual void OnDestroy()
        {
        }

        protected override UniTask OnOpen(object args)
        {
            // QAイベント
            DataSender.Send("Page", gameObject.name.Replace("(Clone)", "").Trim());
            return base.OnOpen(args);
        }
    }
}
