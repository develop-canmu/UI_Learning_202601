using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingCardUnionInformationView : MonoBehaviour
    {
        [SerializeField] 
        private TrainingCardUnionInformationCardView cardUnionInformationCardView = null;
        [SerializeField] 
        private TrainingSpecialTrainingMembersView specialTrainingView = null;

        /// <summary> カードユニオンのItemデータをセット </summary>
        public void SetViewData(long mTrainingCardId, long cardCharacterId, long mCharaId, List<long> specialLectureMembers, Color backgroundColor,  bool isSelected, PracticeCardView.DisplayEnhanceUIFlags displayEnhanceUIFlags)
        {
            // ベースの色を設定
            cardUnionInformationCardView.SetBackgroundColor(backgroundColor);

            // Item内左側の処理
            // 練習メニューカードと所持者のキャラアイコン表示
            cardUnionInformationCardView.SetCard(mTrainingCardId, cardCharacterId, mCharaId, displayEnhanceUIFlags);
            // スペシャルレクチャーのバッジ表示
            cardUnionInformationCardView.SetSpecialLectureBadge(specialLectureMembers, mCharaId);
            // FLOWカード（育成対象選手の練習メニューカード）の場合はフレームを表示
            cardUnionInformationCardView.SetFlowCardFrame(isSelected);
            
            // Item内右側の処理
            // スペシャルトレーニング参加者の表示
            specialTrainingView.SetSpecialTrainingMembers(specialLectureMembers);
            // スペシャルレクチャー参加表示テキストと下線(ライン)の色をセットする
            specialTrainingView.SetSpecialLectureTitleColor(isSelected);
        }
    }
}