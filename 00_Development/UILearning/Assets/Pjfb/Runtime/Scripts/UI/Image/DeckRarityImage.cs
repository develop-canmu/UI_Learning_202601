using System.Collections;
using System.Collections.Generic;
using CruFramework.Addressables;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Pjfb
{
    public class DeckRarityImage : CancellableRawImageWithId
    {
        private const long EmptyId = 1;
        
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetRivalryTeamRarityIconImagePath(id);
        }

        public override UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            if(id != EmptyId)
            {
                gameObject.SetActive(true);
                return base.SetTextureAsync(id, resourcesLoader);
            }
            else
            {
                gameObject.SetActive(false);
                return default;
            }
        }
    }
}