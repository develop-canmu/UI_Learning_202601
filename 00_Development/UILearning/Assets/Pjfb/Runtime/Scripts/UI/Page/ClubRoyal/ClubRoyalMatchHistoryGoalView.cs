using UnityEngine;

namespace Pjfb.ClubRoyal
{
    /// <summary>
    /// 試合履歴の各アイテムのクラブ情報に表示するゴールのView
    /// 今後、ゴールの名前やアイコンを変更する可能性があるためViewとして分離
    /// </summary>
    public class ClubRoyalMatchHistoryGoalView : MonoBehaviour
    {
        /// <summary>ゴールをグレーアウトするフィルター</summary>
        [SerializeField]
        private GameObject goalFilter = null;

        /// <summary>ゴールをグレーアウトするフィルターをセット</summary>
        public void SetFilter(bool isFilter)
        {
            goalFilter.SetActive(isFilter);
        }
    }
}