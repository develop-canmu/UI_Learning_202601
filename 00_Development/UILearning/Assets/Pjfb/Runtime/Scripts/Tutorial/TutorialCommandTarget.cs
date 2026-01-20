using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class TutorialCommandTarget : MonoBehaviour
    {
        [SerializeField] private string focusId = "";
        public string FocusId
        {
            get { return focusId; }
        }
    }
}