using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;


namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandSetCharacterParent : IAdvCommand
    {
    
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.CharacterDatas))]
        [AdvDocument("変更するキャラクタ。")]
        private int characterId = 0;
    
        [SerializeField]
        [AdvDocument("親に設定するオブジェクトId。")]
        public ulong parentId = 0;
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            AdvCharacter c = manager.GetAdvCharacter<AdvCharacter>(characterId);
            
            if(parentId == 0)
            {
                AdvObjectLayer characterLayer = manager.GetObjectLayer(manager.Config.CharacterLayerId);
                c.SetTransformParent(characterLayer.transform);
            }
            else
            {
                AdvObject parent = manager.GetObject(parentId);
                c.SetParent(parent);
            }
        }
    }
}