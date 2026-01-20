using MessagePack;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleAbilityData
    {
        #region Properties and SerializeField

        [Key(0)] public long AbilityId;
        [Key(1)] public long MCharaId;
        [Key(2)] public int CoolTime;
        [Key(3)] public int UsableCount;
        [Key(4)] public int UsableCountMax;
        [Key(5)] public int AbilityLevel;
        [Key(6)] public int RemainTurn;

        #endregion

        #region Fields

        #endregion

        #region Public Methods

        public void SetData()
        {
            AbilityId = 0;
            CoolTime = 0;
        }

        public bool CanUse()
        {
            if (CoolTime > 0)
            {
                return false;
            }

            return UsableCount > 0;
        }

        public bool IsSameData(GuildBattleAbilityData other)
        {
            if (other == null)
            {
                return false;
            }

            return AbilityId == other.AbilityId &&
                   MCharaId == other.MCharaId &&
                   CoolTime == other.CoolTime &&
                   UsableCount == other.UsableCount &&
                   UsableCountMax == other.UsableCountMax &&
                   AbilityLevel == other.AbilityLevel &&
                   RemainTurn == other.RemainTurn;
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}