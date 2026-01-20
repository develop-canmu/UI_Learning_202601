using System.Collections;
using System.Collections.Generic;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    
    public enum ItemIconType
    {
        Item = 1,
        Character = 2,
        VariableCharacter = 3,
        SpecialSupportCharacterCard = 4,
        CharacterPiece = 5,
        UserIcon = 6,
        UserTitle = 7,
        ChatStamp = 8,
        CharacterParent = 9,
        SupportEquipment = 10,
        ProfilePart = 11,
        Adviser = 12,
    }
    
    public class ItemIconContainer : MonoBehaviour
    {
        [SerializeField]
        private ItemIcon itemIcon = null;
        [SerializeField]
        private CharacterIcon characterIcon = null;
        [SerializeField]
        private CharacterVariableIcon characterVariableIcon = null;
        [SerializeField]
        private SpecialSupportCardIcon specialSupportCardIcon = null;
        [SerializeField]
        private CharacterPieceIcon characterPieceIcon = null;
        [SerializeField]
        private UserIcon userIcon = null;
        [SerializeField]
        private UserTitleIcon userTitleIcon = null;
        [SerializeField]
        private ChatStampIcon chatStampIcon = null;
        [SerializeField]
        private ProfilePartIcon profilePartIcon = null;
        [SerializeField]
        private AdviserIcon adviserIcon = null;
        
        [SerializeField]
        private ItemIconType iconType = ItemIconType.Item;
        /// <summary>アイコンタイプ</summary>
        public ItemIconType IconType { get { return iconType; } }
        
        /// <summary>アイコンのタイプ</summary>
        public void SetIconType(ItemIconType iconType)
        {
            // 開いているアイコンを閉じる
            GetIcon().gameObject.SetActive(false);
            // アイコンタイプ
            this.iconType = iconType;
            
            switch(iconType)
            {
                case ItemIconType.Item:
                    itemIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.Character:
                case ItemIconType.SupportEquipment:
                    characterIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.VariableCharacter:
                    characterVariableIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.SpecialSupportCharacterCard:
                    specialSupportCardIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.CharacterPiece:
                    characterPieceIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.UserIcon:
                    userIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.UserTitle:
                    userTitleIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.ChatStamp:
                    chatStampIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.ProfilePart:
                    profilePartIcon.gameObject.SetActive(true);
                    break;
                case ItemIconType.Adviser:
                    adviserIcon.gameObject.SetActive(true);
                    break;
            }
        }

        public void SetCount(long count)
        {
            switch(iconType)
            {
                case ItemIconType.Item:
                    itemIcon.SetCount(count);
                    break;
                case ItemIconType.CharacterPiece:
                    characterPieceIcon.SetCount(count);
                    break;
                case ItemIconType.Character:
                case ItemIconType.SupportEquipment:
                    characterIcon.SetCount(count);
                    break;
                case ItemIconType.Adviser:
                    adviserIcon.SetCount(count);
                    break;
            }
        }
        
        public void SetCountDigitString(long possessionCount, long necessaryCount)
        {
            switch(iconType)
            {
                case ItemIconType.Item:
                    itemIcon.SetCountDigitString(possessionCount, necessaryCount);
                    break;
            }
        }
        
        public void SetCountTextColor(Color color)
        {
            switch(iconType)
            {
                case ItemIconType.Item:
                    itemIcon.SetCountTextColor(color);
                    break;
                case ItemIconType.CharacterPiece:
                    characterPieceIcon.SetCountTextColor(color);
                    break;
                case ItemIconType.Character:
                case ItemIconType.SupportEquipment:
                    characterIcon.SetCountTextColor(color);
                    break;
                case ItemIconType.Adviser:
                    adviserIcon.SetCountTextColor(color);
                    break;
            }
        }

        public ItemIconBase GetIcon()
        {
            switch(iconType)
            {
                case ItemIconType.Item:
                    return itemIcon;
                case ItemIconType.Character:
                case ItemIconType.SupportEquipment:
                    return characterIcon;
                case ItemIconType.VariableCharacter:
                    return characterVariableIcon;
                case ItemIconType.SpecialSupportCharacterCard:
                    return specialSupportCardIcon;
                case ItemIconType.CharacterPiece:
                    return characterPieceIcon;
                case ItemIconType.UserIcon:
                    return userIcon;
                case ItemIconType.UserTitle:
                    return userTitleIcon;
                case ItemIconType.ChatStamp:
                    return chatStampIcon;
                case ItemIconType.ProfilePart:
                    return profilePartIcon;
                case ItemIconType.Adviser:
                    return adviserIcon;
            }
            
            return null;
        }
        
        public T GetIcon<T>() where T : ItemIconBase
        {
            return (T)GetIcon();
        }
        
        /// <summary>Idのセット</summary>
        public void SetIcon(ItemIconType iconType, long id)
        {
            SetIconType(iconType);
            GetIcon().SetIconId(id);
        }
        
        /// <summary>Idのセット</summary>
        public void SetIconId(long id)
        {
            GetIcon().SetIconId(id);
        }

        private void Awake()
        {
            if(itemIcon != null)
                itemIcon.gameObject.SetActive(false);
            if(characterIcon != null)
                characterIcon.gameObject.SetActive(false);
            if(characterVariableIcon != null)
                characterVariableIcon.gameObject.SetActive(false);
            if(specialSupportCardIcon != null)
                specialSupportCardIcon.gameObject.SetActive(false);
            if(characterPieceIcon != null)
                characterPieceIcon.gameObject.SetActive(false);
            if(userIcon != null)
                userIcon.gameObject.SetActive(false);
            if(userTitleIcon != null)
                userTitleIcon.gameObject.SetActive(false);
            if(chatStampIcon != null)
                chatStampIcon.gameObject.SetActive(false);
            if(profilePartIcon != null)
                profilePartIcon.gameObject.SetActive(false);
            if(adviserIcon != null)
                adviserIcon.gameObject.SetActive(false);
            SetIconType(iconType);
        }
        
    }
}