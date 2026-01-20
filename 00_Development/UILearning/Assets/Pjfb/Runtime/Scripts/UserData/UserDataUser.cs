using System;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UniRx;

namespace Pjfb.UserData
{

    public class UserDataUser : IDisposable
    {
        /// <summary>ユーザーID</summary>
        public long uMasterId { get; private set; }
        
        public string friendCode { get; private set; }
        
        /// <summary>ユーザ種別（0: 一般, 1: 社内, 2: 社内ヘビー）</summary>
        public long userType { get; private set; }

        /// <summary>ユーザー名</summary>
        public string name { get; private set; }

        /// <summary>性別（一人称）</summary>
        public long gender { get; private set; }

        /// <summary>
        /// 所属ギルドID
        /// 未所属の場合は0
        /// </summary>
        public long gMasterId { get; private set; }

        /// <summary>ユーザーアイコンID</summary>
        public long mIconId { get; private set; }

        private ReactiveProperty<BigValue> _maxCombatPower = new ReactiveProperty<BigValue>();
        /// <summary>これまでに組んだ戦闘デッキの最大戦力</summary>
        public IReadOnlyReactiveProperty<BigValue> maxCombatPower => _maxCombatPower;

        private ReactiveProperty<long> _maxDeckRank = new ReactiveProperty<long>();
        /// <summary>これまでに組んだ戦闘デッキの最大ランク</summary>
        public IReadOnlyReactiveProperty<long> maxDeckRank { get { return _maxDeckRank; } }

        /// <summary>ギルド勧誘を受け付ける</summary>
        public bool allowsGuildInvitation { get; private set; }

        /// <summary>ギルド勧誘：希望ギルドランク</summary>
        public long guildInvitationGuildRank { get; private set; }

        /// <summary>ギルド勧誘：プレイスタイル</summary>
        public long guildInvitationPlayStyleType { get; private set; }

        /// <summary>ギルド勧誘：ギルドバトルのプレイスタイル</summary>
        public long guildInvitationGuildBattleType { get; private set; }

        /// <summary>ギルド勧誘：アピールコメント</summary>
        public string guildInvitationMessage { get; private set; }
        /// <summary>ギルド勧誘：参加優先度</summary>
        public long guildParticipationPriority { get; private set; }

        /// <summary>ショップ：年齢確認登録</summary>
        public bool hasRegisteredBirthday { get; private set; }
	
        /// <summary>ショップ：親権者（法定代理人）の同意登録</summary>
        public bool hasParentalConsent {get;private set;}
        
        /// <summary>ショップ：月額課金額</summary>
        public long monthPayment { get; private set; }

        /// <summary>ショップ：月額課金上限</summary>
        public long monthPaymentLimit { get; private set; }

        /// <summary>ショップ：初回課金</summary>
        public bool isPaid { get; private set; }

        /// <summary>チュートリアル:完了済みステップ番号</summary>
        public List<long> finishedTutorialNumberList { get; private set; } = new List<long>();

        public void Update(UserLoggedIn user)
        {
            uMasterId = user.uMasterId;
            friendCode = user.friendCode;
            userType = user.userType;
            name = user.name;
            gMasterId = user.gMasterId;
            mIconId = user.mIconId;
            gender = user.gender;
            hasRegisteredBirthday = user.hasRegisteredBirthday;
            hasParentalConsent = user.hasParentalConsent;
            monthPayment = user.monthPayment;
            monthPaymentLimit = user.monthPaymentLimit;
            isPaid = user.isPaid;
            _maxCombatPower.Value = new BigValue(user.maxCombatPower);
            _maxDeckRank.Value = user.maxDeckRank;
            allowsGuildInvitation = user.allowsGuildInvitation;
            guildInvitationGuildRank = user.guildInvitationGuildRank;
            guildInvitationPlayStyleType = user.guildInvitationPlayStyleType;
            guildInvitationGuildBattleType = user.guildInvitationGuildBattleType;
            guildInvitationMessage = user.guildInvitationMessage;
            UpdateFinishedTutorialStep(user.finishedTutorialNumberList);
            guildParticipationPriority = user.guildParticipationPriorityType;
        }

        public void Update(GuildGetInvitationGuildListAPIResponse response)
        {
            if( response.allowsGuildInvitation != 0  ) {
                allowsGuildInvitation = response.allowsGuildInvitation == 1;
            }
        }

        public void Update(UserGetProfileAPIResponse response)
        {
            if( response.allowsGuildInvitation != 0  ) {
                allowsGuildInvitation = response.allowsGuildInvitation == 1;
            }
        }

        public void UpdateUserIconId(long id)
        {
            mIconId = id;
        }

        public void ClearGuildId()
        {
            gMasterId = 0;
        }
        
        public void UpdateGuildData( GuildJoinAPIRequest request )
        {
            gMasterId = request.GetPostData().targetGMasterId;
        }
        

        public void UpdateGuildData(GuildGuildStatus guid)
        {
            gMasterId = guid.gMasterId;
        }

        public void UpdateGuildData(HomeGetDataAPIResponse home)
        {
            gMasterId = home.gMasterId;
        }

        public void UpdateGuildData(GuildAgreeInvitationAPIRequest request)
        {
            gMasterId = request.GetPostData().targetGMasterId;
        }
        public void UpdateUserName(UserUpdateNameAPIResponse request)
        {
            name = request.name;
        }


        public void UpdateUserInvitationSetting(bool allowsGuildInvitation, long guildInvitationGuildRank, long guildInvitationPlayStyleType, long guildInvitationGuildBattleType, string guildInvitationMessage, long guildParticipationPriority)
        {
            this.allowsGuildInvitation = allowsGuildInvitation;
            this.guildInvitationGuildRank = guildInvitationGuildRank;
            this.guildInvitationPlayStyleType = guildInvitationPlayStyleType;
            this.guildInvitationGuildBattleType = guildInvitationGuildBattleType;
            this.guildInvitationMessage = guildInvitationMessage;
            this.guildParticipationPriority = guildParticipationPriority;
        }

        public void UpdateMonthPayment(long monthPayment)
        {
            this.monthPayment = monthPayment;
        }
	
	public void UpdateMonthPaymentData(long monthPayment, long monthPaymentLimit, bool hasParentalConsent)
        {
            this.monthPayment = monthPayment;
            this.monthPaymentLimit = monthPaymentLimit;
            this.hasRegisteredBirthday = true;
            this.hasParentalConsent = hasParentalConsent;
        }
        
        public void UpdateParentalConsent(bool hasParentalConsent)
        {
            this.hasParentalConsent = hasParentalConsent;
        }

        public void UpdateUserPersonalData(UserInitializeAPIResponse response)
        {
            name = response.name;
            gender = response.gender;
            mIconId = response.mIconId;
        }

        public void UpdateFinishedTutorialStep(long[] finishedStep)
        {
            foreach (var value in finishedStep)
            {
                var step = value;
                if (!finishedTutorialNumberList.Contains(step))
                {
                    finishedTutorialNumberList.Add(step);   
                }
            }
        }

        public void UpdateIsPaidValue(bool isPaid)
        {
            this.isPaid = isPaid;
        }

        public void UpdateMaxDeckCombatPower(BigValue combatPower)
        {
            if (combatPower <= maxCombatPower.Value) return;
            _maxCombatPower.Value = combatPower;
            _maxDeckRank.Value = StatusUtility.GetPartyRank(combatPower);

        }

        public void Dispose()
        {
            _maxCombatPower.Dispose();
            _maxDeckRank.Dispose();
        }
    }
}
