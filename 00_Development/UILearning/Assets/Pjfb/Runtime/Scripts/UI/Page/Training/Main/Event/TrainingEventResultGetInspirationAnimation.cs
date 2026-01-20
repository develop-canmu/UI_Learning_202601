using System;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using System.Linq;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class TrainingEventResultGetInspirationAnimation : MonoBehaviour
    {
        [SerializeField]
        private ScrollGrid scrollGrid = null;
        [SerializeField]
        private ScrollGridController arrowButton = null;
        
        [SerializeField]
        private float scrollInterval = 1.0f;
        
        [SerializeField]
        private UIButton skipButton = null;
        
        [SerializeField]
        private Animator inspirationAnimator = null;
        /// <summary>アニメーター</summary>
        public Animator InspirationAnimator{get{return inspirationAnimator;}}

        // 演出タイプ
        private TrainingUtility.InspirationType effectType = TrainingUtility.InspirationType.Normal;
        public TrainingUtility.InspirationType EffectType => effectType;
        
        // スキップした
        private bool isSkiped = false;
        
        public void Begin()
        {
            isSkiped = false;
            skipButton.gameObject.SetActive(true);
            arrowButton.gameObject.SetActive(false);
            scrollGrid.Interactable = false;
        }

        public void End()
        {
            skipButton.gameObject.SetActive(false); 
            scrollGrid.Interactable = true;
        }
        
        public void SetItems(TrainingUtility.InspirationType effectType, TrainingInspirationCardList[] items)
        {
            this.effectType = effectType;
            
            // ソート
            TrainingInspirationCardList[] orderedItems = items.
                OrderBy(v=> MasterManager.Instance.trainingCardMaster.FindData(v.MTrainingCardId).grade).
                ThenBy(v=>MasterManager.Instance.trainingCardMaster.FindData(v.MTrainingCardId).practiceType).
                ToArray();

            scrollGrid.SetItems(orderedItems);
        }

        public async UniTask<bool> ScrollAnimationAsync()
        {
            isSkiped = false;
            // スクロールが終わるまでループ
            while(true)
            {
                TrainingGetInspirationCardScrollItem item = (TrainingGetInspirationCardScrollItem)scrollGrid.GetItem( scrollGrid.GetItems()[scrollGrid.CurrentPage ] );
                // 開くアニメーション
                await item.CardView.PlayOpenAnimation();
                // 一定時間待機
                float waitTime = scrollInterval;
                while(true)
                {
                    waitTime -= Time.deltaTime;
                    if(waitTime <= 0 || isSkiped)break;
                    await UniTask.DelayFrame(1);
                }
                // 終了通知
                item.EndOpen();
                // 最後のページ
                if(scrollGrid.CurrentPage + 1 == scrollGrid.PageCount)
                {
                    break;
                }

                // 次のページへ
                scrollGrid.SetPage(scrollGrid.CurrentPage + 1, false);
                isSkiped = false;
                await UniTask.DelayFrame(1);
            }
            
            arrowButton.gameObject.SetActive(true);
            arrowButton.UpdateButtons();
            
            return false;
        }
        
        private void Awake()
        {
            arrowButton.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnSkipButton()
        {
            isSkiped = true;
        }
    }
}