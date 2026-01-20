using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Training;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pjfb
{
    public class CharacterDetailEventViewElement : MonoBehaviour
    {
        public enum EnhanceProgress
        {
            None,
            Enhanced,
            Occurrence,
            UnEnhanced,
        }

        [SerializeField]
        private GameObject enhanceInfoLabel = null;

        [SerializeField]
        private UIButton enhanceEventButton = null;

        [SerializeField]
        private GameObject enhancePullDownBackground = null;
        
        [SerializeField]
        private RubyTextMeshProUGUI enhanceEventNameText = null;
        
        [SerializeField]
        private TMP_Text eventMinLevel = null;
        
        [SerializeField]
        private GameObject enhancedProgressLabelRoot = null;
        
        [SerializeField]
        private GameObject enhancedLabel = null;
        
        [SerializeField]
        private GameObject occurrenceLabel = null;
        
        [SerializeField]
        private GameObject unEnhancedLabel = null;
        
        [SerializeField]
        private CharacterDetailEventEffectView effectViewPrefab = null;
        [SerializeField]
        private CharacterDetailSkillView skillViewPrefab = null;
        [SerializeField]
        private CharacterDetailComboView comboViewPrefab = null;
        [SerializeField]
        private TrainingGetInspirationView inspirationViewPrefab = null;

        // コンボ達成条件表示テキストルートオブジェクト
        [SerializeField]
        private RectTransform comboConditionArchiveRoot = null;

        // コンボ達成条件未達成時のルートオブジェクト
        [SerializeField]
        private RectTransform comboConditionNotArchiveRoot = null;
        
        // コンボ達成条件表示テキスト
        [SerializeField]
        private TMP_Text comboConditionArchiveText = null;
        
        [SerializeField]
        private RectTransform listRoot = null;

        [SerializeField]
        private RectTransform effectRoot = null;
        
        [SerializeField]
        private RectTransform skillRoot = null;
        
        [SerializeField]
        private RectTransform skillListRoot = null;
        
        [SerializeField]
        private GameObject skillAddLabel = null;
        
        [SerializeField]
        private RectTransform comboRoot = null;
        
        [SerializeField]
        private RectTransform comboListRoot = null;
        
        [SerializeField]
        private TMP_Text comboLabel = null;
        
        [SerializeField]
        private RectTransform inspirationRoot = null;
        
        [SerializeField]
        private GameObject inspirationAddLabel = null;
        
        [SerializeField]
        private GameObject baseOpen = null;
        [SerializeField]
        private GameObject baseClose = null;
        [SerializeField]
        private GameObject baseNoPulldown = null;

        // 強化済みのプルダウン背景
        [SerializeField]
        private GameObject enhancedBackground = null;
        
        private bool hasChildItem = false;

        // 「強化」ラベルのID
        private const long StrengthenLabel = 1;
        
        // アコーディオンUIクリック時の処理
        private Action<int, bool> onClickAccordion = null;
        // 要素番号
        private int index = 0;

        /// <summary> トレーニングデッキがセットされているか？ </summary>
        private bool hasTrainingDeck => TrainingDeckUtility.Deck != null;
        
        /// <summary>
        /// 強化イベントの場合のラベルとプルダウン背景の表示
        /// </summary>
        public void SetEnhanceEventHeader(bool active, EnhanceProgress progress, bool hasChildItem)
        {
            this.hasChildItem = hasChildItem;
            
            enhanceInfoLabel.SetActive(active);
            enhancePullDownBackground.SetActive(active && hasChildItem);
            
            // 子プルダウンそのもののアクティブ
            enhanceEventButton.gameObject.SetActive(active);
            enhanceEventButton.interactable = active && hasChildItem;
            
            // 強化済みの場合はグレーアウト
            enhancedBackground.SetActive(progress == EnhanceProgress.Enhanced);
        }

        /// <summary>
        /// 強化イベントの場合のラベル情報（イベント名、レベル条件）を設定
        /// </summary>
        public void SetEnhanceEventLabel(TrainingEventMasterObject mEvent, long minLevel, EnhanceProgress progress)
        {
            enhanceEventNameText.UnditedText = mEvent.name;
            // レベル条件がある場合のみ表示
            eventMinLevel.gameObject.SetActive(minLevel > 0);
            eventMinLevel.text = string.Format(StringValueAssetLoader.Instance["character.detail.event.condition"], minLevel);

            switch (progress)
            {
                case EnhanceProgress.None:
                case EnhanceProgress.Enhanced:
                case EnhanceProgress.Occurrence:
                    eventMinLevel.color = ColorValueAssetLoader.Instance["default"];
                    break;
                case EnhanceProgress.UnEnhanced:
                    eventMinLevel.color = ColorValueAssetLoader.Instance["red"];
                    break;
            }
        }

        /// <summary>
        /// イベント効果を表示する
        /// </summary>
        public void SetEffects(bool active, Dictionary<long, CharacterDetailEventView.EventEffectChangeState> eventEffectGroupDictionary)
        {
            foreach (var group in eventEffectGroupDictionary.OrderByDescending(g => MasterManager.Instance.trainingEventRewardTypeGroupMaster.FindData(g.Key).priority))
            {
                var groupData = MasterManager.Instance.trainingEventRewardTypeGroupMaster.FindData(group.Key);
                
                CharacterDetailEventEffectView view = GameObject.Instantiate<CharacterDetailEventEffectView>(effectViewPrefab, effectRoot);
                view.SetEffectText(groupData.name);
                
                bool isAddLabelState = group.Value != CharacterDetailEventView.EventEffectChangeState.None;
                bool isChanged = group.Value is CharacterDetailEventView.EventEffectChangeState.Increase or CharacterDetailEventView.EventEffectChangeState.Decrease;
                
                // iconIdが1（=強化）かつ、効果量の減少時は追加ラベルを表示しないフラグ
                bool isWeaken = groupData.iconId == StrengthenLabel && group.Value == CharacterDetailEventView.EventEffectChangeState.Decrease;
                
                // 追加ラベル表示
                view.SetAddLabel(isAddLabelState && isWeaken == false);
                
                // 値の変化に対するラベル表示
                // 効果量に変化があり、強化ラベルに対して弱体化が適用されていない場合のみラベルを表示
                if (isChanged && isWeaken == false)
                {
                    view.SetLabel((int)groupData.iconId).Forget();
                }
                
                view.gameObject.SetActive(true);
            }
            
            // 表示するものがある場合のみ表示
            effectRoot.gameObject.SetActive(active && eventEffectGroupDictionary.Count > 0);
        }
        
        /// <summary>
        /// 獲得可能なスキルリストを表示する
        /// </summary>
        public void SetSkills(bool active, Dictionary<SkillData, bool> skillData)
        {
            skillRoot.gameObject.SetActive(active);

            // スキルがない場合は何もしない
            if (active == false) return;
            
            foreach(var skillMap in skillData)
            {
                // 追加ラベルを先に設定
                skillAddLabel.SetActive(skillMap.Value);
                
                CharacterDetailSkillView skillView = GameObject.Instantiate<CharacterDetailSkillView>(skillViewPrefab, skillListRoot);
                skillView.SetSkillId(skillMap.Key.Id, skillMap.Key.Level);
                skillView.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 獲得可能なインスピレーションを表示する
        /// </summary>
        public void SetInspirations(bool active, Dictionary<long, bool> inspirationList)
        {
            inspirationRoot.gameObject.SetActive(active);

            if (active == false) return;

            foreach (var inspiration in inspirationList)
            {
                // 追加ラベルを先に設定
                inspirationAddLabel.SetActive(inspiration.Value);
                
                TrainingGetInspirationView inspirationView = GameObject.Instantiate<TrainingGetInspirationView>(inspirationViewPrefab, inspirationRoot);
                inspirationView.SetInspiration(inspiration.Key, false);
                inspirationView.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 発生するコンボを表示する
        /// </summary>
        public void SetCombo(bool active, TrainingEventMasterObject mEvent, TrainingUnitComboType comboType)
        {
            comboRoot.gameObject.SetActive(active);

            // コンボがない場合は何もしない
            if (active == false) return;
            
            // mUnitElement
            TrainingUnitElementMasterObject[] mUnitElements = MasterManager.Instance.trainingUnitElementMaster.FindDataByUnitId(mEvent.mTrainingUnitId);
            
            // 非表示
            comboConditionArchiveRoot.gameObject.SetActive(false);
            comboConditionNotArchiveRoot.gameObject.SetActive(false);
            
            // コンボ発動するための必要な条件一致数
            long requireCount = MasterManager.Instance.trainingUnitMaster.FindData(mEvent.mTrainingUnitId).requireCount;
            
            switch(comboType)
            {
                // 共通コンボ
                case TrainingUnitComboType.Common:
                {
                    // ビューを生成
                    CharacterDetailComboView comboView = GameObject.Instantiate<CharacterDetailComboView>(comboViewPrefab, comboListRoot);
                    comboView.gameObject.SetActive(true);
                    // ラベル
                    comboLabel.text = StringValueAssetLoader.Instance["character.detail.event.unit_label1"];
                    // ユニット名を表示
                    comboView.SetName(string.Join(" × ", mUnitElements.OrderBy(c => c.masterId).Select((c)=>c.CharacterName) ));
                    // コンボ発生ラベルを表示するか
                    bool isActiveCombo = IsActiveCombo(comboType, mUnitElements, requireCount);
                    comboView.ShowLabel(isActiveCombo);
                    // コンボ発生ラベル名セット
                    if (isActiveCombo)
                    {
                        comboView.SetConditionArchiveLabelText(StringValueAssetLoader.Instance["character.detail.event.can_unit_event"]);
                    }
                    break;
                }
                // 個別コンボ
                case TrainingUnitComboType.Individual:
                {
                    // コンボ発動条件取得
                    var otherComboElementList = mUnitElements
                        // typeを降順ソート
                        .OrderByDescending(c => c.type)
                        // mCharaIdを昇順ソート
                        .ThenBy(c => c.masterId);
                    
                    // 条件達成数
                    int matchCount = 0;
                    
                    foreach (var element in otherComboElementList)
                    {
                        // ビューを生成
                        CharacterDetailComboView comboView = GameObject.Instantiate<CharacterDetailComboView>(comboViewPrefab, comboListRoot);
                        comboView.gameObject.SetActive(true);
                        // ラベル
                        comboLabel.text = StringValueAssetLoader.Instance["character.detail.event.unit_label2"];
                        // ユニット名を表示
                        comboView.SetName(element.CharacterName);

                        var isMCharaId = element.type == TrainingUnitElementIdType.MCharId;

                        if (isMCharaId && element.minLevel > 0)
                        {
                            comboView.ShowMinLevel(string.Format(StringValueAssetLoader.Instance["character.detail.event.condition"], element.minLevel));
                        }
                        // コンボ発生ラベルを表示するか
                        bool isMatch = IsMatchComboCondition(element, true);
                        comboView.ShowLabel(isMatch);
                        if (isMatch)
                        {
                            // コンボ発生ラベル名セット
                            comboView.SetConditionArchiveLabelText(StringValueAssetLoader.Instance["training.combo.condition_archive"]);
                            matchCount++;
                        }
                    }

                    // 条件達成しているか
                    bool isConditionArchive = false;
                    
                    // トレーニングのデッキが登録されているなら条件達成している数を見る
                    if (hasTrainingDeck)
                    {
                        // 条件達成までに必要な数
                        long archiveRequireCount = requireCount - matchCount;
                        // 条件未達成時(条件達成まであと?人)
                        if (archiveRequireCount > 0)
                        {
                            comboConditionArchiveText.text = string.Format(StringValueAssetLoader.Instance["training.combo.condition_remain_count"], archiveRequireCount);
                        }
                        // 条件達成時
                        else
                        {
                            isConditionArchive = true;
                            comboConditionArchiveText.text = StringValueAssetLoader.Instance["training.combo.condition_archive"];
                        }
                    }
                    // トレーニングデッキがないなら条件達成に必要な人数を表示
                    else
                    {
                        comboConditionArchiveText.text = string.Format(StringValueAssetLoader.Instance["training.combo.condition_archive_require_count"], requireCount);
                    }
                    
                    // 条件達成しているかで表示ラベルを変える
                    comboConditionArchiveRoot.gameObject.SetActive(isConditionArchive);
                    comboConditionNotArchiveRoot.gameObject.SetActive(isConditionArchive == false);

                    break;
                }
            }
        }

        /// <summary> コンボが成立しているか </summary>
        private bool IsActiveCombo(TrainingUnitComboType comboType, TrainingUnitElementMasterObject[] unitElements, long requireCount)
        {
            switch (comboType)
            {
                case TrainingUnitComboType.None:
                {
                    return false;
                }
                case TrainingUnitComboType.Common:
                {
                    return GetConditionMatchCount(unitElements, false) >= requireCount;
                }
                // レベルチェックは個別コンボのみ
                case TrainingUnitComboType.Individual:
                {
                    return GetConditionMatchCount(unitElements, true) >= requireCount;
                }
            }

            return false;
        }

        /// <summary> コンボ対象条件の達成数 </summary>
        private int GetConditionMatchCount(TrainingUnitElementMasterObject[] unitElements, bool isMinLevelCheck)
        {
            // 現在登録されているデッキが無いなら未成立
            if(hasTrainingDeck == false)
            {
                return 0;
            }
            
            // 条件に一致した数
            int matchConditionCount = 0;
            
            foreach(TrainingUnitElementMasterObject mUnitElement in unitElements)
            {
                // 条件満たしているなら条件一致数を加算
                if(IsMatchComboCondition(mUnitElement, isMinLevelCheck))
                {
                    matchConditionCount++;
                }
            }
            
            // 条件合致数を返す
            return matchConditionCount;
        }

        /// <summary> UnitElement１つの条件を達成しているか？ </summary>
        private bool IsMatchComboCondition(TrainingUnitElementMasterObject mUnitElement, bool isMinLevelCheck)
        {
            // 現在登録されているデッキが無いなら未成立
            if(hasTrainingDeck == false)
            {
                return false;
            }
            
            // 条件に合致しているか
            bool isMatchCondition = false;
            // コンボ成立チェック
            foreach(long id in TrainingDeckUtility.Deck.MemberIds)
            {
                // キャラIdが一致
                if(mUnitElement.IsMatchCharacter(id))
                {
                    // レベル条件設定時は条件レベルを見る
                    if (isMinLevelCheck)
                    {
                        long currentLevel = TrainingDeckUtility.Deck.MemberLevelDictionary[id];
                        if (currentLevel >= mUnitElement.minLevel)
                        {
                            isMatchCondition = true;
                        }
                    }
                    else
                    {
                        isMatchCondition = true;
                    }
                    break;
                }
            }
            return isMatchCondition;
        }
        
        
        /// <summary>
        /// 強化イベントプルダウンの表示を切り替える
        /// </summary>
        /// <param name="active"></param>
        public void SetActivePullDown(bool active)
        {
            listRoot.gameObject.SetActive(active);

            // ベースの見た目切り替え
            if(hasChildItem == false)
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

        /// <summary>
        /// 強化イベントプルダウンの強化進捗ラベルを設定する
        /// </summary>
        public void SetEnhanceProgressLabel(EnhanceProgress progress)
        {
            enhancedProgressLabelRoot.SetActive(progress != EnhanceProgress.None);
            
            enhancedLabel.SetActive(progress == EnhanceProgress.Enhanced);
            occurrenceLabel.SetActive(progress == EnhanceProgress.Occurrence);
            unEnhancedLabel.SetActive(progress == EnhanceProgress.UnEnhanced);
        }

        //// <summary> ボタンクリック時のイベント処理登録 </summary>
        public void SetClickEvent(Action<int, bool> onClickEvent, int index)
        {
            this.index = index;
            onClickAccordion = onClickEvent;
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnEventNameButton()
        {
            SetActivePullDown( !listRoot.gameObject.activeSelf );
            // コールバック処理
            onClickAccordion?.Invoke(index, listRoot.gameObject.activeSelf);
        }
    }
}