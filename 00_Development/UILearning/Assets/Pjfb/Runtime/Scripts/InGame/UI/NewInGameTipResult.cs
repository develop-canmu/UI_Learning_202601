using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using TMPro;
using UnityEngine;

namespace Pjfb.InGame
{
    public class NewInGameTipResult : MonoBehaviour
    {
        
        private const long HistoryType = 201;
        private static readonly string ConditionParam = "activityRank";
            
    
        private static readonly string OpenAnimation = "Open";
        private static readonly string CloseAnimation = "Close";
    
        [SerializeField]
        private Animator animator = null;
        
        [SerializeField]
        private TMP_Text resultBonusText = null;
        [SerializeField]
        private TMP_Text rankBonusText = null;
        [SerializeField]
        private TMP_Text totalText = null;
        
        private bool isOpened = false;
    
        public async UniTask PlayTipResultAnimationAsync(TrainingProgressAPIResponse response)
        {

            long rankBonusValue = 0;
            // 活躍度ボーナスの取得
            foreach(TrainingRewardAdditionHistory history in response.eventReward.rewardAdditionHistoryList)
            {
                if(history.historyType == HistoryType && history.conditionParam == ConditionParam)
                {
                    rankBonusValue = history.hp;
                    break;
                }
            }
        
        
            // ボーナス
            resultBonusText.text = (response.eventReward.hp - rankBonusValue).ToString();
            // 活躍ボーナス
            rankBonusText.text = rankBonusValue.ToString();
            // 獲得合計
            totalText.text = response.eventReward.hp.ToString();
        
            isOpened = true;
            await AnimatorUtility.WaitStateAsync(animator, OpenAnimation);
            
#if CRUFRAMEWORK_DEBUG && !PJFB_REL
            DebugAutoChoice();
#endif
            
            // 閉じるまで待つ
            await UniTask.WaitWhile(()=>isOpened);
        }

#if CRUFRAMEWORK_DEBUG && !PJFB_REL
        private void DebugAutoChoice()
        {
            if (TrainingChoiceDebugMenu.EnabledAutoChoiceAction)
            {
                OnClose();
            }
        }
#endif
        
        /// <summary>
        /// UGUI
        /// </summary>
        public void OnClose()
        {
            PlayCloseAnimation().Forget();
        }
        
        /// <summary>閉じるアニメーション</summary>
        private async UniTask PlayCloseAnimation()
        {
            await AnimatorUtility.WaitStateAsync(animator, CloseAnimation);
            isOpened = false;
        }
    }
}
