using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;
using Pjfb.UserData;


namespace Pjfb.Gacha
{
    public class GachaSettingData
    {

        private long gachaSettingId = 0;
        /// <summary>m_gacha_setting.Id</summary>
        public long GachaSettingId { get {return gachaSettingId; } }
        
        private GachaType gachaType = GachaType.Normal;
        /// <summary>ガチャ種別</summary>
        public GachaType GachaType { get { return gachaType; } }
        
        private string name = string.Empty;
        /// <summary>ガチャ名称</summary>
        public string Name { get { return name; } }
        
        private DateTime endAt = DateTime.MaxValue;
        /// <summary>終了時間</summary>
        public DateTime EndAt { get { return endAt; } }
        
        /// <summary>無期限ガチャかどうか</summary>
        public bool IsIndefinitePeriod { get { return GachaUtility.IsIndefinitePeriod(endAt);} }
        
        private long designNumber = 0;
        /// <summary>デザイン番号</summary>
        public long DesignNumber { get { return designNumber; } }
        
        private GachaChoiceData choiceData = null;
        /// <summary>ピックアップデータ</summary>
        public GachaChoiceData ChoiceData { get { return choiceData; } }

        private long exchangeStoreId = 0;
        /// <summary>交換所ショップId</summary>
        public long ExchangeStoreId { get { return exchangeStoreId; } }

        private long exchangePointId = 0;
        /// <summary>交換所ポイントId</summary>
        public long ExchangePointId { get { return exchangePointId; } }

        private bool hasPoint = false;
        /// <summary>必要なポイントを所持しているか</summary>
        public bool HasPoint { get { return hasPoint; } }


        private string detailUrl = "";
        /// <summary>詳細を押した際に展開されるお知らせがどれかを指定</summary>
        public string DetailUrl { get { return detailUrl; } }

        // 活性状態かどうか
        private bool enabled = false;

        // ユーザーの実行回数上限
        private long executeLimitPersonal = 0;
        public long ExecuteLimitPersonal { get { return executeLimitPersonal; } }

        // ユーザーの実行回数
        private long executeCount = 0;
        public long ExecuteCount { get { return executeCount; } }
        // 無効化区分。表示した上でガチャを使用不能にする必要がある場合に用いる（1 => 無効ではない、2 => カミングスーン）
        private long disableType = 0; 
        public long DisableType { get { return disableType; } }


        // 抽選方式（1 => 通常抽選方式, 2 => ボックス抽選方式）
        private long lotteryType = 0; 
        public long LotteryType { get { return lotteryType; } }

        // ボックスガチャの場合のみ、現在のボックスに残っているアイテムの合計数が入る（ボックスガチャでない場合はレスポンスから-1が入る）
        private long boxContentCount = 0;
        public long BoxContentCount { get { return boxContentCount; } }
        
        private bool canResetBoxGacha = false;
        public bool CanResetBoxGacha { get { return canResetBoxGacha; } }


        /// <summary>Boxガチャか</summary>
        public bool IsBoxGacha { get { return lotteryType == 2; } }

        
        
        /// <summary>ガチャが有効かどうか</summary>
        public bool IsEnable
        {
            get
            {
                // 非活性状態
                if(!enabled) return false;
                //カミングスーンのため弾けない
                if(disableType == (long)GachaDisableType.ComingSoon) return false;
                // 実行回数の上限設定がされている
                if(executeLimitPersonal > 0)
                {
                    // 実行回数が上限に達してる
                    if(executeCount >= executeLimitPersonal) return false;
                }
                // 有効
                return true;
            }
        }

        /// <summary>ピックアップ選択済みか</summary>
        public bool IsPickUpSelected
        {
            get
            {
                if( choiceData == null ) {
                    return true;
                }
                
                return choiceData.IsSelected;
            } 
        }


        private GachaCategoryData[] categoryDatas = new GachaCategoryData[2];
        /// <summary>ガチャ実行データ</summary>
        public GachaCategoryData[] CategoryDatas { get { return categoryDatas; } }
        
        /// <summary>単発ガチャ</summary>
        public GachaCategoryData SingleGachaData { get { return categoryDatas[(int)GachaCategory.Single]; } }
        
        /// <summary>複数ガチャ</summary>
        public GachaCategoryData MultiGachaData { get { return categoryDatas[(int)GachaCategory.Multi]; } }
        
        /// <summary>種別ごとのインデックス</summary>
        public int Index { get; set; }
        

        public GachaSettingData(TopSetting topSetting)
        {
            // m_gacha_setting.id
            gachaSettingId = topSetting.id;
            // ガチャ種別
            gachaType = (GachaType)topSetting.type;
            // ガチャ名称
            name = topSetting.name;
            // 終了時間
            endAt = DateTime.Parse(topSetting.endAt);
            // デザイン番号
            designNumber = topSetting.designNumber;
            // ピックアップ
            if(topSetting.choice != null && topSetting.choice.id > 0 )
            {
                choiceData = new GachaChoiceData(topSetting.choice);
            }
            // 活性状態か
            enabled = topSetting.enabled >= 1;
            disableType = topSetting.disableType;
            // ユーザーの実行回数上限
            executeLimitPersonal = topSetting.executeLimitPersonal;
            // ユーザーの実行回数
            executeCount = topSetting.executeCount;

            //Boxガチャ
            lotteryType = topSetting.lotteryType;
            boxContentCount = topSetting.boxContentCount;
            canResetBoxGacha = topSetting.canResetBox;

            // 単発ガチャデータ
            if(topSetting.categorySingle != null)
            {
                categoryDatas[(int)GachaCategory.Single] = new GachaCategoryData(this, topSetting.categorySingle);
            }

            // 複数ガチャデータ
            if( IsBoxGacha ) {
                long pointVal = 0;
                long drawCount = 0;
                var category = topSetting.categorySingle;
                if( category != null && category.id > 0 ) 
                {
                    pointVal = GachaUtility.FetchPointValue(category.mPointId); 
                    drawCount = GachaUtility.CalcMultiDrawCount(pointVal, category.value, GachaUtility.drawMax < boxContentCount ? GachaUtility.drawMax : boxContentCount ); 
                } 
                categoryDatas[(int)GachaCategory.Multi] = new GachaCategoryData(this, category, category.value * drawCount, drawCount);

            } else if( gachaType == GachaType.Ticket || IsBoxGacha ) {
                //チケットの場合は所持ポイントによって引く回数を変更する
                long pointVal = 0;
                long drawCount = 0;
                if( topSetting.categoryMulti != null && topSetting.categoryMulti.id > 0 ) 
                {
                    pointVal = GachaUtility.FetchPointValue(topSetting.categoryMulti.mPointId); 
                    drawCount = GachaUtility.CalcMultiDrawCount(pointVal,topSetting.categoryMulti.value, GachaUtility.drawMax); 
                } 
                
                categoryDatas[(int)GachaCategory.Multi] = new GachaCategoryData(this, topSetting.categoryMulti, topSetting.categoryMulti.value * drawCount, drawCount);
            }
            else
            {
                if(topSetting.categoryMulti != null)
                {
                    categoryDatas[(int)GachaCategory.Multi] = new GachaCategoryData(this, topSetting.categoryMulti);
                }
            }
            

            exchangeStoreId = topSetting.mCommonStoreCategoryId;
            exchangePointId = topSetting.storeMPointId;
            detailUrl = topSetting.detailUrl;
            hasPoint = CheckHasPoint(categoryDatas);
        }


        /// <summary>有効状態の更新</summary>
        public void UpdateEnable( long enable )
        {
            this.enabled = enable >= 1;
        }


        /// <summary>実行回数と最大回数を更新</summary>
        public void UpdateExecuteCount( long count, long limit )
        {
            executeCount = count;
            executeLimitPersonal = limit;
        }

        /// <summary>Box内の残りアイテム数を更新</summary>
        public void UpdateBoxContentCount( long count )
        {
            boxContentCount = count;
        }

        /// <summary>Boxガチャのリセット可能かどうかを更新</summary>
        public void UpdateCanResetBoxGacha( bool canReset )
        {
            canResetBoxGacha = canReset;
        }

        /// <summary>Boxガチャを一度に複数回引く場合の引く数と消費量を更新</summary>
        public void UpdateBoxGachaMultiData( ){
            
            // 所持数取得
            var pointVal = GachaUtility.FetchPointValue(SingleGachaData.PointId); 
            // 所持数から一度に引ける数を計算（最大数を超える場合は最大数まで）
            long drawCount = GachaUtility.CalcMultiDrawCount(pointVal, SingleGachaData.PointValue, GachaUtility.drawMax < boxContentCount ? GachaUtility.drawMax : boxContentCount );
            // 消費量計算
            long pointValue = SingleGachaData.PointValue * drawCount;
            
            MultiGachaData.UpdateGachaCount(drawCount);
            MultiGachaData.UpdatePointValue(pointValue);
        }


        

         /// <summary>ガチャ実行に必要なポイントを所持しているか</summary>
        private bool CheckHasPoint(GachaCategoryData[] categoryDatas)
        {
            foreach (GachaCategoryData categoryData in categoryDatas)
            {
                // ガチャ実行データがない
                if(categoryData == null) continue;
                // ユーザー情報に存在しない
                if(!UserDataManager.Instance.point.data.ContainsKey(categoryData.PointId)) return false;
                // 所持しているかどうか
                return UserDataManager.Instance.point.Find(categoryData.PointId).value > 0;
            }
            
            // ガチャ実行データが一つも存在しない場合はfalseを返しておく
            return false;
        }

    }
}
