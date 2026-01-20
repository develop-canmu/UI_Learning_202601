using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeStage.AntiCheat.Storage;
using CruFramework.Utils;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Story;
using Pjfb.Training;
using Pjfb.UserData;
using Pjfb.Utility;
using UnityEngine;

namespace Pjfb.Rivalry
{
    public enum HuntPrizeRouteType
    {
        Pass = 111, // サブスクパスでブースト
        Chara = 121, // 選手ブースト
    }

    public enum HuntPlayCountType
    {
        Challenge = 0, // 挑戦時に消費
        Win = 1, // 勝利時に消費
    }

    /// <summary> BattleV2PlayerからのPlayerType判別用Enum(なんか渡される場所によって番号変わるらしいので..) </summary>
    public enum HuntPlayerType
    {
        Player = 1,
        Npc = 2
    }
    
    public class RivalryManager : Singleton<RivalryManager>
    {
        #region PublicProperties
        public List<HuntResultStatus> huntResultList = new List<HuntResultStatus>();

        // 敵戦力の表示修正用(インゲームの試合開始画面での戦力の表示を弱体化時のものに変える)
        public BigValue FixedEnemyCombatPower = BigValue.Zero;
        
        public async Task<List<HuntResultStatus>> GetHuntResultList()
        {
            if (huntResultList == null || huntResultList?.Count == 0)
            {
                await GetHuntGetDataAPI();
            }
            return huntResultList;
        }

        public async UniTask GetHuntGetDataAPI()
        {
            HuntGetDataAPIRequest request = new HuntGetDataAPIRequest();
            HuntGetDataAPIPost post = new HuntGetDataAPIPost();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            huntResultList = response.huntResultList.ToList();
        }

        public async UniTask UpdateHuntResultStatus(long mHuntTimetableId, long dailyPlayCount)
        {
            if (huntResultList == null || huntResultList?.Count == 0)
            {
                await GetHuntGetDataAPI();
            }
            var huntResultStatus = huntResultList.FirstOrDefault(data => data.mHuntTimetableId == mHuntTimetableId);
            if (huntResultStatus == null)
            {
                huntResultStatus = new HuntResultStatus(){mHuntTimetableId=mHuntTimetableId, dailyPlayCount=dailyPlayCount};
                huntResultList.Add(huntResultStatus);
            }
            huntResultStatus.dailyPlayCount = dailyPlayCount;
        }

        public bool CanMatchAboveLimit(long mHuntTimetableId)
        {
            var huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindData(mHuntTimetableId);
            if (huntTimetableMasterObject == null || huntTimetableMasterObject.dailyPlayCount == 0) return true;

            var usedLimit = huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            var remainingLimit = huntTimetableMasterObject.dailyPlayCount - usedLimit;

            return (remainingLimit > 0);
        }

        public static string GetHomeRivalryButtonNotificationLabel()
        {
            var cachedData = rivalryCacheData;
            var hasCampaign = cachedData.huntMasterObjectDataList
                .Any(aData => cachedData.huntTimeTableDictionary.TryGetValue(aData.id, out var timetables) && timetables.Any(timetable => timetable.mPointId > 0));
            return hasCampaign ? "イベント開催中!" : string.Empty;
        }

        public static bool GetHomeBadgeFlag()
        {
            var cachedData = rivalryCacheData;
            var now = AppTime.Now;
            foreach (var huntMasterObject in cachedData.huntMasterObjectDataList)
            {
                if (cachedData.huntTimeTableDictionary.TryGetValue(huntMasterObject.id, out var huntTimetables) &&
                    cachedData.huntEnemyObjectList.TryGetValue(huntMasterObject.id, out var showingEnemy))
                {
                    foreach (var huntTimetable in huntTimetables) {
                        // レギューらーは判定しない
                        if (GetMatchType(huntTimetable, huntMasterObject) == RivalryMatchType.Regular) continue;
                        if (huntTimetable.endAt.TryConvertToDateTime().IsPast(now)) continue;
                    
                        // イベント「頂点への挑戦」タイプの場合
                        if (huntMasterObject.isClosedOnceWin)
                        {
                            if (showingNewIconEventDataContainer.ShowBadge(mHuntId: huntMasterObject.id, mHuntTimetableId: huntTimetable.id)) return true;
                        }
                        else
                        {
                            // 記録がない場合、return:true
                            if (!cachedData.challengedDictionary.TryGetValue(huntTimetable.id, out var challengedData)) return true;

                            // 記録した数が表示される数より少ない場合、return:true
                            if (challengedData.Count < showingEnemy.Count) return true;
                        }
                    }
                }
            }

            return false;
        }

        public static RivalryMatchType GetMatchType(HuntTimetableMasterObject huntTimetableMasterObject, HuntMasterObject huntMasterObject)
        {
            return huntTimetableMasterObject?.type == 1 && huntMasterObject?.lotteryType == 1 ? RivalryMatchType.Event : RivalryMatchType.Regular;
        }
        
        #endregion

        #region RivalryCacheData
        private static RivalryCacheData _rivalryCacheData = null;
        public static RivalryCacheData rivalryCacheData
        {
            get
            {
                if (_rivalryCacheData == null)
                {
                    var huntTimetableMasterObjects = MasterManager.Instance.huntTimetableMaster.values;
                    var huntTimeTableDictionary = huntTimetableMasterObjects
                        .GroupBy(aData => aData.mHuntId)
                        .ToDictionary(aData => aData.Key, aData => aData.ToList());
                    _rivalryCacheData = new RivalryCacheData
                    {
                        huntMasterObjectDataList = GetHuntMasterObjects(huntTimeTableDictionary),
                        huntTimeTableDictionary = huntTimeTableDictionary,
                        challengedDictionary = challengedEventMatchDataContainer.challengedDataList.GroupBy(aData => (long)aData.mHuntTimetableId).ToDictionary(aData => aData.Key, aData => aData.ToList()),
                        huntEnemyObjectList = MasterManager.Instance.huntEnemyMaster.values.GroupBy(aData => aData.mHuntId).ToDictionary(aData => aData.Key, aData => aData.ToList())
                    };
                }

                return _rivalryCacheData;
            }
        }

        /// <summary>
        /// ホーム画面へ遷移するたびにリセットする
        /// </summary>
        public static void ResetCache()
        {
            _rivalryCacheData = null;
            _showingNewIconEventDataContainer = null;
            _challengedEventMatchDataContainer = null;
            _seenUnlockedEventMatchDataContainer = null;
            _towerCompleteEventMatchDataContainer = null;
        }

        public class RivalryCacheData
        {
            public List<HuntMasterObject> huntMasterObjectDataList; // RivalryTopに表示されるもの
            public Dictionary<long, List<HuntEnemyMasterObject>> huntEnemyObjectList; // key: mHuntId
            public Dictionary<long, List<HuntTimetableMasterObject>> huntTimeTableDictionary; // key: mHuntId
            public Dictionary<long, List<ChallengedEventMatchData>> challengedDictionary; // key: mHuntTimetableId
        }

        private static List<HuntMasterObject> GetHuntMasterObjects(Dictionary<long, List<HuntTimetableMasterObject>> huntTimeTableDictionary)
        {
            var huntMasterList = MasterManager.Instance.huntMaster.values.ToList();
            var now = AppTime.Now;
            return huntMasterList
                .Where(aData =>
                    huntTimeTableDictionary.TryGetValue(aData.id, out var timeTables) &&
                    timeTables.Any(timeTable => 
                        timeTable.type == 1 &&
                        timeTable.startAt.TryConvertToDateTime().IsPast(now) &&
                        timeTable.viewEndAt.TryConvertToDateTime().IsFuture(now)))
                .ToList();
        }
        #endregion

        #region StaticMethods
        public static void OnClickRivalryButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Rivalry, true, null);
        }

        public async void OnRivalryBattleStart(long mHuntEnemyId, long mHuntTimetableId)
        {
            challengedEventMatchDataContainer.OnBattleStartEventMatch(mHuntEnemyId, mHuntTimetableId);
            var response = await StoryManager.CallHuntStartApi(mHuntEnemyId, mHuntTimetableId);
            OpenInGame(response.huntPending);
        }

        private void OpenInGame(HuntUserPending huntPending)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false,
                new NewInGameOpenArgs(PageType.Rivalry, huntPending.clientData, FixedEnemyCombatPower, null));
        }

        public static HuntSpecificCharaMasterObject GetRewardBoost(long mHuntTimetableId)
        {
            var now = AppTime.Now;
            return MasterManager.Instance.huntSpecificCharaMaster.values.FirstOrDefault (data =>
                data.mHuntTimetableId == mHuntTimetableId &&
                data.startAt.TryConvertToDateTime().IsPast(now) &&
                data.endAt.TryConvertToDateTime().IsFuture(now));
        }

        public static long GetRewardBoostValue(long mHuntTimetableId, DeckPanelScrollGridItem.Parameters deckParameters)
        {
            long value = 0;
            var mHuntSpecificChara = GetRewardBoost(mHuntTimetableId);
            if (mHuntSpecificChara == null) return 0;
            List<object> mCharaIdList = (List<object>)MiniJSON.Json.Deserialize(mHuntSpecificChara.mCharaIdList);
            foreach (var iconParam in deckParameters.viewParams.iconParams)
            {
                if (iconParam.nullableCharacterData != null && mCharaIdList.Contains((long)iconParam.nullableCharacterData?.MCharaId))
                {
                    value += (mHuntSpecificChara.rate/100);
                }
            }
            return value;
        }

        public static async Task<long> GetRewardBoostValueAsync(long mHuntTimetableId)
        {
            long value = 0;
            var mHuntSpecificChara = GetRewardBoost(mHuntTimetableId);
            if (mHuntSpecificChara == null) return 0;
            var deckListData = await DeckUtility.GetBattleDeck();
            List<object> mCharaIdList = (List<object>)MiniJSON.Json.Deserialize(mHuntSpecificChara.mCharaIdList);
            var memberIdList = deckListData.DeckDataList[deckListData.SelectingIndex].GetMemberIds();
            foreach (var memberId in memberIdList)
            {
                var mCharaId = memberId > 0 ? new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(memberId))?.MCharaId : 0;
                if (mCharaIdList.Contains((long)mCharaId))
                {
                    value += (mHuntSpecificChara.rate/100);
                }
            }
            return value;
        }

        /// <summary> 条件タイプ取得 </summary>
        public static HuntDeckRegulationType GetRegulationType(long regulationId)
        {
            // 条件タイプ取得
            var regulationMaster = MasterManager.Instance.huntDeckRegulationMaster.FindData(regulationId);
            HuntDeckRegulationType regulationType = HuntDeckRegulationType.None;
            if (regulationMaster != null)
            {
                regulationType = regulationMaster.HuntConditionType;
            }

            return regulationType;
        }

        /// <summary> すべての条件を満たしているかチェック </summary>
        public static bool CheckHuntCondition(IEnumerable<CharacterVariableDetailData> deckMemberList, IEnumerable<HuntDeckRegulationConditionMasterObject> huntRegulationConditionList)
        {
            // 条件なし
            if (huntRegulationConditionList == null)
            {
                return true;
            }
            
            foreach (HuntDeckRegulationConditionMasterObject master in huntRegulationConditionList)
            {
                if (IsMatchRegulationConditionType(deckMemberList, master) == false)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary> サーバから受け取った編成データを見てチェックを行う </summary>
        public static bool CheckHuntCondition(BattleV2ClientData clientData, IEnumerable<HuntDeckRegulationConditionMasterObject> huntRegulationConditionList)
        {
            // 条件なし
            if (huntRegulationConditionList == null)
            {
                return true;
            }
            
            // プレイヤーの所持キャラ
            List<BattleV2Chara> playerCharaList = new List<BattleV2Chara>();
            foreach(BattleV2Chara c in clientData.charaList)
            {
                BattleV2Player p = clientData.playerList[c.playerIndex - 1];
                // 自分のキャラを取得する
                if(p.playerType == (int)HuntPlayerType.Player)
                {
                    playerCharaList.Add(c);
                }
            }
            
            foreach (HuntDeckRegulationConditionMasterObject master in huntRegulationConditionList)
            {
                if (IsMatchRegulationConditionType(playerCharaList, master) == false)
                {
                    return false;
                }
            }
            
            return true;
            
        }
        
        /// <summary> 条件を満たしているか </summary>
        private static bool IsMatchRegulationConditionType(IEnumerable<CharacterVariableDetailData> deckMemberList, HuntDeckRegulationConditionMasterObject conditionMaster)
        {
            switch (conditionMaster.RegulationLogicType)
            {
                // 特定キャラクターが編成されているか
                case HuntDeckRegulationLogicType.CharacterParentRequire:
                {
                    // 指定の親キャラId
                    long conditionCharaParentId = conditionMaster.value;
                    // 比較タイプ
                    CompareOperationType compareType = CompareOperationUtility.OperationType(conditionMaster.operatorType);
                    // 条件に一致した数
                    long matchCount = 0;
                    
                    foreach (CharacterVariableDetailData data in deckMemberList)
                    {
                        // 親キャラが一致
                        if (MasterManager.Instance.charaMaster.FindData(data.MCharaId).parentMCharaId == conditionCharaParentId)
                        {
                            matchCount++;
                        }
                    }
                    // 比較条件を満たしたか
                    return CompareOperationUtility.IsCompare(compareType, matchCount, conditionMaster.compareValue);
                }
            }

            return false;
        }
        
        /// <summary> 条件を満たしているか </summary>
        private static bool IsMatchRegulationConditionType(IEnumerable<BattleV2Chara> deckMemberList, HuntDeckRegulationConditionMasterObject conditionMaster)
        {
            switch (conditionMaster.RegulationLogicType)
            {
                // 特定キャラクターが編成されているか
                case HuntDeckRegulationLogicType.CharacterParentRequire:
                {
                    // 指定の親キャラId
                    long conditionCharaParentId = conditionMaster.value;
                    // 比較タイプ
                    CompareOperationType compareType = CompareOperationUtility.OperationType(conditionMaster.operatorType);
                    // 条件に一致した数
                    long matchCount = 0;
                    
                    foreach (BattleV2Chara data in deckMemberList)
                    {
                        // 親キャラが一致
                        if (data.parentMCharaId == conditionCharaParentId)
                        {
                            matchCount++;
                        }
                    }
                    // 比較条件を満たしたか
                    return CompareOperationUtility.IsCompare(compareType, matchCount, conditionMaster.compareValue);
                }
            }

            return false;
        }

        
        #endregion

        #region イベント「頂点への挑戦」のバッジ判定関連
        private static ShowingNewIconEventDataContainer _showingNewIconEventDataContainer;
        public static ShowingNewIconEventDataContainer showingNewIconEventDataContainer => _showingNewIconEventDataContainer ??= ShowingNewIconEventDataContainer.LoadFromPlayerPrefs();

        [Serializable]
        public class ShowingNewIconEventData
        {
            [SerializeField] public long mHuntId;
            [SerializeField] public long mHuntTimetableId;

            public ShowingNewIconEventData(long mHuntId, long mHuntTimetableId)
            {
                this.mHuntId = mHuntId;
                this.mHuntTimetableId = mHuntTimetableId;
            }

            public bool Equals(long mHuntId, long mHuntTimetableId)
            {
                return this.mHuntId == mHuntId && this.mHuntTimetableId == mHuntTimetableId;
            }
        }

        [Serializable]
        public class ShowingNewIconEventDataContainer
        {
            [SerializeField] private List<ShowingNewIconEventData> hasNoNewMatchEventDataList = new();

            #region PublicMethods
            public void OnRivalryEventPageShowListItem(long mHuntId, long mHuntTimetableId, bool hasNewMatch)
            {
                var noNewMatchDataIndex = hasNoNewMatchEventDataList.FindIndex(aData => aData.Equals(mHuntId, mHuntTimetableId));
                if (hasNewMatch && noNewMatchDataIndex > -1)
                {
                    hasNoNewMatchEventDataList.RemoveAt(noNewMatchDataIndex);
                    Save();
                }
                else if (!hasNewMatch && noNewMatchDataIndex == -1)
                {
                    hasNoNewMatchEventDataList.Add(new ShowingNewIconEventData(mHuntId, mHuntTimetableId));
                    Save();
                }
            }

            public bool ShowBadge(long mHuntId, long mHuntTimetableId)
            {
                var hasNoNewMatch = hasNoNewMatchEventDataList.Exists(aData => aData.Equals(mHuntId, mHuntTimetableId));
                return !hasNoNewMatch;
            }
            #endregion

            #region PlayerPrefs
            private static readonly string ShowingNewIconEventDataContainerKey = "ShowingNewIconEventDataContainer";
            private void Save()
            {
                var json = ToJson();
                // Debug.Log($"ShowingNewIconEventDataContainer.Save json:{json}\n{ToString()}");
                ObscuredPrefs.Set<string>(ShowingNewIconEventDataContainerKey, json);
            }

            public static ShowingNewIconEventDataContainer LoadFromPlayerPrefs()
            {
                var retVal = FromJson(ObscuredPrefs.Get<string>(ShowingNewIconEventDataContainerKey, "{}"));
                // Debug.Log($"ShowingNewIconEventDataContainer.LoadFromPlayerPrefs retVal:{retVal}");
                return retVal;
            }
            #endregion

            #region PrivateMethods
            private string ToJson() => JsonUtility.ToJson(this);
            private static ShowingNewIconEventDataContainer FromJson(string json)
            {
                return JsonUtility.FromJson<ShowingNewIconEventDataContainer>(json);
            }
            #endregion
        }
        #endregion

        #region Newアイコン関連
        private static ChallengedEventMatchDataContainer _challengedEventMatchDataContainer;
        public static ChallengedEventMatchDataContainer challengedEventMatchDataContainer => _challengedEventMatchDataContainer ??= ChallengedEventMatchDataContainer.LoadFromPlayerPrefs();

        [Serializable]
        public class ChallengedEventMatchData
        {
            [SerializeField] public long mHuntEnemyId;
            [SerializeField] public long mHuntTimetableId;

            public ChallengedEventMatchData(long mHuntEnemyId, long mHuntTimetableId)
            {
                this.mHuntEnemyId = mHuntEnemyId;
                this.mHuntTimetableId = mHuntTimetableId;
            }

            public bool Equals(long mHuntEnemyId, long mHuntTimetableId)
            {
                return this.mHuntEnemyId == mHuntEnemyId && this.mHuntTimetableId == mHuntTimetableId;
            }
        }

        [Serializable]
        public class ChallengedEventMatchDataContainer
        {
            [SerializeField] public List<ChallengedEventMatchData> challengedDataList = new();

            #region PublicMethods
            /// <summary>
            /// 端末変更対策：クリア済みものを記録する
            /// </summary>
            public void OnShowEventMatchPage(HuntEnemyHistory huntEnemyHistory)
            {
                var clearedEnemyIdList = huntEnemyHistory.mHuntEnemyIdList;
                var mHuntTimetableId = huntEnemyHistory.mHuntTimetableId;
                var unregisteredEnemyIdList = clearedEnemyIdList
                    .Where(huntEnemyId => !challengedDataList.Exists(aData => aData.Equals(huntEnemyId, mHuntTimetableId)))
                    .Select(huntEnemyId => new ChallengedEventMatchData(huntEnemyId, mHuntTimetableId)).ToList();
                if (unregisteredEnemyIdList.Count <= 0) return;

                challengedDataList.AddRange(unregisteredEnemyIdList);
                Save();
            }

            /// <summary>
            /// 記録する
            /// </summary>
            public void OnBattleStartEventMatch(long mHuntEnemyId, long mHuntTimetableId)
            {
                if (challengedDataList.Exists(aData => aData.Equals(mHuntEnemyId, mHuntTimetableId))) return;
                challengedDataList.Add(new ChallengedEventMatchData(mHuntEnemyId: mHuntEnemyId, mHuntTimetableId: mHuntTimetableId));
                Save();
            }

            public override string ToString()
            {
                var retVal = string.Empty;
                challengedDataList?.ForEach(aData => retVal += "\n" + aData);
                return retVal;
            }
            #endregion

            #region PlayerPrefs
            private static readonly string ChallengedEventMatchDataKey = "ChallengedEventMatchData";
            public void Save()
            {
                var json = ToJson();
                // Debug.Log($"ShownEventMatchDataContainer.Save json:{json}\n{ToString()}");
                ObscuredPrefs.Set<string>(ChallengedEventMatchDataKey, json);
            }

            public static ChallengedEventMatchDataContainer LoadFromPlayerPrefs()
            {
                var retVal = FromJson(ObscuredPrefs.Get<string>(ChallengedEventMatchDataKey, "{}"));
                // Debug.Log($"ShownEventMatchDataContainer.LoadFromPlayerPrefs retVal:{retVal}");
                return retVal;
            }
            #endregion


            #region PrivateMethods
            private string ToJson() => JsonUtility.ToJson(this);
            private static ChallengedEventMatchDataContainer FromJson(string json)
            {
                return JsonUtility.FromJson<ChallengedEventMatchDataContainer>(json);
            }
            #endregion
        }
        #endregion


        #region ロック解放関連
        private static SeenUnlockedEventMatchDataContainer _seenUnlockedEventMatchDataContainer;
        public static SeenUnlockedEventMatchDataContainer seenUnlockedEventMatchDataContainer => _seenUnlockedEventMatchDataContainer ??= SeenUnlockedEventMatchDataContainer.LoadFromPlayerPrefs();

        [Serializable]
        public class SeenUnlockedEventMatchData
        {
            [SerializeField] public long mHuntEnemyId;
            [SerializeField] public long mHuntTimetableId;

            public SeenUnlockedEventMatchData(long mHuntEnemyId, long mHuntTimetableId)
            {
                this.mHuntEnemyId = mHuntEnemyId;
                this.mHuntTimetableId = mHuntTimetableId;
            }

            public bool Equals(long mHuntEnemyId, long mHuntTimetableId)
            {
                return this.mHuntEnemyId == mHuntEnemyId && this.mHuntTimetableId == mHuntTimetableId;
            }
        }

        [Serializable]
        public class SeenUnlockedEventMatchDataContainer
        {
            [SerializeField] public List<SeenUnlockedEventMatchData> seenUnlockedDataList = new();

            #region PublicMethods
            /// <summary>
            /// 端末変更対策：ロック解放アニメーション見たかのを記録する
            /// </summary>
            public void OnSeenUnlockedAnimation(long mHuntEnemyId, long mHuntTimetableId)
            {
                seenUnlockedDataList.Add(new SeenUnlockedEventMatchData(mHuntEnemyId, mHuntTimetableId));
                Save();
            }

            public override string ToString()
            {
                var retVal = string.Empty;
                seenUnlockedDataList?.ForEach(aData => retVal += "\n" + aData);
                return retVal;
            }
            #endregion

            #region PlayerPrefs
            private static readonly string SeenUnlockedEventMatchDataKey = "SeenUnlockedEventMatchData";
            public void Save()
            {
                var json = ToJson();
                ObscuredPrefs.Set<string>(SeenUnlockedEventMatchDataKey, json);
            }

            public static SeenUnlockedEventMatchDataContainer LoadFromPlayerPrefs()
            {
                var retVal = FromJson(ObscuredPrefs.Get<string>(SeenUnlockedEventMatchDataKey, "{}"));
                return retVal;
            }
            #endregion


            #region PrivateMethods
            private string ToJson() => JsonUtility.ToJson(this);
            private static SeenUnlockedEventMatchDataContainer FromJson(string json)
            {
                return JsonUtility.FromJson<SeenUnlockedEventMatchDataContainer>(json);
            }
            #endregion
        }
        #endregion

        #region 塔イベントCOMPLETE関連
        private static TowerCompleteEventMatchDataContainer _towerCompleteEventMatchDataContainer;
        public static TowerCompleteEventMatchDataContainer towerCompleteEventMatchDataContainer => _towerCompleteEventMatchDataContainer ??= TowerCompleteEventMatchDataContainer.LoadFromPlayerPrefs();

        [Serializable]
        public class TowerCompleteEventMatchData
        {
            [SerializeField] public long mHuntTimetableId;

            public TowerCompleteEventMatchData(long mHuntTimetableId)
            {
                this.mHuntTimetableId = mHuntTimetableId;
            }

            public bool Equals(long mHuntTimetableId)
            {
                return this.mHuntTimetableId == mHuntTimetableId;
            }
        }

        [Serializable]
        public class TowerCompleteEventMatchDataContainer
        {
            [SerializeField] public List<TowerCompleteEventMatchData> towerCompleteDataList = new();

            #region PublicMethods
            /// <summary>
            /// 端末変更対策：記録する
            /// </summary>
            public void OnTowerCompleteAnimation(long mHuntTimetableId)
            {
                towerCompleteDataList.Add(new TowerCompleteEventMatchData(mHuntTimetableId));
                Save();
            }

            public void ResetIfCompleted(long mHuntTimetableId)
            {
                int index = towerCompleteDataList.FindIndex(data => data.mHuntTimetableId == mHuntTimetableId);
                if (index >= 0)
                {
                    towerCompleteDataList.RemoveAt(index);
                    Save();
                }
            }

            public override string ToString()
            {
                var retVal = string.Empty;
                towerCompleteDataList?.ForEach(aData => retVal += "\n" + aData);
                return retVal;
            }
            #endregion

            #region PlayerPrefs
            private static readonly string TowerCompleteEventMatchDataKey = "TowerCompleteEventMatchData";
            public void Save()
            {
                var json = ToJson();
                ObscuredPrefs.Set<string>(TowerCompleteEventMatchDataKey, json);
            }

            public static TowerCompleteEventMatchDataContainer LoadFromPlayerPrefs()
            {
                var retVal = FromJson(ObscuredPrefs.Get<string>(TowerCompleteEventMatchDataKey, "{}"));
                return retVal;
            }
            #endregion


            #region PrivateMethods
            private string ToJson() => JsonUtility.ToJson(this);
            private static TowerCompleteEventMatchDataContainer FromJson(string json)
            {
                return JsonUtility.FromJson<TowerCompleteEventMatchDataContainer>(json);
            }
            #endregion
        }
        #endregion

        
        #region 奪選手選択
        [Serializable]
        public class StealCharaEnemyData
        {
            [SerializeField] public long mHuntEnemyId;
            [SerializeField] public long mCharaId;

            public StealCharaEnemyData(long mHuntEnemyId, long mCharaId)
            {
                this.mHuntEnemyId = mHuntEnemyId;
                this.mCharaId = mCharaId;
            }

            public bool Equals(long mHuntEnemyId, long mCharaId)
            {
                return this.mHuntEnemyId == mHuntEnemyId && this.mCharaId == mCharaId;
            }
        }

        [Serializable]
        public class StealCharaEnemyDataContainer
        {
            [SerializeField] public List<StealCharaEnemyData> stealCharaEnemyDataList = new();

            #region PublicMethods
            /// <summary>
            /// 端末変更対策：奪選手選択データ
            /// </summary>
            public void UpdateStealCharaId(long mHuntEnemyId, long mCharaId)
            {
                var stealCharaData = stealCharaEnemyDataList.FirstOrDefault(data => data.mHuntEnemyId == mHuntEnemyId);
                var isSave = true;
                if (stealCharaData != null)
                {
                    isSave = stealCharaData.mCharaId != mCharaId;
                    stealCharaData.mCharaId = mCharaId;
                }
                else
                {
                    stealCharaEnemyDataList.Add(new StealCharaEnemyData(mHuntEnemyId, mCharaId));
                }

                if (isSave) LocalSaveManager.Instance.SaveData();
            }

            public long GetStealCharaId(long mHuntEnemyId)
            {
                return stealCharaEnemyDataList.FirstOrDefault(data => data.mHuntEnemyId == mHuntEnemyId)?.mCharaId ?? -1;
            }
            #endregion

        }
        #endregion
    }
}