
using System.Linq;
using Pjfb.UserData;

namespace Pjfb.Master {
	public partial class EmblemMasterObject : EmblemMasterObjectBase, IMasterObject
    {
        /// <summary>プロフィールパーツを持っているか</summary>
        private bool HasProfilePart => mProfilePartId != 0 && UserDataManager.Instance.mProfilePartIdList.Contains(mProfilePartId);
        
        /// <summary>持っているか</summary>
        public bool IsHave => isPrimary || HasProfilePart;
    }

}
