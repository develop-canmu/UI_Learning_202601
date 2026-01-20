using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Pjfb.Master;
using CruFramework;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Community;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;


namespace Pjfb.Club {
    public enum ClubMatchPolicy {
        None,
        Positive,
        One,
        Private
    }   

    public enum ClubAccessLevel {
        NotAffiliation,   //未所属
        Master, //マスター
        SubMaster,//サブマスター
        None,   //何もなし
    }   

    // クラブの加入許可タイプ
    public enum ClubEntryPermissionType
    {
        NotApply = 0, //申請不可
        AutoApprove = 1, //自動承認
        ManualApprove = 2, //　手動承認
    }

    public static class ClubMatchPolicyExtensions {
        public static string ToName(this ClubMatchPolicy val) {
            switch (val) {
                case ClubMatchPolicy.None:
                    return StringValueAssetLoader.Instance["club.noneConditions"];
                case ClubMatchPolicy.Positive:
                    return StringValueAssetLoader.Instance["club.matchPolicyPositive"];
                case ClubMatchPolicy.One:
                    return StringValueAssetLoader.Instance["club.matchPolicyOne"];
                case ClubMatchPolicy.Private:
                    return StringValueAssetLoader.Instance["club.matchPolicyPrivate"];
                default:
                    return string.Empty;
            }
        }
    }


    public static class ClubUtility {

        public const int clubLockId = 200001;
        public static readonly int[] activityPolicyIds = new int[]{1,3,5};
        private static Dictionary<long, ConfGuildSearchParticipationPriorityData> participationPriorityDataMap = new ();
        
        static public List<string> CreateActivityPolicyStrings(){
            //表示するIdは固定
            var viewIds = activityPolicyIds;
            
            var list = new List<string>();
            list.Add(StringValueAssetLoader.Instance["club.noneConditions"]);
            foreach( var master in MasterManager.Instance.guildPlayStyleMaster.values ){
                if( viewIds.Any( id => id == master.id ) ) {
                    list.Add(master.name);
                }
            }
            return list;
        }

        static public string FindActivityPolicyStrings( long policyId ){
            foreach( var master in MasterManager.Instance.guildPlayStyleMaster.values ){
                if( master.id == policyId ) {
                    return master.name;
                }
            }
            return StringValueAssetLoader.Instance["club.noneConditions"];
        }

        static public int ActivityPolicyStringsIndexToId( int index ){
            if( index == 0 ) {
                return 0;
            }
            return activityPolicyIds[index-1];
        }


        static public List<string> CreateAutoEnrollmentStrings(){
            var list = new List<string>(){ 
                StringValueAssetLoader.Instance["club.noneConditions"],
                StringValueAssetLoader.Instance["club.automaticAuthentication"],
                StringValueAssetLoader.Instance["club.manualAuthentication"],
            };
            
            return list;
        }

        static public List<string> CreateMemberRecruitmentStatusStrings(){
            var list = new List<string>(){ 
                StringValueAssetLoader.Instance["club.noneConditions"],
                StringValueAssetLoader.Instance["club.recruiting"],
                StringValueAssetLoader.Instance["club.recruitmentStopped"],
            };
            return list;
        }

        static public List<string> CreateMatchPolicyStrings(){
            var list = new List<string>();
            foreach (ClubMatchPolicy Value in System.Enum.GetValues(typeof(ClubMatchPolicy))) {
                list.Add(Value.ToName());
            }
            return list;
        }
        
        private static Dictionary<long, ConfGuildSearchParticipationPriorityData> CreateParticipationPriorityMap()
        {
            if (participationPriorityDataMap.Count > 0)
            {
                // キャッシュが存在する場合はそのまま返す
                return participationPriorityDataMap;
            }
            return ConfigManager.Instance.guildSearchParticipationPriorityTypeList.ToDictionary(data => data.id, data => data);
        }
        
        public static ConfGuildSearchParticipationPriorityData GetParticipationPriorityData(long id)
        {
            if (participationPriorityDataMap.Count == 0)
            {
                // キャッシュが空の場合は初期化
                participationPriorityDataMap = CreateParticipationPriorityMap();
            }
            
            if (participationPriorityDataMap.TryGetValue(id, out ConfGuildSearchParticipationPriorityData data))
            {
                return data;
            }
            else
            {
                CruFramework.Logger.LogError($"ID: {id} が存在しません");
                // 最初の要素を返す
                return participationPriorityDataMap.Values.First();
            }
        }

        static public string CreateMemberQuantityString( long quantity ){
            return quantity + "/" + Pjfb.ConfigManager.Instance.maxGuildMemberCount;
        }

        static public async UniTask LoadAndSetEmblemIcon( Image image,  long iconId ){
            var sprite = await LoadEmblemIcon( iconId, image.GetCancellationTokenOnDestroy() );
            if( image != null ) {
                image.sprite = sprite;
            }
        }

        static public async UniTask<Sprite> LoadEmblemIcon(long iconId, CancellationToken token ){
            string path = ResourcePathManager.GetPath("ClubEmblem", iconId);
            Sprite loadSprite = null;
            await PageResourceLoadUtility.LoadAssetAsync<Sprite>(path, sprite=>loadSprite = sprite, token);
            return loadSprite;
        }


        static public void LoadAndSetEmblemIconSync( Image image,  long iconId ){
            var sprite = LoadEmblemIconSync( iconId );
            if( image != null ) {
                image.sprite = sprite;
            }
        }

        static public Sprite LoadEmblemIconSync(long iconId ){
            string path = ResourcePathManager.GetPath("ClubEmblem", iconId);
            var loadSprite = PageResourceLoadUtility.LoadAsset<Sprite>(path);
            return loadSprite;
        }


        static public async UniTask LoadAndSetRankIcon( Image image,  long iconId ){
            var sprite = await LoadRankIcon( iconId, image.GetCancellationTokenOnDestroy() );
            if( image != null ) {
                image.sprite = sprite;
            }
        }

        static public async UniTask<Sprite> LoadRankIcon( long iconId, CancellationToken token ){
            string path = ResourcePathManager.GetPath("ClubRank");
            path = string.Format(path, iconId);
            Sprite loadSprite = null;
            await PageResourceLoadUtility.LoadAssetAsync<Sprite>(path, sprite=>loadSprite = sprite, token);
            return loadSprite;
        }


        static public int GetToggleParam( ToggleGroup group ){
            var toggles = group.GetComponentsInChildren<Toggle>();
            foreach( var toggle in toggles ){
                if( !toggle.isOn ) {
                    continue;
                }
                var param = toggle.GetComponent<ClubToggleParam>();
                if( param != null ) {
                    return param.param;
                }
            }
            return 0;
        }

        static public void SetActiveToggle( ToggleGroup group, long param ){ 
            var toggles = group.GetComponentsInChildren<Toggle>();
            foreach( var toggle in toggles ){
                var toggleParam = toggle.GetComponent<ClubToggleParam>();
                if( toggleParam.param == param ) {
                    toggle.isOn = true;
                    break;
                }
            }
        }

        static public ClubAccessLevel CreateAccessLevel( long userId,  ClubData clubData ){
            foreach(var member in clubData.memberList ) {
                if( member.userId != userId ) {
                    continue;
                }

                var master = FindRoleMaster( member.roleId );
                if( master == null ) {
                    continue;
                }
                return (ClubAccessLevel)master.accessLevel;
            }
            return ClubAccessLevel.NotAffiliation;
        }

        static public ClubAccessLevel CreateAccessLevel( long roleId ){
            var master = FindRoleMaster(roleId );
            if( master == null ) {
                return ClubAccessLevel.None;
            }
            return (ClubAccessLevel)master.accessLevel;
        }

        static public long AccessLevelToRoleId( ClubAccessLevel accessLevel ){
            foreach( var role in MasterManager.Instance.guildRoleMaster.values ){
                if( role.accessLevel == (int)accessLevel ) {
                    return role.id;
                }
            }

            return -1;
        }

        static public GuildRoleMasterObject FindRoleMaster( long roleId ){

            foreach( var role in MasterManager.Instance.guildRoleMaster.values ){
                if( roleId == role.id ) {
                    return role;
                }
            }
                
            return null;
        }


        //最終ログインテキストの作成
        static public string CreateLastLoginText(string date)
        {  
            return Pjfb.Community.CommunityManager.GetDateTimeDiffByString(date);
        }

        static public List<ClubMemberData> CreateSortList( List<ClubMemberData> guildMemberList ){

            var members = new List<ClubMemberData>();
            members.AddRange(CreateMemberListByAccessLevel(guildMemberList, ClubAccessLevel.Master));
            members.AddRange(CreateMemberListByAccessLevel(guildMemberList, ClubAccessLevel.SubMaster));
            members.AddRange(CreateMemberListByAccessLevel(guildMemberList, ClubAccessLevel.None));
            return members;
        }

        static public List<ClubMemberData> CreateMemberListByAccessLevel( List<ClubMemberData> guildMemberList, ClubAccessLevel targetAccessLevel ){
            var members = new List<ClubMemberData>();
            foreach( var member in guildMemberList ){
                var accessLevel = ClubUtility.CreateAccessLevel( member.roleId );
                if( targetAccessLevel == accessLevel ) {
                    members.Add(member);
                }
            }
            return members;
        }

        /// <summary>
        /// ブロックユーザーのチェックと確認ダイアログの表示
        /// trueで申請、falseでキャンセル
        /// </summary>
        static public async UniTask<bool> CheckAndShowConfirmByBlockUser( List<ClubMemberData> memberList ){
            
            if( await IsAnyBlockUser(memberList) ) {
                var doRequest = false;
                var data = new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"],
                    StringValueAssetLoader.Instance["club.confirmBlockUserText"],
                    null,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],(window)=>{ 
                        doRequest = true;
                        window.Close();
                     }),
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (window) => {
                        doRequest = false;
                        window.Close();
                    })
                );

                var modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
                await modal.WaitCloseAsync();
                return doRequest;
            } else {
                return true;
            }
        }

        /// <summary>
        /// ブロックユーザーが含まれているか
        /// </summary>
        /// <returns></returns>
        static public async UniTask<bool> IsAnyBlockUser( List<ClubMemberData> memberList ){
            //ブロックリストを取得していなかったら取得する
            if( !CommunityManager.isGetBlockUserList ) {
                CommunityGetBlockListAPIRequest request = new CommunityGetBlockListAPIRequest();
                await APIManager.Instance.Connect(request);
            }
            foreach( var member in memberList ){
                foreach( var blockUser in CommunityManager.blockUserList ){
                    if( member.userId == blockUser.uMasterId ) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// メンバー募集状態か
        /// </summary>
        /// <returns></returns>
        static public bool IsMembersWantedFlg( long membersWantedFlg, long memberQuantity ){
            
            if( membersWantedFlg != 1 ) {
                return false;
            }

            //人数が最大だったら停止中
            if( memberQuantity >= Pjfb.ConfigManager.Instance.maxGuildMemberCount ) {
                return false;
            }
            return true;
        }
        
        static public bool OpenClubLockModal()
        {
            var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(Pjfb.Club.ClubUtility.clubLockId);
            // マスタなし
            if(systemLock == null) {
                return false;
            }
            string description = systemLock.description;
            if(string.IsNullOrEmpty(description)){
                return false;
            }
            
            // モーダル
            ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
            ConfirmModalData data = new ConfirmModalData( StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
            return true;
        }
        
        // クラブ検索画面に移動
        public static void ChangeFindClubPage(){
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            //クラブの所属画面に遷移
            if (AppManager.Instance.UIManager.PageManager.CurrentPageObject is ClubPage clubPage)
            {
                var param = new FindClubPage.Param();
                param.isFirstOpenSolicitationList = true;
                clubPage.OpenPage(ClubPageType.FindClub, false, param);
            }
        }
    }

}