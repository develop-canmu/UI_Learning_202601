using System.Collections.Generic;

public class BattleConst
{
    public const string AllyTeamStringColorCode = "8E9FFA"; 
    public const string EnemyTeamStringColorCode = "F661B4";
    public const string AbilityColorCode = "FFC000";

    public const float AbilityActivationWaitSec = 3.0f;
    public const float BattleTime = 300; // とりあえず. 秒ではないです. 
    public const int RequiredScore = 3; 
    public const int FieldSize = 1000;
    public const int DeckSize = 5;
    
    #region 調整対象値群

    public const float AdjustableValueMatchUpParamMinRandValueMin = 0.7f; // マッチアップ乱数最低値を決定するときの最低値
    public const float AdjustableValueMatchUpParamMinRandValueMax = 0.95f; // マッチアップ乱数最低値を決定するときの最大値
    public const float AdjustableValueMatchUpParamRandValueMax = 1.1f; // マッチアップ乱数の最大値
    public const float AdjustableValueRequiredSubParamMaxValueForMatchUp = 0.7f; // マッチアップ乱数最低値を決定するときのサブステ要求値のMAX
    public const float AdjustableValueRequiredSubParamMinValueForMatchUp = 0.3f; // マッチアップ乱数最低値を決定するときのサブステ要求値のMIN
    public const float AdjustableValueShootBlockSuccessRateForMarkCharacter = 0.3f; // シュートブロック成功抽選時, 対象OFをマークしている場合のみかかる加算値
    public const float AdjustableValueRequiredMaxWiseValueForShootBlock = 2.0f; // シュートブロック判定時の視野要求値MAX
    public const float AdjustableValueRequiredMinWiseValueForShootBlock = 0.5f; // シュートブロック判定時の視野要求値MIN
    public const float AdjustableValueMaxShootBlockRate = 0.7f; // シュートブロック確率MAX
    public const float AdjustableValueMinShootBlockRate = 0.1f; // シュートブロック確率MIN
    public const float AdjustableValueTechnicCoefficientToDodgeShootBlock = 3.0f; // シュートブロックを回避するためのテクニック加味割合を決める補正値
    public const float AdjustableValueShootRandValueMax = 1.1f; // キックからシュート値を算出するときの乱数MAX
    public const float AdjustableValueShootRandValueMin = 0.9f; // キックからシュート値を算出するときの乱数MIN
    public const float AdjustableValueShootSuccessRateMax = 1.0f; // シュート成功率MAX
    public const float AdjustableValueShootSuccessRateMin = 0.0f; // シュート成功率MIN
    public const float AdjustableValueRequiredMaxValueForNicePass = 2.5f; // ナイスパス判定要求値MAX
    public const float AdjustableValueRequiredMinValueForNicePass = 0.7f; // ナイスパス判定要求値MIN
    public const float AdjustableValueMaxNicePassActiveRate = 0.7f; // ナイスパス発生率MAX
    public const float AdjustableValueMinNicePassActiveRate = 0.0f; // ナイスパス発生率MIN
    public const float AdjustableValueKickCoefficientForNicePass = 3.0f; // ナイスパス抽選するときにキックの加味割合を決める補正値
    public const float AdjustableValueRequiredMaxWiseValueForPassCut = 3.0f; // パスカット判定時の視野要求値MAX
    public const float AdjustableValueRequiredMinWiseValueForPassCut = 0.6f; // パスカット判定時の視野要求値MIN
    public const float AdjustableValueMaxPassCutRate = 0.3f; // パスカット確率MAX
    public const float AdjustableValueMinPassCutRate = 0.0f; // パスカット確率MIN
    public const float AdjustableValueTechnicCoefficientForDodgePassCut = 3.0f; // パスカットを回避するためのテクニック加味割合を決める補正値
    public const int AdjustableValueMaxMoveValueForDribble = 200; // 通常移動のMAX
    public const int AdjustableValueMinMoveValueForDribble = 100; // 通常移動のMIN
    public const int AdjustableValueMaxMoveValueForForwardPass = 200; // パス時移動のMAX
    public const int AdjustableValueMinMoveValueForForwardPass = 100; // パス時移動のMIN
    public const int AdjustableValueMaxMoveValueForBackPass = -20; // パス時移動のMAX
    public const int AdjustableValueMinMoveValueForBackPass = -70; // パス時移動のMIN
    public const int AdjustableValueMoveRangeValueForLongPass = -70; // パス時移動のMIN
    public const int AdjustableValueMinMoveDistanceForPassCut = -100; // パスカット時ボール移動量
    public const int AdjustableValueMaxMoveDistanceForPassCut = 100; // パスカット時ボール移動量
    public const int AdjustableValueMinMoveDistanceForShootBlockByPlayer = -10; // シュートブロック(プレイヤー)時ボール移動量
    public const int AdjustableValueMaxMoveDistanceForShootBlockByPlayer = 150; // シュートブロック(プレイヤー)時ボール移動量
    public const int AdjustableValueMinMoveDistanceForShootBlockByKeeper = 50; // シュートブロック(キーパー)時ボール移動量
    public const int AdjustableValueMaxMoveDistanceForShootBlockByKeeper = 150; // シュートブロック(キーパー)時ボール移動量
    public const float AdjustableValueRequiredMaxWiseValueForAbilityInvokeRate = 1.1f;
    public const float AdjustableValueRequiredMinWiseValueForAbilityInvokeRate = 0.5f;
    public const float AdjustableValueMaxAbilityInvokeRateCoefficient = 1.1f;
    public const float AdjustableValueMinAbilityInvokeRateCoefficient = 0.5f;
    public const int AdjustableValueNeedDistanceFromGoalToLongPass = 700;

    public const float AdjustableValueConsumeStaminaValueOnAttack = 0.03f; // 通常移動スタミナ
    public const float AdjustableValueRecoveryStaminaValueOnAttack = -0.02f; // 通常移動回復スタミナ
    
    public const float AdjustableValueConsumeStaminaValueForMatchUp = 0.1f; // マッチアップスタミナ
    public const float AdjustableValueConsumeStaminaValueForPass = 0.05f; // パススタミナ
    public const float AdjustableValueConsumeStaminaValueForCross = 0.05f; // クロススタミナ
    public const float AdjustableValueConsumeStaminaValueForShoot = 0.05f; // シュートスタミナ
    public const float AdjustableValueConsumeStaminaValueForShootBlock = 0.05f; // シュートブロックスタミナ
    public const float AdjustableValueConsumeStaminaValueForPassCut = 0.05f; // パスカットスタミナ
    public const float AdjustableValueConsumeStaminaValueForLooseBall = 0.05f; // セカンドボールスタミナ
    
    public const int AdjustableValueAddGameTimeKickOff = 1;
    public const int AdjustableValueAddGameTimeDribble = 5;
    public const int AdjustableValueAddGameTimeRunThrough = 5;
    public const int AdjustableValueAddGameTimePass = 2;
    public const int AdjustableValueAddGameTimeCross = 2;
    public const int AdjustableValueAddGameTimeShoot = 2;
    public const int AdjustableValueAddGameTimeLooseBall = 5;
    public const int AdjustableValueAddGameTimeBallOut = 10;
    public const int AdjustableValueAddGameTimeThrowIn = 2;
    public const int AdjustableValueAddGameTimeKeeperThrow = 2;
    public const int AdjustableValueAddGameTimeGoal = 10;

    public const float AdjustableValuePassCutCatchRate = 0.4f;
    public const float AdjustableValuePassCutLooseBallRate = 0.6f;

    public const float AdjustableValueShootBlockInterferenceTypeRequestMax = 2.0f;
    public const float AdjustableValueShootBlockInterferenceTypeRequestMin = 0.5f;
    public const float AdjustableValueShootBlockInterferenceTypeBlockedRateMax = 0.6f;
    public const float AdjustableValueShootBlockInterferenceTypeBlockedRateMin = 0.1f;
    public const float AdjustableValueShootBlockInterferenceTypeTouchRateMax = 1.0f;
    public const float AdjustableValueShootBlockInterferenceTypeTouchRateMin = 0.2f;
    
    public const float AdjustableValueAppropriateShootRangeDistanceCoefficient = 0.7f;
    public const float AdjustableValueCommandPieceTypeUpgradeRate = 0.1f;
    public const float AdjustableValueActiveCommandAppearRate = 0.4f;

    public const float AdjustableValueNonDigestPlaySpeed = 2.0f;
    
    public static readonly List<int> MatchUpGroundSpriteTypeByDistanceFromGoal = new List<int>() { 100, 250, 400, 600, 1000 };

    #endregion
    
    public static StatusParamType GetMatchUpSubParameter(StatusParamType statusParamType)
    {
        switch (statusParamType)
        {
            case StatusParamType.Speed:
                return StatusParamType.Physical;
            case StatusParamType.Physical:
                return StatusParamType.Technique;
            case StatusParamType.Technique:
                return StatusParamType.Speed;
        }
        
        return StatusParamType.None;
    }

    public enum BattleType
    {
        None,
        StoryBattle,
        TrainingBattle,
        RivalryBattle,
        VersusPlayerBattle,
        ExecLeagueMatch,
        ReplayLeagueMatch,
        MagicOnionServerBattle,
    }

    public enum TeamSide
    {
        Left,
        Right,
        TeamSizeMax,
    }

    public enum PlayerPosition
    {
        None,
        FW,
        MF,
        DF,
    }
    
    public enum PieceType
    {
        Normal,
        Yellow,
        Red,
        Gold
    }
    
    public enum MatchUpActionType
    {
        None = 0,
        Through = 1,
        Pass = 2,
        Shoot = 3,
        Cross = 4,
    }

    public enum LogActionBits
    {
        HasPass = 1 << 0,
        HasLongPass = 1 << 1,
        HasBackPass = 1 << 2,
        HasShoot = 1 << 3,
        HasCross = 1 << 4,
    }
    
    // とりあえずラップ
    public enum MatchUpActionDetailType
    {
        Type0 = 0,
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
    }

    public enum ThroughDetailType
    {
        Normal,
        Nice,
        Bad,
        TooBad,
    }
    public static int[] ThroughDetailTypeWeight =  { 65, 15, 15, 5 };
    public static int[] ThroughDetailTypeRequiredWise = { 0, 800, 0, 0 };
    public static int[] ThroughDetailTypeRequiredWiseRank = { 10, 5, 10, 10 };
    public static int[] ThroughCommandPhraseRequiredWise = { 0, 800, 700, 700 };
    public static int[] ThroughCommandPhraseRequiredWiseRank = { 10, 5, 7, 7 };

    // パスだけちょっと特殊. バックパスとフォワードパスを分けたくない but 抽選の重みは別にしたい、ので大項目は一緒にしときつつ重みだけ分ける.
    public enum PassDetailType
    {
        NormalPass,
        NormalNicePass,
        NormalBadPass,
        ForwardPass,
        ForwardNicePass,
        ForwardBadPass,
        LongPass,
    }
    public static int[] BackPassDetailTypeWeight = { 40, 30, 30, 0, 0, 0 };
    public static int[] BackPassDetailTypeRequiredWise = { 0, 500, 0, 0, 0, 0 };
    public static int[] BackPassDetailTypeRequiredWiseRank = { 10, 10, 10, 0, 0, 0 };
    public static int[] BackPassCommandPhraseRequiredWise = { 0, 0, 500 };
    public static int[] BackPassCommandPhraseRequiredWiseRank = { 10, 10, 8 };
    public static int[] ForwardPassDetailTypeWeight = { 0, 0, 0, 80, 10, 10 };
    public static int[] ForwardPassDetailTypeRequiredWise = { 0, 0, 0, 1000, 1500, 700 };
    public static int[] ForwardPassDetailTypeRequiredWiseRank = { 0, 0, 0, 8, 7, 5 };
    public static int[] ForwardPassCommandPhraseRequiredWise = { 500, 700, 1000 };
    public static int[] ForwardPassCommandPhraseRequiredWiseRank = { 8, 7, 5 };

    // クロス経由も同じ.
    public enum ShootDetailType
    {
        NormalShoot,
        NiceMeetShoot, // (余談) ジャストミートって和製英語なんですね.
        BadMeetShoot,
        NiceCourseShoot,
        BadCourseShoot,
    }
    public static int[] ShootDetailTypeWeight = { 76, 6, 6, 6, 6 };
    public static int[] ShootDetailTypeRequiredWise = { 0, 700, 0, 1500, 0 };
    public static int[] ShootDetailTypeRequiredWiseRank = { 10, 7, 10, 3, 10 };
    public static int[] InShootRangeShootCommandPhraseRequiredWise = { 0, 0, 700, 700, 1500, 1000 };
    public static int[] InShootRangeShootCommandPhraseRequiredWiseRank = { 10, 10, 7, 7, 3, 5 };
    public const int OutOfShootRangePattern1ConditionRange = 100;
    public const int OutOfShootRangePattern2ConditionRange = 250;

    public static int[] CrossDetailTypeWeight = { 76, 6, 6, 6, 6 };
    public static int[] CrossDetailTypeRequiredWise = { 0, 1000, 0, 1500, 0 };
    public static int[] CrossDetailTypeRequiredWiseRank = { 10, 5, 10, 3, 10 };
    public static int[] CrossCommandPhraseRequiredWise = { 1500, 1000, 1000, 1500, 1500 };
    public static int[] CrossCommandPhraseRequiredWiseRank = { 3, 5, 5, 3, 3 };

    // 突破, 縦パス, バックパス, シュート, ロングシュート, クロス, ロングクロス
    public static int[] DefaultStrategyCommandWeight = { 100, 30, 70, 1000, 40, 200, 20 };
    public static int[] LongRangeShootStrategyCommandWeight = { 110, 30, 70, 1000, 200, 200, 100 };
    public static int[] ShootRangeShootStrategyCommandWeight = { 150, 30, 50, 1000, 10, 200, 10 };
    public static int[] PassStrategyCommandWeight = { 100, 70, 100, 1000, 10, 400, 10 };

    public static readonly BattleConst.DeckStrategy DefaultDeckStrategy = DeckStrategy.Dribble;
    public enum DeckStrategy
    {
        None = 0,
        Aggressive, //・遠距離からでも積極的にシュート
        Dribble,    //・ドリブル突破でゴール前まで運んでシュート
        Pass,       //・パス重視でゴール前まで運んでシュート
    }

    public static readonly long[] DefaultRoleOperationIds = { 1, 1, 2, 3, 3 };
    
    public enum BattleResult{
        WinLeft = 1,
        WinRight = 2,
        Draw = 3,
    }

    public enum StatusParamType
    {
        None = 0,
        Speed = 1,
        Technique = 2,
        Physical = 3,
        Kick = 4,
        Wise = 5,
        Stamina = 6,
    }

    // 主体はOF側
    public enum MatchUpResult
    {
        None,
        Success,
        Failed,
        LooseBall,
        BallOut,
    }

    public enum BallInterferenceType
    {
        None,
        Touched, // ちょっとあたったとき
        Blocked, // がっつりあたったとき
        Catched, // とられたとき
        HitGoalPost, // ゴールポストにあたったとき
    }

    public enum LooseBallReasonType
    {
        None,
        FailedRunThrough,
        PassCut,
        ShootBlockedByPlayer,
        ShootBlockedByKeeper,
        ShootBlockedByGoalpost,
    }
    
    // Role
    public enum RoleNumberType
    {
        None = 0,
        Captain = 1,
        SubCaptain = 2,
        Adviser = 11,
        SupportAdviser = 12,    // 不要な場合は削除
    }

    // ダイジェスト発生回数による次のダイジェスト発生基礎確率
    public static readonly float[] ActivateDigestRate = { 0.7f, 0.5f, 0.4f, 0.3f, 0.2f, 0.0f };
    // キメルマン, クロッサー, 突破マン, パサー, バランサー
    public static readonly float[,] AutoResolveActionTypeWhenEnergetic =
    {
        { 0.95f, 0.05f, 0.95f, 0.05f },
        { 0.5f, 0.5f, 0.2f, 0.8f },
        { 0.8f, 0.2f, 0.7f, 0.3f },
        { 0.3f, 0.7f, 0.4f, 0.6f },
        { 0.65f, 0.35f, 0.65f, 0.35f }
    };

    public static readonly float[,] AutoResolveActionTypeWhenTired =
    {
        { 0.5f, 0.5f, 0.8f, 0.2f },
        { 0.3f, 0.7f, 0.2f, 0.8f },
        { 0.5f, 0.5f, 0.5f, 0.5f },
        { 0.2f, 0.8f, 0.2f, 0.8f },
        { 0.4f, 0.6f, 0.3f, 0.7f }
    };
    
    public const float DefaultMatchUpDFTakeBallWeight = 0.4f; // 基準となる突破マッチアップ時DFボール奪取率
    public const float DefaultMatchUpLooseBallWeight = 0.6f; // 基準となる突破マッチアップ時セカンドボール発生率
    public const float DefaultThrowInWeight = 0.2f; // 基準となるルーズボール時スローイン発生率

    public const int MaxLooseBallMatchUpMemberNum = 4; // セカンドボール争いの最大参加人数
    public const int MinDribbleMoveValue = 10; // ドリブル時に保証する最低移動量
    
    #region アビリティ関連
    
    // パッシブ評価タイミング
    public enum AbilityEvaluateTimingType
    {
        None = 0,
        OnGameStart = 1,    // ゲーム開始前 (全員)
        OnRoundStart = 2,   // 参加メンバ、マッチアップ対象の決定後 (ラウンド参加者)
        OnBeforeMatchUp = 3,// マッチアップ処理時(コマンド選択前) (マッチアップ参加者)
        OnSelectThroughOF = 4,// 突破選択時 (OF)
        OnSelectThroughDF = 5,// 突破選択時 (DF)
        OnSuccessThrough = 6,// 突破成功時 (OF)
        OnSelectPassOF = 7, // パス選択時 (OF)
        OnSelectPassDF = 8, // パス選択時 (DF)
        OnSuccessPass = 9,  // パス成功時(OF)
        OnReceivePass = 10,  // パス成功時(パス対象)
        OnSelectShootOF = 11, // シュート選択時 (OF)
        OnSelectShootDF = 12, // シュート選択時 (DF)
        OnSuccessShoot = 13, // シュート成功時(OF時限定)
        OnSelectCrossOF = 14, // クロス選択時 (OF)
        OnSelectCrossDF = 15, // クロス選択時 (DF)
        OnReceiveCross = 16, // クロス受け取り時 (クロス対象)
        
        OnBeforePassCut = 31, // パスカット処理時 (パスカット実行者)
        OnSuccessPassCut = 32, // パスカット成功時 (パスカット成功者)
        OnBeforeShootBlock = 33, // シュートブロック処理時 (シュートブロック実行者)
        OnSuccessShootBlock = 34, // シュートブロック成功時 (シュートブロック成功者)
        
        OnLooseBall = 41,    // セカンドボール処理時 (セカンドボール参加者)
        
        OnAfterAllyGoal = 51,   // 味方がゴールを決めたとき(全員)
        OnAfterEnemyGoal = 52,  // 敵がゴールを決めたとき(全員)
        
        ActiveSelectThroughOF = 101,    // 突破選択時 (OF)
        ActiveSelectThroughDF = 102,    // 突破選択時 (DF)
        ActiveSelectPassOF = 103,       // パス選択時 (OF)
        ActiveSelectPassDF = 104,       // パス選択時 (DF)
        ActiveReceivePass = 105,        // パス成功時(パス対象)
        ActiveSelectShootOF = 106,      // シュート選択時 (OF)
        ActiveSelectShootDF = 107,      // シュート選択時 (DF)
        ActiveSelectCrossOF = 108,      // クロス選択時 (OF)
        // ActiveSelectCrossDF = 109,      // クロス選択時 (DF) 使いません。(1.1.3時メモ)
        ActiveReceiveCross = 110,       // クロス受け取り時 (クロス対象)
        ActiveOnLooseBall = 111,        // セカンドボール処理時 (セカンドボール参加者)
        
        GvGAdviserManualActive = 201,           // 201.クラブ・ロワイヤル、プレイヤーが任意発動
        GvGAdviserAutoActivate = 202,           // 202.クラブ・ロワイヤル、自動発動
        GvGAdviserAddSendUnit = 203,            // 203.クラブ・ロワイヤル、ユニット追加

    }

    public enum AbilityActivationConditionType
    {
        None = 0,
        ScoreDiff = 1,                          // 敵味方スコア差(MinMax)
        AllyScore = 2,                          // 自チームスコア(MinMax)
        EnemyScore = 3,                         // 敵チームスコア(MinMax)
        SelfScoreCount = 4,                     // ゲーム内で自身がシュートを決めた回数(MinMax)
        SelfPosition = 5,                       // 自分のポジション(FW,MF,DF)
        StatusRankAtTeam = 6,                   // 自チーム内での特定ステータス順位(MinMax)
        StatusRankAtRound = 7,                  // 自チームラウンド参加者内での特定ステータス順位(MinMax)
        StatusRankAtWholeTeam = 8,              // 全体での特定ステータス順位(MinMax)
        StatusRankAtRoundWholeTeam = 9,         // ラウンド参加者全体での特定ステータス順位(MinMax)
        BallPosition = 10,                      // 現在のボール位置(MinMax)
        CurrentStamina = 11,                    // 最大スタミナに対する現在スタミナ割合(MinMax)
        AllySpecificCharacterAtTeam = 12,       // 自チームに特定の味方がいる(sameCharaId)
        EnemySpecificCharacterAtTeam = 13,      // 敵チームに特定の味方がいる(sameCharaId)
        MatchUpSpecificCharacter = 14,          // マッチアップ相手が特定の敵(sameCharaId)
        // MatchUpDefenceCount = 15,               // マッチアップ時にDFについている敵の数(MinMax) マークは一人になったので削除
        MatchUpPrimaryStatusDifference = 16,    // マッチアップ相手とのメインステの差分が範囲内か(MinMax)
        AllySpecificCharacterBallOwner = 17,    // 特定の味方が現在ボールを持っている(sameCharaId)
        EnemySpecificCharacterBallOwner = 18,   // 特定の敵が現在ボールを持っている(sameCharaId)
        BaseStatusValueMinMax = 19,             // 特定のステータスがMinMax内か(MinMax)
        BaseStatusRankAtWholeTeam = 20,         // 全体での特定のアビリティなしでのステータス順位(MinMax)
        
        ClubRoyalBallCount = 201,                // クラブ・ロワイヤル専用：ボール数(MinMax)
    }

    public enum AbilityEffectType
    {
        BuffSpeedUpAddition = 1,
        BuffTechniqueUpAddition = 2,
        BuffPhysicalUpAddition = 3,
        BuffWiseUpAddition = 4,
        BuffKickUpAddition = 5,
        BuffShootRangeUpAddition = 6,
        BuffStaminaUpAddition = 7,
        
        BuffSpeedUpMultiply = 21,
        BuffTechniqueUpMultiply = 22,
        BuffPhysicalUpMultiply = 23,
        BuffWiseUpMultiply = 24,
        BuffKickUpMultiply = 25,
        BuffShootRangeUpMultiply = 26,
        BuffStaminaUpMultiply = 27,
        
        BuffIgnoreShootRangePenalty = 51,         // シュート適正距離外のペナルティ無視
        BuffLooseBallCatchRateUp = 52,            // セカンドボール争い参加時ボール奪取率アップ
        BuffLooseBallForceParticipation = 53,     // セカンドボール争い強制参加
        BuffLooseBallForceNonParticipation = 54,     // セカンドボール争い強制不参加
        
        BuffChooseThroughRateUp = 61,
        BuffChoosePassRateUp = 62,
        BuffChooseShootRateUp = 63,
        BuffChooseCrossRateUp = 64,
        
        BuffDribbleDistanceUp = 101,        // ドリブル移動距離アップ
        BuffThroughMatchUpValueUp = 102,    // 突破時のマッチアップ力アップ
        BuffPassCutRateUp = 103,            // パスカット成功率アップ
        BuffPassCatchRateUp = 104,          // パスカット成功時、ボールキャッチ率アップ
        BuffShootSuccessRateUp = 105,       // シュート成功率アップ
        BuffShootBlockSuccessRateUp = 106,  // シュートブロック成功率アップ
        BuffShootBlockBlockRateUp = 107,    // シュートブロック成功時、キャッチ率アップ
        BuffNicePassSuccessRateUp = 108,    // ナイスパス成功率アップ
        BuffMatchUpPowerRandMinUp = 109,    // マッチアップランダム最低補正値アップ
        BuffMatchUpPowerRandMaxUp = 110,    // マッチアップランダム最高補正値アップ
        BuffRoundJoinRateUp = 111,          // ラウンド参加率アップ
        BuffPassableRateUp = 112,           // パス対象選出率アップ
        BuffPassableRateDown = 113,         // パス対象選出率ダウン
        
        GuildBattleCoolTimeTurnDecrement = 201,                     // クラブ・ロワイヤル専用：クールダウンターン短縮 実数値
        GuildBattleCoolTimeTurnDecrementMultiply = 202,             // クラブ・ロワイヤル専用：クールダウンターン短縮 割合
        GuildBattleMilitaryStrengthRecoveryPerTurnAddition = 203,   // クラブ・ロワイヤル専用：ターン経過によるボール自動回復量アップ 実数値
        GuildBattleMilitaryStrengthRecoveryPerTurnMultiply = 204,   // クラブ・ロワイヤル専用：ターン経過によるボール自動回復量アップ 割合
        GuildBattleMilitaryStrengthRecoveryAdditionOverHeal = 205,          // クラブ・ロワイヤル専用：ボール回復 実数値
        GuildBattleMilitaryStrengthRecoveryMultiplyOverHeal = 206,          // クラブ・ロワイヤル専用：ボール回復 割合
        GuildBattleMilitaryStrengthRecoveryAdditionUntilMaxHeal = 207,          // クラブ・ロワイヤル専用：ボール回復 実数値
        GuildBattleMilitaryStrengthRecoveryMultiplyUntilMaxHeal = 208,          // クラブ・ロワイヤル専用：ボール回復 割合

    }

    public enum AbilityTargetType
    {
        None = 0,
        Self = 1,
        MatchUpEnemy = 2,
        AllyTeam = 3,
        EnemyTeam = 4,
        AllyRoundTeam = 5,
        EnemyRoundTeam = 6,
        ActionTarget = 7,
        HighestStatusAllyTeam = 8,      // 自チーム内での最高ステータスを持つキャラ
        HighestStatusEnemyTeam = 9,     // 敵チーム内での最高ステータスを持つキャラ
        LowestStatusAllyTeam = 10,      // 自チーム内での最低ステータスを持つキャラ
        LowestStatusEnemyTeam = 11,     // 敵チーム内での最低ステータスを持つキャラ
        
        GuildBattleSelfSortieUnits = 201,     // クラブ・ロワイヤル専用：プレイヤーが派遣しているチーム
        GuildBattleSelfCoolTimeUnits = 202,   // クラブ・ロワイヤル専用：プレイヤーのクールダウン中のチーム 
        GuildBattlePlayer = 203,              // クラブ・ロワイヤル専用：プレイヤー
        
    }

    public enum AbilityTurnDecrementTiming
    {
        None = 0,
        OnKickOff = 1,              // ゴール後 (ボールが中央に配置されて始まるとき)
        OnMarkChanged = 2,          // マークが変わったとき (マッチアップ選択後等)
        OnBeforeMatchUp = 3,        // マッチアップ処理前
        OnOffenceSideChanged = 4,   // 攻撃側が変わったとき
        GVGTurn = 5,                // クラブ・ロワイヤルターン経過
    }

    public enum LogTiming
    {
        None = 0,
        KickOff = 1,
        OnMatchUp = 2,
        MatchUpResult = 3,
        ThrowIn = 4,
        LooseBall = 5,
        OnBattleEnd = 6,
        DoNextMatchUp = 7,
    }
    
    #endregion

    #region ダイジェスト演出関連
    
    public enum DigestTiming
    {
        Stop = -1,
        None = 0,
        KickOff,
        Dribble,
        MatchUp,
        Through,
        Cross,
        Pass,
        PassCut,
        Shoot,
        ShootBlock,
        ShootResult,
        Goal,
        GoalGameSet,
        SecondBall,
        OutBall,
        ThrowIn,
        TimeUp,
        Special,
    }
    
    public enum DigestType
    {
        Stop = -1,
        None = 0,
        KickOffL,
        KickOffR,
        DribbleL,
        DribbleR,
        MatchUp,
        TechnicMatchUpWinL,
        TechnicMatchUpWinR,
        TechnicMatchUpLoseL,
        TechnicMatchUpLoseR,
        PhysicalMatchUpWinL,
        PhysicalMatchUpWinR,
        PhysicalMatchUpLoseL,
        PhysicalMatchUpLoseR,
        SpeedMatchUpWinL,
        SpeedMatchUpWinR,
        SpeedMatchUpLoseL,
        SpeedMatchUpLoseR,
        Cross,
        PassFailed,
        PassSuccess,
        PassCutBlock,
        PassCutCatch,
        Shoot,
        ShootBlockL,
        ShootBlockR,
        ShootBlockTouchL,
        ShootBlockTouchR,
        ShootBlockNotReachL,
        ShootBlockNotReachR,
        ShootResultSuccessL,
        ShootResultSuccessR,
        ShootResultPunchL,
        ShootResultPunchR,
        ShootResultPostL,
        ShootResultPostR,
        ShootResultCatch,
        Goal,
        Goal_GameSet,
        SecondBall2,
        SecondBall3,
        SecondBall4,
        OutBall,
        ThrowIn,
        ThrowInKeeper,
        TimeUp,
        
        Special,
            
        Max,
    }

    public enum MatchUpDigestType
    {
        None = 0,
        Normal = 1,
        HideSomeWords = 2,
    }

    #endregion
    
#region クラブロワイヤルインゲーム

    public enum ClubRoyalBattleEffectType
    {
        Dribble,
        Spawn,
        DamageTaken,
        DamageTakenBLM,
        DeadCharacter,
        DeadBLM,
    } 

    public enum ClubRoyalWordEffectType
    {
        Dash,
        Spawn,
        MatchUp,
        Hit,
    } 
    
    public enum ClubRoyalBallAnimationType
    {
        Idle,
        LeftRotate,
        RightRotate,
        ShootHitBLM,
        ShootHitOpponent,
        LongShootHitOpponent,
        ShootGoal,
        Pass1,
        Pass2,
        LongPass1,
        LongPass2,
    }
    
    public const float HpColorThresholdUnder20 = 0.2f;
    public const float HpColorThresholdUnder50 = 0.5f;
    
    public enum AbilityType
    {
        None,
        Normal = 1,
        GuildBattleManual = 2,      // クラブ・ロワイヤル：任意発動
        GuildBattleAuto = 3,        // クラブ・ロワイヤル：自動発動
    }


#endregion
}
