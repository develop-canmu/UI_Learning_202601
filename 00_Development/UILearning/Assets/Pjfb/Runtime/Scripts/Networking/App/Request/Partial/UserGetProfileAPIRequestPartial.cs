using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserGetProfileAPIRequest : AppAPIRequestBase<UserGetProfileAPIPost, UserGetProfileAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserGetProfileAPIResponse response ) {
            UserData.UserDataManager.Instance.user.Update(response);
        }
    }
}
