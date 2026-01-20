using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.InGame;
using Spine;
using Spine.Unity;

namespace Pjfb
{
    public enum SpineLayer
    {
        Body = 1,
        Face = 2,
        Eye = 3,
        Mouth = 4,
    }

    public static class SpineUtility
    {
        public static readonly string DefaultBodyAnimationName = "body";
        public static readonly string DefaultFaceAnimationName = "face";
        public static readonly string DefaultEyeAnimationName = "eye";
        public static readonly string DefaultEyeBlinkAnimationName = "blink";
        public static readonly string DefaultMouthAnimationName = "mouth";
        
        /// <summary>瞬きの間隔取得</summary>
        public static float GetEyeBlinkInterval()
        {
            return BattleGameLogic.GetNonStateRandomValue(25, 100) / 10.0f;
        }

        /// <summary>瞬き</summary>
        public static TrackEntry PlayEyeBlinkAnimation(SkeletonGraphic skeletonGraphic, int layer, string animationName)
        {
            TrackEntry entry = PlayAnimation(skeletonGraphic, layer, animationName, false);
            if(entry != null)
            {
                entry.MixDuration = 0;
            }
            return entry;
        }
        
        /// <summary>アニメーション再生</summary>
        public static TrackEntry PlayAnimation(SkeletonGraphic skeletonGraphic, int layer, string animationName, bool loop)
        {
            if(!skeletonGraphic.SkeletonData.Animations.Exists(animation => animation.Name == animationName))
            {
                return null;
            }
            TrackEntry entry = skeletonGraphic.AnimationState.SetAnimation(layer, animationName, loop);
            return entry;
        }
    }
}