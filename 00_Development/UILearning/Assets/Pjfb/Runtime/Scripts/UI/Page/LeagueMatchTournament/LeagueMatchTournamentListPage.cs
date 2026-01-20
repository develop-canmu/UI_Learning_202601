using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using UnityEngine;

namespace Pjfb.LeagueMatchTournament
{
    // 大会リストページ
    public class LeagueMatchTournamentListPage : Page
    {
        // シートマネージャー
        [SerializeField]
        private LeagueMatchTournamentListStatusTabSheetManager sheetManager = null;
        
        // 開催中リスト
        [SerializeField] 
        private LeagueMatchTournamentOnSeasonListView onSeasonListView = null;
        // 終了リスト
        [SerializeField] 
        private LeagueMatchTournamentEndSeasonListView endSeasonListView = null;
        
        // 大会情報
        private List<LeagueMatchTournamentInfo> tournamentInfoList;

        // 開催中の大会情報リスト
        List<LeagueMatchTournamentInfo> onSeasonInfoList = new List<LeagueMatchTournamentInfo>();
        // 終了済みの大会情報リスト
        List<LeagueMatchTournamentInfo> endSeasonInfoList = new List<LeagueMatchTournamentInfo>();
        
        // 次の更新時間
        private DateTime updateTime = DateTime.MaxValue;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // シートを開く際の処理を登録
            sheetManager.OnPreOpenSheet -= UpdateSheet;
            sheetManager.OnPreOpenSheet += UpdateSheet;
            // 大会情報更新
            UpdateMatchInfo();

            // シートを開く
            await sheetManager.OpenSheetAsync(sheetManager.FirstSheet, null);
            await base.OnPreOpen(args, token);
        }
        
        private void Update()
        {
            // データがないなら更新しない
            if (tournamentInfoList == null || tournamentInfoList.Count <= 0)
            {
                return;
            }

            // 更新時間を超えたならViewを更新
            if (updateTime.IsPast(AppTime.Now))
            {
                // 開催情報を更新
                UpdateMatchInfo();
                
                // 更新タイミングではシート更新をかける
                UpdateSheet(sheetManager.CurrentSheetType);
            }
        }

        /// <summary> 大会状況更新 </summary>
        private void UpdateMatchInfo()
        {
            // 更新時間を初期化
            updateTime = DateTime.MaxValue;
            // 最新の大会リストに更新
            tournamentInfoList = LeagueMatchTournamentManager.GetTournamentList();

            // 次の更新時間をセット
            foreach (LeagueMatchTournamentInfo info in tournamentInfoList)
            {
                // 一番更新時間が近い時間を更新時間にする
                if (info.UpdateTime.IsPast(updateTime))
                {
                    updateTime = info.UpdateTime;
                }
            }
            
            // 開催中の大会情報リスト初期化
            onSeasonInfoList.Clear();
            // 終了済みの大会情報リスト初期化
            endSeasonInfoList.Clear();
            
            // 開催中のものと終了済みのもので振り分け
            foreach (LeagueMatchTournamentInfo info in tournamentInfoList)
            {
                // 終了済み大会
                if (info.GroupType == LeagueMatchTournamentManager.BannerGroupType.None)
                {
                    endSeasonInfoList.Add(info);
                    continue;
                }

                // 開催中
                onSeasonInfoList.Add(info);
            }
        }

        /// <summary> シート更新処理 </summary>
        private void UpdateSheet(LeagueMatchTournamentListStatusTabSheetType sheetType)
        {
            switch (sheetType)
            {
                case LeagueMatchTournamentListStatusTabSheetType.OnSeasonTournament:
                {
                    // 開催中のView
                    onSeasonListView.SetView(onSeasonInfoList);
                    break;
                }
                case LeagueMatchTournamentListStatusTabSheetType.EndSeasonTournament:
                {
                    // 終了済みのView
                    endSeasonListView.SetView(endSeasonInfoList);
                    break;
                }
            }
        }
    }
}