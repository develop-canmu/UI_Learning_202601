using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;

namespace Pjfb.Character
{
    public enum CharacterPageType
    {
        CharacterTop,
        SuccessCharaTop,
        SuccessCharaList,
        SuccessCharaSell,
        SuccessCharaFavorite,
        BaseCharaTop,
        BaseCharaGrowthLiberationList,
        BaseCharaGrowthLiberation,
        BaseCharaList,
        SpecialSupportCardList,
        SpecialSupportCardLevelUp,
        FriendBorrowing,
        CombinationTop,
        CombinationMatch,
        CombinationTraining,
        CombinationCollection,
        SupportEquipmentList,
        SupportEquipmentSell,
        SupportEquipmentFavorite,
        DeckEnhance,
        AdviserGrowthLiberationList,
        AdviserGrowthLiberation,
        AdviserList,
    }
    
    public class CharacterPage : PageManager<CharacterPageType>, IFooterPage
    {
        public class Data
        {
            public CharacterPageType selectedPage = CharacterPageType.CharacterTop;
            
            private object selectPageArgs = null;
            /// <summary>ページを指定したときに使う引数</summary>
            public object SelectPageArgs{get{return selectPageArgs;}}

            public Data(CharacterPageType characterPageType, object args = null)
            {
                selectedPage = characterPageType;
                selectPageArgs = args;
            }
        }
        
        protected override string GetAddress(CharacterPageType page)
        {
            return $"Prefabs/UI/Page/Character/{page}Page.prefab";
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var data = (Data)args;
            
            if (TransitionType == PageTransitionType.Back)
            {
                if (CurrentPageObject is CharacterTopPage characterTopPage)
                {
                    characterTopPage.UpdateBadges();
                }
                return default;
            }

            ClearPageStack();
            // 他画面からサブページを指定して指定した画面から開けるようにする（現状は強化選手の強化/能力解放キャラ一覧のみなのでそれ以外はキャラTopを開くようにしておく）
            var selectedPage = data?.selectedPage ?? CharacterPageType.CharacterTop;
            return OpenPageAsync(selectedPage, true, data?.SelectPageArgs, token);
        }
        
        public void OnOpenPage()
        {
            // トップページでなければトップページに遷移
            if(CurrentPageType != CharacterPageType.CharacterTop)
            {
                ClearPageStack();
                OpenPage(CharacterPageType.CharacterTop, true, null);   
            }
        }
    }
}


