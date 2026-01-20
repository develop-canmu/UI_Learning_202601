using System.Text;
using Pjfb.Gacha;
using Pjfb.Networking.API;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Networking.App.Request {
    
    public partial class GachaExecuteAPIRequest : AppAPIRequestBase<GachaExecuteAPIPost, GachaExecuteAPIResponse>
    {
        /// <summary>
        /// APIを受信した
        /// </summary>
        protected override void OnAPIReceivedWithResponseType( GachaExecuteAPIResponse response ) {
        }
    }
}
