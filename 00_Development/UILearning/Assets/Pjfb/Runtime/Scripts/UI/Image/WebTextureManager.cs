using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Logger = CruFramework.Logger;
using CodeStage.AntiCheat.Storage;

namespace Pjfb
{
    public static class WebTextureManager
    {
        private static List<string> checkModified = new List<string>();
        private static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();
        // ロード中のリスト
        private static Dictionary<string, UniTaskCompletionSource<Texture>> loadingList = new Dictionary<string, UniTaskCompletionSource<Texture>>();
        // エラー表示用テクスチャ
        private static Texture2D errorTexture = null;
        
        private static readonly string SaveRoot = Path.Combine(Application.persistentDataPath, "ImageCache");
        private static readonly int expireDay = 7;
        private const string CacheLastCheckDate = "CacheLastCheckDate";
        private static DateTime CacheLastCheckDatePlayerPrefs
        {
            get
            {
                var playerPrefsValue = ObscuredPrefs.Get<string>(key: CacheLastCheckDate, defaultValue: String.Empty);
                return long.TryParse(playerPrefsValue, out var ticks) ? new DateTime(ticks: ticks) : DateTime.MinValue;
            }
            set => ObscuredPrefs.Set<string>(key: CacheLastCheckDate, value.Ticks.ToString());
        }
        
        /// <summary>テクスチャ取得</summary>
        public static async UniTask<Texture> GetTextureAsync(string url, CancellationToken token = default)
        {
            Uri u = new Uri(url);
            var savePath = SaveRoot + u.AbsolutePath;
            
            // 新規タスクの場合は、ダウンロード処理を実行
            Texture result = null;
            bool isCanceled = false;
            UniTaskCompletionSource<Texture> loadTaskSource = null;
            
            // ロード中のタスクがないかチェック
            try
            {
                // ロード中タスクの待機
                var (loadTask, loadingTaskResult) = await WaitLoadingTask(url, token);
                loadTaskSource = loadTask;
                
                // 既存のタスクが完了していた場合は、その結果をそのまま返す
                if (loadingTaskResult != null)
                {
                    return loadingTaskResult;
                }
            }
            catch (Exception)
            {
                return GetErrorTexture();
            }
            
            // ダウンロード処理
            try
            {
                if (!File.Exists(savePath))
                {
                    await DownloadImage(url, savePath, token);
                }
                else
                {
                    if (!checkModified.Contains(url))
                    {
                        await CheckLastModified(url, savePath, token);
                    }
                }

                // キャッシュにないなら追加する
                if (cache.TryGetValue(url, out result) == false)
                {
                    result = GetTexture(savePath);
                    // 取得出来た場合だけキャッシュに追加
                    if (result != null)
                    {
                        cache.Add(url, result);
                    }
                    // テクスチャを取得出来なかった場合はエラーテクスチャを返す
                    else
                    {
                        result = GetErrorTexture();
                    }
                }
            }
            // キャンセル時
            catch (OperationCanceledException e)
            {
                isCanceled = true;
                Logger.Log($"Cancel DownLoad WebTexture \n {e.Message}");
                // キャンセルされたことを待機中のタスクに通知
                loadTaskSource.TrySetCanceled();
                // 即時完了するがawaitする(awaitされないとメモリに残るので)
                await loadTaskSource.Task;
            }
            // エラー発生時
            catch (Exception e)
            {
                Logger.LogError($"Error DownLoad WebTexture return ErrorTexture \n {e.Message}");
                // エラー時はエラー用のテクスチャを返す
                result = GetErrorTexture();
            }
            finally
            {
                // キャンセル時は結果は返さないので実行しない
                if (isCanceled == false)
                {
                    // 取得したテクスチャーを待機中のタスクに渡す
                    loadTaskSource.TrySetResult(result);
                    // 即時完了するがawaitする(awaitされないとメモリに残るので)
                    await loadTaskSource.Task;
                }
                // 完了後はロードタスクから削除
                loadingList.Remove(url);
            }

            return result;
        }
        
        /// <summary>ロード中タスクの待機(まだ無いなら新規タスクを作成)</summary>
        private static async UniTask<(UniTaskCompletionSource<Texture> taskSource, Texture completedTexture)> WaitLoadingTask(string url, CancellationToken token)
        {
            while (true)
            {
                // すでに同じリソースの読み込み実行中ならロードが完了するまで待機
                if (loadingList.TryGetValue(url, out UniTaskCompletionSource<Texture> loadingTask))
                {
                    Texture result = null;
                    try
                    {
                        result = await loadingTask.Task.AttachExternalCancellation(token);
                    }
                    // ロード中のタスクがキャンセルされた
                    catch (OperationCanceledException)
                    {
                        // 自身がキャンセルされた場合はエラーテクスチャを返す
                        if (token.IsCancellationRequested)
                        {
                            result = GetErrorTexture();
                        }
                        // ロードタスクがキャンセルされた場合はnullを返す
                        else
                        {
                           result = null;
                        }
                    }
                    // エラー発生時はエラーテクスチャを返す
                    catch (Exception)
                    {
                        result = GetErrorTexture();
                    }
                    
                    // 結果がnullの場合は再試行（他のタスクがキャンセルされた）
                    if (result == null)
                    {
                        // 次のフレームで再度チェック
                        await UniTask.NextFrame(token);
                        continue;
                    }
                    
                    // 既存タスクが完了した場合は、その結果を返す
                    return (null, result);
                }
                // ロード中のタスクがまだないなら新規タスクを作成
                else
                {
                    var newTaskSource = new UniTaskCompletionSource<Texture>();
                    loadingList.Add(url, newTaskSource);
                    return (newTaskSource, null);
                }
            }
        }

        /// <summary>画像ダウンロード</summary>
        public static async UniTask DownloadImage(string url, string savePath, CancellationToken token = default)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                try
                {
                    // 画像取得
                    request.downloadHandler = new DownloadHandlerFile(savePath);
                    await request.SendWebRequest().ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    Logger.Log($"DownLoad Cancel WebTexture URL : {url}");
                    request.Abort();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.LogError("DownLoad Error WebTexture URL : " + url);
                    Logger.LogError(e.Message);
                    throw;
                }
                // 作成日の変更
                string lastModified = request.GetResponseHeader("last-modified");
                if (lastModified != null)
                {
                    DateTime writeTime = DateTime.Parse(lastModified);
                    File.SetLastWriteTime(savePath, writeTime);
                }
                
                // 作成日のチェック済みに
                if (!checkModified.Contains(url))
                {
                    checkModified.Add(url);
                }
            }
        }

        /// <summary>最終更新日取得</summary>
        public static async UniTask CheckLastModified(string url, string savePath, CancellationToken token = default)
        {
            using (UnityWebRequest request = UnityWebRequest.Head(url))
            {
                try
                {
                    // 更新日取得
                    // Logger.Log("CheckLastModified:" + url);
                    await request.SendWebRequest().ToUniTask(cancellationToken: token);
                }
                catch (OperationCanceledException e)
                {
                    Logger.Log($"CheckLastModify Cancel WebTexture URL : {url} \n {e.Message}");
                    request.Abort();
                    throw;
                }
                catch (Exception e)
                {
                    Logger.LogError($"CheckLastModify Error WebTexture URL : {url}");
                    Logger.LogError(e.Message);
                    throw;
                }

                // 作成日が変わってたら再ダウンロード
                string lastModified = request.GetResponseHeader("last-modified");
                if (lastModified != null)
                {
                    DateTime writeTime = DateTime.Parse(lastModified);
                    DateTime oldWriteTime = File.GetLastWriteTime(savePath);
                    if (writeTime != oldWriteTime)
                    {
                        await DownloadImage(url, savePath, token);
                    }
                }

                if (!checkModified.Contains(url))
                {
                    checkModified.Add(url);
                }
            }
        }
        
        public static void Release()
        {
            ClearCache(expireDay);
        }

        /// <summary>テクスチャの取得</summary>
        private static Texture GetTexture(string savePath)
        {
            if (!File.Exists(savePath)) return null;

            var texture = new Texture2D(1, 1, TextureFormat.ASTC_6x6, false);
            bool isSuccess = texture.LoadImage(File.ReadAllBytes(savePath));
            // 読み込みに失敗した場合は削除しnullを返す
            if (isSuccess == false)
            {
                Logger.LogError("Failed Load Image");
                File.Delete(savePath);
                UnityEngine.Object.Destroy(texture);
                return null;
            }
            return texture;
        }

        /// <summary> エラー用テクスチャ取得 </summary>
        private static Texture GetErrorTexture()
        {
            if (errorTexture == null)
            {
                // お知らせバナーがテクスチャのサイズから領域セットしてるので見えやすいサイズで作成
                errorTexture = new Texture2D(300, 300);
            }
            return errorTexture;
        }
        
        /// <summary>指定したディレクトリとその中身を全て削除する</summary>
        public static void DeleteCaches(string targetDirectoryPath, int expireDay = 0)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }
            TimeSpan ts = AppTime.Now - CacheLastCheckDatePlayerPrefs;
            if (expireDay > 0 && ts.TotalDays <= 1)
            {
                return;
            }
            else 
            {
                CacheLastCheckDatePlayerPrefs = AppTime.Now;
            }

            //ディレクトリ以外の全ファイルを削除
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(targetDirectoryPath);
            foreach (System.IO.FileInfo file in di.GetFiles())
            {
                if (file.IsReadOnly == false) {
                    try
                    {
                        if (expireDay > 0)
                        {
                            var diff = AppTime.Now - file.LastAccessTime;
                            if (diff.TotalDays >= expireDay)
                            {
                                file.Delete();
                            }
                        }
                        else 
                        {
                            file.Delete();
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteCaches(directoryPath, expireDay);
            }
        }

        /// <summary>キャッシュクリア</summary>
        public static void ClearCache(int expireDay = 0)
        {
            // 明示的に破棄する(アンマネージドリソースなのでメモリリーク対策ように)
            foreach (Texture texture in cache.Values)
            {
                UnityEngine.Object.Destroy(texture);
            }
            // エラー用テクスチャ破棄
            UnityEngine.Object.Destroy(errorTexture);
            errorTexture = null;
            
            // キャッシュクリア
            cache.Clear();
            loadingList.Clear();
            checkModified.Clear();
            DeleteCaches(SaveRoot, expireDay);
        }
    }
}