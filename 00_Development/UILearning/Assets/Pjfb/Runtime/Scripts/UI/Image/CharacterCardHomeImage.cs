using System.Collections;
using System.Collections.Generic;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CharacterCardHomeImage : CharacterCardImageBase
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardHomeImagePath(id);
        }

        protected override string GetEffectKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterCardHomeEffectPath(id);
        }
    }
}