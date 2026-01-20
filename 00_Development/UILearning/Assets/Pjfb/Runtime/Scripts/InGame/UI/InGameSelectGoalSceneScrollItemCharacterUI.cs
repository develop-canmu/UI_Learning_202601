using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame
{
    public class InGameSelectGoalSceneScrollItemCharacterUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private CharacterVariableIcon characterIcon;
        [SerializeField] private GameObject captainIcon;
        [SerializeField] private GameObject offenceText;
        [SerializeField] private GameObject defenceText;
        [SerializeField] private Image baseImage;
        
        private Color LeftColor = new Color32(60, 89, 248, 255);
        private Color RightColor = new Color32(255, 0, 138, 255);

        public void SetData(BattleCharacterModel battleCharacterModel, bool isOffence)
        {
            characterNameText.text = battleCharacterModel.CharaMaster.name;
            characterIcon.SetIconTextureWithEffectAsync(battleCharacterModel.CharaMaster.id).Forget();
            characterIcon.SetIcon( battleCharacterModel.CombatPower, battleCharacterModel.Rank);
            characterIcon.SetRoleNumberIcon((RoleNumber)battleCharacterModel.Position);

            captainIcon.SetActive(battleCharacterModel.IsAceCharacter);
            offenceText.SetActive(isOffence);
            defenceText.SetActive(!isOffence);
            baseImage.color = battleCharacterModel.Side == BattleConst.TeamSide.Left ? LeftColor : RightColor;
        }
    }
}