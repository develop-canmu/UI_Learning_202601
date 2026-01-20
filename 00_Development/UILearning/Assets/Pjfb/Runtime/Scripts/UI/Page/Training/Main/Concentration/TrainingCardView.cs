using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary>
    /// CardViewのBaseクラス
    /// </summary>
    public class TrainingCardView : MonoBehaviour
    {
        [SerializeField]
        private PracticeCardView cardView = null;
        
        [SerializeField]
        private CharacterIcon characterIcon = null;
        
        /// <summary>練習メニューカードとキャラアイコンを設定</summary>
        public void SetCardAndCharacter(long cardId, long cardCharacterId, long charaId, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            // レベル表示がある場合は含めたカード表示（キャラクターレベルは不要なので-1に）
            cardView.SetCard(cardId, cardCharacterId, charaId, -1, displayEnhanceUIFlags);
            characterIcon.SetIconTextureWithEffectAsync(charaId).Forget();
        }
    }
}
