using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class DeckRankImage : RankImageBase
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetDeckRankImagePath(id, Size);
        }
    }
}
