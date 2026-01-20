namespace Pjfb.Training
{
    /// <summary>
    /// トレーニングの各PageでArgumentsを受け渡すにあたり、常に保持しておきたい値
    /// トレーニングを中断→再開した際は初期化される
    /// </summary>
    public class TrainingMainArgumentsKeeps
    {
        /// <summary>
        /// 権利獲得演出ページを開けるか否か
        /// トレーニング再開時は演出を入れたくないためフラグで制御
        /// </summary>
        public bool IsLockOpenExtraTurnRightFirstPage { get; set; } = true;
        /// <summary>
        /// 権利確定・延長継続演出ページを開けるか否か
        /// トレーニング再開時は演出を入れたくないためフラグで制御
        /// </summary>
        public bool IsLockOpenExtraTurnLotteryPage { get; set; } = true;
        
        /// <summary>
        /// 最後に演出を再生したフェーズ（NextGoalIndex）
        /// </summary>
        public long LatestShowGetExtraTurnRightFirstEffectGoalIndex { get; set; } = -1;
        /// <summary>
        /// 最後に権利確定演出を再生したフェーズ（NextGoalIndex）
        /// </summary>
        public long LatestShowFirstExtraTurnEffectGoalIndex { get; set; } = -1;
        /// <summary>
        /// 最後に延長継続演出を再生したフェーズ（NextGoalIndex）
        /// </summary>
        public long LatestShowContinueExtraTurnEffectGoalIndex { get; set; } = -1;
        
        /// <summary>
        /// 現行目標の進行中に権利獲得演出を表示したことがあるか（イベント報酬による獲得演出は対象外）
        /// </summary>
        public bool IsShownGetExtraTurnRightFirstEffect { get; set; }
        /// <summary>
        /// 現行目標の進行中に権利確定演出を再生したことがあるか
        /// </summary>
        public bool IsShownFirstExtraTurnEffect { get; set; }
        /// <summary>
        /// 現行目標の進行中に延長継続演出を再生したことがあるか
        /// </summary>
        public bool IsShownContinueExtraTurnEffect { get; set; }
        
        public void UpdateExtraTurnFlags(long nextGoalIndex)
        {
            if (LatestShowGetExtraTurnRightFirstEffectGoalIndex < nextGoalIndex)
            {
                IsShownGetExtraTurnRightFirstEffect = false;
            }
            if (LatestShowFirstExtraTurnEffectGoalIndex < nextGoalIndex)
            {
                IsShownFirstExtraTurnEffect = false;
            }
            if (LatestShowContinueExtraTurnEffectGoalIndex < nextGoalIndex)
            {
                IsShownContinueExtraTurnEffect = false;
            }
        }
    }
}
