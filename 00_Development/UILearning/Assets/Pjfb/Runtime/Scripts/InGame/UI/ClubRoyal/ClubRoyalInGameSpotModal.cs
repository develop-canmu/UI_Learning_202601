using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Storage;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotModal : ModalWindow
    {
        [SerializeField] private TMP_Text modalTitleText;
        [SerializeField] private TMP_Text spotNameText;
        [SerializeField] private TMP_Text myTeamTabText;
        [SerializeField] private TMP_Text confirmButtonText;
        [SerializeField] private UIButton confirmButton;
        [SerializeField] private ClubRoyalInGameSpotModalSheetAllTeam allTeamSheet;
        [SerializeField] private ClubRoyalInGameSpotModalSheetMyTeam myTeamSheet;
        [SerializeField] private Image goalImage;
        [SerializeField] private Sprite[] goalImageSprites;

        private int spotId;
        
        public class Arguments
        {
            public int SpotId;
            
            public Arguments(int spotId)
            {
                SpotId = spotId;
            }
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var openArgs = (Arguments)args;
            spotId = openArgs.SpotId;
            var spotModel = (GuildBattleMapSpotModel)PjfbGuildBattleDataMediator.Instance.BattleField.GetMapSpot(spotId);
            var playerSide = PjfbGuildBattleDataMediator.Instance.PlayerSide;
            
            spotNameText.text = spotModel.GetSpotName();
            goalImage.sprite = goalImageSprites[spotModel.OccupyingSide == playerSide ? 0 : 1];
            
            UpdateView();
            
            var isAllySpot = spotModel.OccupyingSide == playerSide;
            if (isAllySpot)
            {
                modalTitleText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_title_defence"];
                myTeamTabText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_tab_name_defence_my_team"];
                confirmButtonText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_button_text_defence"];
            }
            else
            {
                modalTitleText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_title_attack"];
                myTeamTabText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_tab_name_attack_my_team"];
                confirmButtonText.text = StringValueAssetLoader.Instance["clubroyalingame.spot_modal_button_text_attack"];    
            }
            
            return base.OnPreOpen(args, token);
        }
        
        private (List<GuildBattlePartyModel>, List<GuildBattlePartyModel>, List<GuildBattlePartyModel>) GetPartyList(GuildBattleCommonConst.GuildBattleTeamSide playerSide, GuildBattleMapSpotModel spotModel)
        {
            var isAllySpot = playerSide == spotModel.OccupyingSide;
            var playerIndex = PjfbGuildBattleDataMediator.Instance.PlayerIndex;
            var partyList = new List<GuildBattlePartyModel>();
            var myPartyList = new List<GuildBattlePartyModel>();
            var myDeadPartyList = new List<GuildBattlePartyModel>();
            foreach (var kvp in PjfbGuildBattleDataMediator.Instance.OnMapPjfbBattleParties)
            {
                var party = kvp.Value;
                if (party.PlayerIndex == playerIndex)
                {
                    // 復活軍隊一覧は拠点関係なく表示.
                    if (!party.IsOnMap() && !party.LocalUpdateFlag)
                    {
                        myDeadPartyList.Add(party);
                    }
                    // 自分の軍隊一覧は選択した拠点を目標としていた場合に表示.
                    else if (party.TargetSpotId == spotModel.Id && !party.LocalUpdateFlag)
                    {
                        myPartyList.Add(party);
                    }
                }

                // 軍隊一覧には選択した拠点を目標としていた場合に表示
                // 味方/敵の拠点でそれぞれの陣営のもののみ表示.
                if (party.IsOnMap() && party.TargetSpotId == spotModel.Id && !party.LocalUpdateFlag)
                {
                    if ((isAllySpot && party.Side == playerSide) ||
                        (!isAllySpot && party.Side != playerSide))
                    {
                        partyList.Add(party);
                    }
                }
            }

            return (partyList, myPartyList, myDeadPartyList);
        }

        public void OnClickOKButton()
        {
            var isDefence = PjfbGuildBattleDataMediator.Instance.BattleField.GetMapSpot(spotId).OccupyingSide == PjfbGuildBattleDataMediator.Instance.PlayerSide;
            if (isDefence && !LocalSaveManager.saveData.isPartyBalanceAnnounceModalOpened)
            {
                var confirmModalArgs = new ConfirmModalData(
                    StringValueAssetLoader.Instance["clubroyalingame.party_balance_modal_title"],
                    StringValueAssetLoader.Instance["clubroyalingame.party_balance_modal_body"],
                    string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        modal =>
                        {
                            LocalSaveManager.saveData.isPartyBalanceAnnounceModalOpened = true;
                            LocalSaveManager.Instance.SaveData();
                            modal.Close();
                        }));

                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, confirmModalArgs);
                return;
            }
            
            var args = new ClubRoyalInGameSelectPartyModal.Args(spotId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameSelectParty, args);
        }

        public void OnReceiveBattleTurnResult()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var spotModel = (GuildBattleMapSpotModel)PjfbGuildBattleDataMediator.Instance.BattleField.GetMapSpot(spotId);
            var playerSide = PjfbGuildBattleDataMediator.Instance.PlayerSide;
            var (allPartyList, myPartyList, myDeadPartyList) = GetPartyList(playerSide, spotModel);
            
            allTeamSheet.SetData(allPartyList);
            myTeamSheet.SetData(myPartyList);
            
            var isAllySpot = spotModel.OccupyingSide == playerSide;
            
            // 敵拠点への進行は試合が開始してから.
            if (!isAllySpot)
            {
                confirmButton.interactable = PjfbGuildBattleDataMediator.Instance.GameState == GuildBattleCommonConst.GuildBattleGameState.InFight;
            }
        }
    }
}