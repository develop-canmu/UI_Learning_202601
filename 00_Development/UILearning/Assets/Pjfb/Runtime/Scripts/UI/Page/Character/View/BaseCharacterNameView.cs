using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class BaseCharacterNameView : CharacterNameViewBase
    {
        private static readonly string LvStringValueKey = "character.status.lv_value_max_lv";
        [SerializeField] private CharacterIcon characterIcon;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private CharacterRarity rarityIcon;

        private long prevNextRarity;
        
        public void InitializeById(long id)
        {
            InitializeByIdAsync(id).Forget();
        }
        
        public async UniTask InitializeByIdAsync(long id)
        {
            await InitializeUIAsync(UserDataManager.Instance.chara.Find(id));
        }

        public void InitializeUI(UserDataChara chara)
        {
            InitializeUi(chara.MChara, chara.level, chara.newLiberationLevel, chara.GetMaxGrowthLevel());
        }
        
        public async UniTask InitializeUIAsync(UserDataChara chara)
        {
            await InitializeUiAsync(chara.MChara, chara.level, chara.newLiberationLevel, chara.GetMaxGrowthLevel());
        }

        
        public void InitializeUI(Pjfb.CharacterDetailData chara)
        {
            InitializeUi(chara.MChara, chara.Lv, chara.LiberationLevel, chara.GetMaxGrowthLevel());
        }

        public void InitializeUi(CharaMasterObject mChara, long lv, long liberationLv, long maxGrowthLevel)
        {
            InitializeUiAsync(mChara, lv, liberationLv, maxGrowthLevel).Forget();
        }
        
        public async UniTask InitializeUiAsync(CharaMasterObject mChara, long lv, long liberationLv, long maxGrowthLevel)
        {
            prevNextRarity = 0;
            await InitializeUIByMCharaAsync(mChara);
            if(levelText != null)
                levelText.text = string.Format(StringValueAssetLoader.Instance[LvStringValueKey], lv, maxGrowthLevel);
            if (rarityIcon != null)
            {
                rarityIcon.SetRarity(RarityUtility.GetRarity(mChara.id, liberationLv), mChara.Rarity);
            }

            if (characterIcon != null)
            {
                characterIcon.SetIcon(mChara.id, lv, liberationLv);
                characterIcon.SwipeableParams = null;
            }
        }
        
        public void SetAfterRarity(long id, long lv)
        {
            var uChara = UserDataManager.Instance.chara.Find(id);
            var currentRarity = RarityUtility.GetRarity(uChara.charaId, uChara.newLiberationLevel);
            var nextRarity = RarityUtility.GetRarity(uChara.charaId, lv);
      
            if (currentRarity == nextRarity)
            {
                prevNextRarity = 0;
                rarityIcon.SetRarityAndFlash(nextRarity, currentRarity, uChara.MChara.Rarity);
                return;
            }
            if (prevNextRarity == nextRarity) return;
            prevNextRarity = nextRarity;
            rarityIcon.SetRarityAndFlash(nextRarity, currentRarity, uChara.MChara.Rarity);
        }

    }

}
