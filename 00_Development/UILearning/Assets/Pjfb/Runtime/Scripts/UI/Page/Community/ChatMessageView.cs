using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Community
{
    public class ChatMessageView : MonoBehaviour
    {
        [SerializeField]
        private ScrollDynamic scrollDynamic;
        
        [SerializeField]
        private float chatUpdateScrollThreshold = 150f;
        
        [SerializeField]
        private GameObject chatEmptyObject;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private List<ChatInfo> orderedInfoList = new List<ChatInfo>();
        
        public long CurrentTargetUMasterId => targetUMasterId;
        public string LastLogCreatedAt => lastLogCreatedAt;
        public long LastChatId => lastChatId;

        private long lastChatId;
        private string lastLogCreatedAt;
        
        private long targetUMasterId = -1;

        private Dictionary<long, UserChatUserStatus> userStatusMap = new Dictionary<long, UserChatUserStatus>();
        
        /// <summary>
        /// スクローラを初期化してアイテム生成を行う（個人チャット）
        /// </summary>
        public async UniTask InitializeMessage(List<ChatInfo> infoData, UserChatUserStatus targetStatus, long targetUMasterId)
        {
            // 変数初期化
            orderedInfoList.Clear();
            userStatusMap.Clear();
            lastLogCreatedAt = string.Empty;
            lastChatId = 0;
            
            this.targetUMasterId = targetUMasterId;
            
            UpdateUserStatus(targetStatus);
            
            await InitializeMessageView(infoData);
        }
        
        /// <summary>
        /// スクローラを初期化してアイテム生成を行う
        /// </summary>
        public async UniTask InitializeMessage(List<ChatInfo> infoData, UserChatUserStatus[] targetStatus)
        {
            // 変数初期化
            orderedInfoList.Clear();
            userStatusMap.Clear();
            lastLogCreatedAt = string.Empty;
            lastChatId = 0;
            
            UpdateUserStatus(targetStatus);

            await InitializeMessageView(infoData);
        }

        /// <summary>
        /// 画面初期化の共通処理
        /// </summary>
        private async UniTask InitializeMessageView(List<ChatInfo> infoData)
        {
            // 共通データの設定
            scrollDynamic.CommonItemValue = userStatusMap;
            
            // データ更新
            orderedInfoList = AddDiffOrderedInfoList(infoData);
            
            // アイテムの設定
            scrollDynamic.SetItems(orderedInfoList);

            // 自動レイアウトの反映が間に合わないので1フレ待つ
            await UniTask.NextFrame();
            
            // スクロールする
            SetScrollPosition();
            
            // 空の場合のテキスト表示
            chatEmptyObject.SetActive(scrollDynamic.ItemList.Count == 0);
        }

        /// <summary>
        /// スクローラにアイテムの差分を追加する
        /// </summary>
        public async UniTask AddNewMessage(List<ChatInfo> infoData, UserChatUserStatus targetStatus, bool isForceScroll = false)
        {
            UpdateUserStatus(targetStatus);

            await AddNewMessageView(infoData, isForceScroll);
        }

        /// <summary>
        /// スクローラにアイテムの差分を追加する
        /// </summary>
        public async UniTask AddNewMessage(List<ChatInfo> infoData, UserChatUserStatus[] targetStatus, bool isForceScroll = false)
        {
            UpdateUserStatus(targetStatus);

            await AddNewMessageView(infoData, isForceScroll);
        }
        
        /// <summary>
        /// 差分追加の共通処理
        /// </summary>
        private async UniTask AddNewMessageView(List<ChatInfo> infoData, bool isForceScroll = false)
        {
            orderedInfoList = infoData;
            // 要素追加前のスクロール状態を取得
            var isLookLatest = IsLookLatest();

            if (!infoData.Any())
            {
                return;
            }
            
            var list = AddDiffOrderedInfoList(infoData);
            scrollDynamic.AddItemRange(list);
            
            // 最下部にいる場合は強制スクロール
            if (isLookLatest || isForceScroll)
            {
                await UniTask.NextFrame();
                SetScrollPosition();
            }
            
            chatEmptyObject.SetActive(scrollDynamic.ItemList.Count == 0);
        }
        
        /// <summary>
        /// チャット画面上のユーザの情報を更新する
        /// </summary>
        private void UpdateUserStatus(UserChatUserStatus status)
        {
            if (userStatusMap.TryAdd(status.uMasterId, status)) return;
            
            userStatusMap[status.uMasterId] = status;
        }
        
        /// <summary>
        /// チャット画面上のユーザの情報を更新する
        /// </summary>
        private void UpdateUserStatus(UserChatUserStatus[] statusList)
        {
            statusList.ForEach(status =>
            {
                if (userStatusMap.TryAdd(status.uMasterId, status)) return;
                
                userStatusMap[status.uMasterId] = status;
            });
        }

        /// <summary>
        /// チャット画面の中身だけ消去する
        /// </summary>
        public void ClearMessage()
        {
            orderedInfoList.Clear();
            lastLogCreatedAt = string.Empty;
            lastChatId = 0;
            
            scrollDynamic.SetItems(orderedInfoList);
            
            // 空の場合のテキストを表示
            chatEmptyObject.SetActive(true);
        }

        /// <summary>
        /// 引数の情報でチャット画面を更新する
        /// </summary>
        public void UpdateMessage(List<ChatInfo> info, UserChatUserStatus status, long targetUMasterId)
        {
            if (scrollDynamic.ItemList == null)
            {
                // 初めてメッセージを送る場合
                InitializeMessage(info, status, targetUMasterId).Forget();
            }
            else
            {
                AddNewMessageView(info, true).Forget();
            }
        }
        
        /// <summary>
        /// 引数の情報でチャット画面を更新する
        /// </summary>
        public void UpdateMessage(List<ChatInfo> info, UserChatUserStatus[] status)
        {
            if (scrollDynamic.ItemList == null)
            {
                // 初めてメッセージを送る場合
                InitializeMessage(info, status).Forget();
            }
            else
            {
                AddNewMessageView(info, true).Forget();
            }
        }
        
        /// <summary>
        /// チャットのスクロールを最下部まで移動する
        /// </summary>
        public void SetScrollPosition()
        {
            // 最新のアイテムを読み込む
            scrollDynamic.ForceLoadAll();
            
            scrollDynamic.ScrollToEnd();
        }
        
        /// <summary>
        /// チャットデータが存在するかどうか
        /// </summary>
        public bool IsExistChatData()
        {
            return scrollDynamic.ItemList?.Count > 0;
        }
        
        private List<ChatInfo> AddDiffOrderedInfoList(List<ChatInfo> diff)
        {
            diff = diff.OrderBy(data => CommunityManager.GetDateTimeByString(data.CreatedAt)).ToList();
            
            // 最新のチャットのIDを取得
            var chatList = diff.Where(x => x.Type != ChatInfo.ChatType.ClubLog).ToList();
            if (chatList.Any())
            {
                lastChatId = chatList.Last().ChatId;
            }
                
            // 最新のクラブログの日付を取得
            var logList = diff.Where(x => x.Type == ChatInfo.ChatType.ClubLog).ToList();
            if (logList.Any())
            {
                lastLogCreatedAt = logList.Last().CreatedAt;
            }

            return diff;
        }

        private bool IsLookLatest()
        {
            float contentHeight = scrollDynamic.content.sizeDelta.y - scrollDynamic.viewport.rect.height;
            // スクロール量
            float scrollValue = contentHeight * scrollDynamic.normalizedPosition.y;
            // 指定量スクロールされている
            return scrollValue < chatUpdateScrollThreshold;
        }
        
        /// <summary>
        /// 個人チャットのやり取りの相手を返す
        /// </summary>
        /// <returns></returns>
        public long GetCurrentUMasterId()
        {
            return targetUMasterId;
        }

        /// <summary>
        /// 個人チャットのやり取りの相手の名前を返す
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUserName()
        {
            return userStatusMap.TryGetValue(targetUMasterId, out var status) ? status.name : string.Empty;
        }
        
        /// <summary>
        /// アイテム全体の表示に更新をかける
        /// </summary>
        public void RefreshScrollItem()
        {
            // チャットデータがない場合は何もしない
            if (IsExistChatData() == false)
            {
                return;
            }
            scrollDynamic.RefreshItemView();
        }
        
        /// <summary>
        /// チャットUIの透明度を変える
        /// （自動レイアウト計算のため表示非表示をSetActiveではなくCanvasGroupで行う）
        /// </summary>
        public void SetScrollerAlpha(bool isVisible)
        {
            if (canvasGroup != null) canvasGroup.alpha = isVisible ? 1 : 0;
        }
    }
}