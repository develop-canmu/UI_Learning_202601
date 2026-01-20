using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Deck;
using Pjfb.InGame;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.Voice;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{    
    public class TrainingPracticeGamePreparationPage : TrainingPageBase
    {

        private static readonly string ChangeButtonKeyMyself = "common.self_team";
        private static readonly string ChangeButtonKeyOpponent = "common.enemy_team";
        
        private enum StatusSideType
        {
            Myself, Opponent
        }
        
        [SerializeField]
        private CharacterVariableIcon[] characterIcons = null;

        [SerializeField]
        private TMP_Text changeButtonText = null;

        [SerializeField]
        private GameObject[] playerObjects = null;
        
        [SerializeField]
        private GameObject[] enemyObjects = null;
        
        [SerializeField]
        private TrainingCharacterStatusResult statusResult = null;
        
        [SerializeField]
        private CharacterSpine character = null;
        
        [SerializeField]
        private DeckStrategyView strategyView = null;
        
        [SerializeField]
        private GameObject strategyChoice = null;
        
        [SerializeField]
        private UIToggle skipMatchToggle = null;
        
        // 表示しているステータス
        private StatusSideType statusSideType = StatusSideType.Myself;
    
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            
            // ヘッダーを非表示に
            Header.Hide();
            // フッターを非表示に
            Footer.Hide();
            // ステータス表示更新
            UpdateStatusView();
            
            strategyView.SetStrategy( (BattleConst.DeckStrategy)MainArguments.BattlePending.clientData.playerList[0].optionValue );
            //試合スキップ設定
            skipMatchToggle.isOn = LocalSaveManager.saveData.skipMatchData;
            
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoice();
#endif
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugAutoChoice()
        {
            if (TrainingChoiceDebugMenu.EnabledAutoChoiceGame)
            {
                StartAsync().Forget();
            }
        }
#endif
        
        protected override UniTask<bool> OnPreLeave(CancellationToken token)
        {
            // 試合スキップ設定を保存
            LocalSaveManager.saveData.skipMatchData = skipMatchToggle.isOn;
            // セーブデータ保存
            LocalSaveManager.Instance.SaveData();
            return base.OnPreLeave(token);
        }
        
        private void ActivePlayerObjects(bool active)
        {
            foreach(GameObject obj in playerObjects)
            {
                obj.SetActive(active);
            }
            
            foreach(GameObject obj in enemyObjects)
            {
                obj.SetActive(!active);
            }
        }
        
        private void UpdateStatusView()
        {
            List<CharacterVariableDetailData> detailOrderList = new();
            int detailOrderIndex = 0;
            switch(statusSideType)
            {
                case StatusSideType.Myself:
                {
                    ActivePlayerObjects(true);
                    
                    // 味方のId
                    List<BattleV2Chara> characterList = TrainingUtility.GetBattleCharacterList(MainArguments.BattlePending.clientData, TrainingUtility.PlayerType.Player);
                    
                    // リーダーのId
                    long mCharId = characterList[0].mCharaId;
                    // ステータス表示
                    statusResult.SetStatus(mCharId, MainArguments.Status, MainArguments.CharacterVariable.abilityList);
                    // 変更ボタン名
                    changeButtonText.text = StringValueAssetLoader.Instance[ChangeButtonKeyOpponent];
                    // 変更ボタン
                    //positionChangeButton.gameObject.SetActive(true);
                    
                    // リーダーのテータス表示
                    statusResult.SetNpcStatus(characterList[0]);
                    // 立ち絵
                    character.SetSkeletonDataAsset( CharacterUtility.CharIdToStandingImageId(mCharId) );
                    
                    // メンバー表示
                    for(int i=0;i<characterList.Count;i++)
                    {
                        BattleV2Chara c = characterList[i];
                        CharacterVariableIcon icon = characterIcons[i];
                        CharacterVariableDetailData data = new CharacterVariableDetailData(c);
                        icon.SetIcon(data);
                        icon.SetIconTextureWithEffectAsync(c.mCharaId).Forget();
                        icon.SwipeableParams = new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex++);
                        icon.CharType = CharacterVariableIcon.CharacterType.TrainingCharacter;
                        detailOrderList.Add(data);
                    }

                    // 戦略設定UIを表示
                    strategyChoice.SetActive(true);
                    
                    break;
                }
                
                case StatusSideType.Opponent:
                {
                    ActivePlayerObjects(false);
                    
                    // 敵のId
                    List<BattleV2Chara> characterList = TrainingUtility.GetBattleCharacterList(MainArguments.BattlePending.clientData, TrainingUtility.PlayerType.Npc);
                    // リーダーのテータス表示
                    statusResult.SetNpcStatus(characterList[0]);
                    // 立ち絵
                    character.SetSkeletonDataAsset(characterList[0].visualKey);
                    // 変更ボタン名
                    changeButtonText.text = StringValueAssetLoader.Instance[ChangeButtonKeyMyself];
                    
                    // メンバー表示
                    for(int i=0;i<characterList.Count;i++)
                    {
                        BattleV2Chara c = characterList[i];
                        CharacterVariableIcon icon = characterIcons[i];
                        CharacterVariableDetailData data = new CharacterVariableDetailData(c);
                        icon.SetIcon(data);
                        icon.SetIconTextureWithEffectAsync(c.mCharaId).Forget();
                        icon.SwipeableParams = new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex++);
                        
                        icon.CharType = CharacterVariableIcon.CharacterType.OpponentCharacter;
                        detailOrderList.Add(data);
                    }

                    // 戦略設定UIを表示
                    strategyChoice.SetActive(false);
                    
                    break;
                }
            }
        }
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnChangeButton()
        {
            switch(statusSideType)
            {
                case StatusSideType.Myself:
                    statusSideType = StatusSideType.Opponent;
                    break;
                case StatusSideType.Opponent:
                    statusSideType = StatusSideType.Myself;
                    break;
            }
            
            // 表示更新
            UpdateStatusView();
        }

        public void OnClickSelectStrategyButton()
        {
            StrategyChoiceModalWindow.Open(new StrategyChoiceModalWindow.WindowParams
            {
                strategy = (BattleConst.DeckStrategy)MainArguments.BattlePending.clientData.playerList[0].optionValue,
                onClosed = null,
                onStrategyChanged = (strategy) =>
                {
                    MainArguments.BattlePending.clientData.playerList[0].optionValue = (int)strategy;
                    strategyView.SetStrategy(strategy);
                }
            });
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnStartButton()
        {
            StartAsync().Forget();
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnPositionChangeButton()
        {
            PositionChangeModalWindow.WindowParams p = new PositionChangeModalWindow.WindowParams();
            BattleV2Chara c = MainArguments.GetTrainingBattleCharacter();
            p.CurrentRole = (RoleNumber)c.roleNumber;
            p.onChanged += (v)=>
            {
                c.roleNumber = (int)v;
                SavePositionAsync().Forget();
                
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PositionChange, p);
        }
        
        private async UniTask SavePositionAsync()
        {
            // Request
            TrainingOverwriteRoleAPIRequest request = new TrainingOverwriteRoleAPIRequest();
            // Post
            TrainingOverwriteRoleAPIPost post = new TrainingOverwriteRoleAPIPost();
            post.idRoleList = TrainingUtility.CreateBattleRoleList(MainArguments.BattlePending.clientData);
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
        }
        
        protected virtual async UniTask StartAsync()
        {
            
            // ログを保存
            TrainingUtility.SaveLog(Adv);
            
            BattleV2ClientData clientData = MainArguments.BattlePending.clientData;
            
            // バトル開始APIを叩く
            if(MainArguments.IsBattleStarted == false)
            {
                // BattleStart
                TrainingBattleStartAPIResponse response = await TrainingUtility.BattleStartAPI(MainArguments.BattlePending.clientData);
                // データを更新
                clientData = response.battlePending.clientData;
            }
            
            // ボイス
            await PlayTrainingCharacterInVoiceAsync(VoiceResourceSettings.LocationType.IN_YELL);
            
            // インゲーム
            NewInGameOpenArgs inGameArgs = new NewInGameOpenArgs(PageType.Training, clientData, null);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false, inGameArgs);

        }

        public void OnClickExitConditions()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ExitConditions, null);
        }
        
        public void OnHelpButton()
        {
            TrainingUtility.OpenHelpModal();
        }
    }
}
