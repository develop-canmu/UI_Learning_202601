using TMPro;
using UnityEngine;

namespace Pjfb
{
    public static class TMPMaterialPropertyUtility
    {
        // 除外されるプロパティ(TMP側での自動計算対象パラメータ)
        public static readonly int[] exceptPropertyList = new[]
        {
            ShaderUtilities.ID_ScaleRatio_A, ShaderUtilities.ID_ScaleRatio_B, ShaderUtilities.ID_ScaleRatio_C
        };
    }
}