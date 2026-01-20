using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Combination
{
    public class CombinationTrainingSkillSetDetailModal : ModalWindow
    {
        public class Data
        {
            public CombinationManager.CombinationTraining CombinationTraining;
            public Dictionary<long, UserDataChara> MCharaIdPossessionDictionary;

            public Data(CombinationManager.CombinationTraining combinationTraining, Dictionary<long, UserDataChara> mCharaIdPossessionDictionary)
            {
                CombinationTraining = combinationTraining;
                MCharaIdPossessionDictionary = mCharaIdPossessionDictionary;
            }
        }
        
        [SerializeField] private CombinationTrainingView combinationTrainingView;

        private Data modalData;

        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationTrainingSkillSetDetail, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            var characterDetailDataList = CreateCharacterDetailDataList();
            // 発動済みのIdを取得
            var characterTupleDictionary = new Dictionary<long, long>();
            foreach(var uChara in modalData.MCharaIdPossessionDictionary.Values)
            {
                characterTupleDictionary[uChara.MChara.id] = uChara.level;
            }
            var activatedMCharaTrainingComboBuffStatusIdList = modalData.CombinationTraining.GetActivatingMCharaTrainingComboBuffStatusIdList(characterTupleDictionary);
            var activatedSkillDataList = new List<CombinationManager.TrainingMultipleSkillData>();
            var lockSkillDataList = new List<CombinationManager.TrainingMultipleSkillData>();
            foreach (var mCharaTrainingComboBuffStatusId in modalData.CombinationTraining.MCharaTrainingComboBuffStatusIdList)
            {
                // 発動済みの場合はcontinue
                if(activatedMCharaTrainingComboBuffStatusIdList.Contains(mCharaTrainingComboBuffStatusId)) continue;
                    
                var mCharaTrainingComboBuffStatus =
                    MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                        mCharaTrainingComboBuffStatusId);
                if (mCharaTrainingComboBuffStatus == null) continue;

                // 解放不可能のデータを追加
                var sb = new StringBuilder();
                var lockString = mCharaTrainingComboBuffStatus.requireLevel == modalData.CombinationTraining.GetMinRequireLevel()
                    ? sb.AppendFormat(StringValueAssetLoader.Instance["combination.cell.skill_lock_first"], mCharaTrainingComboBuffStatus.requireLevel)
                    : sb.AppendFormat(StringValueAssetLoader.Instance["combination.cell.skill_lock"], mCharaTrainingComboBuffStatus.requireLevel);
                lockSkillDataList.Add(new CombinationManager.TrainingMultipleSkillData(
                    PracticeSkillUtility.GetComboBuffPracticeSkill(mCharaTrainingComboBuffStatusId), false,lockString.ToString()));
            }

            if (activatedMCharaTrainingComboBuffStatusIdList.Count > 0)
            {
                long currentMaxLevelMCharaTrainingComboBuffStatusId = 0;
                long requireLevel = 0;
                foreach (var mCharaTrainingComboBuffStatusId in activatedMCharaTrainingComboBuffStatusIdList)
                {
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if(mCharaTrainingComboBuffStatus == null || mCharaTrainingComboBuffStatus.requireLevel < requireLevel) continue;
                    currentMaxLevelMCharaTrainingComboBuffStatusId = mCharaTrainingComboBuffStatusId;
                    requireLevel = mCharaTrainingComboBuffStatus.requireLevel;
                }
                var currentMaxLevelMCharaTrainingComboBuffStatus =
                    MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                        currentMaxLevelMCharaTrainingComboBuffStatusId);
                if (currentMaxLevelMCharaTrainingComboBuffStatus != null)
                {
                    activatedSkillDataList.Add(new CombinationManager.TrainingMultipleSkillData(
                        PracticeSkillUtility.GetComboBuffPracticeSkill(currentMaxLevelMCharaTrainingComboBuffStatusId), true));
                }
            }

            combinationTrainingView.Initialize(false, modalData.CombinationTraining.MCharaTrainingComboBuffId,
                characterDetailDataList, modalData.MCharaIdPossessionDictionary, activatedSkillDataList, lockSkillDataList,
                CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>
                    .NotActiveLabelType.Total);
        }

        private List<CharacterDetailData> CreateCharacterDetailDataList()
        {
            var detailDataList = new List<CharacterDetailData>();
            foreach (var mCharaId in modalData.CombinationTraining.MCharaSortedIdList)
            {
                var uChara = modalData.MCharaIdPossessionDictionary.ContainsKey(mCharaId)
                    ? modalData.MCharaIdPossessionDictionary[mCharaId]
                    : null;
                long level = System.Math.Max(uChara?.level ?? 0, 1); 
                long liberationLevel = uChara?.newLiberationLevel ?? 0;
                CharacterDetailData detailData = new CharacterDetailData(mCharaId,
                    level, liberationLevel);
                detailDataList.Add(detailData);
            }

            return detailDataList;
        }
    }
}