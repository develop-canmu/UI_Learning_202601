using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class ClubRankImage : RankImageBase
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetClubRankImagePath(id, Size);
        }
    }
}