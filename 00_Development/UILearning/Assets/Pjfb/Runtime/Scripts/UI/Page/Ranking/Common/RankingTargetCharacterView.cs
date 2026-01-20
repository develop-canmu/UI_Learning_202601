using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Ranking
{
    public class RankingTargetCharacterView : MonoBehaviour
    {

        [SerializeField]
        private GameObject dateRoot = null;
        
        [SerializeField]
        private TMPro.TMP_Text dateText = null;
        
        /// <summary>表示の切り替え１</summary>
        public void ShowDateTime(bool show)
        {
            dateRoot.gameObject.SetActive(show);
        }
        
        /// <summary>時間を表示</summary>
        public void SetDate(string date)
        {
            dateText.text = date;
        }
        
        /// <summary>
        /// 詳細モーダルを開く
        /// </summary>
        public void OpenDetailModal(long mRankingClientId)
        {
            // モーダルに渡すデータ
            RankingTargetTrainingCharacterComfirmModal.Arguments args = new RankingTargetTrainingCharacterComfirmModal.Arguments(mRankingClientId);
            // モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.RankingTargetTrainingCharacterComfirm, args);
        }
    }
}