using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Club
{
    public class ClubAccessLevelBadge : MonoBehaviour
    {
        [SerializeField] private Image captainBadgeImage;
        [SerializeField] private Image subCaptainBadgeImage;

        public void UpdateBadge(ClubAccessLevel level)
        {
            captainBadgeImage.gameObject.SetActive(level == ClubAccessLevel.Master);
            subCaptainBadgeImage.gameObject.SetActive(level == ClubAccessLevel.SubMaster);
        }
    }
}
