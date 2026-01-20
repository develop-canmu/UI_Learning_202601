using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class CharaVariableTrainerReLotteryAPIRequest : AppAPIRequestBase<CharaVariableTrainerReLotteryAPIPost, CharaVariableTrainerReLotteryAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( CharaVariableTrainerReLotteryAPIResponse response ) {
        }
    }
}
