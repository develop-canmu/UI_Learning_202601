using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserUpdateNameAPIRequest : AppAPIRequestBase<UserUpdateNameAPIPost, UserUpdateNameAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserUpdateNameAPIResponse response ) {
            UserData.UserDataManager.Instance.user.UpdateUserName(response);
        }
    }
}
