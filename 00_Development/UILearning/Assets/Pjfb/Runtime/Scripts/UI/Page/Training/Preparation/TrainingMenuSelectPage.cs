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
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;
using Logger = CruFramework.Logger;

namespace Pjfb.Training
{
    
    /// <summary>
    /// トレーニングメニュー選択画面
    /// </summary>
    
    public class TrainingMenuSelectPage : TrainingPreparationPageBase, IAutoTrainingReloadable
    {
        private static readonly string UnlockedAnimation = "Unlocked";
        private static readonly string LockedAnimation = "Locked";
        
        [SerializeField]
        protected ScrollBanner scrollBanner = null;
        [SerializeField]
        private UIButton nextButton = null;

        [SerializeField]
        private GameObject lockRoot = null;
        [SerializeField]
        private TMPro.TMP_Text conditionText = null;
        [SerializeField]
        private Animator lockAnimator = null;
        [SerializeField]
        private CancellableRawImage bannerImage = null;
        
        [SerializeField]
        private StaminaView autoTrainingStaminaView = null;
        
        [SerializeField]
        private AutoTrainingFooterMenu autoTrainingMenu = null;
        
        [SerializeField]
        protected TMP_Text pageNameText = null;
        
        [SerializeField]
        private TrainingMenuSelectBonusView bonusView = null;
        
        // 選択中ページ
        private int selectedPage = -1;
        
        private TrainingMode currentViewMode = TrainingMode.Default;

        private List<int> unlockIndexList = new List<int>();
        
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {            
            // 初期ページ
            int firstPage = 0;
            
            TrainingPreparationArgs arguments = (TrainingPreparationArgs)args;
            
            // 自動トレーニングUI初期化
            autoTrainingMenu.Initialize(arguments, AutoTrainingUserStatus, AutoTrainingPendingStatus);
            
            // 表示するId
            List<TrainingMenuItem.Arguments> menuArgs = new List<TrainingMenuItem.Arguments>();
            
            // 新しく開放されたシナリオ
            unlockIndexList.Clear();

            // マスタから取得
            foreach(TrainingScenarioMasterObject scenario in MasterManager.Instance.trainingScenarioMaster.values)
            {
                // 時限チェック
                if(AppTime.IsInPeriod(scenario.startAt, scenario.endAt) == false)continue;
                // チュートリアルチェック
                if (scenario.IsTutorial()) continue;
                
                // 未開放チェック
                bool isLocked = scenario.mSystemLockSystemNumber > 0 && UserDataManager.Instance.IsUnlockSystem(scenario.mSystemLockSystemNumber) == false;
                // 汎用解放演出中のときは解除
                isLocked = isLocked && !SystemUnlockDataManager.Instance.IsUnlockingSystem(scenario.mSystemLockSystemNumber);
                
                // 開放演出
                if(scenario.mSystemLockSystemNumber > 0 && isLocked == false && SystemUnlockDataManager.Instance.IsUnlockingSystem(scenario.mSystemLockSystemNumber))
                {
                    unlockIndexList.Add(menuArgs.Count);
                }
                
                // リストに追加
                menuArgs.Add( new TrainingMenuItem.Arguments(scenario.id, isLocked) );
            }
            
            // 開放演出チェック
            if(unlockIndexList.Count > 0)
            {
                firstPage = unlockIndexList[0];
            }
            // 指定されている場合
            else if(arguments.TrainingScenarioId > 0)
            {
                for(int i=0;i<menuArgs.Count;i++)
                {
                    // 初期ページチェック
                    if(menuArgs[i].ScenarioId == arguments.TrainingScenarioId)
                    {
                        firstPage = i;
                    }
                }
            }
            // 最後に遊んだトレーニング
            else if(LocalSaveManager.saveData.trainingData.LatestPlayTrainingId >= 0)
            {
                for(int i=0;i<menuArgs.Count;i++)
                {
                    if(menuArgs[i].ScenarioId == LocalSaveManager.saveData.trainingData.LatestPlayTrainingId)
                    {
                        firstPage = i;
                    }
                }
            }
            
            // スクロールに表示
            scrollBanner.SetBannerDatas(menuArgs);
            scrollBanner.SetIndex(firstPage, false);
            OnChangePage(firstPage);
            
            scrollBanner.onChangedPage -= OnChangePage;
            scrollBanner.onChangedPage += OnChangePage;
            
            // スタミナ
            await autoTrainingStaminaView.UpdateAsync( StaminaUtility.StaminaType.AutoTraining );
            
            // 自動トレーニングのみ
            if(arguments.IsAutoTrainingOnly)
            {
                OnChangeAutoTraining();
            }
            else
            {
                // 初期ページ
                switch(currentViewMode)
                {
                    case TrainingMode.Default:
                        OnChangeDefaultTraining();
                        break;
                    case TrainingMode.Auto:
                        OnChangeAutoTraining();
                        break;
                }
            }
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            if(unlockIndexList.Count > 0)
            {
                UnlockAnimationAsync().Forget();
            }
            // 自動トレーニングの解放演出中または自動トレーニングが解放済みかつチュートリアル未再生
            else if (SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining) || (UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining) && AppManager.Instance.TutorialManager.IsAlreadyPlayedTutorial(SystemUnlockDataManager.SystemUnlockNumber.AutoTraining) == false))
            {
                // シナリオを見てないならシナリオを再生
                if (AppManager.Instance.TutorialManager.OpenAutoTrainingScenarioTutorial(true, null))
                {
                    return;
                }
            
                // チュートリアル画像を表示
                AppManager.Instance.TutorialManager.OpenAutoTrainingTutorialAsync().Forget();
            }
        }

        /// <summary>自動トレーニングの再読み込み</summary>
        private async UniTask ReloadAutoTraining()
        {
            TrainingPreparation page = (TrainingPreparation)Manager;
            // 自動レーニングのデータを取得
            await page.ReloadAutoTraining();
            // 自動トレーニングのビューを初期化
            autoTrainingMenu.Initialize((TrainingPreparationArgs)OpenArguments, AutoTrainingUserStatus, AutoTrainingPendingStatus);
            // 現在の選択中のページを設定
            if(selectedPage >= 0)
            {
                OnChangePage(selectedPage);
            }
        }

        private async UniTask UnlockAnimationAsync()
        {

            // タッチガードOn
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            // フェードが終わるまで待つ
            await UniTask.WaitWhile(()=>AppManager.Instance.UIManager.FadeManager.CurrentState != FadeManager.State.Idle);
            // タッチガードOn
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            
            while(true)
            {
                // 全ての演出が終わった
                if(unlockIndexList.Count <= 0)break;
                // タッチガードOn
                AppManager.Instance.UIManager.System.TouchGuard.Show();
                // 演出するトレーニング
                int index = unlockIndexList[0];
                unlockIndexList.RemoveAt(0);
                // 演出するページへ
                scrollBanner.ScrollGrid.SetPage(index, true);
                // 1ページアニメーションが終わるまで待つ
                await UniTask.WaitWhile(()=>scrollBanner.ScrollGrid.IsPagingAnimation);
                // オブジェクト取得
                TrainingMenuItem.Arguments args = (TrainingMenuItem.Arguments)scrollBanner.ScrollGrid.GetItems()[index];
                // アニメーション再生
                await PlayUnlockAnimation();
                // タッチガードOff
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                
                HowToPlayModal.HowToData howtoData = HowToPlayModal.CreateHowToDataByScenarioId(args.ScenarioId);
                // 遊び方モーダルを開く
                CruFramework.Page.ModalWindow howToModal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.HowToPlay, howtoData);
                // 閉じるまで待つ
                await howToModal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
                // 詳細モーダルを開く
                CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainingMenuDetail, args.ScenarioId);
                // 閉じるまで待つ
                await modal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
            }

            await SystemUnlockDataManager.Instance.RequestReadUnlockEffectAsync();
        }

        private void OnChangePage(int page)
        {
            selectedPage = page;
            // Idリスト
            IList list = scrollBanner.ScrollGrid.GetItems();
            // データ
            TrainingMenuItem.Arguments args = (TrainingMenuItem.Arguments)list[scrollBanner.ScrollGrid.CurrentPage];
            
            // マスタ取得
            TrainingScenarioMasterObject m = MasterManager.Instance.trainingScenarioMaster.FindData(args.ScenarioId);

            
            // バナーの読み込み
            bannerImage.SetTexture(ResourcePathManager.GetPath("TrainingMenuBanner", args.ScenarioId));
            
            // ロックされている
            nextButton.interactable = args.IsLocked == false;
            // ロック表示
            lockRoot.SetActive( args.IsLocked );
            
            // 自動トレーニング有効か？
            bool isAutoTrainingEnable = AutoTrainingUtility.IsUnLockAutoTraining() && m.IsEnabledTrainingAuto() && AppTime.IsInPeriod(m.trainingAutoStartAt);
            // 自動トレーニングが有効なシナリオ？
            autoTrainingMenu.SetEnable(isAutoTrainingEnable );
            // 自動トレーニングもロック
            autoTrainingMenu.SetScenarioLock(args.IsLocked || isAutoTrainingEnable == false);
            // モーダルを有効か
            autoTrainingMenu.SetLockModalEnable(true);
            
            if (args.IsLocked)
            {
                AnimatorUtility.WaitStateAsync(lockAnimator, LockedAnimation).Forget();
            }
            
            // バナーの色
            bannerImage.SetColor( args.IsLocked ? Color.gray : Color.white );
            
            // ボーナスビューの更新
            bonusView.SetView(args.ScenarioId);
            
            
            // 解放条件
            if(args.IsLocked)
            {
                SystemLockMasterObject mLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(m.mSystemLockSystemNumber);
                
                // 解放条件テキスト
                if(mLock != null)
                {
                    conditionText.text = mLock.description;
                    // 自動トレーニングのテキスト
                    autoTrainingMenu.SetScenarioLockText( StringValueAssetLoader.Instance["auto_training.scenario_lock"]);
                    // モーダルを無効か
                    autoTrainingMenu.SetLockModalEnable(false);
                }
                else
                {
                    conditionText.text = string.Empty;
                }
            }
            
            // 自動トレーニングが無効の場合はそのメッセージを出す
            if(isAutoTrainingEnable == false)
            {
                autoTrainingMenu.SetScenarioLockText(StringValueAssetLoader.Instance["training.menu.auto_training_disable_text"]);
            }
        }
        
        private async UniTask PlayUnlockAnimation()
        {
            lockRoot.SetActive(true);
            await AnimatorUtility.WaitStateAsync(lockAnimator, UnlockedAnimation);
        }
        
        /// <summary>UGUI</summary>
        public void OnChangeAutoTraining()
        {
            TrainingPreparation page = (TrainingPreparation)Manager;
            
            // ページ名
            pageNameText.text = StringValueAssetLoader.Instance["auto_training.menu_name"];
            // スタミナ表示
            page.StaminaView.UpdateAsync(StaminaUtility.StaminaType.AutoTraining).Forget();
            page.StaminaView.AutoUserStatus = page.AutoTrainingStatus.userStatus;
            currentViewMode = TrainingMode.Auto;
            
            autoTrainingMenu.ShowActiveAutoTrainingMenu();
        }
        
        /// <summary>UGUI</summary>
        public void OnChangeDefaultTraining()
        {
            TrainingPreparation page = (TrainingPreparation)Manager;
            // ページ名
            pageNameText.text = StringValueAssetLoader.Instance["training.page_name.menu"];
            // スタミナ表示
            page.StaminaView.UpdateAsync(StaminaUtility.StaminaType.Training).Forget(); 
            
            currentViewMode = TrainingMode.Default;
            
            autoTrainingMenu.ShowDefaultMenu();
        }
        
        /// <summary>自動トレーニング完了</summary>
        public void OnAutoTrainingCompleteButton()
        {
            TrainingPreparationArgs args = (TrainingPreparationArgs)OpenArguments;
            // 完了させる
            FinishAutoTrainingAsync(args.AutoTrainingSlot).Forget();
        }
        
        /// <summary>自動トレーニングステータス</summary>
        public void OnAutoTrainingStatusButton()
        {
            OnAutoTrainingStatusButtonAsync().Forget();
        }
        
        private async UniTask OnAutoTrainingStatusButtonAsync()
        {
            TrainingPreparationArgs args = (TrainingPreparationArgs)OpenArguments;

            // スロット番号
            long slot = args.AutoTrainingSlot;
            // Pending
            TrainingAutoPendingStatus pendingStatus = null;
            // スロットのデータを探す
            foreach(TrainingAutoPendingStatus pending in AutoTrainingPendingStatus)
            {
                if(pending.slotNumber == slot)
                {
                    pendingStatus = pending;
                    break;
                }
            }
            
            // モーダルに送る引数
            AutoTrainingConfirmModal.Arguments arguments = new AutoTrainingConfirmModal.Arguments(pendingStatus, AutoTrainingUserStatus, this, AutoTrainingModalType.Abort);
            // モーダルを開く
            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync( ModalType.AutoTrainingConfirm, arguments );
            // 閉じるまで待つ
            if(await modal.WaitCloseAsync() is AutoTrainingConfirmModal.CloseParamType p)
            {
                Debug.LogError(p);
            }
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnDetailButton()
        {
            // Idリスト
            IList list = scrollBanner.ScrollGrid.GetItems();
            // データ
            TrainingMenuItem.Arguments args = (TrainingMenuItem.Arguments)list[scrollBanner.ScrollGrid.CurrentPage];
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainingMenuDetail, args.ScenarioId);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelectedAutoTrainingButton()
        {
            SelectTraining(TrainingMode.Auto);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSelectedButton()
        {
            SelectTraining(TrainingMode.Default);
        }
        
        UniTask IAutoTrainingReloadable.OnReloadAsync()
        {
            return ReloadAutoTraining();
        }
        
        private  UniTask FinishAutoTrainingAsync(long slot)
        {
            return AutoTrainingUtility.FinishAutoTrainingAsync(slot, AutoTrainingUserStatus, AutoTrainingPendingStatus); 
        }
        
        private void SelectTraining(TrainingMode trainingMode)
        {
            // アニメーション中は無効
            if(scrollBanner.ScrollGrid.IsPagingAnimation)return;
            // サポート器具上限チェック
            if (SupportEquipmentManager.ShowOverLimitModal()) 
            {
                return;
            }
            
            // Idリスト
            IList list = scrollBanner.ScrollGrid.GetItems();
            // データ
            TrainingMenuItem.Arguments args = (TrainingMenuItem.Arguments)list[scrollBanner.ScrollGrid.CurrentPage];
            
            // マスタ
            TrainingScenarioMasterObject mScenario = MasterManager.Instance.trainingScenarioMaster.FindData(args.ScenarioId);
            // 未開放チェック
            bool isLocked = mScenario.mSystemLockSystemNumber > 0 && UserDataManager.Instance.IsUnlockSystem(mScenario.mSystemLockSystemNumber) == false;
            if(isLocked)return;

            // 選択したシナリオIdを登録
            Arguments.TrainingScenarioId = args.ScenarioId;
            // トレーニングモード
            Arguments.Mode = trainingMode;
            // キャラ選択画面へ移動
            TrainingPreparationManager.OpenPage(TrainingPreparationPageType.CharacterSelect, true, Arguments);
        }

        
#if UNITY_EDITOR
        
        private void Update()
        {
            
            // パス購入導線デバッグ
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.CompleteImmediately );
            }
            
            // パス購入導線デバッグ
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd );
            }
            
            // パス購入導線デバッグ
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.Shortening );
            }
            
            // 自動トレーニング結果画面デバッグ
            if(Input.GetKeyDown(KeyCode.A))
            {
                
                TrainingFinishAutoAPIResponse res = new TrainingFinishAutoAPIResponse();
                
                res.result = new TrainingAutoResultStatus();
                res.result.abilityList = new TrainingAbility[0];
                res.result.turnList = new ResultTurn[0];
                res.result.inspireList = new ResultIdCount[0];
                res.result.concentrationList = new ResultIdCount[0];
                res.result.tierList = new ResultTier[0];
                res.result.inspireExecuteList = new ResultIdCount[0];
                
                res.result.intentionalCount = 10;
                res.result.rareTrainingCount = 12;
                
                res.result.cardList = new ResultCard[5];
                for(int i=0;i<res.result.cardList.Length;i++)
                {
                    res.result.cardList[i] = new ResultCard();
                    res.result.cardList[i].mCharaId = 0;
                    res.result.cardList[i].mTrainingCardId = i + 1;
                }
                
                
                res.pending = new TrainingPending();
                res.pending.mCharaId = 10001001;
                res.pending.mTrainingScenarioId = 2;
                res.pending.nextGoalIndex = -1;
                res.pending.supportDetailList = new TrainingSupport[1];
                res.pending.supportDetailList[0] = new TrainingSupport();
                res.pending.supportDetailList[0].supportType = (long)TrainingUtility.SupportCharacterType.TrainingChar;
                res.pending.supportDetailList[0].mCharaId = res.pending.mCharaId;
                
                res.pending.practiceProgressList = new TrainingPracticeProgress[0];
                
                res.charaVariable = new TrainingCharaVariable();
                res.charaVariable.mCharaId = res.pending.mCharaId;
                foreach(var ids in UserDataManager.Instance.charaVariable.data)
                {
                    res.charaVariable.uCharaVariableId = ids.Key;
                    break;
                }
                res.charaVariable.abilityList = new TrainingAbility[0];
                    
                res.friend = new TrainingFriend();
                
                TrainingAutoUserStatus userStatus = new TrainingAutoUserStatus();
                TrainingAutoPendingStatus pendingStatus = new TrainingAutoPendingStatus();
                TrainingMainArguments mainArgs = new TrainingMainArguments(res, pendingStatus, userStatus);
                // トレーニング結果画面へ
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Training, false, mainArgs);
            }
        }
#endif // UNITY_EDITOR
    }
    
}