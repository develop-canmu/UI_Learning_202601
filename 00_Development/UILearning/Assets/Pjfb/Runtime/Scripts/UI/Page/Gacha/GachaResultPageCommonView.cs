using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using UnityEngine;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Pjfb.Master;
using Pjfb.UserData;


namespace Pjfb.Gacha
{
    public class GachaResultPageCommonView : MonoBehaviour
    {
        
        enum ButtonType : int{
            Back,
            OneMore,
            PendingFix,
            PendingBack,
        }
    
        [Serializable]
        public class CommonView {

            public GachaResultPoint point = null;
            public PossessionItemUi freeGachaPoint = null;
            public UIButton[] buttons = null;
            public GachaRushResultLogoWebTexture rushLogo = null;
            public UIBadgeBalloon executeLimitBalloon = null;
        }

        // 共通ビュー（1回引き用、複数回引き用）
        [SerializeField]
        CommonView[] commonViews = null;

        int prizeLength = 0;
        GachaResultPageData pageData = null;

        public void Init( GachaResultPageData pageData, bool hideButton = true ){
            this.prizeLength = pageData.PrizeList.Length;
            this.pageData = pageData;

            //ポイント設定
            var commonView = GetCommonView();
            if( pageData.IsComebackPage && pageData.PendingData != null ){
                var point = pageData.PendingData.StoreMPointId;
                var pointVal = GachaUtility.FetchPointValue(point);
                commonView.point.UpdateView(point, pointVal);
            } else {
                commonView.point.UpdateView(pageData.ExchangePointId, pageData.BeforePoint, pageData.AfterPoint);
            }
            
            // 代替ポイントの表示設定
            commonView.freeGachaPoint.gameObject.SetActive(false);
            if (this.pageData.GachaCategoryData != null)
            {
                if (this.pageData.GachaCategoryData.FreeGachaPointData != null)
                {
                    GachaSubPointData subPointData = pageData.GachaCategoryData.FreeGachaPointData;
                    long afterFreePointVal = GachaUtility.FetchPointValue(subPointData.subPointId);
                    commonView.freeGachaPoint.gameObject.SetActive(true);
                    commonView.freeGachaPoint.SetAfterCount(subPointData.subPointId, subPointData.value, afterFreePointVal);
                }
            }
            //もう一度引くバルーン表示
            commonView.executeLimitBalloon.gameObject.SetActive(false);
            UpdateBalloon( commonView );
            
            
            //ボタン非表示
            if (hideButton)
            {
                HideButtonAll();
            }
        }

        /// <summary>入手アイテム数に応じてviewを取得する</summary>
        public CommonView GetCommonView()
        {
            return prizeLength > 1 ? commonViews[1] : commonViews[0];
        }

        public void HideButtonAll(){
            //ボタン非表示
            var commonView = GetCommonView();
            foreach( var button in commonView.buttons ){
                button.gameObject.SetActive(false);
            }
        }

        /// <summary>ボタンの表示設定</summary>
        public void ViewResultButtons(){
            // 更新先出し分け
            var commonView = GetCommonView();
            commonView.buttons[(int)ButtonType.PendingFix].gameObject.SetActive(false);
            commonView.buttons[(int)ButtonType.PendingBack].gameObject.SetActive(false);
            commonView.buttons[(int)ButtonType.Back].gameObject.SetActive(true);
            commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(true);

            if( pageData.GachaCategoryData == null ) {
                commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(false);
                return;
            }

            if( pageData.GachaCategoryData.SettingData.GachaType == GachaType.Ticket ) {
                long pointVal =  GachaUtility.FetchPointValue(pageData.GachaCategoryData.PointId); 
                long needVal = pageData.GachaCategoryData.Price;
                if( needVal > pointVal ){
                    commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(false);
                }
            } else if( pageData.GachaCategoryData.SettingData.IsBoxGacha ){
                var contentCount = pageData.GachaCategoryData.SettingData.BoxContentCount;
                var gachaCount = pageData.GachaCategoryData.GachaCount;
                commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive( contentCount >= gachaCount );
            } else if( pageData.GachaCategoryData.CanRetryOnce  ) {
                commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(true);
                commonView.buttons[(int)ButtonType.PendingFix].gameObject.SetActive(false);
            } else {
                commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(false);
            }
        }

        /// <summary>保留用ボタンの表示設定</summary>
        public void ViewPendingButtons(){
            // 更新先出し分け
            var commonView = GetCommonView();
            commonView.buttons[(int)ButtonType.PendingFix].gameObject.SetActive(true);
            commonView.buttons[(int)ButtonType.PendingBack].gameObject.SetActive(true);
            commonView.buttons[(int)ButtonType.Back].gameObject.SetActive(false);
            commonView.buttons[(int)ButtonType.OneMore].gameObject.SetActive(false);
        }


        void UpdateBalloon( CommonView commonView ){
            commonView.executeLimitBalloon.gameObject.SetActive(false);
            if( pageData.GachaCategoryData == null || pageData.GachaCategoryData.SettingData == null ) {
                return;
            }
            
            // 実行上限なし
            var settingData = pageData.GachaCategoryData.SettingData;
            if( settingData.ExecuteLimitPersonal == 0 ) {
                return;
            }

            // 実行上限に達している
            if( settingData.ExecuteLimitPersonal <= settingData.ExecuteCount  ) {
                return;
            }

            commonView.executeLimitBalloon.gameObject.SetActive(true);
            // 残りの引ける回数を算出
            var canDrawCount =  (settingData.ExecuteLimitPersonal - settingData.ExecuteCount) / pageData.GachaCategoryData.PrizeCount;
            var text = string.Format(StringValueAssetLoader.Instance["gacha.execute_limit"], canDrawCount );
            commonView.executeLimitBalloon.SetText(text);
        }

    }
}
