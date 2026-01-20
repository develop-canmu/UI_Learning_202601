using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Pjfb
{
    public class CharacterRarityIconImage : CancellableImage
    {
        // レアリティ限界突破時に表示されるデータクラス
        [Serializable]
        private class RarityLimitOverIcon
        {
            [SerializeField] 
            private long rarity = 0;

            public long Rarity => rarity;

            [SerializeField] 
            private Sprite iconImage;

            public Sprite IconImage => iconImage;
        }
        
        [SerializeField]
        private Sprite baseRarity = null;
        
        [SerializeField]
        private Sprite liberatedRarity = null;
        
        [SerializeField]
        private Sprite unliberatedRarity = null;

        // レアリティ上限突破用のオブジェクト
        [SerializeField]
        private Image limitOverImage = null;

        // レアリティ限界突破時に表示されるアイコンデータ
        [SerializeField] 
        private RarityLimitOverIcon[] rarityLimitOverIconData;
        
        [SerializeField]
        private Animator animator = null;

        public void SetBaseRarity()
        {
            Image.sprite = baseRarity;
            SwitchRarityObject(true);
        }
        
        public void SetLiberatedRarity()
        {
            Image.sprite = liberatedRarity;
            SwitchRarityObject(true);
        }
        
        public void SetUnliberatedRarity()
        {
            Image.sprite = unliberatedRarity;
            SwitchRarityObject(true);
        }

        //// <summary> レアリティオブジェクトのアクティブ切り替え </summary>
        public void SwitchRarityObject(bool isBaseRarity)
        {
            Image.gameObject.SetActive(isBaseRarity);
            limitOverImage.gameObject.SetActive(isBaseRarity == false);
        }

        //// <summary> 星の上限突破 </summary>
        public void SetLimitOverRarity(long rarity)
        {
            limitOverImage.sprite = GetRarityLimitOverIcon(rarity);
            SwitchRarityObject(false);
        }

        /// <summary>点滅</summary>
        public void PlayFlash()
        {
            animator.Play("Flash", 0, 0); 
        }
        
        /// <summary>点滅止める</summary>
        public void StopFlash()
        {
            // アルファを1にする
            Color color = Image.color;
            color.a = 1.0f;
            Image.color = color;
            
            animator.Play("Default"); 
        }

        //// <summary> レアリティ限界突破時のアイコンを取得 </summary>
        private Sprite GetRarityLimitOverIcon(long rarity)
        {
            // Listの末尾から見る(レアリティの高い順から)
            for (int i = rarityLimitOverIconData.Length - 1; i >= 0; i--)
            {
                // 一致するレアリティのアイコンを返す
                if (rarityLimitOverIconData[i].Rarity <= rarity)
                {
                    return rarityLimitOverIconData[i].IconImage;
                }
            }

            // 見つからない場合,最初のデータで返す
            return rarityLimitOverIconData[0].IconImage;
        }
    }
}