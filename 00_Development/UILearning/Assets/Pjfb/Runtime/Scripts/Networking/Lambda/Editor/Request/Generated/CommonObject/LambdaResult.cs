//
// This file is auto-generated
//

#if !(UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID)

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class LambdaResult {
		public string log { get; set; } = ""; // 圧縮ログ。サーバー側では基本的には解析せず、プレビューようにクライアントに素通りさせる
		public LambdaSummary summary { get; set; } = null; // バトル結果の要約情報

   }
   
}

#endif