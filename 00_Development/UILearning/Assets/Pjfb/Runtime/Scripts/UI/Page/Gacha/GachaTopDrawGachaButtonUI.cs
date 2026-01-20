using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Pjfb.UserData;

namespace Pjfb.Gacha
{
    public class GachaTopDrawGachaButtonUI : MonoBehaviour
    {
        public bool interactable {get{ return button.interactable; } set{ button.interactable = value; }}
        
        [SerializeField]
        private UIButton button = null;
        [SerializeField]
        private TextMeshProUGUI prizeCountText = null;
        
        [SerializeField]
        private UIBadgeBalloon discountBalloonUI = null;

        [SerializeField]
        private UIBadgeBalloon rushBalloonUI = null;
        [SerializeField]
        private UIBadgeBalloon executeLimitBalloonUI = null;
        [SerializeField]
        private UIBadgeBalloon descriptionLimitBalloonUI = null;
        [SerializeField]
        private UIBadgeBalloon pointAlternativeBalloonUI = null;
        
        // 仮想ポイント
        [SerializeField] private GachaRequiredPointView alternativeView;
        // 通常のポイント
        [SerializeField] private GachaRequiredPointView normalPointView;
        
        [SerializeField]
        private GameObject disableCover = null;


        [SerializeField]
        private CanvasGroup[] BalloonCanvases = null;

        Sequence _balloonChangeAnimationSequence = null;
        DateTime rushEndAt = default;
        
        //クロスフェイド用変数
        int viewBalloonIndex = default;
        CanvasGroup firstView = null;
        CanvasGroup nextView = null;

        /// <summary>データセット</summary>
        public void SetData(GachaCategoryData data)
        {
            // 非活性状態
            disableCover.SetActive(!data.SettingData.IsEnable);
            button.interactable = data.SettingData.IsEnable;
            
            // ガチャ回数
            prizeCountText.text = string.Format(StringValueAssetLoader.Instance[GachaUtility.PrizeCountStringKey], data.PrizeCount);

            // 仮想ポイントのデータ
            AlternativePointData alternativeData = GachaUtility.GetPointAlternative(data.PointId, data.GachaCategoryId);
            
            // 仮想ポイントを使えるか
            bool isUsableAlternative = GachaUtility.IsUsablePointAlternative(data.PointId, data);
            
            // nullなら通常ポイントの表示
            if(alternativeData == null)
            {
                alternativeView.gameObject.SetActive(false);
                normalPointView.gameObject.SetActive(true);
                normalPointView.SetView(data.PointId, data.Price);
            }
            else
            {
                // 仮想ポイントは使用期限内かつポイントが１以上所持しているなら表示
                if (isUsableAlternative)
                {
                    alternativeView.gameObject.SetActive(true);
                    // 仮想ポイントの所持数が多いと必要数を超えるので最大値を設ける
                    long value = Math.Min(alternativeData.UserData.value, data.Price);
                    alternativeView.SetView(alternativeData.UserData.pointId, value);
                }
                // 条件を満たさないなら非表示
                else
                {
                    alternativeView.gameObject.SetActive(false);
                }

                // 通常のポイントは仮想ポイントがガチャ実行必要数に達していないまたは仮想ポイントが使用できないなら表示
                if (alternativeData.UserData.value < data.Price || !isUsableAlternative)
                {
                    normalPointView.gameObject.SetActive(true);
                    // 仮想ポイントを使えるなら使える仮想ポイントを引いた個数、使えない場合は必要数を表示
                    long value = isUsableAlternative ? data.Price - alternativeData.UserData.value : data.Price;
                    normalPointView.SetView(data.PointId, value);
                }
                // 仮想ポイントが必要数に達しているなら表示しない
                else
                {
                    normalPointView.gameObject.SetActive(false);
                }
            }

            //Rush
            if( data.RushData != null && data.RushData.rushId != 0 && !data.RushData.IsHiddenRush()) {
                var now = AppTime.Now;
                if( data.RushData.expiredAt > now ) {
                    rushBalloonUI.gameObject.SetActive(true);
                    rushBalloonUI.SetText(StringValueAssetLoader.Instance["gacha.rush_chance"]);
                    rushEndAt = data.RushData.expiredAt;
                }
            } else {
                rushBalloonUI.gameObject.SetActive(false);
            }

            descriptionLimitBalloonUI.SetText("");
            descriptionLimitBalloonUI.gameObject.SetActive(false);

            UpdateBallonView( data );
            
        }
        

        public void SetCountText(long val){
            prizeCountText.text = string.Format(StringValueAssetLoader.Instance[GachaUtility.PrizeCountStringKey], val);
        }

        public void SetDescriptionBalloonText(string text){
            descriptionLimitBalloonUI.gameObject.SetActive( !string.IsNullOrEmpty(text) );
            descriptionLimitBalloonUI.SetText( text );
            PlayBalloonChangeAnimation();
        }


        public void Update(){
            UpdateRushView();
        }

        public void UpdateBallonView( GachaCategoryData data ){
            
            // 割引情報がある
            if(data.EnableDiscount) {
                // バルーン表示
                discountBalloonUI.gameObject.SetActive(data.DiscountData.IsEnable);
                // 割引テキスト
                discountBalloonUI.SetText(data.DiscountData.Description);
            } else {
                discountBalloonUI.gameObject.SetActive(false);
            }
            
            //残り回数
            if( data.SettingData.ExecuteLimitPersonal != 0 ) {
                executeLimitBalloonUI.gameObject.SetActive(true);
                var canDrawCount =  (data.SettingData.ExecuteLimitPersonal - data.SettingData.ExecuteCount) / data.PrizeCount;
                var text = string.Format(StringValueAssetLoader.Instance["gacha.execute_limit"], canDrawCount );
                executeLimitBalloonUI.SetText(text);
            } else {
                executeLimitBalloonUI.gameObject.SetActive(false);
            }
            
            //仮想ポイントが使用可能ならバルーンを表示する
            if (GachaUtility.IsUsablePointAlternative(data.PointId, data))
            {
                pointAlternativeBalloonUI.gameObject.SetActive(true);
                string text = StringValueAssetLoader.Instance["gacha.usable_alternativepoint"];
                pointAlternativeBalloonUI.SetText(text);
            }
            else
            {
                pointAlternativeBalloonUI.gameObject.SetActive(false);
            }

            PlayBalloonChangeAnimation();
        }
        
        
        void UpdateRushView(){
            if( !rushBalloonUI.gameObject.activeSelf ) {
                return;
            }
            if( rushEndAt >= AppTime.Now ) {
                return;
            }

            rushBalloonUI.gameObject.SetActive(false);
            PlayBalloonChangeAnimation();
        }

        /// <summary>
        /// バルーンのクロスフェイドアニメーションを表示
        /// </summary>
        void PlayBalloonChangeAnimation(){
            viewBalloonIndex = 0;

            if( _balloonChangeAnimationSequence != null ) {
                _balloonChangeAnimationSequence.Kill();
                _balloonChangeAnimationSequence = null;
            }

            var activeCount = 0;
            foreach( var balloon in BalloonCanvases ){
                if( balloon.gameObject.activeSelf ) {
                    ++activeCount;
                }
            }

            if( activeCount <= 1 ){
                firstView = FindFirstActiveBalloon();
                if( firstView != null ) {
                    firstView.alpha = 1.0f;
                }
                return;
            }

            foreach( var balloon in BalloonCanvases ){
               balloon.alpha = 0.0f;
            }

            //複数表示する場合のみアニメーションする
            firstView = FindFirstActiveBalloon();
            viewBalloonIndex = 0;
            foreach( var balloon in BalloonCanvases ){
                if( balloon == firstView ) {
                    break;
                }
                viewBalloonIndex++;
            }
            
            nextView = FindNextActiveBalloon();
            firstView.alpha = 1.0f;
            nextView.alpha = 0.0f;

            var fadeTime = 0.5f;
            var intervalTime = 1.5f;
            _balloonChangeAnimationSequence = DOTween.Sequence();
            _balloonChangeAnimationSequence.AppendInterval(intervalTime);
            _balloonChangeAnimationSequence.Append( DOTween.To( ()=> 1.0f, (x)=>{ firstView.alpha = x; }, 0.0f, fadeTime) )
            .Join( DOTween.To( ()=> 0.0f, (x)=>{ nextView.alpha = x; }, 1.0f, fadeTime) );
            _balloonChangeAnimationSequence.AppendCallback( ()=>{
                firstView = nextView;
                nextView = FindNextActiveBalloon();
                firstView.alpha = 1.0f;
                nextView.alpha = 0.0f;
            } );
        
            _balloonChangeAnimationSequence.SetLoops(-1);
            _balloonChangeAnimationSequence.Play();
        }

        /// <summary>
        /// 最初にアクティブになっているバルーンの検索、取得
        /// </summary>
        CanvasGroup FindFirstActiveBalloon(){
            foreach( var balloon in BalloonCanvases ){
                if( balloon.gameObject.activeSelf ) {
                    return balloon;
                }
            }
            return null;
        }

        /// <summary>
        /// 次にアクティブになるバルーンの検索、取得
        /// 内部でindexを進める
        /// </summary>
        CanvasGroup FindNextActiveBalloon(){
            for( int i=0; i<BalloonCanvases.Length; ++i ){
                viewBalloonIndex++;
                if( viewBalloonIndex >= BalloonCanvases.Length ) {
                    viewBalloonIndex = 0;
                }
                if( BalloonCanvases[viewBalloonIndex].gameObject.activeSelf ){
                    return BalloonCanvases[viewBalloonIndex];
                }
            }
            return null;
        }

        void OnDestroy(){
            if( _balloonChangeAnimationSequence != null ) {
                _balloonChangeAnimationSequence.Kill();
                _balloonChangeAnimationSequence = null;
            }
        }

    }
}
