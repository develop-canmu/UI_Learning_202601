using System;
using System.Collections.Generic;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CharacterVariableIcon : ItemIconBase
    {
    
        public enum CharacterType
        {
            VariableCharacter,
            TrainingCharacter,
            OpponentCharacter
        }
    
        public override ItemIconType IconType { get { return ItemIconType.VariableCharacter; } }
        
        
        [SerializeField]
        private CharacterType charType = CharacterType.VariableCharacter;
        /// <summary>キャラタイプ</summary>
        public CharacterType CharType{get{return charType;}set{charType = value;}}
        
        [SerializeField]
        private bool openDetailModal = true;
        /// <summary>詳細モーダルを開くか</summary>
        public bool OpenDetailModal
        {
            get { return  openDetailModal; }
            set { openDetailModal = value; }
        }
        
        [SerializeField]
        private bool canOpenCharacterEncyclopediaPage = true;
        
        [SerializeField]
        private TextMeshProUGUI combatPowerText;
        
        [SerializeField]
        private OmissionTextSetter omissionTextSetter;
        
        [SerializeField]
        private ImageEffect iconImageEffect = null;
        
        [SerializeField]
        private CharacterRoleNumberImage roleNumberIcon = null;
        
        [SerializeField]
        private CharacterRankImage rankIcon = null;
        
        [SerializeField]
        private GameObject combatPowerRoot = null;
        
        [SerializeField]
        private GameObject boostFrameObject = null;
        
        [SerializeField]
        private GameObject boostTextRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI boostText = null;
        
        private CharacterVariableDetailData iconData = null;
        public void SetIcon(CharacterVariableDetailData data, bool coverUpFlg = false)
        {
            SetCombatPower(data.CombatPower, coverUpFlg);
            SetRankIcon(data.Rank);
            iconData = data;
        }
        
        public void SetIcon(UserDataCharaVariable uCharaVariable)
        {
            if (uCharaVariable is null) SetEmpty();
            else SetIcon(new CharacterVariableDetailData(uCharaVariable));
        }
        
        
        public void SetIcon(CharaNpcMasterObject mCharaNpc)
        {
            SetIcon(new CharacterVariableDetailData(mCharaNpc));
        }
        
        public void SetIcon(CharacterVariableDetailData data, RoleNumber roleNumber)
        {
            SetIcon(data);
            SetRoleNumberIcon(roleNumber);
        }
        
        public void SetIcon(BigValue combatPower, long rank, bool coverUpFlg = false)
        {
            SetCombatPower(combatPower, coverUpFlg);
            SetRankIcon(rank);
        }
        
        public void SetIcon(BigValue combatPower, long rank, RoleNumber roleNumber, bool coverUpFlg = false)
        {
            SetIcon(combatPower, rank, coverUpFlg);
            SetRoleNumberIcon(roleNumber);
        }
        
        /// <summary>エフェクト付きでアイコンセット</summary>
        public async UniTask SetIconTextureWithEffectAsync(long mCharaId)
        {
            await UniTask.WhenAll(
                SetIconIdAsync(mCharaId),
                SetEffectAsync(mCharaId)
            );
        }
        
        /// <summary>アイコンのIDをセット</summary>
        private async UniTask SetEffectAsync(long mCharaId)
        {
            // エフェクト
            if (iconImageEffect != null)
            {
                long effectId = MasterManager.Instance.charaMaster.FindData(mCharaId)?.imageEffectId ?? 0;
                
                // エフェクトを持っているか
                bool hasEffect = effectId != 0;
                iconImageEffect.gameObject.SetActive(hasEffect);
                
                if (hasEffect)
                {
                    await iconImageEffect.LoadEffect(effectId);
                }
            }
        }
        
        /// <summary>戦力の表示</summary>
        /// <param name="power">戦力</param>
        /// <param name="coverUpFlg">隠蔽表示フラグ</param>
        public void SetCombatPower(BigValue power, bool coverUpFlg = false)
        {
            SetActiveCombatPower(true);
            if (coverUpFlg)
            {
                combatPowerText.text = StringValueAssetLoader.Instance["character.cover_up"];
            }
            else 
            {
                combatPowerText.text = power == -1 ? "????": power.ToDisplayString(omissionTextSetter.GetOmissionData());
            }
        }
        
        /// <summary>ポジションアイコンの表示</summary>
        public void SetRoleNumberIcon(RoleNumber roleNumber)
        {
            SetActiveRoleNumberIcon(true);
            roleNumberIcon.SetTexture(roleNumber);
        }
        
        /// <summary>ランクの表示</summary>
        private void SetRankIcon(long rank)
        {
            SetActiveRankIcon(true);
            rankIcon.SetTexture(rank);
        }
        
        /// <summary>ポジションアイコンのアクティブ</summary>
        public void SetActiveRoleNumberIcon(bool value)
        {
            roleNumberIcon.gameObject.SetActive(value);
        }
        
        /// <summary>戦力のアクティブ</summary>
        public void SetActiveCombatPower(bool value)
        {
            combatPowerRoot.SetActive(value);
        }
        
        /// <summary>ランクのアクティブ</summary>
        public void SetActiveRankIcon(bool value)
        {
            rankIcon.gameObject.SetActive(value);
        }
        
        /// <summary>エフェクトのアクティブ</summary>
        private void SetActiveEffect(bool value)
        {
            iconImageEffect.gameObject.SetActive(value);
        }

        public void SetEmpty()
        {
            Cancel();
            SetActiveImage(false);
            SetActiveCombatPower(false);
            SetActiveRankIcon(false);   
            SetActiveRoleNumberIcon(false);
            SetActiveEffect(false);
        }
        
        /// <summary>報酬ブースト</summary>
        public void SetBoostEffect(long value)
        {
            boostFrameObject.SetActive(value > 0);
            boostTextRoot.SetActive(value > 0);
            boostText.text = string.Format(StringValueAssetLoader.Instance["rivalry.boosteffect.charaicon"], value);
        }
        
        private string GetTitleStringKey()
        {
            switch(charType)
            {
                case CharacterType.VariableCharacter:
                    return "character.success_character_detail";
                case CharacterType.TrainingCharacter:
                    return "character.training_battle_character_detail";
                case CharacterType.OpponentCharacter:
                    return "character.opponent_character_detail";
            }
            
            return string.Empty;
        }

        public SwipeableParams<CharacterVariableDetailData> SwipeableParams = new();
        public void OnLongTap()
        {
            if(!openDetailModal) return;
            bool clearSwipeableParams = false;
            SwipeableParams ??= new SwipeableParams<CharacterVariableDetailData>();
            if (SwipeableParams?.DetailOrderList == null || SwipeableParams.DetailOrderList.Count == 0 || SwipeableParams.StartIndex == -1)
            {
                if(iconData == null)    return;
                SwipeableParams.StartIndex = 0;
                SwipeableParams.DetailOrderList = new List<CharacterVariableDetailData>() { iconData };
                clearSwipeableParams = true;
            }

            SuccessCharaDetailModalParams windowModalParams = new SuccessCharaDetailModalParams(SwipeableParams, canOpenCharacterEncyclopediaPage, GetTitleStringKey());
            SuccessCharaDetailModalWindow.Open(ModalType.SuccessCharaDetail, windowModalParams);
            if (clearSwipeableParams) SwipeableParams = null;
        }
    }
}