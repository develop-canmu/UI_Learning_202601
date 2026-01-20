
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Spine.Unity;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandSpineAnimation : IAdvCommand
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
        [AdvDocument("アニメーションさせるオブジェクトId。")]
        private int objectId = 0;
        
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.SpineAnimationNames))]
        [AdvDocument("アニメーションId。")]
        private int animationId = 0;
        
        [SerializeField]
        [AdvDocument("ループ再生。")]
        private bool isLoop = false;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            AdvSpineObject obj = manager.GetAdvCharacter<AdvSpineObject>(objectId);
            if(obj == null)return;
            AdvSpineAnimationNameId anim = manager.Config.SpineAnimationNames[animationId];
            obj.PlayAnimation(anim.Name, anim.Layer, isLoop);

        }
    }
}

#endif // #if CRUFRAMEWORK_SPINE_SUPPORT