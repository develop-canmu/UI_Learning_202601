using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;
using TMPro;
using System;
using Pjfb.UI;
using Pjfb.Master;

namespace Pjfb.Rivalry
{
    public class RivalryEventItem : ListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public RivalryDifficulty difficulty;
            public int mHuntEnemyId;
            public int mHuntTimetableId;
            public int recPower;
            public string matchTitle;
            public PrizeJsonWrap prizeJsonArgs;
            public bool isNew;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text difficultyText;
        [SerializeField] private TMP_Text recPowerText;
        [SerializeField] private TMP_Text matchTitleText;
        [SerializeField] private TMP_Text rewardPointText;
        [SerializeField] private GameObject newBadge;
        [SerializeField] private PrizeJsonView itemIcon;
        #endregion

        public Data data;

        public override void Init(ItemParamsBase value)
        {
            data = (Data) value;
            string difficultyString = string.Empty;
            switch (data.difficulty)
            {
                case RivalryDifficulty.Beginner:
                default:
                    difficultyString = StringValueAssetLoader.Instance["rivalry.regular.rank1"];
                    break;
                case RivalryDifficulty.Intermediate:
                    difficultyString = StringValueAssetLoader.Instance["rivalry.regular.rank2"];
                    break;
                case RivalryDifficulty.Advanced:
                    difficultyString = StringValueAssetLoader.Instance["rivalry.regular.rank3"];
                    break;
                case RivalryDifficulty.Expert:
                    difficultyString = StringValueAssetLoader.Instance["rivalry.regular.rank4"];
                    break;
            }
            difficultyText.text = difficultyString;
            recPowerText.text = String.Format(StringValueAssetLoader.Instance["rivalry.event.recommended_power"], data.recPower);
            matchTitleText.text = data.matchTitle;
            rewardPointText.text = String.Format(StringValueAssetLoader.Instance["rivalry.event.reward_point"], data.prizeJsonArgs.args.value);
            newBadge?.SetActive(data.isNew);
            itemIcon.SetView(data.prizeJsonArgs);
        }
    }
}