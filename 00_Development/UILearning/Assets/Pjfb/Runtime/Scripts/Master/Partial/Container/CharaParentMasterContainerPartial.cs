
using System.Linq;

namespace Pjfb.Master {

    public partial class CharaParentMasterContainer : MasterContainerBase<CharaParentMasterObject> {
        long GetDefaultKey(CharaParentMasterObject masterObject){
            return masterObject.id;
        }
        
        public CharaParentMasterObject FindDataByMCharId(long id)
        {
            foreach(CharaParentMasterObject value in values)
            {
                if(value.parentMCharaId == id)return value;
            }
            return null;
        }
        
        public CharaParentMasterObject FindDataByParentMCharaId(long id)
        {
            return values.FirstOrDefault(x => x.parentMCharaId == id);
        }
        
        
        
    }
}
