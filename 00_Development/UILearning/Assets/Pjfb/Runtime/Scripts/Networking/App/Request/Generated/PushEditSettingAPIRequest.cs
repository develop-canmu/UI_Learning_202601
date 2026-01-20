//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
プッシュ通知個別ON/OFF設定の変更

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PushEditSettingAPIPost : AppAPIPostBase {
		public WrapperIntList[] settingList = null; // [mPushId, オンオフ設定] を列挙した2次元配列

   }

   [Serializable]
   public class PushEditSettingAPIResponse : AppAPIResponseBase {

   }
      
   public partial class PushEditSettingAPIRequest : AppAPIRequestBase<PushEditSettingAPIPost, PushEditSettingAPIResponse> {
      public override string apiName{get{ return "push/editSetting"; } }
   }
}