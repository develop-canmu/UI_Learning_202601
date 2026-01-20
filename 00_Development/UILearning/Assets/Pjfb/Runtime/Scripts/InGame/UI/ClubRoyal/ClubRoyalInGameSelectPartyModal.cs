using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.ClubDeck;
using Pjfb.Combination;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSelectPartyModal : ModalWindow
    {
        public class Args
        {
            public int SpotId;
            public Args(int spotId)
            {
                SpotId = spotId;
            }
        }
        
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text cautionText;
        [SerializeField] private ScrollGrid scrollGrid;
        [SerializeField] private List<CharacterDeckImage> characterDeckImages;
        [SerializeField] private UIButton confirmButton;
        [SerializeField] private TMP_Text militaryStrengthText;
        [SerializeField] private Slider militaryStrengthSlider;
        [SerializeField] private UIButton minusButton;
        [SerializeField] private UIButton minButton;
        [SerializeField] private UIButton plusButton;
        [SerializeField] private UIButton maxButton;
        [SerializeField] private GameObject lockCombinationSkillButtonObject;
        [SerializeField] private ClubDeckEditTopPage deckEditTopPage;

        private List<GuildBattlePartyModel> setParties = new List<GuildBattlePartyModel>();
        private GuildBattlePartyModel currentParty;
        private int targetSpotId;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var openArgs = (Args)args;
            targetSpotId = openArgs.SpotId;
            var playerIndex = PjfbGuildBattleDataMediator.Instance.PlayerIndex;
            var parties = PjfbGuildBattleDataMediator.Instance.AllPjfbBattlePartyListByPlayerIndex[playerIndex];
            var data = new List<ClubRoyalInGameSelectPartyScrollItem.Arguments>();
            foreach (var party in parties)
            {
                data.Add(new ClubRoyalInGameSelectPartyScrollItem.Arguments(party, true));
                setParties.Add(party);
            }

            var spot = PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary[targetSpotId];
            if (spot.OccupyingSide == PjfbGuildBattleDataMediator.Instance.PlayerSide)
            {
                titleText.text = StringValueAssetLoader.Instance["clubroyalingame.select_party_modal_title_defence"];
                cautionText.text = StringValueAssetLoader.Instance["clubroyalingame.select_party_modal_title_caution_defence"];
            }
            else
            {
                titleText.text = StringValueAssetLoader.Instance["clubroyalingame.select_party_modal_title_attack"];
                cautionText.text = StringValueAssetLoader.Instance["clubroyalingame.select_party_modal_title_caution_attack"];
            }
            
            var playerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayerData(PjfbGuildBattleDataMediator.Instance.PlayerIndex);
            
            militaryStrengthSlider.minValue = 0;
            militaryStrengthSlider.maxValue = Mathf.Min(playerData.AvailableMilitaryStrength, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleMilitaryStrengthCaps[0]);
            militaryStrengthSlider.value = 0;

            scrollGrid.OnChangedPage += OnChangedPage;
            scrollGrid.SetItems(data);
            OnChangedPage(0, false);
            OnChangedSetBallCount(0);
            
            var isMatchSkillUnlocked = UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.CollectionSkill);
            lockCombinationSkillButtonObject.SetActive(!isMatchSkillUnlocked);
            
            return base.OnPreOpen(args, token);
        }

        private void OnChangedPage(int index)
        {
            OnChangedPage(index, true);
        }
        
        private void OnChangedPage(int index, bool playSE)
        {
            if (playSE)
            {
                SEManager.PlaySE(SE.se_common_slide);
            }
            
            var party = setParties[index];
            currentParty = party;
            var mCharaIds = new List<long>(currentParty.UCharaIds.Count);
            for (var i = 0; i < party.UCharaIds.Count; i++)
            {
                var battleChara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[party.UCharaIds[i]];
                characterDeckImages[i].SetTextureAsync(battleChara.mCharaId).Forget();
                mCharaIds.Add(battleChara.mCharaId);
            }

            EvaluateConfirmButtonInteractable();
            deckEditTopPage.SetCombinationMatchUi(mCharaIds);
        }

        public void OnClickConfirm()
        {
            var (isValid, message) = currentParty.EvaluatePartyCondition();
            if (!isValid)
            {
                var args = new ConfirmModalData(StringValueAssetLoader.Instance["common.error"], message, string.Empty, new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], modal => modal.Close()));
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, args);
                return;
            }
            
            RequestSendUnit().Forget();
        }

        private async UniTask RequestSendUnit()
        {
            confirmButton.interactable = false;
            var ballCount = (int)militaryStrengthSlider.value;
            var positionIds = new long[currentParty.UCharaIds.Count];
            for (var i = 0; i < currentParty.UCharaIds.Count; i++)
            {
                var charaData = PjfbGuildBattleDataMediator.Instance.BattleCharaData[currentParty.UCharaIds[i]];
                positionIds[i] = charaData.roleNumber;
            }
            
            var ret = await PjfbGameHubClient.Instance.RequestSendUnit(currentParty.Identifier, targetSpotId, currentParty.UCharaIds.ToArray(), ballCount, currentParty.TacticsId, positionIds);
            confirmButton.interactable = true;
            if (ret != null)
            {
                var spot = PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary[targetSpotId];
                var isDefence = spot.OccupyingSide == PjfbGuildBattleDataMediator.Instance.PlayerSide;
                var body = isDefence ?
                    StringValueAssetLoader.Instance["clubroyalingame.send_defence_party_toast"].Format(spot.GetSpotName()) :
                    StringValueAssetLoader.Instance["clubroyalingame.send_attack_party_toast"].Format(spot.GetSpotName());
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(body);
                
                AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                PjfbGuildBattleDataMediator.Instance.AllPjfbBattleParties[currentParty.Identifier].LocalUpdateFlag = true;
            }
            else
            {
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(StringValueAssetLoader.Instance["clubroyalingame.failed_to_send_party"]);
            }
        }

        public void OnClickPlus()
        {
            var value = Mathf.Clamp((int)militaryStrengthSlider.value + 1, (int)militaryStrengthSlider.minValue, (int)militaryStrengthSlider.maxValue);
            OnChangedSetBallCount(value);
        }
        
        public void OnClickMinus()
        {
            var value = Mathf.Clamp((int)militaryStrengthSlider.value - 1, (int)militaryStrengthSlider.minValue, (int)militaryStrengthSlider.maxValue);
            OnChangedSetBallCount(value);
        }
        
        public void OnClickMax()
        {
            var value = (int)militaryStrengthSlider.maxValue;
            OnChangedSetBallCount(value);
        }
        
        public void OnClickMin()
        {
            var value = (int)militaryStrengthSlider.minValue;
            OnChangedSetBallCount(value);
        }

        public void OnSliderValueChanged()
        {
            var value = (int)militaryStrengthSlider.value;
            OnChangedSetBallCount(value);
        }

        private void OnChangedSetBallCount(int count)
        {
            militaryStrengthText.text = $"{count}/{(int)militaryStrengthSlider.maxValue}";
            militaryStrengthSlider.SetValueWithoutNotify(count);
            
            minusButton.interactable = count > 0;
            minButton.interactable = count > 0;
            plusButton.interactable = count < militaryStrengthSlider.maxValue;
            maxButton.interactable = count < militaryStrengthSlider.maxValue;

            EvaluateConfirmButtonInteractable();
        }

        private void EvaluateConfirmButtonInteractable()
        {
            confirmButton.interactable = militaryStrengthSlider.value > 0 &&
                                         currentParty != null && !currentParty.IsOnMap() && !currentParty.IsDead();
        }

        public void OnClickComboSkillButton()
        {
            if (!CombinationManager.IsUnLockCombination())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(CombinationManager.CombinationLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            var args = new CombinationMatchModal.WindowParams();
            args.IdList = new List<long>();
            foreach (var uCharaId in currentParty.UCharaIds)
            {
                var chara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                args.IdList.Add(chara.mCharaId);
            }
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationMatch, args);
        }

        public void OnReceivePartyData()
        {
            setParties.Clear();
            var pageIndex = 0;
            var playerIndex = PjfbGuildBattleDataMediator.Instance.PlayerIndex;
            var parties = PjfbGuildBattleDataMediator.Instance.AllPjfbBattlePartyListByPlayerIndex[playerIndex];
            var data = new List<ClubRoyalInGameSelectPartyScrollItem.Arguments>();
            for (var i = 0; i < parties.Count; i++)
            {
                var party = parties[i];
                data.Add(new ClubRoyalInGameSelectPartyScrollItem.Arguments(party, true));
                setParties.Add(party);
                if (party.Identifier == currentParty.Identifier)
                {
                    pageIndex = i;
                }
            }

            scrollGrid.SetItems(data);
            scrollGrid.SetPage(pageIndex, false);
            OnChangedPage(pageIndex, false);
        }
        
        public void OnReceivePlayerData()
        {
            var playerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayerData(PjfbGuildBattleDataMediator.Instance.PlayerIndex);
            militaryStrengthSlider.maxValue = Mathf.Min(playerData.AvailableMilitaryStrength, PjfbGuildBattleDataMediator.Instance.GuildBattleSetting.GuildBattleMilitaryStrengthCaps[0]);
            OnChangedSetBallCount((int)militaryStrengthSlider.value);
        }
    }
}