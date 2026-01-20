using System.Collections;
using System.Collections.Generic;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb
{
    public class CharacterCardImage : CharacterCardImageBase
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardImagePath(id);
        }

        protected override string GetEffectKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardEffectPath(id);
        }
    }
}
