using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

using CruFramework;
using CruFramework.UI;
using Pjfb.InGame;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using UnityEditor;
using UnityEngine.Rendering;

using Pjfb.Master;
using Unity.VisualScripting.Dependencies.NCalc;

namespace Pjfb.Training
{
    /// <summary>トレーニング準備画面のサブページ</summary>
    public enum TrainingPreparationPageType
    {
        /// <summary>メニュー選択</summary>
        MenuSelect,
        /// <summary>キャラクタ選択</summary>
        CharacterSelect,
        /// <summary>サポートデッキ選択</summary>
        SupportCharacterDeckSelect,
        /// <summary>サポート器具デッキ選択</summary>
        SupportEquipmentDeckSelect,
        /// <summary>サポートキャラ選択</summary>
        SupportCharacterSelect,
        /// <summary>サポート器具選択</summary>
        SupportEquipmentSelect,
        /// <summary>中断データ確認</summary>
        TrainingConfirm
    }
    
    public enum TrainingMode
    {
        Default, Auto
    }
    
    public enum AutoTrainingModalType
    {
        Start, Abort, Replay
    }
    
    public class TrainingPreparationArgs
    {
        private long trainingUCharId = 0;
        /// <summary>育成対象</summary>
        public long TrainingUCharId{get{return trainingUCharId;}set{trainingUCharId = value;}}
        
        private long trainingScenarioId = 0;
        /// <summary>シナリオId</summary>
        public long TrainingScenarioId{get{return trainingScenarioId;}set{trainingScenarioId = value;}}
        
        private TrainingMode mode = TrainingMode.Default;
        /// <summary>トレーニングの種類</summary>
        public TrainingMode Mode{get{return mode;}set{mode = value;}}
        
        private AutoTrainingModalType autoTrainingModalType = AutoTrainingModalType.Start;
        /// <summary>自動トレーニングのモーダルの種類</summary>
        public AutoTrainingModalType AutoTrainingModalType{get{return autoTrainingModalType;}set{autoTrainingModalType = value;}}
        
        private long autoTrainingPolicy = 0;
        /// <summary>育成方針</summary>
        public long AutoTrainingPolity{get{return autoTrainingPolicy;}set{autoTrainingPolicy = value;}}
        
        private TrainingSupportCharacterDeckView.SelectData supportCharacterDeckSelectedData;
        /// <summary>デッキ番号</summary>
        public TrainingSupportCharacterDeckView.SelectData SupportCharacterDeckSelectedData{get{return supportCharacterDeckSelectedData;}set{supportCharacterDeckSelectedData = value;}}
        
        private long partyNumber = 0;
        /// <summary>パーティ番号</summary>
        public long PartyNumber{get{return partyNumber;}set{partyNumber = value;}}
        
        private long equipmentPartyNumber = 0;
        /// <summary>パーティ番号</summary>
        public long EquipmentPartyNumber{get{return equipmentPartyNumber;}set{equipmentPartyNumber = value;}}
        
        private long supportCharacterId = TrainingSupportDeckSelectPage.EmptyId;
        /// <summary>サポートキャラ選択用 </summary>
        public long SupportCharacterId{get{return supportCharacterId;}set{supportCharacterId = value;}}
        
        private long selectedSupportCharacterId = TrainingSupportDeckSelectPage.None;
        /// <summary>サポートキャラ選択用 </summary>
        public long SelectedSupportCharacterId{get{return selectedSupportCharacterId;}set{selectedSupportCharacterId = value;}}
        
        private TrainingDeckMemberType characterMemberType = TrainingDeckMemberType.Support;
        /// <summary>サポートキャラ選択用 </summary>
        public TrainingDeckMemberType CharacterMemberType{get{return characterMemberType;}set{characterMemberType = value;}}
        
        private CharaV2FriendLend[] friendList = null;
        /// <summary>フレンドリスト</summary>
        public CharaV2FriendLend[] FriendList{get{return friendList;}set{friendList = value;}}
        
        private DeckListData deckList = null;
        /// <summary>デッキリスト</summary>
        public DeckListData DeckList{get{return deckList;}set{deckList = value;}}
        
        private DeckListData equipmentDeckList = null;
        /// <summary>デッキリスト</summary>
        public DeckListData EquipmentDeckList{get{return equipmentDeckList;}set{equipmentDeckList = value;}}
        
        private bool isUpdateFriendList = true;
        /// <summary>フレンドリストの更新</summary>
        public bool IsUpdateFriendList{get{return isUpdateFriendList;}set{isUpdateFriendList = value;}}
        
        /// <summary>Exサポート</summary>
        public bool IsExtraCharacter{get{return supportCharacterDeckSelectedData.IsExtraCharacter;}}
        
        private long autoTrainingSlot = 0;
        /// <summary>自動トレーニングで選択したスロット</summary>
        public long AutoTrainingSlot{get{return autoTrainingSlot;}set{autoTrainingSlot = value;}}
        
        private bool isAutoTrainingOnly = false;
        /// <summary>自動トレーニングのみ</summary>
        public bool IsAutoTrainingOnly{get{return isAutoTrainingOnly;}set{isAutoTrainingOnly = value;}}
        
        /// <summary>フレンド選択</summary>
        private bool isFriendSlotSelectMyChara = false;
        public bool IsFriendSlotSelectMyChara{get{return isFriendSlotSelectMyChara;}}
        
        public List<TrainingSupport> GetAllCharacters()
        {
            List<TrainingSupport> result = new List<TrainingSupport>();
            
            result.Add( GetTrainingCharacter() );
            result.AddRange( GetSupportList() );
            return result;
        }
        
        public TrainingSupport GetTrainingCharacter()
        {
            // 表示用データ
            TrainingSupport s = new TrainingSupport();
            
            // uChar
            UserDataChara uChar = UserDataManager.Instance.chara.Find(trainingUCharId);
            // mChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uChar.charaId);
            
            // 表示用データ
            s.mCharaId = uChar.charaId;
            s.level = uChar.level;
            s.newLiberationLevel = uChar.newLiberationLevel;
            // サポートタイプ
            s.supportType = (long)TrainingUtility.SupportCharacterType.TrainingChar;
            
            return s;
        }
        
        public List<TrainingSupport> GetSupportList()
        {
            
            // トレーニングキャラの設定
            DeckData characterDeck = DeckList.GetDeck(PartyNumber);
            DeckData equipmentDeck = EquipmentDeckList.GetDeck(EquipmentPartyNumber);
            
            long[] supportCharacterIds = characterDeck.GetMemberIds();
            long[] supportEquipmentIds = equipmentDeck.GetMemberIds();
           
            UserDataChara trainingUChar = UserDataManager.Instance.chara.Find(TrainingUCharId);
            
            // 表示する内容
            List<TrainingSupport> supportList = new List<TrainingSupport>();
            // サポート
            for(int i=0;i<supportCharacterIds.Length;i++)
            {
                long id = supportCharacterIds[i];
                // Empty
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                
                // 表示用データ
                TrainingSupport s = new TrainingSupport();
                
                // uChar
                UserDataChara uChar = UserDataManager.Instance.chara.Find(id);
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uChar.charaId);
                // 表示用データ
                s.mCharaId = uChar.charaId;
                s.level = uChar.level;
                s.newLiberationLevel = uChar.newLiberationLevel;
                // サポートタイプ
                s.supportType = 
                    // サーバー側としてのアドバイザーの扱いはサポカと同一なので合わせる
                    mChar.cardType == CardType.SpecialSupportCharacter || mChar.cardType == CardType.Adviser 
                    ? (int)TrainingUtility.SupportCharacterType.Special 
                    : (int)TrainingUtility.SupportCharacterType.Normal;
                // カードタイプ
                s.cardType = (long)uChar.CardType;
                supportList.Add(s);
            }
            
            // サポート器具
            for(int i=0;i<supportEquipmentIds.Length;i++)
            {
                long id = supportEquipmentIds[i];
                // Empty
                if(id == DeckUtility.EmptyDeckSlotId)continue;
                
                // 表示用データ
                TrainingSupport s = new TrainingSupport();
                
                // 未開放なので表示しない
                if(trainingUChar.level < equipmentDeck.GetUnlockLevel(i))continue;
                // uEquipment
                UserDataSupportEquipment uEquipment = UserDataManager.Instance.supportEquipment.Find(id);
                // mChar
                CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(uEquipment.charaId);
                // 表示用データ
                s.mCharaId = uEquipment.charaId;
                s.level = uEquipment.level;
                s.newLiberationLevel = 0;
                s.trainerId = uEquipment.id;
                s.statusIdList = uEquipment.lotteryProcessJson.statusList;
                
                s.supportType = (int)TrainingUtility.SupportCharacterType.Equipment;

                supportList.Add(s);
            }
            
            // フレンド
            {
                TrainingSupport s = new TrainingSupport();
                s.mCharaId = characterDeck.Friend.mCharaId;
                s.level = characterDeck.Friend.level;
                s.newLiberationLevel = characterDeck.Friend.newLiberationLevel;
                s.supportType = (int)TrainingUtility.SupportCharacterType.Friend;
                supportList.Add(s);
            }
            
            return supportList;
        }
        
        public void ChangeFriendSelectedUserChara(bool isUserChara)
        {
            isFriendSlotSelectMyChara = isUserChara;
        }
    }
    
    /// <summary>
    /// トレーニング準備画面
    /// </summary>
    
    public class TrainingPreparation : PageManager<TrainingPreparationPageType>
    {
        
        public class Arguments
        {
            private int trainingId = -1;
            /// <summary>最初に開くトレーニングId</summary>
            public int TrainingId{get{return trainingId;}}
            
            private bool gotoAutoTraining = false;
            /// <summary>自動トレーニングへ</summary>
            public bool GotoAutoTraining{get{return gotoAutoTraining;}}
            
            private bool isAutoTrainingOnly = false;
            /// <summary>自動トレーニングの見</summary>
            public bool IsAutoTrainingOnly{get{return isAutoTrainingOnly;}}
            
            public Arguments(int trainingId)
            {
                this.trainingId = trainingId;
            }
            
            public Arguments(int trainingId, bool gotoAutoTraining, bool autoTrainingOnly)
            {
                this.trainingId = trainingId;
                this.gotoAutoTraining = gotoAutoTraining;
                isAutoTrainingOnly = autoTrainingOnly;
            }
        }
        
        [SerializeField]
        protected StaminaView staminaView = null;
        /// <summary>スタミナ</summary>
        public StaminaView StaminaView{get{return staminaView;}}
        
        // 中断データ
        private TrainingMainArguments abortData = null;
        
        private TrainingGetAutoStatusAPIResponse autoTrainingStatus = null;
        /// <summary>自動トレーニングデータ</summary>
        public TrainingGetAutoStatusAPIResponse AutoTrainingStatus{get{return autoTrainingStatus;}}
        
        private DeckData selectedDeckData = null;
        
        // ターン延長の最大延長数
        private long maxAddTurn = 0;

        protected override string GetAddress(TrainingPreparationPageType page)
        {
            return $"Prefabs/UI/Page/TrainingPreparation/{page}Page.prefab";
        }
        
        /// <summary>ページ表示準備</summary>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if(TransitionType == PageTransitionType.Back)
            {
                if(CurrentPageObject is TrainingPreparationPageBase p)
                {
                    p.OnBackPage();
                }
                return;
            }
            
            Arguments arguments = args as Arguments;
            
            // コンフィグを読み込む
            await TrainingUtility.LoadConfig();
            // 変数初期化
            TrainingUtility.CanExit = false;
            // スタミナ表示
            await staminaView.UpdateAsync(StaminaUtility.StaminaType.Training);
            
            // フレンドのエラー
            PjfbAPIErrorHandler handler = (PjfbAPIErrorHandler)APIManager.Instance.errorHandler;
            handler.SetUniqueErrorHandleFunc(TrainingUtility.API_ERROR_CODE_DATA_ERROR, (request, errorCode, errorMessage)=>
            {
                return default;
            });
            
            
            // 自動トレーニングの情報取得
            await ReloadAutoTraining();
            
            // 自動トレーニングへ
            if(arguments != null && arguments.GotoAutoTraining)
            {
                abortData = null;
                await GotoMenuSelect(arguments.IsAutoTrainingOnly);
                return;
            }
            
            try
            {
                // 汎用解放演出中の場合、サポカ・サポート器具の機能をアンロックし機能誘導を終了する
                await MarkReadSupportCardAndEquipment();
                
                // トレーニングの途中データがあるかチェック
                TrainingConfirmAPIRequest confirmRequest = new TrainingConfirmAPIRequest();
                await APIManager.Instance.Connect(confirmRequest);
                TrainingConfirmAPIResponse confirmResponse = confirmRequest.GetResponseData();
                // 途中データあり
                if(confirmResponse.code == TrainingUtility.ResponseSuccess)
                {
                    // ターン延長の最大数格納
                    maxAddTurn = confirmResponse.maxAddTurnValue;
                    abortData = new TrainingMainArguments(confirmResponse.trainingEvent, confirmResponse.pending, confirmResponse.battlePending, confirmResponse.charaVariable, null, confirmResponse.pointStatus, new TrainingMainArgumentsKeeps());
                    await OpenPageAsync(TrainingPreparationPageType.TrainingConfirm, true, null);
                }
                // 準備画面へ
                else
                {
                    // 初期化
                    abortData = null;
                    // 準備画面へ
                    await GotoMenuSelect();
                }
            }
            catch(Exception e)
            {
                if(e is APIException apiException && apiException.errorParam == TrainingUtility.API_ERROR_CODE_DATA_ERROR)
                {
                    
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["training.error.button"],
                        async (m)=>
                        {
                            // 中断データを破棄
                            TrainingAbortAPIRequest abortRequest = new TrainingAbortAPIRequest();
                            await APIManager.Instance.Connect(abortRequest);
                            // 閉じる
                            await m.CloseAsync();
                        }
                    );
                    
                    ConfirmModalData modalData = new ConfirmModalData(
                        StringValueAssetLoader.Instance["training.error.title"],
                        StringValueAssetLoader.Instance["training.error.message"],
                        string.Empty,
                        button
                    );
                    
                    // モーダルを開く
                    CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, modalData);
                    // 閉じるまで待つ
                    await modal.WaitCloseAsync();
                    
                    button = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.close"],
                        async (m)=>
                        {
                            abortData = null;
                            // トレーニング準備画面を開く
                            await GotoMenuSelect();
                            // 閉じる
                            await m.CloseAsync();
                        }
                    );
                    
                    modalData = new ConfirmModalData(
                        StringValueAssetLoader.Instance["training.error.repair_title"],
                        StringValueAssetLoader.Instance["training.error.repair_message"],
                        string.Empty,
                        button
                    );
                    
                    // モーダルを開く
                    modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, modalData);
                    // 閉じるまで待つ
                    await modal.WaitCloseAsync();
                }
                else
                {
                    throw e;
                }
            }
        }

        protected override void OnOpened(object args)
        {
            if(TransitionType == PageTransitionType.Back)return;
        
            if(abortData != null)
            {
                AppManager.Instance.UIManager.ModalManager.OpenModal(
                    ModalType.PendingConfirm,
                    new PendingConfirmModal.Arguments(
                        StringValueAssetLoader.Instance["common.pending_title"],
                        StringValueAssetLoader.Instance["training.abort.comfirm_message"],
                        // 続きから
                        async ()=>
                        {
                            // 新トレーニング解放の汎用解放演出を既読
                            MarkReadTrainingAdv();
                            
                            // ターン延長の最大延長数設定
                            TrainingUtility.SetMaxAddTurn(maxAddTurn);
                            
                            // インゲームから
                            if(abortData.IsBattleStarted)
                            {
                                // パフォーマンス進捗をキャッシュ
                                TrainingUtility.PerformanceProgressCache = abortData.Pending.overallProgress;
                                // インゲーム
                                NewInGameOpenArgs inGameArgs = new NewInGameOpenArgs(PageType.Training, abortData.BattlePending.clientData, null);
                                await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.NewInGame, false, inGameArgs);
                            }
                            else
                            {
                                // トレーニング画面を開く
                                await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Training, true, abortData);
                            }
                        },
                        // データ削除
                        async ()=>
                        {
                            // 中断データを破棄
                            TrainingAbortAPIRequest abortRequest = new TrainingAbortAPIRequest();
                            await APIManager.Instance.Connect(abortRequest);
                            // ログの削除
                            TrainingUtility.DeleteLog();
                            // トレーニング準備画面を開く
                            await GotoMenuSelect();
                        },
                        // 自動トレーニング
                        UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining) == false ? null : async ()=>
                        {
                            abortData = null;
                            // トレーニング準備画面を開く
                            await GotoMenuSelect(true);
                        }
                    ));
            }
            base.OnOpened(args);
        }
        
        private async UniTask GotoMenuSelect(bool gotoAutoTraining = false)
        {
            TrainingPreparationArgs a = new TrainingPreparationArgs();
            // 自動トレーニングを開く
            a.IsAutoTrainingOnly = gotoAutoTraining;
            // シナリオ指定
            if(OpenArguments is Arguments arguments)
            {
                a.TrainingScenarioId = arguments.TrainingId;
            }
            // トレーニング準備画面を開く
            await OpenPageAsync(TrainingPreparationPageType.MenuSelect, true, a);
        }
        
        /// <summary>自動トレーニングの再読み込み</summary>
        public async UniTask<TrainingGetAutoStatusAPIResponse> ReloadAutoTraining()
        {
            // Post
            TrainingGetAutoStatusAPIPost getAutoTrainingStatusPost = new TrainingGetAutoStatusAPIPost();
            // Req
            TrainingGetAutoStatusAPIRequest getAutoTrainingStatusReq = new TrainingGetAutoStatusAPIRequest();
            getAutoTrainingStatusReq.SetPostData(getAutoTrainingStatusPost);
            // API
            await APIManager.Instance.Connect(getAutoTrainingStatusReq);
            autoTrainingStatus = getAutoTrainingStatusReq.GetResponseData();
            // 完了時間の更新
            AutoTrainingUtility.TimeMinute = autoTrainingStatus.trainingAutoRequireMinute;
            return autoTrainingStatus;
        }

        protected override void OnClosed()
        {
            
            base.OnClosed();
        }
        
        public async UniTask<bool> CheckDeckChange(DeckData deckData)
        {
            // デッキが変更されている場合
            if(deckData.IsDeckChanged)
            {
                // デッキが完成している場合は保存
                if(deckData.IsEnableDeck(false))
                {
                    await deckData.SaveDeckAsync(false);
                }
                // 未完成の場合は確認モーダルをだす
                else
                {
                    
                    bool isCancel = false;
                    
                    ConfirmModalButtonParams button1 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=> 
                        {
                            m.Close();
                        });
                    
                    ConfirmModalButtonParams button2 = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (m)=>
                        {
                            isCancel = true;
                            m.Close();
                        });
                    
                    ConfirmModalData modalData = new ConfirmModalData(
                        StringValueAssetLoader.Instance["character.deckedit.leave_deck_confirm_title"],
                        StringValueAssetLoader.Instance["character.deckedit.leave_deck_confirm_content"],
                        string.Empty,
                        button1, button2
                    );
                    
                    CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, modalData);
                    await modal.WaitCloseAsync();
                    
                    // 選んでいたデッキ編集に戻る
                    if(isCancel)
                    {
                        return true;
                    }

                    // デッキ編集を破棄
                    deckData.DiscardChanges();
                }
            }
            
            return false;
        }
        
        public void SetDeckIndex(DeckData deckData)
        {
            selectedDeckData = deckData;
        }
        

        /// <summary>閉じる前の処理</summary>
        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            if(selectedDeckData != null)
            {
                bool result = await CheckDeckChange(selectedDeckData);
                if(result == true)return false;
            }
        
            TrainingDeckUtility.SetCurrentTrainingDeck(null);
            return await base.OnPreClose(token);
        }

        private void MarkReadTrainingAdv()
        {
            if (SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AnyTraining))
            {
                SystemUnlockDataManager.Instance.RequestReadUnlockEffectAsync().Forget();
            }
        }

        private async UniTask MarkReadSupportCardAndEquipment()
        {
            if (SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.SupportEquipment) ||
                SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.SpecialSupportCard1) ||
                SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.ExSupport))
            {
                await SystemUnlockDataManager.Instance.RequestReadUnlockEffectAsync();
            }
        }
    }
}
