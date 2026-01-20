using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableGetListAPIRequest : AppAPIRequestBase<CharaVariableGetListAPIPost, CharaVariableGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableGetListAPIResponse response ) {
        }
    }
}
