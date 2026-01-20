#if CRUFRAMEWORK_DEBUG && !PJFB_REL

using System.Collections.Generic;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public static class TrainingChoiceDebugMenu
    {
        private const string MenuCategory = "Training Choice";

        public static DebugActionChoiceMode CurrentActionChoiceMode { get; private set; }

        /// <summary>
        /// 練習メニューを自動選択するかどうか
        /// </summary>
        public static bool EnabledAutoChoiceAction => CurrentActionChoiceMode != DebugActionChoiceMode.Manual;

        /// <summary>
        /// 試合時の「試合開始」「OK」などを自動選択するかどうか
        /// </summary>
        public static bool EnabledAutoChoiceGame { get; private set; }

        /// <summary>
        /// 目標達成時の「OK」「次へ」などを自動選択するかどうか
        /// </summary>
        public static bool EnabledAutoChoiceTarget { get; private set; }

        /// <summary>
        /// 通常プレイで自動選択できない場合も自動選択するかどうか（複数の選択肢がある場合など）
        /// </summary>
        public static bool EnabledAutoChoiceAdv { get; private set; }

        public static void AddOptions()
        {
            CruFramework.DebugMenu.AddOption(MenuCategory, "Auto Choice Action", () => CurrentActionChoiceMode, value => CurrentActionChoiceMode = value);
            CruFramework.DebugMenu.AddOption(MenuCategory, "Auto Choice Game", () => EnabledAutoChoiceGame, value => EnabledAutoChoiceGame = value);
            CruFramework.DebugMenu.AddOption(MenuCategory, "Auto Choice Target", () => EnabledAutoChoiceTarget, value => EnabledAutoChoiceTarget = value);
            CruFramework.DebugMenu.AddOption(MenuCategory, "Auto Choice Adv", () => EnabledAutoChoiceAdv, value => EnabledAutoChoiceAdv = value);
        }

        public static void RemoveOptions()
        {
            CruFramework.DebugMenu.RemoveOption(MenuCategory);
        }

        public static int GetMostGrowthRewardIndex(IReadOnlyList<TrainingCardReward> rewards)
        {
            BigValue mostGrowth = BigValue.Zero;
            var mostGrowthIndex = 0;
            for (var i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                var status = TrainingUtility.GetStatus(reward);
                var growth = status.TotalStatusExceptShootRange();
                if (mostGrowth < growth)
                {
                    mostGrowth = growth;
                    mostGrowthIndex = i;
                }
            }

            return mostGrowthIndex;
        }
        
        public static int GetLeastGrowthRewardIndex(IReadOnlyList<TrainingCardReward> rewards)
        {
            BigValue leastGrowth = BigValue.Zero;
            var leastGrowthIndex = 0;
            for (var i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                var status = TrainingUtility.GetStatus(reward);
                var growth = status.TotalStatusExceptShootRange();
                if (leastGrowth > growth)
                {
                    leastGrowth = growth;
                    leastGrowthIndex = i;
                }
            }

            return leastGrowthIndex;
        }
    }
}

#endif
