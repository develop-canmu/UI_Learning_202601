using System;
using System.Linq;
using System.Collections.Generic;
using Pjfb.Extensions;
using Pjfb.Shop;
using Pjfb.UserData;

namespace Pjfb.Master {

    public partial class CommonStoreCategoryMasterContainer : MasterContainerBase<CommonStoreCategoryMasterObject> {
        long GetDefaultKey(CommonStoreCategoryMasterObject masterObject){
            return masterObject.id;
        }
        
        public IEnumerable<CommonStoreCategoryMasterObject> GetAvailableCommonStoreCategory()
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var currentDate = AppTime.Now;
            return values.Where(category =>
                ShopManager.IsAvailableDateTime(category.releaseDatetime, category.closedDatetime, currentDate) &&
                (category.adminTagIdList.IsNullOrEmpty() || UserDataManager.Instance.tag.Any(x => category.adminTagIdList.Contains(x))) &&
                MasterManager.Instance.commonStoreMaster.GetCommonStoreByCategory(category.id).Any()).OrderByDescending(category => category.sortPriority);
        }
        
    }
}
