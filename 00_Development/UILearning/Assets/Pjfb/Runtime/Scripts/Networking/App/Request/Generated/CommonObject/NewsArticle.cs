//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NewsArticle {
		public string url = ""; // 記事URL
		public string category = ""; // カテゴリ（記事一覧でのアイコン表示やタブ分けに使用する）1: カテゴリなし（便宜的に存在するが、お知らせ記事は全ていずれかのカテゴリが指定される想定）1001: ガチャ1002: イベント1003: お得情報1004: お知らせ1005: キャンペーン2001: 重要2002: 不具合2003: メンテナンス2004: アップデート
		public string title = ""; // 記事タイトル
		public string body = ""; // 記事本文（疑似HTML）
		public string imagePath = ""; // 記事のアイキャッチ画像のパス（アイキャッチ画像が不要な場合は空文字列が入る）
		public string startAt = ""; // 記事公開開始日時

   }
   
}