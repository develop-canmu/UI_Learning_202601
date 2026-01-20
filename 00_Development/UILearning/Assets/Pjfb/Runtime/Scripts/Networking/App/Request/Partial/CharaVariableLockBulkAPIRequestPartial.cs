using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableLockBulkAPIRequest : AppAPIRequestBase<CharaVariableLockBulkAPIPost, CharaVariableLockBulkAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableLockBulkAPIResponse response ) {
        }
    }
}
