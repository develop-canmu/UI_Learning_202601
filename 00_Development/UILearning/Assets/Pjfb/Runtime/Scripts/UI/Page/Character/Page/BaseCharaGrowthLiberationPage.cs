using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class BaseCharaGrowthLiberationPage : CharaLevelUpBasePage
    {
        [SerializeField] private CharacterCardBackgroundImage characterCardBackgroundImage;
        [SerializeField] private BaseCharacterNameView nameView;
        [SerializeField] private CharacterStatusTypeView statusTypeView = null;
        [SerializeField] private CharacterStatusValuesView statusValuesView;
        [SerializeField] private CharacterAbilityTabSheetManager characterAbilityTabSheetManager;



        protected override async UniTask InitializePageAsync()
        {
            await base.InitializePageAsync();
            await characterAbilityTabSheetManager.OpenSheetAsync(CharacterAbilityTabSheetType.BaseStatus, null);
            await statusTypeView.SetCharacter(uChara.charaId);
            await characterCardBackgroundImage.SetTextureAsync(uChara.ParentMCharaId);
        }

        public override void OnClickDetailButton()
        {
            CharacterDetailModalBase.Open(ModalType.BaseCharacterDetail,
                new BaseCharaDetailModalParams(
                    new SwipeableParams<CharacterDetailData>(detailOrderList, data.CurrentIndex, SetCharacterByIndex), true));
        }

        protected override void OnModifyGrowthLevel(long lv)
        {
            base.OnModifyGrowthLevel(lv);
            statusValuesView.SetCharacter(uChara.charaId, lv, uChara.newLiberationLevel);
            statusValuesView.SetColor(uChara.charaId, uChara.level, lv, uChara.newLiberationLevel);
        }
        
        protected override void OnModifyLiberationLevel(long lv)
        {
            nameView.SetAfterRarity(uChara.id, lv);
        }
        
        protected override async UniTask InitializeNameViewAsync(UserDataChara chara)
        {
            await nameView.InitializeUIAsync(uChara);
        }

        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) {
                switch(type) {
                    case PageManager.MessageType.BeginFade:
                        if(uChara.CardType != CardType.SpecialSupportCharacter)
                        {
                            InitializeNameViewAsync(uChara).Forget();
                            OnModifyLiberationLevel(growthLiberationTabSheetManager.LiberationAfterLevel);
                        }
                        break;
                }
            }
            return base.OnMessage(value);
        }
    }
}


