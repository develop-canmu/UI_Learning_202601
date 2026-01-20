using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.ResourceManagement;
using Pjfb.ClubMatch;
using Pjfb.ClubRoyal;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using TMPro;
using UniRx;
using UnityEngine;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;

namespace Pjfb
{
    public class UIHeader : MonoBehaviour
    {
        [SerializeField]
        private DeckRankImage deckRankImage = null;
        
        [SerializeField]
        private TextMeshProUGUI gemPossessionText = null;
        
        [SerializeField]
        private OmissionTextSetter gemPossessionOmissionTextSetter = null;
        
        [SerializeField]
        private TextMeshProUGUI maxTotalPowerText = null;
        
        [SerializeField]
        private OmissionTextSetter totalPowerOmissionTextSetter = null;
        
        [SerializeField]
        private GameObject menuBadge = null;

        [SerializeField] 
        private ClubRoyalBanner clubRoyalBanner = null;
        
        private RectTransform _rectTransform = null;
        // RectTransform
        private RectTransform rectTransform
        {
            get
            {
                if(_rectTransform == null)
                {
                    _rectTransform = GetComponent<RectTransform>();
                }
                return _rectTransform;
            }
        }
        
        private ResourcesLoader resourcesLoader = new ResourcesLoader();

        /// <summary> データ更新イベント登録 </summary>
        public void SubscribeUpdateEvents()
        {
            // 課金通貨
            UserDataManager.Instance.point.gem.Subscribe(value =>
            {
                gemPossessionText.text = new BigValue(value).ToDisplayString(gemPossessionOmissionTextSetter.GetOmissionData());
            })
            .AddTo(gameObject);
            
            // 総合戦力値
            UserDataManager.Instance.user.maxCombatPower.Subscribe(value =>
            {
                maxTotalPowerText.text = value.ToDisplayString(totalPowerOmissionTextSetter.GetOmissionData());
            })
            .AddTo(gameObject);
            
            // デッキランク
            UserDataManager.Instance.user.maxDeckRank.Subscribe(value =>
            {
                deckRankImage.SetTexture(value, resourcesLoader);
            })
            .AddTo(gameObject);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>メニュー</summary>
        public void OnClickMenuButton()
        {
            MenuManager.OnClickMenuButton();
        }
        
        /// <summary>課金通貨購入ボタン</summary>
        public void OnClickGemPurchaseButton()
        {
            if(AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.Shop)
            {
                // ページが違うので遷移
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Shop, true, null);
            }
            else
            {
                // 同じページ内
                if(AppManager.Instance.UIManager.PageManager.CurrentPageObject is IFooterPage footerPage)
                {
                    footerPage.OnOpenPage();
                }
            }
        }

        public void UpdateMenuBadge()
        {
            menuBadge.SetActive(MenuManager.IsTrainerCardBadge());
        }

        //// <summary> クラブロワイヤルのバナーセット </summary>
        public void UpdateClubRoyalBanner()
        {
            clubRoyalBanner.gameObject.SetActive(true);
            // クラブロワイヤルの開催情報を取得
            LeagueMatchInfo matchInfo = LeagueMatchUtility.GetLeagueMatchInfo(ColosseumClientHandlingType.ClubRoyal);
            clubRoyalBanner.SetView(matchInfo);
        }
        
        private void OnDestroy()
        {
            resourcesLoader.Release();
        }
    }
}
