using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;


namespace Pjfb.Club
{

    public class ClubUserData { 
        public long userId{get; protected set;} = 0;
        public string name{get; protected set;} = "";
        public long iconId{get; protected set;} = 0;
        public BigValue power{get; protected set;} = new BigValue(0);
        public long deckRank{get; protected set;} = 0;
        public long emblemId{get; protected set;} = 0;
        public string lastLogin{get; protected set;} = "";
        public string message{get; protected set;} = "";
        public long activityPolicy{get; protected set;} = 0;
        public long rankCondition{get; protected set;} = 0;
        public long clubMatchPolicy{get; protected set;} = 0;
        public long roleId{get; protected set;} = 0;
        public long guildParticipationPriorityType { get; protected set; } = 0;


        public ClubUserData( ClubUserData status ){
            this.userId = status.userId;
            this.name = status.name;
            this.message = status.message;
            this.iconId = status.iconId;
            this.power = status.power;
            this.deckRank = status.deckRank;
            this.emblemId = status.emblemId;
            this.lastLogin = status.lastLogin;
            this.activityPolicy = status.activityPolicy;
            this.rankCondition = status.rankCondition;
            this.clubMatchPolicy = status.clubMatchPolicy;
            this.roleId = status.roleId;
            this.guildParticipationPriorityType = status.guildParticipationPriorityType;
        }

        public ClubUserData( GuildMemberMemberStatus status ){
            this.userId = status.uMasterId;
            this.name = status.name;
            // this.message = status.message;
            this.iconId = status.mIconId;
            this.power = new BigValue(status.maxCombatPower);
            this.deckRank = status.maxDeckRank;
            this.emblemId = status.mTitleId;
            this.lastLogin = status.lastLogin;
            this.roleId = status.mGuildRoleId;
            this.guildParticipationPriorityType = status.guildParticipationPriorityType;
        }

        public ClubUserData( UserGuildJoinRequestUserStatus status ){
            this.userId = status.uMasterId;
            this.name = status.name;
            // this.message = status.message;
            this.iconId = status.mIconId;
            this.power = new BigValue(status.maxCombatPower);
            this.deckRank = status.maxDeckRank;
            this.emblemId = status.mTitleId;
            this.lastLogin = status.lastLogin;
            this.guildParticipationPriorityType = status.participationPriorityType;
        }

        public ClubUserData( UserGuildInvitationUserStatus status ){
            this.userId = status.uMasterId;
            this.name = status.name;
            this.message = status.message;
            this.iconId = status.mIconId;
            this.power = new BigValue(status.maxCombatPower);
            this.deckRank = status.maxDeckRank;
            this.emblemId = status.mTitleId;
            this.activityPolicy = status.playStyleType;
            this.rankCondition = status.guildRank;
            this.clubMatchPolicy = status.guildBattleType;
            this.lastLogin = status.lastLogin;
            this.guildParticipationPriorityType = status.participationPriorityType;
        }
    }
    
    public class ClubMemberData : ClubUserData { 
        

        public ClubMemberData( GuildMemberMemberStatus status ) : base( status ){
            this.roleId = status.mGuildRoleId;
        }

        public void UpdateRoleId( long roleId ){
            this.roleId = roleId;
        }
    }

    public class ClubRequestUserData : ClubUserData { 
        public long requestId{get; private set;} = 0; // ギルド加入申請ID

        public ClubRequestUserData( UserGuildJoinRequestUserStatus status ) : base( status ){
            requestId = status.gJoinRequestId;
        }
    }

    public class ClubInvitationMemberData : ClubUserData { 

        public ClubInvitationMemberData( UserGuildInvitationUserStatus status ) : base( status ){
        }
    }

    public class ClubData {
        public long clubId{get; private set;} = 0;
        public string name{get; private set;} = "";
        public string introduction{get; private set;} = "";
        public long emblemId{get; private set;} = 0;
        public long rankId{get; private set;} = 0;
        public BigValue power{get; private set;} = new BigValue(0);
        public long activityPolicyId{get; private set;} = 0;
        public ClubEntryPermissionType isAutoEnrollment{get; private set;} = 0;
        public string tactics{get; private set;} = "";
        public long guildRankPoint{get; private set;} = 0;
        public long nextGuildRankPoint{get; private set;} = 0;
        public long recruitmentStatus{get;set;} = 0;
        public long clubMatchPolicy{get;set;} = 0;
        public string recruitmentComment{get;set;} = "";
        public long[] emblemIdList {get; private set;} = null;
        public List<ClubMemberData> memberList {get; private set;} = new List<ClubMemberData>();
        public List<ClubRequestUserData> requestUserList {get; private set;} = new List<ClubRequestUserData>();
        public long participationPriority { get; private set; } = 0;

        public ClubData( GuildGuildStatus status ){
            UpdateData(status);
        }

        public void UpdateData( GuildGuildStatus status ){ 
            this.clubId = status.gMasterId;
            this.name = status.name;
            this.introduction = status.introduction;
            this.emblemId = status.mGuildEmblemId;
            this.power = new BigValue(status.combatPower);
            this.rankId = status.mGuildRankId;
            this.tactics = status.tactics;
            this.guildRankPoint = status.guildRankPoint;
            this.nextGuildRankPoint = status.nextGuildRankPoint;
            this.emblemIdList = status.mGuildEmblemIdList;
            
            this.activityPolicyId = status.mGuildPlayStyleId;
            this.isAutoEnrollment = (ClubEntryPermissionType) status.autoEnrollmentFlg;

            this.clubMatchPolicy = status.guildBattleStyleType;
            this.recruitmentComment = status.membersWantedComment;
            this.recruitmentStatus = status.membersWantedFlg;
            
            this.participationPriority = status.participationPriorityType;


            memberList.Clear();
            foreach(var user in status.guildMemberList ){
                var member = new ClubMemberData(user);
                memberList.Add(member);
            }

            requestUserList.Clear();
            foreach(var user in status.joinRequestList ){
                var requestUser = new ClubRequestUserData(user);
                requestUserList.Add(requestUser);
            }
        }

        public void UpdateData( EditClubDate editData ){
            emblemId = editData.emblemId;
            introduction = editData.introduction;
            activityPolicyId = editData.activityPolicyId;
            recruitmentStatus =  editData.recruitmentStatus;
            recruitmentComment = editData.recruitmentComment;
            isAutoEnrollment = (ClubEntryPermissionType) editData.isAutoEnrollment;
            clubMatchPolicy = editData.clubMatchPolicy;
            participationPriority = editData.participationPriority;
        }

        public void UpdateDataRoleId( long userId, long roleId){
            foreach( var member in memberList ) {
                if( member.userId == userId ) {
                    member.UpdateRoleId(roleId);
                    break;
                }
            }
        }

        public void RemoveMember( long userId ){
            ClubMemberData removeMember = null;
            foreach( var member in memberList ) {
                if( member.userId == userId ) {
                    removeMember = member;
                    break;
                }
            }
            if( removeMember != null ) {
                memberList.Remove(removeMember);
            }

            UpdatePowerByLocalData();
        }

        public void UpdateTactics( string tactics ){
            this.tactics = tactics;
        }

        /// <summary>
        /// ローカルデータから戦力更新
        /// </summary>
        public void UpdatePowerByLocalData(){
            BigValue power = new BigValue(0);
            foreach( var member in memberList ) {
                power += member.power;
            }
            this.power = power;
        }

    }
}
