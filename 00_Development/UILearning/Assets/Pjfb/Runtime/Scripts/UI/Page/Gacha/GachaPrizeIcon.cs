using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using CruFramework.ResourceManagement;
using Pjfb.Master;
using DG.Tweening;


namespace Pjfb.Gacha
{
    public class GachaPrizeIcon : MonoBehaviour
    {
        readonly int playEffectRarity = 3;
        
        readonly string animatorKeyGold = "Gold";
        readonly string animatorKeyNormal = "Normal";

        /// <summary>報酬アイコン</summary>
        // 獲得演出を再生するか
        public bool IsPlayGetEffect => isPlayEffect;
        //開いているか
        public bool IsOpened => isOpened;
        public PrizeJsonView MainView => mainPrizeView;
        public PrizeJsonViewData MainViewData => mainViewData;
        public PrizeJsonViewData[] ViewDataList => viewDataList;
        public GachaPrizeIconPendingInfo Pending => pending;
        public long Rarity => rarity;
        public bool IsNewByServer => isNewByServer;
        
        


        [SerializeField]
        private PrizeJsonView mainPrizeView = null;
        [SerializeField]
        private PrizeJsonView subPrizeView = null;

        [SerializeField]
        private CanvasGroup mainPrizeViewCanvasGroup = null;
        [SerializeField]
        private CanvasGroup subPrizeViewCanvasGroup = null;
        [SerializeField]
        private GameObject ballRoot = null;
        [SerializeField]
        private Animator ballAnimator = null;
        [SerializeField]
        private GameObject goldBall = null;
        [SerializeField]
        private GameObject normalBall = null;
        [SerializeField]
        private GameObject objectRoot = null;
        [SerializeField]
        private GameObject iconRoot = null;
        [SerializeField]
        private GachaPrizeIconPendingInfo pending = null;

        [SerializeField]
        private GachaPrizeIconAnimationController iconAnimation = null;


        
        PrizeJsonViewData[] viewDataList = null;
        PrizeJsonViewData mainViewData = null;

        bool isPlayEffect = false;
        long rarity = 0;
        int subPrizeViewIndex = 0;
        bool isSkip = false;
        bool isOpened = false;
        bool isNewByServer = false;
        CancellationTokenSource animationCancellationSource = null;

        Sequence iconChangeAnimationSequence = null;
        


        public void SetView(PrizeJsonViewData[] dataList,  bool isNewActive, bool isNewByServer, bool isAnimationSkip )
        {
            viewDataList = dataList;
            mainViewData = dataList[0];
            subPrizeViewIndex = 0;
            isSkip = isAnimationSkip;
            isOpened = false;
            this.isNewByServer = isNewByServer;
            UpdatePrizeView(mainPrizeView, mainViewData);
            mainPrizeView.SetNewIconActive(isNewActive);
            UpdateSubPrizeView();

            mainPrizeView.gameObject.SetActive(false);
            subPrizeView.gameObject.SetActive(false);
            
            rarity = GachaUtility.GetGachaPrizeRarity(mainViewData);

            ballRoot.gameObject.SetActive(true);
            isPlayEffect = IsPlayEffect(mainViewData, isNewActive);
            var isGoldBallRarity = GachaUtility.IsGoldBallRarity(rarity);
            goldBall.SetActive( isGoldBallRarity );
            normalBall.SetActive( !isGoldBallRarity );
            HidePendingView();
            if( isAnimationSkip ) {
                UpdateViewOpenFinishedIdle();
            }
        }

        /// <summary>
        /// カードオープンアニメーション
        /// </summary>
        public async UniTask Open( CharacterGetEffect getEffect, bool isForcePlay, ResourcesLoader resourcesLoader){
            if( isSkip && !isForcePlay ) {
                UpdateViewOpenFinishedIdle();
                await UniTask.Delay(1);
                return;
            }

            animationCancellationSource = new CancellationTokenSource();
            
            try{
                //キャラクターアニメーション
                if( isPlayEffect || isForcePlay ) {
                    await getEffect.PlayAsync(mainViewData.Id, resourcesLoader);
                } 
                
                var key = GachaUtility.IsGoldBallRarity(rarity) ? animatorKeyGold : animatorKeyNormal;
                ballAnimator.SetTrigger(key);
                await AnimatorUtility.WaitStateAsync(ballAnimator, key, animationCancellationSource.Token);
                UpdateViewOpenFinishedIdle();
            } catch( System.OperationCanceledException) {
                UpdateViewOpenFinishedIdle();
                animationCancellationSource = null;
            } finally{
                //再生終了したらAnimatorからパラメータを上書きされた内容にfalseにする
                if( ballAnimator != null ) {
                    ballAnimator.enabled = false;
                }
                isOpened = true;
            }
        }

        /// <summary>
        /// アイコンの変更アニメーション再生
        /// </summary>
        public void PlayIconChangeAnimation(){
            //報酬がひとつだけだったらアニメーションしない
            if( viewDataList.Length <= 1 ) {
                return;
            }

            StopIconChangeAnimation();

            mainPrizeView.gameObject.SetActive(true);
            subPrizeView.gameObject.SetActive(true);
            mainPrizeViewCanvasGroup.alpha = 1.0f;
            subPrizeViewCanvasGroup.alpha = 0.0f;

            var fadeTime = 0.5f;
            var intervalTime = 1.5f;
            iconChangeAnimationSequence = DOTween.Sequence();
            
            iconChangeAnimationSequence.Append( mainPrizeViewCanvasGroup.DOFade(0.0f, fadeTime) ).
            Join( subPrizeViewCanvasGroup.DOFade(1.0f, fadeTime) );
            iconChangeAnimationSequence.AppendInterval(intervalTime);
            iconChangeAnimationSequence.Append( mainPrizeViewCanvasGroup.DOFade(1.0f, fadeTime) ).
            Join( subPrizeViewCanvasGroup.DOFade(0.0f, fadeTime) )
            .AppendCallback(()=>{ UpdateSubPrizeView(); });
            iconChangeAnimationSequence.AppendInterval(intervalTime);
            iconChangeAnimationSequence.SetLoops(-1);

            iconChangeAnimationSequence.Play();
        }

        public void ResetCardsChangeIconAnimation(){
            
            mainPrizeViewCanvasGroup.alpha = 1.0f;
            mainPrizeViewCanvasGroup.transform.localScale = Vector3.one;
            subPrizeViewIndex = 0;
            UpdateSubPrizeView();
        }

        public void StopIconChangeAnimation(){
            if( iconChangeAnimationSequence != null ) {
                iconChangeAnimationSequence.Kill();
                iconChangeAnimationSequence = null;
            }
        }


        /// <summary>
        /// 演出スキップ
        /// </summary>
        public void Skip(){
            isSkip = true;
            if( animationCancellationSource != null ) {
                animationCancellationSource.Cancel();
            }
        }

        /// <summary>
        /// 保留情報を非表示にする
        /// </summary>
        public void HidePendingView(){
            pending.HidePendingView();
            iconAnimation.SetEnable();
        }

        /// <summary>
        /// 保留表示に更新
        /// </summary>
        public void UpdatePendingView( GachaPendingFrameData pendingData, System.Action<GachaPrizeIcon> onClickPending ){
            pending.UpdatePendingView( this, pendingData, onClickPending );
            if( pending.canRetry ) {
                iconAnimation.SetEnable();
            } else {
                iconAnimation.SetDisable();
            }
            
        }

        public void UpdatePendingView( Pjfb.Networking.App.Request.GachaPendingFrame pendingData, System.Action<GachaPrizeIcon> onClickPending ){
            pending.UpdatePendingData( this, pendingData, onClickPending);
            pending.RefreshView();
            if( pending.canRetry ) {
                iconAnimation.SetEnable();
            } else {
                iconAnimation.SetDisable();
            }
        }
        
        /// <summary>
        /// カードオープン終了待機状態にする
        /// </summary>
        public void UpdateViewOpenFinishedIdle(){
            ballAnimator.enabled = false;
            foreach( Transform child in objectRoot.transform ){
                child.gameObject.SetActive(false);
            }
            iconRoot.SetActive(true);
            mainPrizeView.gameObject.SetActive(true);
            mainPrizeViewCanvasGroup.alpha = 1.0f;
            mainPrizeViewCanvasGroup.transform.localScale = Vector3.one;
        }


        public void SetNewIconActive( bool isActive ){
           mainPrizeView.SetNewIconActive(isActive);
        }



        void OnDestroy(){
            if( iconChangeAnimationSequence != null ) {
                iconChangeAnimationSequence.Kill();
            }
            iconChangeAnimationSequence = null;
        }   

        /// <summary>
        /// 演出再生するか
        /// </summary>
        public bool IsPlayEffect( PrizeJsonViewData viewData, bool isFirstNew ){
            
            if( viewData.ItemIconType == ItemIconType.Character ) {
                if( isFirstNew ) {
                    return true;
                }
                
                return rarity >= playEffectRarity;
            }

            return false;
        }

        void UpdatePrizeView( PrizeJsonView view, PrizeJsonViewData data ){
            view.SetView(data);
            if( view.IconType == ItemIconType.Character ) {
                var characterIcon = view.GetIcon<CharacterIcon>();
                characterIcon.SetActiveLv(false);
            }
        }

        void UpdateSubPrizeView( ){
            if( viewDataList.Length <= 1 ) {
                return;
            }
            subPrizeViewIndex++;
            if( subPrizeViewIndex >= viewDataList.Length  ) {
                subPrizeViewIndex = 1;
            }

            if( subPrizeViewIndex <= 0 ) {
                subPrizeViewIndex = viewDataList.Length-1;
            }
            var viewData = viewDataList[subPrizeViewIndex];
            
            UpdatePrizeView( subPrizeView, viewData );
        }


    }
}