using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchPlayerTeamInfo : MonoBehaviour
    {
        [SerializeField] private RectTransform unRegistrationParent;
        [SerializeField] private RectTransform registrationParent;
        [SerializeField] private GameObject precedingFlg; 
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private TextMeshProUGUI teamScore;
        [SerializeField] private OmissionTextSetter teamScoreOmissionTextSetter;
        [SerializeField] private DeckRankImage deckRankImage;
        [SerializeField] private UserIcon userIcon;
        [SerializeField] private List<CharacterVariableIcon> charaIcons;

        public void InitUI(BattleReserveFormationPlayerInfo info, bool firstSide)
        {
            // コンパイルエラーになってたのでコメントアウト
            unRegistrationParent.gameObject.SetActive(info.player.playerId == 0);
            registrationParent.gameObject.SetActive(info.player.playerId != 0);
            precedingFlg.SetActive(firstSide);
            BigValue combatPower = new BigValue(info.combatPower);
            if (info.player.playerId != 0)
            {
                playerName.text = info.player.name;
                if (combatPower == -1)
                {
                    deckRankImage.gameObject.SetActive(false);
                    teamScore.text = "??????????";
                }
                else
                {
                    deckRankImage.SetTextureAsync(info.rank).Forget();
                    deckRankImage.gameObject.SetActive(true);
                    teamScore.text = combatPower.ToDisplayString(teamScoreOmissionTextSetter.GetOmissionData());
                }
                
                if (info.player.mIconId != 0)
                {
                    userIcon.SetIconId(info.player.mIconId);
                }
                else
                {
                    long iconId = LeagueMatchUtility.GetNpcIconId(info.player.name);
                    if(iconId > 0)
                    {
                        userIcon.SetIconId(iconId);
                    }
                }
                
                if (info.charaVariableList != null)
                    for (var index =0; index < info.charaVariableList.Length; index++)
                    {
                        var charaVariable = info.charaVariableList[index];
                        charaIcons[index].SetIconTextureWithEffectAsync(charaVariable.mCharaId).Forget();
                        charaIcons[index].SetIcon(new BigValue(combatPower == -1 ? -1 : charaVariable.combatPower), 
                            charaVariable.rank, (RoleNumber)charaVariable.roleNumber);
                    }
            }
        }
    }
}