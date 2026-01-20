using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.Audio;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public enum BGM
    {
        None = 0,
        bgm_home = 1,
        bgm_gacha_top = 2,
        bgm_gacha_result = 3,
        bgm_trainning = 4,
        bgm_trainning_result = 5,
        bgm_gvg_top = 6,
        bgm_title = 7,
        bgm_game_01 = 8,
        bgm_ingame_clubroyal = 9,
    }

    public static class BGMManager
    {
        private static CancellationTokenSource cancelSource = null;
        private static ResourcesLoader resourcesLoader = new ResourcesLoader();
        
        /// <summary>ファイル名取得</summary>
        private static string GetFileName(BGM bgm)
        {
            string fileName = null;
            if(bgm != BGM.None)
            {
                fileName = bgm.ToString();
            }
            return fileName;
        }
        
        /// <summary>アドレス取得</summary>
        public static string GetAddress(string fileName)
        {
            return ResourcePathManager.GetPath("BGM", fileName); 
        }

        /// <summary>BGM再生</summary>
        public static async UniTask PlayBGMAsync(BGM bgm, bool forcePlay = false)
        {
            if(bgm == BGM.None)
            {
                // 停止
                AudioManager.Instance.Stop(AudioGroup.BGM);
                // 読み込みキャンセル
                Cancel();
                return;
            }
            
            // ファイル名取得
            string fileName = GetFileName(bgm);
            // BGM再生
            await PlayBGMAsync(fileName, forcePlay);
        }
        
        /// <summary>BGM再生</summary>
        public static async UniTask PlayBGMAsync(string fileName, bool forcePlay = false)
        {
            if(string.IsNullOrEmpty(fileName)) return;
            
            // 読み込みキャンセル
            Cancel();
            // トークン生成
            cancelSource = new CancellationTokenSource();
            // アドレス取得
            string address = GetAddress(fileName);
            
            try
            {
                // クリップ取得
                AudioClip clip = await resourcesLoader.LoadAssetAsync<AudioClip>(address, cancelSource.Token);
                // BGM再生
                AudioManager.Instance.PlayBGM(clip, 1.0f, forcePlay);
            }
            catch (OperationCanceledException)
            {
            }
        }
        
        /// <summary>キャンセル</summary>
        private static void Cancel()
        {
            if(cancelSource != null)
            {
                cancelSource.Cancel();
                cancelSource.Dispose();
                cancelSource = null;
            }
        }
        
        /// <summary>解放</summary>
        public static void Release()
        {
            // BGM止める
            AudioManager.Instance.Stop(AudioGroup.BGM);
            // 読み込みキャンセル
            Cancel();
            // ハンドル解放
            resourcesLoader.Release();
        }
    }
}