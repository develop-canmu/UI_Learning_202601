using System;
using UnityEngine;

namespace Pjfb.RecommendChara
{
    [Serializable]
    public class RecommendCharaCheckNotificationData
    {
        [SerializeField] protected string lastViewDate = string.Empty;

        public void UpdateViewDate()
        {
            lastViewDate = AppTime.Now.ToString();
        }

        public bool ShouldShowNotificationBadge()
        {
            if (string.IsNullOrEmpty(lastViewDate)) return true;
            
            // クライアント側で管理する
            // 選手リスト更新バッチが起動する時間を過ぎていたらバッジを表示、ユーザーがリストを閲覧したら非表示
            var lastViewDateTime = AppTime.Parse(lastViewDate);
            return lastViewDateTime.Day < AppTime.Now.Day;
        }
    }
}
