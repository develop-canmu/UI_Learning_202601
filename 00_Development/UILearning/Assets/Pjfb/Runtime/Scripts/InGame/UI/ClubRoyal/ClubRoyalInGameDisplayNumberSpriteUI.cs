using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameDisplayNumberSpriteUI : MonoBehaviour
    {
        [Header("数字スプライト設定")]
        [SerializeField] private Sprite[] numberSprites = new Sprite[10]; // 0-9の数字スプライト
        
        [Header("桁表示用Image")]
        [SerializeField] private Image[] digitImages; // 各桁の表示用Image
        
        [Header("デフォルト設定")]
        [SerializeField] private int defaultMaxDigits = 4; // デフォルト最大桁数
        [SerializeField] private bool defaultZeroFill = true; // デフォルトゼロ埋め
        
        /// <summary>
        /// 数字を表示する
        /// </summary>
        /// <param name="number">表示する数字</param>
        /// <param name="maxDigits">表示桁数</param>
        /// <param name="isZeroFill">ゼロ埋めするかどうか</param>
        public void Display(int number, int maxDigits, bool isZeroFill)
        {
            // 負の数は0として扱う
            if (number < 0)
            {
                number = 0;
            }
            
            // 桁数がImage配列より多い場合は配列サイズに制限
            maxDigits = Mathf.Min(maxDigits, digitImages.Length);
            
            // 数字を文字列に変換
            string numberStr = number.ToString();
            
            // ゼロ埋めが必要な場合は桁数を合わせる
            if (isZeroFill)
            {
                numberStr = numberStr.PadLeft(maxDigits, '0');
            }
            
            // 桁数が足りない場合は空文字で埋める
            if (numberStr.Length < maxDigits && !isZeroFill)
            {
                numberStr = numberStr.PadLeft(maxDigits, ' ');
            }
            
            // 桁数が多すぎる場合は切り捨て（右から桁数分を取得）
            if (numberStr.Length > maxDigits)
            {
                numberStr = numberStr.Substring(numberStr.Length - maxDigits);
            }
            
            // 各桁のImageに対応するスプライトを設定
            for (int i = 0; i < maxDigits && i < digitImages.Length; i++)
            {
                if (digitImages[i] != null)
                {
                    char digit = numberStr[i];
                    if (digit == ' ')
                    {
                        // 空白の場合はImageを非表示
                        digitImages[i].gameObject.SetActive(false);
                    }
                    else if (char.IsDigit(digit))
                    {
                        // 数字の場合は対応するスプライトを設定
                        int digitValue = digit - '0';
                        if (digitValue >= 0 && digitValue < numberSprites.Length && numberSprites[digitValue] != null)
                        {
                            digitImages[i].sprite = numberSprites[digitValue];
                            digitImages[i].gameObject.SetActive(true);
                        }
                        else
                        {
                            digitImages[i].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        digitImages[i].gameObject.SetActive(false);
                    }
                }
            }
            
            // 使用しない桁のImageを非表示
            for (int i = maxDigits; i < digitImages.Length; i++)
            {
                if (digitImages[i] != null)
                {
                    digitImages[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 簡易表示メソッド（デフォルト設定を使用）
        /// </summary>
        /// <param name="number">表示する数字</param>
        public void Display(int number)
        {
            Display(number, defaultMaxDigits, defaultZeroFill);
        }

    }
}