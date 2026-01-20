
using System.Linq;
using Pjfb.Extensions;

namespace Pjfb.Master {

    public partial class HuntTimetableMasterContainer : MasterContainerBase<HuntTimetableMasterObject> {
        long GetDefaultKey(HuntTimetableMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>
        /// データ取得
        /// </summary>
        /// <returns></returns>
        public HuntTimetableMasterObject FindDataWithHuntId(long mHuntId){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.FirstOrDefault(item => item.mHuntId == mHuntId);
        }

        /// <summary>
        /// 有効なライバルリーデータ
        /// </summary>
        /// <returns></returns>
        public HuntTimetableMasterObject FindOpenRivalryDataWithHuntId(long mHuntId){
            if( _dataDictionary == null ) {
                CruFramework.Logger.LogError( "dataDictionary is null" );
                return null;
            }

            return _dataDictionary.Values.FirstOrDefault(item => 
                    item.mHuntId == mHuntId &&
                    item.type == 1 &&
                    item.startAt.TryConvertToDateTime().IsPast(AppTime.Now) &&
                    item.viewEndAt.TryConvertToDateTime().IsFuture(AppTime.Now));
        }
    }
}
