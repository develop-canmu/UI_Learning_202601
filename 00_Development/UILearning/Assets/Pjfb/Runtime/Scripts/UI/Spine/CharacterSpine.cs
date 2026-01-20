using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Spine;
using Spine.Unity;

namespace Pjfb
{
    public class CharacterSpine : MonoBehaviour
    {
        [SerializeField]
        private SkeletonGraphic skeletonGraphic = null;
        /// <summary>SkeletonGraphic</summary>
        public SkeletonGraphic Skeleton{get{return skeletonGraphic;}}

        private  CanvasGroup canvasGroup = null;
        public  CanvasGroup UICanvasGroup
        {
            get
            {
                if(canvasGroup == null)
                {
                    canvasGroup = gameObject.GetComponent<CanvasGroup>();
                }
                return canvasGroup;
            }
        }

        private CancellationTokenSource source = null;
        
        private float eyeBlinkTimer = 0;
        private bool skeletonLoaded = false;

        public void SetSkeletonDataAsset(long mCharId)
        {
            SetSkeletonDataAssetAsync(mCharId).Forget();
        }
        
        public UniTask SetSkeletonDataAssetAsync(long mCharId)
        {
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            return SetSkeletonDataAssetAsync(mChar.standingImageMCharaId);
        }

        public void SetSkeletonDataAsset(string id)
        {
            SetSkeletonDataAssetAsync(id).Forget();
        }

        public async UniTask SetSkeletonDataAssetAsync(string id)
        {
            string key = GetAddress(id);
            skeletonLoaded = false;
            // 非表示
            gameObject.SetActive(false);
            // キャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            // スケルトンデータ読み込み
            await PageResourceLoadUtility.LoadAssetAsync<SkeletonDataAsset>(key,
                skeleton =>
                {
                    skeletonGraphic.skeletonDataAsset = skeleton;
                    if(skeleton == null)return;
                    
                    gameObject.SetActive(true);
                    skeletonGraphic.Initialize(true);
                    // デフォルトアニメーション再生
                    SpineUtility.PlayAnimation(skeletonGraphic, (int)SpineLayer.Body, SpineUtility.DefaultBodyAnimationName, true);
                    SpineUtility.PlayAnimation(skeletonGraphic, (int)SpineLayer.Face, SpineUtility.DefaultFaceAnimationName, true);
                    SpineUtility.PlayAnimation(skeletonGraphic, (int)SpineLayer.Eye, SpineUtility.DefaultEyeAnimationName, true);
                    TrackEntry mouthEntry = SpineUtility.PlayAnimation(skeletonGraphic, (int)SpineLayer.Mouth, SpineUtility.DefaultMouthAnimationName, false);
                    // LipSyncしないのでTimeSceleを0にする
                    mouthEntry.TimeScale = 0;
                    // 初期化
                    eyeBlinkTimer = SpineUtility.GetEyeBlinkInterval();
                    skeletonLoaded = true;
                },
                source.Token
            );
        }
        
        /// <summary>ボーン取得</summary>
        public Bone GetBone(string boneName)
        {
            return skeletonGraphic?.Skeleton?.FindBone(boneName);
            //return skeletonGraphic.SkeletonData.FindBone(boneName);
        }
        
        public ExposedList<BoneData> GetBones()
        {
            return skeletonGraphic.SkeletonData.Bones;
        }
        
        private string GetAddress(string id)
        {
            return PageResourceLoadUtility.GetCharacterSpinePath(id);
        }

        /// <summary>キャンセル</summary>
        protected virtual void Cancel()
        {
            // キャンセル
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }

        private void Update()
        {
            if(!skeletonLoaded) return;
            
            eyeBlinkTimer -= Time.deltaTime;
            if(eyeBlinkTimer <= 0)
            {
                SpineUtility.PlayEyeBlinkAnimation(skeletonGraphic, (int)SpineLayer.Eye, SpineUtility.DefaultEyeBlinkAnimationName);
                eyeBlinkTimer += SpineUtility.GetEyeBlinkInterval();
            }
        }

        protected virtual void OnDestroy()
        {
            // ゲームオブジェクト削除時にキャンセル
            Cancel();
        }
    }
}