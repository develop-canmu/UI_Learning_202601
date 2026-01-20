using MagicOnion;

namespace Pjfb
{
    public class PjfbGuildBattleSetting : IGuildBattleSetting
    {
        #region 動作設定系

        // 変えるな危険.
        public int GuildBattlePerTurnDelayMilliseconds { get; set; } = 5000;

        // 連勝ログ表示しきい値
        public int CommendWinStreakCountForLog { get; set; } = 5;
        // 連勝カットイン表示しきい値
        public int CommendWinStreakCountForCutIn { get; set; } = 5;
        
        #endregion

        #region 拠点/ダメージ関連

        // 衛星拠点HP
        public int SpotHP { get; set; } = 3;
        // 本拠地拠点HP
        public int BaseSpotHP { get; set; } = 10;

        // 拠点ダメージ最小値
        public int GuildBattleSpotDamageMin { get; set; } = 1;
        // 拠点ダメージ最大値
        public int GuildBattleSpotDamageMax { get; set; } = 5;
        // ターンによる拠点追加ダメージ
        public int GuildBattleAdditionalSpotDamage { get; set; } = 0;
        // ターンによる拠点追加ダメージ(nターン毎の除算部分)
        public int GuildBattleAdditionalSpotDamagePerTurn { get; set; } = 0;
        // 本拠地ダメージ1拠点落としているとき
        public int GuildBattleAdditionalSpotDamageCoefficientOneSpot { get; set; } = 0;
        // 本拠地ダメージ2拠点落としているとき
        public int GuildBattleAdditionalSpotDamageCoefficientTwoSpot { get; set; } = 0;

        // PJFB不使用
        public int GuildBattleAdditionalSpotDamageCoefficientPerTotalMilitaryStrength { get; set; } = 0;
        // PJFB不使用
        public int GuildBattleMaxAdditionalSpotDamageCoefficientPerTotalMilitaryStrength { get; set; } = 0;
        
        #endregion

        #region 兵力系

        // 開始時ボール数
        public int GuildBattleInitialMilitaryStrength { get; set; } = 5;
        // ボール回復必要ターン
        public int GuildBattleRequiredTurnToRecoveryMilitaryStrength { get; set; } = 5;
        // ターン経過時ボール回復量
        public int GuildBattleRecoveryMilitaryStrengthPerTurn { get; set; } = 1;
        // 拠点破壊時回復ボール量
        public int GuildBattleAdditionalMilitaryStrengthPerSpotBroken { get; set; } = 3;

        // 敗北時復活基礎ターン
        public int GuildBattleRevivalTurn { get; set; } = 20;
        // 敗北時復活追加ターン(敗北毎)
        public int GuildBattleRevivalTurnPenaltyPerBeaten { get; set; } = 3;
        // 連勝ペナルティ開始連勝数
        public int GuildBattleStatusPenaltyByWinStreakCountStartAt { get; set; } = 1;
        // 連勝ペナルティ係数(開始連勝数以降1勝毎)
        public float GuildBattleStatusPenaltyPerWinStreak { get; set; } = 0.05f;
        // 連勝ペナルティ最大係数
        public float GuildBattleMaxStatusPenaltyByWinStreak { get; set; } = 0.5f;
        // NPC防衛設定ボール数
        public int GuildBattleNPCPartyDefaultMilitaryStrength { get; set; } = 3;
        // 設定可能最大ボール数
        public int[] GuildBattleMilitaryStrengthCaps { get; set; } = new[] { 5 };

        #endregion

        #region 移動系

        // 変えるな危険
        public int GuildBattleMovementValue { get; set; } = 100;

        #endregion
                
        #region 勝利点関連

        // 本拠地勝利点
        public int GuildBattleWinningPointOccupyBaseSpot { get; set; } = 2500;
        // 衛星拠点勝利点
        public int GuildBattleWinningPointOccupySpot { get; set; } = 1000;
        // プレイヤー接続時勝利点
        public int GuildBattleWinningPointJoinPlayer { get; set; } = 30;
        // マッチアップ勝利毎の勝利点
        public int GuildBattleWinningPointPerWinBattle { get; set; } = 5;
        // マッチアップ内でのスコアによる勝利点
        public int GuildBattleWinningPointPerInBattlePoint { get; set; } = 1;
        
        // 時間経過評価点最大値
        public int GuildBattleMaxWinningPointFromTime { get; set; } = 600;
        // 時間経過評価点減算値(1分毎)
        public int GuildBattleMinusWinningPointPerMin { get; set; } = 20;
        
        // 勝利時評価点
        public int GuildBattleWinningPointWin { get; set; } = 10000;
        
        #endregion
        
        #region アイテム関連

        // アイテム使用時回復ボール数
        public int GuildBattleRecoveryValueOnUseItem { get; set; } = 3;
        // アイテム使用時クールタイム
        public int GuildBattleItemCoolDown { get; set; } = 20;

        #endregion

    }
}