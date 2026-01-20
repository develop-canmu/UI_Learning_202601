using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Ranking;

namespace Pjfb
{
    public class RankingWebTexture : CancellableWebTextureWithId
    {
        protected override string GetURL(long id)
        {
            return RankingManager.GetBannerURL(id);
        }
    }
}