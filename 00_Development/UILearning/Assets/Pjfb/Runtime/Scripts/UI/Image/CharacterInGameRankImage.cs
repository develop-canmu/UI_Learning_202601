using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class CharacterInGameRankImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterInGameRankIconImagePath(id);
        }
    }
}

