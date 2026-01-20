#if CRUFRAMEWORK_ADDRESSABLE_SUPPORT

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.Exceptions;

namespace CruFramework.Addressables
{
    [FrameworkDocument("Resources", nameof(AddressablesManager), "アセットバンドル管理用クラス")]
    public static class AddressablesManager
    {
        public enum ErrorCode
        {
            Unknown,
            InvalidKey,
            DownloadError,
        }
        
        public enum OperationType
        {
            LoadAsset,
            Download,
        }
        
        public enum OperationResult
        {
            Continue,
            ThrowException,
            Cancel,
        }

        /// <summary> ダウンロード情報 </summary>
        public class DownloadInfo
        {
            // ダウンロードサイズ
            private long downloadSize = 0;
            public long DownloadSize => downloadSize;
            
            // ダウンロードされるバンドルのロケーションリスト
            private IEnumerable<IResourceLocation> bundleLocationList = null;
            public IEnumerable<IResourceLocation> BundleLocationList => bundleLocationList;

            /// <summary> バンドルのダウンロードに使用するキーリストを取得 </summary>
            public IEnumerable<string> GetBundleKeyList() => bundleLocationList.Select(x => x.PrimaryKey).ToArray();
            
            public DownloadInfo(long downloadSize, IEnumerable<IResourceLocation> bundleLocationList)
            {
                this.downloadSize = downloadSize;
                this.bundleLocationList = bundleLocationList;
            }
        }

        /// <summary> カタログハッシュ情報 </summary>
        public class CatalogHashInfo
        {
            // カタログパス
            private string catalogPath = string.Empty;
            public string CatalogPath => catalogPath;

            // キャッシュファイル名
            private string cacheFileName = string.Empty;
            public string CacheFileName => cacheFileName;
            
            // リモートハッシュ値
            private string remoteHash = string.Empty;
            public string RemoteHash => remoteHash;

            // キャッシュ済みのハッシュ値
            private string cachedHash = string.Empty;
            public string CachedHash => cachedHash;

            public CatalogHashInfo(string catalogPath, string cacheFileName, string remoteHash, string cachedHash)
            {
                this.catalogPath = catalogPath;
                this.cacheFileName = cacheFileName;
                this.remoteHash = remoteHash;
                this.cachedHash = cachedHash;
            }

            /// <summary> カタログの更新が必要か？ </summary>
            public bool IsUpdateRequired()
            {
                // リモートとハッシュ値が違う場合は更新が必要
                return remoteHash != cachedHash;
            }
        }
        
        private const string DynamicResourceLocatorName = "DynamicResourceLocator";
        
        private static List<AsyncOperationHandle> handleCache = new List<AsyncOperationHandle>();
       
        private static IResourceLocator remoteCatalogLocator;
        public static IResourceLocator RemoteCatalogLocator => remoteCatalogLocator; 
        
        public static ResourceManager ResourceManager { get { return UnityEngine.AddressableAssets.Addressables.ResourceManager; } }

        /// <summary> Addressableを利用するモードか？ </summary>
        private static bool useAddressable = true;
        public static bool UseAddressable => useAddressable;
        
        public static event Action<int> OnCompleteEvent;
        public static event Func<ErrorCode, OperationType, UniTask<OperationResult>> OnErrorEvent;

        /// <summary> 一度に実行されるバンドルの数(処理負荷分割のための設定値) </summary>
        public static int BundleChunkSize = 1000;
        
        /// <summary>
        /// https://docs.unity3d.com/Packages/com.unity.addressables@1.19/manual/TransformInternalId.html
        /// </summary>
        public static Func<IResourceLocation, string> InternalIdTransformFunc
        {
            set { UnityEngine.AddressableAssets.Addressables.InternalIdTransformFunc = value; }
        }

        /// <summary> Addressableを利用するかセット </summary>
        public static void SetUseAddressable(bool isUse)
        {
            // Editor以外は常に利用する
#if UNITY_EDITOR
            useAddressable = isUse;
#else
            useAddressable = true;
#endif
        }

        /// <summary>システムの初期化</summary>
        public async static UniTask InitializeAsync(CancellationToken token = default)
        {
            AsyncOperationHandle handle = UnityEngine.AddressableAssets.Addressables.InitializeAsync();
            // 待機
            await WaitHandle(handle, null, token);
        }

        /// <summary>カタログの追加</summary>
        public async static UniTask<bool> UpdateContentCatalogAsync(string catalogPath, CancellationToken token = default)
        {
            // 失敗した時に戻せるように現在登録されてるResourceLocatorを保持しておく
            IResourceLocator[] lastLocatorCache = UnityEngine.AddressableAssets.Addressables.ResourceLocators.ToArray();
            
            // ResourceLocatorを一旦破棄
            UnityEngine.AddressableAssets.Addressables.ClearResourceLocators();

            // 再読み込み時に解放するため自動解放しない
            CruFramework.Logger.Log("UpdateContentCatalogAsync : " + catalogPath);
           
            AsyncOperationHandle<IResourceLocator> handle = LoadContentCatalogAsync(catalogPath, false);

            try
            {
                // 読み込み待機
                await WaitHandle(handle);

                remoteCatalogLocator = handle.Result;
                return true;
            }
            catch
            {
                // 失敗したのでキャッシュから状態を復元
                UnityEngine.AddressableAssets.Addressables.ClearResourceLocators();
                foreach (IResourceLocator locator in lastLocatorCache)
                {
                    UnityEngine.AddressableAssets.Addressables.AddResourceLocator(locator);
                }

                return false;
            }
            finally
            {
                // ハンドルを破棄
                ReleaseHandle(handle);
            }
        }

        /// <summary> カタログのハッシュ情報を取得 </summary>
        public static async UniTask<CatalogHashInfo> GetCatalogHashInfo(string catalogPath, CancellationToken token)
        {
            // カタログのロケーション情報を構築
            var catalogLocation = UnityEngine.AddressableAssets.Addressables.CreateCatalogLocationWithHashDependencies<ContentCatalogProvider>(catalogPath);
            // 依存関係にHash値取得処理が入っているので処理を作成
            var op = UnityEngine.AddressableAssets.Addressables.ResourceManager.CreateGroupOperation<string>(catalogLocation.Dependencies);
            // 処理が完了するまで待機
            await WaitHandle(op, token : token);
            // リモートのハッシュ値
            string remoteHash = (string)op.Result[(int)ContentCatalogProvider.DependencyHashIndex.Remote].Result;
            // キャッシュされているハッシュ値(キャッシュされていない場合は空)
            string cacheHash = (string)op.Result[(int)ContentCatalogProvider.DependencyHashIndex.Cache].Result;
            // キャッシュされるカタログファイルの名前(カタログのパス名から決まってるっぽい)
            string cacheFileName = Path.GetFileNameWithoutExtension(catalogLocation.Dependencies[(int)ContentCatalogProvider.DependencyHashIndex.Cache].InternalId);
            ReleaseHandle(op);
            
            CruFramework.Logger.Log($"remoteHash {remoteHash} , cacheHash {cacheHash} , cacheFileName : {cacheFileName}");
            return new CatalogHashInfo(catalogPath, cacheFileName, remoteHash, cacheHash);
        }

        /// <summary> カタログの読み込み(既存関数だとカタログのキャッシュが利用されないので) </summary>
        public static AsyncOperationHandle<IResourceLocator> LoadContentCatalogAsync(string path, bool autoReleaseHandle)
        {
            // 初期化後に勝手に設定書き換えられるけど設定戻んないので戻す
            ContentCatalogProvider provider = GetProvider<ContentCatalogProvider>();
            provider.DisableCatalogUpdateOnStart = false;
            AsyncOperationHandle<IResourceLocator> handle = UnityEngine.AddressableAssets.Addressables.LoadContentCatalogAsync(path, autoReleaseHandle);
            return handle;
        }

        /// <summary>リソースの読み込み</summary>
        [FrameworkDocument("リソースの非同期読み込み。取得したハンドルを解放する必要がある")]
        public static AsyncOperationHandle<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
            // キャッシュに追加
            handleCache.Add(handle);
            return handle;
        }
        
        internal static async UniTask<T> LoadAssetAsync<T>(Func<AsyncOperationHandle<T>> loadAssetAsyncFunc, Action<float> progress, CancellationToken token = default) where T : UnityEngine.Object
        {
            int errorCount = 0;
            while (true)
            {
                // ハンドルを取得
                AsyncOperationHandle handle = loadAssetAsyncFunc();
                try
                {
                    // キャッシュに追加
                    handleCache.Add(handle);
                    // 完了まで待機
                    await WaitHandle(handle, progress, token);
                    // 完了通知
                    OnCompleteEvent?.Invoke(errorCount);
                    // キャストして返す
                    return (T)handle.Result;
                }
                catch(OperationCanceledException)
                {
                    OnCompleteEvent?.Invoke(errorCount);
                    throw;
                }
                catch (AddressablesException e)
                {
                    // ハンドルを解放する
                    ReleaseHandle(handle);
                    OperationResult operationResult = OperationResult.Cancel;
                    // エラーイベントの呼び出し
                    if(OnErrorEvent != null)
                    {
                        errorCount++;
                        operationResult = await OnErrorEvent(e.ErrorCode, OperationType.LoadAsset);
                    }
                    
                    // キャンセル
                    if(operationResult == OperationResult.Cancel)
                    {
                        // イベント通知
                        OnCompleteEvent?.Invoke(errorCount);
                        break;
                    }
                    
                    // 例外を投げる
                    if(operationResult == OperationResult.ThrowException)
                    {
                        // イベント通知
                        OnCompleteEvent?.Invoke(errorCount);
                        throw;
                    }
                }
            }
            return default;
        }

        /// <summary>リソースの読み込み</summary>
        [FrameworkDocument("リソースの同期読み込み。取得したハンドルを解放する必要がある")]
        public static AsyncOperationHandle<T> LoadAsset<T>(string key) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = LoadAssetAsync<T>(key);
            // 同期読み
            handle.WaitForCompletion();
            return handle;
        }

        /// <summary> ダウンロード情報取得 </summary>
        public static async UniTask<DownloadInfo> GetDownloadInfoAsync(string key, CancellationToken token = default)
        {
            return await GetDownloadInfoAsync(new[] { key }, token);
        }
        
        public static async UniTask<DownloadInfo> GetDownloadInfoAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            // Addressable未使用時は空のデータで渡す
            if (UseAddressable == false)
            {
                return new DownloadInfo(0, Array.Empty<IResourceLocation>());
            }
            
            // バンドルのLocation比較クラスを用いてHashListを作成
            HashSet<IResourceLocation> allBundleLocation = new HashSet<IResourceLocation>(new ResourceLocationComparer());
            
            foreach (string key in keys)
            {
                // ラベルに紐づくLocation取得(ここは処理分割したいけど内部で一気に紐づくLocation取得するので重い。。)
                bool isExist = RemoteCatalogLocator.Locate(key, null, out IList<IResourceLocation> locations);

                // ロケーションが無いなら無視
                if (isExist == false)
                {
                    continue;
                }

                foreach (var location in locations)
                {
                    // バンドル自体のロケーションは依存関係に含まれているので追加
                    if (location.HasDependencies)
                    {
                        foreach (IResourceLocation depLocation in location.Dependencies)
                        {
                            allBundleLocation.Add(depLocation);
                        }
                    }
                }
            }

            int bundleCount = 0;
            // ダウンロードするバンドルロケーションリスト
            List<IResourceLocation> downloadBundleLocationList = new List<IResourceLocation>();
            // ダウンロードサイズ取得
            long size = 0;
            foreach (var location in allBundleLocation)
            {
                // バンドルのサイズ計算
                if (location.Data is ILocationSizeData sizeData)
                {
                    long bundleSize = sizeData.ComputeSize(location, ResourceManager);
                    if (bundleSize > 0)
                    {
                        size += bundleSize;
                        downloadBundleLocationList.Add(location);
                        bundleCount++;
                    }
                }
                
                // 負荷分散の為に分割処理数ごとにフレームを分ける(１フレームでの計算コストが高すぎると固まるので)
                if (bundleCount > BundleChunkSize)
                {
                    await UniTask.NextFrame(token);
                    bundleCount = 0;
                }
            }

            return new DownloadInfo(size, downloadBundleLocationList);
        }

        /// <summary>ダウンロードサイズ取得</summary>
        [FrameworkDocument("ダウンロードサイズ取得")]
        public static async UniTask<long> GetDownloadSizeAsync(string key, CancellationToken token = default)
        {
            DownloadInfo downloadInfo = await GetDownloadInfoAsync(key, token);
            return downloadInfo.DownloadSize;
        }
        
         /// <summary>ダウンロードサイズ取得</summary>
        public static async UniTask<long> GetDownloadSizeAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            DownloadInfo downloadInfo = await GetDownloadInfoAsync(keys, token);
            return downloadInfo.DownloadSize;
        }

        /// <summary>ダウンロード</summary>
        public static UniTask<bool> DownloadDependenciesAsync(string key, Action<float> progress = null, CancellationToken token = default)
        {
            return DownloadDependenciesAsync(() => UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync(key), progress, token);
        }
        
        /// <summary>ダウンロード</summary>
        public static UniTask<bool> DownloadDependenciesAsync(IEnumerable keys, Action<float> progress = null, CancellationToken token = default)
        {
            return DownloadDependenciesAsync(() => UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync(keys, UnityEngine.AddressableAssets.Addressables.MergeMode.Union), progress, token);
        }
        
        private static async UniTask<bool> DownloadDependenciesAsync(Func<AsyncOperationHandle> downloadDependenciesAsyncFunc, Action<float> progress, CancellationToken token)
        {
            int errorCount = 0;
            while (true)
            {
                // ハンドルを取得
                AsyncOperationHandle handle = downloadDependenciesAsyncFunc();
                try
                {
                    // 完了まで待機
                    await WaitHandle(handle, progress, token);
                    // ハンドル解放
                    ReleaseHandle(handle);
                    // 完了通知
                    OnCompleteEvent?.Invoke(errorCount);
                    
                    return true;
                }
                catch(OperationCanceledException)
                {
                    OnCompleteEvent?.Invoke(errorCount);
                    throw;
                }
                catch (AddressablesException e)
                {
                    // ハンドル解放
                    ReleaseHandle(handle);
                    
                    OperationResult operationResult = OperationResult.Cancel;
                    // エラーイベントの呼び出し
                    if(OnErrorEvent != null)
                    {
                        errorCount++;
                        operationResult = await OnErrorEvent(e.ErrorCode, OperationType.Download);
                    }
                    
                    if(operationResult == OperationResult.Cancel)
                    {
                        // イベント通知
                        OnCompleteEvent?.Invoke(errorCount);
                        break;
                    }
                    
                    // 例外を投げる
                    if(operationResult == OperationResult.ThrowException)
                    {
                        // イベント通知
                        OnCompleteEvent?.Invoke(errorCount);
                        throw;
                    }
                }
            }
            return false;
        }
        
        /// <summary> 特定のProviderを取得 </summary>
        public static T GetProvider<T>() where T : class, IResourceProvider
        {
            foreach (IResourceProvider provider in ResourceManager.ResourceProviders)
            {
                if (provider is T result)
                {
                    return result;
                }
            }

            return null;
        }
        
        /// <summary>ハンドル待機</summary>
        [FrameworkDocument("ハンドルが利用可能になるまで待機する")]
        public static async UniTask WaitHandle(AsyncOperationHandle handle, Action<float> onProgress = null, CancellationToken token = default)
        {
            while ( !handle.IsDone )
            {
                // 進捗
                onProgress?.Invoke(handle.GetDownloadStatus().Percent);
                await UniTask.DelayFrame(1);
                
                if(token.IsCancellationRequested)
                {
                    throw new OperationCanceledException($"{handle.DebugName} is canceled.");
                }
            }

            CheckHandleError(handle);
        }
        
        public static void CheckHandleError(AsyncOperationHandle handle)
        {
            if(!handle.IsValid()) return;
            // 失敗した
            if(handle.Status == AsyncOperationStatus.Failed)
            {
                // 例外取得
                Exception exception = handle.OperationException;
                while (exception != null)
                {
                    switch (exception)
                    {
                        case RemoteProviderException remoteProviderException:
                            throw new AddressablesException(ErrorCode.DownloadError, remoteProviderException.Message, remoteProviderException.InnerException);
                        case InvalidKeyException invalidKeyException:
                            throw new AddressablesException(ErrorCode.InvalidKey, invalidKeyException.Message, invalidKeyException.InnerException); 
                    }
                    
                    exception = exception.InnerException;
                }
                throw new AddressablesException(ErrorCode.Unknown, "Addressable Unkown Error.", null);
            }
        }
        
        /// <summary>
        /// Check if resource of specified type exist in addressable resources.
        /// </summary>
        /// <param name="key">Path to search.</param>
        /// <typeparam name="T">Type of asset to be searched.</typeparam>
        /// <returns>Return true if asset exist.</returns>
        public static bool HasResources<T>(string key) {
            foreach (var l in UnityEngine.AddressableAssets.Addressables.ResourceLocators) {
                IList<IResourceLocation> locs;
                if (l.Locate(key, typeof(T), out locs))
                    return true;
            }
            return false;
        }

        /// <summary>特定のキャッシュを端末から削除</summary>
        [FrameworkDocument("指定したキャッシュを端末から削除する")]
        public async static UniTask<bool> ClearDependencyCacheAsync(string key, CancellationToken token = default)
        {
            AsyncOperationHandle<bool> handle = UnityEngine.AddressableAssets.Addressables.ClearDependencyCacheAsync(key, false);
            // 待機
            await WaitHandle(handle, null, token);
            // 結果取得
            bool success = handle.Result;
            // ハンドル解放
            ReleaseHandle(handle);
            return success;
        }
        
        /// <summary>特定のキャッシュを端末から削除</summary>
        public async static UniTask<bool> ClearDependencyCacheAsync(IEnumerable keys, CancellationToken token = default)
        {
            AsyncOperationHandle<bool> handle = UnityEngine.AddressableAssets.Addressables.ClearDependencyCacheAsync(keys, false);
            // 待機
            await WaitHandle(handle, null, token);
            // 結果取得
            bool success = handle.Result;
            // ハンドル解放
            ReleaseHandle(handle);
            return success;
        }
        
        public async static UniTask<List<string>> GetResourceLocationPrimaryKeyList(string key, CancellationToken token = default)
        {
            List<string> addressList = new List<string>();
            AsyncOperationHandle<IList<IResourceLocation>> handle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(key);
            await WaitHandle(handle, null, token);
            foreach (IResourceLocation location in handle.Result)
            {
                addressList.Add(location.PrimaryKey);
            }
            UnityEngine.AddressableAssets.Addressables.Release(handle);
            return addressList;
        }

        /// <summary>ハンドルを解放</summary>
        [FrameworkDocument("ハンドルを解放する")]
        public static void ReleaseHandle(AsyncOperationHandle handle)
        {
            // ハンドル解放
            if (handle.IsValid())
            {
                UnityEngine.AddressableAssets.Addressables.Release(handle);
            }

            // キャッシュにある場合は削除
            if (handleCache.Contains(handle))
            {
                handleCache.Remove(handle);
            }
        }

        /// <summary>全てのハンドルを解放</summary>
        public static void ReleaseAllHandle()
        {
            foreach (AsyncOperationHandle handle in handleCache)
            {
                // ハンドル解放
                if (handle.IsValid())
                {
                    UnityEngine.AddressableAssets.Addressables.Release(handle);
                }
            }
            // キャッシュクリア
            handleCache.Clear();
        }
    }
}

#endif