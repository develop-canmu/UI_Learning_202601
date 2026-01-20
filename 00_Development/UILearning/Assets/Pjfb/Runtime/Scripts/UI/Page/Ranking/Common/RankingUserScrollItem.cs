using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Unity.VisualScripting;

namespace Pjfb.Ranking
{
    public class RankingUserScrollItem : ScrollGridItem
    {
        public class Arguments
        {
            
            private long userId = 0;
            /// <summary>UserId</summary>
            public long UserId{get{return userId;}}
            
            private string userName = string.Empty;
            /// <summary>ユーザー名</summary>
            public string UserName{get{return userName;}}
            
            private long mCharId = 0;
            /// <summary>アイコンId</summary>
            public long MCharId{get{return mCharId;}}
            
            private BigValue combatPower = BigValue.Zero;
            /// <summary>戦力</summary>
            public BigValue CombatPower{get{return combatPower;}}
            
            private long charaRank = 0;
            /// <summary>キャラのランク</summary>
            public long CharaRank{get{return charaRank;}}
            
            private long ranking = 0;
            /// <summary>ランキング</summary>
            public long Ranking{get{return ranking;}}
            
            private long scenarioId = 0;
            /// <summary>シナリオId</summary>
            public long ScenarioId{get{return scenarioId;}}
            
            public Arguments(long userId, string userName, long mCharId, BigValue combatPower, long charaRank, long ranking, long scenarioId)
            {
                this.userId      = userId;
                this.userName    = userName;
                this.mCharId     = mCharId;
                this.combatPower = combatPower;
                this.charaRank   = charaRank;
                this.ranking     = ranking;
                this.scenarioId  = scenarioId;
            }
        }
        
        
        [SerializeField]
        private RankingUserInfoView userInfoView = null;
        
        
        protected override void OnSetView(object value)
        {
            Arguments args = (Arguments)value;
            
            // ユーザーId
            userInfoView.SetUserId( args.UserId );
            // キャラId
            userInfoView.SetCharacter( args.MCharId, args.CombatPower, args.CharaRank );
            // ユーザー名
            userInfoView.SetName( args.UserName );
            // ランキング
            userInfoView.SetRank(args.Ranking, args.UserId == UserDataManager.Instance.user.uMasterId );
            // シナリオ
            userInfoView.SetScenario( args.ScenarioId );
            // プロフィールボタン
            userInfoView.SetButtonProfileActive( args.UserId != UserDataManager.Instance.user.uMasterId );

        }
    }
}