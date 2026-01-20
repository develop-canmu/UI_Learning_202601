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
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb.Rivalry
{
    public class RivalryTeamSelectItem : ListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public int mHuntEnemyId;
            public BigValue teamPower;
            public string teamName;
            public long teamRarity;
            public long[] mCharaNpcIdList;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text teamNameText;
        [SerializeField] private TMP_Text teamPowerText;
        [SerializeField] private DeckRankImage rankImage;
        [SerializeField] private DeckRarityImage rarityImage;
        [SerializeField] private ScrollGrid deckScrollGrid;
        #endregion

        public Data data;

        public override void Init(ItemParamsBase value)
        {
            data = (Data) value;
            teamNameText.text = data.teamName;
            teamPowerText.text = data.teamPower.ToString();
            
            var teamRank = StatusUtility.GetPartyRank(data.teamPower);
            rankImage.SetTextureAsync(teamRank).Forget();
            rarityImage.SetTexture(data.teamRarity);

            // キャラ一覧
            var npcList = new List<CharacterScrollData>();
            foreach (var mCharaId in data.mCharaNpcIdList)
            {
                var mCharaNpc = MasterManager.Instance.charaNpcMaster.FindData(mCharaId);
                var charaData = new CharacterScrollData(mCharaNpc.mCharaId, 1, 0, -1);
                npcList.Add(charaData);
            }
            deckScrollGrid.SetItems(npcList);
        }
    }
}