using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class TrainerCardCharacterImageGroup : MonoBehaviour
    {
        [SerializeField]
        private DisplayCharacterImage displayCharacterImage;
        
        /// <summary>画像設定</summary>
        public async UniTask SetTextureAsync(long displayCharacterId, long backGroundId, CancellationToken token = default)
        {
            await displayCharacterImage.SetTextureAsync(displayCharacterId).AttachExternalCancellation(token);
        }
        
        /// <summary>画像更新</summary>
        public async UniTask UpdateCharacterTextureAsync(long displayCharacterId, CancellationToken token = default)
        {
            await displayCharacterImage.SetTextureAsync(displayCharacterId).AttachExternalCancellation(token);
        }
    }
}