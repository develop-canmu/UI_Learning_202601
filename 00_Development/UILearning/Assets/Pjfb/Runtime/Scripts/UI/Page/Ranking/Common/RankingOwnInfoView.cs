using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    public class RankingOwnInfoView : RankingInfoView
    {
        [SerializeField]
        private UserIcon userIcon = null;
        
        private long userId = 0;
        
        /// <summary>
        /// アイコンをセットする
        /// </summary>
        /// <param name="iconId">アイコンのID</param>
        public void SetUserIcon(long iconId)
        {
            userIcon.SetIconId(iconId);
        }
        
        /// <summary>
        /// ユーザーIDをセットする
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        public void SetUserId(long userId)
        {
            this.userId = userId;
        }
        
        /// <summary>
        /// アイテムクリック時の処理
        /// </summary>
        public void OnClickItem()
        {
            // 自分のプロフィールは開かない
            if (userId == UserDataManager.Instance.user.uMasterId)
            {
                return;
            }
            
            // ユーザープロフィールを開く
            Menu.TrainerCardModalWindow.WindowParams windowParams = new Menu.TrainerCardModalWindow.WindowParams(userId, false, showClubInfoHeaderButtons:false, onDissolution:null, onSecession:null);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, windowParams);
        }
    }
}