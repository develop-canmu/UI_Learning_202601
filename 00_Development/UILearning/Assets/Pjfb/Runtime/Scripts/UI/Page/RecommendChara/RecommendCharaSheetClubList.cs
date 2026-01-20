using System.Collections.Generic;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb.RecommendChara
{
    public class RecommendCharaSheetClubList : Sheet
    {
        [SerializeField] private RecommendCharaScroll scroll;

        private IReadOnlyList<RecommendCharaData> dataDictionary;

        protected override async UniTask OnOpen(object args)
        {
            dataDictionary = RecommendCharaPage.ClubDataList;
            InitializeScroller();
            await base.OnPreOpen(args);
        }

        private void InitializeScroller()
        {
            scroll.SetItems(dataDictionary);

            scroll.OnSelectedItem -= OnSelectItem;
            scroll.OnSelectedItem += OnSelectItem;
        }

        /// <summary>
        /// UGUI
        /// </summary>
        private void OnSelectItem(object value)
        {
            var scrollData = (RecommendCharaData)value;
            if (scrollData is null) return;

            var windowModalParams = new SuccessCharaDetailModalParams(scrollData.SwipeableParams, false, "character.success_character_detail");
            SuccessCharaDetailModalWindow.Open(ModalType.SuccessCharaDetail, windowModalParams);
        }
    }
}
