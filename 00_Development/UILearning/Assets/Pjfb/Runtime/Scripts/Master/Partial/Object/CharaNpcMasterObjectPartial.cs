
namespace Pjfb.Master {
	public partial class CharaNpcMasterObject : CharaNpcMasterObjectBase, IMasterObject {  
	
		private SkillData[] skillDataList = null;

		public SkillData[] SkillDataList
		{
			get
			{
				if(skillDataList == null)
				{
					skillDataList = new SkillData[abilityList.Length];
					for(int i=0;i<skillDataList.Length;i++)
					{
						skillDataList[i] = new SkillData(abilityList[i].l[0], abilityList[i].l[1] );
					}
				}
				
				return skillDataList;
			}
		}
		
		public CharacterStatus GetStatus()
		{
			CharacterStatus result = new CharacterStatus();
			
			result.Speed = spd;
			result.Technique = tec;
			result.Stamina = param1;
			result.Physical = param2;
			result.Kick = param4;
			result.Intelligence = param5;
			
			return result;
		}
		
		///////// 各ステータスをラップ
		public new BigValue hp{get{return new BigValue(base.hp);}}
		public new BigValue mp{get{return new BigValue(base.mp);}}
		public new BigValue atk{get{return new BigValue(base.atk);}}
		public new BigValue def{get{return new BigValue(base.def);}}
		public new BigValue spd{get{return new BigValue(base.spd);}}
		public new BigValue tec{get{return new BigValue(base.tec);}}
		public new BigValue param1{get{return new BigValue(base.param1);}}
		public new BigValue param2{get{return new BigValue(base.param2);}}
		public new BigValue param3{get{return new BigValue(base.param3);}}
		public new BigValue param4{get{return new BigValue(base.param4);}}
		public new BigValue param5{get{return new BigValue(base.param5);}}
		
	}
}
