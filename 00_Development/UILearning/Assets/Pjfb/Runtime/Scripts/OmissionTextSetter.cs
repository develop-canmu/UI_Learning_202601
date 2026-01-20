using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class OmissionTextSetter : MonoBehaviour
    {
        [SerializeField, Header("単位の文字サイズ")] private int digitFontSize = 0;
        public int DigitFontSize {get{return digitFontSize;}}
        [SerializeField, Header("小数点の文字サイズ")] private int commaFontSize = 0;
        public int CommaFontSize {get{return commaFontSize;}}
        [SerializeField, Header("単位のフォント")][FontValue] private string digitFontId = "default";
        public string DigitFontName {get{return FontValueAssetLoader.Instance[digitFontId].name;}}
        
        public StringUtility.OmissionTextData GetOmissionData()
        {
            return new StringUtility.OmissionTextData(digitFontSize,  DigitFontName, commaFontSize);
        }
    }
}