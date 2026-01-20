using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CombinationTrainingScrollDynamic : CombinationScrollDynamicBase
    {
        public IReadOnlyList<CombinationManager.CombinationTraining> TrainingList => trainingList.AsReadOnly();
        private List<CombinationManager.CombinationTraining> trainingList;
        private readonly Dictionary<long, UserDataChara> mCharaIdPossessionDictionary = UserDataManager.Instance.CharaDataByMCharaIdContainer.data;
        
        /// <summary>このクラスで使用するソートフィルタータイプ</summary>
        protected override SortFilterUtility.SortFilterType SortFilterType => SortFilterUtility.SortFilterType.ListCombinationTraining;
        
        /// <summary>ソートフィルターモーダルのタイプ</summary>
        protected override ModalType SortFilterModalType => ModalType.CombinationTrainingSortFilter;
        
        public void Initialize()
        {
            // スキルリスト取得
            trainingList = CombinationManager.GetAllCombinationTrainingList();
            if (trainingList == null)
            {
                CruFramework.Logger.LogError("CombinationTrainingScrollDynamic: Initialize: trainingListの取得に失敗しました。");
                return;
            }

            // スキルリスト更新
            RefreshWithSortFilter();
        }

        public void Initialize(Dictionary<long, long> characterTupleDictionary, Dictionary<long, UserDataChara> mCharaIdPossessionDictionary, DateTime endTime)
        {
            trainingList = CombinationManager.GetActivatingCombinationTrainingListBefore(characterTupleDictionary, endTime);
            var scrollData = CreateScrollData(trainingList, mCharaIdPossessionDictionary);
            scrollDynamic.SetItems(scrollData);
            noActivatingCombinationText.SetActive(trainingList.Count == 0);
        }

        /// <summary>
        /// 指定編成で発動するスキル一覧を表示させる
        /// 自分以外のユーザーデータ流し込みを想定
        /// </summary>
        /// <param name="supportCharaList">育成時に編成するキャラのリスト</param>
        public void Initialize(IReadOnlyList<CharacterDetailData> supportCharaList)
        {
            trainingList = CombinationManager.GetAllCombinationTrainingList();
            var scrollData = CreateScrollData(trainingList, supportCharaList);
            noActivatingCombinationText.SetActive(!scrollData.Any());
            scrollDynamic.SetItems(scrollData);
        }

        private List<CombinationTrainingScrollData> CreateScrollData(List<CombinationManager.CombinationTraining> trainingData,  Dictionary<long, UserDataChara> mCharaIdPossessionDictionary)
        {
            var scrollData = new List<CombinationTrainingScrollData>();
            foreach (var combinationTraining in trainingData)
            {
                List<CharacterDetailData> detailDataList = new List<CharacterDetailData>();

                foreach (var mCharaId in combinationTraining.MCharaSortedIdList)
                {
                    var uChara = mCharaIdPossessionDictionary.ContainsKey(mCharaId)
                        ? mCharaIdPossessionDictionary[mCharaId]
                        : null;
                    var level = uChara?.level ?? 1;
                    var liberationLevel = uChara?.newLiberationLevel ?? 0;
                    CharacterDetailData detailData = new CharacterDetailData(mCharaId, level, liberationLevel);
                    detailDataList.Add(detailData);
                }

                // 発動済みのIdを取得
                var characterTupleDictionary = new Dictionary<long, long>();
                foreach(var uChara in mCharaIdPossessionDictionary.Values)
                {
                    characterTupleDictionary[uChara.MChara.id] = uChara.level;
                }
                var activatedMCharaTrainingComboBuffStatusIdList = combinationTraining.GetActivatingMCharaTrainingComboBuffStatusIdList(characterTupleDictionary);
                var activatedSkillDataList = new List<CombinationManager.TrainingMultipleSkillData>();
                var lockSkillDataList = new List<CombinationManager.TrainingMultipleSkillData>();
                bool isAddActivatedSkill = activatedMCharaTrainingComboBuffStatusIdList.Count > 0;
                bool isAddLockSkill = false;
                foreach (var mCharaTrainingComboBuffStatusId in combinationTraining.MCharaTrainingComboBuffStatusIdList)
                {
                    // 発動済みの場合はcontinue
                    if(activatedMCharaTrainingComboBuffStatusIdList.Contains(mCharaTrainingComboBuffStatusId)) continue;
                    
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if (mCharaTrainingComboBuffStatus == null) continue;

                    // 発動済みのスキルと１つでも未解放のスキルが追加されていたら未解放スキルは追加しない
                    if(isAddActivatedSkill && isAddLockSkill || isAddLockSkill) continue;
                    
                    // 解放不可能のデータを追加
                    lockSkillDataList.Add(new CombinationManager.TrainingMultipleSkillData(
                        PracticeSkillUtility.GetComboBuffPracticeSkill(mCharaTrainingComboBuffStatusId),false,
                        CombinationManager.GetLockString(
                            mCharaTrainingComboBuffStatus.requireLevel == combinationTraining.GetMinRequireLevel(),
                            mCharaTrainingComboBuffStatus.requireLevel)));
                    isAddLockSkill = true;
                }

                if (activatedMCharaTrainingComboBuffStatusIdList.Count > 0)
                {
                    var maxLevelMCharaTrainingComboBuffStatusId = GetMaxLevelMCharaTrainingComboBuffStatus(activatedMCharaTrainingComboBuffStatusIdList);
                    var maxLevelMCharaTrainingComboBuffStatus = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(maxLevelMCharaTrainingComboBuffStatusId);
                    if (maxLevelMCharaTrainingComboBuffStatus != null)
                    {
                        activatedSkillDataList.Add(new CombinationManager.TrainingMultipleSkillData(PracticeSkillUtility.GetComboBuffPracticeSkill(maxLevelMCharaTrainingComboBuffStatusId), true));
                    }
                }

                var data = new CombinationTrainingScrollData(combinationTraining.MCharaTrainingComboBuffId,
                    detailDataList, mCharaIdPossessionDictionary, activatedSkillDataList, lockSkillDataList,
                    CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>.NotActiveLabelType.Next,
                    true
                );
                
                scrollData.Add(data);
            }
            
            return scrollData;
        }

        /// <summary>
        /// 指定編成で発動するスキル一覧を生成する
        /// </summary>
        /// <param name="trainingData">検索対象</param>
        /// <param name="supportCharaList">育成時に編成するキャラのリスト</param>
        /// <returns></returns>
        private List<CombinationTrainingScrollData> CreateScrollData(List<CombinationManager.CombinationTraining> trainingData, IReadOnlyList<CharacterDetailData> supportCharaList)
        {
            var scrollData = new List<CombinationTrainingScrollData>();
            foreach (var combinationTraining in trainingData)
            {
                var detailDataList = CreateSkillActivationCharDetailDataList(combinationTraining.MCharaSortedIdList, supportCharaList);

                var characterTupleDictionary = supportCharaList
                    .GroupBy(item => item.MCharaId)
                    .ToDictionary(item => (long)item.FirstOrDefault().MCharaId, item => (long)item.FirstOrDefault().Lv);
                var activatedMCharaTrainingComboBuffStatusIdList = combinationTraining.GetActivatingMCharaTrainingComboBuffStatusIdList(characterTupleDictionary);
                
                // 発動スキルのないものは無視
                if (!activatedMCharaTrainingComboBuffStatusIdList.Any())
                {
                    continue;
                }
                
                var maxLevelMCharaTrainingComboBuffStatusId = GetMaxLevelMCharaTrainingComboBuffStatus(activatedMCharaTrainingComboBuffStatusIdList);
                var maxLevelMCharaTrainingComboBuffStatus = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(maxLevelMCharaTrainingComboBuffStatusId);
                var activatedSkillDataList = new List<CombinationManager.TrainingMultipleSkillData>();
                if (maxLevelMCharaTrainingComboBuffStatus != null)
                {
                    activatedSkillDataList.Add(new CombinationManager.TrainingMultipleSkillData(PracticeSkillUtility.GetComboBuffPracticeSkill(maxLevelMCharaTrainingComboBuffStatusId), true));
                }
                var data = new CombinationTrainingScrollData(combinationTraining.MCharaTrainingComboBuffId,
                    detailDataList, null, activatedSkillDataList, new List<CombinationManager.TrainingMultipleSkillData>(),
                    CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>.NotActiveLabelType.Next,
                    false
                );
                
                scrollData.Add(data);
            }
            
            return scrollData;
        }

        /// <summary>
        /// 指定キャラIDリストをCharacterDetailDataのリストに変換する
        /// </summary>
        /// <param name="skillActivationCharIdList">スキル発動に必要なキャラIDリスト</param>
        /// <param name="supportCharaList">育成時に編成するキャラのリスト</param>
        /// <returns>指定キャラのCharacterDetailDataリスト</returns>
        private static List<CharacterDetailData> CreateSkillActivationCharDetailDataList(List<long> skillActivationCharIdList, IReadOnlyList<CharacterDetailData> supportCharaList)
        {
            var detailDataList = new List<CharacterDetailData>();
            foreach (var mCharaId in skillActivationCharIdList)
            {
                var chara = supportCharaList.FirstOrDefault(item => item.MCharaId == mCharaId);
                if (chara == null) continue;

                var detailData = new CharacterDetailData(mCharaId, chara.Lv, chara.LiberationLevel);
                detailDataList.Add(detailData);
            }

            return detailDataList;
        }

        /// <summary>
        /// 指定スキル効果のうち、発動に必要なレベルが最も高いものを取得
        /// </summary>
        /// <param name="mCharaTrainingComboBuffStatusIdList">スキル効果IDリスト</param>
        /// <returns>発動に必要なレベルが最も高いスキル効果のID</returns>
        private static long GetMaxLevelMCharaTrainingComboBuffStatus(List<long> mCharaTrainingComboBuffStatusIdList)
        {
            long currentMaxLevelMCharaTrainingComboBuffStatusId = 0;
            long requireLevel = 0;
            foreach (var mCharaTrainingComboBuffStatusId in mCharaTrainingComboBuffStatusIdList)
            {
                var mCharaTrainingComboBuffStatus = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(mCharaTrainingComboBuffStatusId);
                if (mCharaTrainingComboBuffStatus == null || mCharaTrainingComboBuffStatus.requireLevel < requireLevel) continue;
                
                currentMaxLevelMCharaTrainingComboBuffStatusId = mCharaTrainingComboBuffStatusId;
                requireLevel = mCharaTrainingComboBuffStatus.requireLevel;
            }

            return currentMaxLevelMCharaTrainingComboBuffStatusId;
        }
        
        /// <summary>
        /// ソート・フィルター設定を適用してスクロールを更新する
        /// </summary>
        public override void RefreshWithSortFilter()
        {
            // trainingListがもしnullの場合は初期化処理に回す
            if (trainingList == null)
            {
                Initialize();
                return;
            }
            
            // セーブデータ取得
            CombinationSkillFilterData filterData = SortFilterUtility.GetFilterDataByType(SortFilterType) as CombinationSkillFilterData;
            CombinationSkillSortData sortData = SortFilterUtility.GetSortDataByType(SortFilterType) as CombinationSkillSortData;
            if (filterData == null || sortData == null)
            {
                CruFramework.Logger.LogError($"RefreshWithSortFilter: フィルターかソートのデータがNullです, sortFilterType={SortFilterType}");
                return;
            }
            
            // フィルター適用したスキルリスト
            List<CombinationManager.CombinationTraining> filteredUserCharaDataList = trainingList.GetFilterCombinationTrainingList(SortFilterType, filterData.selectedCharaIdList);
            // フィルター適用後にソート適用したスキルリスト
            List<CombinationManager.CombinationTraining> sortedUserCharaDataList = filteredUserCharaDataList.GetSortCombinationTrainingList(SortFilterType);
            // スクロールデータ作成
            List<CombinationTrainingScrollData> scrollData = CreateScrollData(sortedUserCharaDataList, mCharaIdPossessionDictionary);

            // テキスト更新
            noActivatingCombinationText.SetActive(scrollData.Count == 0);
            
            // スクロール更新
            scrollDynamic.SetItems(scrollData);
            
            // ソート・フィルター表示を更新
            UpdateSortText();
            UpdateFilterText();
        }

        protected override SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data CreateSortFilterModalData()
        {
            List<long> displayCharacterIdList = GetDisplayCharacterIdList(trainingList);
            
            return new BaseCombinationSkillSortFilterModal<CombinationManager.CombinationTraining>.Data(
                SortFilterSheetType.Filter,
                SortFilterType,
                displayCharacterIdList);
        }
    }
}
