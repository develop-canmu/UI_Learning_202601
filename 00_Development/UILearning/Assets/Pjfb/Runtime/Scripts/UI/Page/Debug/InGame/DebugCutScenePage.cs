#if !PJFB_REL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Pjfb.Community;
using Pjfb.InGame;
using Pjfb.InGame.ClubRoyal;
using Pjfb.Master;
using Pjfb.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Pjfb.DebugPage
{
    public class DebugCutScenePage : Page
    {
        [SerializeField] private Canvas debugCanvas;
        [SerializeField]
        private DropDownUI cutSceneType; 
        [SerializeField]
        private DropDownUI specialSkillCutSceneType;
        [SerializeField]
        private TMP_InputField searchOffenceCharaInputField;
        [SerializeField]
        private TMP_InputField searchDefenceCharaInputField;
        [SerializeField]
        private TMP_InputField adviserIDInputField;
        [SerializeField]
        private TMP_InputField searchSkillInputField;
        [SerializeField]
        private DropDownUI offenceCharacter;
        [SerializeField]
        private DropDownUI defenceCharacter;
        [SerializeField] 
        private Toggle offenceTurn;
        [SerializeField] 
        private Toggle defenceTurn;
        [SerializeField] 
        private GameObject stopScreen;

        [SerializeField] private GameObject normalSpeedButton;
        [SerializeField] private GameObject doubleSpeedButton;
        [SerializeField] private GameObject nonSpeedButton;
        [SerializeField] private GameObject eightSpeedButton;
        
        [SerializeField] protected BattleLogMessageScroll logScroller;
        [SerializeField] private NewInGameMatchUpUi matchUpUi;
        [SerializeField] private InGameActivateAbilityUI activateAbilityUI;
        [SerializeField] private NewInGameDigestUI digestUi;
        [SerializeField] private NewInGameDialogueUI dialogueUI;
        [SerializeField] private InGameFieldRadarUI radarUI;
        [SerializeField] private NewInGameHeaderUI headerUI;
        [SerializeField] private NewInGameFooterUI footerUI;
        [SerializeField] private GameObject blackOverLay;
        [SerializeField] private GameObject headerAndFooterRoot;

        [SerializeField] private ClubRoyalInGameAdviserSkillActivationEffect skillActivationEffect;
        
        
        private UIDebugPanel uiDebugPanel;
        private bool isEightSpeed = false;
        public bool IsEightSpeed => isEightSpeed;

        private BattleDataMediator battleDataMediator;
        private BattleUIMediator battleUIMediator;
        
        private List<string> offenceSkillInfoList = new List<string>();
        private List<string> defenceSkillInfoList = new List<string>();
        private List<string> adviserSkillInfoList = new List<string>();

        private Dictionary<long, AbilityMasterObject>.ValueCollection abilityMasters => MasterManager.Instance.abilityMaster.values;
        private Dictionary<long,TrainingEventMasterObject>.ValueCollection trainingEventMasters => MasterManager.Instance.trainingEventMaster.values;
        private Dictionary<long, long> skillDictionary = new();
        private List<long> adviserSkills = new();
        
        // スキルカットインの再生時間
        private const float SkillCutInTime = 4.24f;
        // カットインがあるレア度のライン
        private const int HasCutInRarityBorder = 3;
        
        private enum CharacterPositionSearchType
        {
            All,
            Offence,
            Defence
        }

        private void Awake()
        {
            battleDataMediator = new BattleDataMediator();
            battleUIMediator = new BattleUIMediator();
            
            BattleUIMediator.Instance.LogMessageScroller = logScroller;
            BattleUIMediator.Instance.MatchUpUi = matchUpUi;
            BattleUIMediator.Instance.ActivateAbilityUI = activateAbilityUI;
            BattleUIMediator.Instance.DialogueUi = dialogueUI;
            BattleUIMediator.Instance.RadarUI = radarUI;
            BattleUIMediator.Instance.HeaderUI = headerUI;
            BattleUIMediator.Instance.FooterUI = footerUI;
            BattleUIMediator.Instance.BlackOverLay = blackOverLay;
            matchUpUi.transform.SetParent(gameObject.transform.GetChild(0), true);
            
            searchOffenceCharaInputField.onValueChanged.AddListener(value =>
            {
                // オフェンスのスキルリストを更新
                RefreshOffenceSkillList(value);
            });
            searchDefenceCharaInputField.onValueChanged.AddListener(value =>
            {
                // ディフェンスのスキルリストを更新
                RefreshDefenceSkillList(value);
            });
            searchSkillInputField.onValueChanged.AddListener(value =>
            {
                // 虹スキルリストを更新
                RefreshSpecialSkillList(value);
            });
            
        }

        private void Start()
        {
            uiDebugPanel = AppManager.Instance.UIManager.RootCanvas.GetComponentInChildren<UIDebugPanel>();
            stopScreen.SetActive(false);
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 30;
#endif
            cutSceneType.AddOptions(Enum.GetNames(typeof(BattleConst.DigestType)).ToList());
            // データ作成
            CreateSkillInfoFiltered();
            CreateAdviserSkill();
            
            offenceCharacter.AddOptions(offenceSkillInfoList);
            defenceCharacter.AddOptions(defenceSkillInfoList);
            specialSkillCutSceneType.AddOptions(adviserSkillInfoList);

            return base.OnPreOpen(args, token);
        }

        /// <summary>
        /// オフェンスのスキルリストを更新する
        /// </summary>
        private void RefreshOffenceSkillList(string inputFilterText = "")
        {
            offenceSkillInfoList.Clear();
            offenceCharacter.ClearOptions();
            CreateSkillInfoFiltered(CharacterPositionSearchType.Offence, inputFilterText);
            offenceCharacter.AddOptions(offenceSkillInfoList);
        }
        
        /// <summary>
        /// ディフェンスのスキルリストを更新する
        /// </summary>
        public void RefreshDefenceSkillList(string inputFilterText = "")
        {
            defenceSkillInfoList.Clear();
            defenceCharacter.ClearOptions();
            CreateSkillInfoFiltered(CharacterPositionSearchType.Defence, inputFilterText);
            defenceCharacter.AddOptions(defenceSkillInfoList);
        }
        /// <summary>
        /// アドバイザースキルリストを更新する
        /// </summary>
        public void RefreshSpecialSkillList(string inputFilterText = "")
        {
            adviserSkillInfoList.Clear();
            adviserSkills.Clear();
            specialSkillCutSceneType.ClearOptions(); 
            CreateAdviserSkill(inputFilterText);
            specialSkillCutSceneType.AddOptions(adviserSkillInfoList);
        }

        private void CreateAdviserSkill(string inputFilterText = "")
        {
            // スキルのidを取得
            foreach (AbilityMasterObject abilityMaster in abilityMasters)
            {
                // 虹スキル、アドバイザースキル、インプットフィールドでフィルター
                if (abilityMaster.rarity >= HasCutInRarityBorder && abilityMaster.AbilityType == BattleConst.AbilityType.GuildBattleManual && (abilityMaster.name.Contains(searchSkillInputField.text) || abilityMaster.id.ToString().Contains(inputFilterText)))
                {
                    // 名前：idでリストに登録する
                    adviserSkillInfoList.Add($"{abilityMaster.name}:{abilityMaster.id}");
                    adviserSkills.Add(abilityMaster.id);
                }

            }               
        }

        private void CreateSkillInfoFiltered(CharacterPositionSearchType characterPositionType = CharacterPositionSearchType.All, string inputFilterText = "")
        {
            skillDictionary.Clear();
            // マスタ全検索してスキルのidを取得
            foreach (TrainingEventMasterObject trainingEventMaster in trainingEventMasters)
            {
                // キャラがいない場合
                if (trainingEventMaster.trainingMCharaId == 0) continue;
                CharaMasterObject chara = MasterManager.Instance.charaMaster.FindData(trainingEventMaster.trainingMCharaId);
                // m_abilityの取得
                List<SkillData> skillList = SkillUtility.GetSkillList(trainingEventMaster);
                foreach (SkillData skillData in skillList)
                {
                    var mAbility = MasterManager.Instance.abilityMaster.FindData(skillData.Id);
                    // 演出がないのでレア度3未満は除外
                    if (mAbility.rarity < HasCutInRarityBorder) continue;
                    
                    //なんのインプットフィールド入力されたかでリストを変える
                    if (characterPositionType != CharacterPositionSearchType.All)
                    {
                        if (chara.id.ToString().Contains(inputFilterText))
                        {
                            skillDictionary.TryAdd(chara.id, skillData.Id);
                        }
                    }
                    else
                    {
                        skillDictionary.TryAdd(chara.id, skillData.Id);
                    }
                   
                }
            }

            // スキルが取得できないキャラもいるので、キャラマスターで回す(キャラで回せばid順になる)
            foreach (CharaMasterObject masterValue in MasterManager.Instance.charaMaster.values)
            {
                // キャラじゃない場合、idが56,57、親キャラじゃない場合は除外
                // 56,57は主人公?で実装されていない
                 if(masterValue.cardType != CardType.Character || masterValue.id == 56 || masterValue.id == 57) continue;
                // スキルが登録されていないキャラはスキルIdを0でとってくる
                skillDictionary.TryGetValue(masterValue.id, out long skillId);
                switch (characterPositionType)
                {
                    case CharacterPositionSearchType.Offence:
                        if (skillId != 0)
                        {
                            // 名前：id：abilityIdの順で表示する
                            String offenceSkill = $"{masterValue.name}:{masterValue.id}:{skillId}";
                            offenceSkillInfoList.Add(offenceSkill);
                        }
                        break;
                    case CharacterPositionSearchType.Defence:
                        if (skillId != 0)
                        {
                            // 名前：id：abilityIdの順で表示する
                            String defenceSkill = $"{masterValue.name}:{masterValue.id}:{skillId}";
                            defenceSkillInfoList.Add(defenceSkill);
                        }
                        break;
                    case CharacterPositionSearchType.All:
                        // 名前：id：abilityIdの順で表示する
                        String skillInfo = $"{masterValue.name}:{masterValue.id}:{skillId}";
                        // offenceとdefenceの両方に追加
                        offenceSkillInfoList.Add(skillInfo);
                        defenceSkillInfoList.Add(skillInfo);
                        break;
                }

            }
        }

        public void PlayAdviserSkill()
        {
            // アドバイザーのスキルIDを持ってくる
            long skillId = adviserSkills[specialSkillCutSceneType.value];
            
            AbilityMasterObject adviserSkill = MasterManager.Instance.abilityMaster.FindData(skillId);
            if (adviserIDInputField.text == "")
            {
                CruFramework.Logger.LogError("アドバイザーIDが入力されていません。");
                return;
            }
            // アドバイザーカットインの再生
            skillActivationEffect.PlayAnimation(long.Parse(adviserIDInputField.text), skillId, adviserSkill.useMessage).Forget(); 
        }

        public async void Play()
        {
            CruFramework.Logger.Log("type : " + cutSceneType.captionText.text);
            CruFramework.Logger.Log("offence : " + offenceCharacter.captionText.text);
            CruFramework.Logger.Log("defence : " + defenceCharacter.captionText.text);
            var type = Enum.Parse<BattleConst.DigestType>(cutSceneType.captionText.text);
           
            //マッチアップUIの初期化
            BattleUIMediator.Instance.MatchUpUi.DebugClosePhraseUI();
            
            var offenceId = 0;
            int.TryParse(offenceCharacter.captionText.text.Split(":")[1], out offenceId);
            var offenceAblityId = 0;
            int.TryParse(offenceCharacter.captionText.text.Split(":")[2], out offenceAblityId);
            var defenceId = 0;
            int.TryParse(defenceCharacter.captionText.text.Split(":")[1], out defenceId);
            var defenceAblityId = 0;
            int.TryParse(defenceCharacter.captionText.text.Split(":")[2], out defenceAblityId);
            var offenceData = new List<BattleDigestCharacterData>();
            var defenceData = new List<BattleDigestCharacterData>();
            BattleDigestCharacterData data = new BattleDigestCharacterData();
            data.MCharaId = offenceId;
            data.StaminaRatio = 1f;
            data.Side = offenceTurn.isOn ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            data.IsAce = isEightSpeed;
            data.AbilityId = offenceAblityId;
            offenceData.Add(data);
            
            data = new BattleDigestCharacterData();
            data.MCharaId = defenceId;
            data.StaminaRatio = 1f;
            data.Side = defenceTurn.isOn ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right;
            data.AbilityId = defenceAblityId;
            defenceData.Add(data);
            List<int> score = new List<int> { 1, 2 };
            
            debugCanvas.enabled = false;
            uiDebugPanel.Hide();
            stopScreen.SetActive(true);
            if (type == BattleConst.DigestType.Special)
            {
                Invoke(nameof(SpecialSkillVoicePlay), SkillCutInTime);
                activateAbilityUI.DebugOpen(offenceAblityId);
            }
            await BattleDigestController.Instance.DebugPlayAsync(type,
                offenceTurn.isOn ? BattleConst.TeamSide.Left : BattleConst.TeamSide.Right, offenceData[0], defenceData,
                score, 500, middleAction: BattleUIMediator.Instance.MatchUpUi.DebugOpenPhraseUI, abilityId: offenceAblityId);
            headerAndFooterRoot.SetActive(false);
        }
        
        private void SpecialSkillVoicePlay()
        {
            bool canParseOffenceAbilityId = int.TryParse(offenceCharacter.captionText.text.Split(":")[2], out int offenceAbilityId);
            if (!canParseOffenceAbilityId)
            {
                CruFramework.Logger.LogError("オフェンスのスキルIDが正しく取得できませんでした。");
                return;
            }
            CharaLibraryVoiceMasterObject charaLibraryVoice = MasterManager.Instance.charaLibraryVoiceMaster.GetDataByAbilityId(offenceAbilityId);
            if (charaLibraryVoice != null)
            {
                VoiceManager.Instance.PlayVoiceForCharaLibraryVoiceAsync(charaLibraryVoice).Forget();
            }
        }
        
        public void OnClickNormalSpeed()
        {
            BattleDataMediator.Instance.IsDoubleSpeed = true;
            normalSpeedButton.gameObject.SetActive(false);
            doubleSpeedButton.gameObject.SetActive(true);
        }
        
        public void OnClickDoubleSpeed()
        {
            BattleDataMediator.Instance.IsDoubleSpeed = false;
            normalSpeedButton.gameObject.SetActive(true);
            doubleSpeedButton.gameObject.SetActive(false);
        }

        public void OnClickEightSpeed()
        {
            isEightSpeed = !isEightSpeed;
            nonSpeedButton.gameObject.SetActive(!isEightSpeed);
            eightSpeedButton.gameObject.SetActive(isEightSpeed);
        }

        public void BackToTitle()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            AppManager.Instance.BackToTitle();
        }

        public void OnClickStop()
        {
            BattleDigestController.Instance.ForceQuitCurrentDigest();
            debugCanvas.enabled = true;
            uiDebugPanel.Show();
            stopScreen.SetActive(false);
        }
    }
}
#endif