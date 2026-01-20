using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaBoxHighlightRewardConfirmModal : ModalWindow
    {
        public class Param{
            public BoxInfo boxInfo = null;
        }

        [SerializeField]
        ScrollDynamic _scroll = null;
        [SerializeField]
        TextMeshProUGUI _cautionText = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            var param = (Param)args;
            var boxInfo = param.boxInfo;
            long loopLap = -1;

            var items = new List<GachaBoxHighlightScrollDynamicItemData>();
            var currentLap = boxInfo.currentBox.boxNumber;
            //現在の週目を一番上に
            foreach( var box in boxInfo.boxList ){
                if( box.boxNumberMin <= currentLap && currentLap <= box.boxNumberMax ) {
                    AddCurrentListItems(box, boxInfo.currentBox, ref items);
                    break;
                } 
            }

            //現在の週目以降のものを表示
            foreach( var box in boxInfo.boxList ){
                if( box.boxNumberMax >= GachaUtility.BoxGachaLoopLap ) {
                    loopLap = box.boxNumberMin;
                }

                //現在の週目以降のものは後に表示
                if( currentLap > box.boxNumberMin ) {
                    continue;
                }

                //現在の週目ものはスキップ
                if( box.boxNumberMin <= currentLap && currentLap <= box.boxNumberMax ) {
                    continue;
                }
                
                AddListItems(box, false, false, ref items);
            }
            
            //現在の週目以前のものを表示
            foreach( var box in boxInfo.boxList ){
                //現在の週目ものはスキップ
                if( box.boxNumberMin <= currentLap && currentLap <= box.boxNumberMax ) {
                    continue;
                }
                
                if( currentLap > box.boxNumberMin ) {
                    AddListItems(box, false, true, ref items);
                }
            }

            // 報酬ループの注意文言設定
            if( loopLap > 0 ) {
                _cautionText.gameObject.SetActive(true);
                _cautionText.text = string.Format(StringValueAssetLoader.Instance["gacha.box_loop_caution"], loopLap );
            } else {
                _cautionText.gameObject.SetActive(false);
            }

            _scroll.SetItems(items);
        }
        
        
        public void OnClickClose()
        {
            Close();
        }

        // 指定されたBoxを今引いているボックスとして目玉商品をリストに追加
        void AddCurrentListItems( BoxBox box, BoxCurrentBox currentBox, ref List<GachaBoxHighlightScrollDynamicItemData> items )
        {
            var item = new GachaBoxHighlightScrollDynamicItemData();
            var labelParam = new GachaBoxHighlightRewardLabel.Param();
            labelParam.isCurrent = true;
            labelParam.lapsMin = box.boxNumberMin;
            labelParam.lapsMax = box.boxNumberMax;
            item.labelParam = labelParam;
            items.Add(item);

            foreach( var content in box.contentList ){
                if( content.isFeatured ) {
                    var currentItem = FindCurrentItem(content, currentBox );
                    item = new GachaBoxHighlightScrollDynamicItemData();
                    var rewardParam = new GachaBoxRewardItem.Param();
                    rewardParam.prizeJsonData = new PrizeJsonViewData( content.prizeList[0] );
                    rewardParam.quantity = content.count;
                    rewardParam.isHighlight = true;
                    // 目玉商品がない、または目玉商品の在庫がない場合はグレーアウトする
                    rewardParam.forceCover = currentItem == null ? true : currentItem.count <= 0;
                    rewardParam.isHighlightReword = true;
                    item.scrollItemParam = rewardParam;
                    items.Add(item);
                }
            }
        }

        // 目玉商品をリストに追加
        void AddListItems( BoxBox box, bool isCurrent, bool forceCover, ref List<GachaBoxHighlightScrollDynamicItemData> items )
        {
            var item = new GachaBoxHighlightScrollDynamicItemData();
            var labelParam = new GachaBoxHighlightRewardLabel.Param();
            labelParam.isCurrent = isCurrent;
            labelParam.lapsMin = box.boxNumberMin;
            labelParam.lapsMax = box.boxNumberMax;
            item.labelParam = labelParam;
            items.Add(item);

            foreach( var content in box.contentList ){
                if( content.isFeatured ) {
                    item = new GachaBoxHighlightScrollDynamicItemData();
                    var rewardParam = new GachaBoxRewardItem.Param();
                    rewardParam.prizeJsonData = new PrizeJsonViewData( content.prizeList[0] );
                    rewardParam.quantity = content.count;
                    rewardParam.isHighlight = true;
                    rewardParam.forceCover = forceCover;
                    rewardParam.isHighlightReword = true;
                    item.scrollItemParam = rewardParam;
                    items.Add(item);
                }
            }
        }

        // 現在引いているボックスの中から指定された目玉商品を探す
        BoxBoxContent FindCurrentItem( BoxBoxContent content, BoxCurrentBox currentBox ){
            foreach( var currentContent in currentBox.contentList ){
                var contentPrize = content.prizeList[0];
                var currentContentPrize = currentContent.prizeList[0];
                if( contentPrize.type != currentContentPrize.type ) {
                    continue;
                }

                if( contentPrize.args.mCharaId != currentContentPrize.args.mCharaId ) {
                    continue;
                }

                if( contentPrize.args.mPointId != currentContentPrize.args.mPointId ) {
                    continue;
                }

                if( contentPrize.args.pieceMCharaId != currentContentPrize.args.pieceMCharaId ) {
                    continue;
                }

                if( contentPrize.args.variableTrainerMCharaId != currentContentPrize.args.variableTrainerMCharaId ) {
                    continue;
                }

                return currentContent;
            }

            CruFramework.Logger.LogError("not find currentItem");
            return null;
        }

    }
}
