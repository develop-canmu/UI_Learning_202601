using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;


namespace Pjfb
{
    public static class StaminaUtility
    {
        
        public enum StaminaType
        {
            Training      = 1,
            RivalryBattle = 2,
            Colosseum  = 3,
            
            
            AutoTraining  = 10,
        }
        
        private class StaminaData
        {
            public StaminaBase staminaBase = null;
            
            public StaminaData(StaminaBase staminaBase)
            {
                this.staminaBase = staminaBase;
            }
        }
        
        private static Dictionary<long, StaminaData> staminaDatas = new Dictionary<long, StaminaData>();

        /// <summary>スタミナの更新</summary>
        public static UniTask UpdateStaminaAsync()
        {
            return default;
        }
        
        /// <summary>スタミナの更新</summary>
        public static void UpdateStamina(StaminaBase[] staminaList)
        {
            // リストを取得
            foreach(StaminaBase stamina in staminaList)
            {
                staminaDatas[stamina.mStaminaId] = new StaminaData(stamina);
            }
        }
        
        /// <summary>トレーニング用のスタミナ</summary>
        public static long GetTrainingStamina()
        {
            return GetStamina((long)StaminaType.Training);
        }
        
        /// <summary>トレーニング用のスタミナ最大値</summary>
        public static long GetTrainingStaminaMax()
        {
            return GetStaminaMax((long)StaminaType.Training);
        }
        
        /// <summary>トレーニング用のスタミナ</summary>
        public static long GetAutoTrainingStamina()
        {
            return GetStamina((long)StaminaType.AutoTraining);
        }
        
        /// <summary>トレーニング用のスタミナ最大値</summary>
        public static long GetAutoTrainingStaminaMax()
        {
            return GetStaminaMax((long)StaminaType.AutoTraining);
        }
        
        /// <summary>ライバルリー用のスタミナ</summary>
        public static long GetRivalryStamina()
        {
            return GetStamina((long)StaminaType.RivalryBattle);
        }
        
        /// <summary>ライバルリー用のスタミナ最大値</summary>
        public static long GetRivalryStaminaMax()
        {
            return GetStaminaMax((long)StaminaType.RivalryBattle);
        }
        
        /// <summary>ライバルリー用のスタミナ回復時間</summary>
        public static string GetRivalryStaminaCuredTime()
        {
            return GetStaminaCureString((long)StaminaType.RivalryBattle);
        }
        
        /// <summary>スタミナの最大値</summary>
        public static long GetStaminaMax(long mStaminaId)
        {
            if(staminaDatas.TryGetValue(mStaminaId, out StaminaData data))
            {
                return MasterManager.Instance.staminaMaster.FindData(data.staminaBase.mStaminaId).max;
            }
            return 0;
        }
        
        /// <summary>現在のスタミナ</summary>
        public static long GetStamina(long mStaminaId)
        {
            if(staminaDatas.TryGetValue(mStaminaId, out StaminaData data))
            {
                // mStamina
                StaminaMasterObject mStamina = MasterManager.Instance.staminaMaster.FindData(data.staminaBase.mStaminaId);
                // 取得時にすでに最大以上だった場合はそのまま帰す
                if(data.staminaBase.currentStamina >= mStamina.max)
                {
                    return data.staminaBase.currentStamina;
                }
                
                // APIで更新してからの回復量を計算
                DateTime curedDate = DateTimeExtensions.TryConvertToDateTime(data.staminaBase.staminaCuredAt);
                TimeSpan t = (AppTime.Now - curedDate);
                // 経過時間から回復量を計算 (cureType = 2は日跨ぎの全回復なので無視)
                long cureStamina = 0;
                if (mStamina.IsDailyRecovery())
                {
                    // 日替わりでstaminaが最大回復
                    cureStamina = curedDate.Date != AppTime.Now.Date ? mStamina.max : 0;
                }
                else
                {
                    cureStamina = (long)t.TotalSeconds / mStamina.cureSecond;
                }
                
                // 回復量を計算して返す
                return Math.Clamp(data.staminaBase.currentStamina + cureStamina, 0, mStamina.max);
            }
            return 0;
        }

        /// <summary>スタミナ消費なしの残量/summary>
        public static long GetStaminaAddition(long mStaminaId)
        {
            long maxAddStamina = GetMaxStaminaAddition(mStaminaId);
            if(staminaDatas.TryGetValue(mStaminaId, out StaminaData data))
            {
                DateTime lastUsedDate = DateTimeExtensions.TryConvertToDateTime(data.staminaBase.additionUseDate);
                if (lastUsedDate.Date != AppTime.Now.Date)
                {
                    return maxAddStamina;
                }
                
                // 0未満にはしない
                return　System.Math.Max(0, maxAddStamina - data.staminaBase.additionStaminaUsed);
            }
            return maxAddStamina;
        }
        
        /// <summary>スタミナ消費なしの最大量/summary>
        public static long GetMaxStaminaAddition(long mStaminaId)
        {
            long retval = 0;
            foreach (var ownedTag in UserDataManager.Instance.tagObj)
            {
                if (ownedTag.expireAt.TryConvertToDateTime().IsPast(AppTime.Now)) continue;
                var staminaAdditionMasterObject = MasterManager.Instance.staminaAdditionMaster.FindDataByTagAndStaminaId(ownedTag.adminTagId, mStaminaId);
                if (staminaAdditionMasterObject == null) continue;
                retval += staminaAdditionMasterObject.additionValue;
            }
            return retval;
        }

        /// <summary>スタミナ消費なしの残回数/summary>
        public static long GetFreeStaminaRemainingUse(long mStaminaId, long staminaCost)
        {
            if (staminaCost == 0) return 0;
            return GetStaminaAddition(mStaminaId) / staminaCost;
        }

        /// <summary>次のスタミナ回復までの時間/summary>
        public static int GetNextCureSecond(StaminaMasterObject mStamina)
        {
            if(staminaDatas.TryGetValue(mStamina.id, out StaminaData data))
            {
                if (mStamina.IsDailyRecovery())
                {
                    // 日替わりまでの秒を計算   
                    var ts = new TimeSpan(0, 0, 0);
                    var day = AppTime.Now.AddDays(1).Date + ts;
                    var t = day - AppTime.Now;
                    return (int)t.TotalSeconds;
                }
                else
                {
                    // APIで更新してからの回復量を計算
                    DateTime curedDate = DateTimeExtensions.TryConvertToDateTime(data.staminaBase.staminaCuredAt);
                    TimeSpan t = (AppTime.Now - curedDate);
                    // 経過時間から回復量を計算
                    return (int)mStamina.cureSecond - (int)t.TotalSeconds % (int)mStamina.cureSecond;

                }
            }
            return 0;
        }
        
        /// <summary>次のスタミナ回復までの時間/summary>
        public static string GetNextCureSecondString(long mStaminaId)
        {
            StaminaMasterObject mStamina = MasterManager.Instance.staminaMaster.FindData(mStaminaId);
            int second = GetNextCureSecond(mStamina);

            var timeSpan = TimeSpan.FromSeconds(second);
            
            if (mStamina.IsDailyRecovery())
            {
                var timeString = timeSpan.Hours > 0 ?
                    string.Format("{0}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds) :
                    string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                return string.Format(StringValueAssetLoader.Instance["common.stamina.full.recover"],timeString);
            }
            else
            {
                return string.Format("{0}:{1:D2}", (int) timeSpan.TotalMinutes, timeSpan.Seconds);
            }
        }
        
        /// <summary>次のスタミナ回復までの時間/summary>
        public static string GetNextCureSecondString(long mStaminaId, string format)
        {
            StaminaMasterObject mStamina = MasterManager.Instance.staminaMaster.FindData(mStaminaId);
            int second = GetNextCureSecond(mStamina);
            return TimeSpan.FromSeconds(second).ToString(format);
        }

        /// <summary>現在のスタミナ</summary>
        public static string GetStaminaCureString(long mStaminaId)
        {
            return GetStaminaCureString(mStaminaId, @"hh\:mm\:ss");
        }
        
        /// <summary>現在のスタミナ</summary>
        public static string GetStaminaCureString(long mStaminaId, string format)
       {
            var retval = string.Empty;
            if(staminaDatas.TryGetValue(mStaminaId, out StaminaData data))
            {
                retval = data.staminaBase.staminaCuredAt;
            }
            var curedDate = DateTimeExtensions.TryConvertToDateTime(retval);
            TimeSpan timeSpan = curedDate - AppTime.Now;
            if (timeSpan.Ticks < 0) timeSpan = TimeSpan.Zero; 
            retval = timeSpan.ToString(format);

            return retval;
        }

        public static StaminaBase GetStaminaBase(long mStaminaId)
        {
            return staminaDatas.TryGetValue(mStaminaId, out var result) ? result.staminaBase : null;
        }
    }
}