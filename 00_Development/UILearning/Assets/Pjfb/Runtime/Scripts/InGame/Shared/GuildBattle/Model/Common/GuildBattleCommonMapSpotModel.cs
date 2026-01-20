using System;
using System.Collections;
using System.Collections.Generic;
using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonMapSpotModel
    {
        #region Properties and SerializeField
        
        // 配置されるレーン番号
        [IgnoreMember]
        public int LaneNumber { get; set; }
        // 本拠地からの距離(左基準)
        [IgnoreMember]
        public int PositionX { get; set; }
        [IgnoreMember]
        public bool IsBase { get; set; }
        [IgnoreMember]
        public bool IsLeftBase { get; set; }
        [IgnoreMember]
        public bool IsRightBase { get; set; }
        [IgnoreMember]
        public List<List<int>> ConnectedSpotIdList; // 0: Left -> Right, 1: Right -> Left

        #region MagicOnion同期対象パラメータ

        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        // 占拠中チーム
        public GuildBattleCommonConst.GuildBattleTeamSide OccupyingSide = GuildBattleCommonConst.GuildBattleTeamSide.All;
        [Key(2)]
        public bool IsBroken = false;
        [Key(3)]
        public int RemainHP = 0;

        [IgnoreMember]
        public int MaxHP;
        
        [IgnoreMember]
        public int AddedWinningPoint;
        
        #endregion

        #endregion

        #region Fields
        #endregion

        #region Public Methods

        // For MessagePack constructor
        public GuildBattleCommonMapSpotModel()
        {
        }

        public GuildBattleCommonMapSpotModel(int id, int laneNumber, int positionX, GuildBattleCommonConst.GuildBattleTeamSide occupyingSide, bool isBase)
        {
            Id = id;
            LaneNumber = laneNumber;
            PositionX = positionX;
            OccupyingSide = occupyingSide;
            IsBase = isBase;
            ConnectedSpotIdList = new List<List<int>>{new List<int>(), new List<int>()};
            RemainHP = isBase ? GuildBattleCommonDataMediator.Instance.GuildBattleSetting.BaseSpotHP : GuildBattleCommonDataMediator.Instance.GuildBattleSetting.SpotHP;
            MaxHP = RemainHP;

            if (isBase)
            {
                if (occupyingSide == GuildBattleCommonConst.GuildBattleTeamSide.Left)
                {
                    IsLeftBase = true;
                }
                else
                {
                    IsRightBase = true;
                }
            }
        }

        public void SyncMapSpotModel(GuildBattleCommonMapSpotModel serverModel)
        {
            this.OccupyingSide = serverModel.OccupyingSide;
            this.IsBroken = serverModel.IsBroken;
            this.RemainHP = serverModel.RemainHP;
        }

        /*
#if !MAGIC_ONION_SERVER
        public string GetSpotName(BattleConst.TeamSide side)
        {
            if (IsBase)
            {
                var key = side == OccupyingSide ? "ingame_guild_battle.ally_base_spot_name" : "ingame_guild_battle.enemy_base_spot_name";
                return LocalizedStringUtility.GetString(LocalizedStringUtility.Table.InGameGuildBattle, key);
            }

            {
                var key = side == OccupyingSide ? "ingame_guild_battle.ally_spot_name" : "ingame_guild_battle.enemy_spot_name";
                return string.Format(LocalizedStringUtility.GetString(LocalizedStringUtility.Table.InGameGuildBattle, key), LaneNumber - 1);
            }
        }
#endif
*/

        public virtual void TakeDamage(int elapsedTurnCount, int brokenSpotCount, int totalMilitaryStrength)
        {
            var damage = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleSpotDamageMin;
            if (GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamagePerTurn > 0 &&
                elapsedTurnCount >= GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamagePerTurn)
            {
                
                damage +=  GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamage *
                           (elapsedTurnCount / GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamagePerTurn);
            }

            damage = Math.Clamp(damage, GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleSpotDamageMin, GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleSpotDamageMax);
            var militaryStrengthBonus = Math.Clamp(totalMilitaryStrength / GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientPerTotalMilitaryStrength, 0, GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleMaxAdditionalSpotDamageCoefficientPerTotalMilitaryStrength);
            damage = (int)Math.Floor(damage * (1.0f + militaryStrengthBonus));
            
            if (IsBase && brokenSpotCount > 0)
            {
                var brokenSpotCountDamageCoef = 1;
                switch (brokenSpotCount)
                {
                    case 1:
                        brokenSpotCountDamageCoef = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientOneSpot;
                        break;
                    case 2:
                        brokenSpotCountDamageCoef = GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleAdditionalSpotDamageCoefficientTwoSpot;
                        break;
                    default:
                        brokenSpotCountDamageCoef = 1;
                        break;
                }
                damage *= brokenSpotCountDamageCoef;
            }

            RemainHP = Math.Max(RemainHP - damage, 0);
            
            var winningPoint = IsBase ? GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointOccupyBaseSpot : GuildBattleCommonDataMediator.Instance.GuildBattleSetting.GuildBattleWinningPointOccupySpot;
            var getWinningPoint = (int)(winningPoint * ((float)damage / MaxHP));
            
            // floatによる計算誤差が発生するので, 拠点を制圧した時点で帳尻を合わせる.
            if (RemainHP <= 0)
            {
                getWinningPoint = winningPoint - AddedWinningPoint;
            }
            
            AddedWinningPoint += getWinningPoint;
            GuildBattleCommonDataMediator.Instance.WinningPoints[(int)GuildBattleCommonLogic.GetOtherTeamSide(OccupyingSide)] += getWinningPoint;

            // isBrokenの設定をしたいところだけど, このターンのIsBrokenの変化を取りたいので一括で別途更新するフローで.
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}