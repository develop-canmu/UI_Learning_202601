using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using Pjfb.Voice;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingStartConfirmModal : ModalWindow
    {
        private TrainingPreparationArgs preparationArgs = null;
        
        private static readonly string MessageStringKey = "training.start_confirm.message";
        private static readonly string FreeStaminaMessageStringKey = "training.start_confirm.free_stamina_message";
        
        [SerializeField]
        private TrainingMemberView memberView = null;
        
        [SerializeField]
        private TMP_Text messageText = null;
        
        [SerializeField]
        private TMP_Text staminaText = null;
        
        [SerializeField]
        private TMP_Text afterStaminaText = null;
        
        [SerializeField] 
        private TextMeshProUGUI staminaFreeText = null;
        [SerializeField] 
        private GameObject staminaFreeRoot = null;
        
        
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            preparationArgs = (TrainingPreparationArgs)args;
            InitPage();
            
            return base.OnPreOpen(args, token);
        }

        private void InitPage()
        {
            // 表示する内容
            List<TrainingSupport> supportList = preparationArgs.GetSupportList();
            // トレーニング対象キャラ
            UserDataChara uTrainingChar = UserDataManager.Instance.chara.Find(preparationArgs.TrainingUCharId);
            // メンバービューの表示
            memberView.SetTrainingCharacter( uTrainingChar.charaId, uTrainingChar.level, uTrainingChar.newLiberationLevel, preparationArgs.TrainingScenarioId, supportList.ToArray());
            memberView.GetTrainingScenarioId = () => preparationArgs.TrainingScenarioId;
            
            // シナリオマスタ
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(preparationArgs.TrainingScenarioId);
            // スタミナ
            long freeStamina = StaminaUtility.GetFreeStaminaRemainingUse((long)StaminaUtility.StaminaType.Training, mScenario.requiredStamina);
            string messageString = freeStamina > 0 ? 
                                string.Format(StringValueAssetLoader.Instance[FreeStaminaMessageStringKey]) : 
                                string.Format(StringValueAssetLoader.Instance[MessageStringKey], mScenario.requiredStamina);
            messageText.text = messageString;
            
            // スタミナ
            long stamina = StaminaUtility.GetTrainingStamina();
            var afterStamina = freeStamina > 0 ? stamina : stamina - mScenario.requiredStamina;
            staminaText.text = stamina.ToString();
            afterStaminaText.text = (afterStamina).ToString();

            // スタミナ消費なしの吹き出し
            if (staminaFreeRoot != null && staminaFreeText != null)
            {
                staminaFreeRoot.SetActive(freeStamina > 0);
                staminaFreeText.text = string.Format(StringValueAssetLoader.Instance["stamina.free"], freeStamina);
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnStartButton()
        {
            StartAPIAsync().Forget();
        }
        
        private async UniTask StartAPIAsync()
        {
            // シナリオマスタ
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(preparationArgs.TrainingScenarioId);
            long requiredStamina = mScenario.requiredStamina;
            
#if UNITY_EDITOR
            // テスト中は邪魔なので
            requiredStamina = 0;
#endif
            
            // スタミナをチェック
            if(StaminaUtility.GetTrainingStamina() < requiredStamina && StaminaUtility.GetStaminaAddition((long)StaminaUtility.StaminaType.Training) < requiredStamina)
            {
                // 引数
                StaminaInsufficientModal.Data data = new StaminaInsufficientModal.Data();
                data.staminaType = StaminaUtility.StaminaType.Training;
                data.mStaminaId = (long)data.staminaType;
                // モーダル開く
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StaminaInsufficient, data);
                
                return;
            }

            // パス期間チェック
            if (UserDataManager.Instance.IsExpiredPass((long)StaminaUtility.StaminaType.Training))
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"], 
                    StringValueAssetLoader.Instance["pass.expired"], 
                    string.Empty, 
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => 
                    {
                        InitPage();
                        window.Close();
                    })));
                return;
            }
            
            try
            {
                // トレーニング開始API
                TrainingStartAPIRequest request = new TrainingStartAPIRequest();
                TrainingStartAPIPost post = new TrainingStartAPIPost();
                request.SetPostData(post);

                // トレーニングキャラの設定
                DeckData deck = preparationArgs.DeckList.GetDeck(preparationArgs.PartyNumber);
                // 準備画面で選択したキャラやデッキをAPIに送る
                post.partyNumber = preparationArgs.PartyNumber;
                post.trainerPartyNumber = preparationArgs.EquipmentPartyNumber;
                post.friendUCharaId = deck.Friend.id;
                post.mTrainingScenarioId = preparationArgs.TrainingScenarioId;
                post.trainingUCharaId = preparationArgs.TrainingUCharId;
            
                // API
                await APIManager.Instance.Connect(request);
                // レスポンス
                TrainingStartAPIResponse response = request.GetResponseData();
                // ログの削除
                TrainingUtility.DeleteLog();
                // ターン延長の最大値設定
                TrainingUtility.SetMaxAddTurn(response.maxAddTurnValue);
                // スキップモードの初期化
                TrainingUtility.AutoMode = 0;
                // 最後に選んだトレーニングを保存しておく
                LocalSaveManager.saveData.trainingData.LatestPlayTrainingId = preparationArgs.TrainingScenarioId;
                // データ保存
                LocalSaveManager.Instance.SaveData();
                // ボイス
                VoiceManager.Instance.PlaySystemVoiceForLocationTypeAsync(UserDataManager.Instance.chara.Find(preparationArgs.TrainingUCharId).MChara, VoiceResourceSettings.LocationType.SYSTEM_TRAINING_START ).Forget();
                
                // モーダルを閉じる
                await AppManager.Instance.UIManager.ModalManager.GetTopModalWindow().CloseAsync();
                // トレーニングページへ
                await AppManager.Instance.UIManager.PageManager.OpenPageAsync( PageType.Training, false, new TrainingMainArguments(response.trainingEvent, response.pending, response.battlePending, response.charaVariable, null, response.pointStatus, new TrainingMainArgumentsKeeps(), TrainingMainArguments.Options.None) );

            }
            catch(APIException e)
            {
                if(e.errorParam == TrainingUtility.API_ERROR_CODE_FRIEND_LIMIT)
                {
                    SetCloseParameter(true);
                    // フレンドを更新
                    preparationArgs.IsUpdateFriendList = true;
                    
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>
                    {
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
                        m.Close();
                    });
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["training.friend_error.title"], e.apiErrorMessage, string.Empty, button);
                    CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync( ModalType.Confirm, data );
                    await modalWindow.WaitCloseAsync();
                }
            }
        }
    }
}
