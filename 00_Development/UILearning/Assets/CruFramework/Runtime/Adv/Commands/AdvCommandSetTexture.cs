using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
    [System.Serializable]
    public class AdvCommandSetTexture : IAdvCommand, IAdvResource
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Textures))]
        [AdvDocument("変更する対象のテクスチャId。")]
        private int textureId = 0;
        
        [SerializeField]
        [AdvDocument("データId。")]
        private int id = 0;
        
        UniTask IAdvResource.PreLoadResources(AdvManager manager)
        {
            return manager.PreLoadResourceAsync( manager.Config.Textures[textureId].ResourceKey, id);
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            manager.SetTexture(textureId, manager.Config.Textures[textureId].ResourceKey, id);
        }
    }
}
