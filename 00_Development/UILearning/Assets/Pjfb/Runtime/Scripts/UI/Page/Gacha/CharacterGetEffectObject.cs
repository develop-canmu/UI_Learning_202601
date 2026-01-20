using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.H2MD;
using CruFramework.ResourceManagement;
using CruFramework.Timeline;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine.Playables;

namespace Pjfb
{
    public class CharacterGetEffectObject : CharacterGetEffectObjectBase
    {
        [SerializeField]
        private CharacterCardGachaImage characterImage = null;
        
        [SerializeField]
        private CharacterCardGachaBackgroundImage backgroundImage = null;
        
        [SerializeField]
        private TextMeshProUGUI nameText = null;
        
        [SerializeField]
        private TextMeshProUGUI nickNameText = null;
        
        public override async UniTask SetAsync(CharaMasterObject mChara, ResourcesLoader resourcesLoader, CancellationToken token)
        {
            try
            {
                nameText.text = mChara.name;
                nickNameText.text = mChara.nickname;
                // キャラ画像読み込み
                UniTask characterTask = characterImage.SetTextureAsync(mChara.id, resourcesLoader);
                // 背景読み込み
                UniTask backgroundTask = backgroundImage.SetTextureAsync(mChara.parentMCharaId, resourcesLoader);
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
                await UniTask.WhenAll(characterTask, backgroundTask, h2mdTask);
            }
            catch (OperationCanceledException)
            {
            }
        }
        
        
    }
}