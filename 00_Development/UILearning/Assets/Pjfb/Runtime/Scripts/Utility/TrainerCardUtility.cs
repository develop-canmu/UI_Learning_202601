using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Utility
{
    public static class TrainerCardUtility
    {
        /// <summary>プロフィール情報取得API</summary>
        public static async UniTask<UserProfileUserStatus> UserGetProfileAPI(long targetUMasterId)
        {
            UserGetProfileAPIRequest request = new UserGetProfileAPIRequest();
            UserGetProfileAPIPost post = new UserGetProfileAPIPost{targetUMasterId = targetUMasterId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData().user;
        }
        
        /// <summary>イイネ</summary>
        public static async UniTask<UserSendProfileLikeAPIResponse> UserSendProfileLikeAPI(long targetMasterId)
        {
            UserSendProfileLikeAPIRequest request = new UserSendProfileLikeAPIRequest();
            UserSendProfileLikeAPIPost post = new UserSendProfileLikeAPIPost();
            post.targetUMasterId = targetMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        
        /// <summary>更新APIを送る</summary>
        private static async UniTask<UserProfileUserStatus> UpdateTrainerCard(UserUpdateProfileAPIPost postData)
        {
            UserUpdateProfileAPIRequest request = new UserUpdateProfileAPIRequest();
            request.SetPostData(postData);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData().user;
        }
        
        /// <summary>プロフィールデータの更新</summary>
        private static async UniTask<UserProfileUserStatus> UpdateTrainerCardProfile(UserProfileCardData profileData)
        {
            UserUpdateProfileAPIPost postData = new UserUpdateProfileAPIPost();
            postData.profileData = profileData;
            return await UpdateTrainerCard(postData);
        }
        
        /// <summary>アイコンの更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateTrainerCardIcon(long iconId)
        {
            UserUpdateProfileAPIPost postData = new UserUpdateProfileAPIPost();
            postData.mIconId = iconId;
            return await UpdateTrainerCard(postData);
        }
        
        /// <summary>自己紹介の更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateTrainerCardIntroduction(string introduction)
        {
            UserUpdateProfileAPIPost postData = new UserUpdateProfileAPIPost();
            postData.wordIntroduction = introduction;
            return await UpdateTrainerCard(postData);
        }

        /// <summary>トレーナーカード着せ替えの更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateProfileFrame(long mProfileFrameId)
        {
            UserProfileCardData profileData = new UserProfileCardData();
            profileData.mProfileFrameId = mProfileFrameId;
            return await UpdateTrainerCardProfile(profileData);
        }
        
        /// <summary>キャラ背景の更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateProfileBackground(long mProfileBackgroundId)
        {
            UserProfileCardData profileData = new UserProfileCardData();
            profileData.mProfileBackgroundId = mProfileBackgroundId;
            return await UpdateTrainerCardProfile(profileData);
        }
        
        /// <summary>表示キャラの更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateProfileChara(long mProfileCharaId)
        {
            UserProfileCardData profileData = new UserProfileCardData();
            profileData.mProfileCharaId = mProfileCharaId;
            return await UpdateTrainerCardProfile(profileData);
        }
        
        /// <summary>マイバッチの更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateProfileEmblem(long[] mEmblemIdList)
        {
            UserProfileCardData profileData = new UserProfileCardData();
            profileData.mEmblemIdList = mEmblemIdList;
            return await UpdateTrainerCardProfile(profileData);
        }
        
        /// <summary>チャットスタンプの更新</summary>
        public static async UniTask<UserProfileUserStatus> UpdateProfileChatStamp(long[] mChatStampIdList)
        {
            UserProfileCardData profileData = new UserProfileCardData();
            profileData.mChatStampIdList = mChatStampIdList;
            return await UpdateTrainerCardProfile(profileData);
        }
    }
}