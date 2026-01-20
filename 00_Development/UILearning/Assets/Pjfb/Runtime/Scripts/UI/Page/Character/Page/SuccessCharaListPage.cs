using System.Linq;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class SuccessCharaListPage : CharacterVariableListBasePage
    {
        #region EventListeners

        protected override void OnSelectCharacter(object value)
        {
            if (value is not CharacterVariableScrollData data) return;

            SuccessCharaDetailModalWindow.Open(ModalType.SuccessCharaDetail,
                new SuccessCharaDetailModalParams(data.SwipeableParams, true));
        }


        public void OnClickSellButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SuccessCharaSell, true, null);
        }

        public void OnClickFavoriteButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SuccessCharaFavorite, true, null);
        }
        #endregion
    }
}
