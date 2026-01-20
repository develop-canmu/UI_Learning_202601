using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Gacha;

namespace Pjfb.Gacha
{
    public class GachaTopTicketBannerWebTexture : CancellableWebTextureWithId
    {
        protected override string GetURL(long id)
        {
            return GachaUtility.GetGachaTicketBannerURL(id);
        }
    }
}