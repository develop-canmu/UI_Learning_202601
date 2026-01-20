using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pjfb
{
    public class CharacterStatusTypeImage : CancellableImageWithEnum<CharacterStatusType>
    {
        protected override string GetAddress(CharacterStatusType type)
        {
            return PageResourceLoadUtility.GetStatusTypeIconImagePath(type);
        }
    }
}