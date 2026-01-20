
using System.Collections.Generic;

namespace Pjfb.Master {

    public partial class ProfileCharaMasterContainer : MasterContainerBase<ProfileCharaMasterObject> {
        long GetDefaultKey(ProfileCharaMasterObject masterObject){
            return masterObject.id;
        }
        
        /// <summary>親キャラIDからマスタ取得</summary>
        public List<ProfileCharaMasterObject> GetListByCharaParentId(long charaParent)
        {
            List<ProfileCharaMasterObject> result = new List<ProfileCharaMasterObject>();
            foreach (ProfileCharaMasterObject masterObject in values)
            {
                if (masterObject.mCharaParentId == charaParent)
                {
                    result.Add(masterObject);
                }
            }
            return result;
        }
        
        /// <summary>プロフィールパーツIDで検索</summary>
        public ProfileCharaMasterObject FindByProfilePartId(long profilePartId)
        {
            foreach (ProfileCharaMasterObject profileCharaMasterObject in values)
            {
                if (profileCharaMasterObject.mProfilePartId == profilePartId)
                {
                    return profileCharaMasterObject;
                }
            }
            return null;
        }
    }
}
