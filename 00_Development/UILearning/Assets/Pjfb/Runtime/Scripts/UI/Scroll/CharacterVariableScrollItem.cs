using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using Pjfb.Extensions;

namespace Pjfb
{
   
    
    public class CharacterVariableScrollItem : ScrollGridItem
    {
        [SerializeField] private CharacterVariableIcon characterVariableIcon;
        [SerializeField] private GameObject favoriteImage;
        [SerializeField] private GameObject deckFormattingRoot;
        [SerializeField] private GameObject deckCurrentEditingRoot;
        [SerializeField] private GameObject impossibleEditRoot;
        [SerializeField] private GameObject assignedByOtherTeamRoot;
        [SerializeField] private GameObject reachMCharaLimitRoot;
        [SerializeField] private GameObject cannotEditRoot;
        [SerializeField] private Animator selectingAnimator;
        private CharacterVariableScrollData data = null;
        
        
        public void OnSelected()
        {
            TriggerEvent(data);
        }
        
        protected override void OnSetView(object value)
        {
            data = (CharacterVariableScrollData)value;
            // アイコンに登録
            characterVariableIcon.SetIconTextureWithEffectAsync(data.MCharaId).Forget();

            if (favoriteImage != null) favoriteImage.SetActive(data.IsFavorite);
            if(deckFormattingRoot != null) deckFormattingRoot.SetActive(data.DeckBadgeType == DeckBadgeType.Formatting);
            if(deckCurrentEditingRoot != null) deckCurrentEditingRoot.SetActive(data.DeckBadgeType == DeckBadgeType.CurrentEditing);
            if(assignedByOtherTeamRoot != null) assignedByOtherTeamRoot.SetActive(data.DeckBadgeType == DeckBadgeType.AssignedByOtherTeam);
            if(reachMCharaLimitRoot != null) reachMCharaLimitRoot.SetActive(data.DeckBadgeType == DeckBadgeType.ReachMCharaLimit);
            if(cannotEditRoot != null) cannotEditRoot.SetActive(data.DeckBadgeType is DeckBadgeType.CannotEdit);
            if(impossibleEditRoot != null) impossibleEditRoot.SetActive(data.DeckBadgeType is DeckBadgeType.AssignedByOtherTeam or DeckBadgeType.ReachMCharaLimit or DeckBadgeType.CannotEdit);
            

            if (selectingAnimator != null)
            {
                selectingAnimator.gameObject.SetActive(data.IsSelecting);
                if (data.IsSelecting) selectingAnimator.Play("Idle", -1, data.GetSelectEffectNormalizedTime?.Invoke() ?? 0);
            } 
                

            UserDataCharaVariable uChara = UserDataManager.Instance.charaVariable.Find(data.id);
            characterVariableIcon.SetIcon(uChara);
            characterVariableIcon.SetActiveRoleNumberIcon(false);
            characterVariableIcon.SwipeableParams = data.SwipeableParams;
        }
    }
}