using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    [RequireComponent(typeof(RawImage))]
    public class RivalryEventBannerImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetRivalryEventBannerImagePath(id);
        }
    }
}