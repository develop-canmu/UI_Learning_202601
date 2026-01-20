using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb
{
    public class DeckCharacterView : MonoBehaviour
    {
        [SerializeField] private CharacterVariableIcon characterVariableIcon;
        [SerializeField] private RawImage emptyCharacterImage;

        private bool isEmpty;
        private int order = 0;

        private Action<int> onClickChara;
        private Action<int> onEdit = null;
        

        public void InitializeUI(int order, long id, RoleNumber role = RoleNumber.None)
        {
            this.order = order;
            if (id == DeckUtility.EmptyDeckSlotId)
            {
                InitializeCharaIcon(null, role);
            }
            else
            {
                UserDataCharaVariable chara = UserDataManager.Instance.charaVariable.Find(id);

                if (chara != null)
                {
                    characterVariableIcon.SetIcon(chara);
                    characterVariableIcon.SetActiveRoleNumberIcon(false);
                }
                    
                InitializeCharaIcon(chara?.MChara, role);
            }
        }

        public void SetNewCombatPower(BigValue newCombatPower)
        {
            characterVariableIcon.SetCombatPower(newCombatPower);
        }

        public void InitializeUI(int order, CharaVariableProfileStatus profileChara, RoleNumber role = RoleNumber.None)
        {
            this.order = order;

            if (profileChara != null)
            {
                characterVariableIcon.SetIcon(new BigValue(profileChara.combatPower), profileChara.rank);
            }
                
            
            var mChara = MasterManager.Instance.charaMaster.FindData(profileChara?.mCharaId ?? -1);
            InitializeCharaIcon(mChara, role);
            
        }


        private void InitializeCharaIcon(CharaMasterObject mChara, RoleNumber role)
        {
            isEmpty = mChara is null;

            if (isEmpty)
            {
                characterVariableIcon.SetEmpty();
                emptyCharacterImage.gameObject.SetActive(true);
            }
            else
            {
                emptyCharacterImage.gameObject.SetActive(false);
                
                characterVariableIcon.SetActiveImage(true);
                characterVariableIcon.SetActiveRankIcon(true);
                characterVariableIcon.SetActiveCombatPower(true);

                characterVariableIcon.SetIconTextureWithEffectAsync(mChara!.id).Forget();
            }

            if (role == RoleNumber.None)
            {
                characterVariableIcon.SetActiveRoleNumberIcon(false);
            }
            else
            {
                characterVariableIcon.SetActiveRoleNumberIcon(true);
                characterVariableIcon.SetRoleNumberIcon(role);
            }
        }

        public void SetDetailOrderList(SwipeableParams<CharacterVariableDetailData> swipeableParams)
        {
            characterVariableIcon.SwipeableParams = swipeableParams;   
        }
        
        public void SetActions(Action<int> onEdit, Action<int> onClickChara)
        {
            this.onEdit = onEdit;
            this.onClickChara = onClickChara;
        }
        
        public void OnClickEditButton()
        {
            onEdit(order);
        }

        public void OnClickCharaButton()
        {
            onClickChara?.Invoke(order);
        }

        public void OnLongTapCharacter()
        {
            if (isEmpty) return;
            characterVariableIcon.OnLongTap();
        }
    }
}