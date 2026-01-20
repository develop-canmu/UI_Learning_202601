using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    [RequireComponent(typeof(RawImage))]
    public class RivalryEventFrameImage : CancellableRawImageWithId
    {
        protected override string GetKey(long difficulty)
        {
            return PageResourceLoadUtility.GetRivalryEventFrameImagePath(difficulty);
        }
    }
}