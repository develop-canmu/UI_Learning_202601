using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Encyclopedia
{
    public class EncyclopediaListPage : Page
    {
        [SerializeField] private CharacterParentScroll characterParentScroll;
     
        private Dictionary<long, CharaParentData> charaParentDataDictionary;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (TransitionType == PageTransitionType.Back)
            {
                characterParentScroll.Scroll.RefreshItemView();
                return;
            }
            charaParentDataDictionary = EncyclopediaPage.CharaParentDataDictionary;
            InitializeScroller();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            if (TransitionType == PageTransitionType.Back) return;
            base.OnOpened(args);
           
        }

        private void OnSelectCharacterParent(object value)
        {
            var scrollData = (CharaParentData)value;
            if(scrollData.PossessionCount == 0) return;

            EncyclopediaPage m = (EncyclopediaPage)Manager;
            m.OpenPage(EncyclopediaPageType.EncyclopediaDetail, true, EncyclopediaPage.GetIndexByCharaParentId(scrollData.MCharaParent.parentMCharaId));
        }

        private void InitializeScroller()
        {
            characterParentScroll.SetItems(charaParentDataDictionary.Values.OrderBy(x => x.PossessionCount == 0).ThenBy(x => x.MCharaParent.sortNumber).ToList());

            characterParentScroll.OnSelectedItem -= OnSelectCharacterParent;
            characterParentScroll.OnSelectedItem += OnSelectCharacterParent;
        }

    }

}
