using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Combination
{
    /// <summary>
    /// スキルデータリストの集合を保持するクラス
    /// </summary>
    public class SkillDataListCollection
    {
        /// <summary>解放済みスキルのデータリスト</summary>
        public readonly List<CombinationManager.CollectionMultipleSkillData> ActivatedSkillDataList;
        
        /// <summary>解放可能スキルのデータリスト</summary>
        public readonly List<CombinationManager.CollectionMultipleSkillData> CanActiveSkillDataList;
        
        /// <summary>ロック済みスキルのデータリスト</summary>
        public readonly List<CombinationManager.CollectionMultipleSkillData> LockSkillDataList;
        
        /// <summary>解放済みスキルのラベルタイプ</summary>
        public readonly CombinationCollectionView.ActivatedLabelType ActivatedLabelType;
        
        /// <summary>未解放スキルのラベルタイプ</summary>
        public readonly CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType NotActiveLabelType;

        public SkillDataListCollection(
            List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList,
            List<CombinationManager.CollectionMultipleSkillData> canActiveSkillDataList,
            List<CombinationManager.CollectionMultipleSkillData> lockSkillDataList,
            CombinationCollectionView.ActivatedLabelType activatedLabelType = CombinationCollectionView.ActivatedLabelType.Total,
            CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType notActiveLabelType =
                CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType.Next)
        {
            ActivatedSkillDataList = activatedSkillDataList;
            CanActiveSkillDataList = canActiveSkillDataList;
            LockSkillDataList = lockSkillDataList;
            ActivatedLabelType = activatedLabelType;
            NotActiveLabelType = notActiveLabelType;
        }
    }

    /// <summary>
    /// コンビネーションコレクションのスキルデータを作成するためのクラス
    /// </summary>
    public class CombinationCollectionDataBuilder
    {
        /// <summary>
        /// キャラクター詳細データリストを作成する
        /// </summary>
        /// <param name="mCharaIdList">キャラクターIDリスト</param>
        /// <param name="charaPossessionDictionary">所持キャラクターのDictionary</param>
        /// <returns>キャラクター詳細データリスト</returns>
        public List<CharacterDetailData> CreateCharacterDetailDataList(IReadOnlyList<long> mCharaIdList, IReadOnlyDictionary<long, UserDataChara> charaPossessionDictionary)
        {
            List<CharacterDetailData> detailDataList = new();
            
            foreach (long mCharaId in mCharaIdList)
            {
                UserDataChara uChara = charaPossessionDictionary.ContainsKey(mCharaId) ? charaPossessionDictionary[mCharaId] : null;
                long level = Math.Max(uChara?.level ?? 0, 1);
                long liberationLevel = Math.Max(uChara?.newLiberationLevel ?? 0, 0);
                CharacterDetailData detailData = new CharacterDetailData(mCharaId, level, liberationLevel);
                detailDataList.Add(detailData);
            }
            
            return detailDataList;
        }

        /// <summary>
        /// スキル発動用のキャラクター詳細データリストを作成する
        /// </summary>
        /// <param name="mCharaIdList">キャラクターIDリスト</param>
        /// <returns>キャラクター詳細データリスト</returns>
        public List<CharacterDetailData> CreateSkillActivationCharDetailDataList(IReadOnlyList<long> mCharaIdList)
        {
            List<CharacterDetailData> detailDataList = new();
            
            foreach (long mCharaId in mCharaIdList)
            {
                CharacterDetailData detailData = new CharacterDetailData(mCharaId, 0, 0);
                detailDataList.Add(detailData);
            }
            
            return detailDataList;
        }

        /// <summary>
        /// コンビネーション進捗マスターとトレーニングステータスを取得する
        /// </summary>
        private bool TryGetProgressAndTrainingStatus(
            long mCombinationProgressId,
            long mCombinationId,
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache,
            out CombinationProgressMasterObject mCombinationProgress,
            out CombinationTrainingStatusMasterObject mCombinationTrainingStatus)
        {
            mCombinationProgress = MasterManager.Instance.combinationProgressMaster.FindData(mCombinationProgressId);
            mCombinationTrainingStatus = null;
            
            if (mCombinationProgress == null) return false;

            long nextLevel = mCombinationProgress.level + 1;
            
            if (!trainingStatusCache.TryGetValue(mCombinationId, out IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject> mCombinationTrainingStatusList)) return false;
            if (!mCombinationTrainingStatusList.TryGetValue(nextLevel, out mCombinationTrainingStatus)) return false;
            
            return true;
        }

        /// <summary>
        /// 解放可能かどうかを判定する
        /// </summary>
        private bool CanActivateSkill(
            IReadOnlyList<long> mCharaIdList,
            IReadOnlyDictionary<long, UserDataChara> charaPossessionDictionary,
            CombinationProgressMasterObject mCombinationProgress)
        {
            foreach (long mCharaId in mCharaIdList)
            {
                if (!charaPossessionDictionary.TryGetValue(mCharaId, out UserDataChara uChara) || 
                    uChara.level < mCombinationProgress.conditionValueSub)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// スキルデータリスト（解放済み/解放可能/ロック）を作成する
        /// </summary>
        /// <param name="combinationCollection">コンビネーションコレクション</param>
        /// <param name="charaPossessionDictionary">所持キャラクターのDictionary</param>
        /// <param name="trainingStatusCache">トレーニングステータスのキャッシュ</param>
        /// <param name="activatedLabelType">発動スキルのラベルタイプ</param>
        /// <param name="notActiveLabelType">未発動スキルのラベルタイプ</param>
        /// <returns>解放済み、解放可能、ロック済みのスキルデータリストを保持するオブジェクト</returns>
        public SkillDataListCollection CreateSkillDataLists(
            CombinationManager.CombinationCollection combinationCollection,
            IReadOnlyDictionary<long, UserDataChara> charaPossessionDictionary,
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache,
            CombinationCollectionView.ActivatedLabelType activatedLabelType,
            CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType notActiveLabelType)
        {
            long currentMaxLevelActivatedProgressLevel = combinationCollection.GetActivatedMaxProgressLevel();
            
            List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList = new();
            List<CombinationManager.CollectionMultipleSkillData> canActiveSkillDataList = new();
            List<CombinationManager.CollectionMultipleSkillData> lockSkillDataList = new();
            
            bool isAddActivatedSkill = false;
            bool isAddCanActiveSkill = false;
            bool isAddLockSkill = false;
            
            foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
            {
                if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, trainingStatusCache, out var mCombinationProgress, out var mCombinationTrainingStatus)) continue;
                
                // 解放済みかどうかを確認
                if (combinationCollection.IsActivatedProgressLevel(mCombinationTrainingStatus.progressLevel))
                {
                    if (mCombinationTrainingStatus.progressLevel < currentMaxLevelActivatedProgressLevel) continue;
                    
                    activatedSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                        mCombinationProgress.id,
                        PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                        false, true));
                    currentMaxLevelActivatedProgressLevel = mCombinationTrainingStatus.progressLevel;
                    isAddActivatedSkill = true;
                    continue;
                }

                // 解放可能かどうかを判定（解放可能スキルが未追加の場合のみ）
                if (canActiveSkillDataList.Count <= 0 && CanActivateSkill(combinationCollection.MCharaIdList, charaPossessionDictionary, mCombinationProgress))
                {
                    canActiveSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                        mCombinationProgressId,
                        PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id),
                        true, false));
                    isAddCanActiveSkill = true;
                    continue;
                }
                
                // 解放済みと解放可能スキルが両方ある場合はスキップ
                if (isAddActivatedSkill && isAddCanActiveSkill) continue;
                //未解放スキルが既に追加されている場合はスキップ
                if(isAddLockSkill) continue;
                
                // 解放不可能のスキルデータを追加
                lockSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                    mCombinationProgressId,
                    PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                    true, 
                    false,
                    CombinationManager.GetLockString(
                        mCombinationTrainingStatus.progressLevel == CombinationManager.CombinationCollection.MinProgressLevel,
                        mCombinationProgress.conditionValueSub)));
                isAddLockSkill = true;
            }

            return new SkillDataListCollection(activatedSkillDataList, canActiveSkillDataList, lockSkillDataList, activatedLabelType, notActiveLabelType);
        }
        
        /// <summary>
        /// 解放済みスキルのみのデータリストを作成する（openedCollections用）
        /// </summary>
        /// <param name="combinationCollection">コンビネーションコレクション</param>
        /// <param name="trainingStatusCache">トレーニングステータスのキャッシュ</param>
        /// <param name="openedCollectionsDict">解放済みコレクションのDictionary</param>
        /// <returns>解放済みスキルのみを保持するSkillDataListCollection</returns>
        public SkillDataListCollection CreateActivatedOnlySkillDataLists(
            CombinationManager.CombinationCollection combinationCollection,
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache,
            Dictionary<long, CombinationOpenedMinimum> openedCollectionsDict)
        {
            long currentMaxLevelActivatedProgressLevel = combinationCollection.GetActivatedMaxProgressLevel(openedCollectionsDict);
            List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList = new();
            
            foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
            {
                if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, trainingStatusCache, out var mCombinationProgress, out var mCombinationTrainingStatus)) continue;
                
                // 解放済みかどうかを確認
                if (combinationCollection.IsActivatedProgressLevel(openedCollectionsDict, mCombinationTrainingStatus.progressLevel))
                {
                    if (mCombinationTrainingStatus.progressLevel < currentMaxLevelActivatedProgressLevel) continue;
                    
                    activatedSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                        mCombinationProgress.id,
                        PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                        false, 
                        true));
                    currentMaxLevelActivatedProgressLevel = mCombinationTrainingStatus.progressLevel;
                }
            }

            return new SkillDataListCollection(
                activatedSkillDataList,
                new List<CombinationManager.CollectionMultipleSkillData>(),
                new List<CombinationManager.CollectionMultipleSkillData>());
        }
        
        /// <summary>
        /// スキルデータリストを作成する（単体表示オプション付き）
        /// </summary>
        /// <param name="combinationCollection">コンビネーションコレクション</param>
        /// <param name="charaPossessionDictionary">所持キャラクターのDictionary</param>
        /// <param name="trainingStatusCache">トレーニングステータスのキャッシュ</param>
        /// <param name="showSingleOnly">単体のみ表示するか（true: 単体のみ / false: 全件表示）</param>
        /// <returns>スキルデータリスト</returns>
        public SkillDataListCollection CreateSkillDataListsWithLimit(
            CombinationManager.CombinationCollection combinationCollection,
            IReadOnlyDictionary<long, UserDataChara> charaPossessionDictionary,
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache,
            bool showSingleOnly)
        {
            long currentMaxLevelActivatedProgressLevel = combinationCollection.GetActivatedMaxProgressLevel();
            List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList = new();
            List<CombinationManager.CollectionMultipleSkillData> canActiveSkillDataList = new();
            List<CombinationManager.CollectionMultipleSkillData> lockSkillDataList = new();
            
            bool isAddActivatedSkill = false;
            bool isAddCanActiveSkill = false;
            bool isAddLockSkill = false;
            
            foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
            {
                if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, trainingStatusCache, out var mCombinationProgress, out var mCombinationTrainingStatus)) continue;
                
                // 解放済みかどうかを確認
                if (combinationCollection.IsActivatedProgressLevel(mCombinationTrainingStatus.progressLevel))
                {
                    if (mCombinationTrainingStatus.progressLevel < currentMaxLevelActivatedProgressLevel) continue;
                    
                    activatedSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                        mCombinationProgress.id,
                        PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                        false, 
                        true));
                    currentMaxLevelActivatedProgressLevel = mCombinationTrainingStatus.progressLevel;
                    isAddActivatedSkill = true;
                    continue;
                }

                // 解放可能かどうかを判定
                if (canActiveSkillDataList.Count <= 0 && CanActivateSkill(combinationCollection.MCharaIdList, charaPossessionDictionary, mCombinationProgress))
                {
                    canActiveSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                        mCombinationProgressId,
                        PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id),
                        true,
                        false));
                    isAddCanActiveSkill = true;
                    continue;
                }
                
                // 解放済みと解放可能スキルが両方ある場合はスキップ
                if (showSingleOnly && isAddActivatedSkill && isAddCanActiveSkill) continue;
                // 既に未解放スキルが追加されている場合はスキップ
                if (showSingleOnly && isAddLockSkill) continue;
                
                // 解放不可能のスキルデータを追加
                lockSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(
                    mCombinationProgressId,
                    PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                    true, 
                    false,
                    CombinationManager.GetLockString(
                        mCombinationTrainingStatus.progressLevel == CombinationManager.CombinationCollection.MinProgressLevel,
                        mCombinationProgress.conditionValueSub)));
                isAddLockSkill = true;
            }

            return new SkillDataListCollection(activatedSkillDataList, canActiveSkillDataList, lockSkillDataList);
        }
    }
}


