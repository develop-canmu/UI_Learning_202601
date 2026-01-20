//
// This file is auto-generated
//

using System;

namespace Pjfb.Master {
   
   [Serializable]
   [MessagePack.MessagePackObject]
   public partial class MasterSeriesOption {
		[MessagePack.Key(0)]
		public long phaseNumber = 0; // シリーズのフェーズ番号（必須。指定しないと1扱い）
		[MessagePack.Key(1)]
		public bool isLast = false; // シリーズの最後のデータかどうか（デフォルトはfalse）
		[MessagePack.Key(2)]
		public long entryChainCount = 0; // シリーズの最後ではない場合、成績上位者（ギルド）のエントリーを次のシーズンに引き継ぐ。その際の人数（デフォルトは0）。シーズンの最後のデータに対しては設定しなくても良い
		[MessagePack.Key(3)]
		public long imageNumber = 0; // 画像番号
		[MessagePack.Key(4)]
		public long bannerImageId = 0; // バナー画像番号

   }
   
}