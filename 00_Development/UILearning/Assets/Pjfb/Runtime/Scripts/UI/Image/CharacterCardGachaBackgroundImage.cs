using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CharacterCardGachaBackgroundImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardGachaBackgroundImagePath(id);
        }
    }
}