using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.Training
{
    public class TrainingGetInspirationCardScrollItem : ScrollGridItem
    {
        
        [SerializeField]
        private TrainingGetInspirationCardView cardView = null;
        /// <summary>View</summary>
        public TrainingGetInspirationCardView CardView{get{return cardView;}}
        
        private bool isEndOpen = false;

        protected override void OnSetView(object value)
        {
            TrainingInspirationCardList args = (TrainingInspirationCardList)value;
            // データセット
            cardView.SetData(args);
        }
        
        public void EndOpen()
        {
            isEndOpen = true;
        }
        
        private void OnEnable()
        {
            if(isEndOpen)
            {
                cardView.PlayIdleAnimation().Forget();
            }
        }
    }
}