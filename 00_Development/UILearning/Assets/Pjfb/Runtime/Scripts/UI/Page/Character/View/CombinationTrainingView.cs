using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Combination;
using Pjfb.UserData;

namespace Pjfb
{
    public class CombinationTrainingView : CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>
    {
        [SerializeField] private GameObject detailButton;
        
        private long mCharaTrainingComboBuffId;
        private Dictionary<long, UserDataChara> mCharaIdPossessionDictionary;
        public void Initialize(bool showDetailButton, long charaTrainingComboBuffId,List<CharacterDetailData> characterDetailList, Dictionary<long, UserDataChara> charaIdPossessionDictionary, 
            List<CombinationManager.TrainingMultipleSkillData> activatedSkillDataList, List<CombinationManager.TrainingMultipleSkillData> lockSkillDataList,
            NotActiveLabelType notActiveLabelType)
        {
            detailButton.SetActive(showDetailButton);
            mCharaTrainingComboBuffId = charaTrainingComboBuffId;
            mCharaIdPossessionDictionary = charaIdPossessionDictionary;
            // キャラアイコンの設定
            InitializeCharaUI(characterDetailList, mCharaIdPossessionDictionary, true);
            // 解放済みと未解放のスキル表示設定
            InitializeSkillUi(activatedSkillDataList, lockSkillDataList, notActiveLabelType);
        }

        protected override void InitializeSkillPrefab(CombinationPracticeSkillView tPrefab, CombinationManager.TrainingMultipleSkillData tSkill)
        {
            if (tSkill == null) return;
            var hasSkill = (tSkill.PracticeSkillDataList?.Count ?? 0) > 0;
            if (!hasSkill) return;

            tPrefab.InitializeSkill(tSkill.PracticeSkillDataList, !string.IsNullOrEmpty(tSkill.LockString),
                tSkill.LockString, tSkill.ShowSkillHighlight);
        }

        public void OnClickDetailButton()
        {
            var combinationTraining = CombinationManager.GetAllCombinationTrainingList().FirstOrDefault(data => data.MCharaTrainingComboBuffId == mCharaTrainingComboBuffId);
            if (combinationTraining == null)
            {
                CruFramework.Logger.LogError($"combinationTrainingがnullです。クライアントに確認をお願いします。　mCharaTrainingComboBuffId：{mCharaTrainingComboBuffId}");
                return;
            }

            CombinationTrainingSkillSetDetailModal.Open(
                new CombinationTrainingSkillSetDetailModal.Data(combinationTraining, mCharaIdPossessionDictionary));
        }
    }
}