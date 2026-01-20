using System.Collections.Generic;
using CruFramework;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb.LeagueMatchTournament
{
    // グループラベルアイテム
    public class LeagueMatchTournamentBannerGroupView : MonoBehaviour
    {
        // バナーラベルイメージ
        [SerializeField]
        private CancellableImage labelImage = null;
        
        // バナーラベルテキスト
        [SerializeField] 
        private TMP_Text labelText = null;

        // ラベルルートオブジェクト
        [SerializeField] 
        private GameObject labelRoot = null; 
        
        // 複製するバナー
        [SerializeField] 
        private LeagueMatchTournamentBanner bannerPrefab;

        // 生成位置
        [SerializeField] 
        private Transform bannerRoot;

        // 生成したバナーリスト
        private List<LeagueMatchTournamentBanner> bannerCacheList = new List<LeagueMatchTournamentBanner>();

        public void SetView(LeagueMatchTournamentManager.BannerGroupType groupType, List<LeagueMatchTournamentInfo> tournamentInfoList)
        {
            // グループラベル設定
            SetGroupLabel(groupType);

            // 必要な分生成
            for (int i = bannerCacheList.Count; i < tournamentInfoList.Count; i++)
            {
                LeagueMatchTournamentBanner banner = Instantiate<LeagueMatchTournamentBanner>(bannerPrefab, bannerRoot);
                bannerCacheList.Add(banner);
            }

            // すべて非表示に
            foreach (LeagueMatchTournamentBanner banner in bannerCacheList)
            {
                banner.gameObject.SetActive(false);
            }

            // Viewをセット
            for (int i = 0; i < tournamentInfoList.Count; i++)
            {
                LeagueMatchTournamentBanner banner = bannerCacheList[i];
                banner.SetView(tournamentInfoList[i]);
                banner.gameObject.SetActive(true);
            }

        }

        /// <summary> グループラベルセット </summary>
        private void SetGroupLabel(LeagueMatchTournamentManager.BannerGroupType groupType)
        {
            // 表示しないならラベル非表示
            if (groupType == LeagueMatchTournamentManager.BannerGroupType.None)
            {
                labelRoot.SetActive(false);
                return;
            }
            
            labelRoot.SetActive(true);
            // グループラベルの下地をセット
            labelImage.SetTexture(ResourcePathManager.GetPath("LeagueMatchTournamentGroupLabel", (int)groupType));
            labelText.text = MasterManager.Instance.colosseumEventGroupLabelMaster.FindData((int)groupType).name;
        }
    }
}