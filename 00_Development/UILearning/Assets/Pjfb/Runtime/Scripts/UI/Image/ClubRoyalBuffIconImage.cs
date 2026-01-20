using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class ClubRoyalBuffIconImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetClubRoyalBuffIconImagePath(id);
        }

        public Sprite GetSprite()
        {
            return Image.sprite;
        }
    }
}