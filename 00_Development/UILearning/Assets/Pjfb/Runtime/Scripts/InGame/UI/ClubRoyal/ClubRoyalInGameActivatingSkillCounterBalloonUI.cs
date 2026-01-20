using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActivatingSkillCounterBalloonUI : MonoBehaviour
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private TMP_Text countText;

        public void Display(List<BattleV2AbilityEffect> abilityEffectList)
        {
            // リストがnullまたは空の場合は非表示にする
            if (abilityEffectList == null || abilityEffectList.Count == 0)
            {
                rootObject.SetActive(false);
                return;
            }
            
            Display(abilityEffectList.Count);
        }
        
        public void Display(int count)
        {
            // カウントが0以下の場合は非表示にする
            if (count <= 0)
            {
                rootObject.SetActive(false);
                return;
            }

            countText.text = count.ToString();
            rootObject.SetActive(true);
        }
    }
}
