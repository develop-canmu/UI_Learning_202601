using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{

    [System.Serializable]
    public class AdvCommandSound : IAdvCommand, IAdvResource
    {
        [SerializeField]
        [AdvObjectId(nameof(AdvConfig.Sounds))]
        private int id = 0;
        
        [SerializeField][Range(0, 1.0f)]
        private float volume = 1.0f;
        
        
        async UniTask IAdvResource.PreLoadResources(AdvManager manager)
        {
            await manager.PreLoadResourceAsync( manager.Config.Sounds[id].ResourceKey, manager.Config.Sounds[id].ResourceId);
        }
        
        void IAdvCommand.Execute(AdvManager manager)
        {
            AdvSoundId sound = manager.Config.Sounds[id];
            if(sound.IsBgm)
            {
                manager.PlayBgm(sound.ResourceKey, sound.ResourceId, volume);
            }
            else
            {
                manager.PlaySe(sound.ResourceKey, sound.ResourceId, volume);
            }
        }
    }
}