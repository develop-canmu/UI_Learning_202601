using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class EventLogoImage : CancellableImageWithId
    {
        protected override string GetKey(long id)
        {
            return PageResourceLoadUtility.GetEventLogoImagePath(id);
        }
    }
}