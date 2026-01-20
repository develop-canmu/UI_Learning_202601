
namespace Pjfb.Master {
	public partial class ProfilePartMasterObject : ProfilePartMasterObjectBase, IMasterObject {  
		
        public enum ProfilePartType
        {
            /// <summary>プロフィールボイス</summary>
            ProfileVoice = 1,
            /// <summary>トレーナーカード着せ替え</summary>
            ProfileFrame = 2,
            /// <summary>表示選手</summary>
            DisplayCharacter = 3,
            /// <summary>表示選手背景</summary>
            DisplayCharacterBackground = 4,
            /// <summary>バッチ</summary>
            Emblem = 5,
        }
        
        public ProfilePartType partType => (ProfilePartType)type;
	}

}
