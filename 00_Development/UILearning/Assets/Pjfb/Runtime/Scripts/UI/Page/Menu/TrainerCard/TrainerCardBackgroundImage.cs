using CruFramework.H2MD;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class TrainerCardBackgroundImage : MonoBehaviour
    {
        [SerializeField] private Image rawImage = null;
        [SerializeField] private H2MDUIPlayer effectPlayer = null;
        
        public void SetTexture(Sprite image, H2MDAsset effect)
        {
            // 非表示にしておく
            effectPlayer.gameObject.SetActive(false);
            // エフェクトあれば再生
            if (effect != null)
            {
                // エフェクト再生
                effectPlayer.Load(effect);
                effectPlayer.Play();
                // エフェクトを表示
                effectPlayer.gameObject.SetActive(true);
            }
            else
            {
                rawImage.sprite = image;
            }
        }
    }
}