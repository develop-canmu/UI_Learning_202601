using System;
using System.Linq;
using Pjfb.Networking.App.Request;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using Logger = CruFramework.Logger;

namespace Pjfb.Training
{
    public static class TrainingManager
    {
        private static int autoTrainingCompleteCount = 0;
        #region PublicProperties
        public static string homeBadgeLabel = string.Empty;
        public static int AutoTrainingCompleteCount{get => autoTrainingCompleteCount;}
        #endregion

        #region StaticMethod
        /// <summary>
        /// ホーム画面にあるトレーニングボタンがクリックされた時に実行される
        /// </summary>
        public static void OnClickTrainingButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TrainingPreparation, true, null);
        }

        /// <summary>
        /// ホームのバッジ更新用
        /// </summary>
        /// <param name="statusList"></param>
        public static void UpdateAutoTrainingCount(TrainingAutoPendingStatus[] statusList)
        {
            autoTrainingCompleteCount = statusList.Count(IsCompleteAutoTraining);
        }
        
        /// <summary>
        /// 自動トレーニングが完了しているか
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool IsCompleteAutoTraining(TrainingAutoPendingStatus status)
        {
            // 自動トレーニングしていない
            if (status.mCharaId == 0)
            {
                return false;
            }

            DateTime finishAt = AppTime.Parse(status.finishAt);
            
            // トレーニングが終わっている
            if(AppTime.Now >= finishAt)
            {
                return true;
            }
          
            return false;
        }
        #endregion
    }
}
