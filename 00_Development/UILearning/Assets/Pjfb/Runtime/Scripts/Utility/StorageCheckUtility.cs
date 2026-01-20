using System;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


namespace Pjfb
{
    public class StorageCheckUtility
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern long _GetAvailableStorage();
#endif
        
        /// <summary>空き容量が足りているかを判定する</summary>
        public static bool AvailableStorageCheck(long downloadSize)
        {
            // 空き容量チェック
            long availableStorage = GetAvailableStorage();
            
            // 容量が足りていないならfalseを返す
            if (availableStorage > -1 && downloadSize > availableStorage)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>端末の空き容量を取得</summary>
        private static long GetAvailableStorage()
        {
#if UNITY_EDITOR
            return -1;
#elif UNITY_ANDROID
            var statFs = new AndroidJavaObject("android.os.StatFs", Application.temporaryCachePath);
            var availableBlocks = statFs.Call<long>("getAvailableBlocksLong");
            var blockSize = statFs.Call<long>("getBlockSizeLong");
            var freeBytes = availableBlocks * blockSize;
            return freeBytes;
#elif UNITY_IOS
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return _GetAvailableStorage();
            }
            throw new System.NotSupportedException();
#else
            return -1;
#endif
        }
    }
}