using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Deck;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb
{ 
    public class ClubDeckSummaryScrollData
    {
        public ClubDeckSummaryScrollData(DeckData deckData, bool isSelecting, Action<long> onClick)
        {
            DeckData = deckData;
            IsSelecting = isSelecting;
            OnClick = onClick;
        }
        public readonly DeckData DeckData;
        public readonly bool IsSelecting;
        public readonly Action<long> OnClick;
    }
    
    public class ClubDeckSummaryScrollItem : ScrollGridItem
    {
        [SerializeField] private ClubDeckSummaryView clubDeckSummaryView;
        
        private ClubDeckSummaryScrollData clubDeckSummaryScrollData;

        protected override void OnSetView(object value)
        {
            clubDeckSummaryScrollData = (ClubDeckSummaryScrollData)value;
        }
        
        private void OnEnable()
        {
            if(clubDeckSummaryScrollData == null) return;
            clubDeckSummaryView.InitializeUI(clubDeckSummaryScrollData.DeckData, clubDeckSummaryScrollData.IsSelecting, clubDeckSummaryScrollData.OnClick);
        }
    }
}