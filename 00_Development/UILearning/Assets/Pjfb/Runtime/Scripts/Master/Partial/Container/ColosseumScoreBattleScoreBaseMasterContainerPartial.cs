
using System.Linq;

namespace Pjfb.Master {

    public partial class ColosseumScoreBattleScoreBaseMasterContainer : MasterContainerBase<ColosseumScoreBattleScoreBaseMasterObject> 
    {
        long GetDefaultKey(ColosseumScoreBattleScoreBaseMasterObject masterObject){
            return masterObject.id;
        }

        public long GetBattleScoreBase(long rank,int type)
        {
            var valueObj = values.FirstOrDefault(value =>
                value.type == type && value.rankTop <= rank && value.rankBottom >= rank);
            return valueObj?.value ?? 0;
        }
    }
}
