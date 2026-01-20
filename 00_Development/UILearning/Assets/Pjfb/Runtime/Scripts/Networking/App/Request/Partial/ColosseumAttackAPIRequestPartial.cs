using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class ColosseumAttackAPIRequest : AppAPIRequestBase<ColosseumAttackAPIPost, ColosseumAttackAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( ColosseumAttackAPIResponse response ) {
        }
    }
}
