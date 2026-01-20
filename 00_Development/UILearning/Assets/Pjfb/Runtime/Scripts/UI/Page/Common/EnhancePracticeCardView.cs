using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Common
{
    /// <summary>
    /// 強化練習カード表示用View
    /// 強化前と強化後のカードを横並びで表示する
    /// </summary>
    public class EnhancePracticeCardView : MonoBehaviour
    {
        /// <summary>
        /// 表示データクラス
        /// </summary>
        public class Data
        {
            /// <summary>強化前のTrainingCardCharaId</summary>
            public long BeforeCardCharacterId { get; }
            
            /// <summary>強化後のTrainingCardCharaId</summary>
            public long AfterCardCharacterId { get; }
            
            /// <summary>キャラId</summary>
            public long CharacterId { get; }
            
            /// <summary>各種条件判定や表示などで使用される基準キャラクターレベル</summary>
            public long BaseReferenceCharaLv { get; }
            
            /// <summary>ユーザーのキャラレベル</summary>
            public long TargetUserCharaLv { get; }
            
            public Data(
                long beforeCardCharacterId,
                long afterCardCharacterId,
                long characterId,
                long baseReferenceCharaLv,
                long targetUserCharaLv)
            {
                BeforeCardCharacterId = beforeCardCharacterId;
                AfterCardCharacterId = afterCardCharacterId;
                CharacterId = characterId;
                BaseReferenceCharaLv = baseReferenceCharaLv;
                TargetUserCharaLv = targetUserCharaLv;
            }
        }
        
        [SerializeField] private PracticeCardView beforeCardView;
        [SerializeField] private PracticeCardView afterCardView;

        /// <summary>
        /// 強化カード表示を設定
        /// </summary>
        public void SetData(Data data)
        {
            // 強化前カードの表示
            TrainingCardCharaMasterObject beforeCardChara = MasterManager.Instance.trainingCardCharaMaster.FindData(data.BeforeCardCharacterId);
            beforeCardView.SetCard(
                beforeCardChara.mTrainingCardId,
                data.BeforeCardCharacterId,
                data.CharacterId,
                data.BaseReferenceCharaLv,
                PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI,
                data.TargetUserCharaLv
            );

            // 強化後カードの表示
            TrainingCardCharaMasterObject afterCardChara = MasterManager.Instance.trainingCardCharaMaster.FindData(data.AfterCardCharacterId);
            afterCardView.SetCard(
                afterCardChara.mTrainingCardId,
                data.AfterCardCharacterId,
                data.CharacterId,
                data.BaseReferenceCharaLv,
                PracticeCardView.DisplayEnhanceUIFlags.Label | PracticeCardView.DisplayEnhanceUIFlags.DetailLabel | PracticeCardView.DisplayEnhanceUIFlags.DetailEnhanceUI,
                data.TargetUserCharaLv
            );
        }
    }
}

