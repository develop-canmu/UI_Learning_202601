using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.H2MD;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using Pjfb.Storage;

namespace Pjfb
{
    public abstract class CharacterCardImageBase : CancellableRawImageWithId
    {
        [SerializeField]
        private H2MDUIPlayer effectPlayer = null;

        [SerializeField]
        private bool forceImageDisplay = false;


        protected abstract string GetEffectKey(long id);

        public override async UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            // 非表示にしておく
            effectPlayer?.gameObject.SetActive(false);
            RawImage.gameObject.SetActive(false);
            // エフェクトのアドレス
            string key = GetEffectKey(id);
            // エフェクトが存在する
            if(HasCharaEffect(key))
            {
                // キャンセル
                Cancel();
                source = new CancellationTokenSource();
                // エフェクト読み込み
                await resourcesLoader.LoadAssetAsync<H2MDAsset>(key,
                    h2md =>
                    {
                        // エフェクト再生
                        effectPlayer.Load(h2md);
                        effectPlayer.Play();
                        // エフェクトを表示
                        effectPlayer.gameObject.SetActive(true);
                    },
                    source.Token
                );
            }
            else
            {
                // 画像読み込み
                await base.SetTextureAsync(id, resourcesLoader);
                RawImage.gameObject.SetActive(true);
            }
        }

        /// <summary> エフェクトを停止する </summary>
        public void StopEffect(long id)
        {
            // エフェクトが再生される場合は実行中のタスクをキャンセル
            if (HasCharaEffect(id))
            {
                Cancel();
            }
            
            // 再生中なら停止
            if(effectPlayer?.IsPlaying ?? false)
            {
                effectPlayer.Stop();
            }

        }

        /// <summary> エフェクトを再生するか </summary>
        private bool HasCharaEffect(long id)
        {
            var key = GetEffectKey(id);
            return HasCharaEffect(key);
        }
        
        /// <summary> エフェクトを再生するか </summary>
        private bool HasCharaEffect(string key)
        {
            return !forceImageDisplay && IsConfigStandard() && ExistEffect(key) && effectPlayer != null;
        }
        
        /// <summary>キーがカタログにあるかどうか</summary>
        private bool ExistEffect(string key)
        {
            return AddressablesManager.HasResources<H2MDAsset>(key);
        }
        
        /// <summary>設定画面で標準設定されているか</summary>
        private bool IsConfigStandard()
        {
            return LocalSaveManager.saveData.appConfig.CharacterCardEffectType == (int)AppConfig.DisplayType.Standard;
        }
    }
}