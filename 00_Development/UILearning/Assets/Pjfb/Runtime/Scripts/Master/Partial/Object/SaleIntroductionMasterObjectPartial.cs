
namespace Pjfb.Master {
	
	public enum SaleIntroductionDisplayType
	{
		/// <summary> ホーム </summary>
		HomeTop = 1,
		/// <summary> ガチャ </summary>
		GachaTop = 2,
		/// <summary> ランクマ </summary>
		ColosseumTop = 3,
		/// <summary> ライバルリートップ </summary>
		Rivalry = 4,
		/// <summary> ライバルリーチーム編成 </summary>
		RivalryTeamSelect = 5,
		/// <summary> ライバルリー敗北 </summary>
		RivalryLose = 6,
		/// <summary> キャラ上限解放の演出後 </summary>
		CharacterLiberationEffected = 7,
	}
	
	public partial class SaleIntroductionMasterObject : SaleIntroductionMasterObjectBase, IMasterObject {
		public SaleIntroductionDisplayType DisplayPageType => (SaleIntroductionDisplayType)displayType;
	}

}
