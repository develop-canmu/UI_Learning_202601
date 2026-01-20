using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;

namespace Pjfb.Ranking
{
    public enum RankingAffiliateTabSheetType
    {
        None = -1,
        Personal = 0,   // 個人
        Club = 1,    // クラブ
    }
    public static class RankingManager
    {
        // ランキングに関するチェッククラス
        private enum RankingCheckType
        {
            Holding = 0, // 開催中のランキング
            Badge = 1, // バッジ表示
        }
        
        // バッジ表示
        public static bool IsShowHomeBadge { get { return CheckRankingHomePage(RankingCheckType.Badge); } }
        // ランキング開催中判定
        public static bool IsHoldingRanking { get { return CheckRankingHomePage(RankingCheckType.Holding); } }
        
        // ランキングの表示確認
        private static bool CheckRankingHomePage(RankingCheckType type)
        {
            foreach (RankingClientPreviewMasterObject rankingData in MasterManager.Instance.rankingClientPreviewMaster.values )
            {
                if (rankingData.holdingType == RankingClientPreviewMasterObject.RankingHoldingType.RegularRanking || AppTime.IsInPeriod(rankingData.displayStartAt, rankingData.displayEndAt))
                {
                    switch (type)
                    {
                        case RankingCheckType.Holding:
                            return true;
                        case RankingCheckType.Badge:
                            if (!LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(rankingData.id))
                            { 
                                return true;
                            }
                            break;
                    }
                }
            }
            return false;
        }
        
        /// バナー画像URL
        public static string GetBannerURL(long id)
        {
            return $"{AppEnvironment.AssetBrowserURL}/ranking/image_ranking/ranking_banner_{id}.png";
        }
        
        // 個人のランキングデータを取得
        public static async UniTask<RankingGetUserCommonRankListAPIResponse> GetRankingUserData(long clientPreviewId, CancellationToken token = default)
        {
            RankingGetUserCommonRankListAPIPost post = new RankingGetUserCommonRankListAPIPost();
            post.mRankingClientPreviewId = clientPreviewId;
            RankingGetUserCommonRankListAPIRequest request = new RankingGetUserCommonRankListAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }

        // クラブのランキングデータを取得
        public static async UniTask<RankingGetGuildCommonRankListAPIResponse> GetRankingGuildData(long clientPreviewId, CancellationToken token = default)
        {
            RankingGetGuildCommonRankListAPIPost post = new RankingGetGuildCommonRankListAPIPost();
            post.mRankingClientPreviewId = clientPreviewId;
            RankingGetGuildCommonRankListAPIRequest request = new RankingGetGuildCommonRankListAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
    }
}