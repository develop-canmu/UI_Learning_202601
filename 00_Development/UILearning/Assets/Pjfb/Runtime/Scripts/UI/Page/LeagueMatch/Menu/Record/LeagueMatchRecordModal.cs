using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRecordModal : ModalWindow
    {
        
        [SerializeField]
        private RectTransform scrollContent = null;
        [SerializeField]
        private LeagueMatchRecordScrollItem scrollItemPrefab = null;
        
        [SerializeField]
        private GameObject emptyListText = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            LeagueMatchMenuModal.Arguments arguments = (LeagueMatchMenuModal.Arguments)args;
            
            
#if UNITY_EDITOR
            /*
            // ダミーデータ
            BattleReserveFormationHistory dummy = new BattleReserveFormationHistory();
            
            dummy.id = 1;
            dummy.groupInfo = new ColosseumGroupMinimum();
            dummy.groupInfo.name = "相手のクラブ名前";
            dummy.name = "相手の名前";
            dummy.mIconId = 10;
            dummy.result = 1;
            dummy.historyOption = new BattleReserveFormationHistoryOption();
            dummy.historyOption.battle = new BattleReserveFormationHistoryOptionBattle();
            dummy.historyOption.battle.pointGet = 1;
            dummy.historyOption.battle.pointLost = 2;
            dummy.historyOption.battle.winningPoint = 3;
            dummy.historyOption.label = new BattleReserveFormationHistoryOptionLabel();
            dummy.historyOption.label.dayNumber = 1;
            dummy.historyOption.label.roundNumber = 2;
            dummy.openAt = "2015:12:04";
            
            // Viewの生成
            LeagueMatchRecordScrollItem dummyItem = GameObject.Instantiate<LeagueMatchRecordScrollItem>(scrollItemPrefab, scrollContent);
            // アクティブ
            dummyItem.gameObject.SetActive(true);
            // データ表示
            dummyItem.SetData(dummy);
            */
#endif
            
            // 対戦履歴取得
            
            // Request
            
            BattleReserveFormationGetHistoryListAPIRequest request = new BattleReserveFormationGetHistoryListAPIRequest();
            // Post
            BattleReserveFormationGetHistoryListAPIPost post = new BattleReserveFormationGetHistoryListAPIPost();
            post.eventType = 1;
            post.eventId = arguments.SeasonId;
            post.lastId = -1;
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            // Response
            BattleReserveFormationGetHistoryListAPIResponse response = request.GetResponseData();
            
            // 履歴分リスト生成
            foreach(BattleReserveFormationHistory history in response.historyList)
            {
                // Viewの生成
                LeagueMatchRecordScrollItem item = GameObject.Instantiate<LeagueMatchRecordScrollItem>(scrollItemPrefab, scrollContent);
                // アクティブ
                item.gameObject.SetActive(true);
                // データ表示
                item.SetData(history, arguments.MColosseumEvent);
            }
            
            emptyListText.SetActive( response.historyList.Length <= 0 );
            
            await base.OnPreOpen(args, token);
        }
    }
}