using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public enum ReadFlgType
    {
        Viewed = 1,
        NotView = 2,
    }
    
    public enum CommunityStatus
    {
        PersonalChat,
        ClubChat,
        ChatRoom,
    }
    public enum CommunityTabType
    {
        PersonalTab,
        ClubTab,
    }

    /// <summary>
    /// 仮のメッセージタイプ
    /// </summary>
    public enum MessageType
    {
        None = 0,
        Message = 1,
        Stamp = 2,
        System = 3,
    }
    
    public class CommunityPage : Page
    {
        [Serializable]
        public class CommunityTabInfo
        {
            public CommunityTabType type;
            public CommunityTabSheetButton chatTab;
            public Image blockImage;
        }
        
        public class CommunityPageInfo
        {
            public CommunityStatus Status = CommunityStatus.ClubChat;
            public long TargetUMasterId = -1;
        }
        
        [SerializeField]
        private float chatUpdateInterval = 5.0f;
        private const float ChatTimeTextUpdateInterval = 5.0f;

        [SerializeField]
        private List<CommunityTabInfo> tabList;
        [SerializeField]
        private CommunitySheetManager sheetManager;
        
        [SerializeField]
        private UIButton updateButton;
        [SerializeField]
        private UIButton chatDeleteButton;
        [SerializeField]
        private UIButton sendButton;

        [SerializeField]
        private ChatMessageView chatMessageView;
        [SerializeField]
        private ScrollGrid roomScroll;
        [SerializeField]
        private ScrollGrid stampScroll;
        
        [SerializeField]
        private GameObject stampViewRoot;
        [SerializeField]
        private GameObject chatFooterViewRoot;
        [SerializeField]
        private GameObject chatRoomViewRoot;
        
        [SerializeField]
        private GameObject chatRoomEmptyText;
        [SerializeField]
        private GameObject stampEmptyText;
        
        [SerializeField]
        private GameObject yellHistoryBadge;
        [SerializeField]
        private GameObject yellAllBadge;
        [SerializeField]
        private GameObject personalChatBadge;
        [SerializeField]
        private GameObject clubChatBadge;
        
        [SerializeField]
        private GameObject clubLockCover;
        [SerializeField]
        private TMP_InputField messageField;
        [SerializeField]
        private UINotification notificationUI;
        
        [SerializeField]
        private Image stampButtonBG;
        [SerializeField]
        private Image stampButtonIcon;
        
        private List<ChatStampInfo> stampInfoList = new List<ChatStampInfo>();
        private List<ChatRoomInfo> chatRoomInfoList = new List<ChatRoomInfo>();
        private List<ChatRoomScrollItem> cacheChatRoomScrollItems = new List<ChatRoomScrollItem>();
        private CommunityStatus currentStatus;
        private CancellationTokenSource updateTextCancellationTokenSource = null;
        private float chatUpdateTimer = 0;
        private bool isInitialized = false;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            CommunityPageInfo info = (CommunityPageInfo)args;
            
            //args指定なし場合の初期化設定
            if (info == null)
            {
                info = new CommunityPageInfo();
                info.Status = UserDataManager.Instance.user.gMasterId != 0 ? CommunityStatus.ClubChat : CommunityStatus.ChatRoom;
            }
                        
            //クラブ未所属のTabロック処理
            bool clubJoined = UserDataManager.Instance.user.gMasterId != 0;
            clubLockCover.SetActive(!clubJoined);
            tabList.First(tab => tab.type == CommunityTabType.ClubTab).chatTab.SwitchButton.interactable = clubJoined;
            
            // タブは開き直す
            SelectTabSheet(info.Status, true);
            
            messageField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, messageField.characterLimit,messageField.fontAsset);
            OnInputFieldValueChanged("");

            await GetBlockListAPI();
            await RefreshBadgesByTodayYell();
            
            await ChangeCommunityStatus(info.Status, info.TargetUMasterId);
            
            isInitialized = true;
            
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            
            tabList.ForEach(tab => { tab.chatTab.OnTabClick = () => OnActiveCommunityTab(tab.type);});
            
            //Scroll Itemのtime Text更新
            UpdateChatRoomTimeText().Forget();
            //スタンプロード
            LoadStamp();
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            isInitialized = false;
            chatUpdateTimer = 0;
            stampInfoList.Clear();
            chatRoomInfoList.Clear();
            return base.OnPreClose(token);
        }

        protected override void OnClosed()
        {
            ReleaseCancelToken();
            base.OnClosed();
        }
        
        protected override void OnDestroy()
        {
            ReleaseCancelToken();
            base.OnDestroy();
        }

        private void Update()
        {
            // 一定間隔でチャット取得を行う
            if (currentStatus is CommunityStatus.PersonalChat or CommunityStatus.ClubChat)
            {
                UpdateChatTimer();
            }
            else
            {
                chatUpdateTimer = 0;
            }
        }

        private void OnActiveCommunityTab(CommunityTabType tabType)
        {
            switch (tabType)
            {
                case CommunityTabType.ClubTab:
                    if (currentStatus == CommunityStatus.ClubChat) return;
                    ChangeCommunityStatus(CommunityStatus.ClubChat).Forget();
                    break;
                case CommunityTabType.PersonalTab:
                    if (currentStatus == CommunityStatus.ChatRoom || currentStatus == CommunityStatus.PersonalChat) return;
                    ChangeCommunityStatus(CommunityStatus.ChatRoom).Forget();
                    break;
            }
        }
        
        public void OnClickSend()
        {
            if (currentStatus == CommunityStatus.PersonalChat)
            {
                SendChatAPI(messageField.text, chatMessageView.GetCurrentUMasterId()).Forget();
            }
            else if(currentStatus == CommunityStatus.ClubChat)
            {
                SendClubChatAPI(messageField.text).Forget();
            }
        }
        
        public void OnClickStampButton()
        {
            ChangeActiveStampView(!stampViewRoot.activeSelf);
        }
        
        private async void OnClickSendStampButton(long stampId)
        {
            ChangeActiveStampView(false);

            if (currentStatus == CommunityStatus.PersonalChat)
            {
                await SendChatAPI(stampId.ToString(), chatMessageView.GetCurrentUMasterId(), MessageType.Stamp);
            }
            else if (currentStatus == CommunityStatus.ClubChat)
            {
                await SendClubChatAPI(stampId.ToString(), MessageType.Stamp);
            }
        }
        
        public void OnClickBack()
        {
            switch (currentStatus)
            {
                case CommunityStatus.PersonalChat:
                    ChangeCommunityStatus(CommunityStatus.ChatRoom).Forget();
                    break;
                case CommunityStatus.ClubChat:
                case CommunityStatus.ChatRoom:
                    AppManager.Instance.UIManager.PageManager.PrevPage();
                    break;
            }
        }

        public void OnClickOpenPersonalChat(long uMasterId)
        {
            //tab変換
            if (currentStatus != CommunityStatus.PersonalChat)
            {
                tabList.ForEach(tab => { tab.chatTab.OnTabClick = null; });
                
                SelectTabSheet(CommunityStatus.PersonalChat);
            }
            
            ChangeCommunityStatus(CommunityStatus.PersonalChat, uMasterId).Forget();
            
            tabList.ForEach(tab => { tab.chatTab.OnTabClick = () => OnActiveCommunityTab(tab.type);});
        }

        public void UpdateBlockChatList(long targetUMasterId)
        {
            if (chatRoomInfoList.Any(info => info.uStatus.uMasterId == targetUMasterId))
            {
                ChangeCommunityStatus(CommunityStatus.ChatRoom).Forget();
            }
            else if (CommunityManager.blockUserList.All(user => user.uMasterId != targetUMasterId))
            {
                FetchChatData().Forget();
            }
        }
        
        public void OnClickYellAll()
        {
            CommunityManager.OpenYellAllModal();
        }

        public void OnClickYellHistory()
        {
            YellHistoryModalWindow.Open(new YellHistoryModalWindow.WindowParams
            {
                onClosed = () => yellHistoryBadge.SetActive(CommunityManager.ShowYellHistoryBadge)
            });
        }
        
        public void OnClickLatest()
        {
            chatMessageView.SetScrollPosition();
        }
        
        /// <summary>
        /// チャットルームの情報を更新する
        /// </summary>
        public void OnClickUpdateChatRooms()
        {
            FetchChatData().Forget();
        }
        
        public void OnInputFieldValueChanged(string input)
        {
            input = StringUtility.GetLimitNumCharacter(input, messageField.characterLimit);
            messageField.SetTextWithoutNotify(input);
            sendButton.interactable = !string.IsNullOrEmpty(input);
        }
        
        public void OnClickFollowButton()
        {
            FollowModalWindow.Open(new FollowModalWindow.WindowParams{onClosed = SetBadge});
        }
        
        public void OnClickChatDelete()
        {
            string targetName = chatMessageView.GetCurrentUserName();
            // 確認モーダル
            ConfirmModalData data = new ConfirmModalData();
            // タイトル
            data.Title = StringValueAssetLoader.Instance["common.confirm"];
            // メッセージ
            data.Message = String.Format(StringValueAssetLoader.Instance["commmunity.chat.delete_confirm"],targetName);
            // 注意
            data.Caution = StringValueAssetLoader.Instance["commmunity.chat.deleted_caution"];

            // OKボタン
            data.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.ok"],
                window1 =>
                {
                    //チャットリスト削除
                    DeleteChatAPI(chatMessageView.GetCurrentUMasterId()).Forget();
                    // 結果モーダル
                    ConfirmModalData data = new ConfirmModalData();
                    // タイトル
                    data.Title = StringValueAssetLoader.Instance["commmunity.chat.deleted"];
                    // メッセージ
                    data.Message =  String.Format(StringValueAssetLoader.Instance["commmunity.chat.list_deleted"],targetName);
                    // 閉じるボタン
                    data.NegativeButtonParams = new ConfirmModalButtonParams(
                        StringValueAssetLoader.Instance["common.close"],
                        window2 =>
                        {
                            // 確認モーダルをスタックから取り除く
                            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => window != this);
                            // 結果モーダルを閉じる
                            window2.Close();
                        }
                    );
                    // 確認モーダルを開く
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                }
            ,isRed: true);
            // キャンセルボタン
            data.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.cancel"],
                // 閉じる
                window => window.Close()
            );
            // 確認モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        /// <summary>
        /// 画面の内容を切り替える
        /// </summary>
        /// <param name="newStatus"></param>
        private async UniTask ChangeCommunityStatus(CommunityStatus newStatus, long targetUMasterId = -1)
        {
            currentStatus = newStatus;

            // チャット画面を画面更新まで非表示にする。
            chatMessageView.SetScrollerAlpha(false);

            await FetchChatData(targetUMasterId);
            
            // スタンプビューのアクティブを切る
            ChangeActiveStampView(false);
            
            ChangeViewContent(currentStatus);
        }

        /// <summary>
        /// スクリプトからタブを選択する
        /// </summary>
        private void SelectTabSheet(CommunityStatus status, bool isForceOpen = false)
        {
            switch (status)
            {
                case CommunityStatus.ChatRoom:
                case CommunityStatus.PersonalChat:
                    if (isForceOpen == false && sheetManager.CurrentSheetType == CommunityTabSheetType.PersonalChat) return;
                    sheetManager.OpenSheet(CommunityTabSheetType.PersonalChat, null);
                    break;
                case CommunityStatus.ClubChat:
                    if (isForceOpen == false && sheetManager.CurrentSheetType == CommunityTabSheetType.ClubChat) return;
                    sheetManager.OpenSheet(CommunityTabSheetType.ClubChat, null);
                    break;
            }
        }

        /// <summary>
        /// スタンプビューの表示を切り替える
        /// </summary>
        private void ChangeActiveStampView(bool visible)
        {
            stampViewRoot.gameObject.SetActive(visible);
            stampButtonBG.color = visible ?  ColorValueAssetLoader.Instance["default"] : Color.white;
            stampButtonIcon.color = visible ?  Color.white : ColorValueAssetLoader.Instance["default"];
        }

        /// <summary>
        /// 画面表示をCommunityStatusによって切り替える
        /// </summary>
        private void ChangeViewContent(CommunityStatus status)
        {
            var isChat = status is CommunityStatus.ClubChat or CommunityStatus.PersonalChat;
            chatDeleteButton.gameObject.SetActive(status == CommunityStatus.PersonalChat);

            chatMessageView.SetScrollerAlpha(isChat);
            sendButton.gameObject.SetActive(isChat);
            chatFooterViewRoot.SetActive(isChat);
            
            chatRoomViewRoot.SetActive(!isChat);
            updateButton.gameObject.SetActive(!isChat);
        }
        
        private void SetBadge()
        {
            yellHistoryBadge.SetActive(CommunityManager.ShowYellHistoryBadge);
            yellAllBadge.SetActive(CommunityManager.ShowYellAllBadge);
            personalChatBadge.SetActive(CommunityManager.ShowPersonalChatBadge);
            clubChatBadge.SetActive(CommunityManager.ShowClubChatBadge);
            AppManager.Instance.UIManager.Footer.UpdateHomeBadge();
        }
        
        private void SetStampScrollItems()
        {
            var ids = UserDataManager.Instance.userChatStamps;
            foreach (var id in ids)
            {
                if (stampInfoList.Any(info => info.stampId == id)) continue;
                ChatStampInfo newInfo = new ChatStampInfo { stampId = id, OnClickEvent = OnClickSendStampButton };
                stampInfoList.Add(newInfo);
            }
            stampScroll.SetItems(stampInfoList);
        }
        
        private void LoadStamp()
        {
            SetStampScrollItems();
            stampEmptyText.SetActive(stampInfoList.Count==0);
        }
        
        private async void SetChatRoomScrollItems(CommunityGetChatListAPIResponse response)
        {
            chatRoomInfoList.Clear();
            foreach (var modelsUChat in response.uChatList)
            {
                //ブロックユーザー非表示
                if(CommunityManager.blockUserList.Any(user=> user.uMasterId == modelsUChat.uMasterId || user.uMasterId == modelsUChat.fromUMasterId)) continue;
                ChatRoomInfo roomInfo = new ChatRoomInfo();
                roomInfo.uChat = modelsUChat;
                long userId = modelsUChat.uMasterId == UserDataManager.Instance.user.uMasterId ? modelsUChat.fromUMasterId : modelsUChat.uMasterId ;
                roomInfo.uStatus = response.chatUserStatusList.FirstOrDefault(s => s.uMasterId == userId);
                roomInfo.OnChatButtonClick = OnClickOpenPersonalChat;
                chatRoomInfoList.Add(roomInfo);
            }
            roomScroll.SetItems(chatRoomInfoList);
            
            //scroller item active状態更新のため,1 frame待ち
            await UniTask.NextFrame();
            cacheChatRoomScrollItems.Clear();
            foreach (Transform child in roomScroll.content)
            {
                if (!child.gameObject.activeSelf) continue;
                var item = child.GetComponent<ChatRoomScrollItem>();
                if(item != null) cacheChatRoomScrollItems.Add(item);
            }
        }

        private void UpdateChatTimer()
        {
            // 初期化が完了するまでは処理しない
            if(isInitialized == false)
            {
                return;
            }
            
            // API接続中は処理しない
            if(APIManager.Instance.isConnecting)
            {
                return;
            }
            
            // チャット更新時間更新
            chatUpdateTimer += Time.deltaTime;
            // 指定秒数経過した
            if(chatUpdateTimer >= chatUpdateInterval)
            {
                // タイマーリセット
                chatUpdateTimer = 0;
                FetchChatData(chatMessageView.CurrentTargetUMasterId, true).Forget();
            }
        }

        private async UniTask FetchChatData(long targetUMasterId = -1, bool isUpdate = false)
        {
            switch (currentStatus)
            {
                case CommunityStatus.PersonalChat:
                    if(targetUMasterId > 0) 
                        await GetPersonalChatDataAPI(targetUMasterId: targetUMasterId, isUpdate: isUpdate);
                    else
                        await GetPersonalChatDataAPI(chatMessageView.GetCurrentUMasterId(), isUpdate: isUpdate);
                    
                    // 未読チャット数で削除ボタンを更新
                    chatDeleteButton.interactable = chatMessageView.IsExistChatData();
                    
                    break;
                case CommunityStatus.ClubChat:
                    await GetClubChatDataAPI(isUpdate);
                    break;
                case CommunityStatus.ChatRoom:
                    await GetChatRoomDataAPI();
                    break;
            }

            if (isUpdate)
            {
                // 内容更新時には要素の更新をかける
                UpdateScrollItemView();
            }
            updateButton.SetClickIntervalTimer(updateButton.ClickTriggerInterval);
            SetBadge();
        }
        
        private async UniTask RefreshBadgesByTodayYell()
        {
            await CommunityManager.GetTodayYellDetailAPI();
            SetBadge();
        }
        private async UniTask GetBlockListAPI()
        {
            CommunityGetBlockListAPIRequest request = new CommunityGetBlockListAPIRequest();
            await APIManager.Instance.Connect(request);
        }
        
        private async UniTask GetChatRoomDataAPI()
        {
            CommunityGetChatListAPIRequest request = new CommunityGetChatListAPIRequest();
            await APIManager.Instance.Connect(request);
            CommunityGetChatListAPIResponse response = request.GetResponseData();
            currentStatus = CommunityStatus.ChatRoom;
            SetChatRoomScrollItems(response);
            chatRoomEmptyText.SetActive(chatRoomInfoList.Count == 0);
        }

        private async UniTask GetPersonalChatDataAPI(long targetUMasterId, bool isUpdate = false)
        {
            CommunityGetChatDetailAPIRequest request = new CommunityGetChatDetailAPIRequest();
            CommunityGetChatDetailAPIPost post = new CommunityGetChatDetailAPIPost()
            {
                targetUMasterId = targetUMasterId
            };

            // 差分取得の場合、最新のIDも送る
            if (isUpdate)
            {
                post.lastUChatId = chatMessageView.LastChatId;
            }

            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            //未読チャット数更新
            int index = chatRoomInfoList.FindIndex(c => c.uStatus.uMasterId == targetUMasterId);
            if (index >= 0)
            {
                chatRoomInfoList[index].uChat.readFlg = (int)ReadFlgType.Viewed;
                CommunityManager.unViewedChatCount = chatRoomInfoList.Count(c => c.uChat.fromUMasterId != UserDataManager.Instance.user.uMasterId && c.uChat.readFlg == (int)ReadFlgType.NotView);
            }

            CommunityGetChatDetailAPIResponse response = request.GetResponseData();
            
            List<ChatInfo> infoList = ConvertToChatInfoList(response.uChatList, response.chatUserStatus);
            
            if (!isUpdate)
            {
                OnInputFieldValueChanged("");
                await chatMessageView.InitializeMessage(infoList, response.chatUserStatus, response.chatUserStatus.uMasterId);
            }
            else
            {
                await chatMessageView.AddNewMessage(infoList, response.chatUserStatus);
            }
        }

        private async UniTask GetClubChatDataAPI(bool isUpdate = false)
        {
            CommunityGetGuildChatAPIRequest request = new CommunityGetGuildChatAPIRequest();
            CommunityGetGuildChatAPIPost post = new CommunityGetGuildChatAPIPost();
            
            // 差分取得の場合、最新のIDと日付も送る
            if (isUpdate)
            {
                post.lastGChatId = chatMessageView.LastChatId;
                post.lastLogCreatedAt = chatMessageView.LastLogCreatedAt;
            }
            
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CommunityGetGuildChatAPIResponse response = request.GetResponseData();
            
            List<ChatInfo> infoList = ConvertToChatInfoList(response.gChatList, response.gActionLogList, response.chatUserStatusList);

            if (!isUpdate)
            {
                OnInputFieldValueChanged("");
                await chatMessageView.InitializeMessage(infoList, response.chatUserStatusList);
            }
            else
            {
                await chatMessageView.AddNewMessage(infoList, response.chatUserStatusList);
            }
            
            currentStatus = CommunityStatus.ClubChat;
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
        }

        private async UniTask SendChatAPI(string body, long targetUMasterId, MessageType messageType = MessageType.Message)
        {
            CommunitySendChatAPIRequest request = new CommunitySendChatAPIRequest();
            CommunitySendChatAPIPost post = new CommunitySendChatAPIPost
            {
                type = (int) messageType,
                body = body,
                targetUMasterId = targetUMasterId,
                lastUChatId = chatMessageView.LastChatId,
            };
            
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CommunitySendChatAPIResponse response = request.GetResponseData();

            List<ChatInfo> infoList = ConvertToChatInfoList(response.uChatList, response.chatUserStatus);
            chatMessageView.UpdateMessage(infoList, response.chatUserStatus, response.chatUserStatus.uMasterId);
            
            OnInputFieldValueChanged("");
            chatDeleteButton.interactable = true;
        }

        private async UniTask SendClubChatAPI(string body, MessageType messageType = MessageType.Message)
        {
            CommunitySendGuildChatAPIRequest request = new CommunitySendGuildChatAPIRequest();
            CommunitySendGuildChatAPIPost post = new CommunitySendGuildChatAPIPost
            {
                type = (int) messageType,
                body = body,
                lastGChatId = chatMessageView.LastChatId,
                lastLogCreatedAt = chatMessageView.LastLogCreatedAt,
            };
            
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            CommunitySendGuildChatAPIResponse response = request.GetResponseData();

            List<ChatInfo> infoList = ConvertToChatInfoList(response.gChatList, response.gActionLogList, response.chatUserStatusList);
            chatMessageView.UpdateMessage(infoList, response.chatUserStatusList);
            
            OnInputFieldValueChanged("");
        }
        
        private async UniTask DeleteChatAPI(long targetUMasterId)
        {
            CommunityDeleteChatAPIRequest request = new CommunityDeleteChatAPIRequest();
            CommunityDeleteChatAPIPost post = new CommunityDeleteChatAPIPost { targetUMasterId = targetUMasterId };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            //チャット内容リセット
            chatMessageView.ClearMessage();
            chatDeleteButton.interactable = false;
        }
        
        private List<ChatInfo> ConvertToChatInfoList(ModelsUChat[] uChats, UserChatUserStatus chatUserStatus)
        {
            var chatList = new List<ChatInfo>();
            uChats.ForEach(uChat =>
            {
                bool isMyself = uChat.fromUMasterId == UserDataManager.Instance.user.uMasterId;
                
                string userName = isMyself ? UserDataManager.Instance.user.name : chatUserStatus.name;
                long mIconId = isMyself ? UserDataManager.Instance.user.mIconId : chatUserStatus.mIconId;
                chatList.Add(new ChatInfo(uChat, userName, mIconId));
            });
            return chatList;
        }
        
        private List<ChatInfo> ConvertToChatInfoList(ModelsGChat[] gChats, GuildLogGuildActionLog[] gActionLogs, UserChatUserStatus[] chatUserStatusList)
        {
            var chatList = new List<ChatInfo>();
            gChats.ForEach(gChat =>
            {
                var chatUserStatus = chatUserStatusList.FirstOrDefault(user => user.uMasterId == gChat.uMasterId);
                chatList.Add(new ChatInfo(gChat, chatUserStatus));
            });
            gActionLogs.ForEach(gActionLog =>
            {
                chatList.Add(new ChatInfo(gActionLog));
            });
            return chatList;
        }

        /// <summary>
        /// 個人チャット一覧では自動更新がかからないため、一定間隔で時間を更新
        /// </summary>
        private async UniTask UpdateChatRoomTimeText()
        {
            ReleaseCancelToken();
            
            updateTextCancellationTokenSource = new CancellationTokenSource();

            await CommunityManager.UpdateActionInterval(ChatTimeTextUpdateInterval, () =>
            {
                if (currentStatus == CommunityStatus.ChatRoom)
                {
                    cacheChatRoomScrollItems.ForEach(item=>item.SetTimeText());
                }
            }, cancellationTokenSource: updateTextCancellationTokenSource);
        }
        
        /// <summary>
        /// チャットの表示を更新する
        /// </summary>
        private void UpdateScrollItemView()
        {
            if (currentStatus is CommunityStatus.PersonalChat or CommunityStatus.ClubChat)
            {
                chatMessageView.RefreshScrollItem();
            }
        }
        
        private void ReleaseCancelToken()
        {
            if(updateTextCancellationTokenSource != null)
            {
                updateTextCancellationTokenSource.Cancel();
                updateTextCancellationTokenSource.Dispose();
                updateTextCancellationTokenSource = null;
            }
        }
    }
}