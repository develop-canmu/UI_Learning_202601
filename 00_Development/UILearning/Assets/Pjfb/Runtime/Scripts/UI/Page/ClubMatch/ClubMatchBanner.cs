using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.ClubMatch
{
    public class ClubMatchBanner : MonoBehaviour
    {
        public class Data
        {
            private ColosseumSeasonData seasonData = null;
            /// <summary>シーズン情報</summary>
            public ColosseumSeasonData SeasonData
            {
                get { return seasonData; }
                set
                {
                    seasonData = value;
                    if(HasSeasonHome)
                    {
                        // 開始時間
                        SeasonStartAt = seasonData.SeasonHome.startAt.TryConvertToDateTime();
                        // 終了時間
                        SeasonEndAt = seasonData.SeasonHome.endAt.TryConvertToDateTime();
                    }
                }
            }
            
            /// <summary>シーズン開始日</summary>
            public DateTime SeasonStartAt { get; private set; }
            /// <summary>シーズン終了日</summary>
            public DateTime SeasonEndAt { get; private set; }

            /// <summary>次シーズン開始日</summary>
            public DateTime NextSeasonStartAt { get; set; }
            /// <summary>次シーズン開始日</summary>
            public DateTime NextSeasonEndAt { get; set; }
            
            public bool HasSeasonHome
            {
                get
                {
                    if(seasonData == null) return false;
                    if(seasonData.SeasonHome == null) return false;
                    return true;
                }
            }
        }
        
        private static readonly string TermStringFormat = "MM/dd HH:mm";
        
        [SerializeField]
        private Sprite unaffiliatedSprite = null;
        
        [SerializeField]
        private Sprite affiliatedSprite = null;
        
        [SerializeField]
        private Image badgeImage = null;
        
        [SerializeField]
        private Image bannerImage = null;
        
        [SerializeField]
        private TextMeshProUGUI messageText = null;
        
        [SerializeField]
        private TextMeshProUGUI termText = null;
        
        [SerializeField]
        private GameObject termRoot = null;
        
        [SerializeField]
        private Material defaultTextMaterial = null;
        
        [SerializeField]
        private Material noClubTextMaterial = null;

        [SerializeField] 
        private TMP_FontAsset textFontAsset = null;
        
        private Data data = null;

        private bool isSeasonEnded = false;
        private bool isSeasonStarted = false;
        
        private ResourcesLoader resourcesLoader = new ResourcesLoader();
        
        private DateTime Now
        {
            get { return AppTime.Now; }
        }

        /// <summary>クラブに参加済み</summary>
        private bool IsJoinedClub
        {
            get { return UserDataManager.Instance.user.gMasterId != 0; }
        }
        
        public void SetView(Data data)
        {
            this.data = data;
            
            isSeasonStarted = false;
            isSeasonEnded = false;

            // マテリアル適用
            messageText.fontMaterial = (data == null || !IsJoinedClub) ? noClubTextMaterial : defaultTextMaterial;
            messageText.font = textFontAsset;
            
            // TODO クラブマッチが設定されてない
            if(data == null)
            {
                // 画像差し替え
                // bannerImage.spriteだと画像が変わらない時があるのでoverrideSpriteに指定
                bannerImage.overrideSprite = unaffiliatedSprite;
                // 
                messageText.text = string.Empty;
                // 開催中バッジ非表示
                badgeImage.gameObject.SetActive(false);
                // 開催期間非表示
                termRoot.SetActive(false);
                return;
            }
            
            // マッチングしてない && クラブに参加してない
            if(data.SeasonData.IsMatchingFailed && !IsJoinedClub)
            {
                // 画像差し替え
                // bannerImage.spriteだと画像が変わらない時があるのでoverrideSpriteに指定
                bannerImage.overrideSprite = unaffiliatedSprite;
                // クラブ未所属
                messageText.text = StringValueAssetLoader.Instance["menu.profile.no_club"];
                // 開催中バッジ非表示
                badgeImage.gameObject.SetActive(false);
                // 開催期間非表示
                termRoot.SetActive(false);
                return;
            }
            
            // シーズン中
            if(data.HasSeasonHome && data.SeasonData.IsOnSeason)
            {
                // 画像差し替え
                // bannerImage.spriteだと画像が変わらない時があるのでoverrideSpriteに指定
                bannerImage.overrideSprite = affiliatedSprite;
                // 開催中バッジ表示
                badgeImage.gameObject.SetActive(true);
                // 開催期間表示
                termRoot.SetActive(true);
                // 開催期間
                DateTime startAt = data.SeasonStartAt;
                DateTime endAt = data.SeasonEndAt;
                termText.text = string.Format(StringValueAssetLoader.Instance["clubmatch.header.banner.term"], startAt.ToString(TermStringFormat), endAt.ToString(TermStringFormat));
                
                // クラブマッチに参加できなかった
                if(data.SeasonData.IsMatchingFailed)
                {
                    messageText.text = StringValueAssetLoader.Instance["clubmatch.header.banner.message_3"];
                }
            }
            // シーズン外
            else
            {
                // 画像差し替え
                // bannerImage.spriteだと画像が変わらない時があるのでoverrideSpriteに指定
                bannerImage.overrideSprite = affiliatedSprite;
                // 開催中バッジ非表示
                badgeImage.gameObject.SetActive(false);
                // 開催期間表示
                termRoot.SetActive(true);
                // 開催期間
                termText.text = string.Format(StringValueAssetLoader.Instance["clubmatch.header.banner.term"], data.NextSeasonStartAt.ToString(TermStringFormat), data.NextSeasonEndAt.ToString(TermStringFormat));
            }
        }

        private void UpdateView()
        {
            if(data.HasSeasonHome)
            {
                // シーズン中
                if(DateTimeExtensions.IsWithinPeriod(Now, data.SeasonStartAt, data.SeasonEndAt + TimeSpan.FromSeconds(1)))
                {
                    if(data.SeasonData.IsMatchingFailed) return;
                    // シーズン終了まで<br>あと{0}
                    messageText.text = string.Format(StringValueAssetLoader.Instance["clubmatch.header.banner.message_2"], data.SeasonEndAt.GetRemainingString(Now));
                }
                
                // シーズンが終了した
                if((data.SeasonEndAt + TimeSpan.FromSeconds(1)).IsPast(Now))
                {
                    OnEndSeason();
                }
            }
            else
            {
                if((data.NextSeasonStartAt + TimeSpan.FromSeconds(1)).IsFuture(Now))
                {
                    // 開催まであと{0}
                    messageText.text = string.Format(StringValueAssetLoader.Instance["clubmatch.header.banner.message_1"], data.NextSeasonStartAt.GetRemainingString(Now));
                    // シーズン開催通知
                    if(data.NextSeasonStartAt.IsPast(Now))
                    {
                        OnStartSeason();
                    }
                }
            }
        }
        
        /// <summary>シーズン開始通知</summary>
        private void OnStartSeason()
        {
            // シーズン開始
            badgeImage.gameObject.SetActive(true);
            messageText.text = StringValueAssetLoader.Instance["club.season.start_title"];
            isSeasonStarted = true;
        }
        
        /// <summary>シーズン終了通知</summary>
        private void OnEndSeason()
        {
            // シーズン終了
            badgeImage.gameObject.SetActive(false);
            messageText.text = StringValueAssetLoader.Instance["club.season.end_title"];
            isSeasonEnded = true;
        }

        private void Update()
        {
            // クラブマッチがない
            if(data == null) return;
            // マッチングしてない && クラブに所属してない
            if(data.SeasonData.IsMatchingFailed && !IsJoinedClub) return; 
            UpdateView();
        }

        private void OnDestroy()
        {
            resourcesLoader.Release();
        }
        
        /// <summary>uGUI</summary>
        public void OnClick()
        {
            // TODO クラブマッチが設定されてない
            if(data == null)
            {
                return;
            }
            
            // クラブに参加してない
            if(!IsJoinedClub)
            {
                // クラブがロックされている
                if(!UserDataManager.Instance.IsUnlockSystem(Pjfb.Club.ClubUtility.clubLockId))
                {
                    ClubUtility.OpenClubLockModal();
                    return;
                }
                
                // マッチングしてない
                if(data.SeasonData.IsMatchingFailed)
                {
                    // クラブ側でクラブを探すページへ遷移する
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, true, null);
                    return;
                }
            }
            
            // シーズン開始モーダル
            if(isSeasonStarted)
            {
                ClubMatchUtility.OpenSeasonChangeModal(ClubMatchSeasonChangeModalType.Start);
                return;
            }
            
            // シーズン終了モーダル
            if (isSeasonEnded)
            {
                ClubMatchUtility.OpenSeasonChangeModal(ClubMatchSeasonChangeModalType.End);
                return;
            }

            // シーズン開催中
            if(data.SeasonData.IsOnSeason)
            {
                // クラブマッチに参加できなかった
                if(data.SeasonData.IsMatchingFailed)
                {
                    ParticipationConditionsModal.Params param = new ParticipationConditionsModal.Params();
                    param.title = StringValueAssetLoader.Instance["clubmatch.participation_conditions_modal.title"];
                    param.message = string.Format(StringValueAssetLoader.Instance["clubmatch.participation_conditions_modal.message"], ConfigManager.Instance.colosseum.groupMatchRequireMemberGuild);
                    param.colosseumEventMaster = data.SeasonData.MColosseumEvent;
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ParticipationConditions, param);
                    return;
                }
            }
            
            if(AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.ClubMatch)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    ClubMatchPage.ShowErrorDialog(() =>
                    {
                    }, msgBodyKey:"network.no_connection");
                    return;
                }

                if (data.SeasonData != null)
                {
                    // クラブマッチトップへ遷移
                    ClubMatchPage.OpenPage(true, new ClubMatchTopPage.Data(data.SeasonData){callerPage = PageType.Home});
                }
            }
        }
    }
}