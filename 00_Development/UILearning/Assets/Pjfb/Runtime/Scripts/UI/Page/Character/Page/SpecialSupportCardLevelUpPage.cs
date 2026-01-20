using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class SpecialSupportCardLevelUpPage : CharaLevelUpBasePage
    {
        [SerializeField] private SpecialSupportCardNameView nameView;
        public override void OnClickDetailButton()
        {
            CharacterDetailModalBase.Open(ModalType.SpecialSupportCardDetail,
                new BaseCharaDetailModalParams(
                    new SwipeableParams<CharacterDetailData>(detailOrderList, data.CurrentIndex, SetCharacterByIndex), false));
        }

        protected override async UniTask InitializeNameViewAsync(UserDataChara chara)
        {
            await nameView.InitializeUIAsync(uChara);
        }
    }
}


