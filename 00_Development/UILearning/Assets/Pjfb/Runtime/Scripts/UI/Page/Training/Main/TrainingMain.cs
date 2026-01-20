using System;
using System.Threading;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.Adv;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Spine;
using UnityEngine;

namespace Pjfb.Training
{
    
    public enum TrainingMainPageType
    {
        /// <summary>トップ画面</summary>
        Top,
        /// <summary>目標表示画面</summary>
        Target,
        /// <summary>会話/summary>
        Adv,
        /// <summary>休憩/summary>
        Rest,
        /// <summary>トレーニング選択</summary>
        Action,
        /// <summary>イベントの結果画面</summary>
        EventResult,
        /// <summary>練習試合準備画面</summary>
        PracticeGamePreparation,
        /// <summary>トレーニングの結果画面</summary>
        TrainingResult,
        /// <summary>ターン延長確定画面</summary>
        ExtraTurnLottery,
        /// <summary>ターン延長権利付与画面（非イベント報酬としての付与）</summary>
        ExtraTurnRightFirst,
        
        /// <summary>コンセントレーション演出画面</summary>
        ConcentrationEffect,
        /// <summary> Flow演出画面 </summary>
        FlowEffect,
    }
    
    public enum TrainingEventType
    {
        /// <summary>会話パートの再生</summary>
        Scenario = 1,
        /// <summary>ユーザーが練習カードを選ぶ</summary>
        Action   = 2,
        /// <summary>休憩。Scenarioと同じで会話パートの再生</summary>
        Rest     = 3,
        /// <summary>インゲームへ</summary>
        Battle   = 4,
    }
    
    
    public class TrainingMain : PageManager<TrainingMainPageType>
    {
        [SerializeField]
        private TrainingHeader header = null;
        /// <summary>ヘッダー</summary>
        public TrainingHeader Header{get{return header;}}
        
        [SerializeField]
        private AppAdvManager adv = null;
        /// <summary>Adv</summary>
        public AppAdvManager Adv{get{return adv;}}
        
        [SerializeField]
        private TrainingTargetResult targetResult = null;
        /// <summary>目標結果</summary>
        public TrainingTargetResult TargetResult{get{return targetResult;}}
        
        
        [SerializeField]
        private TrainingTargetPageView targetPageView = null;
        /// <summary>目標結果</summary>
        public TrainingTargetPageView TargetPageView{get{return targetPageView;}}
        
        [SerializeField]
        private CharacterSpine character = null;
        /// <summary>キャラ</summary>
        public CharacterSpine Character{get{return character;}}
        
        // キャラクターのエフェクトRootオブジェクト
        [SerializeField]
        private RectTransform characterEffectRoot = null;
        public RectTransform CharacterEffectRoot => characterEffectRoot;
        
        // コンセントレーション演出再生Player
        [SerializeField]
        private TrainingConcentrationZoneEffectPlayer concentrationZoneEffectPlayer = null;
        public TrainingConcentrationZoneEffectPlayer ConcentrationZoneEffectPlayer => concentrationZoneEffectPlayer;
        
        [SerializeField]
        RectTransform characterAnchor = null;
        
        [SerializeField]
        private TrainingActionMessage message = null;
        
        private TrainingMainArguments currentTrainingData = null;
        public TrainingMainArguments CurrentTrainingData{get{return currentTrainingData;}}
        
        
        /// <summary>メッセージの表示</summary>
        public void SetMessage(string msg)
        {
            message.SetMessage(msg);
        }
        
        /// <summary>メッセージの予約</summary>
        public void ReservationMessage(string msg)
        {
            message.ReservationMessage(msg);
        }
        
        /// <summary>予約したメッセージの表示</summary>
        public void SetReservationMessage()
        {
            message.SetReservationMessage();
        }
        
        /// <summary>メッセージの表示</summary>
        public bool IsShowMessage()
        {
            return message.gameObject.activeInHierarchy;
        }
        
        public void UpdateData(TrainingMainArguments data)
        {
            currentTrainingData = data;
        }
        
        public void OpenTargetView(bool isSkip, Action onClosed)
        {
            targetPageView.Open( currentTrainingData.CurrentTarget, isSkip, onClosed );
        }

        protected override string GetAddress(TrainingMainPageType page)
        {
            return $"Prefabs/UI/Page/Training/{page}Page.prefab";
        }

        /// <summary>UGUI</summary>
        public void OnChangeSkipMode(int mode)
        {
            OnChangeAutoMode((AppAdvAutoMode)mode);
        }
        
        private void OnChangeAutoMode(AppAdvAutoMode mode)
        {
            OnChangeAutoModeAsync(mode).Forget();
        }
        
        private async UniTask OnChangeAutoModeAsync(AppAdvAutoMode mode)
        {
            TrainingUtility.AutoMode = mode;
            
            // 初回注意モーダルを表示
            if(mode == AppAdvAutoMode.Skip4 && LocalSaveManager.saveData.trainingData.SkipCautionModal == false)
            {
                // 一度見たら次からは開かない
                LocalSaveManager.saveData.trainingData.SkipCautionModal = true;
                
                string title   = StringValueAssetLoader.Instance["training.skip4.caution.title"];
                string message = StringValueAssetLoader.Instance["training.skip4.caution.message"];
                ConfirmModalButtonParams buttonParam = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], m=>m.Close());
                ConfirmModalData modalData = new ConfirmModalData(title, message, string.Empty, buttonParam);
                // モーダルを開く
                CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, modalData);
                // 閉じるまで待つ
                await modalWindow.WaitCloseAsync();
            }
            
            // メッセージの時間を調整
            message.IsSkip = mode == AppAdvAutoMode.Skip4;
            
            // ローカルデータ保存
            LocalSaveManager.Instance.SaveData();
            
            // シナリオスキップ
            if(mode == AppAdvAutoMode.Skip4 && CurrentPageType == TrainingMainPageType.Adv)
            {
                // チェックポイントが開いている時
                if(targetPageView.gameObject.activeInHierarchy)
                {
                   targetPageView.gameObject.SetActive(false);
                }
                Adv.Skip(AdvSkipMode.AllForceSkip);
            }
        }
        
        protected override void OnEnablePage(object args)
        {
            // ヘッダーとフッタを非表示に
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();
            
            character.UICanvasGroup.alpha = 1;
                        
            // メッセージの時間を調整
            message.IsSkip = TrainingUtility.AutoMode == AppAdvAutoMode.Skip4;
            
            Adv.OnChangeAutoMode += OnChangeAutoMode;
            base.OnEnablePage(args);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            TrainingMainArguments arguments = null;
            
            // スキップボタン
            Adv.SetAutoMode(TrainingUtility.AutoMode);
            
            TrainingMainPageType pageType = TrainingMainPageType.Top;
            
            switch(args)
            {
                case TrainingProgressAPIResponse r:
                {
                    arguments = new TrainingMainArguments(r, new TrainingMainArgumentsKeeps());
                    // ステータス獲得がある場合はアクション名を変更
                    if(arguments.AnyReward())
                    {
                        arguments.ActionName = StringValueAssetLoader.Instance["training.log.game"];
                    }
                    arguments.IsFromInGame = true;
                    break;
                }
                case TrainingMainArguments v:
                {
                    arguments = v;
                    break;
                }
            }
            
            character.UICanvasGroup.alpha = 0;
            // スパインの読み込み
            await character.SetSkeletonDataAssetAsync(MasterManager.Instance.charaMaster.FindData(arguments.Pending.mCharaId).standingImageMCharaId);
            // 1フレ待たないとボーンとか取得できなさそうなので
            await UniTask.DelayFrame(1);
            // キャラの座標を調整
            Bone boneData = character.GetBone("Head");  
            // キャラのTransform
            RectTransform characterTransform = (RectTransform)character.transform;
            Vector3 characterPos = characterAnchor.transform.localPosition;
            Vector3 p = characterAnchor.transform.localPosition - (Vector3)(new Vector2(boneData.WorldX, boneData.WorldY) * AppManager.Instance.UIManager.RootCanvas.referencePixelsPerUnit);
            characterTransform.localPosition = new Vector3(p.x, p.y, characterPos.z);
            // アクティブは切っておく
            character.gameObject.SetActive(false);

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            TrainingChoiceDebugMenu.AddOptions();
#endif
            
            await OpenPageAsync(pageType, false, arguments);
        }

        protected override UniTask OnOpenPage(TrainingMainPageType page, CancellationToken token)
        {
            // ページごとに退出可能か切り分ける
            TrainingUtility.CanExit = page == TrainingMainPageType.PracticeGamePreparation || page == TrainingMainPageType.Action || page == TrainingMainPageType.Adv || page == TrainingMainPageType.TrainingResult;
            
            return base.OnOpenPage(page, token);
        }

        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            TrainingDeckUtility.SetCurrentTrainingDeck(null);
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            TrainingChoiceDebugMenu.RemoveOptions();
#endif
            
            return await base.OnPreClose(token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
        }

        protected override async UniTask OnPreClosePage(TrainingMainPageType page, CancellationToken token)
        {        
            
            await base.OnPreClosePage(page, token);
        }

        protected override void OnClosed()
        {
            Adv.Release();
            Adv.OnChangeAutoMode -= OnChangeAutoMode;
            base.OnClosed();
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnMenuButton()
        {
            OnMenuButtonAsync().Forget();            
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OpenHelpModal()
        {
            TrainingUtility.OpenHelpModal();
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnTargetInfoButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingTargetConfirm, currentTrainingData);
        }
        
        private async UniTask OnMenuButtonAsync()
        {
            // Advを停止させる
            adv.IsPause = true;
            // モーダルを開く
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingMenu, currentTrainingData);
            // モーダルが閉じるまで待つ
            bool movePage = (bool)await modal.WaitCloseAsync();
            
            // Advを再開させる
            if(movePage == false)
            {
                adv.IsPause = false;
            }
        }
        
        protected virtual void OnBackkey()
        {
            if(CurrentPageObject is TrainingPageBase page)
            {
                page.OnBackkey();
            }
        }
        
        private void Update()
        {
#if UNITY_EDITOR || UNITY_ANDROID
            if(Input.GetKeyDown(KeyCode.Escape) && TrainingUtility.CanExit && AppManager.Instance.UIManager.ModalManager.GetTopModalWindow() == null)
            {
                OnBackkey();
            }
#endif // UNITY_EDITOR || UNITY_ANDROID
        }
    }
}
