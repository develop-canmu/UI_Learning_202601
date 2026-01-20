using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class ImageWithText : MonoBehaviour
    {
        public TMP_Text text;
        public Image image;

        public virtual void Initialize(Sprite sprite, string str)
        {
            text.text = str;
            image.sprite = sprite;
        }
    }
}


