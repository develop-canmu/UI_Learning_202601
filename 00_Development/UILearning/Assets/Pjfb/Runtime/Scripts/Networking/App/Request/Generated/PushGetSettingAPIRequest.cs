//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
プッシュ通知個別ON/OFF設定の取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PushGetSettingAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class PushGetSettingAPIResponse : AppAPIResponseBase {
		public WrapperIntList[] settingList = null; // [mPushId, オンオフ設定] を列挙した2次元配列

   }
      
   public partial class PushGetSettingAPIRequest : AppAPIRequestBase<PushGetSettingAPIPost, PushGetSettingAPIResponse> {
      public override string apiName{get{ return "push/getSetting"; } }
   }
}