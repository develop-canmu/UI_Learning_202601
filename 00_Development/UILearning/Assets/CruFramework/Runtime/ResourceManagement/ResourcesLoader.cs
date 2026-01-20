#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

using UnityEngine.ResourceManagement.AsyncOperations;
using CruFramework.Addressables;
using UnityEngine.ResourceManagement.Exceptions;
using Exception = System.Exception;

namespace CruFramework.ResourceManagement
{
    [FrameworkDocument("Resources", nameof(ResourcesLoader), "リソース読み込み用クラス。アドレッサブルが有効の場合、ResourcesLoaderのDisposeでハンドルを解放する")]
    public class ResourcesLoader
    {
        private List<AsyncOperationHandle> handleCache = new List<AsyncOperationHandle>();
        private List<string> deleteCacheKeyList = new List<string>();

        /// <summary>
        /// 非同期読み込み
        /// </summary>
        [FrameworkDocument("リソースの非同期読み込み")]
        public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken token = default) where T : UnityEngine.Object
        {
            if(typeof(T).IsSubclassOf(typeof(Component)))
            {
                // コンポーネントの場合はGameObjectを取得
                GameObject go = await AddressablesManager.LoadAssetAsync(
                    () =>
                    {
                        // ハンドル取得
                        AsyncOperationHandle<GameObject> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(key);
                        // ハンドルをキャッシュに追加
                        handleCache.Add(handle);
                        return handle;
                    },
                    null, 
                    token
                );

                // GetComponentして返す
                return go?.GetComponent<T>();
            }
            else 
            {
                T asset = await AddressablesManager.LoadAssetAsync(
                    () =>
                    {
                        // ハンドル取得
                        AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
                        // ハンドルをキャッシュに追加
                        handleCache.Add(handle);
                        return handle;
                    },
                    null,
                    token
                );
                
                return asset;
            }
        }
        
        /// <summary>
        /// 非同期読み込み
        /// キャンセルしても例外を投げない
        /// </summary>
        public async UniTask LoadAssetAsync<T>(string key, Action<T> callback, CancellationToken token = default) where T : UnityEngine.Object
        {
            try
            {
                T asset = await LoadAssetAsync<T>(key, token);
                callback?.Invoke(asset);
            }
            catch(OperationCanceledException)
            {
            }
            catch (AddressablesException)
            {
            }
        }
        
        /// <summary>
        /// 非同期読み込み
        /// Release時に端末からキャッシュを削除
        /// </summary>
        public UniTask<T> LoadAssetAsync<T>(string key, bool deleteCacheOnRelease, CancellationToken token = default) where T : UnityEngine.Object
        {
            if(!deleteCacheKeyList.Contains(key))
            {
                deleteCacheKeyList.Add(key);
            }
            return LoadAssetAsync<T>(key, token);
        }

        /// <summary>
        /// 非同期読み込み
        /// キャンセルしても例外を投げない
        /// Release時に端末からキャッシュを削除
        /// </summary>
        public async UniTask LoadAssetAsync<T>(string key, Action<T> callback, bool deleteCacheOnRelease, CancellationToken token = default) where T : UnityEngine.Object
        {
            try
            {
                T asset = await LoadAssetAsync<T>(key, deleteCacheOnRelease, token);
                callback?.Invoke(asset);
            }
            catch(OperationCanceledException)
            {
            }
            catch (AddressablesException)
            {
            }
        }

        /// <summary>
        /// 同期読み込み
        /// </summary>
        [FrameworkDocument("リソースの同期読み込み")]
        public T LoadAsset<T>(string key) where T : UnityEngine.Object
        {
            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                // コンポーネントの場合はGameObjectでハンドルを取得
                AsyncOperationHandle<GameObject> handle = AddressablesManager.LoadAsset<GameObject>(key);
                // キャッシュに追加
                handleCache.Add(handle);
                // GetComponentして返す
                return handle.Result.GetComponent<T>();
            }
            else
            {
                // ハンドルを取得
                AsyncOperationHandle<T> handle = AddressablesManager.LoadAsset<T>(key);
                // キャッシュに追加
                handleCache.Add(handle);
                // 結果を返す
                return handle.Result;
            }
        }
        
        /// <summary>
        /// 同期読み込み
        /// Release時に端末からキャッシュを削除
        /// </summary>
        public T LoadAsset<T>(string key, bool deleteCacheOnRelease) where T : UnityEngine.Object
        {
            if(!deleteCacheKeyList.Contains(key))
            {
                deleteCacheKeyList.Add(key);
            }
            return LoadAsset<T>(key);
        }

        /// <summary>解放</summary>
        [FrameworkDocument("キャッシュ解放")]
        public void Release()
        {
            if (handleCache != null)
            {
                // ハンドル解放
                foreach (AsyncOperationHandle handle in handleCache)
                {
                    AddressablesManager.ReleaseHandle(handle);
                }
                // キャッシュクリア
                handleCache.Clear();
            }
            
            // 端末から削除
            AddressablesManager.ClearDependencyCacheAsync(new List<string>(deleteCacheKeyList)).Forget();
            deleteCacheKeyList.Clear();
        }
    }
}

#endif
