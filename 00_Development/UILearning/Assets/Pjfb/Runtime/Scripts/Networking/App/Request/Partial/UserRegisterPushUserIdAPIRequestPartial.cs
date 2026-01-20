using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class UserRegisterPushUserIdAPIRequest : AppAPIRequestBase<UserRegisterPushUserIdAPIPost, UserRegisterPushUserIdAPIResponse>
    {
        public enum PushType
        {
            OneSignal = 3,
            Fcm = 4,
        }
        
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( UserRegisterPushUserIdAPIResponse response ) {
        }
    }
}
