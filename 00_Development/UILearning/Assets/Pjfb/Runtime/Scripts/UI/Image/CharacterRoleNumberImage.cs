using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine.AddressableAssets;

namespace Pjfb
{
    public class CharacterRoleNumberImage : CancellableImageWithEnum<RoleNumber>
    {
        protected override string GetAddress(RoleNumber type)
        {
            return PageResourceLoadUtility.GetRoleNumberIconImagePath(type);
        }
    }
}