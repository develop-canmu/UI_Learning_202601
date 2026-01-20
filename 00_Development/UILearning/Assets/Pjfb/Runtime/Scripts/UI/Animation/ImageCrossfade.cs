using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb
{
    public class ImageCrossfade : MonoBehaviour
    {
        
        [SerializeField]
        private Animator animator = null;
    
        [SerializeField]
        private Image[] animationTargetImages = null;
        
        [SerializeField]
        private List<Sprite> spriteList = new List<Sprite>();
        
        // Index
        private int imageIndex = 0;
        
        private void Awake()
        {
            // ２つ以上ある場合はアニメーション
            animator.enabled = spriteList.Count > 1;
        }
        
        /// <summary>クロスフェードするスプライトの登録</summary>
        public void SetSpriteList(List<Sprite> list)
        {
            spriteList.Clear();
            
            // 初期化のためアニメーターを切っておく
            animator.enabled = false;
            
            // ０個の場合は非表示
            if(list.Count <= 0)
            {
                for(int i=0;i<animationTargetImages.Length;i++)
                {
                    animationTargetImages[i].gameObject.SetActive(false);
                }
                return;
            }
            
            spriteList.AddRange(list);
            imageIndex = 0;
            
            // ラベルの初期化
            for(int i=0;i<animationTargetImages.Length;i++)
            {
                animationTargetImages[i].color = i == 0 ? Color.white : new Color(1.0f, 1.0f, 1.0f, 0);
                animationTargetImages[i].gameObject.SetActive(true);
                NextImage();
            }
            
            // ２つ以上ある場合はアニメーション
            if (list.Count > 1)
            {
                animator.enabled = true;
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0);                
            }
            
        }
        
        /// <summary>次のイメージへ</summary>
        public void NextImage()
        {       
            if(animationTargetImages.Length <= 0 || spriteList.Count <= 0)return;
            int index = imageIndex % animationTargetImages.Length;
            int spriteIndex = imageIndex % spriteList.Count;
            animationTargetImages[index].sprite = spriteList[spriteIndex];
            imageIndex++;        
        }
    }
}