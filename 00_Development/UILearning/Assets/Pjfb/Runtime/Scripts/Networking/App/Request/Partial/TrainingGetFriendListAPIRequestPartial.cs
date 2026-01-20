using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class TrainingGetFriendListAPIRequest : AppAPIRequestBase<TrainingGetFriendListAPIPost, TrainingGetFriendListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( TrainingGetFriendListAPIResponse response ) {
        }
    }
}
