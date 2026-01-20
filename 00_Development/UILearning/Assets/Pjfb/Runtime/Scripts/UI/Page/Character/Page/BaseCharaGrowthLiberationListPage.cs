using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Character
{
    public class BaseCharaGrowthLiberationListPage : CharacterListBasePage
    {
        // 選択時に開くページタイプ
        protected virtual CharacterPageType selectedPageType => CharacterPageType.BaseCharaGrowthLiberation;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 戻ってきた時以外はスクロールアイテムの選択を解除
            if (TransitionType != PageTransitionType.Back) characterScroll.Scroll.DeselectAllItems();
            // 強化・解放が出来るキャラをリストにセット(バッジ表示に用いる)
            characterScroll.SetEnableGrowthLiberationCharacterIds(UserDataManager.Instance
                .GetUserDataCharaListByType(characterScroll.CardType).Where(data => data.IsPossibleGrowth() || data.IsPossibleLiberation())
                .Select(data => data.id).ToList());
            
            // ユーザの取得キャラクターをセット
            characterScroll.SetUserCharacterList();
            characterScroll.SetCharacterList();
            Refresh();
            return base.OnPreOpen(args, token);
        }

        protected override void OnSelectCharacter(object value)
        {
            CharacterScrollData scrollData = (CharacterScrollData)value;
            CharacterPage m = (CharacterPage)Manager;
            var userCharacterId = scrollData.UserCharacterId;
            var userCharacterIdList = characterScroll.ItemListSrc.Select(data => data.UserCharacterId).ToList();
            var currentIndex = userCharacterIdList.FindIndex(data => data == userCharacterId);
            // 選択した際のページを開く
            m.OpenPage(selectedPageType, true,
                new CharaLevelUpBasePage.Data(userCharacterId, currentIndex, userCharacterIdList, characterScroll.SelectItemByUCharaId));
        }
        
        protected override void OnSwipeDetailModal(CharacterScrollData scrollData)
        {
            characterScroll.SelectItem(scrollData);
        }
    }
}
