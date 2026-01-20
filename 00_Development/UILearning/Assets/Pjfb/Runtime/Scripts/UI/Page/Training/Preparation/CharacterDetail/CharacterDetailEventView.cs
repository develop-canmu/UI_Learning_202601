using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;
using UnityEngine.UI;
using System;
using System.Linq;
using Pjfb.UserData;
using TMPro;

namespace Pjfb
{
    
    public class CharacterDetailEventView : MonoBehaviour
    {
        public enum EventEffectChangeState
        {
            None,
            Added,
            Increase,
            Decrease,
        }
        
        [SerializeField]
        private RubyTextMeshProUGUI eventNameText = null;
        
        [SerializeField]
        private CharacterDetailEventViewElement eventViewPrefab = null;

        [SerializeField]
        private RectTransform eventListRoot = null;
        
        [SerializeField]
        private UIButton skillNameButton = null;
        
        [SerializeField]
        private GameObject baseOpen = null;
        [SerializeField]
        private GameObject baseClose = null;
        [SerializeField]
        private GameObject baseNoPulldown = null;
        
        [SerializeField]
        private GameObject enhanceLabelRoot = null;

        // シナリオ無しラベル
        [SerializeField]
        private GameObject noScenarioLabelRoot = null;
        
        private bool hasItem = false;

        private List<GameObject> cacheObjects = new List<GameObject>();

        // アコーディオンUIクリック時の処理
        private Action onClickAccordion = null;
        private Action<bool> onSetPullDownActive = null;
        // 子要素のプルダウン開閉状況
        private List<bool> elementActivePullDownList;
        
        public void SetEvent(long mCharacterId, long charaLevel, EventSkillData eventSkill, TrainingUnitComboType comboType, bool isInitComplete, bool isPullDownActive, List<bool> elementActivePullDownList, Action<bool> onSetPullDownActive, Action onClickAccordion = null)
        {
            // mEvent
            TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData(eventSkill.EventId);
            
            // アコーディオンUIクリック時の処理
            this.onClickAccordion = onClickAccordion;
            // プルダウンのアクティブ切り替え時にItem側のフラグ更新イベント
            this.onSetPullDownActive = onSetPullDownActive;
            this.elementActivePullDownList = elementActivePullDownList;
            
            // 初期化初回時はリストの中身をクリア
            if (isInitComplete == false)
            {
                elementActivePullDownList.Clear();
            }
            
            // 強化イベントの有無
            bool hasEnhanceEvent = mEvent.enhanceGroup > 0;
            // 再生シナリオ指定があるか
            bool hasScenario = string.IsNullOrEmpty(mEvent.scenarioNumber) == false;
            
            // 現在発動しているイベントのID
            long currentEventGradeId = mEvent.id;
            
            // 表示する関連イベントのリストを生成
            List<TrainingEventMasterObject> orderedEventList = CreateRelatedOrderedEventList(mEvent, comboType);
            
            
            // レベル条件の表示に使用するレベル
            long targetCharaLevel = charaLevel;

            // 個別コンボの場合は、mUnitからベースとなるイベントのレベル条件を取得しなおす
            if (comboType == TrainingUnitComboType.Individual)
            {
                TrainingUnitElementMasterObject[] mUnitElements = MasterManager.Instance.trainingUnitElementMaster.FindDataByUnitId(mEvent.mTrainingUnitId);
            
                // 設定されている個別コンボのレベル条件が0より大きい場合（親キャラに対しての条件は無視する）
                var mCharaUnit = mUnitElements.Where(c => c.type == TrainingUnitElementIdType.MCharId).ToList();

                // 選択しているキャラにレベル条件が設定されているか
                bool hasTargetMinLevel = mCharaUnit.Any(u => u.IsMatchCharacter(mCharacterId));
                
                if (hasTargetMinLevel)
                {
                    // 選択したキャラのレベルを使用する
                    targetCharaLevel = charaLevel;
                }
                else if (TrainingDeckUtility.Deck != null)
                {
                    // トレーニングデッキがセットされている場合は優先的にレベルを取得
                    targetCharaLevel = TrainingDeckUtility.Deck.MemberLevelDictionary.GetValueOrDefault(mCharacterId, 0);
                }
                else
                {
                    // 選択したキャラ以外のキャラにレベル条件が設定されている場合
                    targetCharaLevel = UserDataManager.Instance.chara.data.Values.FirstOrDefault(x => x.MChara.id == mCharacterId)?.level ?? 0;
                }
            }
            
            
            
            // 各段階の条件判定を入れて、全て満たしているイベントを現在発生しているイベントとして扱う
            foreach (var e in orderedEventList)
            {
                if (comboType == TrainingUnitComboType.Individual)
                {
                    var mUnitElements = MasterManager.Instance.trainingUnitElementMaster.FindDataByUnitId(e.mTrainingUnitId)
                        .Where(c => c.type == TrainingUnitElementIdType.MCharId).ToArray();

                    // その段階のイベントの発生チェック
                    bool isAchieve = false;
                    if (TrainingDeckUtility.Deck != null)
                    {
                        // トレーニングデッキがセットされている場合はこちらから優先的に取得して条件を比較する
                        var deckMemberLevelDictionary = TrainingDeckUtility.Deck.MemberLevelDictionary;
                        // コンボ条件を満たしているか
                        isAchieve = mUnitElements.All(u =>
                        {
                            // 選択したキャラのレベル
                            if (u.masterId == mCharacterId)
                            {
                                return targetCharaLevel >= u.minLevel;
                            }

                            return deckMemberLevelDictionary.GetValueOrDefault(u.masterId, 0) >= u.minLevel;
                        });
                    }
                    else
                    {
                        // コンボ条件を満たしているか
                        isAchieve = mUnitElements.All(u =>
                        {
                            // 選択したキャラのレベル
                            if (u.masterId == mCharacterId)
                            {
                                return targetCharaLevel >= u.minLevel;
                            }
                            
                            return UserDataManager.Instance.chara.data.Values
                                .FirstOrDefault(x => x.MChara.id == u.masterId)?.level >= u.minLevel;
                        });
                    }
                    
                    // この段階の条件レベル
                    if (isAchieve)
                    {
                        // イベントの条件を満たしている
                        currentEventGradeId = e.id;
                    }
                }
                else
                {
                    if (targetCharaLevel >= e.minLevel)
                    {
                        currentEventGradeId = e.id;
                    }
                }
            }
            
            // 強化イベントラベルの設定
            enhanceLabelRoot.SetActive(hasEnhanceEvent);
            // シナリオなしラベル
            noScenarioLabelRoot.SetActive(hasScenario == false);
            
            // イベント名
            eventNameText.UnditedText = MasterManager.Instance.trainingEventMaster.FindData(currentEventGradeId).name;
            
            // 生成済みのオブジェクトを削除
            foreach(GameObject obj in cacheObjects)
            {
                GameObject.Destroy(obj);
            }
            cacheObjects.Clear();
            
            
            // 情報表示
            // 強化イベントの段階マップを作成
            Dictionary<long, List<TrainingEventRewardMasterObject>> eventRewardDictionary = CreateEventRewardDictionary(orderedEventList);
            
            // 前段階のイベントのステータス
            Dictionary<long, long> lastEffectTypeList = new Dictionary<long, long>();
            List<SkillData> lastSkillList = new List<SkillData>();
            List<long> lastInspirationList = new List<long>();
            
            // 現在のイベントが格納されている位置
            int currentEventIndex = orderedEventList.FindIndex(e => e.id == currentEventGradeId);
            // イベント要素の生成
            for (var index = 0; index < orderedEventList.Count; index++)
            {
                var e = orderedEventList[index];
                var eventViewElement = GameObject.Instantiate<CharacterDetailEventViewElement>(eventViewPrefab, eventListRoot);
                eventViewElement.gameObject.SetActive(true);
                // 子要素のクリック時のイベント処理に登録
                eventViewElement.SetClickEvent(OnClickElementPullDown, index);
                cacheObjects.Add(eventViewElement.gameObject);
                
                var minLevel = e.minLevel;
                if (comboType == TrainingUnitComboType.Individual)
                {
                    // キャラ指定のマスタリストを取得
                    var unitElementList = MasterManager.Instance.trainingUnitElementMaster
                        .FindDataByUnitId(e.mTrainingUnitId)
                        .Where(u => u.type == TrainingUnitElementIdType.MCharId).ToList();

                    if (unitElementList.Any())
                    {
                        minLevel =unitElementList.Max(u => u.minLevel);   
                    }
                    else
                    {
                        // マスタ設定ミスってるとき用にエラー出しておく(UnitElement側のレベル使われなくなるので)
                        CruFramework.Logger.LogError($"No List TrainingUnitElement UnitId : {e.mTrainingUnitId}");
                    }
                }

                CharacterDetailEventViewElement.EnhanceProgress progress = CharacterDetailEventViewElement.EnhanceProgress.None;

                // 現在発動しているイベントのindexと一致した場合は発動中
                if (currentEventIndex == index)
                {
                    progress = CharacterDetailEventViewElement.EnhanceProgress.Occurrence;
                }
                else
                {
                    // currentEventIndex以下は強化済み
                    progress = currentEventIndex > index
                        ? CharacterDetailEventViewElement.EnhanceProgress.Enhanced
                        : CharacterDetailEventViewElement.EnhanceProgress.UnEnhanced;
                }
                
                
                var rewards = eventRewardDictionary[e.id];
                
                // 表示するものがベースとなるイベントかどうか
                var isRootEvent = mEvent.id == e.id;
                
                // イベント効果を設定
                var effectTypeDictionary = CreateEffectTypeList(rewards);
                // 前段階のrewardとの変化がある項目をtrueに発火する辞書を作成
                var diffEffectTypeList = CompareEffectTypeList(effectTypeDictionary, lastEffectTypeList, isRootEvent);
                var filteredVisibleEffectGroupList = FilterVisibleEffectList(diffEffectTypeList);
                // 表示するイベント効果があるか
                var hasEffect = filteredVisibleEffectGroupList.Count > 0;
                eventViewElement.SetEffects(hasEffect, filteredVisibleEffectGroupList);
                
                // スキルを設定
                var skillList = SkillUtility.GetSkillList(e);
                var hasSkill = skillList.Count > 0;
                var diffSkillList = CompareSkillList(skillList, lastSkillList, isRootEvent);
                eventViewElement.SetSkills(hasSkill, diffSkillList);
                
                // インスピレーションを設定
                var inspirationList = LoadInspirationList(rewards);
                var hasInspiration = inspirationList.Count > 0;
                var diffInspirationList = CompareInspirationList(inspirationList, lastInspirationList, isRootEvent);
                eventViewElement.SetInspirations(hasInspiration, diffInspirationList);
                
                // コンボを設定
                var hasCombo = e.mTrainingUnitId > 0;
                eventViewElement.SetCombo(hasCombo, e, comboType);
                
                
                // ベースイベントのプルダウン内に中身があるか
                hasItem = hasEnhanceEvent || hasEffect || hasSkill || hasInspiration || hasCombo;
                
                // 強化イベント子プルダウンに何かしらのアイテムがあるか
                bool hasChildItem = hasEffect || hasSkill || hasInspiration || hasCombo;
                
                // 強化イベント用のヘッダーのアクティブ設定
                eventViewElement.SetEnhanceEventHeader(hasEnhanceEvent, progress, hasChildItem);
                // イベントの強化状態のラベルを設定
                eventViewElement.SetEnhanceProgressLabel(progress);
                
                // 強化イベントが存在する場合プルダウンの内容を設定
                if (hasEnhanceEvent)
                {
                    // 強化イベントの情報を設定する
                    eventViewElement.SetEnhanceEventLabel(e, minLevel, progress);
                    // 初期化時のプルダウン開閉状況のセット
                    if (isInitComplete == false)
                    {
                        // 発生するイベントのみプルダウンを展開する
                        eventViewElement.SetActivePullDown(progress == CharacterDetailEventViewElement.EnhanceProgress.Occurrence);
                        // プルダウンの開閉状況を保存
                        elementActivePullDownList.Add(progress == CharacterDetailEventViewElement.EnhanceProgress.Occurrence);
                    }
                    // 初期化時以外は保持しておいた開閉状況でセット
                    else
                    {
                        eventViewElement.SetActivePullDown(elementActivePullDownList[index]);
                    }
                }
                else
                {
                    // 強化イベント以外はプルダウンの開閉なし
                    if (isInitComplete == false)
                    {
                        elementActivePullDownList.Add(false);
                    }
                }

                lastEffectTypeList = effectTypeDictionary;
                lastSkillList = skillList;
                lastInspirationList = inspirationList;
            }

            // 強化イベント以外のプルダウンは開く
            bool activePullDown = hasEnhanceEvent == false && hasItem;
            // 初期化時でないなら以前のアクティブ状況でセットする
            if (isInitComplete)
            {
                activePullDown = isPullDownActive;
            }
            // リストは最初は閉じておく
            SetActiveSkillList(activePullDown);
            // 開閉ボタン
            skillNameButton.interactable = hasItem;
        }
        
        /// <summary>
        /// イベントで獲得できる効果の比較用リストを作成する
        /// </summary>
        private Dictionary<long, long> CreateEffectTypeList(List<TrainingEventRewardMasterObject> rewards)
        {
            Dictionary<long, long> effectTypeList = new Dictionary<long, long>();
            foreach (var reward in rewards)
            {
                for (int i = 0; i < reward.typeList.Length; i++)
                {
                    var type = reward.typeList[i];
                    var val = reward.valueList[i];
                    
                    // 追加出来た場合は続ける
                    if (effectTypeList.TryAdd(type, val)) continue;
                    
                    // 紐づくrewardが複数ありtypeが干渉する場合は、加算する
                    effectTypeList[type] += val;
                }
            }
            return effectTypeList;
        }

        /// <summary>
        /// イベント効果の前段階との比較データを作成する
        /// </summary>
        private Dictionary<long, EventEffectChangeState> CompareEffectTypeList(Dictionary<long, long> currentEffectDictionary, Dictionary<long, long> lastEffectDictionary, bool isRootEvent)
        {
            Dictionary<long, EventEffectChangeState> diffEffectTypeList = new Dictionary<long, EventEffectChangeState>();
            foreach (var effect in currentEffectDictionary)
            {
                // 親イベントでない
                var isChanged = isRootEvent == false;
                
                if (lastEffectDictionary.TryGetValue(effect.Key, out var lastValue))
                {
                    // 効果値が0でない値から変化していた場合
                    isChanged &= lastValue > 0;

                     if (isChanged && lastValue < effect.Value)
                     {
                         diffEffectTypeList.Add(effect.Key, EventEffectChangeState.Increase);
                         continue;
                     }
                     else if (isChanged && lastValue > effect.Value)
                     {
                         diffEffectTypeList.Add(effect.Key, EventEffectChangeState.Decrease);
                         continue;
                     }
                }
                else
                {
                    isChanged &= lastEffectDictionary.ContainsKey(effect.Key) == false;
                    
                    // 新規に獲得した効果の場合
                    if (isChanged)
                    {
                        diffEffectTypeList.Add(effect.Key, EventEffectChangeState.Added);
                        continue;
                    }
                }
                diffEffectTypeList.Add(effect.Key, EventEffectChangeState.None);
            }
            
            return diffEffectTypeList;
        }

        /// <summary>
        /// イベント効果の表示対象をフィルタする
        /// </summary>
        private Dictionary<long, EventEffectChangeState> FilterVisibleEffectList(Dictionary<long, EventEffectChangeState> eventEffectTypeDiffList)
        {
            // イベントグループの重複をのぞき、表示することが確定しているリストを作成
            var eventEffectDictionary = new Dictionary<long, EventEffectChangeState>();
            foreach (var typeDic in eventEffectTypeDiffList)
            {
                var groupId = MasterManager.Instance.trainingEventRewardTypeDetailMaster.values.First(reward => reward.type == typeDic.Key).mTrainingEventRewardTypeGroup;
                var groupData = MasterManager.Instance.trainingEventRewardTypeGroupMaster.FindData(groupId);
                
                // 変化がない時は表示しない設定（isChangeVisible = true）の場合はスキップ
                if (typeDic.Value == EventEffectChangeState.None && groupData.isChangeVisible)
                {
                    continue;
                }
                
                // 重複がない場合は追加
                if (eventEffectDictionary.TryAdd(groupData.id, typeDic.Value) == false && typeDic.Value != EventEffectChangeState.None)
                {
                    // ラベル付加対象の場合
                    
                    var state = eventEffectDictionary[groupData.id];
                    // 増加の場合は上書き
                    if (state == EventEffectChangeState.Decrease && typeDic.Value == EventEffectChangeState.Increase)
                    {
                        eventEffectDictionary[groupData.id] = typeDic.Value;
                    }
                }
            }
            return eventEffectDictionary;
        }
        
        
        
        /// <summary>
        /// 前段階のスキルリストから増えたものがあるか調べる
        /// </summary>
        private Dictionary<SkillData, bool> CompareSkillList(List<SkillData> currentSkillList, List<SkillData> lastSkillList, bool isRootEvent)
        {
            Dictionary<SkillData, bool> diffSkillList = new Dictionary<SkillData, bool>();
            foreach (var skill in currentSkillList)
            {
                // 親イベントでないかつ、前段階のスキルリストに含まれていない場合は追加ラベルを表示するフラグ
                var isNew = isRootEvent == false && lastSkillList.Any(s => s.Id == skill.Id) == false;
                
                diffSkillList.TryAdd(skill, isNew);
            }
            return diffSkillList;
        }
        
        /// <summary>
        /// マスターデータから獲得可能なインスピレーションを取得する
        /// </summary>
        private List<long> LoadInspirationList(List<TrainingEventRewardMasterObject> reward)
        {
            return reward.Where(r => r.mTrainingCardInspireId > 0).Select(x => x.mTrainingCardInspireId).ToList();
        }
        
        /// <summary>
        /// 前段階のインスピレーションリストから増えたものがあるか調べる
        /// </summary>
        private Dictionary<long, bool> CompareInspirationList(List<long> currentInspirationList, List<long> lastInspirationList, bool isRootEvent)
        {
            Dictionary<long, bool> diffInspirationList = new Dictionary<long, bool>();
            foreach (var inspirationId in currentInspirationList)
            {
                // 親イベントでないかつ、前段階のインスピレーションリストに含まれていない場合は追加ラベルを表示するフラグ
                var isNew = isRootEvent == false && lastInspirationList.Contains(inspirationId) == false;
                
                diffInspirationList.TryAdd(inspirationId, isNew);
            }
            return diffInspirationList;
        }

        /// <summary>
        /// イベントidに対応するTrainingEventRewardのマップを作成する
        /// </summary>
        private Dictionary<long, List<TrainingEventRewardMasterObject>> CreateEventRewardDictionary(List<TrainingEventMasterObject> eventList)
        {
            Dictionary<long, List<TrainingEventRewardMasterObject>> eventRewardDictionary = new Dictionary<long, List<TrainingEventRewardMasterObject>>();
            foreach (var e in eventList)
            {
                var rewardList = new List<TrainingEventRewardMasterObject>();
                foreach(WrapperIntList list in e.choicePrizeJson)
                {
                    TrainingEventRewardMasterObject mReward = MasterManager.Instance.trainingEventRewardMaster.FindData(list.l[1]);
                    rewardList.Add(mReward);
                }
                eventRewardDictionary.Add(e.id, rewardList);
            }
            return eventRewardDictionary;
        }
        
        
        /// <summary>
        /// 関連する強化グループのイベントの解放順を昇順にしたリストを生成する
        /// </summary>
        private List<TrainingEventMasterObject> CreateRelatedOrderedEventList(TrainingEventMasterObject mEvent, TrainingUnitComboType comboType)
        {
            List<TrainingEventMasterObject> orderedEventList = new List<TrainingEventMasterObject>();
            if (mEvent.enhanceGroup > 0)
            {
                // 強化グループが設定されている場合、同じ強化グループのイベントを取得してソート
                if (comboType == TrainingUnitComboType.Individual)
                {
                    // 個別コンボの場合は、mEvent.minLevelを参照せず、組み合わせリストの最大レベルを元にソート
                    // ここでunitMaxLevelを取得して、条件を満たす最大レベルのイベントIDを取得
                    orderedEventList = MasterManager.Instance.trainingEventMaster.values
                        .Where(e => e.enhanceGroup == mEvent.enhanceGroup)
                        .OrderBy(e =>
                        {
                            var level = MasterManager.Instance.trainingUnitElementMaster
                                .FindDataByUnitId(e.mTrainingUnitId)
                                .Where(u => u.type == TrainingUnitElementIdType.MCharId)
                                .Max(u => u.minLevel);
                            return level;
                        }).ToList();
                }
                else
                {
                    orderedEventList = MasterManager.Instance.trainingEventMaster.values
                        .Where(e => e.enhanceGroup == mEvent.enhanceGroup)
                        .OrderBy(l => l.minLevel).ToList();
                }
            }
            else
            {
                orderedEventList.Add(mEvent);
            }
            return orderedEventList;
        }
        
        
        private void SetActiveSkillList(bool active)
        {
            RectTransform t = (RectTransform)transform;
            eventListRoot.gameObject.SetActive(active);
            // プルダウンのアクティブ状況をアイテム側に反映
            onSetPullDownActive(active);
            
            // ベースの見た目切り替え
            if(hasItem == false)
            {
                baseNoPulldown.SetActive(true);
                baseOpen.SetActive(false);
                baseClose.SetActive(false);
            }
            else
            {
                baseNoPulldown.SetActive(false);
                baseOpen.SetActive(active);
                baseClose.SetActive(!active);
            }
        }

        //// <summary> 子要素のプルダウンクリック時のイベント </summary>
        private void OnClickElementPullDown(int index, bool isActive)
        {
            elementActivePullDownList[index] = isActive;
            // コールバック処理
            onClickAccordion?.Invoke();
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnEventNameButton()
        {
            SetActiveSkillList( !eventListRoot.gameObject.activeSelf );
            // コールバック処理
            onClickAccordion?.Invoke();
        }
    }
}