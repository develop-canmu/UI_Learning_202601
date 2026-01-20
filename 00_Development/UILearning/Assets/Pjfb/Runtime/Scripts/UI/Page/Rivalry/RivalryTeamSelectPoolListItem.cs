using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.UI;
using Pjfb.Extensions;
using Pjfb.Master;

namespace Pjfb.Rivalry
{
    public class RivalryTeamSelectPoolListItem : PoolListItemBase
    {
        #region InnerClass
        public class ItemParams : ItemParamsBase
        {
            public HuntEnemyMasterObject huntEnemyMasterObject;
            public HuntDifficultyMasterObject huntDifficultyMasterObject;
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public Action<ItemParams> onClickItemParams;
            
            public ItemParams(HuntEnemyMasterObject huntEnemyMasterObject, HuntDifficultyMasterObject huntDifficultyMasterObject, HuntMasterObject huntMasterObject, HuntTimetableMasterObject huntTimetableMasterObject, Action<ItemParams> onClickItemParams)
            {
                this.huntEnemyMasterObject = huntEnemyMasterObject;
                this.huntDifficultyMasterObject = huntDifficultyMasterObject;
                this.huntMasterObject = huntMasterObject;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.onClickItemParams = onClickItemParams;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text powerText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private DeckRankImage rankImage;
        [SerializeField] private DeckRarityImage rarityImage;
        [SerializeField] private List<CharacterVariableIcon> charaIconList;
        #endregion

        private ItemParams _itemParams;

        public override void Init(ItemParamsBase value)
        {
            // ToDo: 画像ロード
            _itemParams = (ItemParams) value;
            nameText.text = _itemParams.huntEnemyMasterObject.name;
            BigValue totalCombatPower = BigValue.Zero;
            var mCharaNpcIdList = _itemParams.huntEnemyMasterObject.mCharaNpcIdList;
            for (var i = 0; i < charaIconList.Count; i++)
            {
                var charaIcon = charaIconList[i];
                if (i >= mCharaNpcIdList.Length) {
                    charaIcon.gameObject.SetActive(false);
                } else {
                    var npcData = MasterManager.Instance.charaNpcMaster.FindData(mCharaNpcIdList[i]);
                    if (npcData == null) charaIcon.gameObject.SetActive(false);
                    else {
                        var detailData = new CharacterVariableDetailData(npcData);
                        charaIcon.gameObject.SetActive(true);
                        charaIcon.SetIcon(detailData, (RoleNumber)_itemParams.huntEnemyMasterObject.roleNumberList[i]);
                        charaIcon.SetIconTextureWithEffectAsync(detailData.MCharaId).Forget();
                        totalCombatPower += detailData.CombatPower;
                    }
                }
            }
            powerText.text = totalCombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            var teamRank = MasterManager.Instance.charaRankMaster.FindDataByTypeAndPower(CharaRankMasterStatusType.PartyTotal, totalCombatPower);
            rankImage.SetTexture(teamRank.rankNumber);
            rarityImage.SetTexture(_itemParams.huntEnemyMasterObject.rarity);
        }

        #region EventListener
        public void OnClickListItem()
        {
            _itemParams?.onClickItemParams?.Invoke(_itemParams);
        }
        #endregion
    }
}