using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class PracticeCardImage : CancellableRawImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetPracticeCardImagePath(id);
        }
    }
}
