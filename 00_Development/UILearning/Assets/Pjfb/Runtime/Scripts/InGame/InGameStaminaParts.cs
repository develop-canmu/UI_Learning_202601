using Pjfb;
using UnityEngine;
using UnityEngine.UI;

public class InGameStaminaParts : MonoBehaviour
{
    public Image[] staminaList;
    
    public static readonly Color32 Color100 = new Color32(72, 162, 246, 255);
    public static readonly Color32 Color80 = new Color32(130, 246, 99, 255);
    public static readonly Color32 Color60 = new Color32(255, 236, 105, 255);
    public static readonly Color32 Color40 = new Color32(255, 184, 64, 255);
    public static readonly Color32 Color20 = new Color32(212, 29, 25, 255);
    public static readonly Color32 Color0 = new Color32(38, 38, 37, 255);
    
    public void SetUI(float percent)
    {
        Color baseColor = Color100;
        if (percent > 0.8f)
        {
            baseColor = Color100;
        }
        else if (percent > 0.6f)
        {
            baseColor = Color80;
        }
        else if (percent > 0.4f)
        {
            baseColor = Color60;
        }else if (percent > 0.2f)
        {
            baseColor = Color40;
        }
        else
        {
            baseColor = Color20;
        }

        for (int i = 0; i < staminaList.Length; i++)
        {
            float rate = (float)i / (float)staminaList.Length;
            if (rate >= percent)
            {
                staminaList[i].color = Color0;
            }
            else
            {
                staminaList[i].color = baseColor;
            }
        }
    }
}
