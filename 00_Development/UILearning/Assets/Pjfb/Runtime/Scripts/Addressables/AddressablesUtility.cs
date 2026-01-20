using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Networking.API;
using CruFramework.Addressables;
using Pjfb.Storage;
using PolyQA;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Pjfb
{
    public class AddressablesUtility : MonoBehaviour
    {
        public static readonly string RemoteAllBundleKey = "remote";
        public static readonly string RemotePreLoadBundleKey = "preload";
        public static readonly string RemoteTutorialBundleKey = "tutorial";
        private const string DynamicResourceLocatorName = "DynamicResourceLocator";
        private static readonly string OverridePlayerVersion = "pjfb";
        
        /// <summary> バンドルの拡張子 </summary>
        private static readonly string BundleExtension = ".bundle";
        
        private static string s_assetVersion = string.Empty;
        /// <summary>アセットバージョン</summary>
        public static string AssetVersion { get { return s_assetVersion; } }
        
        /// <summary>プラットフォーム名</summary>
        private static string Platform
        {
            get
            {
#if UNITY_ANDROID
                return "Android";
#elif UNITY_IOS
                return "iOS";
#else
                return "Android";
#endif

            }
        }
        
        /// <summary>指定バージョンでカタログ更新</summary>
        public static async UniTask<bool> UpdateContentCatalogAsync(CancellationToken token)
        {
            s_assetVersion = Application.version;

            // Addressableを利用しない場合はカタログ更新はスキップ
            if (AddressablesManager.UseAddressable == false)
            {
                return true;
            }
            
            bool isSucceeded = false;
            // カタログのハッシュ情報
            AddressablesManager.CatalogHashInfo catalogHashInfo = null;
            
            while (!isSucceeded)
            {
                
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
                // 強制的にアセットバージョンを変更する
                string forceAssetVersion = PlayerPrefs.GetString("ForceAssetVersion", s_assetVersion);
                if (string.IsNullOrEmpty(forceAssetVersion) == false)
                {
                    // バージョンを上書き
                    s_assetVersion = forceAssetVersion;
                }
                
                int bundleChunkSize = PlayerPrefs.GetInt("bundleChunkSize", 0);
                if (bundleChunkSize > 0)
                {
                    // 分割サイズを上書き
                    AddressablesManager.BundleChunkSize = bundleChunkSize;
                }
#endif

                // カタログパス
                string catalogPath = $"{AppEnvironment.AssetBundleURL}/{Platform}/{AssetVersion}/catalog_{OverridePlayerVersion}.bin";
                
                // カタログのハッシュ情報取得
                catalogHashInfo = await AddressablesManager.GetCatalogHashInfo(catalogPath, token);
                // カタログダウンロード
                isSucceeded = await AddressablesManager.UpdateContentCatalogAsync(catalogPath, token);

                if(!isSucceeded)
                {
                    // 失敗した場合はリトライ処理
                    var isRetry = true;
                    // QAイベント
                    DataSender.Send("AddressableError", "LoadCatalog");
                    
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

            // 失敗の場合タイトルに戻す
            if(!isSucceeded)
            {
                AppManager.Instance.BackToTitle();
            }
            else
            { 
                // 更新が発生するか？(ローカルに保持していたハッシュ値と違う場合も更新)
                bool isUpdateCatalog = catalogHashInfo.IsUpdateRequired() || LocalSaveManager.saveData.catalogHash != catalogHashInfo.RemoteHash;

                // カタログ更新が入った時のみキャッシュを削除する
                if (isUpdateCatalog)
                {
                    // 古いキャッシュ削除
                    await ClearOldCache(catalogHashInfo);

                    string remoteHash = catalogHashInfo.RemoteHash;
                    // 保持しているカタログのハッシュ値を更新
                    LocalSaveManager.saveData.catalogHash = remoteHash;
                    LocalSaveManager.Instance.SaveData();
                }
            }


            return isSucceeded;
        }
        
        /// <summary>端末に保存しているキャッシュを削除</summary>
        public static void ClearCache()
        {
            // カタログ削除
            string catalogPath = AppEnvironment.AddressableCatalogCacheDirectory;
            // ディレクトリが存在している
            if(Directory.Exists(catalogPath))
            {
                // ファイル取得
                string[] files = Directory.GetFiles(catalogPath, "*", SearchOption.AllDirectories);
                // 削除
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            
            // ディレクトリ削除
            if(Directory.Exists(AppEnvironment.AddressableCacheDirectory))
            {
                Directory.Delete(AppEnvironment.AddressableCacheDirectory, true);
            }

            // 保持しているカタログのハッシュ値を初期化
            LocalSaveManager.saveData.catalogHash = string.Empty;
            LocalSaveManager.Instance.SaveData();
        }
        
        /// <summary>古いバージョンのキャッシュを削除</summary>
        public static async UniTask ClearOldCache(AddressablesManager.CatalogHashInfo currentCatalogHashInfo)
        {
            HashSet<string> bundleNameList = new HashSet<string>();
            var allBundleLocation = await GetAllBundleLocationAsync();
            int bundleCount = 0;
            
            // ロケーションマップ取得
            foreach (IResourceLocation location in allBundleLocation)
            {
                // データが存在しない場合は無視
                if(location.Data == null) continue;
                AssetBundleRequestOptions op = (AssetBundleRequestOptions)location.Data;
                // バンドル名キャッシュ
                if(!bundleNameList.Contains(op.BundleName))
                {
                    bundleNameList.Add(op.BundleName);
                }
                // 最新のバージョン以外を削除
                Caching.ClearOtherCachedVersions(op.BundleName, Hash128.Parse(op.Hash));
                bundleCount++;
                // バンドル数に応じて処理負荷分散
                if (bundleCount > AddressablesManager.BundleChunkSize)
                {
                    await UniTask.NextFrame();
                    bundleCount = 0;
                }
            }
            
            if(Directory.Exists(AppEnvironment.AddressableCacheDirectory))
            {
                // サブディレクトリ取得
                string[] directories = Directory.GetDirectories(AppEnvironment.AddressableCacheDirectory, "*", SearchOption.TopDirectoryOnly);
                foreach (string directory in directories)
                {
                    // バンドル名がカタログに存在しないのでディレクトリを削除
                    if(!bundleNameList.Contains(Path.GetFileName(directory)))
                    {
                        Directory.Delete(directory, true);
                    }
                }
            }
            
            // 古いカタログのキャッシュを削除
            string catalogCachePath = AppEnvironment.AddressableCatalogCacheDirectory;
            if (Directory.Exists(catalogCachePath))
            {
                // ファイル取得
                string[] files = Directory.GetFiles(catalogCachePath, "*", SearchOption.AllDirectories);
                // 削除
                foreach (string file in files)
                {
                    // 古いキャッシュファイルは削除
                    if (Path.GetFileNameWithoutExtension(file) != currentCatalogHashInfo.CacheFileName)
                    {
                        File.Delete(file);
                    }
                }
            }
        }
        
        /// <summary> 全バンドルのロケーション位置を返す(分割サイズごとに１フレームで処理するバンドル数を変更する) </summary>
        public static async UniTask<IEnumerable<IResourceLocation>> GetAllBundleLocationAsync()
        {
            // 全ロケーション(IResourceLocation比較クラスを用いて比較する)
            HashSet<IResourceLocation> allLocation = new HashSet<IResourceLocation>(new ResourceLocationComparer());

            // バンドル数
            int bundleCount = 0;
            
            foreach (string key in AddressablesManager.RemoteCatalogLocator.Keys)
            {
                // キー名がバンドルの拡張子以外は無視する(重複をなるべく避けるため)
                if (Path.GetExtension(key) != BundleExtension)
                {
                    continue;
                }

                // バンドル単位でLocationを取得する
                AddressablesManager.RemoteCatalogLocator.Locate(key, typeof(IAssetBundleResource), out IList<IResourceLocation> locations);

                foreach (var l in locations)
                {
                    allLocation.Add(l);
                    bundleCount++;
                }

                // １フレームで処理するバンドル数を超えたら待機(フレーム分割対応)
                if (bundleCount > AddressablesManager.BundleChunkSize)
                {
                    await UniTask.NextFrame();
                    bundleCount = 0;
                }
            }

            return allLocation;
        }
        
        /// <summary>ダウンロード情報取得</summary>
        public static async UniTask<AddressablesManager.DownloadInfo> GetDownloadInfoAsync(string key, CancellationToken token = default)
        {
            // Addressableを利用しないなら空データで返す
            if (AddressablesManager.UseAddressable == false)
            {
                return new AddressablesManager.DownloadInfo(0, new List<IResourceLocation>());
            }

            AddressablesManager.DownloadInfo downloadInfo = await AppManager.Instance.LoadingActionAsync(async () => await AddressablesManager.GetDownloadInfoAsync(key, token));
            
            // デバック用のログを流す
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
            ShowDownloadBundleInfo(downloadInfo);
#endif
            return downloadInfo;
        }
        
        /// <summary>ダウンロード情報取得</summary>
        public static async UniTask<AddressablesManager.DownloadInfo> GetDownloadSizeAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            return await AppManager.Instance.LoadingActionAsync(async () => await AddressablesManager.GetDownloadInfoAsync(keys, token));
        }

        /// <summary>アセットダウンロード</summary>
        public static async UniTask<bool> DownloadAsset(string key, CancellationToken token = default)
        {
            // DL一覧を取得
            AddressablesManager.DownloadInfo downloadInfo = await GetDownloadInfoAsync(key, token);
            var keys = downloadInfo.GetBundleKeyList();
            return await DownloadAsset(keys, token);
        }
        
        /// <summary>アセットダウンロード</summary>
        public static async UniTask<bool> DownloadAsset(IEnumerable<string> keys, CancellationToken token = default)
        {
            bool result = false;
            // 進捗バー表示
            AppManager.Instance.UIManager.System.Progress.Show();
            try
            {
                // DL
                result = await AddressablesManager.DownloadDependenciesAsync(keys, p => AppManager.Instance.UIManager.System.Progress.SetProgress(p), token);
            }
            catch
            {
                throw;
            }
            // 進捗バー非表示
            AppManager.Instance.UIManager.System.Progress.Hide();
            return result;
        }

#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT
        /// <summary> ダウンロード情報をログに表示 </summary>
        public static void ShowDownloadBundleInfo(AddressablesManager.DownloadInfo downloadInfo)
        {
            // ログ表示を行うか
            bool isShow = PlayerPrefs.GetInt("showBundleDownLoadInfo", 0) == 1;
            if (isShow == false)
            {
                return;
            }
            
            // APIと同じ文字数で表示上限を設ける
            int charLimit = 10000;
            
            // StringBuilderのindex
            int sbIndex = 0;
            Dictionary<int, System.Text.StringBuilder> builderDict = new();
            // 最初のStringBuilderを追加
            builderDict.Add(sbIndex, new System.Text.StringBuilder(charLimit));
            
            // 現在の文字数
            int currentCharCount = 0;
            foreach (IResourceLocation location in downloadInfo.BundleLocationList)
            {
                long size = 0;
                // ここで来ているのはバンドルのダウンロード確定してるロケーションなので直でバンドルサイズ追加する(キャッシュ確認はコストそこそこあるので)
                if (location.Data is AssetBundleRequestOptions options)
                {
                    size = options.BundleSize;
                }

                // 追加先
                System.Text.StringBuilder sb = null;
                // バンドルのキーとサイズをログに流す
                string text = $"{location.PrimaryKey} : {size / 1024 / 1024:F2}MB";
                // 文字数を計算
                int charCount = text.Length;
                
                if (charCount + currentCharCount < charLimit)
                {
                    sb = builderDict[sbIndex];
                    currentCharCount += charCount;
                }
                else
                {
                    // index更新
                    sbIndex++;
                    // 文字数リセット
                    currentCharCount = charCount;
                    // 新規作成して追加
                    sb =  new System.Text.StringBuilder(charLimit);
                    builderDict.Add(sbIndex, sb);
                }
                
                sb.AppendLine(text);
            }
            
            int totalCount = builderDict.Count;
            // ログ出力
            for (int i = 0; i < totalCount; i++)
            {
                System.Text.StringBuilder sb = builderDict[i];
                CruFramework.Logger.Log($"更新対象のアセットバンドル（{i + 1} / {totalCount}, ダウンロードバンドル数 : {downloadInfo.BundleLocationList.Count()}）\n{sb.ToString()}");
            }
        }

        public static void ShowExistOldCacheBundle()
        {
            ShowExistOldCacheBundleAsync().Forget();
        }
        
        /// <summary> 古いキャッシュバンドルが残ってないか表示 </summary>
        public static async UniTask ShowExistOldCacheBundleAsync()
        {
            Dictionary<string, CachedAssetBundle> cachedAssetBundles = new Dictionary<string, CachedAssetBundle>();
            // ロケーションマップ取得
            foreach (IResourceLocation location in AddressablesManager.RemoteCatalogLocator.AllLocations)
            {
                // データが存在しない場合は無視
                if (location.Data == null) continue;
                AssetBundleRequestOptions op = (AssetBundleRequestOptions)location.Data;
                CachedAssetBundle cachedAssetBundle = new CachedAssetBundle(op.BundleName, Hash128.Parse(op.Hash));
                // バンドル名キャッシュ
                cachedAssetBundles.Add(op.BundleName, cachedAssetBundle);
            }

            // 現在のバージョン以外のバージョンがキャッシュされているか
            bool existOtherVersion = false;
            List<Hash128> bundleHashList = new List<Hash128>();
            foreach (CachedAssetBundle bundle in cachedAssetBundles.Values)
            {
                bundleHashList.Clear();
                Caching.GetCachedVersions(bundle.name, bundleHashList);
                // 現在のバージョン以外のバンドルキャッシュ数
                int otherCacheVersionCount = bundleHashList.Count(x => x != bundle.hash);
                if (otherCacheVersionCount <= 0) continue;
                existOtherVersion = true;
                Caching.ClearOtherCachedVersions(bundle.name, bundle.hash);
                CruFramework.Logger.LogError($"バンドルキャッシュ {bundle.name}に現在のバージョン以外のキャッシュ{otherCacheVersionCount}個を削除しました");
            }

            // 不要なバージョンキャッシュなし
            if (existOtherVersion == false)
            {
                CruFramework.Logger.Log("現在のバージョン以外のバンドルキャッシュはありませんでした");
            }

            // 参照が存在しないバンドルキャッシュがないか確認する
            if (Directory.Exists(AppEnvironment.AddressableCacheDirectory))
            {
                // 使用してないバンドルキャッシュが存在するか
                bool existNotUseBundle = false;
                // サブディレクトリ取得(バンドル名のフォルダ内にHash値でフォルダがある)
                string[] directories = Directory.GetDirectories(AppEnvironment.AddressableCacheDirectory, "*", SearchOption.TopDirectoryOnly);
                foreach (string directory in directories)
                {
                    // バンドル名がカタログに存在しないのでディレクトリを削除
                    if (cachedAssetBundles.ContainsKey(Path.GetFileName(directory)) == false)
                    {
                        existNotUseBundle = true;
                        Directory.Delete(directory, true);
                        CruFramework.Logger.LogError($"参照が存在しないバンドルキャッシュ:{directory}を削除しました");
                    }
                }

                if (existNotUseBundle == false)
                {
                    CruFramework.Logger.Log("参照が存在しないバンドルキャッシュはありませんでした");
                }
            }
            
            // カタログのキャッシュ状況をチェック
            if (Directory.Exists(AppEnvironment.AddressableCatalogCacheDirectory))
            {
                // ロード指定パス
                string catalogPath = AddressablesManager.RemoteCatalogLocator.LocatorId;
                AddressablesManager.CatalogHashInfo info = await AddressablesManager.GetCatalogHashInfo(catalogPath, default);
                string[] files = Directory.GetFiles(AppEnvironment.AddressableCatalogCacheDirectory, "*", SearchOption.AllDirectories);

                // 使用しないカタログのキャッシュが存在するか？
                bool existNotUseCatalogCache = false;
                
                foreach (string file in files)
                {
                    // カタログのキャッシュファイルと一致しないカタログファイルは削除
                    if (Path.GetFileNameWithoutExtension(file) != info.CacheFileName)
                    {
                        existNotUseCatalogCache = true;
                        File.Delete(file);
                        CruFramework.Logger.LogError($"使用されていないカタログキャッシュ:{file}を削除しました");
                    }
                }
                
                if (existNotUseCatalogCache == false)
                {
                    CruFramework.Logger.Log("使用されていないカタログキャッシュはありませんでした");
                }
            }
        }
#endif


        public static async UniTask ConfirmAndDownloadAsset(AddressablesManager.DownloadInfo downloadInfo, string windowTitle, string windowMessage,  Func<UniTask> completeTask, Func<UniTask> cancelTask, bool needTouchGuard, CancellationToken token = default)
        {
            bool completed = false;
            long downloadSizeMB = Mathf.CeilToInt(downloadInfo.DownloadSize / 1024f / 1024f);
            
            // 確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = windowTitle;
            // 端末の容量確認
            if (StorageCheckUtility.AvailableStorageCheck(downloadInfo.DownloadSize))
            {
                // 容量が足りていればダウンロード確認    
                // メッセージ
                data.Message = string.Format(windowMessage, downloadSizeMB);
                // 注意
                data.Caution = StringValueAssetLoader.Instance["title.asset_all_download_caution"];
                // 決定ボタン
                data.PositiveButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.decision"],
                    async window =>
                    {
                        AppManager.Instance.UIManager.ModalManager.gameObject.SetActive(false);
                    
                        if(needTouchGuard)
                        {
                            AppManager.Instance.UIManager.System.AddressableTouchGuard.Show();
                        }
                        
                        // スリープさせない
                        Screen.sleepTimeout = SleepTimeout.NeverSleep;

                        bool success = false;
                        try
                        {
                            // Download
                            success = await DownloadAsset(downloadInfo.GetBundleKeyList(), token);

                        }
                        catch (Exception e)
                        {
                            // エラーログだけ出す
                            Debug.LogError(e);
                        }
                        finally
                        {
                            // スリープを端末設定に戻す
                            Screen.sleepTimeout = SleepTimeout.SystemSetting;
                        }
                        
                        // 失敗した場合はタイトルに戻るので処理しない
                        if(!success) return;
                    
                        if(needTouchGuard)
                        {
                            AppManager.Instance.UIManager.System.AddressableTouchGuard.Hide();
                        }
                    
                        AppManager.Instance.UIManager.ModalManager.gameObject.SetActive(true);
                    
                        await UniTask.Lazy(completeTask);

                        completed = true;
                    }
                );
            }
            else
            {
                data.Message =
                    string.Format(StringValueAssetLoader.Instance["title.storage_shortage"], downloadSizeMB);
            }

            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.close"],
                async window => 
                {
                    window.CloseAsync().Forget();
                    
                    await UniTask.Lazy(cancelTask);

                    completed = true;
                }
            );
            // 確認モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
            // 完了もしくはキャンセルまで待機
            await UniTask.WaitUntil(() => completed);
        }

        /// <summary>必須アセットダウンロードのまとめ関数</summary>
        public static async UniTask<bool> EssentialAssetDownloadAsync(AddressablesManager.DownloadInfo downloadInfo, CancellationToken token)
        {
            bool isAssetDownloadCanceled = false;
            if (downloadInfo.DownloadSize > 0)
            {
                // アセットダウンロード
                await ConfirmAndDownloadAsset(downloadInfo,
                    StringValueAssetLoader.Instance["title.asset_download"],
                    StringValueAssetLoader.Instance["title.asset_download_confirm"],
                    () =>
                    {
                        AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                        return UniTask.CompletedTask;
                    },
                    () =>
                    {
                        isAssetDownloadCanceled = true;
                        AppManager.Instance.BackToTitle();
                        return UniTask.CompletedTask;
                    },
                    false,
                    token
                );
            }

            // アセットダウンロードがキャンセルされた場合は失敗として返す
            if (isAssetDownloadCanceled) return false;

            // アセットダウンロード後に文字列を更新する
            await OverwriteValueAsset(token);

            return true;
        }
        
        /// <summary>リモートアセットで文字列を更新する</summary>
        private static async UniTask OverwriteValueAsset(CancellationToken token)
        {
            // 文字列定義上書き
            await PageResourceLoadUtility.LoadAssetAsync<StringValueAsset>(StringValueAssetLoader.RemoteAddress,
                asset =>
                {
                    StringValueAssetLoader.Instance.OverwriteValues(asset);
                },
                token
            );

            // カラー定義上書き
            await PageResourceLoadUtility.LoadAssetAsync<ColorValueAsset>(ColorValueAssetLoader.RemoteAddress,
                asset =>
                {
                    ColorValueAssetLoader.Instance.OverwriteValues(asset);
                },
                token
            );

            // フォントサイズ定義上書き
            await PageResourceLoadUtility.LoadAssetAsync<FontSizeValueAsset>(FontSizeValueAssetLoader.RemoteAddress,
                asset =>
                {
                    FontSizeValueAssetLoader.Instance.OverwriteValues(asset);
                },
                token
            );
        }
        
        private static bool isErrorModalAlreadyOpen = false; 
        private static AddressablesManager.OperationResult operationResult;
        
        public static async UniTask<AddressablesManager.OperationResult> OnHandleError(AddressablesManager.ErrorCode errorCode, AddressablesManager.OperationType operationType)
        {
            // アセットロード時のみタッチガード表示
            if(operationType == AddressablesManager.OperationType.LoadAsset)
            {
                AppManager.Instance.UIManager.System.AddressableTouchGuard.Show();
                AppManager.Instance.UIManager.System.Loading.Show();
            }
            
            operationResult = AddressablesManager.OperationResult.Cancel;
            
            // モーダルが開かれている場合
            if(isErrorModalAlreadyOpen)
            {
                // モーダルの結果待ち
                await UniTask.WaitUntil(() => !isErrorModalAlreadyOpen);
            }
            else
            {
                if(errorCode != AddressablesManager.ErrorCode.InvalidKey) 
                { 
                    // フラグ立てる
                    isErrorModalAlreadyOpen = true;
                    
                    // APIが接続中の場合は待機
                    await UniTask.WaitUntil(() => !APIManager.Instance.isConnecting);

                    // QAイベント
                    DataSender.Send("AddressableError", operationType.ToString());
                    // モーダル表示
                    ConfirmModalData data = new ConfirmModalData();
                    data.Title = StringValueAssetLoader.Instance["common.confirm"];
                    data.Message = StringValueAssetLoader.Instance["asset.dl_retry"];
                    data.PositiveButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.retry"], window => 
                    {
                        operationResult = AddressablesManager.OperationResult.Continue;
                        window.Close();
                    });
                    data.NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], window => 
                    {
                        operationResult = AddressablesManager.OperationResult.Cancel;
                        window.Close();
                    });
                    var window = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
                    // 閉じるまで待機
                    await window.WaitCloseAsync();
                    
                    // フラグおろす
                    isErrorModalAlreadyOpen = false;
                    
                    // タイトルに戻る
                    if(operationResult == AddressablesManager.OperationResult.Cancel)
                    {
                        AppManager.Instance.UIManager.System.AddressableTouchGuard.HideForce();
                        AppManager.Instance.UIManager.System.Loading.HideForce();
                        AppManager.Instance.BackToTitle();
                    }
                }
            }

            return operationResult;
        }
    }
}
