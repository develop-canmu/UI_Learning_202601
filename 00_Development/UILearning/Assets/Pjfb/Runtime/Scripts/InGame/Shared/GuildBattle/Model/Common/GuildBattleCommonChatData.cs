using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonChatData
    {
        #region Properties and SerializeField

        [Key(0)]
        public int Index;
        
        [Key(1)]
        public long UserId;
        
        [Key(2)]
        public string Body;

        [Key(3)]
        public long StampId;

        [Key(4)]
        public long CreatedAt;

        [Key(5)]
        public int LogType;

        [Key(6)]
        public long SystemLogUserId1;
        [Key(7)]
        public long SystemLogUserId2;
        [Key(8)]
        public long SystemLogSpotId;
        [Key(9)]
        public int SystemLogWinStreakCount;

        // チャット欄に表示するシステムログの種類.
        // 海外版も一応考慮して, タイプの定義をした上でクライアント側で内容を表示する形にする.
        public enum SystemLogType
        {
            AllyWinStreakLog = 1, // [Player]がn連撃破!!
            EnemyWinStreakLog = 2, // [Player]がn連撃破!!
            AllyStopWinStreakLog = 3,  // [PlayerA]が[PlayerB]の連撃を阻止!
            EnemyStopWinStreakLog = 4, // [PlayerA]に[PlayerB]の連撃を阻止されました…
            AllyOccupySpotLog = 5, // [Player]が[Spot]を制圧!!
            EnemyOccupySpotLog = 6, // [Player]に[Spot]が制圧されました…
            PartyStartFighting = 7, // あなたの軍と[Player]の軍が会敵しました!
            AllyPartyBeatEnemy = 8, // [PlayerA]の軍が[PlayerB]の軍を撃破!
            ReinforcementsArrived = 9, // 援軍が到着しました!
            OnUseItem = 10, // [PlayerA]が[ItemName]を使用しました！!
            OnUseYellAbility = 11, // [PlayerA]が[AbilityName]を使用しました！
        }
        
        #endregion

        #region Fields

        #endregion

        #region Public Methods

        public GuildBattleCommonChatData()
        {
        }
        
        /// <summary>
        /// ユーザチャット用
        /// </summary>
        /// <param name="index"></param>
        /// <param name="body"></param>
        /// <param name="userId"></param>
        /// <param name="stampId"></param>
        /// <param name="createdAt"></param>
        public GuildBattleCommonChatData(int index, string body, long userId, long stampId, long createdAt)
        {
            Index = index;
            Body = body;
            UserId = userId;
            StampId = stampId;
            CreatedAt = createdAt;
        }
        
        /// <summary>
        /// システムログ用
        /// </summary>
        /// <param name="systemLogType"></param>
        /// <param name="logUserIdA"></param>
        /// <param name="logUserIdB"></param>
        /// <param name="spotId"></param>
        /// <param name="winStreakCount"></param>
        public GuildBattleCommonChatData(int index, int systemLogType, long logUserIdA = -1, long logUserIdB = -1, long spotId = -1, int winStreakCount = -1)
        {
            Index = index;
            LogType = systemLogType;
            SystemLogUserId1 = logUserIdA;
            SystemLogUserId2 = logUserIdB;
            SystemLogSpotId = spotId;
            SystemLogWinStreakCount = winStreakCount;
        }

        public bool IsSelfChatData(long userId)
        {
            return UserId == userId;
        }

        public bool IsStampChatData()
        {
            return StampId > 0;
        }

        public bool IsSystemLog()
        {
            return LogType > 0;
        }
        
        #endregion

        #region Protected and Private Methods

        #endregion
    }
}