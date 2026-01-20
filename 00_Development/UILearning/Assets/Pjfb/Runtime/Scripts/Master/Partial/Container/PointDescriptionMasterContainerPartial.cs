
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {

    public partial class PointDescriptionMasterContainer : MasterContainerBase<PointDescriptionMasterObject> {
        private Dictionary<long,PointDescriptionMasterObject> dic = new Dictionary<long, PointDescriptionMasterObject>();
        
        long GetDefaultKey(PointDescriptionMasterObject masterObject){
            return masterObject.id;
        }

        public PointDescriptionMasterObject FindByMPointId(long mPointId)
        {
            //cache作成
            if (dic.Count == 0)
            {
                foreach (var pointDescriptionMasterObject in values)
                {
                    dic[pointDescriptionMasterObject.mPointId] = pointDescriptionMasterObject;
                }
            }

            if (dic.ContainsKey(mPointId))
            {
                return dic[mPointId];
            }
            
            return null;
        }
    }
}
