
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public enum AlternativePointUseType
    {
        Gacha = 1,
        All = 999,
    }
    public partial class PointAlternativeMasterContainer : MasterContainerBase<PointAlternativeMasterObject> {
        long GetDefaultKey(PointAlternativeMasterObject masterObject){
            return masterObject.id;
        }

        /// <summary> mPointIdが一致するデータを全て検索 </summary>
        public IEnumerable<PointAlternativeMasterObject> FindPointAlternative(long mPointId, AlternativePointUseType useType)
        {
            // useTypeとmPointが一致し優先度が高い順にソートして返す
            return values.Where(x => IsUseTypeEqualOrAll(x,useType) && x.mPointId == mPointId).OrderByDescending(x => x.priority);
        }
        
        /// <summary> useTypeとmPointIdが一致し使用期限内のデータを全て検索 </summary>
        public IEnumerable<PointAlternativeMasterObject> FindPointAlternativeInPeriod(long mPointId, AlternativePointUseType useType)
        {
            // mPointが一致し使用期限内のデータでフィルタ
            return values.Where(x =>IsUseTypeEqualOrAll(x,useType) && x.mPointId == mPointId && AppTime.IsInPeriodLatestAPITime(x.startAt, x.endAt))
                //優先度が高い順にソート
                .OrderByDescending(x => x.priority);
        }
        // masterのUseTypeが一致するか
        private bool IsUseTypeEqualOrAll(PointAlternativeMasterObject master, AlternativePointUseType useType)
        {
            return (master.useType == (long)useType) || master.useType == (long)AlternativePointUseType.All;
        }
    }
}
