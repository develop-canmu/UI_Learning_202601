using Pjfb.Networking.API;

namespace Pjfb.Networking.App.Request {
    
    public partial class DummyLambdaBattleLogicAPIRequest : AppAPIRequestBase<DummyLambdaBattleLogicAPIPost, DummyLambdaBattleLogicAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( DummyLambdaBattleLogicAPIResponse response ) {
        }
    }
}
