using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public class SpecialSupportCardNameView : CharacterNameViewBase
    {
        private static readonly string LvStringValueKey = "character.status.lv_value_max_lv";
        [SerializeField] private SpecialSupportCardIcon specialSupportCardIcon;
        [SerializeField] private TextMeshProUGUI levelText;
        
        
        
        public void InitializeById(long id)
        {
            InitializeUI(UserDataManager.Instance.chara.Find(id));
        }
        
        
        public void InitializeUI(UserDataChara chara)
        {
            InitializeUI(chara.MChara, chara.level, chara.newLiberationLevel, chara.GetMaxGrowthLevel());
        }
        
        public async UniTask InitializeUIAsync(UserDataChara chara)
        {
            await InitializeUIAsync(chara.MChara, chara.level, chara.newLiberationLevel, chara.GetMaxGrowthLevel());
        }
        
        public void InitializeUI(CharaMasterObject mChara, long level, long liberationLevel, long maxGrowthLevel)
        {
            InitializeUIAsync(mChara, level, liberationLevel, maxGrowthLevel).Forget();
        }
        
        public async UniTask InitializeUIAsync(CharaMasterObject mChara, long level, long liberationLevel, long maxGrowthLevel)
        {
            await specialSupportCardIcon.SetIconAsync(mChara.id, level, liberationLevel);
            await InitializeUIByMCharaAsync(mChara);
            if(levelText != null)
                levelText.text = string.Format(StringValueAssetLoader.Instance[LvStringValueKey], level, maxGrowthLevel);
            specialSupportCardIcon.SwipeableParams = null;
        }
    }

}
