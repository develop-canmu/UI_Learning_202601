using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Club;
using Pjfb.Community;
using Pjfb.Common;
using Pjfb.LockedItem;
using Pjfb.PresentBox;
using Pjfb.Rivalry;
using Pjfb.UserData;
using UniRx;
using UnityEngine;
using Pjfb.Master;
using Pjfb.RecommendChara;
using Pjfb.Event;
using Pjfb.Ranking;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.Training;

namespace Pjfb
{
    public class UIFooter : MonoBehaviour
    {
        [SerializeField]
        private UIFooterButton homeButton = null;
        /// <summary>ホームボタン</summary>
        public UIFooterButton HomeButton { get { return homeButton; } }
        
        [SerializeField]
        private UIFooterButton characterButton = null;
        /// <summary>キャラクターボタン</summary>
        public UIFooterButton CharacterButton { get { return characterButton; } }
        
        [SerializeField]
        private UIFooterButton clubButton = null;
        /// <summary>クラブボタン</summary>
        public UIFooterButton ClubButton { get {return clubButton; } }
        
        [SerializeField]
        private UIFooterButton gachaButton = null;
        /// <summary>ガチャボタン</summary>
        public UIFooterButton GachaButton { get { return gachaButton; } }
        
        [SerializeField]
        private UIFooterButton shopButton = null;
        /// <summary>ショップボタン</summary>
        public UIFooterButton ShopButton { get { return shopButton; } }

        [SerializeField]
        private GameObject clubLockObject = null;

        [SerializeField] private UIFooterClubButton clubButtonObject = null;
        /// <summary>クラブマッチのバルーン制御</summary>
        public UIFooterClubButton ClubButtonObject { get { return clubButtonObject; } }
        
        [SerializeField]
        UIGachaFooterButton gachaFooterButton = null;
        /// <summary>ガチャボタンのバルーンとか</summary>
        public UIGachaFooterButton GachaFooterButton { get { return gachaFooterButton; } }
        
        private UIFooterButton currentButton = null;

        private CharacterBadge characterBadge = null;
        
        private class CharacterBadge: IUserDataHandler
        {
            public void OnUpdatedData()
            {
                AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
            }
        }

        public void Awake()
        {
            characterBadge = new CharacterBadge();
            UserDataManager.Instance.chara.AddHandler(characterBadge);
            UserDataManager.Instance.point.AddHandler(characterBadge);
            UserDataManager.Instance.point.AddHandler(gachaFooterButton);
            UserDataManager.Instance.charaPiece.AddHandler(characterBadge);
            UserDataManager.Instance.supportEquipment.AddHandler(characterBadge);
            UserDataManager.Instance.onUpdateUnlockSystemData += UpdateClubLockState;
            UpdateClubLockState();
        }

        public void OnDestroy()
        {
            UserDataManager.Instance.chara.RemoveHandler(characterBadge);
            UserDataManager.Instance.point.RemoveHandler(characterBadge);
            UserDataManager.Instance.point.RemoveHandler(gachaFooterButton);
            UserDataManager.Instance.charaPiece.RemoveHandler(characterBadge);
            UserDataManager.Instance.supportEquipment.RemoveHandler(characterBadge);
            UserDataManager.Instance.onUpdateUnlockSystemData -= UpdateClubLockState;
        }
        
        /// <summary>フッター表示</summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>フッター非表示</summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>ホームボタンボタン</summary>
        public void OnClickHomeButton()
        {
            OpenPage(PageType.Home);
        }
        
        /// <summary>キャラクターボタン</summary>
        public void OnClickCharacterButton()
        {
            OpenPage(PageType.Character);
        }
        
        /// <summary>クラブボタン</summary>
        public void OnClickClubButton()
        {
            OpenPage(PageType.Club);
        }
        
        /// <summary>ガチャボタン</summary>
        public void OnClickGachaButton()
        {   
            OpenPage(PageType.Gacha);
        }

        /// <summary>ショップボタン</summary>
        public void OnClickShopButton()
        {
            OpenPage(PageType.Shop);
        }
        
        private void OpenPage(PageType page)
        {
            if(AppManager.Instance.UIManager.PageManager.CurrentPageType != page)
            {
                // ページが違うので遷移
                AppManager.Instance.UIManager.PageManager.OpenPage(page, true, null);
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
        
        /// <summary>ボタン表示切り替え</summary>
        public void SwitchButton(PageType pageType)
        {
            // 選択中のボタンを解除する
            if(currentButton != null)
            {
                currentButton.SetActive(false);
            }
            
            // ボタン取得
            switch (pageType)
            {
                case PageType.Home:
                    currentButton = homeButton;
                    break;
                case PageType.Character:
                case PageType.Deck:
                case PageType.Encyclopedia:
                    currentButton = characterButton;
                    break;
                case PageType.Club:
                    currentButton = clubButton;
                    break;
                case PageType.Gacha:
                    currentButton = gachaButton;
                    break;
                case PageType.Shop:
                    currentButton = shopButton;
                    break;
                default: 
                    currentButton = null;
                    break;
            }
            
            if(currentButton != null)
            {
                // ボタンを選択中にする
                currentButton.SetActive(true);
            }
        }

        public void UpdateHomeBadge()
        {
            AppManager.Instance.UIManager.Footer.HomeButton.SetNotificationBadge(
                isActive: MissionManager.finishedMissionCount > 0 ||
                          PresentBoxManager.unreceivedGiftCount > 0 || 
                          LockedItemManager.unreceivedGiftCount > 0 || 
                          CommunityManager.showHomeBadge ||
                          RecommendCharaManager.showHomeBadge ||
                          EventManager.Instance.showHomeBadge ||
                          RivalryManager.GetHomeBadgeFlag() ||
                          TrainingManager.AutoTrainingCompleteCount > 0||
                          RankingManager.IsShowHomeBadge
            );
        }

        public void OnClickClubLock(){
            ClubUtility.OpenClubLockModal();
        }

        public void UpdateClubLockState(){
            clubLockObject?.gameObject.SetActive( !UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId) );
            UpdateClubBadge();
        }

        public void UpdateClubBadge(){
            var isUnlock = UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId);
            if( !isUnlock ) {
                AppManager.Instance.UIManager.Footer.ClubButton.SetBalloonBadge(string.Empty);
                AppManager.Instance.UIManager.Footer.ClubButton.SetNotificationBadge(false);
                return;
            }
            
            string clubBalloonText = UserDataManager.Instance.user.gMasterId == 0 ? StringValueAssetLoader.Instance["footer.club_balloon"] : string.Empty;
            AppManager.Instance.UIManager.Footer.ClubButton.SetBalloonBadge(clubBalloonText);
            // クラブマッチのバルーン表示
            AppManager.Instance.UIManager.Footer.ClubButtonObject.UpdateClubMatchBalloon();

            var clubNotificationData = LocalSaveManager.saveData.clubCheckNotificationData;
            var isViewBadge = clubNotificationData.solicitation.IsNotification()
            || clubNotificationData.requestJoin.IsNotification()
            || clubNotificationData.sendSolicitation.IsNotification()
            || Pjfb.Community.CommunityManager.unViewedClubChatCount + Pjfb.Community.CommunityManager.unViewedClubInfoCount > 0 ;

                        
            AppManager.Instance.UIManager.Footer.ClubButton.SetNotificationBadge(isViewBadge);
        }
    }
}
