using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    [System.Serializable]
    public class AdvCommandPlaySe : IAdvCommand, IAdvResource
    {
        [SerializeField]
        [AdvDocument("再生するSE。")]
        private string id = string.Empty;
        
        [SerializeField][Range(0, 1.0f)]
        [AdvDocument("音量。")]
        private float volume = 1.0f;
        
        
        async UniTask IAdvResource.PreLoadResources(AdvManager manager)
        {
            await manager.PreLoadResourceAsync( manager.Config.SeResourcePathId, id);
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            // スキップ中は再生しない
            if(manager.IsSkip == false)
            {
                manager.PlaySe(manager.Config.SeResourcePathId, id, volume);
            }
        }
    }
}