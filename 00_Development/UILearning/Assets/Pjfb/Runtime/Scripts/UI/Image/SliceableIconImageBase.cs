using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System;

namespace Pjfb
{
    public abstract class SliceableIconImageBase : CancellableImageWithId
    {
        public abstract ItemIconType IconType { get; }

        protected override string GetKey(long id)
        {
            switch (IconType)
            {
                case ItemIconType.Item: return PageResourceLoadUtility.GetItemIconImagePath(id);
                case ItemIconType.Character: return PageResourceLoadUtility.GetCharacterIconImagePath(id);
                case ItemIconType.VariableCharacter: return PageResourceLoadUtility.GetVariableCharacterIconImagePath(id);
                case ItemIconType.SpecialSupportCharacterCard: return PageResourceLoadUtility.GetSpecialSupportCharacterCardImagePath(id);
                case ItemIconType.CharacterPiece: return PageResourceLoadUtility.GetCharacterPieceImagePath(id);
                case ItemIconType.UserIcon: return PageResourceLoadUtility.GetUserIconImagePath(id);
                // TODO 汎用アイコンのみの表示
                // case ItemIconType.UserTitle: return PageResourceLoadUtility.GetUserTitleImagePath(id);
                case ItemIconType.ChatStamp: return PageResourceLoadUtility.GetChatStampIconImagePath(id);
                case ItemIconType.CharacterParent: return PageResourceLoadUtility.GetCharacterParentIconImagePath(id);
                case ItemIconType.SupportEquipment: return PageResourceLoadUtility.GetSupportEquipmentIconImagePath(id);
                default: throw new NotImplementedException();
            }
        }
    }
}