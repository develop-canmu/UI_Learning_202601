using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CombinationCollectionScrollDynamic : CombinationScrollDynamicBase
    {
        private List<CombinationManager.CombinationCollection> collectionList;
        
        /// <summary>このクラスで使用するソートフィルタータイプ</summary>
        protected override SortFilterUtility.SortFilterType SortFilterType => SortFilterUtility.SortFilterType.ListCombinationCollection;
        
        /// <summary>ソートフィルターモーダルのモーダルタイプ</summary>
        protected override ModalType SortFilterModalType => ModalType.CombinationCollectionSortFilter;
        
        /// <summary>スキルデータ作成を担当するスクリプト</summary>
        private readonly CombinationCollectionDataBuilder combinationCollectionDataBuilder = new();
        
        /// <summary>マスターキャラクターIDをキーとした所持キャラクター辞書（キャッシュ用）</summary>
        private IReadOnlyDictionary<long, UserDataChara> mCharaIdPossessionDictionary = UserDataManager.Instance.CharaDataByMCharaIdContainer.data;
        
        /// <summary>レベル段階更新時のコールバック（キャッシュ用）</summary>
        private Action<long> cachedLevelStageCallback;
        

        /// <summary>
        /// 全てのコンビネーションコレクションを表示する
        /// </summary>
        /// <param name="onProgressCombinationCollectionAction">レベル段階更新時のコールバック</param>
        public void InitializeAll(Action<long> onProgressCombinationCollectionAction = null)
        {
            cachedLevelStageCallback = onProgressCombinationCollectionAction;
            
            // データ取得（nullまたは空リストの場合は再取得）
            if (collectionList == null || collectionList.Count == 0)
            {
                // 全コレクションスキルリストを取得
                collectionList = CombinationManager.GetAllCombinationCollectionList();
            }
            
            if (collectionList == null)
            {
                CruFramework.Logger.LogError("コレクションスキルリストの取得に失敗しました");
                return;
            }

            // ソート設定を適用
            RefreshWithSortFilter();
        }
        
        /// <summary>
        /// 指定されたコンビネーションコレクション1件を表示する
        /// </summary>
        /// <param name="combinationCollection">表示するコンビネーションコレクション</param>
        /// <param name="onProgressCombinationCollectionAction">レベル段階更新時のコールバック</param>
        public void InitializeSelect(CombinationManager.CombinationCollection combinationCollection, Action<long> onProgressCombinationCollectionAction = null)
        {
            noActivatingCombinationText.SetActive(false);
            collectionList = CombinationManager.GetAllCombinationCollectionList();
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            
            // キャラクター詳細データリストを作成
            List<CharacterDetailData> detailDataList = combinationCollectionDataBuilder.CreateCharacterDetailDataList(combinationCollection.MCharaIdList, mCharaIdPossessionDictionary);

            // トレーニングステータスマスターのキャッシュを作成
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache = MasterManager.Instance.combinationTrainingStatusMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>)x.ToDictionary(y => y.progressLevel, y => y));

            // スキルデータリスト（解放済み/解放可能/ロック）を作成
            SkillDataListCollection skillDataLists = combinationCollectionDataBuilder.CreateSkillDataLists(
                combinationCollection,
                mCharaIdPossessionDictionary,
                trainingStatusCache,
                CombinationCollectionView.ActivatedLabelType.Total,
                CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType.Total);

            CombinationCollectionScrollData data = new CombinationCollectionScrollData(combinationCollection.MCombinationId, false, false, true,
                detailDataList, mCharaIdPossessionDictionary, skillDataLists, 
                id =>
                {
                    UpdateAllSkillView(id);
                    onProgressCombinationCollectionAction?.Invoke(id);
                });

            scrollData.Add(data);
            
            scrollDynamic.SetItems(scrollData);
        }

        /// <summary>
        /// 指定スキルを開放済状態で一覧表示する（未開放スキルの表示は行わない）
        /// </summary>
        /// <param name="openedCollections">発動スキルリスト</param>
        public void InitializeSelectOpend(IReadOnlyList<CombinationOpenedMinimum> openedCollections)
        {
            noActivatingCombinationText.SetActive(!openedCollections.Any());
            collectionList = CombinationManager.GetAllCombinationCollectionList().Where(item => openedCollections.Any(item2 => item2.mCombinationId == item.MCombinationId)).ToList();

            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> combinationTrainingStatusMasterGroupCache = MasterManager.Instance.combinationTrainingStatusMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>)x.ToDictionary(y => y.progressLevel, y => y));
            
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            Dictionary<long, CombinationOpenedMinimum> openedCollectionsDict = openedCollections.ToDictionary(x => x.mCombinationId, x => x);
            
            foreach (CombinationManager.CombinationCollection combinationCollection in collectionList)
            {
                List<CharacterDetailData> detailDataList = combinationCollectionDataBuilder.CreateSkillActivationCharDetailDataList(combinationCollection.MCharaIdList);

                // スキルデータリスト（解放済みのみ）を作成
                SkillDataListCollection skillDataLists = combinationCollectionDataBuilder.CreateActivatedOnlySkillDataLists(
                    combinationCollection,
                    combinationTrainingStatusMasterGroupCache,
                    openedCollectionsDict);

                CombinationCollectionScrollData data = new CombinationCollectionScrollData(combinationCollection.MCombinationId, false, false, false, detailDataList, null, skillDataLists);

                scrollData.Add(data);
            }
            
            scrollDynamic.SetItems(scrollData);
        }

        /// <summary>
        /// 指定されたコンビネーションIDのスキル表示を更新する（単体のみ）
        /// </summary>
        /// <param name="levelStageId">更新対象のコンビネーションID</param>
        private void UpdateSkillView(long levelStageId)
        {
            UpdateSkillView(new HashSet<long>() { levelStageId });
        }
        
        /// <summary>
        /// 指定されたコンビネーションIDのスキル表示を更新する（全件表示）
        /// </summary>
        /// <param name="levelStageId">更新対象のコンビネーションID</param>
        private void UpdateAllSkillView(long levelStageId)
        {
            UpdateAllSkillView(new HashSet<long>() { levelStageId });
        }
        
        /// <summary>
        /// 指定されたコンビネーションIDリストのスキル表示を更新する（単体のみ）
        /// </summary>
        /// <param name="levelStageIdHashSet">更新対象のコンビネーションIDセット</param>
        public void UpdateSkillView(HashSet<long> levelStageIdHashSet)
        {
            UpdateSkillViewCore(levelStageIdHashSet, showSingleOnly: true);
        }
        
        /// <summary>
        /// 指定されたコンビネーションIDリストのスキル表示を更新する（全件表示）
        /// </summary>
        /// <param name="levelStageIdHashSet">更新対象のコンビネーションIDセット</param>
        public void UpdateAllSkillView(HashSet<long> levelStageIdHashSet)
        {
            UpdateSkillViewCore(levelStageIdHashSet, showSingleOnly: false);
        }
        
        /// <summary>
        /// スキル表示を更新するコア処理
        /// </summary>
        /// <param name="levelStageIdHashSet">更新対象のコンビネーションIDセット</param>
        /// <param name="showSingleOnly">単体のみ表示するか（true: 単体のみ / false: 全件表示）</param>
        private void UpdateSkillViewCore(HashSet<long> levelStageIdHashSet, bool showSingleOnly)
        {
            // アイテムリストが空の場合は処理しない
            if (scrollDynamic.ItemList == null || scrollDynamic.ItemList.Count == 0)
            {
                return;
            }
            Dictionary<long, CombinationManager.CombinationCollection> dic = collectionList.ToDictionary(x => x.MCombinationId, x => x);

            // トレーニングステータスマスターのキャッシュを作成
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> trainingStatusCache = MasterManager.Instance.combinationTrainingStatusMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>)x.ToDictionary(y => y.progressLevel, y => y));

            foreach (CombinationCollectionScrollData data in scrollDynamic.ItemList)
            {
                if (!levelStageIdHashSet.Contains(data.MCombinationId)) continue;
                if (dic.TryGetValue(data.MCombinationId, out CombinationManager.CombinationCollection combinationCollection))
                {
                    // スキルデータリストを作成（単体のみ/全件）
                    data.SkillDataLists = combinationCollectionDataBuilder.CreateSkillDataListsWithLimit(
                        combinationCollection,
                        mCharaIdPossessionDictionary,
                        trainingStatusCache,
                        showSingleOnly);
                }
            }

            scrollDynamic.RefreshItemView();
        }
        
        /// <summary>
        /// 発動中のコンビネーションコレクションを表示する
        /// </summary>
        public void InitializeActivating()
        {
            collectionList = CombinationManager.GetActivatedCombinationCollectionList();
            List<CombinationCollectionScrollData> scrollData = CreateActivatingCombinationScrollData(collectionList);
            scrollDynamic.SetItems(scrollData);
            noActivatingCombinationText.SetActive(collectionList.Count == 0);
        }

        /// <summary>
        /// 指定されたレベル段階データに基づいて発動中のコンビネーションコレクションを表示する
        /// </summary>
        /// <param name="collectionLevelStageDataList">レベル段階データリスト</param>
        public void InitializeActivating(List<CombinationManager.CollectionProgressData> collectionLevelStageDataList)
        {
            collectionList = CombinationManager.GetAllCombinationCollectionList();
            List<CombinationCollectionScrollData> scrollData = CreateActivatingCombinationScrollDataByLevelStage(collectionList, collectionLevelStageDataList);
           
            scrollDynamic.SetItems(scrollData);
            noActivatingCombinationText.SetActive(collectionList.Count == 0);
        }
        
        /// <summary>
        /// キャラクターによって解放可能なコンビネーションコレクションを表示する
        /// </summary>
        /// <param name="combinationCollectionList">コンビネーションコレクションリスト</param>
        /// <param name="levelStageDataList">レベル段階データリスト</param>
        public void InitializeCanActivateByChara(List<CombinationManager.CombinationCollection> combinationCollectionList, List<CombinationManager.CollectionProgressData> levelStageDataList)
        {
            collectionList = combinationCollectionList;
            List<CombinationCollectionScrollData> scrollData = CreateCanActivateCombinationScrollDataByLevelStage(collectionList, levelStageDataList);

            scrollDynamic.SetItems(scrollData);
            noActivatingCombinationText.SetActive(collectionList.Count == 0);
        }

        /// <summary>
        /// コンビネーションレベル段階マスターとトレーニングステータスを取得する
        /// </summary>
        /// <param name="mCombinationProgressId">コンビネーションレベル段階ID</param>
        /// <param name="mCombinationId">コンビネーションID</param>
        /// <param name="mCombinationProgress">取得したレベル段階マスター（出力）</param>
        /// <param name="mCombinationTrainingStatus">取得したトレーニングステータス（出力）</param>
        /// <returns>両方取得できた場合true</returns>
        private bool TryGetProgressAndTrainingStatus(
            long mCombinationProgressId,
            long mCombinationId,
            out CombinationProgressMasterObject mCombinationProgress,
            out CombinationTrainingStatusMasterObject mCombinationTrainingStatus)
        {
            mCombinationProgress = MasterManager.Instance.combinationProgressMaster.FindData(mCombinationProgressId);
            mCombinationTrainingStatus = null;
            
            if (mCombinationProgress == null) return false;

            //progress側の定義：「段階　0スタート　「そのレベルから次のレベルに強化するときの条件」の設定を入れる」
            //training_status側の定義：「「現在の」達成度レベル　レベルは初期状態では0だが、レベル0の場合はこのデータが存在しても効果の発動は行わない」
            //上記よりtraining_statusを取得する際はprogressのlevelに+1したものを渡す
            long nextLevel = mCombinationProgress.level + 1;
            mCombinationTrainingStatus = MasterManager.Instance.combinationTrainingStatusMaster.FindDataByCombinationIdAndProgressLevel(mCombinationId, nextLevel);
            
            return mCombinationTrainingStatus != null;
        }
        
        /// <summary>
        /// 発動中のコンビネーションスクロールデータを作成する
        /// </summary>
        /// <param name="collectionData">コンビネーションコレクションリスト</param>
        /// <returns>スクロールデータリスト</returns>
        private List<CombinationCollectionScrollData> CreateActivatingCombinationScrollData(List<CombinationManager.CombinationCollection> collectionData)
        {
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            foreach (CombinationManager.CombinationCollection combinationCollection in collectionData)
            {
                List<CharacterDetailData> detailDataList = new List<CharacterDetailData>();

                foreach (long mCharaId in combinationCollection.MCharaIdList)
                {
                    UserDataChara uChara = mCharaIdPossessionDictionary.ContainsKey(mCharaId)
                        ? mCharaIdPossessionDictionary[mCharaId]
                        : null;
                    long level = uChara?.level ?? 1;
                    long liberationLevel = uChara?.newLiberationLevel ?? 0;
                    CharacterDetailData detailData = new CharacterDetailData(mCharaId,
                        level, liberationLevel);
                    detailDataList.Add(detailData);
                }

                long currentMaxLevelActivatedProgressLevel = combinationCollection.GetActivatedMaxProgressLevel();
                List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList = new List<CombinationManager.CollectionMultipleSkillData>();
                foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
                {
                    if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, out CombinationProgressMasterObject mCombinationProgress, out CombinationTrainingStatusMasterObject mCombinationTrainingStatus)) continue;

                    // 解放済みかどうかを確認
                    if (combinationCollection.IsActivatedProgressLevel(mCombinationTrainingStatus.progressLevel))
                    {
                        // 現在解放しているレベルより下の場合はcontinue
                        if(mCombinationTrainingStatus.progressLevel < currentMaxLevelActivatedProgressLevel) continue;
                        // 解放済みのデータを作成
                        activatedSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(mCombinationProgress.id, PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), false, true));
                        currentMaxLevelActivatedProgressLevel = mCombinationTrainingStatus.progressLevel;
                    }
                }

                SkillDataListCollection skillDataLists = new SkillDataListCollection(
                    activatedSkillDataList,
                    new List<CombinationManager.CollectionMultipleSkillData>(),
                    new List<CombinationManager.CollectionMultipleSkillData>());

                CombinationCollectionScrollData data = new CombinationCollectionScrollData(combinationCollection.MCombinationId, false, true, true, detailDataList, mCharaIdPossessionDictionary, skillDataLists);
                
                scrollData.Add(data);
            }

            return scrollData;
        }

        /// <summary>
        /// レベル段階データに基づいて発動中のコンビネーションスクロールデータを作成する
        /// </summary>
        /// <param name="collectionData">コンビネーションコレクションリスト</param>
        /// <param name="levelStageDataList">レベル段階データリスト</param>
        /// <returns>スクロールデータリスト</returns>
        private List<CombinationCollectionScrollData> CreateActivatingCombinationScrollDataByLevelStage(List<CombinationManager.CombinationCollection> collectionData, List<CombinationManager.CollectionProgressData> levelStageDataList)
        {
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            foreach (CombinationManager.CombinationCollection combinationCollection in collectionData)
            {
                CombinationManager.CollectionProgressData levelStageData = levelStageDataList.FirstOrDefault(data => data.MCombinationId == combinationCollection.MCombinationId);
                if(levelStageData == null) continue;
                
                List<CharacterDetailData> detailDataList = new List<CharacterDetailData>();

                foreach (long mCharaId in combinationCollection.MCharaIdList)
                {
                    UserDataChara uChara = mCharaIdPossessionDictionary.ContainsKey(mCharaId)
                        ? mCharaIdPossessionDictionary[mCharaId]
                        : null;
                    long level = uChara?.level ?? 1;
                    long liberationLevel = uChara?.newLiberationLevel ?? 0;
                    CharacterDetailData detailData = new CharacterDetailData(mCharaId, level, liberationLevel);
                    detailDataList.Add(detailData);
                }

                long currentMaxLevelActivatedProgressLevel = combinationCollection.GetActivatedMaxProgressLevel();
                List<CombinationManager.CollectionMultipleSkillData> activatedSkillDataList = new List<CombinationManager.CollectionMultipleSkillData>();
                foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
                {
                    if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, out CombinationProgressMasterObject mCombinationProgress, out CombinationTrainingStatusMasterObject mCombinationTrainingStatus)) continue;
                    
                    // 解放したレベルでない場合はcontinue
                    if(!levelStageData.ProgressIdList.Contains(mCombinationProgress.id)) continue;

                    // 解放済みの場合はMCombinationTrainingStatusIdを保持
                    if (combinationCollection.IsActivatedProgressLevel(mCombinationTrainingStatus.progressLevel))
                    {
                        // 現在解放しているレベルより下の場合はcontinue
                        if(mCombinationTrainingStatus.progressLevel < currentMaxLevelActivatedProgressLevel) continue;
                        // 解放済みのデータを作成
                        activatedSkillDataList.Add(new CombinationManager.CollectionMultipleSkillData(mCombinationProgress.id,
                            PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), 
                            false, 
                            false));
                        currentMaxLevelActivatedProgressLevel = mCombinationTrainingStatus.progressLevel;
                    }
                }

                SkillDataListCollection skillDataLists = new SkillDataListCollection(
                    activatedSkillDataList,
                    new List<CombinationManager.CollectionMultipleSkillData>(),
                    new List<CombinationManager.CollectionMultipleSkillData>(),
                    CombinationCollectionView.ActivatedLabelType.UnLock);

                CombinationCollectionScrollData data = new CombinationCollectionScrollData(combinationCollection.MCombinationId, 
                    false, 
                    false,
                    true,
                    detailDataList, 
                    mCharaIdPossessionDictionary,
                    skillDataLists);
                
                scrollData.Add(data);
            }

            return scrollData;
        }
        
        /// <summary>
        /// レベル段階データに基づいて解放可能なコンビネーションスクロールデータを作成する
        /// </summary>
        /// <param name="collectionData">コンビネーションコレクションリスト</param>
        /// <param name="levelStageDataList">レベル段階データリスト</param>
        /// <returns>スクロールデータリスト</returns>
        private List<CombinationCollectionScrollData> CreateCanActivateCombinationScrollDataByLevelStage(List<CombinationManager.CombinationCollection> collectionData,
            List<CombinationManager.CollectionProgressData> levelStageDataList)
        {
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            foreach (CombinationManager.CombinationCollection combinationCollection in collectionData)
            {
                CombinationManager.CollectionProgressData levelStageData =
                    levelStageDataList.FirstOrDefault(data =>
                        data.MCombinationId == combinationCollection.MCombinationId);
                if(levelStageData == null) continue;
                
                List<CharacterDetailData> detailDataList = new List<CharacterDetailData>();

                foreach (long mCharaId in combinationCollection.MCharaIdList)
                {
                    UserDataChara uChara = mCharaIdPossessionDictionary.ContainsKey(mCharaId)
                        ? mCharaIdPossessionDictionary[mCharaId]
                        : null;
                    long level = uChara?.level ?? 1;
                    long liberationLevel = uChara?.newLiberationLevel ?? 0;
                    CharacterDetailData detailData = new CharacterDetailData(mCharaId, level, liberationLevel);
                    detailDataList.Add(detailData);
                }

                // 解放可能のデータを追加
                List<CombinationManager.CollectionMultipleSkillData> canActiveSkillDataList = new List<CombinationManager.CollectionMultipleSkillData>();
                foreach (long mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
                {
                    if (!TryGetProgressAndTrainingStatus(mCombinationProgressId, combinationCollection.MCombinationId, out CombinationProgressMasterObject mCombinationProgress, out CombinationTrainingStatusMasterObject mCombinationTrainingStatus)) continue;
                    
                    // 解放可能になったレベルでない場合はcontinue
                    if(!levelStageData.ProgressIdList.Contains(mCombinationProgress.id)) continue;

                    canActiveSkillDataList.Add(
                        new CombinationManager.CollectionMultipleSkillData(mCombinationProgressId,
                            PracticeSkillUtility.GetCombinationPracticeSkill(mCombinationTrainingStatus.id), false, false));
                }

                SkillDataListCollection skillDataLists = new SkillDataListCollection(
                    new List<CombinationManager.CollectionMultipleSkillData>(), 
                    canActiveSkillDataList,
                    new List<CombinationManager.CollectionMultipleSkillData>());

                CombinationCollectionScrollData data = new CombinationCollectionScrollData(combinationCollection.MCombinationId, false, false, true, detailDataList, mCharaIdPossessionDictionary, skillDataLists);
                
                scrollData.Add(data);
            }

            return scrollData;
        }

        /// <summary>
        /// スクロールビューを更新する
        /// </summary>
        public void Refresh()
        {
            scrollDynamic.Refresh();
        }
        
        /// <summary>
        /// ソート・フィルター設定を適用してスクロールを更新する
        /// </summary>
        public override void RefreshWithSortFilter()
        {
            // collectionListがもしnullの場合は初期化処理に回す
            if (collectionList == null)
            {
                InitializeAll();
                return;
            }
            
            // セーブデータからフィルター・ソート設定を取得
            CombinationSkillFilterData filterData = SortFilterUtility.GetFilterDataByType(SortFilterType) as CombinationSkillFilterData;
            CombinationSkillSortData sortData = SortFilterUtility.GetSortDataByType(SortFilterType) as CombinationSkillSortData;
            if (filterData == null || sortData == null)
            {
                CruFramework.Logger.LogError("RefreshWithSortFilter: フィルターかソートのデータがNullです, sortFilterType=" + SortFilterType);
                return;
            }
            
            // フィルターを適用
            List<CombinationManager.CombinationCollection> filteredList = collectionList.GetFilterCombinationCollectionList(SortFilterType, filterData.selectedCharaIdList);
            
            // ソートを適用
            List<CombinationManager.CombinationCollection> sortedList = filteredList.GetSortCombinationCollectionList(SortFilterType);
            
            // スクロールデータを作成
            List<CombinationCollectionScrollData> scrollData = CreateScrollDataFromCollectionList(sortedList);
            
            // テキスト更新
            noActivatingCombinationText.SetActive(scrollData.Count == 0);
            
            // スクロール全体を更新
            scrollDynamic.SetItems(scrollData);
            
            // ソート・フィルター表示を更新
            UpdateSortText();
            UpdateFilterText();
        }

        /// <summary>
        /// コンビネーションコレクションリストからスクロールデータを作成する
        /// </summary>
        /// <param name="collectionList">コンビネーションコレクションリスト</param>
        /// <param name="onProgressCombinationCollectionAction">レベル段階更新時のコールバック</param>
        /// <returns>スクロールデータリスト</returns>
        private List<CombinationCollectionScrollData> CreateScrollDataFromCollectionList(List<CombinationManager.CombinationCollection> collectionList, Action<long> onProgressCombinationCollectionAction = null)
        {
            // スクロールデータリストを初期化
            List<CombinationCollectionScrollData> scrollData = new List<CombinationCollectionScrollData>();
            
            // トレーニングステータスマスターのキャッシュを作成
            IReadOnlyDictionary<long, IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>> combinationTrainingStatusMasterGroupCache = MasterManager.Instance.combinationTrainingStatusMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, x => (IReadOnlyDictionary<long, CombinationTrainingStatusMasterObject>)x.ToDictionary(y => y.progressLevel, y => y));

            // 各コンビネーションコレクションのスクロールデータを作成
            foreach (CombinationManager.CombinationCollection combinationCollection in collectionList)
            {
                // キャラクター詳細データリストを作成
                List<CharacterDetailData> detailDataList = combinationCollectionDataBuilder.CreateCharacterDetailDataList(combinationCollection.MCharaIdList, mCharaIdPossessionDictionary);
                
                // スキルデータリスト（解放済み/解放可能/ロック）を作成
                SkillDataListCollection skillDataLists = combinationCollectionDataBuilder.CreateSkillDataLists(
                    combinationCollection,
                    mCharaIdPossessionDictionary,
                    combinationTrainingStatusMasterGroupCache,
                    CombinationCollectionView.ActivatedLabelType.Total,
                    CombinationViewBase<CombinationCollectionPracticeSkillView, CombinationManager.CollectionMultipleSkillData>.NotActiveLabelType.Next);

                // マスターのコンビネーションコレクションID、表示フラグ、キャラクター詳細データリスト、スキルデータリストを使用してスクロールデータを作成
                CombinationCollectionScrollData data = new CombinationCollectionScrollData(
                    mCombinationId: combinationCollection.MCombinationId,
                    showBadge: skillDataLists.CanActiveSkillDataList.Count > 0, 
                    showDetailButton: true, 
                    showCharaLevel: true, 
                    characterDetailDataList: detailDataList, 
                    mCharaIdPossessionDictionary: mCharaIdPossessionDictionary,
                    skillDataLists: skillDataLists,
                    onProgressCombinationCollection: id =>
                    {
                        UpdateSkillView(id);
                        InitializeAll(cachedLevelStageCallback);
                        cachedLevelStageCallback?.Invoke(id);
                    });

                scrollData.Add(data);
            }
            
            // 解放可能なスキルがあるもの上に持ってくる
            scrollData = scrollData
                // 解放可能なスキルがあるかどうかで降順ソート（あるのが上になり、ないのが下になる）
                .OrderByDescending(data => data.SkillDataLists.CanActiveSkillDataList.Count > 0)
                // 同じ条件（解放可能の有り無しが同じ）の中では元の順序で昇順ソート
                .ThenBy(data => scrollData.IndexOf(data))
                // ソート結果をリストに変換
                .ToList();
            
            return scrollData;
        }

        protected override SortFilterBaseModal<CombinationSkillSortData, CombinationSkillFilterData>.Data CreateSortFilterModalData()
        {
            List<long> displayCharacterIdList = GetDisplayCharacterIdList(collectionList);

            return new BaseCombinationSkillSortFilterModal<CombinationManager.CombinationCollection>.Data(
                SortFilterSheetType.Filter,
                SortFilterType,
                displayCharacterIdList);
        }
    }
}