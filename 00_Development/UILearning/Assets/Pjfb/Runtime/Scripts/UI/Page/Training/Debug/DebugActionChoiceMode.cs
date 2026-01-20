#if CRUFRAMEWORK_DEBUG && !PJFB_REL

namespace Pjfb.Training
{
    /// <summary>
    /// トレーニングメニューの選択モード
    /// </summary>
    public enum DebugActionChoiceMode
    {
        /// <summary>
        /// 手動操作（=デバッグ機能OFF）
        /// </summary>
        Manual,
        /// <summary>
        /// 最も能力値が上がる練習を行う（休息・チップ稼ぎも適度に行う）
        /// </summary>
        AutoMostGrowth,
        /// <summary>
        /// 最も能力値が上がりにくい練習を行う（休息・チップ稼ぎは行わない。意図的に失格させたい場合を想定）
        /// </summary>
        AutoLeastGrowth,
    }
}

#endif
