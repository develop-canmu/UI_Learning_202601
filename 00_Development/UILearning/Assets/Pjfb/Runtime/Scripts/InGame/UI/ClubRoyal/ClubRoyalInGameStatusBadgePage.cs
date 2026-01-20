using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class ClubRoyalInGameStatusBadgePage : MonoBehaviour
    {
        #region Properties and SerializeField
        [SerializeField] private List<ClubRoyalInGameStatusBadge> guildBattleStatusBadgesList;
        #endregion

        #region Public Methods
        public const int BadgeMaxCount = 3;
        // ページが持っているバッジの数を返す
        public int GetBadgeCount()
        {
            return guildBattleStatusBadgesList.Count;
        }
        #endregion        
        
        #region Protected and Private Methods
        #endregion

        public void Clear()
        {
            foreach (var badge in guildBattleStatusBadgesList)
            {
                badge.gameObject.SetActive(false);
            }
        }

        public void AddBadge(int badgeIndex, int effectType)
        {
            ClubRoyalInGameStatusBadge target = guildBattleStatusBadgesList[badgeIndex];
            target.Initialize(effectType).Forget();
            target.gameObject.SetActive(effectType > 0);
        }
    }
}