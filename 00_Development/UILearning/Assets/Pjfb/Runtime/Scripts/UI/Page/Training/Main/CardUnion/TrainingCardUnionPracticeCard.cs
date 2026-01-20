using System;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Training;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    /// <summary> カードユニオン表示の練習カード </summary>
    public class TrainingCardUnionPracticeCard : MonoBehaviour
    {
        // カード表示
        [SerializeField]
        private PracticeCardImage card = null;
        
        // インスピレーションエフェクト
        [SerializeField]
        private TrainingInspirationCardEffect inspirationEffect = null;
        
        // カード周辺の回るエフェクト
        [SerializeField]
        private GameObject cardSquareEffect = null;
        
        private Action onAnimationEvent = null;

        private void OnEnable()
        {
            if (onAnimationEvent != null)
            {
                onAnimationEvent();
            }
        }

        public async UniTask SetCardAsync(long cardId, long inspirationEffectNumber)
        {
            onAnimationEvent = null;
            
            var master = MasterManager.Instance.trainingCardMaster.FindData(cardId);
            await card.SetTextureAsync(master.imageId);

            cardSquareEffect.gameObject.SetActive(true);
            
            // インスピレーションエフェクト
            bool hasInspiration = inspirationEffectNumber > 0;
            inspirationEffect.gameObject.SetActive(hasInspiration);
            if (hasInspiration)
            {
                // 通常カード以外は特殊カードとみなす
                bool isSpecial = master.cardGroupType != TrainingCardGroup.Basic;
                
                // アニメーションイベント登録
                onAnimationEvent = () => inspirationEffect.PlayEffect(inspirationEffectNumber, isSpecial);
                
                // アクティブ時はそのまま発火
                if (gameObject.activeInHierarchy)
                {
                    onAnimationEvent();
                }
            }
            
        }

        /// <summary> カードのエフェクトを切る </summary>
        public void StopCardEffect()
        {
            inspirationEffect.ShowEffect(false);
            cardSquareEffect.gameObject.SetActive(false);
        }
    }
}