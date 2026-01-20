using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using CruFramework;

namespace Pjfb.Combination
{
    /// <summary> ソートインターフェース </summary>
    public interface ICombinationSortable
    {
        /// <summary> ソート番号 </summary>
        long SortNumber { get; }
        /// <summary> 発動スキル数 </summary>
        int SkillCount { get; }
        /// <summary> スキルレアリティ（マッチスキルでのみ使用） </summary>
        long SkillRarity { get; }
        /// <summary> スキルマスタのID </summary>
        long Id { get; }
        /// <summary> スキルコネクトのキャラアイコンのIDを獲得する共通メソッド </summary>
        IReadOnlyList<long> GetCharacterIconIds();
    }
    /// <summary> フィルターインターフェース </summary>
    public interface ICombinationFilterable
    {
        /// <summary> 発動に必要なキャラIDのHashSet </summary>
        HashSet<long> CharaIdHashSet { get; }
    }
    
    public static class CombinationManager
    {
        //機能番号：コンビネーション機能 510001
        //機能番号：スキルコネクトシステム、トレーニング 510002
        //機能番号：マッチスキル 510003
        //各スキルにロックがかかっているが基本同じトリガーで行うとのことなので1つのロックだけ参照する
        public const int CombinationLockId = 510001;
        
        public static bool IsUnLockCombination()
        { 
            return UserDataManager.Instance.IsUnlockSystem(CombinationLockId) || SystemUnlockDataManager.Instance.IsUnlockingSystem(CombinationLockId);
        }

        public static bool HasCombinationBadge => IsUnLockCombination() && (HasNewCombinationMatchBadge ||
                                                                            HasNewCombinationTrainingBadge ||
                                                                            CanActiveCombinationCollectionBadge);

        #region Combination Match
        private static List<CombinationMatch> combinationMatchSortedCache;
        private static DateTime combinationMatchMinUpdateTime = DateTime.MinValue;

        
        public class CombinationMatch : ICombinationSortable, ICombinationFilterable
        {
            public CombinationMatch(long mCharaComboAbilityId, List<CharaComboAbilityElementMasterObject> mCharaComboAbilityElementSortedList)
            {
                MCharaComboAbilityId = mCharaComboAbilityId;
                var mCharaComboAbility = MCharaComboAbility;

                abilityCount = mCharaComboAbility.mAbilityIdList.Length;
                MCharaComboAbilityElementSortedList = mCharaComboAbilityElementSortedList;
                
                mCharaIdHashSet = MCharaComboAbilityElementSortedList.Select(x => x.mCharaId).ToHashSet();
                SortNumber = mCharaComboAbility.sortNumber;
                
                // スキルレアリティ取得
                var skillRarities = GetSkillRarities(mCharaComboAbility.mAbilityIdList);
                if (skillRarities.Count > 0)
                {
                    highestSkillRarity = skillRarities.Max();
                }
                
#if UNITY_EDITOR
                if (mCharaComboAbility.abilityLevelList.Length != mCharaComboAbility.mAbilityIdList.Length)
                {
                    throw new Exception($"MCharaComboAbilityId : {MCharaComboAbilityId}, the number of abilityLevelList does not match the number of mAbilityIdList!");
                }
#endif
            }

            public bool IsMatched(HashSet<long> idHashSet)
            {
                return mCharaIdHashSet.IsSubsetOf(idHashSet);
            }
            
            public readonly long MCharaComboAbilityId;
            public readonly long SortNumber;
            public readonly List<CharaComboAbilityElementMasterObject> MCharaComboAbilityElementSortedList;
            private readonly HashSet<long> mCharaIdHashSet;
            private readonly int abilityCount;
            private readonly long highestSkillRarity;
            
            // インターフェース実装
            long ICombinationSortable.SortNumber => SortNumber;
            int ICombinationSortable.SkillCount => abilityCount;
            long ICombinationSortable.SkillRarity => highestSkillRarity;
            long ICombinationSortable.Id => MCharaComboAbilityId;
            /// <summary> マッチスキルを所持するキャラIDを取得</summary>
            IReadOnlyList<long> ICombinationSortable.GetCharacterIconIds()
            {
                List<long> resultList = new List<long>();
                foreach (CharaComboAbilityElementMasterObject element in MCharaComboAbilityElementSortedList)
                {
                    resultList.Add(element.mCharaId);
                }
                return resultList.AsReadOnly();
            }
            HashSet<long> ICombinationFilterable.CharaIdHashSet => mCharaIdHashSet;
            
            public CharaComboAbilityMasterObject MCharaComboAbility => MasterManager.Instance.charaComboAbilityMaster.FindData(MCharaComboAbilityId);

            public List<SkillData> GetAbilityList()
            {
                var mCharaComboAbility = MCharaComboAbility;
                var result = new List<SkillData>();
                for (int i = 0; i < abilityCount; i++)
                {
                    result.Add(new SkillData(mCharaComboAbility.mAbilityIdList[i], mCharaComboAbility.abilityLevelList[i]));
                }

                return result;
            }
        }

        public static int ActivatingCombinationMatchCount(List<long> idList) => GetActivatingCombinationMatchList(idList).Count; 
        
        public static List<CombinationMatch> GetAllCombinationMatchList()
        {
            DateTime now = AppTime.Now;
            if (combinationMatchMinUpdateTime.IsFuture(now))
            {
                return combinationMatchSortedCache;
            }
            
            combinationMatchMinUpdateTime = DateTime.MaxValue;
            Dictionary<long, CombinationMatch> dictionary = new Dictionary<long, CombinationMatch>();
            var elementSortedGroupCache = MasterManager
                .Instance.charaComboAbilityElementMaster.values
                .GroupBy(x => x.mCharaComboAbilityId)
                .ToDictionary(x => x.Key,
                    x => x.OrderByDescending(y => y.isTarget).ThenBy(y => y.mCharaId).ToList());

            foreach (var mCharaComboAbility in MasterManager.Instance.charaComboAbilityMaster.values)
            {
                long mCharaComboAbilityId = mCharaComboAbility.id;
                if (!elementSortedGroupCache.ContainsKey(mCharaComboAbilityId))
                {
                    Logger.LogError($"MCharaComboAbilityId : {mCharaComboAbilityId} not found!");
                    continue;    
                } 
                
                
                DateTime dateTime = mCharaComboAbility.startAt.TryConvertToDateTime(); 
                if (dateTime.IsFuture(now))
                {
                    combinationMatchMinUpdateTime = (dateTime < combinationMatchMinUpdateTime ? dateTime : combinationMatchMinUpdateTime);
                    continue;
                }
                
                if (dictionary.ContainsKey(mCharaComboAbilityId))
                {
                    Logger.LogError($"Duplicate id : {mCharaComboAbilityId}");
                }
                else
                {
                    dictionary.Add(mCharaComboAbilityId, new CombinationMatch(mCharaComboAbilityId, elementSortedGroupCache[mCharaComboAbilityId]));
                }
            }
            
            combinationMatchSortedCache = dictionary.Values.OrderBy(x => x.SortNumber).ToList(); 
            return combinationMatchSortedCache;
        }

        public static List<CombinationMatch> GetActivatingCombinationMatchList(List<long> mCharaIdList)
        {
            HashSet<long> idHashSet = mCharaIdList.ToHashSet();
#if UNITY_EDITOR
            if (idHashSet.Count != mCharaIdList.Count)
            {
                throw new Exception("Contains duplicate Id!");
            }            
#endif


            List<CombinationMatch> allCombinationMatchList = GetAllCombinationMatchList();
            return allCombinationMatchList.Where(combinationMatch => combinationMatch.IsMatched(idHashSet)).ToList();
        }

        public static bool HasNewCombinationMatchBadge => HasNewCombinationMatch(GetAllCombinationMatchList());

        private static List<CombinationTraining> combinationTrainingSortedCache;
        private static DateTime combinationTrainingMinUpdateTime = DateTime.MinValue;
        
        public static int ActivatingCombinationTrainingCount(Dictionary<long, long> characterTupleDictionary) =>
            GetActivatingCombinationTrainingList(characterTupleDictionary).Count; 

        
        #endregion
        #region Combination Training
        public class CombinationTraining : ICombinationSortable, ICombinationFilterable
        {
            public CombinationTraining(long mCharaTrainingComboBuffId, List<long> mCharaSortedIdList, List<long> mCharaTrainingComboBuffStatusIdList)
            {
                MCharaTrainingComboBuffId = mCharaTrainingComboBuffId;
                // 発動に必要なキャラIDリスト
                MCharaSortedIdList = mCharaSortedIdList;
                mCharaIdHashSet = MCharaSortedIdList.ToHashSet();
                // 開放段階リスト
                MCharaTrainingComboBuffStatusIdList = mCharaTrainingComboBuffStatusIdList;
                
                var mCharaTrainingComboBuff = MCharaTrainingComboBuff;
                SortNumber = mCharaTrainingComboBuff.sortNumber;
                StartAt = mCharaTrainingComboBuff.startAt.TryConvertToDateTime();

                // 最大開放段階の発動スキル数を取得
                if (MCharaTrainingComboBuffStatusIdList.Count > 0)
                {
                    long maxLevelComboBuffId = GetMaxLevelTrainingComboBuffId(MCharaTrainingComboBuffStatusIdList);
                    maxLevelSkillCount = PracticeSkillUtility.GetComboBuffPracticeSkill(maxLevelComboBuffId).Count;
                }
                
#if UNITY_EDITOR
                if (mCharaIdHashSet.Count != mCharaSortedIdList.Count)
                {
                    throw new Exception("Contains duplicate mChara Id!");
                }                          
#endif
            }

            public long MCharaTrainingComboBuffId;
            public List<long> MCharaSortedIdList;
            public List<long> MCharaTrainingComboBuffStatusIdList;
            public readonly long SortNumber;
            private readonly HashSet<long> mCharaIdHashSet;
            public readonly DateTime StartAt;
            // 最大発動スキル数
            private int maxLevelSkillCount;
            
            // インターフェース実装
            long ICombinationSortable.SortNumber => SortNumber;
            int ICombinationSortable.SkillCount => maxLevelSkillCount;
            long ICombinationSortable.Id => MCharaTrainingComboBuffId;
            // ここでは不要なため固定値を返す
            long ICombinationSortable.SkillRarity => -1;
            /// <summary> トレーニングスキルを所持するキャラIDを取得</summary>
            IReadOnlyList<long> ICombinationSortable.GetCharacterIconIds()
            {
                return MCharaSortedIdList.AsReadOnly();
            }
            HashSet<long> ICombinationFilterable.CharaIdHashSet => mCharaIdHashSet;

            private CharaTrainingComboBuffMasterObject MCharaTrainingComboBuff => MasterManager.Instance.charaTrainingComboBuffMaster.FindData(MCharaTrainingComboBuffId);
            
            public bool IsActivating(Dictionary<long, long> characterTupleDictionary)
            {
                var isMatched = false;
                foreach (var mCharaTrainingComboBuffStatusId in MCharaTrainingComboBuffStatusIdList)
                {
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if(mCharaTrainingComboBuffStatus == null) continue;
                    foreach (var mCharaId in mCharaIdHashSet)
                    {
                        if (!characterTupleDictionary.TryGetValue(mCharaId, out var level))
                        {
                            isMatched = false;
                            break;
                        }

                        if (level < mCharaTrainingComboBuffStatus.requireLevel)
                        {
                            isMatched = false;
                            break;
                        }
                        isMatched = true;
                    }
                    
                    if(isMatched) break;
                }

                return isMatched;
            }
            
            public List<long> GetActivatingMCharaTrainingComboBuffStatusIdList(Dictionary<long, long> characterTupleDictionary)
            {
                var result = new List<long>();
                foreach (var mCharaTrainingComboBuffStatusId in MCharaTrainingComboBuffStatusIdList)
                {
                    var isMatched = false;
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if(mCharaTrainingComboBuffStatus == null) continue;
                    foreach (var mCharaId in mCharaIdHashSet)
                    {
                        if (!characterTupleDictionary.TryGetValue(mCharaId, out var level))
                        {
                            isMatched = false;
                            break;
                        }
                        if (level < mCharaTrainingComboBuffStatus.requireLevel)
                        {
                            isMatched = false;
                            break;
                        }
                        isMatched = true;
                    }
                    if(!isMatched) continue;
                    result.Add(mCharaTrainingComboBuffStatusId);
                }

                return result;
            }
            
            public long GetMinRequireLevel()
            {
                long minRequireLevel = 0;
                foreach (var mCharaTrainingComboBuffStatusId in MCharaTrainingComboBuffStatusIdList)
                {
                    var mCharaTrainingComboBuffStatus =
                        MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(
                            mCharaTrainingComboBuffStatusId);
                    if(mCharaTrainingComboBuffStatus == null || (minRequireLevel != 0 && minRequireLevel < mCharaTrainingComboBuffStatus.requireLevel)) continue;
                    minRequireLevel = mCharaTrainingComboBuffStatus.requireLevel;
                }

                return minRequireLevel;
            }
        }

        public static List<CombinationTraining> GetAllCombinationTrainingList()
        {
            DateTime now = AppTime.Now;
            if (combinationTrainingMinUpdateTime.IsFuture(now))
            {
                return combinationTrainingSortedCache;
            }

            combinationTrainingMinUpdateTime = DateTime.MaxValue;
            Dictionary<long, CombinationTraining> dictionary = new Dictionary<long, CombinationTraining>();
            
            
            var elementGroupCache = MasterManager.Instance
                .charaTrainingComboBuffElementMaster.values
                .GroupBy(x => x.mCharaTrainingComboBuffId)
                .ToDictionary(x => x.Key, 
                    x => x.Select(y => y.mCharaId).OrderBy(y => y).ToList());
            
            var statusGroupCache = MasterManager.Instance
                .charaTrainingComboBuffStatusMaster.values
                .GroupBy(x => x.mCharaTrainingComboBuffId)
                .ToDictionary(x => x.Key, 
                    x => x.Select(y => y.id).OrderBy(y => y).ToList());

            
            foreach (var mCharaTrainingComboBuff in MasterManager.Instance.charaTrainingComboBuffMaster.values)
            {
                long mBuffId = mCharaTrainingComboBuff.id;

                if (!elementGroupCache.ContainsKey(mBuffId) || !statusGroupCache.ContainsKey(mBuffId)) continue;
                List<long> mCharaSortedIdList = elementGroupCache[mBuffId];
                List<long> statusIdList = statusGroupCache[mBuffId]; 

                DateTime dateTime = mCharaTrainingComboBuff.startAt.TryConvertToDateTime();
                if (dateTime.IsFuture(now))
                {
                    combinationTrainingMinUpdateTime = (dateTime < combinationTrainingMinUpdateTime ? dateTime : combinationTrainingMinUpdateTime);
                    continue;
                }
                
                if (dictionary.ContainsKey(mBuffId))
                {
                    Logger.LogError($"Duplicate id : {mBuffId}");
                }
                else
                {
                    dictionary[mBuffId] = new CombinationTraining(mBuffId, mCharaSortedIdList, statusIdList);
                }
            }
            
            combinationTrainingSortedCache = dictionary.Values.OrderBy(x => x.SortNumber).ToList();
            return combinationTrainingSortedCache;
        }


        public static List<CombinationTraining> GetActivatingCombinationTrainingList(Dictionary<long, long> characterTupleDictionary)
        {
            List<CombinationTraining> allCombinationTrainingList = GetAllCombinationTrainingList();
            return allCombinationTrainingList.Where(combinationTraining => combinationTraining.IsActivating(characterTupleDictionary)).ToList();
        }
        
        public static List<CombinationTraining> GetActivatingCombinationTrainingListBefore(Dictionary<long, long> characterTupleDictionary, DateTime endDate)
        {
            List<CombinationTraining> allCombinationTrainingList = GetAllCombinationTrainingList();
            return allCombinationTrainingList.Where(combinationTraining => combinationTraining.IsActivating(characterTupleDictionary) && combinationTraining.StartAt.IsPast(endDate)).ToList();
        }

        public static bool HasNewCombinationTrainingBadge => HasNewCombinationTraining(GetAllCombinationTrainingList());

        private static List<CombinationCollection> combinationCollectionSortedCache;
        private static DateTime combinationCollectionMinUpdateTime = DateTime.MinValue;

        #endregion
        #region Combination Collection
        public class CombinationCollection : ICombinationSortable, ICombinationFilterable
        {
            public const long MinProgressLevel = 1;
            
            public CombinationCollection(long mCombinationId, List<long> mCharaIdList, List<long> mCombinationProgressIdList, List<long> mCombinationTrainingStatusIdList)
            {
                MCombinationId = mCombinationId;
                MCharaIdList = mCharaIdList;
                mCharaIdHashSet = MCharaIdList.ToHashSet();
                MCombinationProgressIdList = mCombinationProgressIdList;
                MCombinationTrainingStatusIdList = mCombinationTrainingStatusIdList;
                var mCombination = MCombination;
                StartAt = mCombination.startAt.TryConvertToDateTime();
                SortNumber = MCombination.sortNumber;
                
                // 最大開放段階の発動スキル数を取得
                maxLevelSkillCount = GetMaxLevelCollectionComboBuffSkillCount(MCombinationTrainingStatusIdList);
            }

            public readonly long MCombinationId;
            public readonly IReadOnlyList<long> MCharaIdList;
            public readonly IReadOnlyList<long> MCombinationProgressIdList;
            public readonly IReadOnlyList<long> MCombinationTrainingStatusIdList;
            public readonly DateTime StartAt;
            // MCombinationProgressにおいてキャラの必要データは下記データになっており所持数は必要ないためレベルだけ見るようにする
            // conditionType 条件種別(1 => 所持数, 2 => 所持数・潜在レベル到達)
            // conditionValue 条件値1（type1・2のとき対象キャラの必要所持数）
            // conditionValueSub 条件値2 （type2のとき、対象キャラたちが達している必要がある潜在レベル。それ以外の時は0とか入れる）
            
            public readonly long SortNumber;
            private readonly int maxLevelSkillCount;
            private readonly HashSet<long> mCharaIdHashSet;
            
            // インターフェース実装
            long ICombinationSortable.SortNumber => SortNumber;
            int ICombinationSortable.SkillCount => maxLevelSkillCount;
            long ICombinationSortable.Id => MCombinationId;
            // ここでは不要なため固定値を返す
            long ICombinationSortable.SkillRarity => -1;

            /// <summary> コレクションスキルを所持するキャラIDを取得</summary>
            IReadOnlyList<long> ICombinationSortable.GetCharacterIconIds()
            {
                return MCharaIdList.ToList();
            }

            HashSet<long> ICombinationFilterable.CharaIdHashSet => mCharaIdHashSet;
            
            public CombinationMasterObject MCombination => MasterManager.Instance.combinationMaster.FindData(MCombinationId);

            public bool IsActivatedProgressLevel(long progressLevel)
            {
                var combinationOpenedMinimum = CombinationCollectionOpenedListCacheDictionary.GetValueOrDefault(MCombinationId, null);
                if (combinationOpenedMinimum == null) return false;
                return progressLevel <= combinationOpenedMinimum.progressLevel;
            }
            
            public bool IsActivatedProgressLevel(Dictionary<long, CombinationOpenedMinimum> openedMinimums, long progressLevel)
            {
                var combinationOpenedMinimum = openedMinimums.GetValueOrDefault(MCombinationId, null);
                if (combinationOpenedMinimum == null) return false;
                return progressLevel <= combinationOpenedMinimum.progressLevel;
            }

            public long GetActivatedMaxProgressLevel()
            {
                var combinationOpenedMinimum = CombinationCollectionOpenedListCacheDictionary.GetValueOrDefault(MCombinationId, null);
                if (combinationOpenedMinimum == null) return 0;
                return combinationOpenedMinimum.progressLevel;
            }
            
            public long GetActivatedMaxProgressLevel(Dictionary<long, CombinationOpenedMinimum> openedMinimums)
            {
                var combinationOpenedMinimum = openedMinimums.GetValueOrDefault(MCombinationId, null);
                if (combinationOpenedMinimum == null) return 0;
                return combinationOpenedMinimum.progressLevel;
            }
        }

        public static int ActivatingCombinationCollectionCount() => GetActivatedCombinationCollectionList().Count;
        
        public static List<CombinationCollection> GetAllCombinationCollectionList()
        {
            DateTime now = AppTime.Now;
            if (combinationCollectionMinUpdateTime.IsFuture(now))
            {
                return combinationCollectionSortedCache;
            }

            combinationCollectionMinUpdateTime = DateTime.MaxValue;
            Dictionary<long, CombinationCollection> dictionary = new Dictionary<long, CombinationCollection>();
            
            Dictionary<long, List<long>> combinationCollectionSortedMCharaIdGroupCache = MasterManager.Instance.combinationCharaMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, 
                    x => x.Select(y => y.mCharaId).OrderBy(y => y).ToList());

            Dictionary<long, List<long>> mCombinationCollectionProgressIdGroupCache = MasterManager.Instance.combinationProgressMaster.values
                .GroupBy(x => x.mCombinationProgressGroupId)
                .ToDictionary(x => x.Key, 
                    x => x.OrderBy(x => x.level).Select(y => y.id).ToList());
            
            Dictionary<long, List<long>> mCombinationCollectionTrainingStatusIdGroupCache = MasterManager.Instance.combinationTrainingStatusMaster.values
                .GroupBy(x => x.mCombinationId)
                .ToDictionary(x => x.Key, 
                    x => x.Select(y => y.id).ToList());
            
            foreach (var mCombination in  MasterManager.Instance.combinationMaster.values)
            {
                long mCombinationId = mCombination.id;

                if (!combinationCollectionSortedMCharaIdGroupCache.ContainsKey(mCombinationId)) continue;
                
                DateTime dateTime = mCombination.startAt.TryConvertToDateTime();
                if (dateTime.IsFuture(now))
                {
                    combinationCollectionMinUpdateTime = (dateTime < combinationCollectionMinUpdateTime ? dateTime : combinationCollectionMinUpdateTime);
                    continue;
                }

                List<long> mCharaIdList;
                if (combinationCollectionSortedMCharaIdGroupCache.ContainsKey(mCombinationId))
                {
                    mCharaIdList = combinationCollectionSortedMCharaIdGroupCache[mCombinationId];
                }
                else
                {
                    Logger.LogError(
                        $"combinationCollectionSortedMCharaIdGroupCacheにmCombinationIdのデータがありません　mCombinationId : {mCombinationId}");
                    continue;
                }
                List<long> mCombinationProgressIdList;
                if (mCombinationCollectionProgressIdGroupCache.ContainsKey(mCombination.mCombinationProgressGroupId))
                {
                    mCombinationProgressIdList = mCombinationCollectionProgressIdGroupCache[mCombination.mCombinationProgressGroupId];
                }
                else
                {
                    Logger.LogError(
                        $"mCombinationCollectionProgressGroupCacheにmCombinationProgressGroupIdのデータがありません　mCombinationProgressGroupId : {mCombination.mCombinationProgressGroupId}");
                    continue;
                }
                List<long> mCombinationTrainingStatusIdList;
                if (mCombinationCollectionTrainingStatusIdGroupCache.ContainsKey(mCombinationId))
                {
                    mCombinationTrainingStatusIdList = mCombinationCollectionTrainingStatusIdGroupCache[mCombinationId];
                }
                else
                {
                    Logger.LogError($"mCombinationCollectionTrainingStatusCacheにmCombinationIdのデータがありません　mCombinationId : {mCombinationId}");
                    continue;
                }
                if (dictionary.ContainsKey(mCombinationId))
                {
                    Logger.LogError($"Duplicate id : {mCombination.id}");
                }
                else
                {
                    dictionary.Add(mCombinationId,
                        new CombinationCollection(mCombinationId, mCharaIdList, mCombinationProgressIdList, mCombinationTrainingStatusIdList));
                }
            }
            combinationCollectionSortedCache = dictionary.Values.OrderBy(x => x.SortNumber).ToList();
            return combinationCollectionSortedCache;
        }
        
        public static List<CombinationCollection> GetActivatedCombinationCollectionList()
        {
            List<CombinationCollection> allCombinationCollectionList = GetAllCombinationCollectionList();
            List<CombinationCollection> result = new List<CombinationCollection>();
            if (CombinationCollectionOpenedIdHashSet is null) return result;
            result.AddRange(allCombinationCollectionList.Where(combinationCollection => CombinationCollectionOpenedIdHashSet.Contains(combinationCollection.MCombinationId)));

            return result;
        }
        
        public static List<CombinationCollection> GetActivatedCombinationCollectionListBefore(DateTime endDate)
        {
            List<CombinationCollection> allCombinationCollectionList = GetAllCombinationCollectionList();
            List<CombinationCollection> result = new List<CombinationCollection>();
            if (CombinationCollectionOpenedIdHashSet is null) return result;
            foreach (var combinationCollection in allCombinationCollectionList)
            {
                if (!CombinationCollectionOpenedIdHashSet.Contains(combinationCollection.MCombinationId))continue;
                if (combinationCollection.StartAt.IsFuture(endDate)) continue;
                result.Add(combinationCollection);
            }
            
            return result;
        }

        public static List<CombinationCollection> GetActivatedCombinationCollectionList(List<long> mCombinationCollectionIdList)
        {
            List<CombinationCollection> allCombinationCollectionList = GetAllCombinationCollectionList();
            List<CombinationCollection> result = new List<CombinationCollection>();
            foreach (var combinationCollection in allCombinationCollectionList)
            {
                if (mCombinationCollectionIdList.Contains(combinationCollection.MCombinationId))
                    result.Add(combinationCollection);
            }
            
            return result;
        }
        
        
        
        public static List<CombinationCollection> GetCanActiveCombinationCollectionList()
        {
            Dictionary<long, UserDataChara> mCharaIdPossessionDictionary = UserDataManager.Instance.chara.data.Values.ToDictionary(x => x.charaId, x => x);
            List<CombinationCollection> allCombinationCollectionList = GetAllCombinationCollectionList();
            List<CombinationCollection> result = new List<CombinationCollection>();
            foreach (var combinationCollection in allCombinationCollectionList)
            {
                foreach (var mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
                {
                    var mCombinationProgress = MasterManager.Instance.combinationProgressMaster.FindData(mCombinationProgressId);
                    if(mCombinationProgress == null) continue;
                    //progress側の定義としては「段階。0スタート。「そのレベルから次のレベルに強化するときの条件」の設定を入れる。」
                    //training_status側の定義としては「「現在の」達成度レベル。レベルは初期状態では0だが、レベル0の場合はこのデータが存在しても効果の発動は行わない。」
                    //上記より解放しているかを取得する際はprogressのlevelに+1したものを渡す
                    var nextLevel = mCombinationProgress.level + 1;
                    if(combinationCollection.IsActivatedProgressLevel(nextLevel)) continue;
                    var canActive = true;
                    foreach (var mCharaId in combinationCollection.MCharaIdList)
                    {
                        if (mCharaIdPossessionDictionary.ContainsKey(mCharaId) && mCharaIdPossessionDictionary[mCharaId].level >= mCombinationProgress.conditionValueSub) continue;
                        canActive = false;
                        break;
                    }
                    if (!canActive) continue;
                    result.Add(combinationCollection);
                    break;
                }
            }

            return result;
        }

        public static bool CanActiveCombinationCollectionBadge => GetCanActiveCombinationCollectionList().Count > 0;
        #endregion


        /// <summary>
        /// Dictionary&lt;mStatusAdditionId&gt;, &gt;Dictionary&lt;level&gt;, maxGrowthLevel&gt;&gt;
        /// </summary>
        public static Dictionary<long, Dictionary<long, long>> LiberationMaxLevelGrowthCache
        {
            get
            {
                if(liberationMaxLevelGrowthCache is null || liberationMaxLevelGrowthCache.Count == 0)
                {
                    liberationMaxLevelGrowthCache = MasterManager.Instance.statusAdditionLevelMaster?.values
                        .GroupBy(x => x.mStatusAdditionId)
                        .ToDictionary(x => x.Key, 
                            x => x.ToDictionary(y => y.level, y => y.maxLevelGrowth));
                }

                return liberationMaxLevelGrowthCache;
            }
        }

        private static Dictionary<long, Dictionary<long, long>> liberationMaxLevelGrowthCache;
        
        
        public static void ClearCache()
        {
            // マッチスキル
            combinationMatchSortedCache?.Clear();
            combinationMatchMinUpdateTime = DateTime.MinValue;

            
            // トレーニングスキル
            combinationTrainingSortedCache?.Clear();
            combinationTrainingMinUpdateTime = DateTime.MinValue;
            
            // コレクションスキル
            combinationCollectionMinUpdateTime = DateTime.MinValue;
            combinationCollectionSortedCache?.Clear();
            
            LiberationMaxLevelGrowthCache?.Clear();
        }

        
        /// <summary> トレーニングスキル：最大開放段階のIDを取得 </summary>
        public static long GetMaxLevelTrainingComboBuffId(List<long> mCharaComboBuffStatusIdList)
        {
            // 開放に必要なレベル
            long maxLevel = 0;
            // 最大開放段階のID
            long maxLevelId = 0;

            // 開放に必要なキャラレベルを元に、最大開放段階のIDを取得
            foreach (var id in mCharaComboBuffStatusIdList)
            {
                // マスターデータ
                CharaTrainingComboBuffStatusMasterObject statusMaster = MasterManager.Instance.charaTrainingComboBuffStatusMaster.FindData(id);
                
                if (statusMaster != null && statusMaster.requireLevel > maxLevel)
                {
                    maxLevel = statusMaster.requireLevel;
                    maxLevelId = statusMaster.id;
                }
            }

            return maxLevelId;
        }
        
        /// <summary> コレクションスキル：最大開放段階のスキル数を取得　</summary>
        /// <param name="mCombinationTrainingStatusIdList">CombinationTrainingStatusのIDリスト</param>
        /// <returns>最大開放段階のスキル数</returns>
        public static int GetMaxLevelCollectionComboBuffSkillCount(IReadOnlyList<long> mCombinationTrainingStatusIdList)
        {
            long maxProgressLevel = 0;
            CombinationTrainingStatusMasterObject maxLevelTrainingStatus = null;
            
            foreach (var trainingStatusId in mCombinationTrainingStatusIdList)
            {
                CombinationTrainingStatusMasterObject trainingStatus = MasterManager.Instance.combinationTrainingStatusMaster.FindData(trainingStatusId);
                
                if (trainingStatus != null && trainingStatus.progressLevel > maxProgressLevel)
                {
                    maxProgressLevel = trainingStatus.progressLevel;
                    maxLevelTrainingStatus = trainingStatus;
                }
            }
            
            // スキル数を返す
            return maxLevelTrainingStatus?.typeList.Length ?? 0;
        }

        /// <summary> スキルレアリティリストを取得 </summary>
        private static List<long> GetSkillRarities(long[] skillIdList)
        {
            List<long> skillRarityList = new List<long>();

            foreach (long skillId in skillIdList)
            {
                AbilityMasterObject abilityMaster = MasterManager.Instance.abilityMaster.FindData(skillId);
                skillRarityList.Add(abilityMaster.rarity);
            }

            return skillRarityList;
        }

        #region Api

        public static CombinationOpenedMinimum[] CombinationCollectionOpenedListCache;
        public static Dictionary<long, CombinationOpenedMinimum> CombinationCollectionOpenedListCacheDictionary = null;    // Dictionary<mCombinationId, CombinationOpenedMinimum>
        public static CombinationStatusTrainingBase CombinationCollectionTrainingBuffCache = null;
        private static HashSet<long> CombinationCollectionOpenedIdHashSet = null;
        
        
        
        
        public static async UniTask<CombinationOpenedMinimum[]> GetCombinationCollectionListOpenedAPI()
        {
            if (CombinationCollectionOpenedListCache != null)
            {
                return CombinationCollectionOpenedListCache;
            }
            var request = new CombinationGetListOpenedAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            CombinationCollectionOpenedListCache = response.openedList;
            CombinationCollectionOpenedListCacheDictionary = CombinationCollectionOpenedListCache.ToDictionary(x => x.mCombinationId, x => x);
            CombinationCollectionOpenedIdHashSet = CombinationCollectionOpenedListCache.Select(data => data.mCombinationId).ToHashSet();
            return CombinationCollectionOpenedListCache;
        }
        
        public static async UniTask<CombinationStatusTrainingBase> GetCombinationCollectionTrainingBuffAPI()
        {
            if (CombinationCollectionTrainingBuffCache != null)
            {
                return CombinationCollectionTrainingBuffCache;
            }
            var request = new CombinationGetTrainingBuffAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            CombinationCollectionTrainingBuffCache = response.statusTrainingBase;
            return CombinationCollectionTrainingBuffCache;
        }
        
        public static async UniTask CombinationCollectionProgressAPI(long mCombinationId)
        {
            CombinationProgressAPIRequest request = new CombinationProgressAPIRequest();
            CombinationProgressAPIPost post = new CombinationProgressAPIPost();
            post.mCombinationId = mCombinationId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            // 解放で追加された物しか入っていないため現在のキャッシュと比較してないものを追加する
            var tempList = CombinationCollectionOpenedListCache?.ToList() ?? new List<CombinationOpenedMinimum>();
            foreach (var openedCombinationCollection in response.openedList)
            {
                if (CombinationCollectionOpenedListCache != null && CombinationCollectionOpenedListCache.Any(data =>
                        data.mCombinationId == openedCombinationCollection.mCombinationId))
                {
                    foreach (var combinationOpenedMinimum in tempList)
                    {
                        if(combinationOpenedMinimum.mCombinationId != openedCombinationCollection.mCombinationId) continue;
                        combinationOpenedMinimum.progressLevel = openedCombinationCollection.progressLevel;
                        break;
                    }
                }
                else
                {
                    tempList.Add(openedCombinationCollection);
                }
            }
            CombinationCollectionOpenedListCache = tempList.ToArray();
            CombinationCollectionOpenedListCacheDictionary = CombinationCollectionOpenedListCache.ToDictionary(x => x.mCombinationId, x => x);
            CombinationCollectionTrainingBuffCache = response.statusTrainingBase;
            CombinationCollectionOpenedIdHashSet = CombinationCollectionOpenedListCache.Select(data => data.mCombinationId).ToHashSet();
        }
        
        public static async UniTask CombinationCollectionProgressBulkAPI(List<long> mCombinationIdList)
        {
            CombinationProgressBulkAPIRequest request = new CombinationProgressBulkAPIRequest();
            CombinationProgressBulkAPIPost post = new CombinationProgressBulkAPIPost();
            post.mCombinationIdList = mCombinationIdList.ToArray();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            // 解放で追加された物しか入っていないため現在のキャッシュと比較して存在しないもののみを追加する
            var tempList = CombinationCollectionOpenedListCache?.ToList() ?? new List<CombinationOpenedMinimum>();
            foreach (var openedCombinationCollection in response.openedList)
            {
                if (CombinationCollectionOpenedListCache != null && CombinationCollectionOpenedListCache.Any(data =>
                        data.mCombinationId == openedCombinationCollection.mCombinationId))
                {
                    foreach (var combinationOpenedMinimum in tempList)
                    {
                        if(combinationOpenedMinimum.mCombinationId != openedCombinationCollection.mCombinationId) continue;
                        combinationOpenedMinimum.progressLevel = openedCombinationCollection.progressLevel;
                        break;
                    }
                }
                else
                {
                    tempList.Add(openedCombinationCollection);
                }
            }
            CombinationCollectionOpenedListCache = tempList.ToArray();
            CombinationCollectionOpenedListCacheDictionary = CombinationCollectionOpenedListCache.ToDictionary(x => x.mCombinationId, x => x);
            CombinationCollectionTrainingBuffCache = response.statusTrainingBase;
            CombinationCollectionOpenedIdHashSet = CombinationCollectionOpenedListCache.Select(data => data.mCombinationId).ToHashSet();
        }

        public static async UniTask CombinationCollectionProgress(long mCombinationId,long progressId, Action<long> onProgressCombinationCollection)
        {
            await CombinationCollectionProgressAPI(mCombinationId);
            var progressDataList = new List<CollectionProgressData>();
            progressDataList.Add(new CollectionProgressData(mCombinationId, new List<long> { progressId }));
            CombinationCollectionSkillUnlockedModal.Open(
                new CombinationCollectionSkillUnlockedModal.Data(progressDataList, () => {onProgressCombinationCollection?.Invoke(mCombinationId);}));
        }
        
        public static void ClearApiCache()
        {
            CombinationCollectionOpenedListCache = null;
            CombinationCollectionTrainingBuffCache = null;
            CombinationCollectionOpenedIdHashSet = null;
        }

        #endregion

        #region SaveData

        
        private static HashSet<long> confirmedCombinationMatchIdHashSet;
        private static HashSet<long> confirmedCombinationTrainingIdHashSet;

        public static void SaveConfirmedCombinationMatchId()
        {
            if (confirmedCombinationMatchIdHashSet == null) GetConfirmedCombinationMatchId();
            var allCombinationMatchList = GetAllCombinationMatchList();

            HashSet<long> newIdList = confirmedCombinationMatchIdHashSet!.ToHashSet();
            foreach (var combinationMatch in allCombinationMatchList)
            {
                newIdList.Add(combinationMatch.MCharaComboAbilityId);
            }

            if (confirmedCombinationMatchIdHashSet.IsSupersetOf(newIdList))
            {
                return;
            }

            confirmedCombinationMatchIdHashSet = newIdList;
            var saveString = string.Join(",", confirmedCombinationMatchIdHashSet);   
            
            LocalSaveManager.saveData.combinationMatchIdString = saveString;
            LocalSaveManager.Instance.SaveData();
        }
        
        public static void SaveConfirmedCombinationTrainingId()
        {
            if (confirmedCombinationTrainingIdHashSet == null) GetConfirmedCombinationTrainingId();
            var allCombinationTrainingList = GetAllCombinationTrainingList();
            
            HashSet<long> newIdList = confirmedCombinationTrainingIdHashSet!.ToHashSet();
            foreach (var combinationTraining in allCombinationTrainingList)
            {
                newIdList.Add(combinationTraining.MCharaTrainingComboBuffId);
            }

            if (confirmedCombinationTrainingIdHashSet.IsSupersetOf(newIdList))
            {
                return;
            }
            

            if (confirmedCombinationTrainingIdHashSet.IsSupersetOf(newIdList)) return;
            confirmedCombinationTrainingIdHashSet = newIdList;
            var saveString = string.Join(",", confirmedCombinationTrainingIdHashSet);   
           
            LocalSaveManager.saveData.combinationTrainingIdString = saveString;
            LocalSaveManager.Instance.SaveData();
        }
        
        public static bool HasNewCombinationMatch(List<CombinationMatch> combinationMatchList)
        {
            if (confirmedCombinationMatchIdHashSet == null) GetConfirmedCombinationMatchId();
            return combinationMatchList.Any(combinationMatch => !confirmedCombinationMatchIdHashSet!.Contains(combinationMatch.MCharaComboAbilityId));
        }
        
        public static bool HasNewCombinationTraining(List<CombinationTraining> combinationTrainingList)
        {
            if (confirmedCombinationTrainingIdHashSet == null) GetConfirmedCombinationTrainingId();
            return combinationTrainingList.Any(combinationTraining => !confirmedCombinationTrainingIdHashSet!.Contains(combinationTraining.MCharaTrainingComboBuffId));
        }
        
        private static void GetConfirmedCombinationMatchId()
        {
            if (confirmedCombinationMatchIdHashSet == null)
            {
                confirmedCombinationMatchIdHashSet = new HashSet<long>();
                var ids = LocalSaveManager.saveData.combinationMatchIdString;
                if (!string.IsNullOrEmpty(ids))
                {
                    confirmedCombinationMatchIdHashSet = ids.Split(",").Select(long.Parse).ToHashSet();
                }
            }
        }
        
        private static void GetConfirmedCombinationTrainingId()
        {
            if (confirmedCombinationTrainingIdHashSet == null)
            {
                confirmedCombinationTrainingIdHashSet = new HashSet<long>();
                var ids = LocalSaveManager.saveData.combinationTrainingIdString;
                if (!string.IsNullOrEmpty(ids))
                {
                    confirmedCombinationTrainingIdHashSet = ids.Split(",").Select(long.Parse).ToHashSet();
                }
            }
        }

        public static void ClearSavedDataCache()
        {
            confirmedCombinationMatchIdHashSet = null;
            confirmedCombinationTrainingIdHashSet = null;
        }

        #endregion

        #region SkillData
        
        public class TrainingMultipleSkillData
        {
            public List<PracticeSkillInfo> PracticeSkillDataList;
            public bool ShowSkillHighlight;
            public string LockString;

            public TrainingMultipleSkillData(List<PracticeSkillInfo> practiceSkillDataList, bool showSkillHighlight, string lockString = null)
            {
                PracticeSkillDataList = practiceSkillDataList;
                ShowSkillHighlight = showSkillHighlight;
                LockString = lockString ?? string.Empty;
            }
        }

        public class CollectionMultipleSkillData
        {
            public long ProgressId;
            public List<PracticeSkillInfo> PracticeSkillDataList;
            public bool ShowActiveButton;
            public bool ShowSkillHighlight;
            public string LockString;

            public CollectionMultipleSkillData(long progressId, List<PracticeSkillInfo> practiceSkillDataList, bool showActiveButton, bool showSkillHighlight, string lockString = null)
            {
                ProgressId = progressId;
                PracticeSkillDataList = practiceSkillDataList;
                ShowActiveButton = showActiveButton;
                ShowSkillHighlight = showSkillHighlight;
                LockString = lockString ?? string.Empty;
            }
        }

        public static string GetLockString(bool isFirstLevel, long requireLevel)
        {
            // 解放不可能のデータを追加
            var sb = new StringBuilder();
            var lockString = isFirstLevel
                ? sb.AppendFormat(StringValueAssetLoader.Instance["combination.cell.skill_lock_first"], requireLevel)
                : sb.AppendFormat(StringValueAssetLoader.Instance["combination.cell.skill_lock"], requireLevel);
            return lockString.ToString();
        }

        #endregion

        #region ProgressData

        public class CollectionProgressData
        {
            public long MCombinationId;
            public List<long> ProgressIdList;

            public CollectionProgressData(long mCombinationId, List<long> progressIdList)
            {
                MCombinationId = mCombinationId;
                ProgressIdList = progressIdList;
            }
        }
        
        public static List<CollectionProgressData> GetCanActiveProgressDataListByCharaId(long selectedCharaId)
        {
            Dictionary<long, UserDataChara> mCharaIdPossessionDictionary = UserDataManager.Instance.chara.data.Values.ToDictionary(x => x.charaId, x => x);
            List<CombinationCollection> allCombinationCollectionList = GetAllCombinationCollectionList();
            List<CollectionProgressData> result = new List<CollectionProgressData>();
            foreach (var combinationCollection in allCombinationCollectionList)
            {
                // 該当のキャラIdが対象になっていない場合はcontinue
                if(!combinationCollection.MCharaIdList.Contains(selectedCharaId)) continue;
                var progressData = new CollectionProgressData(combinationCollection.MCombinationId, new List<long>());
                foreach (var mCombinationProgressId in combinationCollection.MCombinationProgressIdList)
                {
                    var mCombinationProgress = MasterManager.Instance.combinationProgressMaster.FindData(mCombinationProgressId);
                    if (mCombinationProgress == null) continue;
                    // 解放済みの場合はcontinue
                    //progress側の定義としては「段階。0スタート。「そのレベルから次のレベルに強化するときの条件」の設定を入れる。」
                    //training_status側の定義としては「「現在の」達成度レベル。レベルは初期状態では0だが、レベル0の場合はこのデータが存在しても効果の発動は行わない。」
                    //上記より解放しているかを取得する際はprogressのlevelに+1したものを渡す
                    var nextLevel = mCombinationProgress.level + 1;
                    if (combinationCollection.IsActivatedProgressLevel(nextLevel)) continue;
                    var canActive = true;
                    foreach (var mCharaId in combinationCollection.MCharaIdList)
                    {
                        if (mCharaIdPossessionDictionary.ContainsKey(mCharaId) && mCharaIdPossessionDictionary[mCharaId].level >= mCombinationProgress.conditionValueSub) continue;
                        canActive = false;
                        break;
                    }
                    if (!canActive) continue;
                    progressData.ProgressIdList.Add(mCombinationProgress.id);
                }
                if(progressData.ProgressIdList.Count <= 0) continue;
                result.Add(progressData);
            }

            return result;
        }

        #endregion
    }
}
