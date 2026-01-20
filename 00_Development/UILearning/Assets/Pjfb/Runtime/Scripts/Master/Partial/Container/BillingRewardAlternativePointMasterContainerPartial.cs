
using Pjfb.Extensions;

namespace Pjfb.Master {

    public partial class BillingRewardAlternativePointMasterContainer : MasterContainerBase<BillingRewardAlternativePointMasterObject> {
        long GetDefaultKey(BillingRewardAlternativePointMasterObject masterObject){
            return masterObject.id;
        }

        // 代用ポイントの取得
        public BillingRewardAlternativePointMasterObject FindDataByMBillingRewardId(long mBillingRewardId, PointMasterObject.PointType pointType = PointMasterObject.PointType.Item)
        {
            BillingRewardAlternativePointMasterObject result = null;
            foreach(BillingRewardAlternativePointMasterObject value in values)
            {
                // 課金商品IDが違う場合continue
                if(value.mBillingRewardId != mBillingRewardId)continue;
                // 交換できる期間外の場合continue
                if(value.startAt.TryConvertToDateTime().IsFuture(AppTime.Now) || value.endAt.TryConvertToDateTime().IsPast(AppTime.Now))continue;
                // 交換終了期間が保持しているものより後の場合もしくは同じ場合continue
                if (result != null &&
                    (value.endAt.TryConvertToDateTime().IsFuture(result.endAt.TryConvertToDateTime()) ||
                     value.endAt.TryConvertToDateTime().IsSame(result.endAt.TryConvertToDateTime()))) continue;
                PointMasterObject point = MasterManager.Instance.pointMaster.FindData(value.mPointId);
                // ポイントタイプが違う場合continue
                if(point.pointType != (long)pointType) continue;
                result = value;
            }

            return result;
        }
    }
}
