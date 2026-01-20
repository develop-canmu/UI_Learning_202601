using UnityEngine;
using Pjfb.Storage;



namespace Pjfb.Club
{
    public class MemberTopPageBadgeController : MonoBehaviour
    {

        [SerializeField]
        UIBadgeNotification _notificationBoardBadge = null;
        [SerializeField]
        UIBadgeNotification _findMemberBadge = null;
        [SerializeField]
        UIBadgeNotification _findClubBadge = null;
        [SerializeField]
        UIBadgeNotification _clubInfoBadge = null;
        [SerializeField]
        UIBadgeNotification _clubChatBadge = null;

        
        public void UpdateNotificationBadge( ClubData data ){
            var notificationData = LocalSaveManager.saveData.clubCheckNotificationData;
            _findMemberBadge.SetActive( notificationData.requestJoin.IsNotification() 
            || notificationData.sendSolicitation.IsNotification() );

            _findClubBadge.SetActive( notificationData.solicitation.IsNotification() );

            _notificationBoardBadge.SetActive( notificationData.informationBoard.IsNotification(data) );
            _clubInfoBadge.SetActive( notificationData.clubInfo.IsNotification(data) );

            _clubChatBadge.SetActive( Pjfb.Community.CommunityManager.unViewedClubChatCount + Pjfb.Community.CommunityManager.unViewedClubInfoCount > 0  );
            
        }
    }
}
