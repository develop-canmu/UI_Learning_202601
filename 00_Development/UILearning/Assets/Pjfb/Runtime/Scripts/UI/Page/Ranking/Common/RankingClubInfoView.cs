using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    public class RankingClubInfoView : RankingInfoView
    {
        [SerializeField]
        private Image emblemImage = null;
        [SerializeField] 
        private GameObject joinClubObject = null;
        [SerializeField]
        private GameObject noJoinClubText = null;
        private long clubId = 0;
        
        /// <summary>
        /// エンブレムをセットする
        /// </summary>
        /// <param name="emblemId">エンブレムのID</param>
        public void SetEmblem(long emblemId)
        {
            // クラブ未所属の場合は設定を行わない
            if (emblemId == 0)
            {
                emblemImage.gameObject.SetActive(false);
                return;
            }
            emblemImage.gameObject.SetActive(true);
            ClubUtility.LoadAndSetEmblemIconSync(emblemImage, emblemId);
        }
        
        /// <summary>
        /// クラブIDのセット
        /// </summary>
        /// <param name="id">クラブID</param>
        public void SetClubId(long id)
        {
            clubId = id;
        }

        /// <summary>
        /// アイテムをタップした時の処理
        /// </summary>
        public void OnClickItem()
        {
            OpenClubInfo(clubId).Forget();
        }

        /// <summary>
        /// クラブ未所属の時の処理
        /// </summary>
        public void ShowNoJoinClub()
        {
            joinClubObject.SetActive(false);
            noJoinClubText.SetActive(true);
        }

        /// <summary>
        /// クラブ所属時にオブジェクトを表示する処理
        /// </summary>
        public void ShowJoinClub()
        {
            joinClubObject.SetActive(true);
            noJoinClubText.SetActive(false);
        }
        
        /// <summary>
        /// クラブ情報をモーダルで表示する処理
        /// </summary>
        /// <param name="id">クラブID</param>
        private async UniTask OpenClubInfo(long id)
        {
            // クラブ未所属の場合、また所属しているクラブの場合は処理を行わない
            if(id == 0 || id == UserDataManager.Instance.user.gMasterId)
            {
                return;
            }
            // APIを使ってクラブ情報を取得
            GuildGetGuildAPIRequest request = new GuildGetGuildAPIRequest();
            GuildGetGuildAPIPost post = new GuildGetGuildAPIPost();
            post.gMasterId = id;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            GuildGetGuildAPIResponse response = request.GetResponseData();
            
            // クラブ情報をモーダルで表示
            ClubInfoModal.Param param = new ClubInfoModal.Param();
            param.clubData = new ClubData(response.guild);
            param.myUserID = 0;
            param.showUserProfileOtherButtons = false;
            param.showHeaderButtons = false;
            param.onFinishedDissolution = null;
            param.onFinishedSecession = null;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInfo, param);
        }
    }
}