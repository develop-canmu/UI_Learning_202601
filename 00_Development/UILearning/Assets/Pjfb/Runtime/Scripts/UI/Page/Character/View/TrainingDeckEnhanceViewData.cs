using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Unity.VisualScripting;

namespace Pjfb
{
    // マージ後のバフデータ情報
    public class BuffIconData
    {
        // バフアイコンのId
        private long buffIconId;
        public long BuffIconId {get => buffIconId;}
        
        // 効果値(実数)
        private long realValue = 0;
        public long RealValue {get => realValue;}
        
        // 効果値実数で新たにバフを獲得しているか
        private bool isNewAcquisitionReal = false;
        public bool IsNewAcquisitionReal{get => isNewAcquisitionReal;}
        
        // 効果値(%)
        private long percentValue = 0;
        
        public long PercentValue => percentValue;

        // 効果値%で新たにバフを獲得しているか
        private bool isNewAcquisitionPercent = false;
        public bool IsNewAcquisitionPercent{get => isNewAcquisitionPercent;}
        
        //// <summary> 実数部と小数部でそれぞれ計算して文字列を連結して返す </summary>
        private string GetPercentString()
        {
            // 整数部分の表記
            string integerString = (percentValue / 100).ToString();
            // 小数部分の表記
            string decimalString = ".";
            
            // 小数部の計算をする(小数第2位までの小数を取得)
            int decimalValue = (int)percentValue % 100;
            
            // 小数第1位
            int fistDecimalValue = decimalValue / 10;
            // 小数第2位
            int secondDecimalValue = decimalValue % 10;
            
            // 小数第２位が0の場合は表示しない(0.50などの表記になってしまうので)
            decimalString += secondDecimalValue == 0 ? fistDecimalValue.ToString() : fistDecimalValue.ToString() + secondDecimalValue.ToString();
            
            // 小数が存在するなら小数部分をつける、ないなら整数部分のみ
            return decimalValue > 0 ? integerString + decimalString : integerString;
        }
        
        // 効果値%を末尾に追加して返す
        public string PercentValueString => $"{GetPercentString()}{StringValueAssetLoader.Instance["common.percent"]}";

        public BuffIconData(long buffIconId, TrainingDeckEnhanceInfo enhanceInfo)
        {
            this.buffIconId = buffIconId;
            realValue = enhanceInfo.RealValue;
            percentValue = enhanceInfo.PercentValue;
            isNewAcquisitionReal = enhanceInfo.IsNewAcquisitionReal;
            isNewAcquisitionPercent = enhanceInfo.IsNewAcquisitionPercent;
        }
    }
    
    // バフのカテゴリデータ
    public class BuffCategoryData
    {
        // カテゴリId
        private long categoryId;
        public long CategoryId {get => categoryId;}

        // バフ対象がスキルか
        private bool isStatus;
        public bool IsStatus{get => isStatus;}
        
        private List<BuffIconData> buffIconDataList;
        public List<BuffIconData> BuffIconDataList{get => buffIconDataList;}

        public BuffCategoryData(long buffCategoryId, bool isStatus, List<TrainingDeckEnhanceInfo> enhanceList)
        {
            categoryId = buffCategoryId;
            this.isStatus = isStatus;
            buffIconDataList = new List<BuffIconData>();
            var groupList = enhanceList.GroupBy(x => x.BuffIconId);

            foreach (IGrouping<long, TrainingDeckEnhanceInfo> group in groupList)
            {
                List<TrainingDeckEnhanceInfo> groupEnhanceList = group.ToList();
                // Listが１以上ならマージがうまくできてないのでエラーを出す
                if (groupEnhanceList.Count > 1)
                {
                    CruFramework.Logger.LogError($"Not only BuffIconData");
                }
                
                // マージができていればリストは1つだけ
                BuffIconData buffCategoryData = new BuffIconData(group.Key, groupEnhanceList.FirstOrDefault());
                buffIconDataList.Add(buffCategoryData);
            }
        }
        
        //// <summary> バフの表示カテゴリ名を取得 </summary>
        public string GetBuffCategoryName()
        {
            return MasterManager.Instance.trainingStatusTypeDetailCategoryMaster.FindData(categoryId).name;
        }
    }

    // バフの対象データ
    public class BuffTargetData
    {
        // デッキ種別
        private TrainingDeckSlotType deckType;
        public TrainingDeckSlotType DeckType{get => deckType;}

        // 枠番号
        private long deckSlotIndex;
        public long DeckSlotIndex{get => deckSlotIndex;}
        
        // 発生するレベル
        private long level;
        public long Level{get => level;}
        
        private List<BuffCategoryData> buffCategoryDataList;
        public List<BuffCategoryData> BuffCategoryDataList {get => buffCategoryDataList;}

        public BuffTargetData(TrainingDeckSlotType deckType, long deckSlotIndex, long level, List<TrainingDeckEnhanceInfo> enhanceList)
        {
            this.deckType = deckType;
            this.deckSlotIndex = deckSlotIndex;
            this.level = level;
            buffCategoryDataList = new List<BuffCategoryData>();

            // 指定したレベルでフィルタしてカテゴリ毎にグループ化
            foreach (IGrouping<long, TrainingDeckEnhanceInfo> categoryGroup in enhanceList.GroupBy(x => x.BuffCategoryId))
            {
                // スキルかどうかで分ける(アイコンで判断)　ステータスならtrue,スキルならfalse
                foreach (IGrouping<bool, TrainingDeckEnhanceInfo> skillGroup in categoryGroup.GroupBy(x => x.BuffIconId != TrainingDeckEnhanceUtility.EnhanceTypeSkillBuffIconId))
                {
                   
                    BuffCategoryData buffDeckSlotData = new BuffCategoryData(categoryGroup.Key, skillGroup.Key, skillGroup.ToList());
                    buffCategoryDataList.Add(buffDeckSlotData);
                }
            }
        }
    }
}