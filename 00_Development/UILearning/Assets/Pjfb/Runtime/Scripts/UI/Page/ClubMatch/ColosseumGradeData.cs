using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Colosseum
{
    public class ColosseumGradeData
    {
        /// <summary></summary>
        private Dictionary<long, Dictionary<long, ColosseumGroupGrade>> groupGradeList = new Dictionary<long, Dictionary<long, ColosseumGroupGrade>>();
        
        public void Clear()
        {
            groupGradeList.Clear();
        }
        
        public void AddDeta(long mColosseumGradeGroupId, long gMasterId, ColosseumGroupGrade data)
        {
            if(groupGradeList.ContainsKey(mColosseumGradeGroupId) && groupGradeList[mColosseumGradeGroupId].ContainsKey(gMasterId))
            {
                // 重複している場合はエラーログ出しておく
                CruFramework.Logger.LogError($"ColosseumGroupGradeが重複してます。\nmColosseumGradeGroupId:{mColosseumGradeGroupId}, gMasterId:{gMasterId}");
                return;
            }
            
            // mColosseumGradeGroupId追加
            if(!groupGradeList.ContainsKey(mColosseumGradeGroupId))
            {
                groupGradeList.Add(mColosseumGradeGroupId, new Dictionary<long, ColosseumGroupGrade>());
            }
            
            // gMasterId追加
            if(!groupGradeList[mColosseumGradeGroupId].ContainsKey(gMasterId))
            {
                groupGradeList[mColosseumGradeGroupId].Add(gMasterId, data);    
            }
        }

        /// <summary>ランクポイント取得</summary>
        public long GetRankPoint(long mColosseumGradeGroupId, long gMasterId)
        {
            if(!groupGradeList.ContainsKey(mColosseumGradeGroupId)) return 0;
            if(!groupGradeList[mColosseumGradeGroupId].ContainsKey(gMasterId)) return 0;
            return groupGradeList[mColosseumGradeGroupId][gMasterId].rankPoint;
        }
        
        /// <summary>ランク取得</summary>
        public long GetGradeNumber(long mColosseumGradeGroupId, long gMasterId)
        {
            if(!groupGradeList.ContainsKey(mColosseumGradeGroupId)) return ColosseumManager.MinGradeNumber;
            if(!groupGradeList[mColosseumGradeGroupId].ContainsKey(gMasterId)) return ColosseumManager.MinGradeNumber;
            return groupGradeList[mColosseumGradeGroupId][gMasterId].gradeNumber;
        }
        
        /// <summary>次のランクまでに必要なランクポイント</summary>
        public long GetNextRankPoint(long mColosseumGradeGroupId, long gMasterId)
        {
            // 1ランクごとの最大ランクポイント
            long maxRankPointPerGradeNumber = ColosseumManager.MaxRankPointPerGradeNumber;
            // 現在のランクポイント
            long rankPoint = GetRankPoint(mColosseumGradeGroupId, gMasterId);
            // ランク
            long gradeNumber = GetGradeNumber(mColosseumGradeGroupId, gMasterId);
        
            long maxGradeNumber = MasterManager.Instance.colosseumGradeMaster.values
                // 対象のgradeGroupId
                .Where(m => m.mColosseumGradeGroupId == mColosseumGradeGroupId)
                // 最大値
                .Max(m => m.gradeNumber);

            // 最大ランクに到達してる
            if(gradeNumber >= maxGradeNumber)
            {
                return -1;
            }
            
            // 100ポイントごとにランクアップらしい
            return maxRankPointPerGradeNumber - (rankPoint % maxRankPointPerGradeNumber);
        }
        
        
    }
}