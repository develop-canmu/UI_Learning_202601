using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;
using TMPro;
using System;
using Pjfb.UI;

namespace Pjfb.Rivalry
{
    public enum RivalryDifficulty
    {
        Beginner=1,
        Intermediate=2,
        Advanced=3,
        Expert=4
    }

    public class RivalryRegularItem : ListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public RivalryDifficulty difficulty;
            public int mHuntTimetableId;
            public int teamPower;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image bannerImage;
        [SerializeField] private TMP_Text teamPowerText;
        #endregion

        public Data data;

        public override void Init(ItemParamsBase value)
        {
            data = (Data) value;
            teamPowerText.text = data.teamPower.ToString();
            string titleString = string.Empty;
            switch (data.difficulty)
            {
                case RivalryDifficulty.Beginner:
                default:
                    titleString = StringValueAssetLoader.Instance["rivalry.regular.rank1"];
                    break;
                case RivalryDifficulty.Intermediate:
                    titleString = StringValueAssetLoader.Instance["rivalry.regular.rank2"];
                    break;
                case RivalryDifficulty.Advanced:
                    titleString = StringValueAssetLoader.Instance["rivalry.regular.rank3"];
                    break;
                case RivalryDifficulty.Expert:
                    titleString = StringValueAssetLoader.Instance["rivalry.regular.rank4"];
                    break;
            }
            titleText.text = titleString;
            // ToDo: 画像ロード
        }
    }
}