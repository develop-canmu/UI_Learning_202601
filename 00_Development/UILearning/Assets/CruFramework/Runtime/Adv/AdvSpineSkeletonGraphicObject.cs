
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using CruFramework.Audio;
using Spine;
using UnityEngine;
using Spine.Unity;

namespace CruFramework.Adv
{
    public class AdvSpineSkeletonGraphicObject : AdvSpineObject
    {
        [SerializeField]
        private SkeletonGraphic skeletonGraphic = null;
        public SkeletonGraphic SkeletonGraphic{get{return skeletonGraphic;}}

        public override Transform ShakeTarget {get{return skeletonGraphic.transform; }}

        public override void Grayout()
        {
            skeletonGraphic.color = Color.gray;
        }

        public override void Highlight()
        {
            skeletonGraphic.color = Color.white;
        }

        public override void LoadSkeletonData(AdvManager manager, SkeletonDataAsset skeletonDataAsset)
        {
            skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
            skeletonGraphic.Initialize(true);
        }
        
        /// <summary>アニメーションの再生</summary>
        public override void PlayAnimation(string animationName, int layer, bool isLoop)
        {
            skeletonGraphic.AnimationState.SetAnimation(layer, animationName, isLoop);
        }
    }
}

#endif // CRUFRAMEWORK_SPINE_SUPPORT