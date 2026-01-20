using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using Pjfb.Shop;
using Pjfb.UserData;

namespace Pjfb.Training
{
    public class AutoTrainingPassEffectiveModal : ModalWindow
    {
        public class Arguments
        {
            private TrainingAutoCostMasterObject.CostType type;
            /// <summary>パスの種類</summary>
            public TrainingAutoCostMasterObject.CostType Type{get{return type;}}
            
            public Arguments(TrainingAutoCostMasterObject.CostType type)
            {
                this.type = type;
            }
        }
        
        private struct PassData
        {
            private long tagId;
            /// <summary>タグ</summary>
            public long TagId{get{return tagId;}}
            
            private long value;
            /// <summary>直</summary>
            public long Value{get{return value;}}
            
            public PassData(long tagId, long value)
            {
                this.tagId = tagId;
                this.value = value;
            }
        }

        [SerializeField]
        private TMP_Text messageText = null;
        [SerializeField]
        private CancellableRawImage bannerImage = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arguments = (Arguments)args;
            
            
            int passType = (int)arguments.Type;
            
            // マスタを取得
            PassData passData = GetPassData(arguments.Type);
            
            // 効果メッセージ
            string msg = string.Format(StringValueAssetLoader.Instance[$"auto_training.pass.message.{passType}"], passData.Value);
            // メッセージ表示
            messageText.text = string.Format(StringValueAssetLoader.Instance["auto_training.pass.message"], msg);
            // バナー
            bannerImage.SetTexture( ResourcePathManager.GetPath("AutoTrainingPassBanner", passData.TagId));
            // 
            return base.OnPreOpen(args, token);
        }
        
        
        private PassData GetPassData(TrainingAutoCostMasterObject.CostType costType)
        {

            switch(costType)
            {
                case TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd:
                {
                    foreach(StaminaAdditionMasterObject mStamina in MasterManager.Instance.staminaAdditionMaster.values)
                    {
                        if(mStamina.mStaminaId == (long)StaminaUtility.StaminaType.AutoTraining)
                        {
                            return new PassData(mStamina.adminTagId, mStamina.additionValue);
                        }
                    }
                    break;
                }
                
                // 即時完了の場合はレコード数＋無料分のみカウント
                case TrainingAutoCostMasterObject.CostType.CompleteImmediately:
                {
                    long tag = -1;
                    long count = 0;
                    foreach(TrainingAutoCostMasterObject mCost in MasterManager.Instance.trainingAutoCostMaster.values)
                    {
                        if(mCost.type == costType)
                        {
                            // 無料？
                            if(mCost.value != 0 || mCost.count != 0)continue;
                            // ショップへの導線がない
                            if(string.IsNullOrEmpty(mCost.mBillingRewardBonusIdList))continue;
                            long[] bonusIds = JsonHelper.FromJson<long>(mCost.adminTagIdList);
                            if(bonusIds.Length <= 0)continue;
                            
                            // タグをチェック
                            if(string.IsNullOrEmpty(mCost.adminTagIdList))continue;
                            List<long> tags = new List<long>(JsonHelper.FromJson<long>(mCost.adminTagIdList));
                            if(tags.Count <= 0)continue;
                            // 調べるタグ
                            if(tag < 0)
                            {
                                tag = tags[0];
                            }
                            else
                            {
                                // 対象のタグが入っていない
                                if(tags.Contains(tag) == false)continue;
                            }
                            
                            count++;
                        }
                    }
                    
                    return new PassData(tag, count);
                }
                
                default:
                {
                    foreach(TrainingAutoCostMasterObject mCost in MasterManager.Instance.trainingAutoCostMaster.values)
                    {
                        if(mCost.type == costType)
                        {
                            // ショップへの導線がない
                            if(string.IsNullOrEmpty(mCost.mBillingRewardBonusIdList))continue;
                            long[] bonusIds = JsonHelper.FromJson<long>(mCost.adminTagIdList);
                            if(bonusIds.Length <= 0)continue;
                            
                            // タグをチェック
                            if(string.IsNullOrEmpty(mCost.adminTagIdList))continue;
                            long[] tags = JsonHelper.FromJson<long>(mCost.adminTagIdList);
                            return new PassData(tags[0], mCost.targetValue);
                        }
                    }
                    
                    break;
                }
            }
            
            CruFramework.Logger.LogError("パスが見つかりません : " + costType);
            return new PassData(0, 0);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnShopButton()
        {
            // モーダルを全て閉じる
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            // 引数
            ShopPageArgs args = new ShopPageArgs();
            // パスを開く
            args.selectedTab = ShopTabSheetType.Pass;
            // ショップページへ
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, args);
        }
    }
}