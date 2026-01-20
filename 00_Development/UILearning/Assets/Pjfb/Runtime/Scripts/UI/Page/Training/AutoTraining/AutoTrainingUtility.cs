using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using Pjfb.UserData;

namespace Pjfb.Training
{
    public static class AutoTrainingUtility
    {
        
        /// <summary>時間を更新する間隔</summary>
        public const float UpdateTimeInterval = 0.5f;
        
        private static long timeMinute = 0;
        /// <summary>自動トレーニング完了時間</summary>
        public static long TimeMinute{get{return timeMinute;}set{timeMinute = value;}}
        
        /// <summary>時間表示の文字列に変換</summary>
        public static string ToTimeString(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        
        /// <summary>
        /// キャラ所持数が上限に達しているかチェック
        /// </summary>
        public static bool CheckLimitCharacter()
        {
            // キャラの所持数をチェック
            if(UserDataManager.Instance.charaVariable.data.Values.Count >= ConfigManager.Instance.uCharaVariableCountMax)
            {
                // Ok
                ConfirmModalButtonParams positiveButtonParams = new ConfirmModalButtonParams( StringValueAssetLoader.Instance["character.character_list"], (modal)=>
                {
                    // モーダルを全て閉じる
                    AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(m=>true);
                    // キャラページへ
                    CharacterPage.Data data = new CharacterPage.Data(CharacterPageType.SuccessCharaList);
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Character, true, data);
                    modal.Close();
                } );
                
                // Cancel
                ConfirmModalButtonParams negativeButtonParams = new ConfirmModalButtonParams( StringValueAssetLoader.Instance["common.cancel"], (modal)=>
                {
                    modal.Close();
                });
                
                ConfirmModalData modalData = new ConfirmModalData(StringValueAssetLoader.Instance["auto_training.character_max_modal.title"], StringValueAssetLoader.Instance["auto_training.character_max_modal.msg"], string.Empty, positiveButtonParams, negativeButtonParams);
                // モーダルを開く
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, modalData);
                return true;
            }
            
            return false;
        }
        
        private static bool HasPass(TrainingAutoCostMasterObject.CostType costType)
        {

            switch(costType)
            {
                case TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd:
                {
                    foreach(StaminaAdditionMasterObject mStamina in MasterManager.Instance.staminaAdditionMaster.values)
                    {
                        if(mStamina.mStaminaId == (long)StaminaUtility.StaminaType.AutoTraining)
                        {
                            if(UserDataManager.Instance.tag.Contains(mStamina.adminTagId))
                            {
                                return true;
                            }
                        }
                    }
                    break;
                }
                
                default:
                {
                    foreach(TrainingAutoCostMasterObject mCost in MasterManager.Instance.trainingAutoCostMaster.values)
                    {
                        if(mCost.type == costType)
                        {
                            if(string.IsNullOrEmpty(mCost.adminTagIdList))continue;
                            long[] ids = JsonHelper.FromJson<long>(mCost.adminTagIdList);
                            // ユーザーが所持している
                            foreach(long id in ids)
                            {
                                if(UserDataManager.Instance.tag.Contains(id))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                }
            }
            

            
            return false;
        }
        
        /// <summary>時短パスを持っているか</summary>
        public static bool HasTimeSavePass()
        {
            return HasPass( TrainingAutoCostMasterObject.CostType.Shortening );
        }
        
        /// <summary>回数増加パスを持っているか</summary>
        public static bool HasStaminaPass()
        {
            return HasPass( TrainingAutoCostMasterObject.CostType.AutoTrainingRemainingTimesAdd );
        }
        
        /// <summary>即時完了パスを持っているか</summary>
        public static bool HasCompleteImmediatelyPass()
        {
            return HasPass( TrainingAutoCostMasterObject.CostType.CompleteImmediately );
        }
        
        /// <summary>パス導線を開く</summary>
        public static void OpenPassModal(TrainingAutoCostMasterObject.CostType passType)
        {
            OpenPassModalAsync(passType).Forget();
        }
        
        /// <summary>パス導線を開く</summary>
        public static UniTask<CruFramework.Page.ModalWindow> OpenPassModalAsync(TrainingAutoCostMasterObject.CostType passType)
        {
            AutoTrainingPassEffectiveModal.Arguments args = new AutoTrainingPassEffectiveModal.Arguments(passType);
            return AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoTrainingPassEffective, args);
        }
        
        /// <summary>自動トレーニングが解放されているか</summary>
        public static bool IsUnLockAutoTraining()
        {
            return UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining) || SystemUnlockDataManager.Instance.IsUnlockingSystem((long)SystemUnlockDataManager.SystemUnlockNumber.AutoTraining);
        }
        
        /// <summary>次に使用する可能性のあるコストデータ一覧を取得する</summary>
        public static TrainingAutoCostMasterObject[] GetExpectedTrainingAutoCostMastersNextRequired(TrainingAutoUserStatus userStatus ,TrainingAutoCostMasterObject.CostType costType)
        {
            // 使用済みのマスタ一覧
            List<TrainingAutoCostMasterObject> usedMTrainingAutoCostList = new List<TrainingAutoCostMasterObject>();
            foreach(long id in userStatus.mTrainingAutoCostIdList)
            {
                TrainingAutoCostMasterObject mTrainingAutoCost = MasterManager.Instance.trainingAutoCostMaster.FindData(id);
                // タイプが一致
                if(mTrainingAutoCost.type == costType)
                {
                    usedMTrainingAutoCostList.Add(mTrainingAutoCost);
                }
            }
            
            // 次回実行回数
            long nextCount = 0;
            foreach(TrainingAutoCostMasterObject mTrainingAutoCost in usedMTrainingAutoCostList)
            {
                // 大きい場合は更新
                if(mTrainingAutoCost.count > nextCount)
                {
                    nextCount = mTrainingAutoCost.count;
                }
            }
            // +1する
            nextCount++;
            
            List<TrainingAutoCostMasterObject> result = new List<TrainingAutoCostMasterObject>();
            foreach(TrainingAutoCostMasterObject mTrainingAutoCost in MasterManager.Instance.trainingAutoCostMaster.values)
            {
                // タイプが違う
                if(mTrainingAutoCost.type != costType) continue;
                // 使用済み
                if(usedMTrainingAutoCostList.Contains(mTrainingAutoCost)) continue;
                // 実行回数の指定がされている
                if(mTrainingAutoCost.count > 0)
                {
                    // 次の使用回数でない
                    if(mTrainingAutoCost.count != nextCount) continue;
                }
                // 結果に追加
                result.Add(mTrainingAutoCost);
            }
        
            return result
                // 優先度順にソート
                .OrderByDescending(m => m.priority)
                // 配列に変換
                .ToArray();
        }
        
        /// <summary>確認モーダルを開く</summary>
        public static void OpenConfirmModal(string title, string message, string buttonText, Action onClose = null)
        {
            OpenConfirmModalAsync(title, message, buttonText, onClose).Forget();
        }
        
        /// <summary>確認モーダルを開く</summary>
        public static UniTask<CruFramework.Page.ModalWindow> OpenConfirmModalAsync(string title, string message, string buttonText, Action onClose)
        {
            if(string.IsNullOrEmpty(buttonText))
            {
                buttonText = StringValueAssetLoader.Instance["common.ok"];
            }
            
            ConfirmModalData data = new ConfirmModalData();
            data.Title = title;
            data.Message = message;
            data.PositiveButtonParams = new ConfirmModalButtonParams(buttonText, async (m)=>
            {
                await m.CloseAsync();
                
                if(onClose != null)
                {
                    onClose();
                }
            });
            
            return AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
        }
        
        /// <summary>確認モーダルを開く</summary>
        public static void OpenNegativeConfirmModal(string title, string message, string buttonText, Action onClose = null)
        {
            OpenNegativeConfirmModalAsync(title, message, buttonText, onClose).Forget();
        }
        
        /// <summary>確認モーダルを開く</summary>
        public static UniTask<CruFramework.Page.ModalWindow> OpenNegativeConfirmModalAsync(string title, string message, string buttonText, Action onClose)
        {
            if(string.IsNullOrEmpty(buttonText))
            {
                buttonText = StringValueAssetLoader.Instance["common.close"];
            }
            
            ConfirmModalData data = new ConfirmModalData();
            data.Title = title;
            data.Message = message;
            data.NegativeButtonParams = new ConfirmModalButtonParams(buttonText, async (m)=>
            {
                await m.CloseAsync();
                
                if(onClose != null)
                {
                    onClose();
                }
            });
            
            return AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
        }
        
        /// <summary>ジェム不足のモーダルを開く</summary>
        public static void OpenGemModal(string title, long needCount)
        {
            OpenGemModalAsync(title, needCount).Forget();
        }
        
        /// <summary>ジェム不足のモーダルを開く</summary>
        public static UniTask<CruFramework.Page.ModalWindow> OpenGemModalAsync(string title, long needCount)
        {
            PointShortageModal.Arguments args = new PointShortageModal.Arguments();
            
            // ジェム
            PointMasterObject mPoint = MasterManager.Instance.pointMaster.FindData( ConfigManager.Instance.mPointIdGem);
            // ジェムのId
            args.PointId = mPoint.id;
            // タイトル
            args.Title = title;
            // メッセージ
            args.Message = string.Format(StringValueAssetLoader.Instance["auto_training.gem_shortage"], mPoint.name);
            // 必要数
            args.NeedCount = needCount;
            // 所持数
            args.CurrentCount = UserDataManager.Instance.GetPointValue(mPoint.id);
            
            // モーダルを開く
            return AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.PointShortage, args);
        }

        /// <summary>使用するアイテムの取得</summary>
        public static TrainingAutoCostMasterObject GetCostMasterNextRequired(TrainingAutoUserStatus userStatus ,TrainingAutoCostMasterObject.CostType costType)
        {
            // 可能性のあるマスタ一覧
            TrainingAutoCostMasterObject[] expectedTrainingAutoCostMasters = GetExpectedTrainingAutoCostMastersNextRequired(userStatus, costType);
            
            foreach(TrainingAutoCostMasterObject mTrainingAutoCost in expectedTrainingAutoCostMasters)
            {
                // タグが設定されている
                if(string.IsNullOrEmpty(mTrainingAutoCost.adminTagIdList) == false)
                {
                    // パース
                    long[] ids = JsonHelper.FromJson<long>(mTrainingAutoCost.adminTagIdList);
                    foreach(long id in ids)
                    {
                        // タグを所持してない
                        if(UserDataManager.Instance.tag.Contains(id) == false) continue;
                        
                        // アイテムを持ってる
                        if(HasPoint(mTrainingAutoCost))
                        {
                            return mTrainingAutoCost;
                        }
                    }
                    continue;
                }
                
                // アイテムを所持していれば返却
                if(HasPoint(mTrainingAutoCost))
                {
                    return mTrainingAutoCost;
                }
                
                // ジェムの場合は無条件で返却
                if(mTrainingAutoCost.mPointId == ConfigManager.Instance.mPointIdGem)
                {
                    return mTrainingAutoCost;
                }
            }
            
            // 該当のデータがない
            return null;
        }
        
        /// <summary>タグ効果の合計値を取得</summary>
        public static long GetTotalFreeAutoCostWithTag(TrainingAutoCostMasterObject.CostType costType)
        {
            long result = 0;
            
            foreach(TrainingAutoCostMasterObject mTrainingAutoCost in MasterManager.Instance.trainingAutoCostMaster.values)
            {
                // タイプが違う
                if(mTrainingAutoCost.type != costType) continue;
                // 無料？
                if(mTrainingAutoCost.value > 0)continue;
                
                // タグがない場合
                if(string.IsNullOrEmpty(mTrainingAutoCost.adminTagIdList))
                {
                    // 追加
                    result++;
                }
                else
                {
                    // パース
                    long[] ids = JsonHelper.FromJson<long>(mTrainingAutoCost.adminTagIdList);
                    foreach(long id in ids)
                    {
                        // タグを所持してない
                        if(UserDataManager.Instance.tag.Contains(id) == false)continue;
                        // 追加
                        result++;
                        
                        break;
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>アイテムを所持しているか</summary>
        private static bool HasPoint(TrainingAutoCostMasterObject mTrainingAutoCost)
        {
            // 必要個数が0
            if(mTrainingAutoCost.value <= 0) return true;
            
            // 所持アイテム情報
            UserDataPoint point = UserDataManager.Instance.point.Find(mTrainingAutoCost.mPointId);
            // 所持していない
            if(point == null) return false;
            // 所持数が足りてない
            if(point.value < mTrainingAutoCost.value) return false;
            
            return true;
        }
        
        /// <summary>自動トレーニング完了</summary>
        public static async UniTask FinishAutoTrainingAsync(long slot, TrainingAutoUserStatus userStatus, TrainingAutoPendingStatus[] pendingStatuses)
        {
            // キャラの所持数をチェック
            if(CheckLimitCharacter())return;
            
            // Pending
            TrainingAutoPendingStatus pendingStatus = null;
            // スロットのデータを探す
            foreach(TrainingAutoPendingStatus pending in pendingStatuses)
            {
                if(pending.slotNumber == slot)
                {
                    pendingStatus = pending;
                    break;
                }
            }
            
            await FinishAutoTrainingAsync(slot, userStatus, pendingStatus);
        }
        
        /// <summary>自動トレーニング完了</summary>
        public static async UniTask FinishAutoTrainingAsync(long slot, TrainingAutoUserStatus userStatus, TrainingAutoPendingStatus pendingStatus)
        {
            // キャラの所持数をチェック
            if(CheckLimitCharacter())return;
            
            
            // Request
            TrainingFinishAutoAPIRequest request = new TrainingFinishAutoAPIRequest();
            // Post
            TrainingFinishAutoAPIPost post = new TrainingFinishAutoAPIPost();
            // スロット番号
            post.slotNumber = slot;
            request.SetPostData(post);
            
            // API
            await APIManager.Instance.Connect(request);
            // Response
            TrainingFinishAutoAPIResponse res = request.GetResponseData();
            TrainingMainArguments mainArgs = new TrainingMainArguments(res, pendingStatus, userStatus);
            // モーダルを全て閉じる
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            // トレーニング結果画面へ
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Training, false, mainArgs);
        }
    }
}