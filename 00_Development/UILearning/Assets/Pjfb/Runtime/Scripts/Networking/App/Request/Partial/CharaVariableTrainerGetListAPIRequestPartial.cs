using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableTrainerGetListAPIRequest : AppAPIRequestBase<CharaVariableTrainerGetListAPIPost, CharaVariableTrainerGetListAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableTrainerGetListAPIResponse response ) {
        }
    }
}
