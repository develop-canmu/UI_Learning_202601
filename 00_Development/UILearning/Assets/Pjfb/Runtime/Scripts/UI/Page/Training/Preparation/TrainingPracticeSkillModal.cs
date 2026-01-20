using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Logger = CruFramework.Logger;

namespace Pjfb.Training
{
    public class TrainingPracticeSkillModal : ModalWindow
    {
        
        public enum OptionType
        {
            None = 0,
            Join = 1 << 0
        }
    
        public class CharacterData
        {            
            private OptionType options = OptionType.None;
            /// <summary>オプション</summary>
            public OptionType Options{get{return options;}}
            
            private TrainingCharacterData character;
            /// <summary>キャラデータ</summary>
            public TrainingCharacterData Character{get{return character;}}
            
            private long[] statusIdList = null;
            public long[] StatusIdList{get{return statusIdList;}}
            
            public CharacterData(TrainingCharacterData character, OptionType options)
            {
                this.character = character;
                this.options = options;
            }
            
            public CharacterData(TrainingCharacterData character, long[] statusIdList, OptionType options)
            {
                this.character = character;
                this.options = options;
                this.statusIdList = statusIdList;
            }
        }
    
        public class Arguments
        {
        
            private TrainingCard card = null;
            /// <summary>Card</summary>
            public  TrainingCard Card{get{return card;}}

            private TrainingActiveTrainingStatusType[] activeTrainingStatusTypes = null;

            public TrainingActiveTrainingStatusType[] ActiveTrainingStatusTypes
            {
                get { return activeTrainingStatusTypes; }
            }
        
            private List<CharacterData> characters = null;
            /// <summary>uChar</summary>
            public IReadOnlyList<CharacterData> Characters{get{return characters;}}
            
            private List<CharacterData> specialSupportCharacters = null;
            /// <summary>uChar</summary>
            public IReadOnlyList<CharacterData> SpecialSupportCharacters{get{return specialSupportCharacters;}}
            
            private List<CharacterData> equipments = null;
            /// <summary>uChar</summary>
            public IReadOnlyList<CharacterData> Equipments{get{return equipments;}}

            // アドバイザー
            private List<CharacterData> advisers = null;
            public IReadOnlyList<CharacterData> Advisers{get{return advisers;}}
            
            private long trainingScenarioId = -1;
            /// <summary>シナリオId</summary>
            public long TrainingScenarioId{get{return trainingScenarioId;}}
            
            private bool enableHighlight = false;
            /// <summary>ハイライト</summary>
            public bool EnableHighlight{get{return enableHighlight;}}
            
            private bool canGrowth = false;
            /// <summary>レベル上げ出来るか</summary>
            public bool CanGrowth{get{return canGrowth;}}
            
            public Arguments(TrainingCard card, TrainingActiveTrainingStatusType[] activeTrainingStatusTypes, long trainingScenarioId, bool enableHighlight, List<CharacterData> characters, List<CharacterData> specialSupportCharacters, List<CharacterData> equipments, List<CharacterData> advisers, bool canGrowth)
            {
                this.card = card;
                this.activeTrainingStatusTypes = activeTrainingStatusTypes;
                this.characters = characters;
                this.specialSupportCharacters = specialSupportCharacters;
                this.trainingScenarioId = trainingScenarioId;
                this.equipments = equipments;
                this.advisers = advisers;
                this.enableHighlight = enableHighlight;
                this.canGrowth = canGrowth;
            }
        }
        
        private enum ActiveTrainingId
        {
            TrainingComboBuff = -1,
            CombinationTraining = -2,
            Trainer = -4,
        }

        private class TotalSkillData
        {
            // training_status_type_detailのid
            private long typeDetailId;
            public long TypeDetailId{get{return typeDetailId;}}
            
            // training_status_type_detailのType
            private long typeDetailType;
            public long TypeDetailType{get{return typeDetailType;}}
            // CharacterDataのリスト
            private List<TrainingPracticeSkillDetailView.IndividualCharacterData> characterDataList;
            public List<TrainingPracticeSkillDetailView.IndividualCharacterData> CharacterDataList{get{return characterDataList;}}
            
            // 描画優先度
            private long displayPriority;
            public long DisplayPriority{get{return displayPriority;}}
            
            // 選択中
            private bool isSelected = false;
            public bool IsSelected{get{return isSelected;}}
            
            public TotalSkillData(long typeDetailId,long typeDetailType, List<TrainingPracticeSkillDetailView.IndividualCharacterData> characters, long displayPriority, bool isSelected)
            {
                this.typeDetailId = typeDetailId;
                this.typeDetailType = typeDetailType;
                characterDataList = characters;
                this.displayPriority = displayPriority;
                this.isSelected = isSelected;
            }
            
            public void AddCharacter(TrainingPracticeSkillDetailView.IndividualCharacterData character)
            {
                characterDataList.Add(character);
            }
            
            public void AddCharacterList(List<TrainingPracticeSkillDetailView.IndividualCharacterData> characters)
            {
                characterDataList.AddRange(characters);
            }

            public void SortCharacterDataList()
            {
                // 個別リストのソート
                characterDataList = characterDataList
                    // EXか
                    .OrderByDescending( (v)=>CharacterUtility.IsExtraCharacter(v.CharaId))
                    // index順
                    .ThenBy( (v)=>v.Index)
                    // マスタの種類順
                    .ThenBy((v) => v.PracticeSkillInfo.MasterType)
                    // マスタID順
                    .ThenBy((v) => v.PracticeSkillInfo.MasterId)
                    // レベル順
                    .ThenByDescending((v)=>
                    {
                        if (v.PracticeSkillInfo.MasterType != PracticeSkillMasterType.CharaTrainingStatusSub && 
                            v.PracticeSkillInfo.MasterType != PracticeSkillMasterType.TrainingPointStatusEffectChara)
                        {
                            return MasterManager.Instance.charaTrainingStatusMaster.FindData(v.PracticeSkillInfo.MasterId).level;
                        }
                        
                        return 0;
                    })
                    .ToList();
            }
            
        }
        
        
        [SerializeField]
        private ScrollDynamic totalSkillScroll = null;
        
        [SerializeField]
        private ScrollGrid supportCharacterScroll = null;
        
        [SerializeField]
        private ScrollGrid specialSupportCharacterScroll = null;
        
        [SerializeField]
        private ScrollGrid equipmentScroll = null;

        [SerializeField]
        private ScrollGrid adviserScroll;
        
        [SerializeField]
        private GameObject totalNoSkill = null;
        [SerializeField]
        private GameObject supportNoSkill = null;
        [SerializeField]
        private GameObject specialSupportNoSkill = null;
        [SerializeField]
        private GameObject equipmentNoSkill = null;
        [SerializeField]
        private GameObject adviserNoSkill = null;
        [SerializeField]
        private  TrainingPracticeSkillModalSheetManager sheetManager = null;
        
        private bool isSpecialLecture = false;
        
        private Dictionary<long, List<PracticeSkillInfo>> skillDataCache = new Dictionary<long, List<PracticeSkillInfo>>();
        
        private TrainingPracticeSkillDetailView.ViewData[] totalSkillViewDataCache = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments argument = (Arguments)args;

            if(argument.Card != null)
            {
                List<CharacterData> allCharacters = new List<CharacterData> { };
                allCharacters.AddRange(argument.Characters);
                allCharacters.AddRange(argument.SpecialSupportCharacters);
                //カードに参加しているキャラの中にカード所有者がいるか
                isSpecialLecture = allCharacters.Exists(data => data.Character.MCharId == argument.Card.mCharaId && data.Options == OptionType.Join);
            }
            else
            {
                isSpecialLecture = false;
            }


            // サポートキャラのスキル
            TrainingPracticeSkillView.ViewData[] skillList = GetSkillList(argument, argument.Characters);
            // スペシャルサポートのスキル
            TrainingPracticeSkillView.ViewData[] specialSkillList = GetSkillList(argument, argument.SpecialSupportCharacters);
            // サポート器具のスキル
            TrainingPracticeSkillView.ViewData[] equipmentSkillList = GetSkillList(argument, argument.Equipments);
            // アドバイザーのスキル
            TrainingPracticeSkillView.ViewData[] adviserSkillList = GetSkillList(argument, argument.Advisers);
            
            // 総合に表示するスキルリスト
            totalSkillViewDataCache = GetTotalSkillViewData(argument);
            
            // スクロールにセット
            supportCharacterScroll.SetItems(skillList);
            specialSupportCharacterScroll.SetItems(specialSkillList);
            equipmentScroll.SetItems(equipmentSkillList);
            adviserScroll.SetItems(adviserSkillList);
            totalSkillScroll.SetItems(totalSkillViewDataCache);
            
            // スキルが無い場合の表示
            supportNoSkill.SetActive( skillList.Length <= 0 );
            specialSupportNoSkill.SetActive( specialSkillList.Length <= 0 );
            equipmentNoSkill.SetActive( equipmentSkillList.Length <= 0 );
            adviserNoSkill.SetActive(adviserSkillList.Length <= 0);
            totalNoSkill.SetActive( totalSkillViewDataCache.Length <= 0);
            
            // シートオープン時のイベント登録
            sheetManager.OnOpenSheet -= OnOpenSheet;
            sheetManager.OnOpenSheet += OnOpenSheet;
            
            return base.OnPreOpen(args, token);
        }

        // 各シートを開いたときの処理
        private void OnOpenSheet(TrainingPracticeSkillModalSheetType sheetType)
        {
            switch (sheetType)
            {
                case TrainingPracticeSkillModalSheetType.TotalPracticeSkill:
                    if (totalSkillViewDataCache == null || totalSkillViewDataCache.Length <= 0) break;
                    foreach (var viewData in totalSkillViewDataCache)
                    {
                        // プルダウンの表示状態をリセット
                        viewData.SetOpenPullDown(false);
                    }
                    // Scrollをリフレッシュ
                    totalSkillScroll.Refresh();
                    break;
                default:
                    break;
            }
        }

        private List<PracticeSkillInfo> GetSkillDataList(CharacterData characterData, long scenarioId)
        {
            // 空きスロットの場合はnullを返す
            long id = characterData.Character.MCharId;
            if(id == DeckUtility.EmptyDeckSlotId) return null;
            
            // キャッシュがあればそれを返す
            if (skillDataCache.TryGetValue(characterData.Character.UCharId, out var cachedData))
            {
                return cachedData;
            }
            
            // 各キャラのスキルを取得
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(id);
            List<PracticeSkillInfo> skillData = new List<PracticeSkillInfo>();

            switch (mChar.cardType)
            {
                case CardType.Character:
                case CardType.SpecialSupportCharacter:
                case CardType.Adviser:
                {
                    skillData.AddRange(PracticeSkillUtility.GetCharacterPracticeSkill(id, characterData.Character.Lv, scenarioId));
                    break;
                }
                case CardType.SupportEquipment:
                {
                    skillData.AddRange(PracticeSkillUtility.GetCharacterPracticeSkill(id));
                    if (characterData.StatusIdList != null)
                    {
                        skillData.AddRange(PracticeSkillUtility.GetCharaTrainerLotteryStatusPracticeSkill(characterData.StatusIdList));
                    }

                    break;
                }
            }
            
            // キャッシュに保存
            skillDataCache[characterData.Character.UCharId] = skillData;
            return skillData;
        }
        
        private TrainingPracticeSkillView.ViewData[] GetSkillList(Arguments argument, IReadOnlyList<CharacterData> dataList)
        {
            List<TrainingPracticeSkillView.ViewData> result = new List<TrainingPracticeSkillView.ViewData>();

            // 各メンバーの練習能力を取得
            for(int n=0;n<dataList.Count;n++)
            {
                CharacterData c = dataList[n];
                List<PracticeSkillInfo> skillData = GetSkillDataList(c, argument.TrainingScenarioId);
                if(skillData == null || skillData.Count <= 0) continue;
                
                foreach (var skill in skillData)
                {
                    // マスタを引っ張る
                    var detail = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(skill.TrainingStatusTypeDetailId);
                    // detailがnullかどうかをboolで持つ
                    bool isDetailNotNull = detail != null;
                    
                    // 合算表示は除外
                    if (isDetailNotNull && detail.isDisplayAggregatedEffect == true) continue;
                    
                    // サブスキルがある場合それらも引っ張る
                    var subSkillDetail = skill.SubSkills?.Select(x =>
                        MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(x.TrainingStatusTypeDetailId)).ToList();
                    // サブスキルのデータがあるか
                    bool hasSubSkill = subSkillDetail != null && subSkillDetail.Any();
                    
                    // サブスキルも全て合算表示なら除外
                    if (hasSubSkill && subSkillDetail.All(x => x.isDisplayAggregatedEffect)) continue;
                    
                    // 強調表示フラグ
                    bool isHighlight = IsSelected(argument, c, detail, subSkillDetail);
                    
                    // 特攻スキル
                    bool isSpecialAttack = IsSpecialAttack(argument,skill);
                                        
                    result.Add( new TrainingPracticeSkillView.ViewData(skill, isHighlight && argument.EnableHighlight, c.Character.MCharId, c.Character.UCharId, n, isSpecialAttack, c.Character.Lv, c.Character.LiberationId, c.StatusIdList, argument.CanGrowth));
                }
            }
            
            
            // ソートして返す
            return 
                result
                    .OrderByDescending( (v)=>v.IsSelected)
                    .ThenByDescending( (v)=>CharacterUtility.IsExtraCharacter(v.SkillInfo.GetMCharaId()))
                    .ThenBy( (v)=>v.Index)
                    .ThenBy((v) => v.SkillInfo.MasterType)
                    .ThenBy((v) => v.SkillInfo.MasterId) // 固有が先にきてしまうためソート
                    .ThenByDescending((v)=>
                    {
                        if (v.SkillInfo.MasterType != PracticeSkillMasterType.CharaTrainingStatusSub && 
                            v.SkillInfo.MasterType != PracticeSkillMasterType.TrainingPointStatusEffectChara)
                        {
                            return MasterManager.Instance.charaTrainingStatusMaster.FindData(v.SkillInfo.MasterId).level;
                        }
                        
                        return 0;
                    })
                    .ToArray();
        }
        
        // 総合に表示するスキルのリストを生成する
        private TrainingPracticeSkillDetailView.ViewData[] GetTotalSkillViewData(Arguments argument)
        {
            List<TrainingPracticeSkillDetailView.ViewData> result = new List<TrainingPracticeSkillDetailView.ViewData>();
            
            // 選手、アドバイザー、サポカ、器具の順で取得
            List<TotalSkillData> totalSkillDataList = new List<TotalSkillData>();
            GetTotalSkillDataList(totalSkillDataList, argument, argument.Characters);
            GetTotalSkillDataList(totalSkillDataList, argument, argument.Advisers);
            GetTotalSkillDataList(totalSkillDataList, argument, argument.SpecialSupportCharacters);
            GetTotalSkillDataList(totalSkillDataList, argument, argument.Equipments);
            
            // 並び替え
            var totalSkillSortList = totalSkillDataList.
                // 選択状態を優先
                OrderByDescending(v => v.IsSelected).
                // priority順
                ThenBy(v => v.DisplayPriority);
            
            // totalSkillDataListをresultに変換していく
            foreach (var totalSkillData in totalSkillSortList)
            {
                BigValue totalValue = new BigValue(0);
                foreach (var individualCharacterData in totalSkillData.CharacterDataList)
                {
                    if (individualCharacterData.PracticeSkillInfo.IsUnique())
                    {
                        // ユニークスキルの場合はサブスキルの該当detailIDの値を合算する
                        foreach (PracticeSkillInfo skill in individualCharacterData.PracticeSkillInfo.SubSkills.Where(x => x.TrainingStatusTypeDetailId == totalSkillData.TypeDetailId))
                        {
                            totalValue += skill.Value;
                        }
                    }
                    else
                    {
                        totalValue += individualCharacterData.PracticeSkillInfo.Value;
                    }
                }
                PracticeSkillInfo parentInfo = new PracticeSkillInfo(
                    totalSkillData.TypeDetailType,
                    totalValue,
                    PracticeSkillMasterType.TotalTrainingStatusSkill,
                    0,
                    null);
                result.Add(new TrainingPracticeSkillDetailView.ViewData(parentInfo, totalSkillData.CharacterDataList, totalSkillData.IsSelected));
            }
            
            return result.ToArray();
        }
        
        
        // 総合に表示するスキルのリストをcardTypeごとに取得する
        private void GetTotalSkillDataList(List<TotalSkillData> resultSkillDataList, Arguments argument, IReadOnlyList<CharacterData> dataList)
        {
            List<TotalSkillData> cacheDataList = new List<TotalSkillData>();

            // 各メンバーの練習能力を取得
            for(int n=0;n<dataList.Count;n++)
            {
                CharacterData c = dataList[n];
                List<PracticeSkillInfo> skillData = GetSkillDataList(c, argument.TrainingScenarioId);
                if(skillData == null || skillData.Count <= 0) continue;
                
                foreach (var skill in skillData)
                {
                    // マスタを引っ張る
                    var detail = MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(skill.TrainingStatusTypeDetailId);
                    // detailがnullかどうかをboolで持つ
                    bool isDetailNotNull = detail != null;
                    
                    // 特攻スキル
                    bool isSpecialAttack = IsSpecialAttack(argument, skill);
                    // 通常
                    if (isDetailNotNull && detail.isDisplayAggregatedEffect == true)
                    {
                        // resultに追加
                        AddTotalSkillData(cacheDataList, c, skill, argument, n, isSpecialAttack, detail);
                    }
                    // 固有スキル
                    else
                    {
                        // サブスキルを取る
                        var subSkillDetailList = skill.SubSkills?.Select(x => MasterManager.Instance.trainingStatusTypeDetailMaster.FindData(x.TrainingStatusTypeDetailId)).ToList();
                        // サブスキルのデータがあるか
                        if (subSkillDetailList != null && subSkillDetailList.Any())
                        {
                            foreach (var subSkillTypeDetail in subSkillDetailList)
                            {
                                if(subSkillTypeDetail.isDisplayAggregatedEffect == false) continue;
                                // resultに追加
                                AddTotalSkillData(cacheDataList, c, skill, argument, n, isSpecialAttack, subSkillTypeDetail);
                            }
                        }
                    }
                }
            }
            
            // 個別リストのソート
            foreach (var cacheSkillData in cacheDataList)
            {
                //追加済みフラグ
                bool isAdded = false;
                cacheSkillData.SortCharacterDataList();
                // 最終的なリストへ追加
                foreach (TotalSkillData resultSkillData in resultSkillDataList)
                {
                    if(resultSkillData.TypeDetailType == cacheSkillData.TypeDetailType)
                    {
                        // 既にある場合はキャラクター追加
                        resultSkillData.AddCharacterList( cacheSkillData.CharacterDataList);
                        isAdded = true;
                        break;
                    }
                }
                
                // 無ければ追加
                if (isAdded == false)
                {
                    resultSkillDataList.Add(cacheSkillData);
                }
            }
        }

        private void AddTotalSkillData(List<TotalSkillData> skillDataList, CharacterData characterData, PracticeSkillInfo skill, Arguments argument, int n, bool isSpecialAttack, TrainingStatusTypeDetailMasterObject skillTypeDetail)
        {
            var individualCharacterData = new TrainingPracticeSkillDetailView.IndividualCharacterData(skill, characterData.Character.MCharId, characterData.Character.UCharId,  characterData.Character.Lv, characterData.Character.LiberationId, characterData.StatusIdList, isSpecialAttack, n, argument.CanGrowth);
            if(skillDataList.Any(x => x.TypeDetailType == skillTypeDetail.type))
            {
                // 既にある場合は値を加算
                var existingData = skillDataList.First(x => x.TypeDetailType == skillTypeDetail.type);
                existingData.AddCharacter(individualCharacterData);
            }
            else
            {
                // 強調表示フラグ
                bool isHighlight = IsSelected(argument, characterData, skillTypeDetail, null);
                skillDataList.Add( new TotalSkillData(skillTypeDetail.id,skillTypeDetail.type, new List<TrainingPracticeSkillDetailView.IndividualCharacterData>{individualCharacterData}, skillTypeDetail.displayPriority, isHighlight));
            }
        }
        
        // 特攻スキルかどうか
        private bool IsSpecialAttack(Arguments argument, PracticeSkillInfo skill)
        {
            return argument.TrainingScenarioId != 0 && 
                   skill.MasterType != PracticeSkillMasterType.CharaTrainingStatusSub && 
                   skill.MasterType != PracticeSkillMasterType.TrainingPointStatusEffectChara && 
                   argument.TrainingScenarioId == MasterManager.Instance.charaTrainingStatusMaster.FindData(skill.MasterId).mTrainingScenarioId;
        }
        
        // 選択中か
        private bool IsSelected(Arguments argument, CharacterData c, TrainingStatusTypeDetailMasterObject detail, List<TrainingStatusTypeDetailMasterObject> subSkillDetail)
        {
            // トレーニング中のデータがある時のみ強調表示を行う
            if (argument.ActiveTrainingStatusTypes != null)
            {
                foreach (var statusType in argument.ActiveTrainingStatusTypes)
                {
                    // キャラの所持スキルでない時かつ、サポート器具（-4）以外の時
                    if (c.Character.MCharId != statusType.mCharaId && statusType.mCharaId != (long)ActiveTrainingId.Trainer) continue;
                    if (detail?.type == statusType.type)
                    {
                        return true;
                    }
                    
                    // ユニーク等のサブスキルの判定
                    if (subSkillDetail != null)
                    {
                        foreach (var info in subSkillDetail)
                        {
                            if (info.type == statusType.type)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}