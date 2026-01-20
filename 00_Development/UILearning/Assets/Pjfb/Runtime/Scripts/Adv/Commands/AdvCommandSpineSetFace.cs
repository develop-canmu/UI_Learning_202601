
#if CRUFRAMEWORK_SPINE_SUPPORT

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandSpineSetFace : IAdvCommand
    {
        [System.Serializable]
        private class FaceData
        {
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.CharacterDatas), AdvObjectIdAttribute.WindowType.SearchWindow)]
            [AdvDocument("表情を変えるキャラクタId。")]
            public int characterId = 0;
        
            [SerializeField]
            [AdvObjectId(nameof(AdvConfig.SpineFaces))]
            [AdvDocument("表情Id。")]
            public int faceId = 0;
        }
        

        [SerializeField]
        [AdvDocument("表情を変えるキャラクタId。")]
        private FaceData[] faceDatas = null;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            for(int i=0;i<faceDatas.Length;i++)
            {
                // 対象のキャラ
                AdvSpineCharacter c = manager.GetAdvCharacter<AdvSpineCharacter>(faceDatas[i].characterId);
                // 表情データ取得
                AdvSpineFaceId face = manager.Config.SpineFaces[faceDatas[i].faceId];
                // 表情セット
                c.SetFace(manager, face);
            }

        }
    }
}

#endif // #if CRUFRAMEWORK_SPINE_SUPPORT