using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using System.Globalization;

namespace Pjfb.Networking.App.Request 
{
    
    public enum BillingRewardBonusCategory
    {
        None = 0,    // 
        Normal = 1,    // 通常パック
        Vip = 2,    // Vipパック
        Pass = 3,    // パス
        Recommend = 4,    // 期間限定
        BattlePass = 5,    // バトルパス
        QuestBattlePass = 6,    // クエストバトルパス
        Beginner = 7,    // 初心者パック
        Secret = 8,     //シークレット
    }
    
    public enum BillingRewardBonusSubCategory
    {
        Daily = 1,    // 毎日
        Weekly = 2,    // 週間
        Monthly = 3,    // 月間
        Secret = 4,    // シークレット
        Limited = 5,    // 限定
        NoUse = 6,    // 未使用
        Event = 7,    // イベント
        Gem = 8,    // ジェム
    }
    
    public partial class BillingRewardBonusDetail
    {
        public BillingRewardBonusCategory GetCategory()
        {
            return (BillingRewardBonusCategory)category;
        }

        public BillingRewardBonusSubCategory GetSubCategory()
        {
            return (BillingRewardBonusSubCategory)subCategory;
        }
        
        public bool IsStepUpLocked(IEnumerable<BillingRewardBonusDetail> sameGroupBonus)
        {
            if (sameGroupBonus == null) return false;
            
            return stepGroup != 0 && buyCount <= 0 &&
                   sameGroupBonus.Count(groupBonus => groupBonus.stepNumber < stepNumber) > 0　&&
                   !sameGroupBonus.Where(groupBonus => groupBonus.stepNumber < stepNumber).All(groupBonus => groupBonus.buyCount > 0);
        }
        
        
        /// <summary>
        /// buyLimitが0で購入が可能か
        /// </summary>
        public bool IsPurchaseUnlimitedCanBuy()
        {
            if (GetCategory() == BillingRewardBonusCategory.Secret)
            {
                return buyLimit == 0 && saleIntroductionActiveFlg;
            }
            return buyLimit == 0;
        }
        
    }
    
}
