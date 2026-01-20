using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Shop;

namespace Pjfb.Master {

    public partial class CommonStoreMasterContainer : MasterContainerBase<CommonStoreMasterObject> {
        long GetDefaultKey(CommonStoreMasterObject masterObject){
            return masterObject.id;
        }
        
        public IEnumerable<CommonStoreMasterObject> GetCommonStoreByCategory(long categoryId)
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var currentDate = AppTime.Now;
            return values.Where(store =>
                    store.mCommonStoreCategoryId == categoryId &&
                    ShopManager.IsAvailableDateTime(store.releaseDatetime, store.closedDatetime, currentDate))
                .OrderByDescending(store => store.sortPriority);
        }
    }
}
