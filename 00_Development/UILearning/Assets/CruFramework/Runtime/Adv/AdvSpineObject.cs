
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Spine.Unity;
using UnityEditor;

namespace CruFramework.Adv
{
    public  abstract class AdvSpineObject : AdvUICharacter
    {
        /// <summary>スケルトンの読み込み</summary>
        public abstract void LoadSkeletonData(AdvManager manager, SkeletonDataAsset skeletonDataAsset);
        /// <summary>アニメーションの再生</summary>
        public abstract void PlayAnimation(string animationName, int layer, bool isLoop);
        
        public override async UniTask PreLoadResource(AdvManager manager, string id)
        {
            await manager.PreLoadResourceAsync(manager.Config.SpineResourcePathId, id);
        }
        
        public override void LoadResource(AdvManager manager, string id)
        {
            // リソース読み込み
            SkeletonDataAsset skeletonData = manager.LoadResource<SkeletonDataAsset>(manager.Config.SpineResourcePathId, id);
            // セット
            LoadSkeletonData(manager, skeletonData);
        }
    }
    
}

#endif // CRUFRAMEWORK_SPINE_SUPPORT