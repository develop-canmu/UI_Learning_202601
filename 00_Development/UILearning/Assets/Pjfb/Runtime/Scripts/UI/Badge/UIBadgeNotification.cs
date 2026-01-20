using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class UIBadgeNotification : MonoBehaviour
    {
        /// <summary>表示切り替え</summary>
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
