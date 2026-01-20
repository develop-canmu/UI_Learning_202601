using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Pjfb.Master;
using DG.Tweening;


namespace Pjfb.Rivalry
{
    public class RivalryRewardIcon : MonoBehaviour
    {
        readonly string animatorKey = "Normal";

        [SerializeField]
        private PrizeJsonView mainPrizeView = null;

        [SerializeField]
        private CanvasGroup mainPrizeViewCanvasGroup = null;
        [SerializeField]
        private GameObject ballRoot = null;
        [SerializeField]
        private Animator ballAnimator = null;
        [SerializeField]
        private GameObject objectRoot = null;
        [SerializeField]
        private GameObject iconRoot = null;


        /// <summary>報酬アイコン</summary>
        public PrizeJsonView PrizeJsonView { get { return mainPrizeView; } }
        // 獲得演出を再生するか
        public bool IsPlayGetEffect => isPlayEffect;
        //開いているか
        public bool IsOpened => isOpened;

        PrizeJsonViewData mainViewData = null;

        bool isPlayEffect = false;
        long rarity = 0;
        bool isSkip = false;
        bool isOpened = false;
        CancellationTokenSource animationCancellationSource = null;

        Sequence iconChangeAnimationSequence = null;
        


        public void SetView(PrizeJsonViewData data, bool isFirstNew, bool isPassEffect)
        {
            mainViewData = data;
            isSkip = false;
            isOpened = false;
            UpdatePrizeView(mainPrizeView, mainViewData, isPassEffect);
            mainPrizeView.SetNewIconActive(isFirstNew);

            mainPrizeView.gameObject.SetActive(false);
            
            
            if( mainViewData.ItemIconType == ItemIconType.Character ) {
                var rarityId = RarityUtility.GetRarityId(mainViewData.Id, 0);
                rarity = MasterManager.Instance.rarityMaster.FindData(rarityId).value;
            }

            ballRoot.gameObject.SetActive(true);
            isPlayEffect = IsPlayEffect(mainViewData, isFirstNew);
        }

        /// <summary>
        /// カードオープンアニメーション
        /// </summary>
        /// <param name="getEffect"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask Open( CharacterGetEffect getEffect){
            
            if( isSkip ) {
                UpdateViewOpenFinishedIdle();
                await UniTask.Delay(1);
                return;
            }

            animationCancellationSource = new CancellationTokenSource();
            
            try{
                //キャラクターアニメーション
                if( isPlayEffect ) {
                    await getEffect.PlayAsync(mainViewData.Id);
                } 
                
                ballAnimator.SetTrigger(animatorKey);
                await AnimatorUtility.WaitStateAsync(ballAnimator, animatorKey, animationCancellationSource.Token);
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
        /// 演出スキップ
        /// </summary>
        public void Skip(){
            isSkip = true;
            if( animationCancellationSource != null ) {
                animationCancellationSource.Cancel();
            }
        }

        /// <summary>
        /// カードオープン終了待機状態にする
        /// </summary>
        void UpdateViewOpenFinishedIdle(){
            ballAnimator.enabled = false;
            foreach( Transform child in objectRoot.transform ){
                child.gameObject.SetActive(false);
            }
            iconRoot.SetActive(true);
            mainPrizeView.gameObject.SetActive(true);
            mainPrizeViewCanvasGroup.alpha = 1.0f;
            mainPrizeViewCanvasGroup.transform.localScale = Vector3.one;
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
        bool IsPlayEffect( PrizeJsonViewData viewData, bool isFirstNew ){
            
            if( isFirstNew ) {
                return true;
            }
            
            if( viewData.ItemIconType == ItemIconType.Character ) {
                var charaMaster = MasterManager.Instance.charaMaster.FindData(viewData.Id);
                if( charaMaster.cardType != CardType.Character ) {
                    return false;
                }
                return true;
            }

            return false;
        }

        void UpdatePrizeView( PrizeJsonView view, PrizeJsonViewData data ,  bool isPassEffect){
            view.SetView(data);
            view.SetCountTextColor(isPassEffect ? ColorValueAssetLoader.Instance["highlight.orange"] : ColorValueAssetLoader.Instance["white"]);
            if( view.IconType == ItemIconType.Character ) {
                var characterIcon = view.GetIcon<CharacterIcon>();
                characterIcon.SetActiveLv(false);
            }
        }
    }
}