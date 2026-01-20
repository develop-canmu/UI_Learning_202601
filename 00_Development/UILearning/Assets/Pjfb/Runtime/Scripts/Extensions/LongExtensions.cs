using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.Extensions
{
    public static class LongExtensions
    {
        public static string GetStringNumberWithComma(this long value, int digitCount = 3)
        {
            var retVal = value.ToString();
            for (var i = retVal.Length - digitCount; i > 0; i -= digitCount) retVal = retVal.Insert(i, ",");
            return retVal;
        }
    }
}