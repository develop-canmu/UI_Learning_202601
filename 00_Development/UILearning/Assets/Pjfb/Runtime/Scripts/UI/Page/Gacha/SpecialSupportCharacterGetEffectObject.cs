using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.H2MD;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Timeline;

namespace Pjfb
{
    public class SpecialSupportCharacterGetEffectObject : CharacterGetEffectObjectBase
    {
        [SerializeField]
        private SpecialSupportCharacterGachaImage backgroundImage = null;
        
        [SerializeField]
        private SpecialSupportCharacterGachaImage foregroundImage = null;
        
        [SerializeField]
        private TextMeshProUGUI nameText = null;
        
        [SerializeField]
        private GameObject exIcon = null;
        
        public override async UniTask SetAsync(CharaMasterObject mChara, ResourcesLoader resourcesLoader, CancellationToken token)
        {
            try
            {
                // 名前
                nameText.text = mChara.name;
                // Exアイコン
                exIcon.SetActive(mChara.isExtraSupport);
                // キャラ画像読み込み
                UniTask foregroundTask = foregroundImage.SetTextureAsync(mChara.id, resourcesLoader);
                // 背景読み込み
                UniTask backgroundTask = backgroundImage.SetTextureAsync(mChara.id, resourcesLoader);
                // H2MD読み込み
                UniTask h2mdTask = resourcesLoader.LoadAssetAsync<H2MDAsset>(PageResourceLoadUtility.GetCharacterGetEffectPath(mChara.id), 
                    h2md =>
                    {
                        // 指定のPlayableClipを取得
                        H2MDPlayableClip[] clips = PlayableDirector.GetTimelineClips<H2MDPlayableClip>("H2MDTrack", "H2MDClip");
                        // H2MDアセットセット
                        for(int i = 0; i < clips.Length; i++)
                        {
                            clips[i].H2MDAsset = h2md;
                        }
                    },
                    token
                );
                
                // 待機
                await UniTask.WhenAll(foregroundTask, backgroundTask, h2mdTask);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}