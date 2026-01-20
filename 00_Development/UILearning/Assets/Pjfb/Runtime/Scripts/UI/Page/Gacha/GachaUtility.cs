using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Networking.App.Request;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Pjfb.UserData;
using Pjfb.Storage;
using Pjfb.Shop;
using Pjfb.Master;

namespace Pjfb.Gacha
{
    public enum GachaType
    {
        // 通常ガチャ
        Normal = 1,
        // チケットガチャ
        Ticket = 2,
    }
    
    public enum GachaCategory
    {
        // 単発ガチャ
        Single = 0,
        // 複数ガチャ
        Multi = 1,
    }
    
    public enum GachaDisableType
    {
        None = 0,
        // 融合状態
        Enable = 1,
        // 近日公開
        ComingSoon = 2,
    }
    

    public static class GachaUtility
    {
        //無期限に設定するendAt
        public static readonly DateTime indefinitePeriodDateTime = new DateTime(2099,1,1,0,0,0);
        
        public static readonly int TicketGachaDesignNumber = 999;

        public static readonly int goldBallRarity = 2;

        // 最大ガチャ回数
        public static readonly int drawMax = 10;

        // Boxガチャの最大週数
        public static readonly int BoxGachaLoopLap = 99999;

        // <PointId, 仮想ポイントのデータリスト>
        private static Dictionary<long, List<AlternativePointData>> pointAlternativeDictionary = new Dictionary<long, List<AlternativePointData>>();

        /// <summary>{0}回ガチャ</summary>
        public static readonly string PrizeCountStringKey = "gacha.prize_count";
        
        /// <summary>{0}{1}{2}を消費して{3}回ガチャを引きます。<br>よろしいですか？</summary>
        public static readonly string DrawGachaConfirmStringKey = "gacha.draw_gacha_confirm";
    
        /// <summary>保留情報があるか</summary>
        public static bool HasPendingInfo(GachaGetListAPIResponse response)
        {
            return response.pendingInfo?.uGachaResultPendingId > 0;
        }
        
        /// <summary>保留情報があるか</summary>
        public static bool HasPendingInfo(GachaExecuteAPIResponse response)
        {
            return response.pendingInfo?.uGachaResultPendingId > 0;
        }
        
        /// <summary>バナー画像URL</summary>
        public static string GetGachaLargeBannerURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/large_banner/gacha_large_banner_{id}.png";
        }
        
        /// <summary>バナー画像URL</summary>
        public static string GetGachaSmallBannerURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/small_banner/gacha_small_banner_{id}.png";
        }
        
        /// <summary>バナー画像URL</summary>
        public static string GetGachaTicketBannerURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/ticket_banner/gacha_ticket_banner_{id}.png";
        }

        /// <summary>ラッシュロゴ画像URL</summary>
        public static string GetGachaRushLogoURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_logo.png";
        }

        /// <summary>ラッシュリザルトロゴ画像URL</summary>
        public static string GetGachaRushResultLogoURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/result_logo.png";
        }
        
        
        /// <summary>ラッシュ画像URL</summary>
        public static string GetGachaRushImageURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_image.png";
        }
        
        /// <summary>ラッシュ背景画像URL</summary>
        public static string GetGachaRushBGURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_bg.png";
        }

        
        /// <summary>ラッシュ終了時ロゴ画像URL</summary>
        public static string GetGachaRushFinishedImageURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_finished_image.png";
        }
        
        /// <summary>ラッシュ終了時BG画像URL</summary>
        public static string GetGachaRushFinishedBGURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_finished_bg.png";
        }

        /// <summary>ガチャトップのラッシュ画像</summary>
        public static string GetGachaRushTopImageURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/gacha/rush_image/{id}/rush_topon_image.png";
        }

        /// <summary>終了期限かあるか</summary>
        public static bool IsIndefinitePeriod(DateTime endAt)
        {
            return endAt >= GachaUtility.indefinitePeriodDateTime;
        }


        /// <summary>
        /// ガチャの実行確認と実行
        /// </summary>
        /// <param name="categoryData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async UniTask<GachaResultPageData> ConfirmAndExecuteGacha(GachaCategoryData categoryData, CancellationToken token)
        {
            long pointCount = 0;
            long pointAlternativeCount = 0;
            
            // ポイントの所持数取得
            if(UserDataManager.Instance.point.data.ContainsKey(categoryData.PointId))
            {
                pointCount = UserDataManager.Instance.point.Find(categoryData.PointId).value;
            }
            
            // 仮想ポイントが使えるなら所持数取得
            if (IsUsablePointAlternative(categoryData.PointId, categoryData))
            {
                pointAlternativeCount = GetPointAlternativeCount(categoryData.PointId, categoryData.GachaCategoryId);
            }

            // ポイントと仮想ポイントの合算が価格を超えていればガチャ実行できる
            if(pointCount + pointAlternativeCount >= categoryData.Price)
            {
                var resultPageData = await GachaUtility.ExecuteGacha(categoryData, token);
                return resultPageData;
            }
            else
            {
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GachaExecuteConfirmPointShortage, categoryData);
                return null;
            }
        }

        /// <summary>
        /// ガチャの実行
        /// </summary>
        /// <param name="categoryData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async UniTask<GachaResultPageData> ExecuteGacha(GachaCategoryData categoryData, CancellationToken token)
        {
            //実行前ポイント量取得
            var exchangePointId = categoryData.SettingData.ExchangePointId;
            var beforePoint = FetchPointValue(exchangePointId);

            CruFramework.Page.ModalWindow modalWindow = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.GachaExecuteConfirm, categoryData, token);

            // モーダルが閉じるまで待機
            GachaExecuteAPIResponse response = (GachaExecuteAPIResponse)await modalWindow.WaitCloseAsync();
            // 実行結果がない
            if(response == null) return null;
            
            GachaResultPageData resultPageData = null;
            // 保留情報がある
            if(GachaUtility.HasPendingInfo(response))
            {
                resultPageData = new GachaResultPageData(response.pendingInfo, categoryData, response.productionNumberList, response.rushCategoryInfoList );
            }
            else
            {
                resultPageData = new GachaResultPageData(response.contentListList, response.autoSell, response.canShowReview, response.rushCategoryInfoList, categoryData, response.productionNumberList);
            }

            //交換後ポイント取得
            var afterPoint = FetchPointValue(exchangePointId);
            resultPageData.SetPointData( exchangePointId, beforePoint, afterPoint );
            
            return resultPageData;
        }

        /// <summary>
        /// ポイント量の取得
        /// </summary>
        public static long FetchPointValue( long pointId ) {
            if( pointId == 0 ) {
                return 0;
            }

            if( !UserDataManager.Instance.point.Contains(pointId) ) {
                return 0;
            }

            var point = UserDataManager.Instance.point.Find(pointId);
            if( point == null ) {
                CruFramework.Logger.LogError("not find point : " + pointId);
                return 0;
            } 
            return point.value;
        }

        /// <summary>
        /// ガチャの引ける回数の計算
        /// </summary>
        public static long CalcMultiDrawCount( GachaCategoryData data ) {
            long pointVal =  GachaUtility.FetchPointValue(data.PointId); 
            long needVal = data.Price;
            return CalcMultiDrawCount(pointVal, needVal, GachaUtility.drawMax);
        }

        public static long CalcMultiDrawCount( long pointValue, long needVal, long maxCount ) {
            long count = (pointValue / needVal);
            if( count > maxCount ) {
                count = maxCount;
            }
            return count;
        }


        /// <summary>
        /// ピックアップ選択データを確認済みか
        /// </summary>
        public static bool IsConfirmedPickUpSelectableData( long choiceId, long elementId,  GachaPickUpSelectableCharacterData selectableData ){
            var gachaSaveData = LocalSaveManager.saveData.gachaData.lastCheckDate.FirstOrDefault(itr=> itr.choiceId == choiceId && itr.elementId == elementId );
            if( gachaSaveData == null ) {
                //セーブデータが存在しなかったら未確認
                return false;
            }

            //セーブデータにIdが保存されていなかったら未確認
            var prizeData = PrizeJsonUtility.GetPrizeContainerData(selectableData.prize);
            long id = 0;
            if( prizeData.itemIconType == ItemIconType.Character ) {
                id = selectableData.prize.args.mCharaId;
            } else if ( prizeData.itemIconType == ItemIconType.Item ){
                id = selectableData.prize.args.mPointId;
            } else if ( prizeData.itemIconType == ItemIconType.SupportEquipment ){
                id = selectableData.prize.args.variableTrainerMCharaId;
            } else {
                return true;
            }

            if( gachaSaveData.confirmedPrizeIds.Any(itr=>itr == id) ){
                return true;
            }
        
            return false;
        }

        // 金サッカーボール判定
        public static bool IsGoldBallRarity( long rarity ){
            return rarity >= goldBallRarity;
        }

        public static long GetGachaPrizeRarity( PrizeJsonViewData data ){
            long rarity = 0;
            
            // ItemIconTypeがCharacterかSupportEquipmentの場合はレアリティを取得
            // サポカやアドバイザーもCharacterで来る
            if( data.ItemIconType == ItemIconType.Character || data.ItemIconType == ItemIconType.SupportEquipment ) {
                var rarityId = RarityUtility.GetRarityId(data.Id, 0);
                rarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
            }
            return rarity;
        }
        
        /// <summary>仮想ポイント情報の更新</summary>
        public static void UpdatePointAlternativeDictionary()
        {
            // 綺麗にする
            pointAlternativeDictionary.Clear();
            // m_point_alternativeから所持してる仮想ポイントの情報を構築
            // すべてのマスタをチェック
            foreach (PointAlternativeMasterObject master in MasterManager.Instance.pointAlternativeMaster.values)
            {
                // ポイントIDが登録されているならとばす
                if (pointAlternativeDictionary.ContainsKey(master.mPointId))
                {
                    continue;
                }
                
                // 一致するポイントIdのマスタを取得
                IEnumerable<PointAlternativeMasterObject> pointAlternativeList = MasterManager.Instance.pointAlternativeMaster.FindPointAlternativeInPeriod(master.mPointId,AlternativePointUseType.Gacha);
                // 仮想ポイントのデータリスト
                List<AlternativePointData> alternativePointList = new List<AlternativePointData>();
                
                // ポイントIDに紐づいている仮想ポイントのデータをリストにまとめる
                foreach (var mPointAlternative in pointAlternativeList)
                {
                    // 仮想ポイントのユーザーデータを取得
                    UserDataPoint point = UserDataManager.Instance.point.Find(mPointAlternative.mPointIdAlternative);
                    if (point != null)
                    {
                        AlternativePointData alternativeData = new AlternativePointData(point.pointId, mPointAlternative);
                        alternativePointList.Add(alternativeData);
                    }
                }
                // ポイントIDごとの仮想ポイントリストに追加
                pointAlternativeDictionary.Add(master.mPointId, alternativePointList);
            }
        }
        
        /// <summary>代用可能な仮想ポイントの一覧を取得</summary>
        public static List<AlternativePointData> GetPointAlternativeList(long mPointId)
        {
            // データがあればそれを返す
            if(pointAlternativeDictionary.ContainsKey(mPointId))
            {
                return pointAlternativeDictionary[mPointId]; 
            }
            // なければ空のリストを返す
            return new List<AlternativePointData>();
        }
        
        /// <summary>代用可能な仮想ポイントの中で優先度の高いものを取得</summary>
        public static AlternativePointData GetPointAlternative(long mPointId, long gachaCategoryId)
        {
            AlternativePointData result = null;
            // 優先度順に並べられたリストを取得
            List<AlternativePointData> pointAlternativeList = GetPointAlternativeList(mPointId);
            // リストが存在する場合、優先度の高いものから使用可否を確認
            if(pointAlternativeList.Count > 0)
            {
                foreach (var alternativeData in pointAlternativeList)
                {
                    // idの指定がない場合は無制限
                    if(alternativeData.AlternativeMasterObject.targetIdList == null || alternativeData.AlternativeMasterObject.targetIdList.Length == 0)
                    {
                        // 無制限のものがあればそれを使用
                        result = alternativeData;
                        break;
                    }

                    // ガチャ指定のものがあればそれを使用
                    if (alternativeData.AlternativeMasterObject.targetIdList.Contains(gachaCategoryId))
                    {
                        result = alternativeData;
                        break;
                    }
                    
                }
            }
            // 結果を返す
            return result;
        }

        /// <summary>使用可能な仮想ポイントがあるか</summary> 
        private static bool IsUsablePointAlternative(long pointId, long gachaCategoryId)
        {
            AlternativePointData data = GetPointAlternative(pointId, gachaCategoryId);
            
            if (data == null)
            {
                return false;
            }
            return data.UserData.value > 0;
        }

        /// <summary> 仮想ポイントを使用できるか(割引と代替ポイントが設定されている場合、仮想ポイントは使用できない) </summary>
        public static bool IsUsablePointAlternative(long pointId, GachaCategoryData data)
        {
            if (data == null)
            {
                return false;
            }
            
            return !data.EnableDiscount && !data.EnableSubPoint && IsUsablePointAlternative(pointId, data.GachaCategoryId);
        }

        /// <summary>仮想ポイントの所持数を取得</summary>
        public static long GetPointAlternativeCount(long mPointId, long gachaCategoryId)
        {
            AlternativePointData data = GetPointAlternative(mPointId, gachaCategoryId);
            // ポイントを持ってない
            if (data == null)
            {
                return 0;
            }
            // ポイントを持っている
            return data.UserData.value;
        }
        
        /// <summary>キャッシュ削除</summary>
        public static void Release()
        {
            pointAlternativeDictionary.Clear();
        }
    }
}
