using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    /// <summary>カードユニオンのカードのView</summary>
    public class TrainingCardUnionInformationCardView : MonoBehaviour
    {
        [SerializeField]
        private GameObject specialLectureBadge = null;
        [SerializeField]
        private GameObject flowCardFrame = null;
        [SerializeField]
        private Image backgroundImage = null;
        [SerializeField]
        private TrainingCardView trainingCardView = null;
        
        [SerializeField]
        private bool isDisplayCardLevel = false;

        /// <summary>カード情報登録</summary>
        public void SetCard(long mTrainingCardId, long cardCharacterId, long mCharaId, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            // 練習メニューカードと所持者のキャラアイコン表示(レベル表示を含めたカード表示も含有)
            trainingCardView.SetCardAndCharacter(mTrainingCardId, cardCharacterId, mCharaId, displayEnhanceUIFlags);
        }
        
        /// <summary>スペシャルレクチャーの表示</summary>
        public void SetSpecialLectureBadge(List<long> memberCharaIds, long mCharaId)
        {
            // スペシャルレクチャー発動時はバッジの表示
            specialLectureBadge.gameObject.SetActive(memberCharaIds.Contains(mCharaId));
        }

        /// <summary>FLOWカード（育成対象選手の練習メニューカード）にフレームを表示</summary>
        public void SetFlowCardFrame(bool isSelected)
        {
            // FLOWカード（育成対象選手の練習メニューカード）のみ表示
            flowCardFrame.SetActive(isSelected);
        }
        
        /// <summary>背景の色</summary>
        public void SetBackgroundColor(Color color)
        {
            backgroundImage.color = color;
        }
    }
}