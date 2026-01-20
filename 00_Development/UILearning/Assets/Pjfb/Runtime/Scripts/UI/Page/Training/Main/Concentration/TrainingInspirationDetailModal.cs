using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using System;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingInspirationDetailModal : ModalWindow
    {
        public  class Argument
        {
            private long inspirationId = 0;
            /// <summary>インスピレーションのId</summary>
            public long InspirationId{get{return inspirationId;}}
            
            public Argument(long inspirationId)
            {
                this.inspirationId = inspirationId;
            }
        }
        
        [System.Serializable]
        private class AbilityJson
        {
            [SerializeField]
            public long id;
            [SerializeField]
            public long level;
            [SerializeField]
            public long rate;
        }
        
        [System.Serializable]
        private class AbilityJsonArray
        {
            [SerializeField]
            public AbilityJson[] abilities;
        }
        
        
        [SerializeField]
        private TrainingInspirationIcon icon = null;
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        [SerializeField]
        private TMPro.TMP_Text descriptionText = null;
        
        [SerializeField]
        private Image baseImage = null;
        
        [SerializeField]
        private Sprite[] baseSprites = null;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Argument argument = (Argument)args;
            // マスタ
            TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(argument.InspirationId);
            // アイコン
            icon.SetInspirationId(argument.InspirationId);
            // 名前
            nameText.text = mCard.name;
            // 下地をグレードに合わせる
            baseImage.sprite = baseSprites[mCard.grade-1];
            
            // 効果値を取得
            List<PracticeSkillInfo> skills = PracticeSkillUtility.GetTrainingCardInspirationPracticeSkill(argument.InspirationId);
            
            StringBuilder descriptionBuilder = new StringBuilder();

            // 説明文がないときは自動で生成
            if(string.IsNullOrEmpty(mCard.description))
            {
                for(int i=0;i<skills.Count;i++)
                {
                    descriptionBuilder.AppendLine( skills[i].GetNameWithValue() );
                }
            }
            else
            {
                string description = mCard.description;
                // 文字列構築用
                descriptionBuilder.Append(description);
                // 練習能力名置換
                ReplaceDescription(descriptionBuilder, description, "name", (id)=>
                {
                    // Index
                    int index = GetIndex(mCard.typeList, id);
                    // 練習能力名
                    return skills[index].GetName();
                });
                
                // 練習能力効果値置換
                ReplaceDescription(descriptionBuilder, description, "value", (id)=>
                {
                    // Index
                    int index = GetIndex(mCard.typeList, id);
                    // 練習能力値
                    return skills[index].ToValueName();
                });
                
                string json = mCard.getAbilityRateMap;
                
                if(string.IsNullOrEmpty(json) == false)
                {
                    json = "{\"abilities\":" + json + "}";
                    // Jsonをパース
                    AbilityJson[] abilities = JsonUtility.FromJson<AbilityJsonArray>(json).abilities;
                    
                    // 取得アビリティ置換
                    ReplaceDescription(descriptionBuilder, description, "ability_name", (id)=>
                    {
                        return MasterManager.Instance.abilityMaster.FindData(abilities[id].id).name;
                    });
                    // レベル
                    ReplaceDescription(descriptionBuilder, description, "ability_level", (id)=>
                    {
                        return abilities[id].level.ToString();
                    });
                    // レート
                    ReplaceDescription(descriptionBuilder, description, "ability_rate", (id)=>
                    {
                        return (abilities[id].rate / 100) + GetPercentString();
                    });
                }
                
                // レアレート
                ReplaceDescription(descriptionBuilder, "rare_rate", (mCard.rarePracticeEnhanceRate / 100) + GetPercentString());
            }
            
            // 表示
            descriptionText.text = descriptionBuilder.ToString();

            return base.OnPreOpen(args, token);
        }
        
        private string GetPercentString()
        {
            return StringValueAssetLoader.Instance["common.percent"];
        }
        
        private void ReplaceDescription(StringBuilder sb, string description, string replaceTarget, Func<long, string> replaceString)
        {
            // 取得アビリティ
            MatchCollection matches = Regex.Matches(description, $"{{{replaceTarget}:[0-9]+}}");
            foreach(Match value in matches)
            {
                // Id
                long id = long.Parse(Regex.Match(value.Value, "[0-9]+").Value);
                // 置換
                string str = replaceString(id);
                sb.Replace($"{{{replaceTarget}:{id}}}", str);
            }
        }
        
        private void ReplaceDescription(StringBuilder sb, string replaceTarget, string replaceString)
        {
            sb.Replace($"{{{replaceTarget}}}", replaceString);
        }
        
        private int GetIndex(long[] typeList, long id)
        {
            for(int i=0;i<typeList.Length;i++)
            {
                if(typeList[i] == id)return i;
            }
            
            return -1;
        }
    }
}