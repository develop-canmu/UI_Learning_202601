using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaTopPageView : MonoBehaviour
    {
        [SerializeField]
        private GachaTopKeyVisualUI gachaKeyVisualUI = null;
        /// <summary>ガチャキービジュアル</summary>
        public GachaTopKeyVisualUI GachaKeyVisualUI { get { return gachaKeyVisualUI; } }
        
        [SerializeField]
        private GachaTopTicketBannerScrollUI ticketBannerScrollUI = null;
        /// <summary>チケットバナースクロール</summary>
        public GachaTopTicketBannerScrollUI TicketBannerScrollUI { get { return ticketBannerScrollUI; } }
        
        [SerializeField]
        private ScrollBanner gachaBannerScroll = null;
        /// <summary>ガチャバナースクロール</summary>
        public ScrollBanner GachaBannerScroll { get { return gachaBannerScroll; } } 
        
        [SerializeField]
        private GachaTopDrawGachaButtonUI[] drawGachaButtons = null;
        /// <summary>ガチャ実行ボタン</summary>
        public GachaTopDrawGachaButtonUI[] DrawGachaButtons { get { return drawGachaButtons; } }
        
        [SerializeField]
        private GachaTopSubPointUI subPointUI = null;
        /// <summary>代替ポイント所持数UI</summary>
        public GachaTopSubPointUI SubPointUI { get { return subPointUI; } }

        [SerializeField]
        private GachaTopExchangeUI exchangeUI = null;
        /// <summary>ポイント交換所UI</summary>
        public GachaTopExchangeUI ExchangeUI { get { return exchangeUI; } }

        [SerializeField]
        private GachaPickUpButton pickUpButton = null;
        /// <summary>ピックアップボタン</summary>
        public GachaPickUpButton PickUpButton { get { return pickUpButton; } }

        [SerializeField]
        private UIButton probabilityButton = null;
        /// <summary>提供割合ボタン</summary>
        public UIButton ProbabilityButton { get { return probabilityButton; } }

        [SerializeField]
        private UIButton detailButton = null;
        /// <summary>ガチャ詳細ボタン</summary>
        public UIButton DetailButton { get { return detailButton; } }

        [SerializeField]
        private GachaTopRushUI rushUI = null;
        /// <summary>代替ポイント所持数UI</summary>
        public GachaTopRushUI RushUI { get { return rushUI; } }

        [SerializeField]
        private UIButton boxGachaDetailButton = null;
        /// <summary>ピックアップボタン</summary>
        public UIButton BoxGachaDetailButton { get { return boxGachaDetailButton; } }
        
        [SerializeField] private GachaRequiredPointView alternativePointView;
        //// <summary>仮想ポイント所持数UI</summary>
        public GachaRequiredPointView AlternativePointView{get {return alternativePointView;}}
    }
}