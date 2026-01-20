
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Pjfb.Master {
	public partial class TrainingPointStatusEffectMasterObject : TrainingPointStatusEffectMasterObjectBase, IMasterObject {  
		
		/// <summary>表示用の説明文を取得</summary>
		public string GetDisplayDescription(StringUtility.OmissionTextData omissionTextData = null)
		{
			// 説明がない場合は中身のスキルを表示
			if(string.IsNullOrEmpty(description))
			{
				// スキル取得
				List<PracticeSkillInfo> skillList = PracticeSkillUtility.GetTrainingPointStatusEffectPracticeSkill(id);
				StringBuilder sb = new StringBuilder();
				foreach(PracticeSkillInfo skill in skillList)
				{
					// そのまま表示
					if(displayFlg)
					{
						sb.AppendLine(skill.GetNameWithValue());
					}
					// 効果値を隠して表示
					else
					{
						// 効果値を取得
						string value = skill.ToValueName(omissionTextData);
						//効果値がない場合は名前だけ表示
						if(string.IsNullOrEmpty(value))
						{
							sb.AppendLine(skill.GetName());
						}
						else
						{
							// 数字の部分を？に変換
							value = Regex.Replace(value, "[0-9]+", StringValueAssetLoader.Instance["common.question"]);
							sb.AppendLine($"{skill.GetName()} {value}");
						}
					}
				}
				return sb.ToString();
			}
			else
			{
				return description;
			}
		}
		
	}

}
