using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class NewInGameMatchUpStatusUI : MonoBehaviour
    {
        [SerializeField] private CharacterInGameIconImage characterIconImage;
        [SerializeField] private CharacterInGameIconImage characterIconShadowImage;
        [SerializeField] private CharacterRankImage characterRankImage;
        [SerializeField] private InGameStaminaParts staminaParts;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Image characterNameJp;
        [SerializeField] private Image characterNameEng;
        [SerializeField] private Color leftShadowColor;
        [SerializeField] private Color rightShadowColor;

        public void SetView(BattleCharacterModel character)
        {
            characterIconImage.SetTextureAsync(character.MCharaId).Forget();
            characterIconShadowImage.SetTextureAsync(character.MCharaId).Forget();
            characterRankImage.SetTextureAsync(character.Rank).Forget();
            characterIconShadowImage.SetColor(character.Side == BattleConst.TeamSide.Left ? leftShadowColor : rightShadowColor);
            staminaParts.SetUI(character.GetStaminaRate());
            characterNameText.text = character.Name;
        }
    }
}