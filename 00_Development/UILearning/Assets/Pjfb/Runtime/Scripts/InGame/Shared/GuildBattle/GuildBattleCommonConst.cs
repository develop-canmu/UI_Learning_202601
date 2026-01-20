namespace MagicOnion
{
    public static class GuildBattleCommonConst
    {
        public const string DummyRoomName = "dummy";
        
        public const int MaxReConnectCount = 3;
        public const float ConnectTimeoutSeconds = 5.0f;
        public const float WaitReceiveBattleDataTimeOutSeconds = 15.0f;
        
        public enum JoinResult
        {
            None,
            Success,
            Error,
            BattleStateError,
        }
        
        public enum GuildBattleTeamSide
        {
            Left,
            Right,
            All,
        }
        
        public enum GuildBattleWinTeam
        {
            Left = 1,
            Right = 2,
            Draw = 3,
        }
        
        public enum GuildBattleGameState
        {
            None,
            BeforeFight,
            InFight,
            Result,
        }
        
        public enum GuildBattleTurnState
        {
            None,
            Prepare,
            TurnStart,
            PreMove,
            ExecBattle,
            ExecMove,
            TurnFinish,
            BattleFinish,
        }
        
        public const int GuildBattleItemMaxUseCount = 3;
        public const int MaxWinStreakCount = 99; // 演出の都合上99でストップ
    }
}