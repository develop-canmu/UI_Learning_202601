using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    [RequireComponent(typeof(RawImage))]
    public class RivalryTopBannerImage : CancellableRawImageWithId
    {
        protected override string GetKey(long mHuntId)
        {
            return PageResourceLoadUtility.GetRivalryTopBannerImagePath(mHuntId);
        }
    }
}