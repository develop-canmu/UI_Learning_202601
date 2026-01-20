using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.API;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.HTTP;
using Pjfb.Storage;
using CruFramework.Extensions;
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
using Pjfb.DebugMenu;
#endif

namespace Pjfb.Master
{

    [CruFramework.FrameworkDocument("Master", nameof(MasterManager), "マスタ管理クラス")]
    public partial class MasterManager : CruFramework.Utils.Singleton<MasterManager>
    {
        class MasterVersionInfo
        {
            public long serverMasterVer = 0; // サーバー側のマスタバージョン
            public long size = 0; // マスタデータの通算サイズ量
            public string cdnPath = ""; // CDNのドメイン + マスタデータ格納ディレクトリのベース（fileListと結合して使用）
            public string[] fileList = null; // マスタデータの各ファイルパス
            public MasterVersionInfo(MasterDownloadCheckFileAPIResponse version)
            {
                serverMasterVer = version.serverMasterVer;
                size = version.size;
                cdnPath = version.cdnPath;
                fileList = version.fileList;
            }

            public MasterVersionInfo(VersionGetVariousVerAPIResponse version)
            {
                serverMasterVer = version.serverMasterVer;
                size = version.masterSize;
                cdnPath = version.cdnPath;
                fileList = version.masterFileList;
            }
        }


        const string MasterDataDirectory = "MasterDataMsg";
        const string MasterDataDirectoryJson = "MasterData";
        const string saveIV = @"R2qSWfyUPd3bFpM8";
        const string savePassKey = @"Fa7SieQHPf9m5LCjLy7iz2mTF6nLZQkJ";


        public Dictionary<string, IMasterContainer> containers { private set; get; }

        protected override void Init()
        {
#if UNITY_EDITOR
            // LocalMasterEditor用にEditorだけ読み込む
            ReloadMaster();
#endif
        }

        /// <summary>
        /// データ保存ディレクトリのパス作成
        /// </summary>
        [CruFramework.FrameworkDocument("Masterが保存されているPathを返します")]
        static public string CreateDataDirectoryPath(string masterDirectoryName = MasterDataDirectory)
        {
            var directoryName = Pjfb.Storage.IO.CreateHashFileName(masterDirectoryName);
            var directoryPath = Pjfb.Storage.IO.AppendPersistentPath(directoryName);
            return directoryPath;
        }

        /// <summary>
        /// 最新のマスタダウンロード
        /// </summary>
        /// <returns></returns>
        [CruFramework.FrameworkDocument("最新のMasterをダウンロードします。不要な場合はSkipします")]
        public async UniTask DownloadMaster()
        {
            //マスタ存在チェック
            var directoryPath = CreateDataDirectoryPath();
            if (!Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                //存在しない場合はversionを０にする
                LocalSaveManager.saveData.masterVersion = 0;
                LocalSaveManager.Instance.SaveData();
            }

            var checkAPIResponse = await ConnectCheckAPI();
            var versionInfo = new MasterVersionInfo(checkAPIResponse);
            if (!IsNeedVersionUp(versionInfo))
            {
                ReloadMaster();
                return;
            }

            await UpdateLocalMasterData(versionInfo);
        }

        /// <summary>
        /// 最新のマスタダウンロード
        /// </summary>
        /// <returns></returns>
        [CruFramework.FrameworkDocument("最新のMasterをダウンロードします。不要な場合はSkipします")]
        public async UniTask DownloadMaster(VersionGetVariousVerAPIResponse version)
        {
            //マスタ存在チェック
            var directoryPath = CreateDataDirectoryPath();
            if (!Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                //存在しない場合はversionを０にする
                LocalSaveManager.saveData.masterVersion = 0;
                LocalSaveManager.Instance.SaveData();
            }

            var versionInfo = new MasterVersionInfo(version);
            if (!IsNeedVersionUp(versionInfo))
            {
                ReloadMaster();
                return;
            }

            await UpdateLocalMasterData(versionInfo);
        }

        /// <summary>
        /// tableNameからContainerを取得
        /// </summary>
        [CruFramework.FrameworkDocument("tableNameからContainerを取得します。")]
        public IMasterContainer FindContainer(string tableName)
        {

            if (!containers.ContainsKey(tableName))
            {
                CruFramework.Logger.LogWarning("[Master] not find masterContainer : " + tableName);
                return null;
            }

            return containers[tableName];
        }


        /// <summary>
        /// マスタの削除
        /// </summary>
        /// <returns></returns>
        [CruFramework.FrameworkDocument("マスタを全て削除します")]
        public void DeleteMaster()
        {
            LocalSaveManager.saveData.masterVersion = 0;
            LocalSaveManager.Instance.SaveData();
            
            // Json形式の旧マスタが残っていたら削除
            string directoryPath = CreateDataDirectoryPath(MasterDataDirectoryJson);
            if (Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                Pjfb.Storage.IO.DeleteDirectory(directoryPath);
            }
            
            directoryPath = CreateDataDirectoryPath(MasterDataDirectory);
            if (!Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                return;
            }

            Pjfb.Storage.IO.DeleteDirectory(directoryPath);

            ReloadMaster();
        }


        /// <summary>
        /// マスタ更新チェックAPI
        /// </summary>
        async UniTask<MasterDownloadCheckFileAPIResponse> ConnectCheckAPI()
        {
            var request = new MasterDownloadCheckFileAPIRequest();
            var post = new MasterDownloadCheckFileAPIPost();
            post.currentMasterVer = LocalSaveManager.saveData.masterVersion;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }

        /// <summary>
        /// マスタダウンロードAPI
        /// </summary>
        async UniTask<MasterDownloadExecuteAPIRequest> ConnectDownloadAPI()
        {
            var request = new MasterDownloadExecuteAPIRequest();
            var post = new MasterDownloadExecuteAPIPost();
            post.currentMasterVer = LocalSaveManager.saveData.masterVersion;
            request.SetPostData(post);
            return await APIManager.Instance.Connect(request);
        }

        /// <summary>
        /// ローカルのマスタ更新
        /// </summary>
        /// <param name="checkAPIResponse"></param>
        async UniTask UpdateLocalMasterData(MasterVersionInfo checkAPIResponse)
        {

            ReloadMaster();

            foreach (var file in checkAPIResponse.fileList)
            {
                // メモリ効率改善のため、ここで解放しておく
                GC.Collect();
                
                var url = checkAPIResponse.cdnPath + file;
#if PJFB_ENABLE_API_LOG
                CruFramework.Logger.Log("[Master] Download Master : " + url);
#endif
                using var httpConnector = new HTTPConnector(url, HTTPMethod.Get);
                httpConnector.timeoutSecond = APIManager.Instance.setting.timeoutSecond;
                var httpResponse = await httpConnector.Connect();
                if (httpResponse.statusCode != 200)
                {
                    throw new MasterException("[Master] download Master file Error");
                }

                try
                {
                    var json = Pjfb.Storage.IO.DecompressGZipToString(httpResponse.data);
                    var masterRoot = JsonUtility.FromJson<MasterDeserializeRoot>(json);
                    var dataDictionaryList = masterRoot.CreateDataDictionaryList();

                    foreach (var dataDictionary in dataDictionaryList)
                    {
                        if (dataDictionary.Value == null || dataDictionary.Value.Count <= 0)
                        {
                            continue;
                        }

                        var container = FindContainer(dataDictionary.Key);
                        if (container == null)
                        {
                            continue;
                        }

                        CruFramework.Logger.Log("[Master] update Master : " + dataDictionary.Key);
                        foreach (var masterObject in dataDictionary.Value)
                        {
                            var masterValueObject = masterObject as IMasterValueObject;
                            if (!masterValueObject._deleteFlg)
                            {
                                container.DeleteLocalData(masterValueObject);
                            }
                            else
                            {
                                container.UpdateLocalData(masterValueObject);
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    throw new MasterException("[Master] Master update Error : " + e.Message);
                }

            }
            
            try
            {
                await SaveMaster();
                LocalSaveManager.saveData.masterVersion = checkAPIResponse.serverMasterVer;
                LocalSaveManager.Instance.SaveData();

            }
            catch (System.Exception e)
            {
                throw new MasterException("[Master] local save Error : " + e.Message);
            }

            await UniTask.NextFrame();
        }

        /// <summary>
        /// バージョンアップが必要か
        /// </summary>
        /// <param name="apiResponse"></param>
        bool IsNeedVersionUp(MasterVersionInfo versionInfo)
        {
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            // 強制削除フラグが立っていればtrueで返す
            if (DebugMenuGeneral.IsForceDeleteLocalMaster)
            {
                return true;
            }
#endif
            
            var directoryPath = CreateDataDirectoryPath();
            if (!Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                return true;
            }

            if (versionInfo.serverMasterVer != LocalSaveManager.saveData.masterVersion)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// マスタロード
        /// </summary>
        T LoadAndAddMaster<T>() where T : IMasterContainer, new()
        {
            var directoryPath = CreateDataDirectoryPath();
            var container = new T();
            container.LoadMaster(directoryPath, saveIV, savePassKey);
            containers.Add(container.masterName, container);
            return container;
        }

        /// <summary>
        /// 全てのマスタ保存
        /// </summary>
        async UniTask SaveMaster()
        {
            var directoryPath = CreateDataDirectory();
            foreach (var pair in containers)
            {
                pair.Value.SaveMaster(directoryPath, saveIV, savePassKey);
                await UniTask.NextFrame();
            }
        }

        /// <summary>
        /// データディレクトリの作成
        /// </summary>
        /// <returns></returns>
        string CreateDataDirectory()
        {
            var directoryPath = CreateDataDirectoryPath();
            if (Pjfb.Storage.IO.ExistsDirectory(directoryPath))
            {
                return directoryPath;
            }

            Pjfb.Storage.IO.CreateDirectory(directoryPath);
#if UNITY_IOS
            // iCloudの保存対象外にする
            UnityEngine.iOS.Device.SetNoBackupFlag(directoryPath);
#endif

            return directoryPath;
        }


        void ReloadMaster()
        {
            containers = new Dictionary<string, IMasterContainer>();
            try
            {
                LoadMaster();
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
                // フラグオンかつローカルマスタが存在する場合はエラーを起こす
                if (DebugMenuGeneral.IsForceDeleteLocalMaster && Pjfb.Storage.IO.ExistsDirectory(CreateDataDirectoryPath()))
                {
                    throw new System.Exception();   
                }
#endif
            }
            catch (System.Exception e)
            {
                // マスタの読み込みに失敗した場合は、ローカルマスタを削除
                CruFramework.Logger.LogError("[Master] load error : " + e.Message);
                DeleteMaster();
            }
        }
    }
}