using System.Collections.Generic;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingInspirationCardEffect : TrainingEffectBase
    {
        protected override string ResourceKey { get{return "InspirationCardEffect";}}
        
        private static readonly string InspirationAnimation = "Inspiration";
        private static readonly string InspirationSPAnimation = "InspirationSP";
        
        public void PlayEffect(long id, bool isSpecial)
        {
            PlayEffectAsync(id, isSpecial ? InspirationSPAnimation : InspirationAnimation, 1.0f).Forget();
        }
        
    }
}
