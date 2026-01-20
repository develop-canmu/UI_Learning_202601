using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

namespace Pjfb.Menu
{
    public enum TermsSheetType
    {
        /// <summary>利用規約</summary>
        Terms,
        /// <summary>プライバシーポリシー</summary>
        PrivacyPolicy,
        /// <summary>資金決済法に基づく表示</summary>
        PaymentLaw
    }
    
    public class TermsSheetManager : SheetManager<TermsSheetType>
    {
    }
}
