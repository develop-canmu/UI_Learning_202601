using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableTrainerLockAPIRequest : AppAPIRequestBase<CharaVariableTrainerLockAPIPost, CharaVariableTrainerLockAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableTrainerLockAPIResponse response ) {
        }
    }
}
