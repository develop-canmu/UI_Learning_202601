
namespace Pjfb.Master {
	
	// m_chara_trainer_lottery_reloadマスタのreloadType(再抽選方式)
	public enum PracticeSkillLotteryReloadType
	{
		All = 1, //　完全再抽選
		SelectFrame = 2,　//　枠数指定再抽選
		SelectValue = 3, // 効果量再抽選
		SelectTable = 4,　// スキルテーブル指定抽選
	}
	
	public partial class CharaTrainerLotteryReloadMasterObject : CharaTrainerLotteryReloadMasterObjectBase, IMasterObject {

		public new PracticeSkillLotteryReloadType reloadType
		{
			get
			{
				return (PracticeSkillLotteryReloadType)base.reloadType;
			}
		}
	}

}
