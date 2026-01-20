using MessagePack;
using Pjfb.Networking.App.Request;

namespace MagicOnion
{
    [MessagePackObject]
    public class GuildBattleCommonItemData
    {
        #region Properties and SerializeField

        [Key(0)] public long ItemId;
        [Key(1)] public int RemainCount;
        [Key(2)] public int MaxUseCount;
        [Key(3)] public int CoolTime;
        [Key(4)] public int BringInCount;

        #endregion

        #region Fields

        #endregion

        #region Public Methods

        public void SetData(BattleV2GvgItem item)
        {
            ItemId = item.mPointId;
            RemainCount = (int)item.value;
            MaxUseCount = GuildBattleCommonConst.GuildBattleItemMaxUseCount;
            CoolTime = 0;

            BringInCount = RemainCount;
        }

        public bool IsOnCoolTime()
        {
            return CoolTime > 0;
        }
        
        public bool CanUse()
        {
            return CoolTime == 0 && RemainCount > 0 && !IsUsedMaxCount();
        }

        public bool IsUsedMaxCount()
        {
            return BringInCount - RemainCount >= MaxUseCount;
        }

        public bool IsBringInMaxCount()
        {
            return BringInCount >= MaxUseCount;
        }

        #endregion

        #region Protected and Private Methods

        #endregion
    }
}