using Pjfb.Networking.API;
using Pjfb.Storage;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserCreateAPIRequest : AppAPIRequestBase<UserCreateAPIPost, UserCreateAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserCreateAPIResponse response ) {
            LocalSaveUtility.CreateImmutableSaveData(response.appToken);
        }
    }
}
