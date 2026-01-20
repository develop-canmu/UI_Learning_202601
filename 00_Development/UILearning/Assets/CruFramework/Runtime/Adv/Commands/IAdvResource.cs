using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
    public interface IAdvResource
    {
        /// <summary>リソースの読み込み</summary>
        UniTask PreLoadResources(AdvManager manager);
    }
    
    public interface IAdvCharacterResource
    {
        /// <summary>リソースの読み込み</summary>
        UniTask PreLoadResources(AdvManager manager, AdvCharacter characterPrefab);
    }
}
