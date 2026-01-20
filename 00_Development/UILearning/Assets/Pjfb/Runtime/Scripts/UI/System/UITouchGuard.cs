using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class UITouchGuard : MonoBehaviour
    {
        private int referenceCount = 0;

        public void Show()
        {
            referenceCount++;
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            referenceCount--;
            // 0以下にはならない
            if(referenceCount < 0)
            {
                referenceCount = 0;
            }
            // 非表示
            if(referenceCount == 0)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void HideForce()
        {
            referenceCount = 0;
            gameObject.SetActive(false);
        }
    }
}
