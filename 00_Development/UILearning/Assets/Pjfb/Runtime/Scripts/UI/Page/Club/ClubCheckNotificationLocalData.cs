using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Club
{
    public interface IClubCheckNotificationData {
        bool IsNotification();
    }

    /// <summary>
    /// クラブ通知チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationData  { 
        public ClubCheckNotificationInformationBoard informationBoard = new();
        public ClubCheckNotificationClubInfo clubInfo = new();

        public ClubCheckNotificationSolicitation solicitation = new();  //受信勧誘
        public ClubCheckNotificationRequestJoin requestJoin = new();    //加入申請
        public ClubCheckNotificationSendSolicitation sendSolicitation = new();   //送信勧誘
        

        public void UpdateDateTime( HomeGetDataAPIResponse response ){
            solicitation.UpdateLatestUpdateDate( response.newestGuildInvitationDate );
            requestJoin.UpdateLatestUpdateDate( response.newestJoinRequestDate );
            sendSolicitation.UpdateLatestUpdateDate( response.newestUserInvitationDate );
        }


    }

    /// <summary>
    /// 通知チェック用基クラス
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationBase { 
        [SerializeField]
        protected string lastViewDate = string.Empty; 
        [SerializeField]
        protected string latestUpdateDate = string.Empty; 
        
        public virtual bool IsNotification( ){
            if( string.IsNullOrEmpty(latestUpdateDate) ) {
                return false;
            }

            if( string.IsNullOrEmpty(lastViewDate) ) {
                return true;
            }

            var viewDate = DateTime.Parse(lastViewDate);
            var updateDate = DateTime.Parse(latestUpdateDate);
            return viewDate < updateDate;
        }    

        public void UpdateLastViewDate( string date ){
            lastViewDate = date;
        }

        public void UpdateLastViewDateByNow(){
            lastViewDate = AppTime.Now.ToString();
        }

        public void UpdateLatestUpdateDate( string date ){
            latestUpdateDate = date;
        }
    }

    /// <summary>
    /// お知らせボード通知チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationInformationBoard { 
        [SerializeField]
        string text = string.Empty;
        
        public void Update( ClubData data ){
            this.text = data.tactics;
        }

        public bool IsNotification( ClubData data ){
            return this.text != data.tactics;
        }      
    }

    /// <summary>
    /// クラブ情報通知チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationClubInfo { 
        [SerializeField]
        string introduction = string.Empty;
        [SerializeField]
        long activityPolicyId = 0;
        [SerializeField]
        long recruitmentStatus = 0;
        
        public void Update( ClubData data ){
            this.introduction = data.introduction;
            this.activityPolicyId = data.activityPolicyId;
            this.recruitmentStatus = data.recruitmentStatus;
        }

        public bool IsNotification( ClubData data ){
        
            if( this.introduction != data.introduction ) {
                return true;
            }

            if( this.activityPolicyId != data.activityPolicyId ) {
                return true;
            }

            if( this.recruitmentStatus != data.recruitmentStatus ) {
                return true;
            }

            return false;
        }      
    }


    /// <summary>
    /// 勧誘通知チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationSolicitation : ClubCheckNotificationBase{ 
        [SerializeField]
        List<long> guidIdList = new(); // 勧誘を行なったギルドIDリスト
        
        public void Update( GuildGetInvitationGuildListAPIResponse response ){
            guidIdList.Clear();
            foreach( var club in response.invitationList ){
                guidIdList.Add(club.gMasterId);
            }
            UpdateLastViewDateByNow();
        }

        public bool Contains( long clubId ){
            return guidIdList.Contains(clubId);
        }      
    }


    /// <summary>
    /// 勧誘申請チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationRequestJoin : ClubCheckNotificationBase { 
    
        [SerializeField]
        List<long> requestIdList = new(); // 勧誘を行なったギルドIDリスト
        
        public void Update( ClubData data ){
            requestIdList.Clear();
            foreach( var requestUser in data.requestUserList ){
                requestIdList.Add(requestUser.requestId);
            }
            UpdateLastViewDateByNow();
        }

        public bool Contains( long clubId ){
            return requestIdList.Contains(clubId);
        }      
    }

    /// <summary>
    /// 送信勧誘チェック用
    /// </summary>    
    [Serializable]
    public class ClubCheckNotificationSendSolicitation : ClubCheckNotificationBase { 
    
        [SerializeField]
        List<long> userIdList = new(); // 勧誘を行なったギルドIDリスト
        
        public void Update( GuildGetInvitationUserListAPIResponse response ){
            userIdList.Clear();
            foreach( var user in response.userList ){
                userIdList.Add(user.uMasterId);
            }
            UpdateLastViewDateByNow();
        }

        public void Add( long userId ){
            if( Contains( userId ) ) {
                return;
            }
            userIdList.Add(userId);
            UpdateLastViewDateByNow();
        }

        public void Remove( long userId ){
            if( !Contains( userId ) ) {
                return;
            }
            userIdList.Remove(userId);
            UpdateLastViewDateByNow();
        }

        public bool Contains( long clubId ){
            return userIdList.Contains(clubId);
        }      
    }

    
    

    
}
