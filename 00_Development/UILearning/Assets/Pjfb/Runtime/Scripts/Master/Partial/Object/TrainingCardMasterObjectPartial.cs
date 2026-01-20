
namespace Pjfb.Master {
	
	public enum TrainingCardGroup
	{
		Basic = 0,
		Special = 1,
		Flow = 3,
	}
	
	public partial class TrainingCardMasterObject : TrainingCardMasterObjectBase, IMasterObject {  
		
		public new TrainingCardGroup cardGroupType{get{return (TrainingCardGroup)base.cardGroupType;}}
		
		/// <summary>黒カードは試合カード</summary>
		public  bool IsBattleCard{get{return false;}}
		
	}

}
