using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class ScrollBannerPage : MonoBehaviour
    {
        [SerializeField]
        private GameObject enablePage = null;
        
        public void SetEnable(bool enable)
        {
            enablePage.SetActive(enable);
        }
    }
}
