using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CallAnimationEventAction : MonoBehaviour
    {
        public Action AnimationEventAction { get; set; }

        public void CallAnimationEvent()
        {
            AnimationEventAction?.Invoke();
        }
    }
}