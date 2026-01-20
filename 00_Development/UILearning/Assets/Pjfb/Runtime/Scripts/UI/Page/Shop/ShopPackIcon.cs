using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Shop
{
    public class ShopPackIcon : MonoBehaviour
    {
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject paidObject;
        
        /// <summary>データセット</summary>
        public void SetView(Networking.App.Request.PrizeJsonWrap prizeJson)
        {
            prizeJsonView.SetView(prizeJson);
            paidObject.SetActive(false);
        }
        
        /// <summary>データセット</summary>
        public void SetView(Master.PrizeJsonWrap prizeJson, bool isPaid)
        {
            prizeJsonView.SetView(prizeJson);
            paidObject.SetActive(isPaid);
        }
    }
}