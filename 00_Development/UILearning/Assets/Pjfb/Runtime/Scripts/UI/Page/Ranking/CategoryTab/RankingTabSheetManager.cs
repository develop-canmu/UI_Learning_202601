using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Ranking
{
    public enum RankingTabSheetType
    {
        None = -1,
        TotalPower = 0, // 総戦力
        CharacterPower = 1, // 選手戦力
        TrainerLevel = 2,　// トレーナーレベル
        TotalPoint = 3,　// イベントポイント
    }
    
    public class RankingTabSheetManager : SheetManager<RankingTabSheetType>
    {
        [Serializable]
        public class RankingTabBadge
        {
            [SerializeField] private RankingTabSheetType type;
            public RankingTabSheetType Type => type;
            [SerializeField] private UIBadgeNotification badgeObject;
            public UIBadgeNotification BadgeObject => badgeObject;
        }
        
        // 個人：辞書型で管理する<rankingClientViewId,APIのレスポンスクラス>
        private Dictionary<long,RankingGetUserCommonRankListAPIResponse> userRankingResponseData = new Dictionary<long,RankingGetUserCommonRankListAPIResponse>();
        // ギルド：辞書型で管理する<rankingClientViewId,APIのレスポンスクラス>
        private Dictionary<long,RankingGetGuildCommonRankListAPIResponse> clubRankingResponseData = new Dictionary<long,RankingGetGuildCommonRankListAPIResponse>();
        
        // 個人ランキングデータ取得
        public async UniTask<RankingGetUserCommonRankListAPIResponse> GetUserRankingResponseData(long rankingClientViewId)
        {
            if (userRankingResponseData.ContainsKey(rankingClientViewId) == false)
            {
                CancellationToken token = this.GetCancellationTokenOnDestroy();
                RankingGetUserCommonRankListAPIResponse response = await RankingManager.GetRankingUserData(rankingClientViewId, token);
                userRankingResponseData.Add(rankingClientViewId, response);
            }
            return userRankingResponseData[rankingClientViewId];
        }
        
        // クラブランキングデータ取得
        public async UniTask<RankingGetGuildCommonRankListAPIResponse> GetGuildRankingResponseData(long rankingClientViewId)
        {
            if (clubRankingResponseData.ContainsKey(rankingClientViewId) == false)
            {
                CancellationToken token = this.GetCancellationTokenOnDestroy();
                RankingGetGuildCommonRankListAPIResponse response = await RankingManager.GetRankingGuildData(rankingClientViewId, token);
                clubRankingResponseData.Add(rankingClientViewId, response);
            }
            return clubRankingResponseData[rankingClientViewId];
        }
    }

    public abstract class RankingCategoryTabSheet : Sheet
    {
        [SerializeField] protected RankingTabSheetType sheetType;
        public RankingTabSheetType SheetType => sheetType;
        
        /// <summary>個人とクラブのシートを管理しているSheetManager</summary>
        [SerializeField]
        private RankingAffiliateTabSheetManager rankingAffiliateTabSheetManager = null;

        /// <summary>ランキング画面で個人、クラブのシートを開く処理を実行するための参照</summary>
        public RankingAffiliateTabSheetManager RankingAffiliateTabSheetManager => rankingAffiliateTabSheetManager;
        
        /// <summary>描画の更新処理 </summary>
        public void UpdateView(object rankingData)
        {
            RankingTabSheet currentSheet = (RankingTabSheet)rankingAffiliateTabSheetManager.CurrentSheet;
            currentSheet.UpdateView(rankingData, sheetType != RankingTabSheetType.TotalPoint);
        }

        /// <summary>現在開いている個人、クラブのタイプに報酬画面で表示する報酬データをセットする</summary>
        public void UpdateRewardView(RankingRewardView rankingRewardView)
        {
            RankingTabSheet currentSheet = (RankingTabSheet)rankingAffiliateTabSheetManager.CurrentSheet;
            currentSheet.OnUpdateRewardView(rankingRewardView);
        }
        
        public void Init()
        {
            InitView();    
        }
        
        protected virtual void InitView(){}
    }
}