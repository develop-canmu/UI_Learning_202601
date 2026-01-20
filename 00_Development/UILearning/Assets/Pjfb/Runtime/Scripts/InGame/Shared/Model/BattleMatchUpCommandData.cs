namespace Pjfb.InGame
{
    public class BattleMatchUpCommandData
    {
        public BattleConst.MatchUpActionType ActionType;
        public BattleConst.MatchUpActionDetailType ActionDetailType;
        public long TargetCharaId;
        public bool IsAutoChosen;
        public bool IsEnoughClose;
        public byte Index;
    }
}
