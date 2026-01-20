
using System.Linq;
using Pjfb.UserData;

namespace Pjfb.Master {
	public partial class ProfileFrameMasterObject : ProfileFrameMasterObjectBase, IMasterObject {  
		
        /// <summary>紐づいているプロフィールパーツを持っているか</summary>
        private bool HasProfilePart => mProfilePartId != 0 && UserDataManager.Instance.mProfilePartIdList.Contains(mProfilePartId);
        
        /// <summary>プロフィール着せ替えを持っているか</summary>
        public bool IsHave => isPrimary || HasProfilePart;
	}

}
