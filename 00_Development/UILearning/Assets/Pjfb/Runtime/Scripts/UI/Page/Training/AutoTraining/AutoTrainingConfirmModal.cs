using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using Pjfb.Voice;
using TMPro;

namespace Pjfb.Training
{
    
    public class AutoTrainingConfirmModal : ModalWindow
    {
        
        public enum CloseParamType
        {
            None,
            /// <summary>リプレイの実行</summary>
            ExecuteReplay,
            /// <summary>トレーニング中止</summary>
            ExecuteAbort,
        }
        
        public class Arguments
        {
            private long trainingScenarioId = 0;
            /// <summary>シナリオId</summary>
            public long TrainingScenarioId{get{return trainingScenarioId;}}
            
            private AutoTrainingModalType modalType = AutoTrainingModalType.Start;
            /// <summary>モーダルタイプ</summary>
            public AutoTrainingModalType ModalType{get{return modalType;}}
            
            private TrainingSupport[] supportCharacters = null;
            /// <summary>サポートキャラの情報</summary>
            public TrainingSupport[] SupportCharacters{get{return supportCharacters;}}
            
            private TrainingPreparationArgs preparationArgs = null;
            /// <summary>準備画面引数</summary>
            public TrainingPreparationArgs PreparationArgs{get{return preparationArgs;}}
            
            private long statusType = 0;
            /// <summary>育成方針</summary>
            public long StatusType{get{return statusType;}}
            
            private long slotNumber = 0;
            /// <summary>スロット番号</summary>
            public long SlotNumber{get{return slotNumber;}}
            
            private TimeSpan finishAt;
            /// <summary>トレーニング完了時間</summary>
            public TimeSpan FinishAt{get{return finishAt;}}
            
            private long freeCompleteCount = 0;
            /// <summary>無料即完了回数</summary>
            public long FreeCompleteCount{get{return freeCompleteCount;}}
            
            private TrainingAutoUserStatus autoTrainingUserStatus = null;
            /// <summary>自動トレーニング情報</summary>
            public TrainingAutoUserStatus AutoTrainingUserStatus{get{return autoTrainingUserStatus;}}
            
            private TrainingAutoPendingStatus autoTrainingPendingStatus = null;
            /// <summary>自動トレーニング情報</summary>
            public TrainingAutoPendingStatus AutoTrainingPendingStatus{get{return autoTrainingPendingStatus;}}
            
            private IAutoTrainingReloadable reloadable = null;
            /// <summary>リロードインターフェイス</summary>
            public IAutoTrainingReloadable Reloadable{get{return reloadable;}}
            
            private bool isEnablePass = false;
            /// <summary>パスが有効</summary>
            public bool IsEnablePass{get{return isEnablePass;}}
            
            public Arguments(TrainingPreparationArgs preparationArgs, TrainingAutoPendingStatus pendingStatus, TrainingAutoUserStatus status, AutoTrainingModalType modalType)
            {
                this.preparationArgs = preparationArgs;
                this.modalType = modalType;
                // シナリオId
                trainingScenarioId = preparationArgs.TrainingScenarioId;
                // 育成方針
                statusType = preparationArgs.AutoTrainingPolity;
                // サポートキャラ
                supportCharacters = preparationArgs.GetAllCharacters().ToArray();
                // 完了時間
                finishAt = TimeSpan.FromMinutes(AutoTrainingUtility.TimeMinute);
                // パス
                isEnablePass = AutoTrainingUtility.HasTimeSavePass();
                // 即完はなし
                freeCompleteCount = 0;
                // 自動トレーニング情報
                autoTrainingUserStatus = status;
                autoTrainingPendingStatus = pendingStatus;
            }
            
            public Arguments(TrainingAutoPendingStatus pendingStatus, TrainingAutoUserStatus status, IAutoTrainingReloadable reloadable, AutoTrainingModalType modalType)
            {
                this.modalType = modalType;
                // シナリオId
                trainingScenarioId = pendingStatus.mTrainingScenarioId;
                // サポートキャラ
                supportCharacters = pendingStatus.supportDetailList;
                // 育成方針
                statusType = pendingStatus.statusType;
                // トレーニング完了時間
                finishAt = modalType == AutoTrainingModalType.Abort ? TimeSpan.FromTicks( AppTime.Parse(pendingStatus.finishAt).Ticks ) : TimeSpan.FromMinutes(AutoTrainingUtility.TimeMinute);
                // 無料回数
                freeCompleteCount = status.freeCompleteRemainCount;
                // スロット番号
                slotNumber = pendingStatus.slotNumber;
                // リロード
                this.reloadable = reloadable;
                // 自動トレーニング情報
                autoTrainingUserStatus = status;
                autoTrainingPendingStatus = pendingStatus;
                // パス
                switch(modalType)
                {
                    case AutoTrainingModalType.Abort:
                    {
                        isEnablePass = pendingStatus.isShorten;
                        break;
                    }
                    default:
                    {
                        // パス
                        isEnablePass = AutoTrainingUtility.HasTimeSavePass();
                        break;
                    }
                }
                
            }
        }
        
        [SerializeField]
        private TrainingMemberView memberView = null;
        
        [SerializeField]
        private CancellableRawImage scenerioImage = null;
        
        [SerializeField]
        private AutoTrainingStatusTypeView statusTypeView = null;

        [SerializeField]
        private UIButton startAutoTrainingButton = null;
        [SerializeField]
        private UIButton abortTrainingButton = null;
        
        [SerializeField]
        private GameObject startConfirmRoot = null;
        [SerializeField]
        private GameObject abortConfirmRoot = null;
        
        
        [SerializeField]
        private TMP_Text startTimeText = null;
        [SerializeField]
        private TMP_Text abortTimeText = null;

        [SerializeField]
        private GameObject freeCompleteCountRoot = null;
        [SerializeField]
        private TMP_Text freeCompleteCountText = null;
        
        [SerializeField]
        private PossessionItemUi staminaValue = null;
        
        [SerializeField]
        private TMP_Text titleText = null;
        [SerializeField]
        private TMP_Text messageText = null;

        [SerializeField]
        private TMP_Text closeButtonText = null;
        
        [SerializeField]
        private GameObject[] enablePassObjects = null;
        [SerializeField]
        private GameObject[] disablePassObjects = null;
        
        private float updateTimer = 0;
        
        /// <summary>トレーニングが完了した</summary>
        private bool isCompleted = false;
        // モーダルを開いている
        private bool isOpendModal = false;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            
            Arguments arguments = (Arguments)args;
            
            // シナリオ画像
            scenerioImage.SetTexture( ResourcePathManager.GetPath("TrainingMenuSmallBanner", arguments.TrainingScenarioId) );
            
            // ステータスタイプ
            statusTypeView.SetType(arguments.StatusType);
            
            // スタミナ
            UpdateStaminaView();
            
            // パスを所持しているか
            // パス関連
            foreach(GameObject obj in enablePassObjects)
            {
                obj.SetActive( arguments.IsEnablePass );
            }
            foreach(GameObject obj in disablePassObjects)
            {
                obj.SetActive( arguments.IsEnablePass == false );
            }
            
            // トレーニング対象キャラ
            TrainingSupport trainingChar = null;
            // トレーニングキャラを探す
            foreach(TrainingSupport support in arguments.SupportCharacters)
            {
                TrainingUtility.SupportCharacterType type = (TrainingUtility.SupportCharacterType)support.supportType;
                if(type == TrainingUtility.SupportCharacterType.TrainingChar)
                {
                    trainingChar = support;
                    break;
                }
            }
            
            // メンバービューの表示
            memberView.SetTrainingCharacter( trainingChar.mCharaId, trainingChar.level, trainingChar.newLiberationLevel, arguments.TrainingScenarioId, arguments.SupportCharacters);
            memberView.GetTrainingScenarioId = () => arguments.TrainingScenarioId;

            // 一旦全て非表示
            abortTrainingButton.gameObject.SetActive(false);
            abortConfirmRoot.SetActive(false);
            startAutoTrainingButton.gameObject.SetActive(false);
            startConfirmRoot.SetActive(false);
            
            
            switch(arguments.ModalType)
            {
                case AutoTrainingModalType.Start:
                {
                    startAutoTrainingButton.gameObject.SetActive(true);
                    startConfirmRoot.SetActive(true);
                    
                    // タイトル
                    titleText.text =  StringValueAssetLoader.Instance["auto_training.confirm_modal.start.title"];
                    // メッセージ
                    messageText.text = StringValueAssetLoader.Instance["auto_training.confirm_modal.msg.start"];
                    // 閉じるボタン
                    closeButtonText.text = StringValueAssetLoader.Instance["common.cancel"];
                    break;
                }
                
                case AutoTrainingModalType.Abort:
                {
                    abortTrainingButton.gameObject.SetActive(true);
                    abortConfirmRoot.SetActive(true);
                    
                    // タイトル
                    titleText.text =  StringValueAssetLoader.Instance["auto_training.confirm_modal.abort.title"];
                    // メッセージ
                    messageText.text = string.Empty;
                    // 閉じるボタン
                    closeButtonText.text = StringValueAssetLoader.Instance["common.close"];
                    
                    break;
                }
                
                case AutoTrainingModalType.Replay:
                {
                    startAutoTrainingButton.gameObject.SetActive(true);
                    startConfirmRoot.SetActive(true);
                    
                    // タイトル
                    titleText.text =  StringValueAssetLoader.Instance["auto_training.confirm_modal.replay.title"];
                    // メッセージ
                    messageText.text = StringValueAssetLoader.Instance["auto_training.confirm_modal.msg.replay"];
                    // 閉じるボタン
                    closeButtonText.text = StringValueAssetLoader.Instance["common.cancel"];
                    break;
                }
            }
            
            // 時間のテキスト
            startTimeText.text = AutoTrainingUtility.ToTimeString(arguments.FinishAt);
            
            UpdateTimeText();
            
            // 無料回数の表示
            if(arguments.FreeCompleteCount > 0)
            {
                freeCompleteCountRoot.SetActive(true);
                freeCompleteCountText.text = string.Format(StringValueAssetLoader.Instance["auto_training.free_complete"], arguments.FreeCompleteCount);
            }
            else
            {
                freeCompleteCountRoot.SetActive(false);
            }
            
            return base.OnPreOpen(args, token);
        }
        
        private void UpdateStaminaView()
        {
            // スタミナ
            long stamina = StaminaUtility.GetAutoTrainingStamina() + StaminaUtility.GetStaminaAddition((long)StaminaUtility.StaminaType.AutoTraining);
            staminaValue.SetAfterCount(0, stamina, stamina - 1 );
        }
        
        /// <summary>開始ボタン</summary>
        public void OnStartButton()
        {
            OnStartButtonAsync().Forget();
        }
            
        private async UniTask OnStartButtonAsync()
        {
                
            Arguments arguments = (Arguments)ModalArguments;
            
            // スタミナ
            long stamina = StaminaUtility.GetAutoTrainingStamina() + StaminaUtility.GetStaminaAddition((long)StaminaUtility.StaminaType.AutoTraining);

            // スタミナのチェック
            if(stamina <= 0)
            {
                // 回復モーダルを開く
                AutoTrainingRemainingTimesAddModal.RemainingTimesAddData data = new AutoTrainingRemainingTimesAddModal.RemainingTimesAddData();
                // ステータス
                data.UserStatus = arguments.AutoTrainingUserStatus;
                // スタミナは0
                data.StaminaValue = 0;
                // モーダルを開く
                CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingRemainingTimesAdd, data);
                // 閉じるまで待つ
                await modal.WaitCloseAsync();
                // スタミナ表示更新
                if(IsClosed() == false)
                {
                    UpdateStaminaView();
                }
                return;
            }
            
            // トレーニング開始API
            TrainingStartAutoAPIRequest request = new TrainingStartAutoAPIRequest();
            TrainingStartAutoAPIPost post = new TrainingStartAutoAPIPost();
            request.SetPostData(post);

            switch(arguments.ModalType)
            {
                case AutoTrainingModalType.Replay:
                {
                    post.slotNumber = arguments.SlotNumber;
                    post.isRetry = true;
                    break;
                }
                    
                default:
                {
                    TrainingPreparationArgs preparationArgs = arguments.PreparationArgs;
                    // デッキ
                    DeckData deck = preparationArgs.DeckList.GetDeck(preparationArgs.PartyNumber);
                    // 準備画面で設定したものをAPIに送る
                    post.slotNumber = preparationArgs.AutoTrainingSlot;
                    post.isRetry = false;
                    post.statusType = preparationArgs.AutoTrainingPolity;
                    post.partyNumber = preparationArgs.PartyNumber;
                    post.trainerPartyNumber = preparationArgs.EquipmentPartyNumber;
                    post.friendUCharaId = deck.Friend.id;
                    post.mTrainingScenarioId = preparationArgs.TrainingScenarioId;
                    post.trainingUCharaId = preparationArgs.TrainingUCharId;
                    break;
                }
            }

            // トレーニング対象キャラ
            TrainingSupport trainingChar = null;
            // トレーニングキャラを探す
            foreach(TrainingSupport support in arguments.SupportCharacters)
            {
                TrainingUtility.SupportCharacterType type = (TrainingUtility.SupportCharacterType)support.supportType;
                if(type == TrainingUtility.SupportCharacterType.TrainingChar)
                {
                    trainingChar = support;
                    break;
                }
            }
            
            // API
            await APIManager.Instance.Connect(request);
            // レスポンス
            TrainingStartAutoAPIResponse response = request.GetResponseData();
            // ボイス
            VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(MasterManager.Instance.charaMaster.FindData(trainingChar.mCharaId), VoiceResourceSettings.LocationType.SYSTEM_TRAINING_START ).Forget();
            // モーダルを閉じる
            await AppManager.Instance.UIManager.ModalManager.GetTopModalWindow().CloseAsync();
            
            
            // リプレイ実行済み
            if(arguments.ModalType == AutoTrainingModalType.Replay)
            {
                SetCloseParameter(CloseParamType.ExecuteReplay);
            }
            
            // トレーニングトップページへ
            if(arguments.ModalType == AutoTrainingModalType.Start)
            {
                // 最後に選んだトレーニングを保存しておく
                LocalSaveManager.saveData.trainingData.LatestPlayTrainingId = arguments.TrainingScenarioId;
                
                TrainingPreparation.Arguments args = new TrainingPreparation.Arguments(0, true, arguments.PreparationArgs.IsAutoTrainingOnly);
                await AppManager.Instance.UIManager.PageManager.OpenPageAsync( PageType.TrainingPreparation, true, args);
            }
        }
        
        private void UpdateTimeText()
        {
            Arguments arguments = (Arguments)ModalArguments;
            // 残り時間の表示更新
            if(arguments.ModalType == AutoTrainingModalType.Abort)
            {
                abortTimeText.text = AutoTrainingUtility.ToTimeString(arguments.FinishAt - new TimeSpan(AppTime.Now.Ticks));
            }
        }
        
        /// <summary>
        /// パス購入ボタン
        /// </summary>
        public void OnCompleteImmediatelyPassBuyButton()
        {
            // 完了済みの場合は新しいモーダルは開けない
            if(isCompleted)return;
            // モーダル管理
            CheckModalAsync( AutoTrainingUtility.OpenPassModalAsync( TrainingAutoCostMasterObject.CostType.CompleteImmediately ) ).Forget();
        }
        
        /// <summary>
        /// パス購入ボタン
        /// </summary>
        public void OnShorteningPassBuyButton()
        {
            // 完了済みの場合は新しいモーダルは開けない
            if(isCompleted)return;
            // モーダル管理
            CheckModalAsync( AutoTrainingUtility.OpenPassModalAsync( TrainingAutoCostMasterObject.CostType.Shortening ) ).Forget();
        }
        
        /// <summary>
        /// 即完了ボタン
        /// </summary>
        public void OnImmediatelyButton()
        {
            // 完了済みの場合は新しいモーダルは開けない
            if(isCompleted)return;
            
            // 上限チェック
            if(AutoTrainingUtility.CheckLimitCharacter())return;
            
            Arguments args = (Arguments)ModalArguments;
            // モーダル管理
            CheckModalAsync( AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingImmediateCompletion, new AutoTrainingImmediateCompletionModal.Arguments(args.AutoTrainingPendingStatus, args.AutoTrainingUserStatus, args.SlotNumber, args.Reloadable)) ).Forget();

        }
        
        /// <summary>
        /// 中止ボタン
        /// </summary>
        public void OnAbortButton()
        {
            // 完了済みの場合は新しいモーダルは開けない
            if(isCompleted)return;
            
            // モーダルを開く
            Arguments args = (Arguments)ModalArguments;
            // モーダル管理
            CheckModalAsync( AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingAbortConfirm, new AutoTrainingAbortConfirmModal.Arguments(args.SlotNumber, args.Reloadable)) ).Forget();

        }
        
        /// <summary>開いたモーダルを監視する</summary>
        private async UniTask CheckModalAsync(UniTask<CruFramework.Page.ModalWindow> openTask)
        {
            // 完了済みの場合は新しいモーダルは開けない
            if(isCompleted)return;
            
            isOpendModal = true;
            // 開くまで待つ
            CruFramework.Page.ModalWindow modal = await openTask;
            
            while(true)
            {
                // 閉じてる
                if(modal.IsClosed())break;
                // 完了チェック
                UpdateCompleteTimeCheck();
                // 途中で完了した場合
                if(isCompleted)
                {
                    await modal.CloseAsync();
                    break;
                }
                await UniTask.DelayFrame(1);
            }
            
            isOpendModal = false;
        }
        
        /// <summary>完了時の処理</summary>
        private async UniTask CompleteAsync()
        {
            Arguments args = (Arguments)ModalArguments;
            // 完了に
            isCompleted = true;
            
            // すでに閉じている
            if(IsClosed())return;
            
            // 他のモーダルを開いている場合
            if(isOpendModal)
            {
                // このモーダルを削除
                AppManager.Instance.UIManager.ModalManager.RemoveModals(m=>m == this);
                // モーダルが閉じるまで待つ
                await UniTask.WaitWhile(()=>isOpendModal);
            }
            else
            {
                // このモーダルを閉じる
                await CloseAsync();
            }

            // 再読み込み
            if(args.Reloadable != null)
            {
                await args.Reloadable.OnReloadAsync();
            }
            
            // 完了通知モーダルを開く
            AutoTrainingUtility.OpenConfirmModal(
                StringValueAssetLoader.Instance["auto_training.complete_modal.title"], 
                StringValueAssetLoader.Instance["auto_training.complete_modal.message"],
                StringValueAssetLoader.Instance["common.close"],
                ()=>AutoTrainingUtility.FinishAutoTrainingAsync(args.SlotNumber, args.AutoTrainingUserStatus, args.AutoTrainingPendingStatus).Forget()
            );
        }
        
        private void UpdateCompleteTimeCheck()
        {
            Arguments arguments = (Arguments)ModalArguments;
                
            if(arguments.ModalType == AutoTrainingModalType.Abort)
            {
                // 完了した
                if(arguments.FinishAt <= new TimeSpan(AppTime.Now.Ticks))
                {
                    CompleteAsync().Forget();
                }
            }
        }

        protected void OnEnable()
        {
            // Updateですぐに更新されるように
            updateTimer = AutoTrainingUtility.UpdateTimeInterval;
        }

        private void Update()
        {
            // 完了済み
            if(isCompleted)return;
            
            updateTimer += Time.deltaTime;
            if(updateTimer >= AutoTrainingUtility.UpdateTimeInterval)
            {
                updateTimer = 0;
                UpdateTimeText();
                UpdateCompleteTimeCheck();
            }
        }
    }
}