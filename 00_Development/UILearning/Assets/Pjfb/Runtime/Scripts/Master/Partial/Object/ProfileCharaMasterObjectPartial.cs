
using System.Linq;
using Pjfb.UserData;

namespace Pjfb.Master {
	public partial class ProfileCharaMasterObject : ProfileCharaMasterObjectBase, IMasterObject {  
		
        /// <summary>紐づいているプロフィールパーツを持っているか</summary>
        private bool HasProfilePart => mProfilePartId != 0 && UserDataManager.Instance.mProfilePartIdList.Contains(mProfilePartId);
        
        /// <summary>紐づいているキャラクターを持っているか</summary>
        private bool HasCharacter => mCharaId != 0 && UserDataManager.Instance.chara.HaveCharacterWithMasterCharaId(mCharaId);
        
        /// <summary>選手全身図を持っているか</summary>
        public bool IsHave => isPrimary || HasProfilePart || HasCharacter;
	}

}
