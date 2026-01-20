using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using CruFramework.H2MD;
using CruFramework.ResourceManagement;
using CruFramework.Timeline;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Voice;
using UnityEngine;
using UnityEngine.Playables;

namespace Pjfb
{
    public class CharacterGetEffect : CancellableResource
    {
        private const int VoiceIndex = 2;
        
        [SerializeField]
        private Transform effectParent = null;
        
        [SerializeField]
        private GameObject skipButton = null;
        
        private CharacterGetEffectObjectBase effectObject = null;
        private bool isComplete = false;

        public async UniTask PlayAsync(long mCharaId)
        {
            await PlayAsync(mCharaId, PageResourceLoadUtility.resourcesLoader);
        }
        
        public async UniTask PlayAsync(long mCharaId, ResourcesLoader loader)
        {
            // 完了フラグおろす
            isComplete = false;
            
            // 読み込み中ならキャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            
            // 非表示
            skipButton.SetActive(false);
            // 一つ前のエフェクトを削除
            if(effectObject != null)
            {
                Destroy(effectObject.gameObject);
            }

            // キャラのレアリティ取得
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            // m_charaがない場合は演出再生しない
            if(mChara == null) return;
            // サポート器具の場合は無視
            if(mChara.cardType == CardType.SupportEquipment) return;

            // ルートオブジェクトアクティブ
            gameObject.SetActive(true);
            
            // レアリティ
            long rarity = MasterManager.Instance.rarityMaster.FindData(mChara.mRarityId).value;

            await AppManager.Instance.LoadingActionAsync(async () => 
            {
                // エフェクトオブジェクト生成
                await loader.LoadAssetAsync<GameObject>(PageResourceLoadUtility.GetCharacterGetEffectObjectPath(rarity, mChara.cardType), 
                    go => 
                    {
                        effectObject = GameObject.Instantiate(go, effectParent).GetComponent<CharacterGetEffectObjectBase>();
                        effectObject.gameObject.SetActive(false);
                    }, 
                    source.Token);
                
                // エフェクトデータセット
                await effectObject.SetAsync(mChara, loader, source.Token);
                
                // キャラの場合
                if(mChara.cardType == CardType.Character || mChara.cardType == CardType.Adviser)
                {
                    // レアリティが3以上なら演出再生と同時にボイス再生
                    if(rarity >= 3)
                    {
                        // ボイス再生中なら止める
                        VoiceManager.Instance.StopVoice();
                        // ボイス再生
                        await VoiceManager.Instance.PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.System, mChara, VoiceIndex);
                    }
                }
                
                // 演出表示
                effectObject.gameObject.SetActive(true);
                // スキップボタン表示
                skipButton.SetActive(true);
                // 演出再生
                effectObject.PlayableDirector.Play();
            });
            
            // 完了まで待機
            await UniTask.WaitUntil(() => isComplete, cancellationToken: source.Token);
            
            gameObject.SetActive(false);
        }
        

        public async UniTask PreLoadAsset(long mCharaId, ResourcesLoader loader){
            
            // 完了フラグおろす
            isComplete = false;

            // 読み込み中ならキャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            
            // 非表示
            skipButton.SetActive(false);
            // 一つ前のエフェクトを削除
            if(effectObject != null)
            {
                Destroy(effectObject.gameObject);
            }

            // キャラのレアリティ取得
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            // m_charaがない場合は演出再生しない
            if(mChara == null) return;
            // サポート器具の場合は無視
            if(mChara.cardType == CardType.SupportEquipment) return;

            // ルートオブジェクトアクティブ
            gameObject.SetActive(true);
            
            // レアリティ
            long rarity = MasterManager.Instance.rarityMaster.FindData(mChara.mRarityId).value;

            await AppManager.Instance.LoadingActionAsync(async () => 
            {
                // エフェクトオブジェクト生成
                await loader.LoadAssetAsync<GameObject>(PageResourceLoadUtility.GetCharacterGetEffectObjectPath(rarity, mChara.cardType), 
                    go => 
                    {
                        effectObject = GameObject.Instantiate(go, effectParent).GetComponent<CharacterGetEffectObjectBase>();
                        effectObject.gameObject.SetActive(false);
                    }, 
                    source.Token);
                
                // エフェクトデータセット
                await effectObject.SetAsync(mChara, loader, source.Token);
                
            });
        }

        public async UniTask PlayVoiceWithPreLoad( long mCharaId ){
            // キャラのレアリティ取得
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(mCharaId);
            // m_charaがない場合は演出再生しない
            if(mChara == null) return;
            // サポート器具の場合は無視
            if(mChara.cardType == CardType.SupportEquipment) return;

            long rarity = MasterManager.Instance.rarityMaster.FindData(mChara.mRarityId).value;
            // キャラの場合
            if(mChara.cardType == CardType.Character)
            {
                // レアリティが3以上なら演出再生と同時にボイス再生
                if(rarity >= 3)
                {
                    // ボイス再生中なら止める
                    VoiceManager.Instance.StopVoice();
                    // ボイス再生
                    await VoiceManager.Instance.PlayVoiceForIndexAsync(VoiceResourceSettings.VoiceType.System, mChara, VoiceIndex);
                }
            }
        }
        
        public async UniTask PlayAsyncWithPreLoad( )
        {
            if(effectObject == null) return;    
            // 演出表示
            effectObject.gameObject.SetActive(true);
            // スキップボタン表示
            skipButton.SetActive(true);
            // 演出再生
            effectObject.PlayableDirector.Play();
            
            // 完了まで待機
            await UniTask.WaitUntil(() => isComplete, cancellationToken: source.Token);
            
            gameObject.SetActive(false);
        }
        

        /// <summary>uGUI</summary>
        public void OnSkip()
        {
            if(effectObject == null) return;
            PlayableDirector playableDirector = effectObject.PlayableDirector;
            // 完了してたら
            if(playableDirector.time >= playableDirector.duration)
            {
                // 非表示
                playableDirector.gameObject.SetActive(false);
                isComplete = true;
            }
            else
            {
                // スキップ
                playableDirector.GetComponent<TimelineSkipController>().Skip();
            }
        }
    }
}