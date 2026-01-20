using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.RecommendChara
{
    public class RecommendCharaSheetButton : SheetSwitchButton<RecommendCharaSheetManager, RecommendCharaTabSheetType>
    {
        public Action OnTabClick;

        [SerializeField] private Image activeImage;
        [SerializeField] private Sprite singleTabSprite;
        [SerializeField] private Sprite multiTabSprite;
        
        protected override void OnOpened()
        {
            OnTabClick?.Invoke();
            base.OnOpened();
        }

        public void SetSingleTabView()
        {
            activeImage.sprite = singleTabSprite;
        }

        public void SetMultiTabView()
        {
            activeImage.sprite = multiTabSprite;
        }
    }
}
