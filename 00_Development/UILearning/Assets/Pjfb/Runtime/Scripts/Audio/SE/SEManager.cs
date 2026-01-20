using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.Audio;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    /// <summary>タイムラインから再生するSE以外</summary>
    public enum SE
    {
        None = 0,
        /// <summary>通常ボタン</summary>
        se_common_icon_tap = 1,
        /// <summary>キャンセルボタン</summary>
        se_common_cancel = 2,
        /// <summary>決定ボタン</summary>
        se_common_tap = 3,
        /// <summary>項目選択</summary>
        se_common_koumoku_select = 4,
        /// <summary>クリック音</summary>
        se_common_click = 5,
        /// <summary>フッターボタン</summary>
        se_common_footer_tap = 6,
        /// <summary>バナースライド</summary>
        se_common_slide = 7,
        /// <summary>モーダルオープン</summary>
        se_common_window_open = 8,
        /// <summary>モーダルクローズ</summary>
        se_common_window_close = 9,
        /// <summary>エラー音</summary>
        se_common_error = 10,
        /// <summary>アイテム獲得</summary>
        se_common_get_item = 11,
        
        /// <summary>ステータスアップ</summary>
        se_training_status_up = 101,
        /// <summary>スキル獲得</summary>
        se_training_skill_get = 102,
        /// <summary>練習カード選択</summary>
        se_training_menu_card = 103,
        
        /// <summary>インゲーム コマンドセレクトSE</summary>
        se_game_heartbeat = 201,
    }
    
    public static class SEManager
    {
        private const int AudioPlayerCount = 4;
        private const float DuplicateAllowableTime = 0.1f;
        
        private class SEPlayer
        {
            public AudioPlayer AudioPlayer = null;
            public CancellationTokenSource CancelTokenSource = null;
            public int Index = 0;
            
            public void Cancel()
            {
                if(CancelTokenSource != null)
                {
                    CancelTokenSource.Cancel();
                    CancelTokenSource.Dispose();
                    CancelTokenSource = null;
                }
            }
        }
        
        private static int index = 0;
        private static SEPlayer[] sePlayers = null;
        private static ResourcesLoader resourcesLoader = new ResourcesLoader();

        /// <summary>ファイル名取得</summary>
        private static string GetFileName(SE se)
        {
            string fileName = null;
            if(se != SE.None)
            {
                fileName = se.ToString();
            }
            return fileName;
        }
        
        /// <summary>アドレス取得</summary>
        public static string GetAddress(string fileName)
        {
            return ResourcePathManager.GetPath("SE", fileName);
        }
        
        /// <summary>SE再生</summary>
        public static void PlaySE(SE se)
        {
            PlaySEAsync(se).Forget();
        }
        
        /// <summary>SE再生</summary>
        public static void PlaySE(string fileName)
        {
            PlaySEAsync(fileName).Forget();
        }
        
        /// <summary>SE再生</summary>
        public static void PlaySE(AudioClip clip, bool canDuplicatePlayback = true, float allowableTime = DuplicateAllowableTime)
        {
            if(clip == null) return;
            // 重複再生不可の場合
            if (canDuplicatePlayback == false)
            {
                // 0.1秒未満であれば0.1秒を閾値にする
                allowableTime = allowableTime < DuplicateAllowableTime ? DuplicateAllowableTime : allowableTime;
                // allowableTime以内に再生されたクリップがあれば再生しない
                if (GetSEPlayerCount(clip, allowableTime) > 0) return;
            }

            SEPlayer sePlayer = GetSEPlayer();
            // 読み込みキャンセル
            sePlayer.Cancel();
            // クリップセット
            sePlayer.AudioPlayer.AudioSource.clip = clip;
            // 再生
            sePlayer.AudioPlayer.AudioSource.Play();
        }
        
        /// <summary>SE再生</summary>
        public static UniTask PlaySEAsync(SE se)
        {
            // ファイル名取得
            string fileName = GetFileName(se);
            // 再生
            return PlaySEAsync(fileName);
        }
        
        /// <summary>SE再生</summary>
        public static async UniTask PlaySEAsync(string fileName)
        {
            if(string.IsNullOrEmpty(fileName)) return;
            
            // プレイヤー取得
            SEPlayer sePlayer = GetSEPlayer();
            // 読み込みキャンセル
            sePlayer.Cancel();
            // トークン生成
            sePlayer.CancelTokenSource = new CancellationTokenSource();

            // クリップ読み込み
            await resourcesLoader.LoadAssetAsync<AudioClip>(GetAddress(fileName), 
                clip =>
                {
                    // クリップセット
                    sePlayer.AudioPlayer.AudioSource.clip = clip;
                    // 再生
                    sePlayer.AudioPlayer.AudioSource.Play();
                },
                sePlayer.CancelTokenSource.Token
            );
        }

        /// <summary>プレイヤー取得　</summary>
        private static SEPlayer GetSEPlayer()
        {
            if(sePlayers == null)
            {
                // 指定した個数だけプレイヤー作成
                sePlayers = new SEPlayer[AudioPlayerCount];
                for(int i = 0; i < sePlayers.Length; i++)
                {
                    sePlayers[i] = new SEPlayer();
                    sePlayers[i].AudioPlayer = AudioManager.Instance.CreateSEPlayer(null);
                }
            }

            SEPlayer sePlayer = null;
            for (int i = 0; i < sePlayers.Length; i++)
            {
                // 再生中してない
                if(!sePlayers[i].AudioPlayer.AudioSource.isPlaying)
                {
                    sePlayer = sePlayers[i];
                    break;
                }
            }
            
            // 全て再生中
            if(sePlayer == null)
            {
                sePlayer = sePlayers[0];
                for(int i = 1; i < sePlayers.Length; i++)
                {
                    // もっとも古いプレイヤーを検索
                    if(sePlayers[i].Index < sePlayer.Index)
                    {
                        sePlayer = sePlayers[i];
                    }
                }
            }
            
            // インデックス更新
            index++;
            sePlayer.Index = index;
            return sePlayer;
        }

        public static void StopSE(SE se)
        {
            var seName = GetFileName(se);
            foreach (var sePlayer in sePlayers)
            {
                if (!sePlayer.AudioPlayer.AudioSource.isPlaying)
                {
                    continue;
                }

                if (sePlayer.AudioPlayer.AudioSource.clip.name.Equals(seName))
                {
                    sePlayer.AudioPlayer.AudioSource.Stop();
                    sePlayer.Cancel();
                }
            }
        }

        /// <summary>解放</summary>
        public static void Release()
        {
            if(sePlayers != null)
            {
                // プレイヤー読み込みキャンセル
                for (int i = 0; i < sePlayers.Length; i++)
                {
                    sePlayers[i].Cancel();
                }
            }
            
            // ハンドル解放
            resourcesLoader.Release();
        }

        /// <summary>同じクリップを再生しているSEPlayerの個数取得</summary>
        private static int GetSEPlayerCount(AudioClip clip, float allowableTime)
        {
            int count = 0;
            foreach (SEPlayer sePlayer in sePlayers)
            {
                AudioSource audioSource = sePlayer.AudioPlayer.AudioSource;
                if (audioSource.clip == clip && audioSource.isPlaying && audioSource.time < allowableTime)
                {
                    count++;
                }
            }

            return count;
        }
    }
}