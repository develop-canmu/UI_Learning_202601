using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.Addressables;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace Pjfb
{
    #if UNITY_EDITOR
    /// <summary> Addressable初期化時に用いる設定 </summary>
    [CreateAssetMenu(fileName = "PjfbInitializationSettings.asset", menuName = "Addressables/Initialization/Pjfb Initialization Settings")]
    public class AddressableInitializationCustomSettings :  ScriptableObject, IObjectInitializationDataProvider
    {
        [SerializeField]
        private AddressableInitialization.SettingData data = null;
        
        public ObjectInitializationData CreateObjectInitializationData()
        {
            return  ObjectInitializationData.CreateSerializedInitializationData<AddressableInitialization>(nameof(AddressableInitialization), data);
        }

        /// <summary> Inspector表示名 </summary>
        public string Name => "Pjfb Custom Settings";
    }
    #endif
    
    [Serializable]
    public class AddressableInitialization : IInitializableObject
    {
        [Serializable]
        public class SettingData
        {
            // 一度に実行するバンドル数(塊の数ごとに1フレームで処理する)
            [SerializeField]
            private int bundleChunkSize = 1000;
            public int BundleChunkSize => bundleChunkSize;
        }
        
        public bool Initialize(string id, string data)
        {
            try
            {
                SettingData setting = JsonUtility.FromJson<SettingData>(data);
                // データをセット
                AddressablesManager.BundleChunkSize = setting.BundleChunkSize;
                CruFramework.Logger.Log($"バンドル分割数:{setting.BundleChunkSize}で初期化しました");
            }
            catch (Exception)
            {
                return false;
            }
            
            return true;
        }

        public AsyncOperationHandle<bool> InitializeAsync(ResourceManager rm, string id, string data)
        {
            bool result = Initialize(id, data);
            return AddressablesManager.ResourceManager.CreateCompletedOperation(result, "");
        }
    }
}