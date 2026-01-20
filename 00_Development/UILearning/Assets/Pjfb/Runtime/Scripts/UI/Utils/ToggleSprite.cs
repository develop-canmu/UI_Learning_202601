using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pjfb
{
    /// <summary>
    /// 複数のSprite切り替えコンポーネント
    /// </summary>
    public class ToggleSprite : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private int value = 1;
        [SerializeField] private bool refreshOnAwake;
	
        /// <summary>
        /// トグルの値を設定/取得
        /// </summary>
        public int Value
        {
            get => value;
            set
            {
                this.value = value;
                Refresh();
            }
        }
        
        public bool BoolValue
        {
            get => value > 0;
            set
            {
                this.value = value ? 1 : 0;
                Refresh();
            }
        }

        void Awake()
        {
            if (refreshOnAwake)
            {
                Refresh();
            }
        }
	
        /// <summary>
        /// 現在の値をもとに表示を切り替える
        /// </summary>
        private void Refresh()
        {
            if (value < 0 || value > sprites.Length)
            {
                return;
            }

            if (targetImage != null)
            {
                targetImage.sprite = sprites[value];
            }

            if (targetSpriteRenderer != null)
            {
                targetSpriteRenderer.sprite = sprites[value];
            }
        }
    }
}
