using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Unity.VisualScripting;

namespace Pjfb.Training
{
    public class AutoTrainingImmediateCompletionModal : ModalWindow
    {
        public class Arguments
        {
            
            private long slotNumber = 0;
            /// <summary>スロット番号</summary>
            public long SlotNumber{get{return slotNumber;}}
            
            private IAutoTrainingReloadable reloadable = null;
            /// <summary>再読み込みよう</summary>
            public IAutoTrainingReloadable Reloadable{get{return reloadable;}}

            private TrainingAutoPendingStatus pendingStatus = null;
            /// <summary>自動トレーニング情報</summary>
            public TrainingAutoPendingStatus PendingStatus{get{return pendingStatus;}}
            
            private TrainingAutoUserStatus status = null;
            /// <summary>自動トレーニング情報</summary>
            public TrainingAutoUserStatus Status{get{return status;}}
            
            public Arguments(TrainingAutoPendingStatus pendingStatus, TrainingAutoUserStatus status, long slotNumber, IAutoTrainingReloadable reloadable)
            {
                this.pendingStatus = pendingStatus;
                this.slotNumber = slotNumber;
                this.reloadable = reloadable;
                this.status = status;
            }
        }
        
        [SerializeField]
        private AutoTrainingImmediateCompletionModalBody gemRoot = null;
        [SerializeField]
        private AutoTrainingImmediateCompletionModalBody itemRoot = null;
        [SerializeField]
        private AutoTrainingImmediateCompletionModalBody freeRoot = null;
        [SerializeField]
        private AutoTrainingImmediateCompletionModalBody limitRoot = null;
        
        [SerializeField]
        private UIButton passBuyButton = null;
        
        [SerializeField]
        private UIButton limitPassBuyButton = null;
        
        [SerializeField]
        private UIButton executeButton = null;
        
        [SerializeField]
        private GameObject termsTransactionLawRoot = null;
        
        private long executeAutoCostId = -1;
        
        // ジェムショップへ
        private bool isGotoGemShop = false;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            // 全部非表示
            gemRoot.gameObject.SetActive(false);
            itemRoot.gameObject.SetActive(false);
            freeRoot.gameObject.SetActive(false);
            limitRoot.gameObject.SetActive(false);
            termsTransactionLawRoot.SetActive(false);
            // 基本On
            executeButton.gameObject.SetActive(true);
            
            // 使用アイテムを取得
            TrainingAutoCostMasterObject mAutoCost = AutoTrainingUtility.GetCostMasterNextRequired(arguments.Status, TrainingAutoCostMasterObject.CostType.CompleteImmediately);
            
            // Nullの場合は上限に達していて実行できない
            if(mAutoCost == null)
            {
                limitRoot.gameObject.SetActive(true);
                // パス購入ボタン
                limitPassBuyButton.interactable = AutoTrainingUtility.HasCompleteImmediatelyPass() == false;
                // 実行ボタンを隠す
                executeButton.gameObject.SetActive(false);
            }
            else
            {
                // 取得したIdを保持
                executeAutoCostId = mAutoCost.id;
                
                // 無料？
                if(mAutoCost.mPointId <= 0)
                {
                    freeRoot.gameObject.SetActive(true);
                    // 無料回数
                    long maxFreeCount = AutoTrainingUtility.GetTotalFreeAutoCostWithTag( TrainingAutoCostMasterObject.CostType.CompleteImmediately );
                    // メッセージ表示
                    string msg = string.Format(StringValueAssetLoader.Instance["auto_training.pass.free_count.message"], maxFreeCount);
                    freeRoot.SetMessage(msg);
                    // 無料回数
                    freeRoot.SetItemCount( 0, arguments.Status.freeCompleteRemainCount, arguments.Status.freeCompleteRemainCount-1 );
                }
                else
                {
                    // MPoint
                    PointMasterObject mPoint = MasterManager.Instance.pointMaster.FindData(mAutoCost.mPointId);
                    
                    // Gem?
                    if(mPoint.id == ConfigManager.Instance.mPointIdGem)
                    {
                        // パスの状態
                        bool hasPass = AutoTrainingUtility.HasCompleteImmediatelyPass();
                        
                        // 特定商取引法の表示
                        termsTransactionLawRoot.SetActive(true);
                        // 導線ボタンの表示切り替え
                        passBuyButton.interactable = hasPass == false;
                        
                        gemRoot.gameObject.SetActive(true);
                        // メッセージ表示
                        string msg = string.Format(StringValueAssetLoader.Instance["auto_training.immediate_modal.message_gem"], mPoint.name, mAutoCost.value, mPoint.unitName);
                        gemRoot.SetMessage(msg);
                        // アイテム数
                        long count = UserDataManager.Instance.GetPointValue(mPoint.id);
                        long afterCount = count -  mAutoCost.value;
                        gemRoot.SetItemCount( mPoint.id, count,　afterCount < 0 ? 0 : afterCount );
                        gemRoot.SetShortageColor();
                        // 不足している場合はショップへ
                        isGotoGemShop = count < mAutoCost.value;
                    }
                    else
                    {
                        // アイテム
                        itemRoot.gameObject.SetActive(true);
                        // メッセージ表示
                        string msg = string.Format(StringValueAssetLoader.Instance["auto_training.immediate_modal.message_item"], mPoint.name, mAutoCost.value, mPoint.unitName);
                        itemRoot.SetMessage(msg);
                        // アイテム数
                        long count = UserDataManager.Instance.GetPointValue(mPoint.id);
                        itemRoot.SetItemCount( mPoint.id, count, count -  mAutoCost.value);
                    }
                }
            }
            
            return base.OnPreOpen(args, token);
                
        }
        
        
        /// <summary>UGUI</summary>
        public void OnImmediateButton()
        {
            OnImmediateButtonAsync().Forget();
        }
        
        /// <summary>UGUI</summary>
        public async UniTask OnImmediateButtonAsync()
        {
            // キャラ上限
            if(AutoTrainingUtility.CheckLimitCharacter())return;
            
            // ジェム不足なのでショップの導線モーダルを開く
            if(isGotoGemShop)
            {
                // 必要数の取得
                TrainingAutoCostMasterObject mAutoCost = MasterManager.Instance.trainingAutoCostMaster.FindData(executeAutoCostId);
                AutoTrainingUtility.OpenGemModal( StringValueAssetLoader.Instance["auto_training.gem_shortage.immediate_title"], mAutoCost.value );
                return;
            }
            
            Arguments arguments = (Arguments)ModalArguments;
            
            // モーダルを全て閉じる
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((m)=>true);
            
            // Request
            TrainingFinishAutoAPIRequest request = new TrainingFinishAutoAPIRequest();
            // Post
            TrainingFinishAutoAPIPost post = new TrainingFinishAutoAPIPost();
            // スロット
            post.slotNumber = arguments.SlotNumber;
            // 使用アイテム
            post.id = executeAutoCostId;
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            // 再読み込み
            if(arguments.Reloadable != null)
            {
                await arguments.Reloadable.OnReloadAsync();
            }
            
            // モーダル閉じる
            await CloseAsync();
            
            TrainingMainArguments mainArgs = new TrainingMainArguments(request.GetResponseData(), arguments.PendingStatus, arguments.Status);
            // トレーニング結果画面へ
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Training, false, mainArgs);
            
        }
        
        /// <summary>UGUI</summary>
        public void OnPassBuyButton()
        {
            AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.CompleteImmediately );
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
    }
}