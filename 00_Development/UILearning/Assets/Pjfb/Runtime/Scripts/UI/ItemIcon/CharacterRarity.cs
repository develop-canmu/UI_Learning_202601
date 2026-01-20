using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class CharacterRarity : MonoBehaviour
    {
        [SerializeField]
        private CharacterRarityIconImage[] rarityIconImages = null;
        
        
        public void SetBaseRarity(long baseRarity)
        {
            // 星表示可能数から何個超えているか
            long rarityLimitOverCount = baseRarity - rarityIconImages.Length;
           
            for(int i = 0; i < rarityIconImages.Length; i++)
            {
                //星の表示数上限を超える場合(レアリティ限界突破)
                if (rarityLimitOverCount > 0 && i < rarityLimitOverCount)
                {
                    rarityIconImages[i].SetLimitOverRarity(baseRarity);
                    continue;
                }
                // 未解放
                if(i > baseRarity-1)
                {
                    rarityIconImages[i].SetUnliberatedRarity();
                    continue;
                }
                // ベース
                if(i <= baseRarity-1)
                {
                    rarityIconImages[i].SetBaseRarity();
                }
            }
        }
        
        public void SetRarity(long currentRarity, long baseRarity)
        {
            // 星表示可能数から何個超えているか
            long rarityLimitOverCount = currentRarity - rarityIconImages.Length;
           
            for(int i = 0; i < rarityIconImages.Length; i++)
            {
                // 星の表示数上限を超える場合(レアリティ限界突破)
                if (rarityLimitOverCount > 0 && i < rarityLimitOverCount)
                {
                    rarityIconImages[i].SetLimitOverRarity(currentRarity);
                    continue;
                }
                // 未解放
                if(i > currentRarity-1)
                {
                    rarityIconImages[i].SetUnliberatedRarity();
                    continue;
                }
                // ベース
                if(i <= baseRarity-1)
                {
                    rarityIconImages[i].SetBaseRarity();
                    continue;
                }
                // 解放済み
                rarityIconImages[i].SetLiberatedRarity();
            }
        }

        /// <summary>レアリティセットして点滅</summary>
        public void SetRarityAndFlash(long nextRarity, long currentRarity, long baseRarity)
        {
            // レアリティセット
            SetRarity(nextRarity, baseRarity);

            // レアリティが上昇する表示画像のIndex番号
            HashSet<long> rarityUpIndexList = new HashSet<long>();
            
            for (long i = currentRarity; i < nextRarity; i++)
            {
                // Indexが重複するので表示分を回したらループを抜ける
                if (i > currentRarity + rarityIconImages.Length)
                {
                    break;
                }
                
                rarityUpIndexList.Add(i % rarityIconImages.Length);
            }
            
            for (int i = 0; i < rarityIconImages.Length; i++)
            {
                // レアリティ上昇のIndexに合致するなら点滅させる
                if(rarityUpIndexList.Contains(i))
                {
                    rarityIconImages[i].PlayFlash();
                }
                else
                {
                    rarityIconImages[i].StopFlash();
                }
            }
        }
    }
}