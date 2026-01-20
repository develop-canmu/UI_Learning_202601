using System.Collections;
using System.Collections.Generic;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    public class CharacterInGameIconImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterInGameIconPath(id);
        }
    }
}