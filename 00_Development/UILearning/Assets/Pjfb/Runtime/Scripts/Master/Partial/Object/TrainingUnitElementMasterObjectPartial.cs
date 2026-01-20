
namespace Pjfb.Master {
	
	public enum TrainingUnitElementIdType
	{
		ParentId = 1,
		MCharId = 2,
	}
	
	public partial class TrainingUnitElementMasterObject : TrainingUnitElementMasterObjectBase, IMasterObject {  
		
		new public TrainingUnitElementIdType type{get{return (TrainingUnitElementIdType)base.type;}}
		
		public string CharacterName
		{
			get
			{
				switch(type)
				{
					case TrainingUnitElementIdType.ParentId:
						return MasterManager.Instance.charaParentMaster.FindDataByMCharId(masterId).name;
					case TrainingUnitElementIdType.MCharId:
					{
						// 固有キャラは固有名称で返す
						CharaMasterObject charaMaster = MasterManager.Instance.charaMaster.FindData(masterId);
						return string.Format(StringValueAssetLoader.Instance["common.character_name"], charaMaster.nickname, charaMaster.name);
					}
				}
				
				return string.Empty;
			}
		}
		
		public bool IsMatchCharacter(long mCharId)
		{
			switch(type)
			{
				case TrainingUnitElementIdType.ParentId:
					return MasterManager.Instance.charaParentMaster.FindDataByMCharId(masterId).parentMCharaId == MasterManager.Instance.charaMaster.FindData(mCharId).parentMCharaId;
				case TrainingUnitElementIdType.MCharId:
					return masterId == mCharId;
			}
			
			return false;
		}
	}

}
