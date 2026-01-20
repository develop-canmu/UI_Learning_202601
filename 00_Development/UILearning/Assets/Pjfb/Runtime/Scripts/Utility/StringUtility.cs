using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.UserData;
using TMPro;

namespace Pjfb
{
    public static class StringUtility
    {
        private static readonly List<string> RemoveTagList = new List<string>(){"<quad"};
        private static readonly Regex RemoveTagPattern = new Regex("(<.*>)|(<.*=)");
        private const int MaxDigitCount = 4;
        // 1単位辺りの桁数
        private const int UnitQuantity = 4;
        
        /// <summary>省略表記の基準値</summary>
        private const double OmissionCriterion = 10000f;
        /// <summary>桁数定義</summary>>
        private static readonly string[] DigitString = 
        {
            "万",
            "億",
            "兆",
            "京",
            "垓",
            "𥝱",
            "穣",
            "溝",
            "澗",
            "正",
            "載",
            "極",
            "恒河沙",
            "阿僧祇",
            "那由多",
            "不可思議",
            "無量大数"
        };
        
        public class OmissionTextData
        {
            private int digitFontSize;
            public int DigitFontSize{get{return digitFontSize;}}
            private string digitFontName;
            public string DigitFontName{get{return digitFontName;}}
            private int commaFontSize;
            public int CommaFontSize{get{return commaFontSize;}}
            public OmissionTextData(int digitFontSize, string digitFontName, int commaFontSize)
            {
                this.digitFontSize = digitFontSize;
                this.digitFontName = digitFontName;
                this.commaFontSize = commaFontSize;
            }
        }
        

        public static string GetLimitNumCharacter(string inputStr,int limitNum, bool isSingleLine = false)
        {
            inputStr = RemoveRichTextTag(RemoveEmojiAndCombiningCharacter(inputStr));
            if (isSingleLine)
            {
                inputStr = inputStr.RemoveLineEnd();
                inputStr = inputStr.RemoveBeginAndEndSpace();
            }
            if (inputStr.Length > limitNum)
            {
                inputStr = inputStr.Substring(0, limitNum);
            }
            return inputStr;
        }
        
        public static string GetLimitNumUserName(string inputStr,int limitNum)
        {
            inputStr = RemoveRichTextTag(RemoveEmojiAndCombiningCharacter(inputStr,true));
            inputStr = inputStr.RemoveLineEnd();
            inputStr = inputStr.RemoveBeginAndEndSpace();
            // Advで埋め込むタグが誤作動するので単発の　< or > もユーザー名からは削る
            inputStr = inputStr.Replace("<", "");
            inputStr = inputStr.Replace(">", "");
            if (inputStr.Length > limitNum)
            {
                inputStr = inputStr.Substring(0, limitNum);
            }
            return inputStr;
        }
        
        public static string RemoveRichTextTag(string inputStr)
        {
            inputStr = RemoveTagPattern.Replace(inputStr,"");
            foreach (var s in RemoveTagList)
            {
                inputStr = inputStr.Replace(s, "");
            }

            return inputStr;
        }
        
        public static string RemoveEmojiAndCombiningCharacter(string inputStr,bool removeOtherSymbol = false)
        {
            string outputStr = "";
            TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(inputStr);
 
            while (textElementEnumerator.MoveNext()) {
                string textElement = textElementEnumerator.GetTextElement();
                //バイト数が多い文字を削除（サロゲートと結合文字の一部）
                if (textElement.Length >= 2) continue;
                if (textElement.Any(c => IsRemoveCharacter(c,removeOtherSymbol)))  continue;
                outputStr += textElement;
            }

            return outputStr;
        }

        public static string RemoveLineEnd(this string content)
        {
            content = content.Replace("\\n", "");
            content = content.Replace("¥n", "");
            content = content.Replace("\n", "");
            content = Regex.Replace(content, @"\t|\n|\r", "");

            return content;
        }

        public static string RemoveBeginAndEndSpace(this string content)
        {
            return content.Trim();
        }
        

        public static string ToEndAtString( DateTime date )
        {
            return string.Format(StringValueAssetLoader.Instance["common.end_at"], date.ToString("yyyy/MM/dd HH:mm:ss"));
        }

        public static char OnValidateInput(string text, int charIndex, char addedChar, int maxLength, TMP_FontAsset asset, bool removeOtherSymbol = false)
        {
            if (text.Length >= maxLength) return '\0';
            if(!DynamicAssetFontUtility.Instance.HasCharacter(addedChar)) return '\0';
            return IsRemoveCharacter(addedChar,removeOtherSymbol) ? '\0' : addedChar;
        }
        private static bool IsRemoveCharacter(char c,bool removeOtherSymbol = false)
        {
            bool result = char.GetUnicodeCategory(c) switch
            {
                UnicodeCategory.OtherSymbol => removeOtherSymbol,　//特殊
                UnicodeCategory.NonSpacingMark => true, //組文字
                UnicodeCategory.Surrogate => true,　//絵文字
                _ => false
            };
            return result;
        }

        public static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
        
        /// <summary>
        /// 表示よう文字列
        /// 1兆234みたいな表示
        /// </summary>
        public static string ToDisplayString(this BigValue value, OmissionTextData data)
        {
            // 省略が必要ない場合はそのまま返す
            if (value < OmissionCriterion && value > -OmissionCriterion) return ToDisplayCommaString(value);
            string valueString = value.ToString();
            // マイナスの場合は先頭の-分調整する
            int startIndex = value < 0 ? 1 : 0;
            int digitValue = valueString.Length;
            
            // 単位取得用のキーを用意
            int digitKeyNum = (digitValue - startIndex - 1) / UnitQuantity;
            // 最大桁超えたら最大桁にする
            if (digitKeyNum > DigitString.Length)
            {
                digitKeyNum = DigitString.Length;
            }
            string useDigitString = $"<size={data.DigitFontSize}><font={data.DigitFontName}>{DigitString[digitKeyNum - 1]}</font></size>";
            string truncateCalcValueString = $"{valueString.Substring(0, digitValue - digitKeyNum * UnitQuantity)}";
            // 文字数が表示桁数以下の場合
            int truncateCalcValueStringLength = truncateCalcValueString.Length - startIndex;
            if(truncateCalcValueStringLength < UnitQuantity)
            { 
                string decimalString = valueString.Substring(truncateCalcValueStringLength + startIndex, UnitQuantity - truncateCalcValueStringLength);
                truncateCalcValueString += $"<size={data.CommaFontSize}>.</size>{decimalString}";
            }
            return $"{truncateCalcValueString}{useDigitString}";
        }
        
        /// <summary>
        /// カンマ表示
        /// </summary>
        public static string ToDisplayCommaString(this BigValue value)
        {
            return $"{value.Value:#,0}";
        }
        
        /// <summary>
        /// 表示よう文字列
        /// </summary>
        public static string ToRateString(this BigValue value,long rate = BigValue.DefaultRateValue)
        {
            return $"{value / rate}.{value % rate / (rate / 10)}";
        }
        
    }
}