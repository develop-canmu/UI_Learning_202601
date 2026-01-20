using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaExecuteConfirmModal : GachaExecuteConfirmModalBase
    {
        [SerializeField]
        private TextMeshProUGUI periodText = null;
        
        [SerializeField]
        private PossessionItemUi alternativePointUi = null;
        
        [SerializeField]
        private PossessionItemUi mainPointUi = null;
        
        private long usingPointAlternativeId = 0;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await base.OnPreOpen(args, token);
            
            /* パターン 
             * 1.仮想ポイントを所持していて充足する場合
             * 2.仮想ポイントを所持していて充足しない・所持ポイントは充足する場合
             * 3.仮想ポイントを所持していない場合
             */
            // 優先度：無料分(discount) ＞ ガチャチケット(subPoint) ＞ 仮想ポイント(pointAlternative) ＞ブルージェム
            
            var mPoint = MasterManager.Instance.pointMaster.FindData(CategoryData.PointId);

            var uPointValue = GachaUtility.FetchPointValue(CategoryData.PointId);
            var uPointAlternative = GachaUtility.GetPointAlternative(mPoint.id, CategoryData.GachaCategoryId);

            // 仮想ポイントを使用できるか
            var isUsableAlternative = GachaUtility.IsUsablePointAlternative(mPoint.id, CategoryData);
            
            if (isUsableAlternative && uPointAlternative.UserData.value >= CategoryData.Price)
            {
                // 仮想ポイントのみを使用する
                // 使用する仮想ポイントIDをキャッシュ
                usingPointAlternativeId = uPointAlternative.UserData.pointId;
                
                alternativePointUi.gameObject.SetActive(true);
                alternativePointUi.SetAfterCount(uPointAlternative.UserData.pointId, uPointAlternative.UserData.value, uPointAlternative.UserData.value - CategoryData.Price);
                // 使用期限を反映
                SetPeriodText(uPointAlternative.AlternativeMasterObject);
                
                mainPointUi.gameObject.SetActive(false);
                
                // メッセージ
                UpdateMessage(uPointAlternative.UserData.pointId, CategoryData.Price, CategoryData.PrizeCount);
            }
            else if (isUsableAlternative)
            {
                // 仮想ポイントと通常ポイントを併用する
                var priceRemain = CategoryData.Price - uPointAlternative.UserData.value;
                // 使用する仮想ポイントIDをキャッシュ
                usingPointAlternativeId = uPointAlternative.UserData.pointId;

                alternativePointUi.gameObject.SetActive(true);
                alternativePointUi.SetAfterCount(uPointAlternative.UserData.pointId, uPointAlternative.UserData.value, 0);
                // 使用期限を反映
                SetPeriodText(uPointAlternative.AlternativeMasterObject);
                
                mainPointUi.gameObject.SetActive(true);
                mainPointUi.SetAfterCount(CategoryData.PointId, uPointValue, uPointValue - priceRemain);
                
                // メッセージ
                UpdateMessage(mPoint.id, priceRemain, CategoryData.PrizeCount, uPointAlternative.UserData.pointId, uPointAlternative.UserData.value);
            }
            else
            {
                // 仮想ポイントを使用できない（割引中・チケットを使用・未所持）
                alternativePointUi.gameObject.SetActive(false);
                
                // メッセージ
                mainPointUi.SetAfterCount(CategoryData.PointId, uPointValue, uPointValue - CategoryData.Price);
                UpdateMessage(mPoint.id, CategoryData.Price, CategoryData.PrizeCount);
            }
        }

        private void UpdateMessage(long mPointId, long price, long prizeCount)
        {
            var mPoint = MasterManager.Instance.pointMaster.FindData(mPointId);
            messageText.text = string.Format(StringValueAssetLoader.Instance["gacha.draw_gacha_confirm"], mPoint.name, price, mPoint.unitName, prizeCount);
        }

        private void UpdateMessage(long mPointId, long price, long prizeCount, long mPointAlternativeId, long mPointAlternativeRemain)
        {
            var mPoint = MasterManager.Instance.pointMaster.FindData(mPointId);
            var mPointAlternative = MasterManager.Instance.pointMaster.FindData(mPointAlternativeId);
            
            var altText = string.Format(StringValueAssetLoader.Instance["gacha.draw_gacha_confirm.with_alternative"], mPointAlternative.name, mPointAlternativeRemain, mPointAlternative.unitName);
            var mainText = string.Format(StringValueAssetLoader.Instance["gacha.draw_gacha_confirm"], mPoint.name, price, mPoint.unitName, prizeCount);
            messageText.text = $"{altText}{mainText}";
        }
        
        /// <summary> 使用期限を反映する </summary>
        private void SetPeriodText(PointAlternativeMasterObject pointAlternativeMaster)
        {
            periodText.text = pointAlternativeMaster.endAt.TryConvertToDateTime().ToString(StringValueAssetLoader.Instance["point.expire_date_format"]);
        }
        
        /// <summary>
        /// uGUI
        /// ガチャ実行
        /// </summary>
        public void OnClickPositiveButton()
        {
            ExecuteGachaAsync().Forget();
        }
        
        private async UniTask ExecuteGachaAsync()
        {
            //Rushの時間が過ぎていたらローカルデータ更新する
            if( CategoryData.RushData != null && CategoryData.RushData.rushId != 0 ) {
                var now = AppTime.Now;
                if( CategoryData.RushData.expiredAt <= now ) {
                    CategoryData.ClearRushData();
                }
            }

            // 割引ID
            var discountId = CategoryData.EnableDiscount ? CategoryData.DiscountData.GachaDiscountId : 0;
            // 代替ポイントID
            var subPointId = CategoryData.EnableSubPoint ? CategoryData.SubPointData.id : 0;
            
            int bundleCount = 1;
            
            // API

            //Token取得
            var token = await Pjfb.Networking.App.APIUtility.ConnectOneTimeTokeRequest();
            
            //ガチャ実行
            var request = new GachaExecuteAPIRequest();
            var post = new GachaExecuteAPIPost();
            post.mGachaCategoryId = CategoryData.GachaCategoryId;
            post.mGachaSettingId = CategoryData.SettingData.GachaSettingId;
            post.gachaCount = CategoryData.GachaCount;
            post.bundleCount = bundleCount;
            post.mGachaCategoryDiscountId = discountId;
            post.mGachaCategorySubPointId = subPointId;
            if (usingPointAlternativeId > 0)
            {
                // 使用する仮想ポイントの配列
                post.mPointIdAlternativeList = new long [] { usingPointAlternativeId };
            }
            request.SetPostData(post);
            request.oneTimeToken = token.oneTimeToken;
            await APIManager.Instance.Connect(request);
            
            var response = request.GetResponseData();
            
            //有効状態更新
            CategoryData.SettingData.UpdateEnable(response.enabled);

            // 割引残使用回数デクリメント
            if(CategoryData.EnableDiscount)
            {
                CategoryData.DiscountData.DecrementRestCount();
            }
            
            // 実行回数更新
            CategoryData.SettingData.UpdateExecuteCount(response.executeCount, response.executeLimitPersonal);

            //Boxガチャの場合はBox内の残りアイテム数を更新
            if( CategoryData.SettingData.IsBoxGacha ) {
                CategoryData.SettingData.UpdateBoxContentCount( CategoryData.SettingData.BoxContentCount - CategoryData.GachaCount );
            }

            // 閉じた時に受け取るデータセット
            SetCloseParameter( response );
            Close();
        }
    }
}
