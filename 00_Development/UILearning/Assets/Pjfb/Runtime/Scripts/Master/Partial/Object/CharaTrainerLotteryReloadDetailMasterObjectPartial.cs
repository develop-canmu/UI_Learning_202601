
namespace Pjfb.Master {
	
	// m_chara_trainer_lottery_reload_detailマスタのlotteryType(抽選種別 reloadTypeが4の時に使用)
	public enum PracticeSkillLotteryReloadDetailType
	{
		None = 0, // 使用しない
		SelectSkill = 1, //　練習能力の再抽選
		SelectValue = 2,　// 効果量の再抽選
	}
	public partial class CharaTrainerLotteryReloadDetailMasterObject : CharaTrainerLotteryReloadDetailMasterObjectBase, IMasterObject {

		public new PracticeSkillLotteryReloadDetailType lotteryType
		{
			get
			{
				return (PracticeSkillLotteryReloadDetailType)base.lotteryType;
			}
		}
	}

}
