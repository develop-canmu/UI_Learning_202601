using System;
using Pjfb.Colosseum;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using Pjfb.Extensions;
using Pjfb.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchVSItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public BattleReserveFormationRoundMasterObject battleConfig;
            public BattleReserveFormationMatchLineup matchLineup;
            public Action<long, ColosseumPlayerType> onLongTapUserIcon;
        }
        #endregion
        
        [SerializeField] private TextMeshProUGUI battleName;
        [SerializeField] private UnityEngine.UI.Image battleNameHeaderImage;
        [SerializeField] private Color normalHeaderColor;
        [SerializeField] private Sprite normalHeaderSprite;
        [SerializeField] private Sprite important1Sprite;
        [SerializeField] private Sprite important2Sprite;
        [SerializeField] private Sprite important3Sprite;
        
        [SerializeField] private LeagueMatchPlayerTeamInfo playerInfoItem;
        [SerializeField] private LeagueMatchPlayerTeamInfo opponentPlayerInfoItem;
        [SerializeField] private TextMeshProUGUI termText;
        [SerializeField] private TextMeshProUGUI winPoint;
        [SerializeField] private TextMeshProUGUI battleScore;
        [SerializeField] private GameObject battleResultExtraFlg;
        [SerializeField] private TextMeshProUGUI battleResultExtraInfo;
        [SerializeField] private GameObject winFlg;
        [SerializeField] private GameObject loseFlg;
        [SerializeField] private GameObject drawFlg;
        [SerializeField] private GameObject vsBadge;
        [SerializeField] private GameObject battleEndedFlg;
        [SerializeField] private UIButton replayButton;
        
        public ItemParams data;
        
        public override void Init(ItemParamsBase value)
        {
            data = (ItemParams) value;
            battleName.text = data.battleConfig.nameLabel;
            BattleImportanceLabelNumber battleImportanceLabelNumber = (BattleImportanceLabelNumber)data.battleConfig.importanceLabelNumber;
            
            if (battleImportanceLabelNumber != BattleImportanceLabelNumber.Normal)
            {
                battleNameHeaderImage.color = Color.white;
            }
            else
            {
                battleNameHeaderImage.sprite = normalHeaderSprite;
                battleNameHeaderImage.color = normalHeaderColor;
            }
            
            if (battleImportanceLabelNumber == BattleImportanceLabelNumber.Bronze)
            {
                battleNameHeaderImage.sprite = important1Sprite;
            }
            else if (battleImportanceLabelNumber == BattleImportanceLabelNumber.Silver)
            {
                battleNameHeaderImage.sprite = important2Sprite;
            }
            else if (battleImportanceLabelNumber == BattleImportanceLabelNumber.Gold)
            {
                battleNameHeaderImage.sprite = important3Sprite;
            }
            
            BattleResult result = (BattleResult)data.matchLineup.result;
            battleEndedFlg.SetActive(result != BattleResult.Unprocessed);
            winPoint.text = string.Format(StringValueAssetLoader.Instance["league.win_point"], data.battleConfig.winningPoint);
            if (result != BattleResult.Unprocessed)
            {
                var notExistPlayerTeam = data.matchLineup.playerInfo.player.playerId == 0;
                var notExistEnemyTeam = data.matchLineup.playerInfoOpponent.player.playerId == 0;
                
                battleResultExtraFlg.SetActive(notExistPlayerTeam || notExistEnemyTeam);

                // 無効試合
                if (notExistPlayerTeam && notExistEnemyTeam)
                {
                    battleResultExtraInfo.text = StringValueAssetLoader.Instance["league.match.top.vs_item.draw_without_battle"];
                }
                // 不戦敗
                else if (notExistPlayerTeam)
                {
                    battleResultExtraInfo.text = StringValueAssetLoader.Instance["league.match.top.vs_item.lose_without_battle"];
                }
                // 不戦勝
                else if (notExistEnemyTeam)
                {
                    battleResultExtraInfo.text = StringValueAssetLoader.Instance["league.match.top.vs_item.win_without_battle"];
                }
                
                battleScore.text = string.Format(StringValueAssetLoader.Instance["league.match_result.point_result"], data.matchLineup.pointGet, data.matchLineup.pointLost);
                battleScore.gameObject.SetActive(true);
            }
            else
            {
                battleResultExtraFlg.SetActive(false);
                battleScore.gameObject.SetActive(false);
            }
            
            winFlg.SetActive(result == BattleResult.Win);
            loseFlg.SetActive(result == BattleResult.Lose);
            drawFlg.SetActive(result == BattleResult.Draw);
            vsBadge.SetActive(result == BattleResult.Unprocessed);
            
            var startTime = data.matchLineup.openAt.TryConvertToDateTime();
            termText.text = startTime.GetDateTimeString() + StringValueAssetLoader.Instance["start"];
            playerInfoItem.InitUI(data.matchLineup.playerInfo, data.matchLineup.fieldOption.firstSide == 1);
            opponentPlayerInfoItem.InitUI(data.matchLineup.playerInfoOpponent, data.matchLineup.fieldOption.firstSide == 2);
            
            if (data.matchLineup.previewId != 0)
            {
                replayButton.gameObject.SetActive(true);
                replayButton.onClick.RemoveAllListeners();
                replayButton.onClick.AddListener(
                    async () =>
                    {
                        await LeagueMatchManager.Instance.OnReplayButtonAsync(data.matchLineup.previewId);
                    }
                );
            }
            else
            {
                replayButton.gameObject.SetActive(false);
            }
        }
        
        public void OnLongTapUserIcon(bool left)
        {
            if (left)
                data.onLongTapUserIcon(data.matchLineup.playerInfo.player.playerId, (ColosseumPlayerType)data.matchLineup.playerInfo.player.playerType);
            else
                data.onLongTapUserIcon(data.matchLineup.playerInfoOpponent.player.playerId, (ColosseumPlayerType)data.matchLineup.playerInfoOpponent.player.playerType);
        }
    }
}