using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class HuntStartAndFinishAPIRequest : AppAPIRequestBase<HuntStartAndFinishAPIPost, HuntStartAndFinishAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( HuntStartAndFinishAPIResponse response ) {
        }
    }
}
