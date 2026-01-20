using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.ClubDeck;
using Pjfb.ClubMatch;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Deck
{
    public class ClubDeckSummaryView : MonoBehaviour
    {
        [SerializeField] private RankPowerUI rankPowerUI;
        
        [SerializeField] private GameObject selectingObject;
        [SerializeField] private TextMeshProUGUI deckNameText;
        [SerializeField] private Image nameBackgroundImage;
        [SerializeField] private List<CharacterVariableIcon> characterIconList;
        
        [SerializeField] private ConditionView conditionView;
        [SerializeField] private GameObject cannotEditObject;

        private Action<long> onClick;
        private DeckData deckData;
        
        public void InitializeUI(DeckData deckData, bool isSelecting, Action<long> onClick)
        {
            this.deckData = deckData;
            this.onClick = onClick;
            rankPowerUI.InitializePartyRankUI(deckData.CombatPower);
            deckNameText.text = deckData.Deck.name;
            
            List<CharacterVariableDetailData> detailOrderList = new List<CharacterVariableDetailData>();
            int scrollIndex = 0;
            BigValue newTotalCombatPower = BigValue.Zero;
            
            
            for (int i = 0; i < characterIconList.Count; i++)
            {
                long memberId = deckData.GetMemberId(i); 
                if (memberId != DeckUtility.EmptyDeckSlotId)
                {
                    var uChara = UserDataManager.Instance.charaVariable.Find(memberId);
                    if (uChara is not null)
                    {
                        characterIconList[i].SwipeableParams = new SwipeableParams<CharacterVariableDetailData>(detailOrderList, scrollIndex++);
                        detailOrderList.Add(new CharacterVariableDetailData(uChara));
                        characterIconList[i].SetIconTextureWithEffectAsync(uChara.charaId).Forget(); 
                        characterIconList[i].SetIcon(uChara);
                        
                        BigValue newPower = BigValue.MulRate(new BigValue(uChara.combatPower), deckData.FixedClubConditionData.combatPowerAmplifier);
                        newTotalCombatPower += newPower;    
                    }
                    else
                    {
                        characterIconList[i].SetEmpty();
                    }
                }
                else
                {
                    characterIconList[i].SetEmpty();
                }
            }
            
            conditionView.SetCondition(deckData.FixedClubConditionData);
            selectingObject.SetActive(isSelecting);
            nameBackgroundImage.color = ColorValueAssetLoader.Instance[isSelecting ? "2117a2" : "145f8b"];
            cannotEditObject.SetActive(deckData.FixedClubConditionData.condition is ClubDeckCondition.Awful);
            if (deckData.FixedClubConditionData.condition == ClubDeckCondition.Good)
            {
                rankPowerUI.SetCombatPowerTextColor(ColorValueAssetLoader.Instance["default"]);
            }
            else
            {
                rankPowerUI.SetCombatPowerTextColor(deckData.FixedClubConditionData.combatPowerTextColor);    
            }

            rankPowerUI.InitializePartyCombatPowerOnly(newTotalCombatPower);

        }

        public void OnClick()
        {
            onClick?.Invoke(deckData.Index);
        }
    }
}