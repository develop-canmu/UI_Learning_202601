
using System.Collections.Generic;
using System.Linq;

namespace Pjfb.Master {
	public partial class CharaParentMasterObject : CharaParentMasterObjectBase, IMasterObject {  
		
		
		public LevelRewardPrizeMasterObject[] MLevelRewardPrize => 
			MasterManager.Instance.levelRewardPrizeMaster.values.Where(x => x.mLevelRewardId == mLevelRewardIdTrust).ToArray();

		public LevelRewardPrizeMasterObject GetMLevelRewardPrizeByLevel(long level) =>
			MLevelRewardPrize.FirstOrDefault(x => x.level == level);
		
		public Dictionary<long, EnhanceLevelMasterObject> MEnhanceLevelDictionary =>
			MasterManager.Instance.enhanceLevelMaster.values.Where(x => x.mEnhanceId == mEnhanceIdTrust).ToDictionary(o => o.level);
		
		
		public Dictionary<CharacterProfileType, string> MCharaLibraryProfileDictionary
		{
			get
			{
				if (mCharaLibraryProfileDictionary is null)
				{
					mCharaLibraryProfileDictionary = new Dictionary<CharacterProfileType, string>();
					foreach (var mCharaLibraryProfile in MasterManager.Instance.charaLibraryProfileMaster.values.Where(x => x.masterType == 1 && x.masterId == parentMCharaId))
					{
						mCharaLibraryProfileDictionary.Add((CharacterProfileType)mCharaLibraryProfile.useType, mCharaLibraryProfile.text);
					}
				}
				return mCharaLibraryProfileDictionary;
			}
		}
		private Dictionary<CharacterProfileType, string> mCharaLibraryProfileDictionary;

		public bool IsSupportEquipment => mCharaIdList.Any(x =>
			MasterManager.Instance.charaMaster.FindData(x).cardType == CardType.SupportEquipment);
	}
	
	

}
