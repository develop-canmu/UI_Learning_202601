using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using System.Linq;
using Pjfb.Master;

namespace Pjfb
{
     public class PracticeSkillLotteryInfo
    {
        private PracticeSkillInfo skillInfo;
        public  PracticeSkillInfo SkillInfo {get => skillInfo;}
        
        private CharaTrainerLotteryReloadMasterObject reloadMaster;
        public CharaTrainerLotteryReloadMasterObject ReloadMaster{get => reloadMaster;}
        
        private CharaTrainerLotteryReloadDetailMasterObject reloadDetailMaster;
        public CharaTrainerLotteryReloadDetailMasterObject ReloadDetailMaster{get => reloadDetailMaster;}

        //スキルが何枠目なのか
        private long slotNumber;
        public long SlotNumber {get => slotNumber;}

        private PracticeSkillInfo previewSkillInfo;
        public PracticeSkillInfo PreviewSkillInfo {get => previewSkillInfo;}

        private bool isResult;
        public bool IsResult {get => isResult;}

        private long[] updateNumberList;
        public long[] UpdateNumberList {get => updateNumberList;}
        
        public PracticeSkillLotteryInfo(PracticeSkillInfo skillInfo, long slotNumber, PracticeSkillInfo previewSkillInfo, CharaTrainerLotteryReloadMasterObject reloadMaster = null,bool isResult = false, long[] updateNumberList = null)
        {
            this.skillInfo = skillInfo;
            this.slotNumber = slotNumber;
            this.previewSkillInfo = previewSkillInfo; 
            this.reloadMaster = reloadMaster;
            this.isResult = isResult;
            this.updateNumberList = updateNumberList;
            
            if(this.reloadMaster != null)
            {
                // マスタ検索
                var mCharaTrainerLotteryReloadDetailList = MasterManager.Instance.charaTrainerLotteryReloadDetailMaster
                    .FindDetailGroupId(reloadMaster.mCharaTrainerLotteryDetailGroupId);

                // マスタが見つからない
                if (mCharaTrainerLotteryReloadDetailList.Count == 0)
                {
                    // テーブル抽選でDetailマスターが見つからないならエラーを出す
                    if (reloadMaster.reloadType == PracticeSkillLotteryReloadType.SelectTable)
                    {
                        CruFramework.Logger.LogError($"Not found charaTrainerLotteryReloadDetailMaster Type {reloadMaster.mCharaTrainerLotteryDetailGroupId}");
                    }
                }
                else
                {
                    reloadDetailMaster = mCharaTrainerLotteryReloadDetailList.FirstOrDefault(x => x.number == slotNumber);
                }
            }
        }
        
        // 上書きできるか
        public bool CanReloadOverwrite()
        {
            CharaTrainerLotteryStatusMasterObject mStatus = MasterManager.Instance.charaTrainerLotteryStatusMaster.FindData(SkillInfo.MasterId);
            return mStatus.isReloadOverwrite;
        }
        
        // スキルが抽選対象か
        public bool IsLotterySubject()
        {
            //　上書き不可
            if (!CanReloadOverwrite())
            {
               return false;
            }

            if (ReloadMaster == null)
            {
                return false;
            }
            
            //　テーブル抽選
            if (ReloadMaster.reloadType == PracticeSkillLotteryReloadType.SelectTable)
            {
                // Detailマスターが存在するなら抽選対象
                return reloadDetailMaster != null;
            }
            else
            {
                return true;
            }
        }

        public SupportEquipmentLotterySkillEffectType  GetLotteryEffectType()
        {
            // 抽選タイプが効果値抽選でリザルト画面のときのみ表示
            if (ReloadMaster?.reloadType == PracticeSkillLotteryReloadType.SelectValue && IsResult)
            {
                // 効果値が上昇したなら
                if (SkillInfo.Value > PreviewSkillInfo.Value)
                {
                    return SupportEquipmentLotterySkillEffectType.EffectUp;
                }
                // 効果値が減少したら
                else if (SkillInfo.Value < PreviewSkillInfo.Value)
                {
                    return SupportEquipmentLotterySkillEffectType.EffectDown;
                }
                // 効果値が変わらないなら
                else
                {
                    return SupportEquipmentLotterySkillEffectType.EffectKeep;
                }
            }
            else
            {
                return SupportEquipmentLotterySkillEffectType.None;
            }
        }
    }

  
    /// <summary> 抽選テーブル情報 </summary>
    public class PracticeSkillLotteryTableInfo
    {
        // スキル情報
        private List<PracticeSkillValueRangeInfo> skillInfos;
        public List<PracticeSkillValueRangeInfo> SkillInfos => skillInfos;
        
        // 抽選テーブル
        private CharaTrainerLotteryFrameTableMasterObject lotteryFrameTableMaster;
        public CharaTrainerLotteryFrameTableMasterObject LotteryFrameTableMaster => lotteryFrameTableMaster;
        
        public PracticeSkillLotteryTableInfo(List<PracticeSkillValueRangeInfo> skillRangeInfos, CharaTrainerLotteryFrameTableMasterObject lotteryFrameTableMaster)
        {
            this.skillInfos = skillRangeInfos;
            this.lotteryFrameTableMaster = lotteryFrameTableMaster;
        }
    }

    /// <summary> 枠ごとの抽選テーブル情報 </summary>
    public class PracticeSkillLotteryTableSlotInfo
    {
        // 抽選テーブル情報
        private List<PracticeSkillLotteryTableInfo> tableInfos;
        public List<PracticeSkillLotteryTableInfo> TableInfos => tableInfos;
        
        // 対象枠
        private long[] slotNumbers;
        public long[] SlotNumbers => slotNumbers;
        
        public PracticeSkillLotteryTableSlotInfo(List<PracticeSkillLotteryTableInfo> tableInfos, long[] slotNumbers)
        {
            this.tableInfos = tableInfos;
            this.slotNumbers = slotNumbers;
        }
    }

     
    public static class PracticeSkillLotteryUtility
    {
        /// <summary> 上書き不可のラベル表示用の練習能力取得 </summary>
        public static List<PracticeSkillLotteryInfo> GetCharaTrainerLotteryInfo(long[] mCharaTrainerLotteryStatusIds)
        {
            // 結果格納用
            List<PracticeSkillLotteryInfo> result = new List<PracticeSkillLotteryInfo>();

            //　ガチャのピックアップ欄ではサブスキルに何も入らないのでnullの場合はそのまま返す
            if (mCharaTrainerLotteryStatusIds == null || mCharaTrainerLotteryStatusIds.Length == 0)
            {
                return result;
            }
            
            // すべてのIdを取得
            for(int i = 0; i < mCharaTrainerLotteryStatusIds.Length; i++)
            {
                // スキル取得
                GetCharaTrainerLotteryInfo(mCharaTrainerLotteryStatusIds[i], i+1, result);   
            }
            
           
            
            return result;
        }
        
        /// <summary>
        /// 抽選に使われる情報をpracticeSkillとともに返す
        /// </summary>
        private static void GetCharaTrainerLotteryInfo(long mCharaTrainerLotteryStatusId, int slotNumber,List<PracticeSkillLotteryInfo> result)
        {
            CharaTrainerLotteryStatusMasterObject mStatus = MasterManager.Instance.charaTrainerLotteryStatusMaster.FindData(mCharaTrainerLotteryStatusId);
            
            // Idと効果値を取得
            for(int i=0;i<mStatus.typeList.Length;i++)
            {
                var skillInfo = new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainerLotteryStatus, mCharaTrainerLotteryStatusId, null);
                result.Add( new PracticeSkillLotteryInfo(skillInfo, slotNumber, skillInfo));
            }
        }
        
        /// <summary>抽選アイテム使用後の練習能力</summary>
        public static List<PracticeSkillLotteryInfo> GetCharaTrainerLotteryAfterStatusPracticeSkill(long[] mCharaTrainerLotteryStatusIds, CharaTrainerLotteryReloadMasterObject reloadMaster)
        {
            // 結果格納用
            List<PracticeSkillLotteryInfo> result = new List<PracticeSkillLotteryInfo>();

            for (int i = 0; i < mCharaTrainerLotteryStatusIds.Length; i++)
            {
                // スキル取得
                GetCharaTrainerLotteryAfterStatusPracticeSkill(mCharaTrainerLotteryStatusIds[i], i+1, reloadMaster, result);   
            }

            return result;
        }
        
        /// <summary>
        /// 抽選アイテムを使用しての抽選情報を取得
        /// </summary>
        private static void GetCharaTrainerLotteryAfterStatusPracticeSkill(long mCharaTrainerLotteryStatusId, int slotNumber, CharaTrainerLotteryReloadMasterObject reloadMaster, List<PracticeSkillLotteryInfo> result)
        {
            CharaTrainerLotteryStatusMasterObject mStatus = MasterManager.Instance.charaTrainerLotteryStatusMaster.FindData(mCharaTrainerLotteryStatusId);

            // Idと効果値を取得
            for(int i=0;i<mStatus.typeList.Length;i++)
            {
                var skillInfo = new PracticeSkillInfo(mStatus.typeList[i], new BigValue(mStatus.valueList[i]), PracticeSkillMasterType.CharaTrainerLotteryStatus, mCharaTrainerLotteryStatusId, null);
                result.Add(new PracticeSkillLotteryInfo(skillInfo, slotNumber, skillInfo, reloadMaster));
            }
        }
        
        /// <summary> 抽選後に変化したスキルを表示する </summary>
        public static List<PracticeSkillLotteryInfo> GetCharaTrainerLotteryResultPracticeInfo(List<PracticeSkillLotteryInfo> currentInfo, List<PracticeSkillLotteryInfo> previewSkillInfo, CharaTrainerLotteryReloadMasterObject reloadMaster, long[] updateNumberList)
        {
            // 結果格納用
            List<PracticeSkillLotteryInfo> result = new List<PracticeSkillLotteryInfo>();

            var reloadDetail = MasterManager.Instance.charaTrainerLotteryReloadDetailMaster.FindDetailGroupId(reloadMaster.mCharaTrainerLotteryDetailGroupId);
            
            for (int i = 0; i < currentInfo.Count; i++)
            {
                // スキル取得
                result.Add(new PracticeSkillLotteryInfo(currentInfo[i].SkillInfo, i+1, previewSkillInfo[i].SkillInfo, reloadMaster, true, updateNumberList));
            }

            return result;
        }

        /// <summary> 抽選テーブルのグループIdを取得 </summary>
        public static long[] GetLotteryFrameTableGroupIdList(long mCharaId)
        {
            CharaMasterObject chara = MasterManager.Instance.charaMaster.FindData(mCharaId);

            // 抽選設定がない
            if (chara.mCharaTrainerLotteryId <= 0)
            {
                return Array.Empty<long>();
            }

            CharaTrainerLotteryMasterObject lotteryMaster = MasterManager.Instance.charaTrainerLotteryMaster.FindData(chara.mCharaTrainerLotteryId);
           
            return lotteryMaster.mCharaTrainerLotteryFrameTableGroupIdList;

        }

        /// <summary> 抽選テーブル情報を取得 </summary>
        public static List<PracticeSkillLotteryTableSlotInfo> GetTrainerLotteryTableList(long mCharaId)
        {
            // 結果用
            List<PracticeSkillLotteryTableSlotInfo> result = new List<PracticeSkillLotteryTableSlotInfo>();
            Dictionary<long, List<PracticeSkillLotteryTableInfo>> cacheGroupDataList = new Dictionary<long, List<PracticeSkillLotteryTableInfo>>();
            // TableGroupIdごとのSlotリスト
            Dictionary<long, List<long>> tableGroupIdSlotList = new Dictionary<long, List<long>>();
            
            long[] tableGroupIdList = GetLotteryFrameTableGroupIdList(mCharaId);

            // TableGroupIdごとにスロット番号を分ける
            for (int i = 0; i < tableGroupIdList.Length; i++)
            {
                // まだデータがないなら追加
                if (tableGroupIdSlotList.TryGetValue(tableGroupIdList[i], out List<long> slotList) == false)
                {
                    slotList = new List<long>();
                    tableGroupIdSlotList.Add(tableGroupIdList[i], slotList);
                }
                slotList.Add(i+1);
            }

            for (int i = 0; i < tableGroupIdList.Length; i++)
            {
                // キャッシュ済みデータはすでに計算しているのでとばす
                if (cacheGroupDataList.TryGetValue(tableGroupIdList[i], out List<PracticeSkillLotteryTableInfo> cacheData))
                {
                    continue;
                }

                // キャッシュデータを作成
                cacheData = new List<PracticeSkillLotteryTableInfo>();

                // GroupIdが一致するデータを抽出
                List<CharaTrainerLotteryFrameTableMasterObject> frameTableList = new List<CharaTrainerLotteryFrameTableMasterObject>();

                foreach (var frameTable in MasterManager.Instance.charaTrainerLotteryFrameTableMaster.values)
                {
                    // GroupIdが一致
                    if (frameTable.mCharaTrainerLotteryFrameTableGroupId == tableGroupIdList[i])
                    {
                        frameTableList.Add(frameTable);
                    }
                }

                foreach (var tableMasterObject in frameTableList)
                {
                    // m_chara_trainer_lottery_statusのIdリスト
                    HashSet<long> lotteryStatusIdList = new HashSet<long>();

                    foreach (var content in MasterManager.Instance.charaTrainerLotteryContentMaster.values)
                    {
                        if (content.mCharaTrainerLotteryContentGroupId == tableMasterObject.mCharaTrainerLotteryContentGroupId)
                        {
                            lotteryStatusIdList.Add(content.mCharaTrainerLotteryStatusId);
                        }
                    }

                    // １つの抽選テーブルから獲得される可能性のあるスキルデータリスト
                    List<PracticeSkillInfo> lotteryTableSkillList = PracticeSkillUtility.GetCharaTrainerLotteryStatusPracticeSkill(lotteryStatusIdList.ToArray());
                    // 効果範囲を計算
                    List<PracticeSkillValueRangeInfo> skillRangeInfos = PracticeSkillUtility.GetPracticeSkillValueRangeList(lotteryTableSkillList);
                    cacheData.Add(new PracticeSkillLotteryTableInfo(skillRangeInfos, tableMasterObject));
                }

                // 並び替え 
                cacheData = cacheData
                    // レアリティ降順
                    .OrderByDescending(x => MasterManager.Instance.rarityMaster.FindData(x.LotteryFrameTableMaster.mRarityId).value)
                    // TableId昇順
                    .ThenBy(x => x.LotteryFrameTableMaster.id).ToList();
                
                // キャッシュリストに追加
                cacheGroupDataList.Add(tableGroupIdList[i], cacheData);
            }

            foreach (var data in cacheGroupDataList)
            {
                result.Add(new PracticeSkillLotteryTableSlotInfo(data.Value, tableGroupIdSlotList[data.Key].ToArray()));
            }
            return result;
        }
    }
}