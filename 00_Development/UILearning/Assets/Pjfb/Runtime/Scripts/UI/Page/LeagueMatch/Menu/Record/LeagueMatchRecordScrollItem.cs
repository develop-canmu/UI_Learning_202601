using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.InGame;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRecordScrollItem : MonoBehaviour
    {
    
        [SerializeField]
        private GameObject deckRoot = null;
        
        [SerializeField]
        private TMPro.TextMeshProUGUI titleText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI pointText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI winPointText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI dateText = null;
        
        [SerializeField]
        private UserIcon opponentUserIcon = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI opponentClubNameText = null; 
        [SerializeField]
        private TMPro.TextMeshProUGUI opponentUserNameText = null; 
        
        
        [SerializeField]
        private CharacterVariableIcon[] deckCharacterIcons = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI deckTotalPowerText = null;
        [SerializeField]
        private OmissionTextSetter deckTotalPowerOmissionTextSetter = null;
        [SerializeField]
        private DeckRankImage deckRankImage = null;
        
        [SerializeField]
        private GameObject winBadge = null;
        [SerializeField]
        private GameObject loseBadge = null;
        [SerializeField]
        private GameObject drawBadge = null;
        
        private BattleReserveFormationHistory historyData = null;
        private ColosseumDeck deckData = null;
        
        /// <summary>データ表示</summary>
        public void SetData(BattleReserveFormationHistory history, ColosseumEventMasterObject mColosseumEvent)
        {
            historyData = history;
            
            // マスタ
            BattleReserveFormationMasterObject mBattleReserveFormation = MasterManager.Instance.battleReserveFormationMaster.FindData(historyData.optionLabel.mBattleReserveFormationId);
            // ラウンドマスタ
            BattleReserveFormationRoundMasterObject mBattleReserveFormationRound = MasterManager.Instance.battleReserveFormationRoundMaster.values
                .Where(v => v.mBattleReserveFormationRoundGroupId == mBattleReserveFormation.mBattleReserveFormationRoundGroupId)
                .Where(v => v.roundNumber == historyData.optionLabel.roundNumber)
                .FirstOrDefault();
            
            // タイトル
            if(history.optionLabel.matchType == (int)MatchType.Season)
            {
                // 簡易大会
                if(mColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
                {
                    titleText.text = mColosseumEvent.name + string.Format( StringValueAssetLoader.Instance["league.match.history_name_add"], history.optionLabel.dayNumber, mBattleReserveFormationRound.nameLabel);
                }
                // リーグマッチ
                else
                {
                    titleText.text = string.Format( StringValueAssetLoader.Instance["league.match.history_name_1"], history.optionLabel.dayNumber, mBattleReserveFormationRound.nameLabel);
                }
            }
            // 入れ替え戦
            else
            {
                titleText.text = string.Format( StringValueAssetLoader.Instance["league.match.history_name_2"], mBattleReserveFormationRound.nameLabel);
            }
            // 得点
            pointText.text = string.Format( StringValueAssetLoader.Instance["league.match_result.point_result"], history.resultDetail.pointGet, history.resultDetail.pointLost);
            // 勝ち点
            winPointText.text = history.winningPoint.ToString();
            // 日付
            dateText.text = history.openAt.TryConvertToDateTime().ToString("MM/dd HH:mm");
            
            // 相手のアイコン
            opponentUserIcon.SetIconId( history.mIconId );
            // 相手のクラブ名
            opponentClubNameText.text = history.groupInfo.name;
            // 対戦帯の名前
            opponentUserNameText.text = history.name;
            
            // 結果アイコン
            winBadge.SetActive(false);
            loseBadge.SetActive(false);
            drawBadge.SetActive(false);
            
            switch(history.result)
            {
                case ColosseumManager.ResultWin:
                {
                    winBadge.SetActive(true);
                    break;
                }
                case ColosseumManager.ResultLose:
                {
                    loseBadge.SetActive(true);
                    break;
                }
                case ColosseumManager.ResultDraw:
                {
                    drawBadge.SetActive(true);
                    break;
                }
            }
            
        }
        
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnReplayButton()
        {
            OnReplayButtonAsync().Forget();
        }
        
        private async UniTask OnReplayButtonAsync()
        {
            // Request
            
            BattleReserveFormationGetBattlePreviewDataFromHistoryAPIRequest request = new BattleReserveFormationGetBattlePreviewDataFromHistoryAPIRequest();
            // Post
            BattleReserveFormationGetBattlePreviewDataFromHistoryAPIPost post = new BattleReserveFormationGetBattlePreviewDataFromHistoryAPIPost();
            post.id = historyData.id;
            request.SetPostData(post);
            // API
            await APIManager.Instance.Connect(request);
            // Response
            BattleReserveFormationGetBattlePreviewDataFromHistoryAPIResponse response = request.GetResponseData();
            
            // 閉じる
            AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
            
            // 現在のページと引数
            PageType pageType = AppManager.Instance.UIManager.PageManager.CurrentPageType;
            object openArguments = AppManager.Instance.UIManager.PageManager.CurrentPageObject.OpenArguments;
            // インゲームへ
            NewInGameOpenArgs args = new NewInGameOpenArgs(pageType, response.srcData, response.destData, response.previewIndex, openArguments);
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false, args);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnTeamButton(bool isOn)
        {
            if(isOn)
            {
                UpdateDeckAsync().Forget();
            }
            else
            {
                deckRoot.SetActive(false);
            }
            
        }
        
        private async UniTask UpdateDeckAsync()
        {
            // デッキ情報未取得の場合は取得
            if(deckData == null)
            {
                deckData = await ColosseumManager.RequestBattleReserveFormationGetHistoryDeckAsync(historyData.id);                
            }
            
            BigValue totalCombatPower = BigValue.Zero;
            // デッキのキャラ表示
            for(int i=0;i<deckData.charaList.Length;i++)
            {
                // キャラ情報
                ColosseumDeckChara charData = deckData.charaList[i];
                // アイコン
                CharacterVariableIcon charIcon = deckCharacterIcons[i];
                // アイコンの表示
                charIcon.SetIconTextureWithEffectAsync(charData.mCharaId ).Forget();
                BigValue combatPower = new BigValue(charData.combatPower);
                charIcon.SetIcon(combatPower, charData.rank, (RoleNumber)charData.roleNumber);
                
                // 総合力
                totalCombatPower += combatPower;
            }
            
            // ランク
            deckRankImage.SetTexture( StatusUtility.GetPartyRank(totalCombatPower) );
            // 総合力
            deckTotalPowerText.text = totalCombatPower.ToDisplayString(deckTotalPowerOmissionTextSetter.GetOmissionData());

            // 表示
            deckRoot.SetActive(true);
        }
    }
}