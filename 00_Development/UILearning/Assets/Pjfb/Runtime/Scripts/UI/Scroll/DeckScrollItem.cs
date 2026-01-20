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
    public class DeckScrollData
    {
        public DeckData DeckData;
        public Action OnClickReset;
        public Action OnApplyDeckRecommendations;
        public Action OnStrategyChanged;
        public Action<int> OnClickEditPosition;
        public Action<int, int> OnClickChara;
        public Func<long, HashSet<long>> GetRecommendationExcludedIdList;
    }
    
    public class DeckScrollItem : ScrollGridItem
    {
        [SerializeField] private DeckView deckView;
        
        private List<CharacterScrollData> characterData;
        private DeckScrollData deckScrollData;
        
        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            TriggerEvent(characterData);
        }
        
        protected override void OnSetView(object value)
        {
            deckScrollData = (DeckScrollData)value;
            deckView.InitializeUI(deckScrollData.DeckData);
            deckView.GetRecommendationExcludedIdList = deckScrollData.GetRecommendationExcludedIdList;
            deckView.OnClickEditPosition = deckScrollData.OnClickEditPosition;
            deckView.OnClickReset = deckScrollData.OnClickReset;
            deckView.OnApplyDeckRecommendations = deckScrollData.OnApplyDeckRecommendations;
            deckView.OnClickChara = deckScrollData.OnClickChara;
            deckView.OnStrategyChanged = deckScrollData.OnStrategyChanged;
        }

        

    }
}