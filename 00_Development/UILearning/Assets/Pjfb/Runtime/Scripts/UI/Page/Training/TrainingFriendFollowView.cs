using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Community;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class TrainingFriendFollowView : MonoBehaviour
    {
        [SerializeField]
        private CharacterIcon characterIcon = null;
        
        [SerializeField]
        private TMPro.TMP_Text userNameText = null;
        
        [SerializeField]
        private TMPro.TMP_Text loginText = null;
        
        [SerializeField]
        private TMPro.TMP_Text combatPowerText = null;
        
        [SerializeField]
        private OmissionTextSetter combatPowerOmissionTextSetter = null;
        
        [SerializeField]
        private CharacterRankImage rankImage = null;
        
        [SerializeField]
        private UserTitleImage titleImage = null;
        
        private TrainingFriend friend = null;
        
        public void OpenProfile()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(friend.communityUserStatus.uMasterId, false, false);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        
        public void SetView(TrainingFriend friend)
        {
            this.friend = friend;
            // ユーザー名
            userNameText.text = friend.communityUserStatus.name;
            // 称号
            titleImage.SetTexture( friend.communityUserStatus.mTitleId);
            BigValue maxCombatPower = new BigValue(friend.communityUserStatus.maxCombatPower);
            // 戦力
            combatPowerText.text = maxCombatPower.ToDisplayString(combatPowerOmissionTextSetter.GetOmissionData());
            // ランク
            rankImage.SetTexture( StatusUtility.GetRank(CharaRankMasterStatusType.PartyTotal, maxCombatPower ) );
            // ログイン時間
            loginText.text = CommunityManager.GetDateTimeDiffByString(friend.communityUserStatus.lastLogin);
        }
        
        public void SetCharacter(long mCharId, long lv, long liberationLv)
        {
            characterIcon.SetIcon(mCharId, lv, liberationLv);
        }
    }
}