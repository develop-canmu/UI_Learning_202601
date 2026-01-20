using System.Collections;
using System.Collections.Generic;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    public class AbilityNameImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetAbilityNameImagePath(id);
        }
    }
}