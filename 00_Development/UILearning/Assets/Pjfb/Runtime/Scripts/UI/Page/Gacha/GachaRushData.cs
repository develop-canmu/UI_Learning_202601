using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.Gacha
{
    public class GachaRushData {
        public static readonly long HiddenEffectNumber = -1;

        public long gachaCategoryId = 0; // ガチャカテゴリ
        public long rushId{get; private set;} = 0; // ガチャ確変マスタID
		public DateTime expiredAt{get; private set;}  = DateTime.MinValue; // 有効期限
		public long effectNumber{get; private set;}  = 0; // 演出番号
        public long imageNumber{get; private set;}  = 0; // （nativeタイトル用）rush時に表示する画像などの設定
        public long isFinished{get; private set;}  = 0; // 終了したかどうか（ガチャ実施時に、そのガチャに紐づく確変が「なかった場合」1の情報を返す） 0 => 終了してない、 1 => 終了

        
        public GachaRushData(TopRush rush) {
            rushId = rush.mGachaRushId;
            if( !string.IsNullOrEmpty(rush.expiredAt) ) {
                expiredAt = DateTime.Parse(rush.expiredAt);
            }
            effectNumber = rush.effectNumber;
            imageNumber = rush.imageNumber;
        }

        public GachaRushData(RushRushCategoryInfo rush) {
            gachaCategoryId = rush.mGachaCategoryId;
            rushId = rush.mGachaRushId;
            if( !string.IsNullOrEmpty(rush.expiredAt) ) {
                expiredAt = DateTime.Parse(rush.expiredAt);
            }
            effectNumber = rush.effectNumber;
            imageNumber = rush.imageNumber;
            isFinished = rush.isFinished;
        }

        public GachaRushData(GachaRushData rush) {
            gachaCategoryId = rush.gachaCategoryId;
            rushId = rush.rushId;
            expiredAt = rush.expiredAt;
            effectNumber = rush.effectNumber;
            imageNumber = rush.imageNumber;
            isFinished = rush.isFinished;
        }

        public bool IsHiddenRush()
        {
            return effectNumber == HiddenEffectNumber;
        }

    }
}