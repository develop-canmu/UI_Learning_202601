using System;
using Pjfb.Colosseum;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using Pjfb.UI;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchFightItem : PoolListItemBase
    {
        public class ItemParams : ItemParamsBase
        {
            public BattleReserveFormationRoundMasterObject battleConfig;
            public BattleReserveFormationMatchLineup matchLineup;
            public long inGameMatchId;
            public bool hasUnsetAuthority;
            public ColosseumEventMasterObject mColosseumEvent;
            public Action entryTermEndedWarnPopup;
            public Func<bool> isWithEntryTerm;
            public Func<bool> enteredMax;
            public Action enteredMaxWarnPopup;
            public Action<long, ColosseumPlayerType> onLongTapUserIcon;
            
            // チーム登録系ポップアップ
            public Action<long, long, long, bool, Action> confirmClubMateEntry;
            public Action<long, long, long> confirmOtherClubEntry;
            public Action onCancelMateEntry;
        }
        
        [SerializeField] private TextMeshProUGUI battleName;
        [SerializeField] private TextMeshProUGUI winPointText;
        [SerializeField] private TextMeshProUGUI leftPlayerName;
        [SerializeField] private UIButton leftButton;
        [SerializeField] private GameObject rightEmptyButton;
        [SerializeField] private UserIcon leftCharaIcon;
        [SerializeField] private GameObject leftEmptyIcon;
        [SerializeField] private GameObject precedingFlagLeft;
        [SerializeField] private GameObject precedingFlagRight;
        [SerializeField] private GameObject normalHeader;
        [SerializeField] private GameObject bronzeHeader;
        [SerializeField] private GameObject silverHeader;
        [SerializeField] private GameObject goldHeader;
        
        
        ItemParams data;

        public override void Init(ItemParamsBase value)
        {
            data = (ItemParams) value;
            
            battleName.text = data.battleConfig.nameLabel;
            BattleImportanceLabelNumber battleImportanceLabelNumber = (BattleImportanceLabelNumber)data.battleConfig.importanceLabelNumber;
            normalHeader.SetActive(battleImportanceLabelNumber == BattleImportanceLabelNumber.Normal);
            bronzeHeader.SetActive(battleImportanceLabelNumber == BattleImportanceLabelNumber.Bronze);
            silverHeader.SetActive(battleImportanceLabelNumber == BattleImportanceLabelNumber.Silver);
            goldHeader.SetActive(battleImportanceLabelNumber == BattleImportanceLabelNumber.Gold);
            winPointText.text = string.Format(StringValueAssetLoader.Instance["league.win_point"], data.battleConfig.winningPoint);
            
            void OnClickEntry(BattleReserveFormationPlayerInfo playerInfo, bool myTeam = true)
            {
                if (myTeam)
                {
                    if (data.isWithEntryTerm())
                    {
                        if (playerInfo.player.playerId == 0)
                        {
                            if (data.enteredMax())
                            {
                                data.enteredMaxWarnPopup();
                            }
                            else
                            {
                                LeagueMatchManager.OnClickLeagueMatchDeckEditButton(data.inGameMatchId, data.matchLineup, data.mColosseumEvent);
                            }
                        }
                        else
                        {
                            if (data.hasUnsetAuthority)
                            {
                                // 対象が自分のチームの場合
                                if (playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId)
                                {
                                    LeagueMatchManager.OnClickLeagueMatchDeckEditButton(data.inGameMatchId, data.matchLineup, data.mColosseumEvent);
                                }
                                else
                                {
                                    // ◆自クラブメンバーの登録チーム確認ポップアップ（管理者）
                                    data.confirmClubMateEntry(data.inGameMatchId, 1, data.matchLineup.roundNumber, true, data.onCancelMateEntry);
                                }
                            }
                            else
                            {
                                // 対象が自分のチームの場合
                                if (playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId)
                                {
                                    LeagueMatchManager.OnClickLeagueMatchDeckEditButton(data.inGameMatchId, data.matchLineup, data.mColosseumEvent);
                                }
                                else
                                {
                                    // ◆自クラブメンバーの登録チーム確認ポップアップ_新規
                                    data.confirmClubMateEntry(data.inGameMatchId, 1, data.matchLineup.roundNumber, false, null);
                                }
                            }
                        }
                    }
                    else
                    {
                        data.entryTermEndedWarnPopup();
                    }
                }
                else
                {
                    if (playerInfo.player.playerId != 0)
                    {
                        // ◆他クラブメンバー登録チーム確認ポップアップ（戦力非公開）
                        data.confirmOtherClubEntry(data.inGameMatchId, 2, data.matchLineup.roundNumber);
                    }
                    else
                    {
                        // 何も起こらない
                    }
                }
            }
            
            leftButton.onClick.RemoveAllListeners();
            leftButton.onClick.AddListener(()=>
            {
                OnClickEntry(data.matchLineup.playerInfo, true);
            });
            
            rightEmptyButton.SetActive(true);
            
            leftEmptyIcon.SetActive(data.matchLineup.playerInfo.player.mIconId == 0);
            
            if (data.matchLineup.playerInfo.player.mIconId != 0)
            {
                leftCharaIcon.SetIconId(data.matchLineup.playerInfo.player.mIconId);
            }

            var unRegistrationString = StringValueAssetLoader.Instance["league.match.un_registration"];
            leftPlayerName.text = data.matchLineup.playerInfo.player.playerId != 0 ? data.matchLineup.playerInfo.player.name : unRegistrationString;
            
            precedingFlagLeft.SetActive(data.matchLineup.fieldOption.firstSide == 1);
            precedingFlagRight.SetActive(data.matchLineup.fieldOption.firstSide == 2);
        }

        public void OnLongTapUserIcon()
        {
            data.onLongTapUserIcon(data.matchLineup.playerInfo.player.playerId, (ColosseumPlayerType)data.matchLineup.playerInfo.player.playerType);
        }
    }
}