using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableTrainerSellAPIRequest : AppAPIRequestBase<CharaVariableTrainerSellAPIPost, CharaVariableTrainerSellAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableTrainerSellAPIResponse response ) {
        }
    }
}
