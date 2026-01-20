using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pjfb
{
    public class ClubRoyalInGameStatusBadge : MonoBehaviour
    {
        [SerializeField] private ClubRoyalBuffIconImage icon;
        [SerializeField] private ParticleSystem particle;
        
        public async UniTask Initialize(int effectType)
        {
            // アイコンが通信のたびに破棄されるため、画像のロードが終わってからnullチェックを行う
            if (icon != null)
            {
                await icon.SetTextureAsync((long)effectType);
            }
            if (particle != null)
            {
                particle.textureSheetAnimation.SetSprite(0, icon.GetSprite());
            }
        }
    }
}