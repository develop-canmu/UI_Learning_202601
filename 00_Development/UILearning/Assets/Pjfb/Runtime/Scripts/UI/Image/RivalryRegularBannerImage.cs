using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    [RequireComponent(typeof(RawImage))]
    public class RivalryRegularBannerImage : CancellableRawImageWithId
    {
        protected override string GetKey(long difficulty)
        {
            return PageResourceLoadUtility.GetRivalryRegularBannerImagePath(difficulty);
        }
    }
}