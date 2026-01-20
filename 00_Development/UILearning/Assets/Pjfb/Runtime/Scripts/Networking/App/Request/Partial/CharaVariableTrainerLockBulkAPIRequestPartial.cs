using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableTrainerLockBulkAPIRequest : AppAPIRequestBase<CharaVariableTrainerLockBulkAPIPost, CharaVariableTrainerLockBulkAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableTrainerLockBulkAPIResponse response ) {
        }
    }
}
