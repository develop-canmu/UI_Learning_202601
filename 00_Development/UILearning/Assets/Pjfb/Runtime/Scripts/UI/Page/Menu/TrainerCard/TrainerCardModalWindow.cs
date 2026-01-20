using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Coffee.UIEffects;
using CruFramework;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Club;
using Pjfb.Community;
using Pjfb.Deck;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UserData;
using Pjfb.Utility;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Menu
{
    public class TrainerCardModalWindow : ModalWindow
    {
        public class WindowParams
        {
            private long targetUMasterId;
            public long TargetUMasterId { get { return targetUMasterId; } }
            private bool showOtherButtons;
            public bool ShowOtherButtons{ get { return showOtherButtons; } }
            private bool showClubInfoHeaderButtons;
            public bool ShowClubInfoHeaderButtons{ get { return showClubInfoHeaderButtons; } }
            private Action onDissolution;
            public Action OnDissolution{get { return onDissolution; } }
            private Action onSecession;
            public Action OnSecession{get { return onSecession; } }
            private Action onClosed;
            public Action OnClosed{get { return onClosed; } }

            public WindowParams(long targetUMasterId, bool showOtherButtons = true, bool showClubInfoHeaderButtons = true, Action onDissolution = null, Action onSecession = null, Action onClosed = null)
            {
                this.targetUMasterId = targetUMasterId;
                this.showOtherButtons = showOtherButtons;
                this.showClubInfoHeaderButtons = showClubInfoHeaderButtons;
                this.onDissolution = onDissolution;
                this.onSecession = onSecession;
                this.onClosed = onClosed;
            }
        }
        
        [Header("共通")]
        [SerializeField] private UserIcon userIconImage;
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI introductionText;
        [SerializeField] private UIButton customizeButton;
        [SerializeField] private GameObject newCustomizeBadge;
        [SerializeField] private UIButton trainingMatchButton;

        // 自分のカードのみ表示される系オブジェクト
        [SerializeField] private List<UIButton> editButtonList;
        
        [Header("トレーナーカード1枚目")]
        [SerializeField] private UserTitleImage userTitleIconImage;
        [SerializeField] private GameObject newTitleBadge;
        [SerializeField] private TextMeshProUGUI goodNumText;
        [SerializeField] private UIButton goodButton;
        [SerializeField] private UIButton goodSelfCover;
        [SerializeField] private TrainerCardMyBadgeGroup myBadgeGroup;
        [SerializeField] private GameObject newBadgeEditBadge;
        [SerializeField] private TrainerCardCharacterImageGroup characterImage;
        [SerializeField] private GameObject newCharacterEditBadge;
        
        [Header("トレーナーカード2枚目")]
        // クラブ関係
        [SerializeField] private GameObject clubRaycastBlock;
        [SerializeField] private GameObject noClubRoot;
        [SerializeField] private GameObject clubInfoRoot;
        [SerializeField] private Image clubEmblemImage;
        [SerializeField] private Image clubRankImage;
        [SerializeField] private TextMeshProUGUI clubNameText;
        [SerializeField] private TextMeshProUGUI clubMemberCountText;
        [SerializeField] private TextMeshProUGUI clubPowerText;
        [SerializeField] private OmissionTextSetter clubPowerOmissionTextSetter;
        // デッキ
        [SerializeField] private TextMeshProUGUI deckNameText;
        [SerializeField] private DeckView deckView;
        // フレンド貸出選手
        [SerializeField] private CharacterIcon friendBorrowingIcon;
        // フレコ
        [SerializeField] private TextMeshProUGUI friendCodeText;
        // 他人のカードの下のほうに表示される系ボタン
        [SerializeField] private GameObject youButtonsRoot;
        [SerializeField] private GameObject otherButtonsRoot;
        [SerializeField] private UIButton clubInviteButton;
        [SerializeField] private UIButton yellButton;
        [SerializeField] private UIButton blockButton;
        [SerializeField] private UIButton followButton;
        // 通知
        [SerializeField] private UINotification notificationUI;
        // カード切り替え
        [SerializeField] private PageScrollRect pageScrollRect;
        
        [Header("着せ替え系")]
        // モーダル名
        [SerializeField] private Image titleImage;
        [SerializeField] private UIGradient titleGradient;
        [SerializeField] private TextMeshProUGUI titleText;
        // カード背景
        [SerializeField] private TrainerCardBackgroundImage cardBaseImage;
        [SerializeField] private UIGradient cardBaseGradient;
        // ユーザー名
        [SerializeField] private Image userNameBaseImage;
        [SerializeField] private UIGradient userNameBaseGradient;
        // 自己紹介ヘッダー
        [SerializeField] private Image introductionHeaderImage;
        [SerializeField] private UIGradient introductionHeaderGradient;
        [SerializeField] private TextMeshProUGUI introductionHeaderText;
        // バッジヘッダー
        [SerializeField] private Image badgeHeaderImage;
        [SerializeField] private UIGradient badgeHeaderGradient;
        [SerializeField] private TextMeshProUGUI badgeHeaderText;
        // サポートキャラ
        [SerializeField] private Image friendLoanPlayerFrameImage;
        [SerializeField] private TextMeshProUGUI friendLoanPlayerTitleText;
        // フレンドコード
        [SerializeField] private Image friendCodeBaseImage;
        
        private WindowParams windowParams = null;
        private UserProfileUserStatus userProfileUserStatus = null;
        private long customizeCardId;
        private TrainerCardCustomizeConfig customizeConfig = null;
        private bool isMyself = false;
        private const string CustomizeConfigKey = "TrainerCardCustomizeConfig";
        private const int GoodLimit = 9999999;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            windowParams = (WindowParams)args;
            userProfileUserStatus = await TrainerCardUtility.UserGetProfileAPI(windowParams.TargetUMasterId);
            await RefreshView(destroyCancellationToken);
            // カードスクロールの初期化
            pageScrollRect.InitScroll();
            await base.OnPreOpen(args, token);
        }
        
        public void OnClickClose()
        {
            Close(windowParams.OnClosed);
        }

        // 初期化を行う
        private async UniTask RefreshView(CancellationToken token)
        {
            // Viewセットの並列処理
            List<UniTask> taskList = new List<UniTask>();
            
            // 着せ替えの適用
            ProfileFrameMasterObject frameMasterObject = MasterManager.Instance.profileFrameMaster.FindData(userProfileUserStatus.profileCardData.mProfileFrameId);
            taskList.Add(SetCustomizeCard(frameMasterObject.imageId, token));
            
            isMyself = UserDataManager.Instance.user.uMasterId == userProfileUserStatus.uMasterId;
            // 自分と相手とで表示を切り替える
            foreach (UIButton editButton in editButtonList)
            {
                editButton.gameObject.SetActive(isMyself);
            }
            // 最下部のボタン切り替え
            customizeButton.gameObject.SetActive(isMyself);
            otherButtonsRoot.SetActive(!isMyself && windowParams.ShowOtherButtons);
            youButtonsRoot.SetActive(isMyself);
            trainingMatchButton.gameObject.SetActive(!isMyself && windowParams.ShowOtherButtons);
            if (otherButtonsRoot.activeSelf)
            {
                clubInviteButton.interactable = userProfileUserStatus.canInvitation;
                yellButton.interactable = userProfileUserStatus.canYell;
                blockButton.interactable = userProfileUserStatus.canBlock;
                followButton.interactable = userProfileUserStatus.canFollow;
            }
            
            // 共通部分初期化
            userNameText.text = userProfileUserStatus.name;
            // アイコン関係
            taskList.Add(userIconImage.SetIconIdAsync(userProfileUserStatus.mIconId).AttachExternalCancellation(token));
            introductionText.text = userProfileUserStatus.wordIntroduction;
            
            // 称号
            taskList.Add(userTitleIconImage.SetTextureAsync(userProfileUserStatus.mTitleId).AttachExternalCancellation(token));
            // イイネ表示
            if (isMyself)
            {
                // 自分の場合は色そのままで無効化
                goodSelfCover.gameObject.SetActive(true);
                goodButton.interactable = true;
            }
            else
            {
                goodSelfCover.gameObject.SetActive(false);
                goodButton.interactable = userProfileUserStatus.canLike;
            }
            GoodValueSetText(userProfileUserStatus.likeCount);
            
            // キャラクター表示
            taskList.Add(characterImage.SetTextureAsync(userProfileUserStatus.profileCardData.mProfileCharaId, userProfileUserStatus.profileCardData.mProfileBackgroundId, token));
            // バッジの表示
            taskList.Add(myBadgeGroup.SetMyBadgeList(userProfileUserStatus.profileCardData.mEmblemIdList, token));
            
            // 2枚目初期化
            //Club設定
            if (userProfileUserStatus.guild != null && userProfileUserStatus.guild.gMasterId > 0)
            {
                noClubRoot.SetActive(false);
                clubInfoRoot.SetActive(true);
                clubRaycastBlock.gameObject.SetActive(false);
                clubNameText.text = userProfileUserStatus.guild.name;
                clubMemberCountText.text = $"{userProfileUserStatus.guild.numberOfPeople}/{ConfigManager.Instance.maxGuildMemberCount}";
                clubPowerText.text = new BigValue(userProfileUserStatus.guild.totalCombatPower).ToDisplayString(clubPowerOmissionTextSetter.GetOmissionData());
                taskList.Add(ClubUtility.LoadAndSetEmblemIcon(clubEmblemImage, userProfileUserStatus.guild.mGuildEmblemId).AttachExternalCancellation(token));
                taskList.Add(ClubUtility.LoadAndSetRankIcon(clubRankImage, userProfileUserStatus.guild.mGuildRankId).AttachExternalCancellation(token));
            }
            else
            {
                noClubRoot.SetActive(true);
                clubInfoRoot.SetActive(false);
                clubRaycastBlock.gameObject.SetActive(!isMyself);
            }

            if (isMyself)
            {
                friendCodeText.text = string.Format(StringValueAssetLoader.Instance["menu.trainer-card.friend-code"], UserDataManager.Instance.user.friendCode);
            }
            
            // デッキ設定
            deckView.InitializeUI(userProfileUserStatus.charaVariableList, userProfileUserStatus.deck);
            deckNameText.text = userProfileUserStatus.deck.name;
            
            //フレンド貸出選手
            CharacterDetailData detailData = new CharacterDetailData(new UserDataChara(userProfileUserStatus.friendDeckCharaList.FirstOrDefault()));
            taskList.Add(friendBorrowingIcon.SetIconAsync(detailData).AttachExternalCancellation(token));
            friendBorrowingIcon.CanLiberation = false;
            friendBorrowingIcon.SwipeableParams ??= new SwipeableParams<CharacterDetailData>();
            friendBorrowingIcon.SwipeableParams.StartIndex = 0;
            friendBorrowingIcon.SwipeableParams.DetailOrderList = new List<CharacterDetailData>() { detailData };
            // バッジの更新
            // 称号のバッジ
            newTitleBadge.SetActive(MenuManager.IsTitleBadge());
            // MyBadgeのバッジ
            newBadgeEditBadge.SetActive(MenuManager.IsEmblemBadge());
            // キャラクターのバッジ
            newCharacterEditBadge.SetActive(MenuManager.IsProfileCharaBadge());
            // 着せ替えボタンのバッジ
            newCustomizeBadge.SetActive(MenuManager.IsProfileFrameBadge());

            // 全てのタスクが終了するまで待機する
            await UniTask.WhenAll(taskList);
        }

        private async UniTask UpdateProfile(CancellationToken token)
        {
            if (userProfileUserStatus == null)
            {
                userProfileUserStatus = await TrainerCardUtility.UserGetProfileAPI(windowParams.TargetUMasterId);
            }
            await RefreshView(token);
            // ヘッダーのバッジ更新
            AppManager.Instance.UIManager.Header.UpdateMenuBadge();
        }
        
        // ユーザーアイコン変更
        public void OnClickEditUserIcon()
        {
            EditUserIcon(destroyCancellationToken).Forget();
        }

        private async UniTask EditUserIcon(CancellationToken token)
        {
            UserIconChangeModalWindow.WindowParams param = new UserIconChangeModalWindow.WindowParams();
            param.DefaultSelectId = UserDataManager.Instance.user.mIconId;
            UserIconChangeModalWindow window = (UserIconChangeModalWindow)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.UserIconChange, param, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }

        // ユーザーネーム変更
        public void OnClickChangeUserName()
        {
            ChangeUserName(destroyCancellationToken).Forget();
        }

        private async UniTask ChangeUserName(CancellationToken token)
        {
            UserNameChangeModalWindow.WindowParams param = new UserNameChangeModalWindow.WindowParams();
            param.useName = userNameText.text;
            param.applyName = text => userNameText.text = text;
            UserNameChangeModalWindow window = (UserNameChangeModalWindow) await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.UserNameChange, param, token);
            await window.WaitCloseAsync(token);
            userProfileUserStatus = await TrainerCardUtility.UserGetProfileAPI(windowParams.TargetUMasterId);
            await UpdateProfile(token);
        }
        
        // 自己紹介
        public void OnClickEditIntroduction()
        {
            EditIntroduction(destroyCancellationToken).Forget();
        }

        private async UniTask EditIntroduction(CancellationToken token)
        {
            IntroductionEditModalWindow.WindowParams param = new IntroductionEditModalWindow.WindowParams();
            param.Introduction = introductionText.text;
            IntroductionEditModalWindow window = (IntroductionEditModalWindow)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.IntroductionEdit, param, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }

        // 称号
        public void OnClickEditUserTitle()
        {
            EditUserTitle(destroyCancellationToken).Forget();
        }

        private async UniTask EditUserTitle(CancellationToken token)
        {
            UserTitleChangeModalWindow.WindowParams param = new UserTitleChangeModalWindow.WindowParams();
            param.DefaultSelectId = userProfileUserStatus.mTitleId;
            UserTitleChangeModalWindow window = (UserTitleChangeModalWindow)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.UserTitleChange, param, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }
        
        // いいね
        public void OnClickGood()
        {
            goodButton.interactable = false;
            OnUserSendProfileLikeAPI(destroyCancellationToken).Forget();
        }

        private async UniTask OnUserSendProfileLikeAPI(CancellationToken token)
        {
            // いいねAPI
            UserSendProfileLikeAPIResponse response = await TrainerCardUtility.UserSendProfileLikeAPI(userProfileUserStatus.uMasterId);
            GoodValueSetText(response.likeCount);
        }

        private void GoodValueSetText(long goodNum)
        {
            if (goodNum > GoodLimit)
            {
                // イイネが表示上限に達している場合"+9,999,999"を表示
                goodNumText.text = string.Format(StringValueAssetLoader.Instance["menu.trainer-card.good_limit"], GoodLimit.GetStringNumberWithComma());
            }
            else
            {
                goodNumText.text = goodNum.GetStringNumberWithComma();
            }
        }
        
        // 選手
        public void OnClickEditCharacter()
        {
            // 選手編集
            OpenCharacterSetting(destroyCancellationToken).Forget();
        }

        private async UniTask OpenCharacterSetting(CancellationToken token)
        {
            TrainerCardDisplayCharacterSettingModal.ModalData data = new TrainerCardDisplayCharacterSettingModal.ModalData(userProfileUserStatus.profileCardData.mProfileCharaId, userProfileUserStatus.profileCardData.mProfileBackgroundId);
            TrainerCardDisplayCharacterSettingModal window = (TrainerCardDisplayCharacterSettingModal)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardDisplayCharacterSetting, data, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }
        
        // バッジ編集
        public void OnClickEditBadge()
        {
            EditBadge(destroyCancellationToken).Forget();
        }

        private async UniTask EditBadge(CancellationToken token)
        {
            MyBadgeChangeModal.ModalData data = new MyBadgeChangeModal.ModalData();
            data.emblemIds = userProfileUserStatus.profileCardData.mEmblemIdList;
            MyBadgeChangeModal window = (MyBadgeChangeModal) await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.MyBadgeChange, data, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }

        // クラブ情報
        public void OnClickClubInfo()
        {
            OnOpenClubInfo(destroyCancellationToken).Forget();
        }

        private async UniTask OnOpenClubInfo(CancellationToken token)
        {
            // クラブ機能解放確認
            if(!CheckClubUnlock()) return;
            // 自分がクラブ未所属の場合クラブ画面に遷移
            if (userProfileUserStatus.guild == null || userProfileUserStatus.guild.gMasterId == 0)
            {
                AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
                Close(() =>
                {
                    bool addStack = AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.Club;
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, addStack, null); 
                });
                return;
            }
            // クラブ表示
            long guildId = userProfileUserStatus.guild.gMasterId;
            // 自分の情報または自分のクラブだった場合guildIDを0にする
            if (UserDataManager.Instance.user.uMasterId == userProfileUserStatus.uMasterId || UserDataManager.Instance.user.gMasterId == guildId)
            {
                guildId = 0;
            }
            
            // クラブ情報取得API
            GuildGetGuildAPIRequest request = new GuildGetGuildAPIRequest();
            GuildGetGuildAPIPost post = new GuildGetGuildAPIPost();
            post.gMasterId = guildId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // 奥のクラブ情報を削除する
            AppManager.Instance.UIManager.ModalManager.RemoveModals(window => window is ClubInfoModal);
            
            ClubInfoModal.Param param = new ClubInfoModal.Param();
            param.onFinishedDissolution = windowParams.OnDissolution ?? SecessionClubRemoveModals;
            param.onFinishedSecession = windowParams.OnSecession ?? SecessionClubRemoveModals;
            param.clubData = new ClubData(request.GetResponseData().guild);
            param.myUserID = UserDataManager.Instance.user.uMasterId;
            param.showUserProfileOtherButtons = windowParams.ShowOtherButtons;
            param.showHeaderButtons = windowParams.ShowClubInfoHeaderButtons;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInfo, param);
        }
        
        // サポートキャラクター情報
        public void OnClickSupportCharacterInfo()
        {
            // キーを設定
            string stringKey = "menu.profile.friend_borrowing_detail";
            BaseCharaDetailModalParams param = new BaseCharaDetailModalParams(friendBorrowingIcon.SwipeableParams, false, false, stringKey);
            BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail, param);
        }

        // フレンドコードコピー
        public void OnClickFriendCodeCopy()
        {
            GUIUtility.systemCopyBuffer = UserDataManager.Instance.user.friendCode;
            string message = StringValueAssetLoader.Instance["menu.profile.friend_code_copy"];
            notificationUI.ShowNotification(message);
        }
        
        // クラブ勧誘設定
        public void OnClickClubSolicitation()
        {
            if(!CheckClubUnlock()) return;
            ClubSolicitationSettingsModalWindow.Open(new ClubSolicitationSettingsModalWindow.WindowParams());
        }
        
        // クラブ勧誘ボタン
        public void OnClickClubInvite()
        {
            GuildMemberMemberStatus memberStatus = new GuildMemberMemberStatus();
            memberStatus.name = userProfileUserStatus.name;
            memberStatus.uMasterId = userProfileUserStatus.uMasterId;
            ClubInvitationUserModal.Param param = new ClubInvitationUserModal.Param();
            param.user = new ClubUserData(memberStatus);
            param.onInvitationUser = clubUserData =>
            {
                clubInviteButton.interactable = false;
                string text = string.Format( StringValueAssetLoader.Instance["club.invitationNotificationText"], param.user.name);
                notificationUI.ShowNotification(text);
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInvitationUser, param);
        }
        
        // エールボタン
        public void OnClickYell()
        {
            string badgeCountText = string.Format(StringValueAssetLoader.Instance["community.yell.count_badge"], ConfigManager.Instance.yellLimit - CommunityManager.yellCount, ConfigManager.Instance.yellLimit);
            FollowConfirmModalWindow.WindowParams param = new FollowConfirmModalWindow.WindowParams();
            param.BadgeCountText = badgeCountText;
            param.UMasterId = userProfileUserStatus.uMasterId;
            param.UserName = userProfileUserStatus.name;
            // モーダルを閉じた際にエールボタンの切り替えを行う
            param.onClosed = () => yellButton.interactable = CommunityManager.CheckCanYell(userProfileUserStatus.uMasterId);
            FollowConfirmModalWindow.Open(param);
        }
        
        // チャットボタン
        public void OnClickChat()
        {
            // チャット画面に遷移するため、モーダルのストックを削除しておく
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            // トレーナーカードを閉じながらチャット画面を開く
            Close(() =>
            {
                CommunityPage.CommunityPageInfo info = new CommunityPage.CommunityPageInfo();
                info.Status = CommunityStatus.PersonalChat;
                info.TargetUMasterId = userProfileUserStatus.uMasterId;
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, info);
            });
        }
        
        // ブロックボタン
        public void OnClickBlock()
        {
            ConfirmModalData data = new ConfirmModalData();
            data.Title = StringValueAssetLoader.Instance["common.confirm"];
            data.Message = string.Format(StringValueAssetLoader.Instance["community.block_confirm"], userProfileUserStatus.name);
            data.PositiveButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["community.block"], blockButtonParam =>
            {
                UserBlock(destroyCancellationToken).Forget();
                blockButtonParam.Close();
            });
            data.NegativeButtonParams = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], cancelButtonParam => cancelButtonParam.Close());
            ConfirmModalWindow.Open(data);
        }

        private async UniTask UserBlock(CancellationToken token)
        {
            // ブロックAPI
            CommunityBlockAPIRequest request = new CommunityBlockAPIRequest();
            CommunityBlockAPIPost post = new CommunityBlockAPIPost();
            post.targetUMasterId = userProfileUserStatus.uMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            string message = string.Format(StringValueAssetLoader.Instance["community.blocked"],userProfileUserStatus.name);
            notificationUI.ShowNotification(message);
            blockButton.interactable = false;
            UserCommunityUserStatus userStatus = GetUserCommunityUserStatus();
            CommunityManager.blockUserList.Add(userStatus);
            // ブロックしたユーザーをフォローしていた場合
            bool isFollowed = CommunityManager.followUserList.Contains(userStatus);
            if (isFollowed)
            {
                CommunityManager.followUserList.Remove(userStatus);
                CommunityManager.yellDetail.followedCount--;
                bool isYelled = false;
                foreach(ModelsUYell yell in CommunityManager.yellDetail.todayYelledList)
                {
                    // ブロックしたユーザーを今日エールしたか
                    if (yell.uMasterId == userStatus.uMasterId)
                    {
                        isYelled = true;
                        break;
                    }
                }
                if (!isYelled)
                {
                    // エールできる人数を更新する
                    CommunityManager.yellDetail.followedCanYellCount--;
                    //メニューバッジ更新
                    AppManager.Instance.UIManager.Header.UpdateMenuBadge();
                }
            }
            //コミュニティチャットリスト更新
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Community)
            {
                ((CommunityPage)AppManager.Instance.UIManager.PageManager.CurrentPageObject).UpdateBlockChatList(userProfileUserStatus.uMasterId);
            }
        }
        
        // フォローボタン
        public void OnClickFollow()
        {
            SendFollow(destroyCancellationToken).Forget();
        }

        private async UniTask SendFollow(CancellationToken token)
        {
            // フォローAPI
            CommunityFollowAPIRequest request = new CommunityFollowAPIRequest();
            CommunityFollowAPIPost post = new CommunityFollowAPIPost();
            post.targetUMasterId = userProfileUserStatus.uMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            string message = string.Format(StringValueAssetLoader.Instance["community.followed"],userProfileUserStatus.name);
            notificationUI.ShowNotification(message);
            followButton.interactable = false;
            // フォローリストに追加
            UserCommunityUserStatus userStatus = GetUserCommunityUserStatus();
            CommunityManager.followUserList.Add(userStatus);
            CommunityManager.yellDetail.followedCount++;
            
            bool isYelled = false;
            foreach(ModelsUYell yell in CommunityManager.yellDetail.todayYelledList)
            {
                // フォローしたユーザーを今日エールしたか
                if (yell.uMasterId == userStatus.uMasterId)
                {
                    isYelled = true;
                    break;
                }
            }
            if (!isYelled)
            {
                CommunityManager.yellDetail.followedCanYellCount++;
                //メニューバッジ更新
                AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            }
        }
        
        // カード着せ替え
        public void OnClickCustomize()
        {
            CustomizeCard(destroyCancellationToken).Forget();
        }

        private async UniTask CustomizeCard(CancellationToken token)
        {
            TrainerCardCustomizeModal.ModalData data = new TrainerCardCustomizeModal.ModalData(userProfileUserStatus.profileCardData.mProfileFrameId);
            TrainerCardCustomizeModal window = (TrainerCardCustomizeModal) await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCardCustomize, data, token);
            userProfileUserStatus = (UserProfileUserStatus)await window.WaitCloseAsync(token);
            await UpdateProfile(token);
        }

        // トレーニングマッチ
        public void OnClickTrainingMatch()
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            TeamConfirmPage.PageParams param = new TeamConfirmPage.PageParams(PageType.Home, null, userProfileUserStatus.uMasterId, userProfileUserStatus);
            Close(onCompleted: () =>
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TeamConfirmTrainingMatch, true, param);
            });
        }
        
        // クラブ機能解放チェック
        private bool CheckClubUnlock()
        {
            if(UserDataManager.Instance.IsUnlockSystem(ClubUtility.clubLockId)) return true;
            // 解放されていなければモーダル表示
            ClubUtility.OpenClubLockModal();
            return false;
        }
        
        // クラブ脱退or解散時のスタック削除&トレーナーカード更新
        private void SecessionClubRemoveModals()
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
            userProfileUserStatus = null;
            UpdateProfile(destroyCancellationToken).Forget();
        }
        
        // UserCommunityUserStatusに変換する
        private UserCommunityUserStatus GetUserCommunityUserStatus()
        {
            UserCommunityUserStatus userStatus = new UserCommunityUserStatus();
            userStatus.uMasterId = userProfileUserStatus.uMasterId;
            userStatus.name = userProfileUserStatus.name;
            userStatus.mIconId = userProfileUserStatus.mIconId;
            userStatus.maxCombatPower = userProfileUserStatus.deck.combatPower;
            userStatus.lastLogin = userProfileUserStatus.lastLogin;
            userStatus.mTitleId = userProfileUserStatus.mTitleId;
            userStatus.maxDeckRank = userProfileUserStatus.deck.rank;
            return userStatus;
        }
        
        // トレーナーカードの着せ替え設定
        private async UniTask LoadCustomizeCard(long id, CancellationToken token)
        {
            // 設定が変わっていなければ読み込まない
            if(customizeCardId == id) return;
            // 設定ファイルのの読み込み
            await PageResourceLoadUtility.LoadAssetAsync<TrainerCardCustomizeConfig>( ResourcePathManager.GetPath(CustomizeConfigKey, id), (config)=>
                {
                    // 着せ替え設定をキャッシュしておく
                    customizeCardId = id;
                    customizeConfig = config;
                },
                token
            );
        }

        public async UniTask SetCustomizeCard(long id, CancellationToken token = default)
        {
            await LoadCustomizeCard(id, token);
            // モーダル名
            titleImage.sprite = customizeConfig.HeaderImage;
            titleGradient.color1 = customizeConfig.HeaderColorGradient.GradientColor1;
            titleGradient.color2 = customizeConfig.HeaderColorGradient.GradientColor2;
            titleText.color = customizeConfig.TitleTextColor;
            // カード背景
            cardBaseImage.SetTexture(customizeConfig.BodyImage, customizeConfig.BodyH2MD);
            cardBaseGradient.color1 = customizeConfig.BodyColorGradient.DiagonalColorDownLeft;
            cardBaseGradient.color2 = customizeConfig.BodyColorGradient.DiagonalColorDownRight;
            cardBaseGradient.color3 = customizeConfig.BodyColorGradient.DiagonalColorUpLeft;
            cardBaseGradient.color4 = customizeConfig.BodyColorGradient.DiagonalColorUpRight;
            // ユーザー名
            userNameBaseImage.sprite = customizeConfig.UserNameRootImage;
            userNameBaseGradient.color1 = customizeConfig.UserNameRootColorGradient.GradientColor1;
            userNameBaseGradient.color2 = customizeConfig.UserNameRootColorGradient.GradientColor2;
            userNameText.color = customizeConfig.UserNameTextColor;
            // 自己紹介ヘッダー
            introductionHeaderImage.sprite = customizeConfig.IntroductionHeaderImage;
            introductionHeaderGradient.color1 = customizeConfig.IntroductionHeaderColorGradient.GradientColor1;
            introductionHeaderGradient.color2 = customizeConfig.IntroductionHeaderColorGradient.GradientColor2;
            introductionHeaderText.color = customizeConfig.IntroductionHeaderTextColor;
            // バッジヘッダー
            badgeHeaderImage.sprite = customizeConfig.BadgeHeaderImage;
            badgeHeaderGradient.color1 = customizeConfig.BadgeHeaderColorGradient.GradientColor1;
            badgeHeaderGradient.color2 = customizeConfig.BadgeHeaderColorGradient.GradientColor2;
            badgeHeaderText.color = customizeConfig.BadgeTextColor;
            // サポート選手
            friendLoanPlayerFrameImage.sprite = customizeConfig.SupportCharacterHeaderImage;
            friendLoanPlayerFrameImage.color = customizeConfig.SupportCharacterHeaderColor;
            friendLoanPlayerTitleText.color = customizeConfig.SupportCharacterHeaderTextColor;
            // フレンドコード
            friendCodeBaseImage.sprite = customizeConfig.FriendCodeBaseImage;
            friendCodeBaseImage.color = customizeConfig.FriendCodeBaseColor;
            friendCodeText.color = customizeConfig.FriendCodeTextColor;
        }
                
#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
        private readonly string debugMenuKey = "トレーナーカード";
        // longを使えない?のでキャストして使う
        private int debugIconId = 1;
        private int debugTitleId = 1;
        private int debugCustomizeFrameId = 1;
        private int[] debugBadgeIdList = { 1, 1, 1};

        private void OnEnable()
        {
            CruFramework.DebugMenu.AddOption(debugMenuKey, "アイコンID", () => debugIconId, 
                id =>
                {
                    debugIconId = id;
                    userIconImage.SetIconIdAsync(id).Forget();
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "称号ID", () => debugTitleId, 
                id =>
                {
                    debugTitleId = id;
                    userTitleIconImage.SetTextureAsync(id).Forget();
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "着せ替えID", () => debugCustomizeFrameId, 
                id =>
                {
                    debugCustomizeFrameId = id;
                    SetCustomizeCard(id).Forget();
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "バッジID1", () => debugBadgeIdList[0],
                id =>
                {
                    debugBadgeIdList[0] = id;
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "バッジID2", () => debugBadgeIdList[1],
                id =>
                {
                    debugBadgeIdList[1] = id;
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "バッジID3", () => debugBadgeIdList[2],
                id =>
                {
                    debugBadgeIdList[2] = id;
                });
            CruFramework.DebugMenu.AddOption(debugMenuKey, "バッジ更新", () =>
            {
                myBadgeGroup.SetMyBadgeList(new long[] { debugBadgeIdList[0], debugBadgeIdList[1], debugBadgeIdList[2] }, destroyCancellationToken).Forget();
            });
        }

        private void OnDisable()
        {
            CruFramework.DebugMenu.RemoveOption(debugMenuKey);
        }
#endif
    }
}