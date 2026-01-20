using UnityEngine;

using Pjfb.Master;


namespace Pjfb.Common
{
    public class MissionSheetSwitchButton : SheetSwitchButton<MissionSheetManager, MissionTabType>
    {
        [SerializeField] private UIBadgeNotification badgeNotification;

        public void SetBadge(bool isActive)
        {
            badgeNotification.SetActive(isActive);
        }
    }
}