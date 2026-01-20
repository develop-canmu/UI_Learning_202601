using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.Playables;

namespace Pjfb
{
    public abstract class CharacterGetEffectObjectBase : MonoBehaviour
    {
        [SerializeField]
        private PlayableDirector playableDirector = null;
        public PlayableDirector PlayableDirector { get { return playableDirector; } }
        
        public abstract UniTask SetAsync(CharaMasterObject mChara, ResourcesLoader resourcesLoader, CancellationToken token);
    }
}