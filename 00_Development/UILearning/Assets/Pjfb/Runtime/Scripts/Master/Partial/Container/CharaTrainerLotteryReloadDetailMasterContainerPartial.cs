
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public partial class CharaTrainerLotteryReloadDetailMasterContainer : MasterContainerBase<CharaTrainerLotteryReloadDetailMasterObject> {
        long GetDefaultKey(CharaTrainerLotteryReloadDetailMasterObject masterObject){
            return masterObject.id;
        }
        
        public List<CharaTrainerLotteryReloadDetailMasterObject> FindDetailGroupId(long mCharaTrainerLotteryDetailGroupId)
        {
            return values.Where(data => data.mCharaTrainerLotteryDetailGroupId == mCharaTrainerLotteryDetailGroupId)
                .ToList();
        }
    }
}
