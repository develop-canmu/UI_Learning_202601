using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{

    public class CharacterStatusValuesView : MonoBehaviour
    {
        [SerializeField]
        private CharacterStatusValueView staminaValue = null;
        [SerializeField]
        private CharacterStatusValueView speedValue = null;
        [SerializeField]
        private CharacterStatusValueView physicalValue = null;
        [SerializeField]
        private CharacterStatusValueView techniqueValue = null;
        [SerializeField]
        private CharacterStatusValueView intelligenceValue = null;
        [SerializeField]
        private CharacterStatusValueView kickValue = null;
        [SerializeField]
        private CharacterStatusValueView shootRangeValue = null;
        
        /// <summary>キャラクタを表示</summary>
        public void SetUserCharacter(long id)
        {
            // ユーザーデータ
            UserDataChara uChar = UserDataManager.Instance.chara.data[id];
            
            SetCharacter(uChar.charaId, uChar.level, uChar.newLiberationLevel);
        }
        
        /// <summary>キャラクタを表示</summary>
        public void SetCharacter(long mCharId, long lv, long liberationLv)
        {
            // ステータス
            CharacterStatus status = StatusUtility.CalcCharacterStatus(mCharId, lv, liberationLv);
            
            SetStatus(status);
        }
        
        /// <summary>キャラクタを表示</summary>
        public void SetCharacterVariable(CharacterVariableDetailData chara)
        {
            staminaValue.SetValue(chara.Status.Stamina);
            speedValue.SetValue(chara.Status.Speed);
            physicalValue.SetValue(chara.Status.Physical);
            techniqueValue.SetValue(chara.Status.Technique);
            intelligenceValue.SetValue(chara.Status.Intelligence);
            kickValue.SetValue(chara.Status.Kick);
            if(shootRangeValue != null)
                shootRangeValue.SetValue(StatusUtility.GetShootRangeName(chara.Status.ShootRange));
        }
        
        public CharacterStatusValueView GetView(CharacterStatusType type)
        {
            switch(type)
            {
                case CharacterStatusType.Stamina:return staminaValue;
                case CharacterStatusType.Speed:return speedValue;
                case CharacterStatusType.Physical:return physicalValue;
                case CharacterStatusType.Technique:return techniqueValue;
                case CharacterStatusType.Kick:return kickValue;
                case CharacterStatusType.Intelligence:return intelligenceValue;
            }
            
            return null;
        }

        public void SetStatus(CharacterStatus status)
        {
            // 各ステータス
            staminaValue.SetValue(status.Stamina);
            staminaValue.SetTypeImage(CharacterStatusType.Stamina);
            speedValue.SetValue(status.Speed);
            speedValue.SetTypeImage(CharacterStatusType.Speed);
            physicalValue.SetValue(status.Physical);
            physicalValue.SetTypeImage(CharacterStatusType.Physical);
            techniqueValue.SetValue(status.Technique);
            techniqueValue.SetTypeImage(CharacterStatusType.Technique);
            intelligenceValue.SetValue(status.Intelligence);
            intelligenceValue.SetTypeImage(CharacterStatusType.Intelligence);
            kickValue.SetValue(status.Kick);
            kickValue.SetTypeImage(CharacterStatusType.Kick);
            if(shootRangeValue != null)
                shootRangeValue.SetValue(StatusUtility.GetShootRangeName(status.ShootRange));
        }

        public void SetColor(long mCharId, long currentLv, long afterLv, long liberationLv)
        {
            // ステータス
            CharacterStatus currentStatus = StatusUtility.CalcCharacterStatus(mCharId, currentLv, liberationLv);
            // ステータス
            CharacterStatus afterStatus = StatusUtility.CalcCharacterStatus(mCharId, afterLv, liberationLv);
            
            staminaValue.SetColor(afterStatus.Stamina > currentStatus.Stamina);
            speedValue.SetColor(afterStatus.Speed > currentStatus.Speed);
            physicalValue.SetColor(afterStatus.Physical > currentStatus.Physical);
            techniqueValue.SetColor(afterStatus.Technique > currentStatus.Technique);
            intelligenceValue.SetColor(afterStatus.Intelligence > currentStatus.Intelligence);
            kickValue.SetColor(afterStatus.Kick > currentStatus.Kick);
        }
    }
}
