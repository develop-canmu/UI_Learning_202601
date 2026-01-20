using System.Collections;
using System.Collections.Generic;

namespace MagicOnion
{
    public interface IGuildBattleSetting
    {

#region 動作設定系

        public int GuildBattlePerTurnDelayMilliseconds { get; set; }
        public int CommendWinStreakCountForLog { get; set; }
        public int CommendWinStreakCountForCutIn { get; set; }
        
#endregion

#region 拠点/ダメージ関連

        public int SpotHP { get; set; }
        public int BaseSpotHP { get; set; }
        
        public int GuildBattleSpotDamageMin { get; set; }
        public int GuildBattleSpotDamageMax { get; set; }
        public int GuildBattleAdditionalSpotDamage { get; set; }
        public int GuildBattleAdditionalSpotDamagePerTurn { get; set; }
        public int GuildBattleAdditionalSpotDamageCoefficientOneSpot { get; set; }
        public int GuildBattleAdditionalSpotDamageCoefficientTwoSpot { get; set; }
        
        public int GuildBattleAdditionalSpotDamageCoefficientPerTotalMilitaryStrength { get; set; }
        public int GuildBattleMaxAdditionalSpotDamageCoefficientPerTotalMilitaryStrength { get; set; }
        
#endregion

#region 兵力系

        public int GuildBattleInitialMilitaryStrength { get; set; }
        public int GuildBattleRequiredTurnToRecoveryMilitaryStrength { get; set; }
        public int GuildBattleRecoveryMilitaryStrengthPerTurn { get; set; }
        public int GuildBattleAdditionalMilitaryStrengthPerSpotBroken { get; set; }
        
        public int GuildBattleRevivalTurn { get; set; }
        public int GuildBattleRevivalTurnPenaltyPerBeaten { get; set; }
        public int GuildBattleStatusPenaltyByWinStreakCountStartAt { get; set; }
        public float GuildBattleStatusPenaltyPerWinStreak { get; set; }
        public float GuildBattleMaxStatusPenaltyByWinStreak { get; set; }
        public int GuildBattleNPCPartyDefaultMilitaryStrength { get; set; }
        public int[] GuildBattleMilitaryStrengthCaps { get; set; }
        
#endregion

#region 移動系

        public int GuildBattleMovementValue { get; set; }

#endregion
                
#region 勝利点関連

        public int GuildBattleWinningPointOccupyBaseSpot { get; set; }
        public int GuildBattleWinningPointOccupySpot { get; set; }
        public int GuildBattleWinningPointJoinPlayer { get; set; }
        public int GuildBattleWinningPointPerWinBattle { get; set; }
        public int GuildBattleWinningPointPerInBattlePoint { get; set; }
        
        public int GuildBattleMaxWinningPointFromTime { get; set; }
        public int GuildBattleMinusWinningPointPerMin { get; set; }
        
        public int GuildBattleWinningPointWin { get; set; }
        
#endregion
        
#region アイテム関連

        public int GuildBattleRecoveryValueOnUseItem { get; set; }
        public int GuildBattleItemCoolDown { get; set; }

#endregion
        
    }
}