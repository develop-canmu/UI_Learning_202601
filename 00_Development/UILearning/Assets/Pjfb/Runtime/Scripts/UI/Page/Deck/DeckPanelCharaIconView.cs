using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class DeckPanelCharaIconView : MonoBehaviour
    {
        #region Params
        public class ViewParams
        {
            public CharacterVariableDetailData nullableCharacterData;
            public long type;
            public RoleNumber position;
            public SwipeableParams<CharacterVariableDetailData> swipeableParams;
            public long boostValue = 0;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private CharacterVariableIcon charaIcon;
        
        #endregion

        #region PrivateFields
        private ViewParams _viewParams;
        #endregion
        
        #region PublicMethods
        public void Init()
        {
            
        }

        public void SetDisplay(ViewParams viewParams)
        {
            _viewParams = viewParams;
            if (viewParams.nullableCharacterData == null) charaIcon.gameObject.SetActive(false);
            else {
                charaIcon.gameObject.SetActive(true);
                charaIcon.SetIcon(viewParams.nullableCharacterData);
                charaIcon.SetIconTextureWithEffectAsync(viewParams.nullableCharacterData.MCharaId).Forget();
                charaIcon.SwipeableParams = viewParams.swipeableParams;
                charaIcon.SetBoostEffect(viewParams.boostValue);
            }

            if (viewParams.position != RoleNumber.None) charaIcon.SetRoleNumberIcon(viewParams.position);
            else charaIcon.SetActiveRoleNumberIcon(false);
        }
        #endregion
    }
}