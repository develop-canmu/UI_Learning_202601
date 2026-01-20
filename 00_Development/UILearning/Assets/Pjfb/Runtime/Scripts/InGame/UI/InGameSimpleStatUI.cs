using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using TMPro;

namespace Pjfb.InGame
{
    public class InGameSimpleStatUI : MonoBehaviour
    {
        [SerializeField] private CharacterVariableIcon characterIcon;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI activityPointText;
        [SerializeField] private TextMeshProUGUI goalCountText;
        [SerializeField] private TextMeshProUGUI assistCountText;
        [SerializeField] private GameObject captainIcon;
        [SerializeField] private UIButton button;

        private BattleCharacterModel characterModel;

        private void Awake()
        {
            button.OnClickEx.AddListener(OnClickButton);
        }

        public void SetData(BattleCharacterModel _characterModel)
        {
            characterModel = _characterModel;
            gameObject.SetActive(true);
            
            characterIcon.SetIconTextureWithEffectAsync(characterModel.MCharaId).Forget();
            characterIcon.SetIcon(characterModel.CombatPower, characterModel.Rank);
            characterIcon.SetRoleNumberIcon((RoleNumber)characterModel.Position);
            captainIcon.SetActive(characterModel.IsAceCharacter);
            characterNameText.text = characterModel.CharaMaster.name;
            activityPointText.text = characterModel.Stats.ActivityPoint.ToString();
            goalCountText.text = characterModel.Stats.GoalCount.ToString();
            assistCountText.text = characterModel.Stats.GoalAssistCount.ToString();
        }

        private async void OnClickButton()
        {
            await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.StatDetail, characterModel);
        }
    }
}