using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopLargeBannerWebTexture : CancellableWebTextureWithId
    {
        protected override string GetURL(long id)
        {
            return GachaUtility.GetGachaLargeBannerURL(id);
        }
    }
}