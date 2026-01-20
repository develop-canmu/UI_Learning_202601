
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public partial class CharaTrainerLotteryReloadMasterContainer : MasterContainerBase<CharaTrainerLotteryReloadMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryReloadMasterObject masterObject){
            return masterObject.id;
        }

        public List<CharaTrainerLotteryReloadMasterObject> FindDataListByMCharaTrainerLotteryReloadGroupId(long mCharaTrainerLotteryReloadGroupId)
        {
            return values.Where(data => data.mCharaTrainerLotteryReloadGroupId == mCharaTrainerLotteryReloadGroupId)
                .ToList();
        }
    }
}
