using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.LeagueMatch;
using Pjfb.LeagueMatchTournament;
using Pjfb.UserData;

namespace Pjfb
{
    /// <summary> リーグマッチ大会ホームバナー </summary>
    public class LeagueMatchTournamentHomeBanner : MonoBehaviour
    {
        // ベースイメージオブジェクト
        [SerializeField] 
        private GameObject baseImage = null;
        
        // クラブ未加入時のイメージオブジェクト
        [SerializeField] 
        private GameObject noClubImage = null;

        // Newバッジ
        [SerializeField]
        private UIBadgeNotification newBadge = null;

        /// <summary>クラブに参加済み</summary>
        private bool IsJoinedClub
        {
            get { return UserDataManager.Instance.user.gMasterId != 0; }
        }
        
        //// <summary> 表示セット </summary>
        public void SetView(List<LeagueMatchTournamentInfo> tournamentList)
        {
            // 表示するデータがないならバナーを非表示に
            if (tournamentList == null || tournamentList.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            // Newバッジを表示するか
            bool showNewBadge = false;
            
            foreach (LeagueMatchTournamentInfo info in tournamentList)
            {
                // 新規イベントならバッジを表示
                if (ColosseumManager.IsNewEvent(info.MColosseumEvent.id))
                {
                    showNewBadge = true;
                    break;
                }
            }
            // Newバッジのアクティブ
            newBadge.SetActive(showNewBadge);
            
            // クラブ未参加時のイメージセット
            if (IsJoinedClub == false)
            {
                baseImage.gameObject.SetActive(false);
                noClubImage.gameObject.SetActive(true);
                return;
            }
            
            baseImage.gameObject.SetActive(true);
        }

        /// <summary> ボタン押下時の処理 </summary>
        public void OnClick()
        {
            // クラブ未参加
            if (IsJoinedClub == false)
            {
                // クラブ機能が解放されてないなら解放条件ポップアップを表示
                if(!UserDataManager.Instance.IsUnlockSystem(ClubUtility.clubLockId))
                {
                    ClubUtility.OpenClubLockModal();
                    return;
                }
                
                // 機能が解放されているがクラブ未参加の場合はクラブを探すページへ遷移する
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, true, null);
                return;
            }
            
            // 大会ページを開く
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatchTournament, true, null);
        }
    }
}