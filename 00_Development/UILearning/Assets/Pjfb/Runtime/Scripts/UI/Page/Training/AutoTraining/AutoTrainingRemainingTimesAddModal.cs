using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

namespace Pjfb.Training
{
    public class AutoTrainingRemainingTimesAddModal : ModalWindow
    {
        public class RemainingTimesAddData
        {   
            // 自動トレーニングユーザーデータ
            private TrainingAutoUserStatus userStatus;
            public TrainingAutoUserStatus UserStatus
            {
                get { return userStatus; }
                set { userStatus = value; }
            }
            // 現在のスタミナ量
            private long staminaValue;
            public long StaminaValue
            {
                get { return staminaValue; }
                set { staminaValue = value; }
            }
            // 回復後のユーザーステータス更新
            private Action<TrainingAutoUserStatus> onSetUserStatus;
            public Action<TrainingAutoUserStatus> OnSetUserStatus
            {
                get{return onSetUserStatus;}
                set{onSetUserStatus = value;}
            }
        }

        [SerializeField] private AutoTrainingRemainingTimesAddModalBody gem;
        [SerializeField] private AutoTrainingRemainingTimesAddModalBody item;
        [SerializeField] private GameObject limit;
        [SerializeField] private UIButton buttonBuyPass;
        [SerializeField] private UIButton buttonLimitBuyPass;
        [SerializeField] private TextMeshProUGUI canselButtonText;
        [SerializeField] private GameObject addButton;
        [SerializeField] private GameObject termsTransactionLaw;
        
        private RemainingTimesAddData remainingTimesAddData;
        private TrainingAutoCostMasterObject selectedAutoCost;
        private bool enoughItem = false;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            remainingTimesAddData = (RemainingTimesAddData)args;
            AcquisitionData();
            InitUI();
            return base.OnPreOpen(args, token);
        }

        private void InitUI() 
        {
            // 各オブジェクトを非表示にする
            limit.SetActive(false);
            gem.gameObject.SetActive(false);
            item.gameObject.SetActive(false);
            termsTransactionLaw.SetActive(false);
            

            addButton.gameObject.SetActive(selectedAutoCost != null);
            // 交換上限に達していた場合の処理
            if (selectedAutoCost == null)
            {
                limit.SetActive(true);
                canselButtonText.text = StringValueAssetLoader.Instance["common.close"];
                // パス購入状況を見てボタンの表示非表示を設定
                bool hasPass = AutoTrainingUtility.HasStaminaPass();
                buttonLimitBuyPass.interactable = hasPass == false;
                return;
            }

            canselButtonText.text = StringValueAssetLoader.Instance["common.cancel"];
            // 所持アイテムの個数を取得(無ければ0)
            UserDataPoint point = UserDataManager.Instance.point.Find(selectedAutoCost.mPointId);
            long itemValue = 0;
            if (point != null)
            {
                itemValue = point.value;
            }
            // アイテムが足りているか
            long afterValue = itemValue - selectedAutoCost.value;
            enoughItem = afterValue >= 0;
            if (afterValue < 0)
            {
                afterValue = 0;
            }
            // 消費アイテムがジェムかそれ以外かで分岐
            if (selectedAutoCost.mPointId == ConfigManager.Instance.mPointIdGem)
            {
                gem.gameObject.SetActive(true);
                termsTransactionLaw.SetActive(true);
                gem.SetItemView(selectedAutoCost.mPointId, itemValue, afterValue);
                gem.SetStaminaView(remainingTimesAddData.StaminaValue, remainingTimesAddData.StaminaValue + selectedAutoCost.targetValue);
                string text = string.Format(StringValueAssetLoader.Instance["auto_training.remaining_times_add_modal.message_gem"], selectedAutoCost.value, selectedAutoCost.targetValue);
                gem.SetTextBody(text);
                // パス購入状況を見てボタンの表示非表示を設定
                bool hasPass = AutoTrainingUtility.HasStaminaPass();
                buttonBuyPass.interactable = hasPass == false;
            }
            else
            {
                item.gameObject.SetActive(true);
                item.SetItemView(selectedAutoCost.mPointId, itemValue, afterValue);
                item.SetStaminaView(remainingTimesAddData.StaminaValue, remainingTimesAddData.StaminaValue + selectedAutoCost.targetValue);
                // アイテム名を取得し表示
                string itemName = MasterManager.Instance.pointMaster.FindData(selectedAutoCost.mPointId).name;
                string text = string.Format(StringValueAssetLoader.Instance["auto_training.remaining_times_add_modal.message_item"], itemName,selectedAutoCost.value, selectedAutoCost.targetValue);
                item.SetTextBody(text);
            }
        }
        
        private void AcquisitionData()
        {
            // 次に使用するアイテムを取得
            selectedAutoCost = AutoTrainingUtility.GetCostMasterNextRequired(remainingTimesAddData.UserStatus, TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd);
        }

        public void OnPassBuyButton()
        {
            AutoTrainingUtility.OpenPassModal( TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd );
        }

        public void OnClickAddButton()
        {
            OnClickAddButtonAsync().Forget();
        }

        private async UniTask OnClickAddButtonAsync()
        {
            // ジェムの場合数が足りていなければ購入画面を開く
            if (selectedAutoCost.mPointId == ConfigManager.Instance.mPointIdGem)
            {
                if (enoughItem == false)
                {
                    AutoTrainingUtility.OpenGemModal(StringValueAssetLoader.Instance["auto_training.gem_shortage.remaining_add_title"],selectedAutoCost.value);
                    return;
                }
            }
            
            TrainingCureAutoStaminaAPIPost post = new TrainingCureAutoStaminaAPIPost();
            post.id = selectedAutoCost.id;
            TrainingCureAutoStaminaAPIRequest request = new TrainingCureAutoStaminaAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            // ユーザーデータを更新
            if (remainingTimesAddData.OnSetUserStatus != null)
            {
                remainingTimesAddData.OnSetUserStatus(response.userStatus);
            }
            // モーダル閉じる
            await CloseAsync();
        }
        
        public void OnClickTermsTransactionLaw()
        {
            TransactionLowModal.Open();
        }
    }
}