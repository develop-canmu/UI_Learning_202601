using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserSaveFinishedTutorialAPIRequest : AppAPIRequestBase<UserSaveFinishedTutorialAPIPost, UserSaveFinishedTutorialAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserSaveFinishedTutorialAPIResponse response ) {
        }
    }
}
