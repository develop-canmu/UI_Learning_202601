//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
プレゼントボックスから一括受け取りする

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGiftAllReceiveAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGiftAllReceiveAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugGiftAllReceiveAPIRequest : AppAPIRequestBase<DebugGiftAllReceiveAPIPost, DebugGiftAllReceiveAPIResponse> {
      public override string apiName{get{ return "debug/giftAllReceive"; } }
   }
}