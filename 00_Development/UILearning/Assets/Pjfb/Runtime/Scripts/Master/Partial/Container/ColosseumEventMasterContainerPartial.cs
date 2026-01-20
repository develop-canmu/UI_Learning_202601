using Pjfb.Extensions;
namespace Pjfb.Master {

    public partial class ColosseumEventMasterContainer : MasterContainerBase<ColosseumEventMasterObject> {
        long GetDefaultKey(ColosseumEventMasterObject masterObject){
            return masterObject.id;
        }

        // サイクルが進行中のイベントを取得
        public ColosseumEventMasterObject GetAvailableColosseumMaster()
        {
            var now = AppTime.Now;
            
            foreach (var value in values)
            {
                if (value.clientHandlingType != ColosseumClientHandlingType.PvP) continue;
                var startAt = value.startAt.TryConvertToDateTime();
                var endAt = value.endAt.TryConvertToDateTime();
                if (startAt.IsPast(now) && endAt.IsFuture(now)) return value;
            }
            
            // ここでnullが帰る場合はPvP未開催
            CruFramework.Logger.LogError("期間内のm_colosseum_eventが存在しません");
            return null;
        }
        
    }
}
