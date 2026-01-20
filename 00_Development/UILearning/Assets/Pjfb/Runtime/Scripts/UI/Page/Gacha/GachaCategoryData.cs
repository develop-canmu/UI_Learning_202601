using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaCategoryData
    {
        private GachaSettingData settingData = null;
        /// <summary>親設定</summary>
        public GachaSettingData SettingData { get { return settingData; } }
        
        private long gachaCategoryId = 0;
        /// <summary>m_gacha_category.Id</summary>
        public long GachaCategoryId { get { return gachaCategoryId; } }
        
        private long prizeCount = 0;
        /// <summary>報酬数</summary>
        public long PrizeCount { get { return prizeCount; } }
        
        private long pointId = 0;
        /// <summary>ガチャ実行に必要なポイント種別</summary>
        public long PointId { get { return pointId; } }
        
        private long pointValue = 0;
        /// <summary>ガチャ実行に必要な量</summary>
        public long PointValue { get { return pointValue; } }
        

        private GachaDiscountData discountData = null;
        /// <summary>割引情報</summary>
        public GachaDiscountData DiscountData { get { return discountData; } }
        
        private GachaSubPointData subPointData = null;
        /// <summary>代替ポイント</summary>
        public GachaSubPointData SubPointData { get { return subPointData; } }

        private TopSubPoint[] subPointList = null;
        /// <summary>代替ポイントのリスト</summary>
        public TopSubPoint[] SubPointList { get { return subPointList; } }

        private GachaRushData rushData = null;
        /// <summary>Rush用データ</summary>
        public GachaRushData RushData { get { return rushData; } }


        private bool canRush = false;
        /// <summary>確変があるガチャかどうか</summary>
        public bool CanRush { get { return canRush; } }

        private GachaSubPointData freeGachaPointData = null;
        /// <summary>表示する無料ガチャポイントの情報</summary>
        public GachaSubPointData FreeGachaPointData { get { return freeGachaPointData; } }

        private long gachaCount = 0;

        
        public GachaCategoryData(GachaSettingData settingData, TopCategory topCategory)
        {
            Init(settingData, topCategory, topCategory.value, 1 );
        }

        public GachaCategoryData(GachaSettingData settingData, TopCategory topCategory, long pointValue, long GachaCount )
        {
            Init(settingData, topCategory, pointValue, GachaCount );
        }
        
        /// <summary>割引情報がある</summary>
        public bool EnableDiscount { get { return discountData != null && discountData.IsEnable; } }
        
        /// <summary>代替ポイントの設定がある</summary>
        public bool EnableSubPoint { get { return subPointData != null; } }
        
        /// <summary>実施回数</summary>
        public long GachaCount
        {
            get 
            {
                return gachaCount;
            }
        }
        
        /// <summary>もう一度引くが実行可能か？</summary>
        public bool CanRetryOnce
        {
            get
            {
                if(discountData != null)
                {
                    // 割引がもう使えない
                    if(!discountData.IsEnable) return false;
                }


                if(subPointData != null)
                {
                    // 所持数が足りない
                    var val = GachaUtility.FetchPointValue(subPointData.subPointId);
                    if(subPointData.value > val) {
                        return false;
                    }
                }

                // ガチャ実行上限に達した
                if(!settingData.IsEnable) return false;
                
                return true;
            }
        }

        public long Price { 
            get { 
                 // 割引情報がある
                if(EnableDiscount)
                {
                    return discountData.Price;
                }
                
                // 代替ポイントがある
                if(EnableSubPoint)
                {
                    return subPointData.value;
                }
                
                
                return pointValue; 
            } 
        }



        void Init(GachaSettingData settingData, TopCategory topCategory, long pointValue, long gachaCount )
        {
            this.settingData = settingData;
            
            // m_gacha_category.id
            gachaCategoryId = topCategory.id;
            // 報酬数
            prizeCount = topCategory.prizeCount;
            // 使用ポイント
            pointId = topCategory.mPointId;
            // 金額
            this.pointValue = pointValue;
            //Rushがあるか
            canRush = topCategory.canRush;
            
            // memo
            // discountとsubPointはどちらか一方しか適用されない

            // 割引情報がある
            if(topCategory.discount != null && topCategory.discount.id > 0 )
            {
                discountData = new GachaDiscountData(topCategory.discount);
            }
            // 代替ポイントが設定されてる
            else
            {
                SetSubPointData(topCategory.subPointList);
            }

            if( topCategory.rush != null ) {
                rushData = new GachaRushData(topCategory.rush);
            }
            this.gachaCount = gachaCount;
        }

        public void UpdateRushData( GachaRushData rushData){
            if( rushData.isFinished >= 1  ) {
                this.rushData = null;
            } else {
                this.rushData = new GachaRushData(rushData);
            }
        }

        public void ClearRushData(){
            this.rushData = null;
        }

        public void UpdateGachaCount( long count ){
            gachaCount = count;
        }

        public void UpdatePointValue( long val ){
            pointValue = val;
        }
        
        // 代替ポイントの設定
        private void SetSubPointData(TopSubPoint[] categorySubPointList)
        {
            subPointList = categorySubPointList;
            UpdatePointId();
        }
        
        /// <summary>どの代替ポイントを使用するか更新</summary>
        public void UpdatePointId()
        {
            if(subPointList == null) return;
            
            TopSubPoint useSubPoint = null;
            // 所持数が充足しているもの > 表示フラグが立っている最も優先度の高いものの順で一番満たしているもののみ表示する
            // そのため、表示フラグが立っている最も優先度の高いものをキャッシュしておく
            GachaSubPointData highPrioritySubPointData = null;
            // 表示対象の無料ガチャポイントをリセット
            freeGachaPointData = null;
            // 優先度でソート
            foreach(TopSubPoint subPoint in subPointList.OrderByDescending(data => data.priority))
            {
                // ポイントを所持しているか
                bool possessionPointFlg = UserDataManager.Instance.point.data.ContainsKey(subPoint.mPointId);
                // 現在の所持数を取得
                long subItemValue = possessionPointFlg ? UserDataManager.Instance.point.Find(subPoint.mPointId).value : 0;
                // 所持数が足りているか
                bool enoughFlg = subPoint.value <= subItemValue;
                // 表示フラグが立っていてかつhighPrioritySubPointDataがnullの場合キャッシュしておく(priorityが最も高いものがキャッシュされるイメージ)
                if (subPoint.displayFlg && highPrioritySubPointData == null)
                {
                    highPrioritySubPointData = new GachaSubPointData(subPoint.id, subPoint.mPointId, subItemValue, subPoint.priority);
                }
                // ポイントを所持していない
                if(!possessionPointFlg) continue;
                // 所持数が足りない
                if(!enoughFlg) continue;
                
                // 表示フラグが立っているならそのポイントをキャッシュする
                if (subPoint.displayFlg)
                {
                    freeGachaPointData = new GachaSubPointData(subPoint.id, subPoint.mPointId, subItemValue, subPoint.priority);
                }
                
                // 使用ポイントとして格納
                useSubPoint = subPoint;
                
                break;
            }
            // 使用可能な代替ポイントがある
            if(useSubPoint != null)
            {
                subPointData = new GachaSubPointData(useSubPoint);
                // 使用ポイントを上書き
                pointId = useSubPoint.mPointId;
            }
            // freeGachaPointDataがnullの場合はhighPrioritySubPointDataを表示させる
            if (freeGachaPointData == null)
            {
                freeGachaPointData = highPrioritySubPointData;
            }
        }
    }
}
