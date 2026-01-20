using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonPartyModel
    {
        #region Properties and SerializeField

        public enum PartyState
        {
            None,
            OnMap,      // マップに出ていて戦闘が可能な状態
            Standby,    // 出撃後敗北して再出撃を待っている状態
        }

        [Key(0)]
        public long PlayerId;
        [Key(1)]
        public int PlayerIndex;
        // 頻繁にやりとりされるデータなのでintに. もしlongである必要性が出てきたら変更.
        [Key(2)]
        public List<long> UnitIds;
        [Key(3)]
        public List<int> MilitaryStrength;
        [Key(19)]
        public List<int> LastMilitaryStrength;
        [Key(4)]
        public List<int> MaxMilitaryStrength;
        [Key(5)]
        public int LaneNumber;
        [Key(6)]
        public int XPosition;
        [Key(18)]
        public int LastXPosition;
        [Key(7)]
        public int ExpectedXPosition;
        [Key(8)]
        public int TargetSpotId;
        [Key(9)]
        public int LastJoinedSpotId;
        [Key(21)]
        public int StartSpotId;
        [Key(10)]
        public int PartyId; // マジでユニークなid
        [Key(11)]
        public int PlayerPartyId; // 表示用のプレイヤーごとに1から振られるid.

        [Key(13)]
        public GuildBattleCommonConst.GuildBattleTeamSide Side;

        [Key(14)]
        public int WinStreakCount;

        [Key(15)]
        public PartyState State = PartyState.None;

        [Key(16)]
        public long TacticsId;
        
        [Key(17)]
        public long[] RoleOperationIds;

        [Key(20)]
        public bool IsFighting;
        
        [IgnoreMember]
        public bool OnReDeployable = false;
        
        #endregion

        #region Fields

        public static readonly long[] DefaultRoleOperationIds = { 1, 2, 3, 4, 5 };

        #endregion

        #region Public Methods

        /// <summary>
        /// PositionXとLaneNumberから作られるユニークなId
        /// </summary>
        /// <returns></returns>
        public int GetLaneAndPositionId()
        {
            return GuildBattleCommonLogic.GetLanePositionId(LaneNumber, XPosition);
        }
        
        public int GetLastLaneAndPositionId()
        {
            return GuildBattleCommonLogic.GetLanePositionId(LaneNumber, LastXPosition);
        }

        public bool IsOnMap()
        {
            return State == PartyState.OnMap;
        }
        
        public void SyncLastMilitaryStrengthToMilitaryStrength()
        {
            for (var i = 0; i < MilitaryStrength.Count; i++)
            {
                LastMilitaryStrength[i] = MilitaryStrength[i];
            }
        }

        public void SetAsStandby()
        {
            for (var i = 0; i < MilitaryStrength.Count; i++)
            {
                MilitaryStrength[i] = 0;
            }
            
            State = PartyState.Standby;
        }

        public bool IsDefendingAtSpot(int spotId)
        {
            return IsDefendingAtAnySpot() && TargetSpotId == spotId;
        }
        
        public bool IsDefendingAtAnySpot()
        {
            return LastJoinedSpotId == TargetSpotId;
        }
        
        public virtual void OnDeadParty()
        {
            var player = GuildBattleCommonDataMediator.Instance.GetBattlePlayerData(PlayerIndex);
            player.OnDeadPartyCount++;
            SetAsStandby();
        }
        
        /// <summary>
        /// 解散フロー
        /// </summary>
        public virtual void OnDissolutionParty(bool isBeforeFight)
        {
            var revivalTurn = 0;

            // 開戦後 && マップ上にいる場合のみペナルティをかける.
            // マップ上にいない場合=戦闘のフローでOnDeadPartyにいっている.
            if (!isBeforeFight && IsOnMap())
            {
                OnDeadParty();
            }
            
            foreach (var unitId in UnitIds)
            {
                if (unitId <= 0)
                {
                    continue;
                }

                var unit = GuildBattleCommonDataMediator.Instance.GetUnitState(unitId);
                if (unit == null)
                {
                    continue;
                }

                if (revivalTurn > 0)
                {
                    unit.OnDeadUnit(revivalTurn);
                }
                
                unit.OnDissolutionParty();
            }
        }
        
        /*
        public bool IsStayingAtAnySpot()
        {
            var joinedSpot = GuildBattleDataMediator.Instance.BattleField.GetMapSpot(LastJoinedSpotId);
            if (joinedSpot == null)
            {
                return false;
            }

            return joinedSpot.PositionX == XPosition;
        }

        public float GetMilitaryStrengthRatio(bool isLast)
        {
            var maxSum = MaxMilitaryStrength.Sum();
            var currentSum = isLast ? LastMilitaryStrength.Sum() : MilitaryStrength.Sum();
            return currentSum / (float)maxSum;
        }

        

        


        public int GetRevivalTurnCount()
        {
            if (IsOnMap())
            {
                return -1;
            }

            var unitId = UnitIds.FirstOrDefault();
            // 同じ軍隊に属する部隊は全て同じクールタイムなので, 一つ見れば十分
            var unit = GuildBattleDataMediator.Instance.GetUnitState(unitId);
            if (unit == null)
            {
                return -1;
            }
            
            return unit.RevivalTurn;
        }
        */
        
        public bool IsStayingAtAnySpot()
        {
            var joinedSpot = GuildBattleCommonDataMediator.Instance.BattleField.GetMapSpot(LastJoinedSpotId);
            if (joinedSpot == null)
            {
                return false;
            }

            return joinedSpot.PositionX == XPosition;
        }
        
        public bool IsWinStreakCountGreaterThanCommendWinStreakCountForLog()
        {
            return WinStreakCount >= GuildBattleCommonDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForLog;
        }
        
        public bool IsAchieveLogCommendedWinStreakCount()
        {
            return WinStreakCount > 0 && WinStreakCount % GuildBattleCommonDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForLog == 0;
        }
        
        public bool IsAchieveCutInCommendedWinStreakCount()
        {
            return WinStreakCount > 0 && WinStreakCount % GuildBattleCommonDataMediator.Instance.GuildBattleSetting.CommendWinStreakCountForCutIn == 0;
        }
        
        #endregion

        #region Protected and Private Methods

        #endregion
    }
}