using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;


namespace Pjfb.Training
{
    public class TrainingResultFestivalPointsView : MonoBehaviour
    {
        
        private const float TotalPointAnimationWaitDuration = 0.5f;
        private const float TotalPointAnimationDuration = 1.0f;

        private const int RankPointType = 1;
        private const int WinPointType  = 2;
        
        [SerializeField]
        private TMPro.TMP_Text rankPointText = null;
        [SerializeField]
        private TMPro.TMP_Text winPointText = null;
        [SerializeField]
        private TMPro.TMP_Text totalRankPointText = null;
        
        [SerializeField]
        private TMPro.TMP_Text boostBonusText = null;
        [SerializeField]
        private TMPro.TMP_Text eventBonusText = null;
        [SerializeField]
        private TMPro.TMP_Text totalBonusText = null;
        
        [SerializeField]
        private TMPro.TMP_Text totalPointText = null;
        
        
        private long beforePoints = 0;
        private long afterPoints = 0;
        
        public void Set(TrainingMainArguments args)
        {
            if(args.FestivalPointProgress == null)return;
            // ランクポイント
            SetRankPoint(args.FestivalPointProgress);
            // ボーナス
            SetBonusPoint(args.FestivalPointProgress);

            // 現状のポイント
            beforePoints = 0;
            afterPoints = args.FestivalPointProgress.valueDelta;
            totalPointText.text = beforePoints.ToString();
        }
        
        
        public async UniTask PlayPointAnimation()
        {
            // アニメーション開始までちょっと待つ
            await UniTask.Delay( (int)(TotalPointAnimationWaitDuration * 1000.0f) );
            
            // 時間
            float time = TotalPointAnimationDuration;
            
            while(true)
            {
                // ポイント計算
                long p = beforePoints + (long)((afterPoints - beforePoints) * (1.0f - time / TotalPointAnimationDuration));
                // 表示
                totalPointText.text = p.ToString();
                // 時間計測
                time -= Time.deltaTime;
                // 終了
                if(time <= 0)break;
                // 1フレ
                await UniTask.DelayFrame(1);
            }
            
            // 最終結果を表示
            totalPointText.text = afterPoints.ToString();
        }
        
        /// <summary>ランクポイント</summary>
        private void SetRankPoint(FestivalPointProgress festivalPointProgress)
        {
            long rankPoint = 0;
            long winPoint = 0;
            
            foreach(FestivalOriginalPoint point in festivalPointProgress.valueDeltaOriginalPointList)
            {
                switch(point.factor)
                {
                    case RankPointType:
                        rankPoint += point.value;
                        break;
                    case WinPointType:
                        winPoint += point.value;
                        break;
                }
            }
            
            rankPointText.text = rankPoint.ToString();
            winPointText.text = winPoint.ToString();
            totalRankPointText.text = (rankPoint + winPoint).ToString();
        }
        
        /// <summary>ボーナスポイント</summary>
        private void SetBonusPoint(FestivalPointProgress festivalPointProgress)
        {
            long boostBonus = 0;
            long eventBonus = festivalPointProgress.effectStatusBonusRate;
            
            
            foreach(FestivalSpecificCharaBonusRate bonus in festivalPointProgress.specificCharaBonusRateList)
            {
                boostBonus += bonus.bonusRate;
            }
            
            boostBonusText.text = string.Format(StringValueAssetLoader.Instance["event.bonus.value"], boostBonus / 100);
            eventBonusText.text = string.Format(StringValueAssetLoader.Instance["event.bonus.value"], eventBonus / 100);
            totalBonusText.text = string.Format(StringValueAssetLoader.Instance["event.bonus.value"], (boostBonus + eventBonus) / 100);
        }
    }
}
