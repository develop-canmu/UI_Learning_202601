using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.LeagueMatchTournament
{
    // 大会リスト表示ビュー
    public abstract class LeagueMatchTournamentListBaseView : MonoBehaviour
    {
        // スクローラー
        [SerializeField] 
        protected ScrollDynamic scroll = null;

        // 注略テキスト
        [SerializeField] 
        protected TMP_Text annotationText = null;

        public void SetView(List<LeagueMatchTournamentInfo> tournamentInfos)
        {
            // データが存在するか
            bool isExistData = tournamentInfos != null && tournamentInfos.Count > 0;
            // 表示するデータがないなら注略テキストを表示
            annotationText.gameObject.SetActive(isExistData == false);
            // データが無い場合はスクロールを切る
            scroll.enabled = isExistData;
            
            // スクロールバーの表示も切る(AutoHideだとスクロール制御切った際に更新かからないので)
            if (scroll.verticalScrollbar != null)
            {
                scroll.verticalScrollbar.gameObject.SetActive(isExistData);
            }
            
            // 表示するデータが無いなら空でセット
            if (isExistData == false)
            {
                scroll.SetItems(new List<LeagueMatchTournamentBannerScrollDynamicItem.Param>());
                return;
            }
            
            // データがあるならスクロールアイテムをセット
            SetItem(tournamentInfos);
        }

        /// <summary> 表示データを並び替え </summary>
        protected List<LeagueMatchTournamentInfo> SortTournamentDataList(List<LeagueMatchTournamentInfo> tournamentInfos)
        {
            return tournamentInfos
                // 更新時間が近い順
                .OrderBy(x => x.UpdateTime)
                // マスタId順
                .ThenBy(x => x.MColosseumEvent.id).ToList();
        }
        
        /// <summary> アイテムのセット </summary>
        protected abstract void SetItem(List<LeagueMatchTournamentInfo> tournamentInfos);
    }
}