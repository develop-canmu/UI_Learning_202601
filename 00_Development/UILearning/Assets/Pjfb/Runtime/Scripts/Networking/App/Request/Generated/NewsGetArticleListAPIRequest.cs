//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
お知らせ記事一覧を取得

デバッグユーザ（設定値 debugUMasterIdList に登録されているユーザ）の場合は articleList の内容がステージ環境相当のものになります。
お知らせの表示を本番環境反映前に確認したい場合に便利です。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class NewsGetArticleListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class NewsGetArticleListAPIResponse : AppAPIResponseBase {
		public NewsArticle[] articleList = null; // お知らせ記事情報の配列

   }
      
   public partial class NewsGetArticleListAPIRequest : AppAPIRequestBase<NewsGetArticleListAPIPost, NewsGetArticleListAPIResponse> {
      public override string apiName{get{ return "news/getArticleList"; } }
   }
}