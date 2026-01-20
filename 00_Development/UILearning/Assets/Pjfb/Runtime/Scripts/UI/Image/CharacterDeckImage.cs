using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CharacterDeckImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetCharacterDeckImagePath(id);
        }
        
        public void SetImageEnable(bool isActive)
        {
            RawImage.enabled = isActive;
        }
    }
}