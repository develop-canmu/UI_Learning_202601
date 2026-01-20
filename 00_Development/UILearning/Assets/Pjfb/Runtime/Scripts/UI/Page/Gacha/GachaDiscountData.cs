using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaDiscountData
    {
        private long gachaDiscountId = 0;
        /// <summary>割引Id</summary>
        public long GachaDiscountId { get { return gachaDiscountId; } }
        
        private string description = string.Empty;
        /// <summary>説明文</summary>
        public string Description { get { return description; } }

        private long price = 0;
        /// <summary>値段</summary>
        public long Price { get { return price; } }

        private long restCount = 0;
        /// <summary>使用回数</summary>
        public long RestCount { get { return restCount; } }

        /// <summary>有効かどうか</summary>
        public bool IsEnable { get { return restCount > 0; } }

        public GachaDiscountData(TopDiscount topDiscount)
        {
            gachaDiscountId = topDiscount.id;
            description = topDiscount.description;
            price = topDiscount.price;
            restCount = topDiscount.restCount;
        }
        
        /// <summary>使用回数減らす</summary>
        public void DecrementRestCount()
        {
            restCount--;
        }
    }
}
